#region
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acGeom = Autodesk.AutoCAD.Geometry;
using civApp = Autodesk.Civil.ApplicationServices;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using Autodesk.DesignScript.Geometry;
using AeccSurfaceElevationLabel = Autodesk.Civil.DatabaseServices.SurfaceElevationLabel;
using DynamoServices;
using Camber.Civil.Styles.Labels.Surface;
using Camber.Civil.Styles.Objects;
#endregion

namespace Camber.Civil.Labels
{
    [RegisterForTrace]
    public sealed class SurfaceElevationLabel : Label
    {
        #region properties
        internal AeccSurfaceElevationLabel aeccSurfaceElevationLabel => AcObject as AeccSurfaceElevationLabel;

        /// <summary>
        /// Gets the Surface that a Surface Elevation Label is associated with.
        /// </summary>
        public civDynNodes.Surface Surface { get; set; }

        /// <summary>
        /// Gets the elevation of a Surface Elevation Label's location.
        /// </summary>
        public double Elevation => Surface.ElevationByXY(AnchorPoint.X, AnchorPoint.Y);
        #endregion

        #region constructors
        internal SurfaceElevationLabel(AeccSurfaceElevationLabel AeccSurfaceSlopeLabel, civDynNodes.Surface surface, bool isDynamoOwned = false) : base(AeccSurfaceSlopeLabel, isDynamoOwned)
        {
            Surface = surface;
        }

        /// <summary>
        /// Creates a Surface Elevation Label by point.
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="point"></param>
        /// <param name="labelStyle"></param>
        /// <param name="markerStyle"></param>
        /// <returns></returns>
        public static SurfaceElevationLabel ByPoint(civDynNodes.Surface surface, Point point, SurfaceElevationLabelStyle labelStyle, MarkerStyle markerStyle)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                civApp.CivilDocument cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                acDb.ObjectId labelId = acDynApp.ElementBinder.GetObjectIdFromTrace(ctx.Database);

                if (labelId.IsValid && !labelId.IsErased)
                {
                    AeccSurfaceElevationLabel aeccLabel = (AeccSurfaceElevationLabel)labelId.GetObject(acDb.OpenMode.ForWrite);
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
                    acGeom.Point2d location = new acGeom.Point2d(point.X, point.Y);
                    labelId = AeccSurfaceElevationLabel.Create(surface.InternalObjectId, location, labelStyle.InternalObjectId, markerStyle.InternalObjectId);
                }

                var createdLabel = labelId.GetObject(acDb.OpenMode.ForRead) as AeccSurfaceElevationLabel;
                if (createdLabel != null)
                {
                    return new SurfaceElevationLabel(createdLabel, surface, true);
                }
                return null;
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"SurfaceElevationLabel(Elevation = {Elevation:F2})";
        #endregion
    }
}
