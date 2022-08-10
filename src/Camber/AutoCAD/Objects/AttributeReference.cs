using Autodesk.DesignScript.Runtime;
using AcAttRef = Autodesk.AutoCAD.DatabaseServices.AttributeReference;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;

namespace Camber.AutoCAD.Objects
{
    public class AttributeReference : Object
    {
        #region properties
        internal AcAttRef AcAttRef => AcObject as AcAttRef;

        /// <summary>
        /// Gets the visibility of an Attribute Reference.
        /// </summary>
        public bool Invisible => GetBool();

        /// <summary>
        /// Gets if an Attribute Reference is set to be constant.
        /// </summary>
        public bool IsConstant => GetBool();

        /// <summary>
        /// Gets if an Attribute Reference is a multi-line attribute.
        /// </summary>
        public bool IsMTextAttribute => GetBool();

        /// <summary>
        /// Gets if an Attribute Reference has a preset value and will not prompt for user input.
        /// </summary>
        public bool IsPreset => GetBool();

        /// <summary>
        /// Gets if the an Attribute Reference is set to verify user input.
        /// </summary>
        public bool IsVerifiable => GetBool();

        /// <summary>
        /// Gets if an Attribute Reference is set to be immovable relative to the geometry in the block.
        /// </summary>
        public bool LockPositionInBlock => GetBool();

        /// <summary>
        /// Gets the tag of an Attribute Reference.
        ///  This is the identifier you see if you explode a Block Reference that owns the attribute,
        ///  so that the attribute reverts back to the Attribute Definition that was part of the original reference’s block definition.
        /// </summary>
        public string Tag => GetString();
        #endregion

        #region constructors
        [SupressImportIntoVM]
        internal static AttributeReference GetByObjectId(acDb.ObjectId attRefId)
            => ObjectSupport.Get<AttributeReference, AcAttRef>
            (attRefId, (attRef) => new AttributeReference(attRef));

        internal AttributeReference(AcAttRef AcAttRef, bool isDynamoOwned = false) : base(AcAttRef, isDynamoOwned) { }
        #endregion

        #region methods
        public override string ToString() => $"{nameof(AttributeReference)}(Tag = {Tag})";

        /// <summary>
        /// Sets the visibility of an Attribute Reference.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public AttributeReference SetInvisible(bool @bool)
        {
            SetValue(@bool);
            return this;
        }

        /// <summary>
        /// Sets the tag of an Attribute Reference.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public AttributeReference SetTag(string value)
        {
            SetValue(value);
            return this;
        }
        #endregion
    }
}
