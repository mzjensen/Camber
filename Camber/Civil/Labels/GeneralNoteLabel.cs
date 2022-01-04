#region
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acGeom = Autodesk.AutoCAD.Geometry;
using civApp = Autodesk.Civil.ApplicationServices;
using Autodesk.DesignScript.Geometry;
using AeccGeneralNoteLabel = Autodesk.Civil.DatabaseServices.NoteLabel;
using DynamoServices;
using Camber.Civil.Styles.Labels.General;
using Camber.Civil.Styles.Objects;
#endregion

namespace Camber.Civil.Labels
{
    [RegisterForTrace]
    public sealed class GeneralNoteLabel : Label
    {
        #region properties
        internal AeccGeneralNoteLabel AeccGeneralNoteLabel => AcObject as AeccGeneralNoteLabel;
        #endregion

        #region constructors
        internal GeneralNoteLabel(
            AeccGeneralNoteLabel AeccGeneralNoteLabel, 
            bool isDynamoOwned = false) 
            : base(AeccGeneralNoteLabel, isDynamoOwned) { }

        /// <summary>
        /// Creates a General Note Label by point.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="labelStyle"></param>
        /// <param name="markerStyle"></param>
        /// <returns></returns>
        public static GeneralNoteLabel ByPoint(
            Point point, 
            GeneralNoteLabelStyle labelStyle, 
            MarkerStyle markerStyle)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                civApp.CivilDocument cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                acDb.ObjectId labelId = acDynApp.ElementBinder.GetObjectIdFromTrace(ctx.Database);

                if (labelId.IsValid && !labelId.IsErased)
                {
                    AeccGeneralNoteLabel aeccLabel = (AeccGeneralNoteLabel)labelId.GetObject(acDb.OpenMode.ForWrite);
                    if (aeccLabel != null)
                    {
                        // Update the label's location
                        acGeom.Point3d oldLocation = aeccLabel.AnchorInfo.Location;
                        acGeom.Point3d newLocation = new acGeom.Point3d(point.X, point.Y, point.Z);
                        acGeom.Vector3d acVector = oldLocation.GetVectorTo(newLocation);
                        aeccLabel.TransformBy(acGeom.Matrix3d.Displacement(acVector));

                        // Update label style
                        aeccLabel.StyleId = labelStyle.InternalObjectId;

                        // Update marker style
                        aeccLabel.AnchorMarkerStyleId = markerStyle.InternalObjectId;
                    }
                }
                else
                {
                    // Create new label
                    acGeom.Point3d location = new acGeom.Point3d(point.X, point.Y, point.Z);
                    labelId = AeccGeneralNoteLabel.Create(
                        location, 
                        labelStyle.InternalObjectId, 
                        markerStyle.InternalObjectId);
                }

                var createdLabel = labelId.GetObject(acDb.OpenMode.ForRead) as AeccGeneralNoteLabel;
                if (createdLabel != null)
                {
                    return new GeneralNoteLabel(createdLabel, true);
                }
                return null;
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"GeneralNoteLabel";
        #endregion
    }
}
