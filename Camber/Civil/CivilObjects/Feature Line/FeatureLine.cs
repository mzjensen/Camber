#region references
using System;
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acGeom = Autodesk.AutoCAD.Geometry;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using AeccFeatureLine = Autodesk.Civil.DatabaseServices.FeatureLine;
using Autodesk.DesignScript.Runtime;
using Autodesk.DesignScript.Geometry;
using Dynamo.Graph.Nodes;
using DynamoServices;
using Camber.DynamoExtensions.GeometryExtensions;
using Camber.Utils;
#endregion

namespace Camber.Civil.CivilObjects
{
    [RegisterForTrace]
    [IsVisibleInDynamoLibrary(false)]
    public sealed class FeatureLine : CivilObject
    {
        #region properties
        internal AeccFeatureLine AeccFeatureLine => AcObject as AeccFeatureLine;
        private const string InvalidStationRangeMessage = "Station value must be greater than or equal to zero and less than or equal to the 2D length of the Feature Line";

        /// <summary>
        /// Gets the 2D length of a Feature Line.
        /// </summary>
        public double Length2D => GetDouble();

        /// <summary>
        /// Gets the 3D length of a Feature Line.
        /// </summary>
        public double Length3D => GetDouble();

        /// <summary>
        /// Gets the maximum elevation of a Feature Line.
        /// </summary>
        public double MaxElevation => GetDouble();

        /// <summary>
        /// Gets the maximum grade along a Feature Line.
        /// </summary>
        public double MaxGrade => GetDouble();

        /// <summary>
        /// Gets the minimum elevation of a Feature Line.
        /// </summary>
        public double MinElevation => GetDouble();

        /// <summary>
        /// Gets the minimum grade along a Feature Line.
        /// </summary>
        public double MinGrade => GetDouble();

        /// <summary>
        /// Gets the number of PI points in a Feature Line.
        /// </summary>
        public int PIPointsCount => GetInt();

        /// <summary>
        /// Gets the number of points in a Feature Line, including both PI points and elevation points.
        /// </summary>
        public int PointsCount => GetInt();

        /// <summary>
        /// Gets the number of elevation points in a Feature Line.
        /// </summary>
        public int ElevationPointsCount => GetInt();

        /// <summary>
        /// Gets the segments that make up a Feature Line.
        /// </summary>
        internal List<FeatureLineSegment> Segments
        {
            get
            {
                List<FeatureLineSegment> retList = new List<FeatureLineSegment>();
                for (int i = 0; i < PointsCount - 1; i++)
                {
                    retList.Add(new FeatureLineSegment(this, i, i + 1));
                }
                return retList;
            }
        }

        /// <summary>
        /// Gets the 2D base Polycurve of a Feature Line in the XY plane. The output curve will consist of line and/or arc segments.
        /// </summary>
        public PolyCurve PolyCurve2D
        {
            get
            {
                List<Point> dynPoints = new List<Point>();
                foreach (Point point in PIPoints)
                {
                    dynPoints.Add(Point.ByCoordinates(point.X, point.Y));
                }
                List<Curve> subCurves = new List<Curve>();
                for (int i = 0; i < PIPointsCount - 1; i++)
                {
                    subCurves.Add(CurveExtensions.ByStartPointEndPointBulge(dynPoints[i], dynPoints[i + 1], Bulges[i]));
                }
                return PolyCurve.ByJoinedCurves(subCurves);
            }
        }

        /// <summary>
        /// Gets the 3D Polycurve representation of a Feature Line. The output curve will consist of line, arc, and/or helical segments.
        /// </summary>
        public PolyCurve PolyCurve3D
        {
            get
            {
                List<Curve> segmentCurves = new List<Curve>();
                foreach (FeatureLineSegment segment in Segments)
                {
                    segmentCurves.Add(segment.Curve3D);
                }
                return PolyCurve.ByJoinedCurves(segmentCurves);
            }
        }
        [IsVisibleInDynamoLibrary(false)]
        public List<Autodesk.Civil.FeatureLinePointType> PointTypes
        {
            get
            {
                List<int> indices = new List<int>();
                foreach (Point point in PIPoints)
                {
                    indices.Add(AllPoints.IndexOf(point));
                }
                List<Autodesk.Civil.FeatureLinePointType> retList = new List<Autodesk.Civil.FeatureLinePointType>();
                for (int i = 0; i < PointsCount; i++)
                {
                    retList.Add(Autodesk.Civil.FeatureLinePointType.ElevationPoint);
                }
                foreach (int index in indices)
                {
                    retList[index] = Autodesk.Civil.FeatureLinePointType.PIPoint;
                }
                return retList;
            }
        }
        protected List<double> Bulges
        {
            get
            {
                // Get bulge values for all segments
                List<double> bulges = new List<double>();
                for (int i = 0; i < PointsCount - 1; i++)
                {
                    bulges.Add(AeccFeatureLine.GetBulge(i));
                }
                List<int> indices = new List<int>();
                // Get indices of PI points
                foreach (Point point in PIPoints)
                {
                    indices.Add(AllPoints.IndexOf(point));
                }
                // Drop the last item, which is the last PI point
                indices.RemoveAt(indices.Count - 1);
                // Filter list of bulges to get only the values for segments between PI points
                List<double> retList = new List<double>();
                foreach (int index in indices)
                {
                    retList.Add(bulges[index]);
                }
                return retList;
            }
        }
        public List<Point> PIPoints => GetPointsByType(Autodesk.Civil.FeatureLinePointType.PIPoint);
        public List<Point> ElevationPoints => GetPointsByType(Autodesk.Civil.FeatureLinePointType.ElevationPoint);
        public List<Point> AllPoints => GetPointsByType(Autodesk.Civil.FeatureLinePointType.AllPoints);
        #endregion

        #region constructors
        internal FeatureLine(AeccFeatureLine aeccFeatureLine, bool isDynamoOwned = false) : base(aeccFeatureLine, isDynamoOwned) { }

        [SupressImportIntoVM]
        internal static FeatureLine GetByObjectId(acDb.ObjectId featureLineId)
            => CivilObjectSupport.Get<FeatureLine, AeccFeatureLine>
            (featureLineId, (featureLine) => new FeatureLine(featureLine));
        #endregion

        #region methods
        public override string ToString() => $"FeatureLine(Name = {Name})";

        /// <summary>
        /// Gets the points, by type, that define the Feature Line.
        /// </summary>
        /// <param name="pointType"></param>
        /// <returns></returns>
        internal List<Point> GetPointsByType(Autodesk.Civil.FeatureLinePointType pointType)
        {
            acGeom.Point3dCollection acadPnts = AeccFeatureLine.GetPoints(pointType);
            List<Point> retList = new List<Point>();
            foreach (acGeom.Point3d pnt in acadPnts)
            {
                retList.Add(Point.ByCoordinates(pnt.X, pnt.Y, pnt.Z));
            }
            return retList;
        }

        /// <summary>
        /// Gets a Feature Line from a Civil Object.
        /// </summary>
        /// <param name="civilObject"></param>
        /// <returns></returns>
        [NodeCategory("Actions")]
        public static FeatureLine GetFromCivilObject(civDynNodes.CivilObject civilObject)
        {
            var document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                var aeccFeatureLine = ctx.Transaction.GetObject(civilObject.InternalObjectId, acDb.OpenMode.ForRead);
                if (!(aeccFeatureLine is AeccFeatureLine)) { throw new ArgumentException("Civil Object is not a Feature Line."); }
                return GetByObjectId(civilObject.InternalObjectId);
            }
        }

        /// <summary>
        /// Gets the deflection angle (in degrees) between consecutive segments at a given point along a Feature Line. 
        /// The angle is measured between the incoming and outgoing segments and is relative to the incoming segment. 
        /// Positive angles are clockwise, negative angles are counterclockwise.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double DeflectionAngleAtPoint(Point point) => AeccFeatureLine.GetDeflectionAngleAtPoint((acGeom.Point3d)GeometryConversions.DynPointToAcPoint(point, true));

        /// <summary>
        /// Gets the incoming grade at a point along a Feature Line.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double GradeInAtPoint(Point point) => AeccFeatureLine.GetGradeInAtPoint((acGeom.Point3d)GeometryConversions.DynPointToAcPoint(point, true));

        /// <summary>
        /// Gets the outgoing grade at a given point along a Feature Line.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double GradeOutAtPoint(Point point) => AeccFeatureLine.GetGradeOutAtPoint((acGeom.Point3d)GeometryConversions.DynPointToAcPoint(point, true));

        /// <summary>
        /// Get the 3D distance at a given point along a Feature Line relative to the start point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double Get3dDistanceAtPoint(Point point) => AeccFeatureLine.Get3dDistanceAtPoint((acGeom.Point3d)GeometryConversions.DynPointToAcPoint(point, true));

        /// <summary>
        /// Gets the station value of a point along a Feature Line.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double StationAtPoint(Point point) => PolyCurve2D.ParameterAtPoint(Point.ByCoordinates(point.X, point.Y)) * Length2D;

        /// <summary>
        /// Deletes a point along a Feature Line by point index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public FeatureLine DeletePoint(int index)
        {
            if (index < 0) { throw new ArgumentException("Index cannot be less than zero."); }
            if (index > PointsCount - 1) { throw new ArgumentException("Index cannot be greater than the total number of points."); }

            try
            {
                bool isOpenedForWrite = AeccFeatureLine.IsWriteEnabled;
                if (!isOpenedForWrite) { AeccFeatureLine.UpgradeOpen(); }
                acGeom.Point3d point = AeccFeatureLine.GetPoints(Autodesk.Civil.FeatureLinePointType.AllPoints)[index];
                if (PointTypes[index] is Autodesk.Civil.FeatureLinePointType.PIPoint)
                {
                    AeccFeatureLine.DeletePIPoint(point);
                    if (!isOpenedForWrite) { AeccFeatureLine.DowngradeOpen(); }
                    return this;
                }
                AeccFeatureLine.DeleteElevationPoint(point);
                if (!isOpenedForWrite) { AeccFeatureLine.DowngradeOpen(); }
                return this;
            }
            catch { throw; }
        }

        /// <summary>
        /// Inserts an elevation point along a Feature Line by station and elevation.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public FeatureLine InsertElevationPoint(double station, double elevation)
        {
            if (station < 0 || station > Length2D) { throw new ArgumentException(InvalidStationRangeMessage); }
            try
            {
                bool isOpenedForWrite = AeccFeatureLine.IsWriteEnabled;
                if (!isOpenedForWrite) { AeccFeatureLine.UpgradeOpen(); }
                Point point = PointAtStation(station);
                acGeom.Point3d acPoint = new acGeom.Point3d(point.X, point.Y, elevation);
                AeccFeatureLine.InsertElevationPoint(acPoint);
                if (!isOpenedForWrite) { AeccFeatureLine.DowngradeOpen(); }
                return this;
            }
            catch { throw; }
        }

        /// <summary>
        /// Add a PI point to a Feature Line.
        /// </summary>
        /// <param name="featureLine"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        [NodeCategory("Actions")]
        public FeatureLine InsertPIPoint(Point point)
        {
            try
            {
                bool isOpenedForWrite = AeccFeatureLine.IsWriteEnabled;
                if (!isOpenedForWrite) { AeccFeatureLine.UpgradeOpen(); }
                AeccFeatureLine.InsertPIPoint((acGeom.Point3d)GeometryConversions.DynPointToAcPoint(point, true));
                if (!isOpenedForWrite) { AeccFeatureLine.DowngradeOpen(); }
                return this;
            }
            catch { throw; }
        }

        /// <summary>
        /// Assigns surface elevations to all points along a Feature Line, with an optional toggle to add intermediate elevation points.
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="addIntermediatePoints"></param>
        /// <returns></returns>
        public FeatureLine AssignElevationsFromSurface(civDynNodes.Surface surface, bool addIntermediatePoints = false)
        {
            try
            {
                bool isOpenedForWrite = AeccFeatureLine.IsWriteEnabled;
                if (!isOpenedForWrite) { AeccFeatureLine.UpgradeOpen(); }
                AeccFeatureLine.AssignElevationsFromSurface(surface.InternalObjectId, addIntermediatePoints);
                if (!isOpenedForWrite) { AeccFeatureLine.DowngradeOpen(); }
                return this;
            }
            catch { throw; }
        }

        /// <summary>
        /// Gets a point along a Feature Line at a given station.
        /// </summary>
        /// <param name="station"></param>
        /// <returns></returns>
        public Point PointAtStation(double station = 0)
        {
            if (station < 0 || station > Length2D) { throw new ArgumentException(InvalidStationRangeMessage); }
            return PolyCurve2D.PointAtSegmentLength(station);
        }

        /// <summary>
        /// Gets the elevation of a Feature Line at a given station.
        /// </summary>
        /// <param name="station"></param>
        /// <returns></returns>
        public double ElevationAtStation(double station = 0)
        {
            if (station < 0 || station > Length2D) { throw new ArgumentException(InvalidStationRangeMessage); }
            return PointAtStation(station).Z;
        }

        /// <summary>
        /// Gets a coordinate system at a station along a Feature Line.
        /// </summary>
        /// <param name="station"></param>
        /// <param name="vertical"></param>
        /// <returns></returns>
        public CoordinateSystem CoordinateSystemAtStation(FeatureLine featureLine, double station = 0, bool vertical = true)
        {
            if (station < 0 || station > Length2D) { throw new ArgumentException(InvalidStationRangeMessage); }

            CoordinateSystem retCS;
            double param = station / featureLine.Length2D;
            var baseCurve = featureLine.PolyCurve2D;
            var baseCS = baseCurve.CoordinateSystemAtParameter(param).Translate(0, 0, ElevationAtStation(station));
            if (baseCS.ZAxis.Z < 0)
            {
                baseCS = CoordinateSystem.ByOriginVectors(baseCS.Origin, baseCS.XAxis.Reverse(), baseCS.YAxis);
            }
            var point = PointAtStation(station);

            if (vertical == false)
            {
                if (param == 0)
                {
                    retCS = baseCS.Rotate(baseCS.YZPlane, MathUtils.GradeToAngle(GradeOutAtPoint(point)));
                }
                else
                {
                    retCS = baseCS.Rotate(baseCS.YZPlane, MathUtils.GradeToAngle(GradeInAtPoint(point)));
                }
            }
            else
            {
                retCS = baseCS;
            }
            return retCS;
        }
        #endregion
    }
}
