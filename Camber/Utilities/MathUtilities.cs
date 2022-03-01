#region references
using System;
using System.IO;
using System.Collections.Generic;
using Camber.DynamoExtensions.GeometryExtensions;
using Autodesk.DesignScript.Runtime;
using Autodesk.DesignScript.Geometry;
#endregion

namespace Camber.Utilities
{
    [IsVisibleInDynamoLibrary(false)]
    public class MathUtilities
    {
        /// <summary>
        /// Converts an angle in degrees to radians
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double DegreesToRadians(double angle) => angle * Math.PI / 180;

        /// <summary>
        /// Converts an angle in radians to degrees
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double RadiansToDegrees(double angle) => angle * 180 / Math.PI;

        /// <summary>
        /// Converts a percent grade to an angle in degrees
        /// </summary>
        /// <param name="grade"></param>
        /// <returns></returns>
        public static double GradeToAngle(double grade) => RadiansToDegrees(Math.Atan(grade));

        /// <summary>
        /// Converts a percent grade to a slope expressed as run:1, with positive slopes running uphill.
        /// </summary>
        /// <param name="grade"></param>
        /// <returns></returns>
        public static double GradeToSlope(double grade) => 1 / grade;

        /// <summary>
        /// Converts a slope expressed as run:1 to a grade, with positive grades running uphill.
        /// </summary>
        /// <param name="slope"></param>
        /// <returns></returns>
        public static double SlopeToGrade(double slope) => 1 / slope;

        /// <summary>
        /// Writes a string to the log file in the user's temp folder
        /// </summary>
        /// <param name="message"></param>
        public static void WriteLog(string message)
        {
            string fileName = "Jensen_Camber.log";

            string path = Path.Combine(Path.GetTempPath(), fileName);
            using (StreamWriter textFile = new StreamWriter(path, true))
            {
                textFile.WriteLine($"[{DateTime.Now}] {message}");
                textFile.Close();
            }
        }

        /// <summary>
        /// Returns the center point, start angle, end angle, radius, and length of an arc described by two points and bulge value. Currently only works with 2D points.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="bulge"></param>
        /// <returns></returns>
        [MultiReturn(new[] { "centerPoint", "startAngle", "endAngle", "radius" })]
        public static Dictionary<string, object> BulgeToParameters(Point p1, Point p2, double bulge)
        {
            // TODO: currently only works for 2D points
            Point startPoint = Point.ByCoordinates(p1.X, p1.Y);
            Point endPoint = Point.ByCoordinates(p2.X, p2.Y);

            double startAngle;
            double endAngle;

            // Angle between start point and end point, in radians
            double theta = DegreesToRadians(PointExtensions.AngleBetweenPoints(startPoint, endPoint));

            double d = startPoint.DistanceTo(endPoint);

            // Angle between the radius lines that connect the vertices to the center point, in radians
            double a = 2 * Math.Atan(bulge);
            double radius = d / 2 / Math.Sin(a);
            var xAxis = Vector.XAxis();
            // Vector describing direction from start point to center point
            var dir = xAxis.Rotate(Vector.ZAxis(), RadiansToDegrees(theta + (Math.PI / 2 - a)));
            Point centerPoint = Line.ByStartPointDirectionLength(startPoint, dir, radius).EndPoint;

            if (bulge < 0)
            {
                startAngle = PointExtensions.AngleBetweenPoints(centerPoint, endPoint);
                endAngle = PointExtensions.AngleBetweenPoints(centerPoint, startPoint);
            }
            else
            {
                startAngle = PointExtensions.AngleBetweenPoints(centerPoint, startPoint);
                endAngle = PointExtensions.AngleBetweenPoints(centerPoint, endPoint);
            }

            Dictionary<string, object> dict = new Dictionary<string, object>
            {
                {"centerPoint", centerPoint },
                {"startAngle", startAngle },
                {"endAngle", endAngle },
                {"radius", Math.Abs(radius) }
            };
            return dict;
        }

    }
}
