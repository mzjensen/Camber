#region
using System;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acGeom = Autodesk.AutoCAD.Geometry;
using civDb = Autodesk.Civil.DatabaseServices;
using civApp = Autodesk.Civil.ApplicationServices;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using Autodesk.DesignScript.Geometry;
using AeccSurfaceSlopeLabel = Autodesk.Civil.DatabaseServices.SurfaceSlopeLabel;
using DynamoServices;
using Camber.Civil.Styles.Labels.Surface;
using Camber.Utilities.GeometryConversions;
#endregion

namespace Camber.Civil.Labels
{
    [RegisterForTrace]
    public sealed class SurfaceSlopeLabel : Label
    {
        #region properties
        internal AeccSurfaceSlopeLabel AeccSurfaceSlopeLabel => AcObject as AeccSurfaceSlopeLabel;

        /// <summary>
        /// Gets the Surface that a Surface Slope Label is associated with.
        /// </summary>
        public civDynNodes.Surface Surface
        {
            get
            {
                acDynNodes.Document document = acDynNodes.Document.Current;

                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    civDb.Surface surface =
                        (civDb.Surface)ctx
                        .Transaction
                        .GetObject(AeccSurfaceSlopeLabel.FeatureId, acDb.OpenMode.ForRead);
                    return civDynNodes.Selection.SurfaceByName(surface.Name, document);
                }
            }
        }

        /// <summary>
        /// Gets the type of a Surface Slope Label.
        /// </summary>
        public string SlopeLabelType => GetString();

        /// <summary>
        /// Gets the location of a Surface Slope Label. If the label is a one-point label, then the location is a Point.
        /// If it is a two-point label, then the location is a Line.
        /// </summary>
        public Geometry Location
        {
            get
            {
                if (SlopeLabelType == "OnePoint")
                {
                    return Surface.SamplePoint(GeometryConversions.AcPointToDynPoint(AeccSurfaceSlopeLabel.Location));
                }
                else if (SlopeLabelType == "TwoPoint")
                {
                    Point startPoint = Surface.SamplePoint(GeometryConversions.AcPointToDynPoint(AeccSurfaceSlopeLabel.Location));
                    Point endPoint = Surface.SamplePoint(GeometryConversions.AcPointToDynPoint(AeccSurfaceSlopeLabel.Location2));
                    return Line.ByStartPointEndPoint(startPoint, endPoint);
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion

        #region constructors
        internal SurfaceSlopeLabel(
            AeccSurfaceSlopeLabel AeccSurfaceSlopeLabel, 
            bool isDynamoOwned = false) 
            : base(AeccSurfaceSlopeLabel, isDynamoOwned)
        { }

        /// <summary>
        /// Creates a one-point Surface Slope Label.
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="point"></param>
        /// <param name="labelStyle"></param>
        /// <returns></returns>
        public static SurfaceSlopeLabel ByPoint(
            civDynNodes.Surface surface, 
            Point point, 
            SurfaceSlopeLabelStyle labelStyle)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                civApp.CivilDocument cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                acDb.ObjectId labelId = acDynApp.ElementBinder.GetObjectIdFromTrace(ctx.Database);

                if (labelId.IsValid && !labelId.IsErased)
                {
                    AeccSurfaceSlopeLabel aeccLabel = (AeccSurfaceSlopeLabel)labelId.GetObject(acDb.OpenMode.ForWrite);
                    if (aeccLabel != null)
                    {
                        // Update the label's location
                        aeccLabel.Location = new acGeom.Point2d(point.X, point.Y);

                        // Update label style
                        aeccLabel.StyleId = labelStyle.InternalObjectId;
                    }
                }
                else
                {
                    // Create new label
                    acGeom.Point2d location = new acGeom.Point2d(point.X, point.Y);
                    labelId = AeccSurfaceSlopeLabel.Create(
                        surface.InternalObjectId, 
                        location, 
                        labelStyle.InternalObjectId);
                }

                var createdLabel = labelId.GetObject(acDb.OpenMode.ForRead) as AeccSurfaceSlopeLabel;
                if (createdLabel != null)
                {
                    return new SurfaceSlopeLabel(createdLabel, true);
                }
                return null;
            }
        }

        /// <summary>
        /// Creates a two-point Surface Slope Label using a Dynamo Line.
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="line"></param>
        /// <param name="labelStyle"></param>
        /// <returns></returns>
        public static SurfaceSlopeLabel ByLine(civDynNodes.Surface surface, Line line, SurfaceSlopeLabelStyle labelStyle)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                civApp.CivilDocument cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                acDb.ObjectId labelId = acDynApp.ElementBinder.GetObjectIdFromTrace(ctx.Database);

                if (labelId.IsValid && !labelId.IsErased)
                {
                    AeccSurfaceSlopeLabel aeccLabel = (AeccSurfaceSlopeLabel)labelId.GetObject(acDb.OpenMode.ForWrite);
                    if (aeccLabel != null)
                    {
                        // Update the label's location
                        aeccLabel.Location = new acGeom.Point2d(line.StartPoint.X, line.StartPoint.Y);
                        aeccLabel.Location2 = new acGeom.Point2d(line.EndPoint.X, line.EndPoint.Y);

                        // Update label style
                        aeccLabel.StyleId = labelStyle.InternalObjectId;
                    }
                }
                else
                {
                    // Create new label
                    acGeom.Point2d location1 = new acGeom.Point2d(line.StartPoint.X, line.StartPoint.Y);
                    acGeom.Point2d location2 = new acGeom.Point2d(line.EndPoint.X, line.EndPoint.Y);
                    labelId = AeccSurfaceSlopeLabel.Create(
                        surface.InternalObjectId, 
                        location1, 
                        location2, 
                        labelStyle.InternalObjectId);
                }

                var createdLabel = labelId.GetObject(acDb.OpenMode.ForRead) as AeccSurfaceSlopeLabel;
                if (createdLabel != null)
                {
                    return new SurfaceSlopeLabel(createdLabel, true);
                }
                return null;
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"SurfaceSlopeLabel(Type = {SlopeLabelType})";

        /// <summary>
        /// Sets the Surface Slope Label's location by Geometry. If the label is a one-point label, then the Geometry must be a Point.
        /// If it is a two-point label, then the Geometry must be a Line.
        /// </summary>
        /// <param name="geometry">Point or Line</param>
        /// <returns></returns>
        public SurfaceSlopeLabel SetLocationByGeometry(Geometry geometry)
        {
            if (!(geometry is Line) && !(geometry is Point))
            {
                throw new ArgumentException("The geometry must be a Point or a Line.");
            }

            if (SlopeLabelType == "OnePoint")
            {
                if (!(geometry is Point))
                {
                    throw new ArgumentException("Geometry must be a Point when the label type is one-point.");
                }
                Point point = (Point)geometry;
                acGeom.Point2d location = new acGeom.Point2d(point.X, point.Y);

                SetValue(location, "Location");
                return this;
            }
            else if (SlopeLabelType == "TwoPoint")
            {
                if (!(geometry is Line))
                {
                    throw new ArgumentException("Geometry must be a Line when the label type is two-point.");
                }
                Line line = (Line)geometry;
                acGeom.Point2d location = new acGeom.Point2d(line.StartPoint.X, line.StartPoint.Y);
                acGeom.Point2d location2 = new acGeom.Point2d(line.EndPoint.X, line.EndPoint.Y);
                SetValue(location, "Location");
                SetValue(location2, "Location2");
                return this;
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}
