using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Camber.Utilities.GeometryConversions;
using DynamoServices;
using AcAttDef = Autodesk.AutoCAD.DatabaseServices.AttributeDefinition;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acGeom = Autodesk.AutoCAD.Geometry;

namespace Camber.AutoCAD.Objects
{
    [RegisterForTrace]
    public class AttributeDefinition : Object
    {
        #region properties
        internal AcAttDef AcAttDef => AcObject as AcAttDef;

        /// <summary>
        /// Gets whether an Attribute Definition is set to be constant or not.
        /// </summary>
        public bool Constant => GetBool();

        /// <summary>
        /// Gets an Attribute Definition's field length value.
        /// </summary>
        public int FieldLength => GetInt();

        /// <summary>
        /// Gets whether an Attribute Definition is set to be visible or invisible.
        /// </summary>
        public bool Invisible => GetBool();

        /// <summary>
        /// Gets whether an Attribute Definition is multi-line.
        /// </summary>
        public bool MultiLine => GetBool("IsMTextAttributeDefinition");

        /// <summary>
        /// Gets whether an Attribute Definition's position is locked or movable.
        /// </summary>
        public bool LockPosition => GetBool("LockPositionInBlock");

        /// <summary>
        /// Gets whether an Attribute Definition is set to be preset.
        /// </summary>
        public bool Preset => GetBool();

        /// <summary>
        /// Gets the prompt for an Attribute Definition.
        /// </summary>
        public string Prompt => GetString();

        /// <summary>
        /// Gets the tag of an Attribute Definition.
        /// </summary>
        public string Tag => GetString();

        /// <summary>
        /// Gets the value of an Attribute Definition.
        /// </summary>
        public string Value => GetString("TextString");

        /// <summary>
        /// Gets whether the Attribute Definition is set to be verifiable.
        /// </summary>
        public bool Verifiable => GetBool();
        #endregion

        #region constructors
        [SupressImportIntoVM]
        internal static AttributeDefinition GetByObjectId(acDb.ObjectId attDefId)
            => ObjectSupport.Get<AttributeDefinition, AcAttDef>
            (attDefId, (attDef) => new AttributeDefinition(attDef));

        internal AttributeDefinition(AcAttDef AcAttDef, bool isDynamoOwned = false) : base(AcAttDef, isDynamoOwned) { }

        /// <summary>
        /// Creates a Block Attribute Definition.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="position"></param>
        /// <param name="value"></param>
        /// <param name="tag"></param>
        /// <param name="prompt"></param>
        /// <returns></returns>
        public static AttributeDefinition ByBlock(
            acDynNodes.Block block,
            Point position,
            string value,
            string tag,
            string prompt,
            bool multiLine,
            bool constant = false,
            bool visible = true,
            bool lockPosition = false,
            bool preset = false,
            bool verifiable = false)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;

            acGeom.Point3d insertionPoint = (acGeom.Point3d)GeometryConversions.DynPointToAcPoint(position, true);

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                acDb.Transaction t = ctx.GetTransaction(document.AcDocument);
                acDb.BlockTable bt = (acDb.BlockTable)t.GetObject(ctx.Database.BlockTableId, acDb.OpenMode.ForRead);
                acDb.BlockTableRecord btr = (acDb.BlockTableRecord)t.GetObject(bt[block.Name], acDb.OpenMode.ForWrite);

                acDb.ObjectId attDefId = acDynApp.ElementBinder.GetObjectIdFromTrace(ctx.Database);

                if (attDefId.IsValid && !attDefId.IsErased)
                {
                    AcAttDef acAttDef = (AcAttDef)attDefId.GetObject(acDb.OpenMode.ForWrite);
                    if (acAttDef != null)
                    {
                        acAttDef.Position = insertionPoint;
                        acAttDef.TextString = value;
                        acAttDef.Tag = tag;
                        acAttDef.Prompt = prompt;
                        acAttDef.Constant = constant;
                        acAttDef.Invisible = !visible;
                        acAttDef.IsMTextAttributeDefinition = multiLine;
                        acAttDef.LockPositionInBlock = lockPosition;
                        acAttDef.Preset = preset;
                        acAttDef.Verifiable = verifiable;
                    }
                }
                else
                {
                    // Create new Attribute Definition
                    AcAttDef attDef = new AcAttDef();
                    attDef.TextString = value;
                    attDef.Tag = tag;
                    attDef.Prompt = prompt;
                    attDef.Constant = constant;
                    attDef.Invisible = !visible;
                    attDef.IsMTextAttributeDefinition = multiLine;
                    attDef.LockPositionInBlock = lockPosition;
                    attDef.Preset = preset;
                    attDef.Verifiable = verifiable;
                    btr.AppendEntity(attDef);
                    t.AddNewlyCreatedDBObject(attDef, true);
                    attDefId = attDef.ObjectId;
                }

                AcAttDef createdAttDef = (AcAttDef)attDefId.GetObject(acDb.OpenMode.ForRead);
                if (createdAttDef != null)
                {
                    return new AttributeDefinition(createdAttDef, true);
                }
                return null;
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"AttributeDefinition(Tag = {Tag}, Value = {Value}";

        /// <summary>
        /// Sets whether an Attribute Definition is constant or not.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public AttributeDefinition SetConstant(bool @bool)
        {
            SetValue(@bool);
            return this;
        }

        /// <summary>
        /// Sets an Attribute Definition's field length value.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public AttributeDefinition SetFieldLength(int length)
        {
            SetValue(length);
            return this;
        }

        /// <summary>
        /// Sets whether an Attribute Definition is visible or invisible.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public AttributeDefinition SetInvisible(bool @bool)
        {
            SetValue(@bool);
            return this;
        }

        /// <summary>
        /// Sets whether an Attribute Definition is multi-line.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public AttributeDefinition SetMultiLine(bool @bool)
        {
            SetValue(@bool, "IsMTextAttributeDefinition");
            return this;
        }

        /// <summary>
        /// Sets whether an Attribute Definition's position is locked or movable.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public AttributeDefinition SetLockPosition(bool @bool)
        {
            SetValue(@bool, "LockPositionInBlock");
            return this;
        }

        /// <summary>
        /// Sets whether an Attribute Definition is preset.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public AttributeDefinition SetPreset(bool @bool)
        {
            SetValue(@bool);
            return this;
        }

        /// <summary>
        /// Sets the prompt for an Attribute Definition.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public AttributeDefinition SetPrompt(string value)
        {
            SetValue(value);
            return this;
        }

        /// <summary>
        /// Sets the tag of an Attribute Definition.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public AttributeDefinition SetTag(string value)
        {
            SetValue(value);
            return this;
        }

        /// <summary>
        /// Sets whether an Attribute Definition is verifiable.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public AttributeDefinition SetVerifiable(bool @bool)
        {
            SetValue(@bool);
            return this;
        }
        #endregion
    }
}
