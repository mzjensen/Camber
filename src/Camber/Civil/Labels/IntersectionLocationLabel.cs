#region
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civApp = Autodesk.Civil.ApplicationServices;
using AeccIntxLocationLabel = Autodesk.Civil.DatabaseServices.IntersectionLocationLabel;
using DynamoServices;
using Camber.Civil.Styles.Labels;
using Camber.Civil.Intersections;
#endregion

namespace Camber.Civil.Labels
{
    [RegisterForTrace]
    public sealed class IntersectionLocationLabel : Label
    {
        #region properties
        internal AeccIntxLocationLabel AeccIntxLocationLabel => AcObject as AeccIntxLocationLabel;

        /// <summary>
        /// Gets the Intersection that an Intersection Location Label is associated with.
        /// </summary>
        public Intersection Intersection => Intersection.GetByObjectId(AeccIntxLocationLabel.FeatureId);
        #endregion

        #region constructors
        internal IntersectionLocationLabel(
            AeccIntxLocationLabel AeccIntxLocationLabel, 
            bool isDynamoOwned = false) 
            : base(AeccIntxLocationLabel, isDynamoOwned)
        { }

        /// <summary>
        /// Creates an Intersection Location Label by Intersection.
        /// </summary>
        /// <param name="intersection"></param>
        /// <param name="labelStyle"></param>
        /// <returns></returns>
        public static IntersectionLocationLabel ByIntersection(Intersection intersection, IntersectionLocationLabelStyle labelStyle)
        {

            acDynNodes.Document document = acDynNodes.Document.Current;

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                civApp.CivilDocument cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                acDb.ObjectId labelId = acDynApp.ElementBinder.GetObjectIdFromTrace(ctx.Database);

                if (labelId.IsValid && !labelId.IsErased)
                {
                    AeccIntxLocationLabel aeccLabel = (AeccIntxLocationLabel)labelId.GetObject(acDb.OpenMode.ForWrite);
                    if (aeccLabel != null)
                    {
                        // Update label style
                        aeccLabel.StyleId = labelStyle.InternalObjectId;
                    }
                }
                else
                {
                    // Create new label
                    labelId = AeccIntxLocationLabel.Create(intersection.InternalObjectId, labelStyle.InternalObjectId);
                }

                var createdLabel = labelId.GetObject(acDb.OpenMode.ForRead) as AeccIntxLocationLabel;
                if (createdLabel != null)
                {
                    return new IntersectionLocationLabel(createdLabel, true);
                }
                return null;
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"IntersectionLocationLabel(Intersection = {Intersection.Name})";
        #endregion
    }
}
