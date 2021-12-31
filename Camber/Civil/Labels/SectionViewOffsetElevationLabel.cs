#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acGeom = Autodesk.AutoCAD.Geometry;
using civApp = Autodesk.Civil.ApplicationServices;
using Autodesk.DesignScript.Geometry;
using AeccSectionViewOffsetElevationLabel = Autodesk.Civil.DatabaseServices.SectionViewOffsetElevationLabel;
using Camber.Civil.Styles.Labels.SectionView;
using DynamoServices;
using Camber.Civil.Styles.Objects;
using Camber.Civil.CivilObjects;
#endregion

namespace Camber.Civil.Labels
{
    [RegisterForTrace]
    public sealed class SectionViewOffsetElevationLabel : Label
    {
        #region properties
        internal AeccSectionViewOffsetElevationLabel AeccSectionViewOffsetElevationLabel => AcObject as AeccSectionViewOffsetElevationLabel;

        /// <summary>
        /// Gets the Section View that a Section View Offset Elevation Label is associated with.
        /// </summary>
        public SectionView SectionView { get; set; }

        /// <summary>
        /// Gets the offset value of a Section View Offset Elevation Label's location.
        /// </summary>
        public double Offset => (double)SectionView.GetOffsetElevationAtPoint(SectionView, AnchorPoint)["Offset"];

        /// <summary>
        /// Gets the elevation value of a Section View Offset Elevation Label's location.
        /// </summary>
        public double Elevation => (double)SectionView.GetOffsetElevationAtPoint(SectionView, AnchorPoint)["Elevation"];
        #endregion

        #region constructors
        internal SectionViewOffsetElevationLabel(
            AeccSectionViewOffsetElevationLabel AeccSectionViewOffsetElevationLabel, 
            SectionView sectionView, 
            bool isDynamoOwned = false) 
            : base(AeccSectionViewOffsetElevationLabel, isDynamoOwned)
        {
            SectionView = sectionView;
        }

        /// <summary>
        /// Creates a Section View Offset Elevation Label by offset and elevation.
        /// </summary>
        /// <param name="sectionView"></param>
        /// <param name="offset"></param>
        /// <param name="elevation"></param>
        /// <param name="labelStyle"></param>
        /// <param name="markerStyle"></param>
        /// <returns></returns>
        public static SectionViewOffsetElevationLabel ByOffsetElevation(SectionView sectionView, double offset, double elevation, SectionViewOffsetElevationLabelStyle labelStyle, MarkerStyle markerStyle)
        {
            return ByPoint(sectionView, SectionView.GetPointAtOffsetElevation(sectionView, offset, elevation), labelStyle, markerStyle);
        }

        /// <summary>
        /// Creates a Section View Offset Elevation Label by point.
        /// </summary>
        /// <param name="sectionView"></param>
        /// <param name="point"></param>
        /// <param name="labelStyle"></param>
        /// <param name="markerStyle"></param>
        /// <returns></returns>
        public static SectionViewOffsetElevationLabel ByPoint(SectionView sectionView, Point point, SectionViewOffsetElevationLabelStyle labelStyle, MarkerStyle markerStyle)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                civApp.CivilDocument cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                acDb.ObjectId labelId = acDynApp.ElementBinder.GetObjectIdFromTrace(ctx.Database);

                if (labelId.IsValid && !labelId.IsErased)
                {
                    AeccSectionViewOffsetElevationLabel aeccLabel = (AeccSectionViewOffsetElevationLabel)labelId.GetObject(acDb.OpenMode.ForWrite);
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
                    double offset = (double)SectionView.GetOffsetElevationAtPoint(sectionView, point)["Offset"];
                    double elevation = (double)SectionView.GetOffsetElevationAtPoint(sectionView, point)["Elevation"];
                    labelId = AeccSectionViewOffsetElevationLabel.Create(sectionView.InternalObjectId, offset, elevation, labelStyle.InternalObjectId, markerStyle.InternalObjectId);
                }

                var createdLabel = labelId.GetObject(acDb.OpenMode.ForRead) as AeccSectionViewOffsetElevationLabel;
                if (createdLabel != null)
                {
                    return new SectionViewOffsetElevationLabel(createdLabel, sectionView, true);
                }
                return null;
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"SectionViewOffsetElevationLabel(Offset = {Offset:F2}, Elevation = {Elevation:F2})";
        #endregion
    }
}
