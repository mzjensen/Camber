#region references
using Autodesk.DesignScript.Geometry;
using System;
using System.Linq;
using System.Collections.Generic;
using Camber.Utilities;
using Autodesk.DesignScript.Runtime;
#endregion

namespace Camber.DynamoExtensions.GeometryExtensions
{
    [IsVisibleInDynamoLibrary(false)]
    public class CurveExtensions
    {
        #region constructors
        private CurveExtensions() { }
        #endregion

        #region methods
        /// <summary>
        /// Create curve by start point, end point, and bulge value.
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="bulge"></param>
        /// <returns>If the bulge is zero, a Line is returned. Otherwise an Arc is returned.</returns>
        public static Curve ByStartPointEndPointBulge(Point startPoint, Point endPoint, double bulge)
        {
            if (bulge == 0)
            {
                return Line.ByStartPointEndPoint(startPoint, endPoint);
            }
            else
            {
                Dictionary<string, object> paramDict = MathUtilities.BulgeToParameters(startPoint, endPoint, bulge);
                return Arc.ByCenterPointRadiusAngle((Point)paramDict["centerPoint"], (double)paramDict["radius"], (double)paramDict["startAngle"], (double)paramDict["endAngle"], Vector.ZAxis());
            }
        }

        /// <summary>
        /// Gets a coordinate system at the specified parameter.
        /// Different from the OOTB node in that the Y-Axis will be consistenly aligned along the curve's tangent.
        /// Also includes a toggle to align the local Z-Axis with the world Z-Axis.
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="parameter"></param>
        /// <param name="vertical"></param>
        /// <returns></returns>
        public static CoordinateSystem CoordinateSystemAtParameter(Curve curve, double parameter, bool vertical = false)
        {
            if (parameter < 0 || parameter > 1)
            {
                throw new ArgumentException("Parameter out of range");
            }

            Vector tangent = curve.TangentAtParameter(parameter);
            Vector xAxis = tangent.Cross(Vector.ZAxis());
            Vector yAxis = Vector.ByCoordinates(tangent.X, tangent.Y, tangent.Z);
            if (vertical)
            {
                yAxis = Vector.ByCoordinates(tangent.X, tangent.Y, 0);
            }

            CoordinateSystem cs = CoordinateSystem.ByOriginVectors(curve.PointAtParameter(parameter), xAxis, yAxis);
            return cs;
        }

        /// <summary>
        /// Offsets a curve by the specified distance.
        /// Different from the equivalent OOTB node in that positive offsets are always to the right and negative always to the left (relative to start point).
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static Curve Offset(Curve curve, double distance, double elevDiff = 0)
        {
            Curve outputCurve;
            double absDist = System.Math.Abs(distance);

            // Check input distances
            if (distance == 0 && elevDiff == 0)
            {
                throw new ArgumentException("Horizontal and vertical offset both equal zero");
            }
            else if (distance == 0 && elevDiff != 0)
            {
                outputCurve = curve.Translate(0, 0, elevDiff) as Curve;
            }
            else
            {
                // Get Z-aligned normal and check direction
                var cs = curve.CoordinateSystemAtParameter(0);
                var vZ = cs.ZAxis;
                // If Z component is negative, reverse
                if (vZ.Z < 0)
                {
                    vZ = vZ.Reverse();
                }
                // Cross with Y axis to ensure normal is always pointing to the right
                var vCross = cs.YAxis.Cross(vZ);
                // Create list of offset curves
                List<Geometry> lstOff = new List<Geometry>();
                if (curve is Line)
                {
                    // If the curve is a line, the offset plane will be aligned incorrectly. Supposedly fixed in Dynamo v2.10.
                    lstOff.Add(curve.Translate(vCross, absDist));
                    lstOff.Add(curve.Translate(vCross, -absDist));
                }
                else
                {
                    lstOff.Add(curve.Offset(absDist));
                    lstOff.Add(curve.Offset(-absDist));
                }
                // Check to see which offset curve is on the right  
                var line = Line.ByStartPointDirectionLength(curve.StartPoint, vCross, absDist);
                var lstInt = new List<Boolean>();
                foreach (var c in lstOff)
                {
                    lstInt.Add(c.DoesIntersect(line));
                }
                // Get indices - true means curve is on the right, false means left
                int tIndex = lstInt.FindIndex(a => a == true);
                int fIndex = lstInt.FindIndex(a => a == false);
                // If input is positive, return offset curve on right, else return offset curve on left
                Geometry offsetCurve = (distance > 0) ? lstOff[tIndex] : lstOff[fIndex];
                // Translate curve by input elevation difference
                outputCurve = offsetCurve.Translate(0, 0, elevDiff) as Curve;
            }
            return outputCurve;
        }
        public static bool CheckNormal(Curve curve)
        {
            // Get curve normal
            var vNorm = curve.NormalAtParameter(0);
            // Return false if Z component is nonzero, else return true
            if(vNorm.Z != 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static Point PointAtSegmentLengthFromPoint(Curve curve, Point point, double segmentLength = 1)
        {
            var startParam = curve.ParameterAtPoint(point);

            if (segmentLength < -curve.SegmentLengthAtParameter(startParam) || segmentLength > curve.Length - curve.SegmentLengthAtParameter(startParam))
            {
                throw new ArgumentException("Segment length results in point off input curve");
            }

            var paramIncrement = segmentLength / curve.Length;
            double endParam = startParam + paramIncrement;
            return curve.PointAtParameter(endParam);
        }
        
        public static IList<double> KeyParameters(Curve curve)
        {
            // Note 6.19.21 - this will return parameters that are very very close, but not quite equal.
            // Possible to implement some type of IEqualityComparer that defines if they are equal within a tolerance?
            Geometry[] childCurvesArray = curve.Explode();
            var childCurvesList = childCurvesArray.ToList();
            List<Point> points = new List<Point>();
            foreach (Curve childCurve in childCurvesList)
            {
                points.Add(childCurve.StartPoint);
                points.Add(childCurve.EndPoint);
            }
            List<double> parameters = new List<double>();
            foreach (Point point in points)
            {
                parameters.Add(curve.ParameterAtPoint(point));
            }
            List<double> uniqueParameters = parameters.Distinct().ToList();
            uniqueParameters.Sort();

            if (curve.IsClosed)
            {
                if (uniqueParameters.Contains(1))
                {
                    uniqueParameters.Remove(1);
                }
            }

            return uniqueParameters;
        }

        public static IList<Point> KeyPoints(Curve curve)
        {
            List<Point> retList = new List<Point>();
            foreach (double parameter in KeyParameters(curve))
            {
                retList.Add(curve.PointAtParameter(parameter));
            }
            return retList;
        }
        
        public static IList<double> ParametersAtFrequency(Curve curve, double frequency = 1, bool includeKeyParameters = true)
        {
            if (frequency <= 0)
            {
                throw new ArgumentException("Frequency is less than or equal to zero");
            }

            if (frequency > curve.Length)
            {
                throw new ArgumentException("Frequency is greater than curve length");
            }
            
            double paramIncrement = frequency / curve.Length;
            List<double> parameters = new List<double>();
            for (double i = 0; i < 1; i += paramIncrement)
            {
                parameters.Add(i);
            }
            parameters.Add(1);
            
            if (includeKeyParameters)
            {
                IList<double> keyParams = KeyParameters(curve);
                foreach (double keyParam in keyParams)
                {
                    parameters.Add(keyParam);
                }
            }
            List<double> uniqueParameters = parameters.Distinct().ToList();
            uniqueParameters.Sort();

            if (curve.IsClosed)
            {
                if (uniqueParameters.Contains(1))
                {
                    uniqueParameters.Remove(1);
                }
            }

            return uniqueParameters;
        }

        public static IList<double> ParametersAtFrequency(Curve curve, double startParam, double frequency = 1, bool includeKeyPoints = true)
        {
            IList<double> parameters = ParametersAtFrequency(curve, frequency, includeKeyPoints);

            List<double> filteredParams = new List<double>();
            foreach (double parameter in parameters)
            {
                if (parameter >= startParam)
                {
                    filteredParams.Add(parameter);
                }
            }
            return filteredParams;
        }

        public static IList<double> ParametersAtFrequency(Curve curve, double startParam, double endParam, double frequency = 1, bool includeKeyPoints = true)
        {
            IList<double> parameters = ParametersAtFrequency(curve, frequency, includeKeyPoints);

            if (startParam >= endParam)
            {
                throw new ArgumentException("Start parameter is greater than or equal to end parameter");
            }

            List<double> filteredParams = new List<double>();
            foreach (double parameter in parameters)
            {
                if (parameter >= startParam && parameter <= endParam)
                {
                    filteredParams.Add(parameter);
                }
            }

            return filteredParams;
        }
        #endregion
    }
}