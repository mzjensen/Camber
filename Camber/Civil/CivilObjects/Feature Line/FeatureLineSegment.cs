#region references
using System;
using System.Collections.Generic;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Camber.Utils;
#endregion

namespace Camber.Civil.CivilObjects
{
    [IsVisibleInDynamoLibrary(false)]
    public sealed class FeatureLineSegment
    {
        #region properties
        public FeatureLine FeatureLine { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public Autodesk.Civil.FeatureLinePointType StartPointType => FeatureLine.PointTypes[StartIndex];
        public Autodesk.Civil.FeatureLinePointType EndPointType => FeatureLine.PointTypes[EndIndex];
        public Point StartPoint => FeatureLine.AllPoints[StartIndex];
        public Point EndPoint => FeatureLine.AllPoints[EndIndex];
        public int ParentStartIndex
        {
            get
            {
                int retVal = 0;
                for (int i = StartIndex; i > 0; i--)
                {
                    if (FeatureLine.PointTypes[i] is Autodesk.Civil.FeatureLinePointType.PIPoint)
                    {
                        retVal = i;
                        break;
                    }
                }
                return retVal;
            }
        }
        public int ParentEndIndex
        {
            get
            {
                int retVal = 0;
                for (int i = EndIndex; i < FeatureLine.PointsCount; i++)
                {
                    if (FeatureLine.PointTypes[i] is Autodesk.Civil.FeatureLinePointType.PIPoint)
                    {
                        retVal = i;
                        break;
                    }
                }
                return retVal;
            }
        }
        public Point ParentStartPoint => FeatureLine.AllPoints[ParentStartIndex];
        public Point ParentEndPoint => FeatureLine.AllPoints[ParentEndIndex];
        public double ParentBulge => FeatureLine.AeccFeatureLine.GetBulge(ParentStartIndex);
        public double Grade => FeatureLine.GradeOutAtPoint(StartPoint);
        public double Length3D => FeatureLine.Get3dDistanceAtPoint(EndPoint) - FeatureLine.Get3dDistanceAtPoint(StartPoint);
        public double Length2D => FeatureLine.StationAtPoint(EndPoint) - FeatureLine.StationAtPoint(StartPoint);
        public double ParentLength3D => FeatureLine.Get3dDistanceAtPoint(ParentEndPoint) - FeatureLine.Get3dDistanceAtPoint(ParentStartPoint);
        public double ParentLength2D => FeatureLine.StationAtPoint(ParentEndPoint) - FeatureLine.StationAtPoint(ParentStartPoint);
        public FeatureLineSegmentType SegmentType
        {
            get
            {
                double parentBulge = ParentBulge;
                if (parentBulge == 0)
                {
                    return FeatureLineSegmentType.Line;
                }
                else
                {
                    if (Math.Abs(Grade) < 0.000001)
                    {
                        return FeatureLineSegmentType.Arc;
                    }
                    else
                    {
                        return FeatureLineSegmentType.Helix;
                    }
                }
            }
        }
        public Curve Curve3D
        {
            get
            {
                Dictionary<string, object> parentParamDict = MathUtils.BulgeToParameters(ParentStartPoint, ParentEndPoint, ParentBulge);
                if (SegmentType is FeatureLineSegmentType.Line)
                {
                    return Line.ByStartPointEndPoint(StartPoint, EndPoint);
                }
                else if (SegmentType is FeatureLineSegmentType.Arc)
                {
                    var parentCenterPoint = (Point)parentParamDict["centerPoint"];
                    parentCenterPoint = (Point)parentCenterPoint.Translate(0, 0, StartPoint.Z);
                    var endPoint = Point.ByCoordinates(EndPoint.X, EndPoint.Y, StartPoint.Z);
                    if (ParentBulge > 0)
                    {
                        return Arc.ByCenterPointStartPointEndPoint(parentCenterPoint, StartPoint, endPoint);
                    }
                    else
                    {
                        return Arc.ByCenterPointStartPointEndPoint(parentCenterPoint, endPoint, StartPoint);
                    }
                }
                else
                {
                    var parentCenterPoint = (Point)parentParamDict["centerPoint"];
                    var angle = ((double)parentParamDict["endAngle"] - (double)parentParamDict["startAngle"]) * (Length2D / ParentLength2D);
                    double rise = EndPoint.Z - StartPoint.Z;
                    double pitch = rise * 360 / angle;
                    Vector axis = Vector.ZAxis();
                    if (ParentBulge < 0)
                    {
                        axis = axis.Reverse();
                        if (rise >= 0)
                        {
                            pitch *= -1;
                        }
                    }
                    return Helix.ByAxis(parentCenterPoint, axis, StartPoint, pitch, angle);
                }
            }
        }
        #endregion

        #region constructors
        internal FeatureLineSegment(FeatureLine featureLine, int startIndex, int endIndex)
        {
            FeatureLine = featureLine;
            StartIndex = startIndex;
            EndIndex = endIndex;
        }
        #endregion
    }
}
