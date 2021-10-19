#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acGeom = Autodesk.AutoCAD.Geometry;
using civApp = Autodesk.Civil.ApplicationServices;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using Autodesk.DesignScript.Geometry;
using AeccStationOffsetLabel = Autodesk.Civil.DatabaseServices.StationOffsetLabel;
using DynamoServices;
using Camber.Civil.Styles.Objects;
using Camber.Civil.Styles.Labels.Alignment;
#endregion

namespace Camber.Civil.Labels
{
    [RegisterForTrace]
    public sealed class AlignmentStationOffsetLabel : Label
    {
        #region properties
        internal AeccStationOffsetLabel AeccStationOffsetLabel => AcObject as AeccStationOffsetLabel;

        /// <summary>
        /// Gets the Alignment that an Alignment Station Offset Label is associated with.
        /// </summary>
        public civDynNodes.Alignment Alignment { get; set; }

        /// <summary>
        /// Gets the station value of an Alignment Station Offset Label's location.
        /// </summary>
        public double Station => Alignment.StationOffsetByPoint(AnchorPoint)["Station"];

        /// <summary>
        /// Gets the offset value of an Alignment Station Offset Label's location.
        /// </summary>
        public double Offset => Alignment.StationOffsetByPoint(AnchorPoint)["Offset"];
        #endregion

        #region constructors
        internal AlignmentStationOffsetLabel(AeccStationOffsetLabel AeccStationOffsetLabel, civDynNodes.Alignment alignment, bool isDynamoOwned = false) : base(AeccStationOffsetLabel, isDynamoOwned)
        {
            Alignment = alignment;
        }

        /// <summary>
        /// Creates an Alignment Station Offset Label by station and offset.
        /// </summary>
        /// <param name="alignment"></param>
        /// <param name="station"></param>
        /// <param name="offset"></param>
        /// <param name="labelStyle"></param>
        /// <param name="markerStyle"></param>
        /// <returns></returns>
        public static AlignmentStationOffsetLabel ByStationOffset(civDynNodes.Alignment alignment, double station, double offset, AlignmentStationOffsetLabelStyle labelStyle, MarkerStyle markerStyle)
        {
            Point point = alignment.CoordinateSystemByStationOffset(station, offset).Origin;
            return ByPoint(alignment, point, labelStyle, markerStyle);
        }

        /// <summary>
        /// Creates an Alignment Station Offset Label by point.
        /// </summary>
        /// <param name="alignment"></param>
        /// <param name="point"></param>
        /// <param name="labelStyle"></param>
        /// <param name="markerStyle"></param>
        /// <returns></returns>
        public static AlignmentStationOffsetLabel ByPoint(civDynNodes.Alignment alignment, Point point, AlignmentStationOffsetLabelStyle labelStyle, MarkerStyle markerStyle)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                civApp.CivilDocument cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                acDb.ObjectId labelId = acDynApp.ElementBinder.GetObjectIdFromTrace(ctx.Database);

                if (labelId.IsValid && !labelId.IsErased)
                {
                    AeccStationOffsetLabel aeccLabel = (AeccStationOffsetLabel)labelId.GetObject(acDb.OpenMode.ForWrite);
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
                    labelId = AeccStationOffsetLabel.Create(alignment.InternalObjectId, labelStyle.InternalObjectId, markerStyle.InternalObjectId, location);
                }

                var createdLabel = labelId.GetObject(acDb.OpenMode.ForRead) as AeccStationOffsetLabel;
                if (createdLabel != null)
                {
                    return new AlignmentStationOffsetLabel(createdLabel, alignment, true);
                }
                return null;
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"AlignmentStationOffsetLabel(Station = {Station:F2}, Offset = {Offset:F2})";
        #endregion
    }
}
