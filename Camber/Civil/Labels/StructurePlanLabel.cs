#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acGeom = Autodesk.AutoCAD.Geometry;
using civApp = Autodesk.Civil.ApplicationServices;
using AeccStructureLabel = Autodesk.Civil.DatabaseServices.StructureLabel;
using Autodesk.DesignScript.Runtime;
using DynamoServices;
using Camber.Civil.PipeNetworks.Parts;
using Camber.Civil.Styles.Labels;
using Camber.Utils;
using Camber.Civil.CivilObjects;
#endregion

namespace Camber.Civil.Labels
{
    [RegisterForTrace]
    public sealed class StructurePlanLabel : Label
    {
        #region properties
        internal AeccStructureLabel AeccStructureLabel => AcObject as AeccStructureLabel;

        /// <summary>
        /// Gets the Structure that a Structure Plan Label is associated with.
        /// </summary>
        public Structure Structure => Structure.GetByObjectId(AeccStructureLabel.FeatureId);
        #endregion

        #region constructors
        internal StructurePlanLabel(
            AeccStructureLabel AeccStructureLabel, 
            bool isDynamoOwned = false) 
            : base(AeccStructureLabel, isDynamoOwned) { }

        [SupressImportIntoVM]
        internal static StructurePlanLabel GetByObjectId(acDb.ObjectId labelId)
            => CivilObjectSupport.Get<StructurePlanLabel, AeccStructureLabel>
            (labelId, (label) => new StructurePlanLabel(label));

        /// <summary>
        /// Creates a Structure Plan Label by Structure.
        /// </summary>
        /// <param name="structure"></param>
        /// <param name="labelStyle"></param>
        /// <returns></returns>
        public static StructurePlanLabel ByStructure(Structure structure, StructureLabelStyle labelStyle)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            var insertionPoint = (acGeom.Point3d)GeometryConversions.DynPointToAcPoint(structure.Location, true);

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                civApp.CivilDocument cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                acDb.ObjectId labelId = acDynApp.ElementBinder.GetObjectIdFromTrace(ctx.Database);

                if (labelId.IsValid && !labelId.IsErased)
                {
                    AeccStructureLabel aeccLabel = (AeccStructureLabel)labelId.GetObject(acDb.OpenMode.ForWrite);
                    if (aeccLabel != null)
                    {
                        // If the structure object ID has changed, erase the old label and create a new one
                        if (structure.InternalObjectId != aeccLabel.FeatureId)
                        {
                            aeccLabel.Erase();
                            labelId = AeccStructureLabel.Create(structure.InternalObjectId, labelStyle.InternalObjectId, insertionPoint);
                        }
                        else
                        {
                            // Update label style
                            aeccLabel.StyleId = labelStyle.InternalObjectId;
                        }
                    }
                }
                else
                {
                    // Create new label
                    labelId = AeccStructureLabel.Create(
                        structure.InternalObjectId, 
                        labelStyle.InternalObjectId, 
                        insertionPoint);
                }

                var createdLabel = labelId.GetObject(acDb.OpenMode.ForRead) as AeccStructureLabel;
                if (createdLabel != null)
                {
                    return new StructurePlanLabel(createdLabel, true);
                }
                return null;
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"StructurePlanLabel";
        #endregion
    }
}
