#region references
using Autodesk.DesignScript.Geometry;
#endregion

namespace Camber.DynamoExtensions.GeometryExtensions
{
    public class PointExtensions
    {
        #region constructors
        private PointExtensions() { }
        #endregion

        #region methods
        /// <summary>
        /// Measures the angle (in degrees) between two points. The angle is measured from the world X axis with angles increasing in the counterclockwise direction.
        /// If 3D points are supplied, they are projected onto the world XY plane. 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double AngleBetweenPoints(Point p1, Point p2)
        {
            var p1Flat = Point.ByCoordinates(p1.X, p1.Y);
            var p2Flat = Point.ByCoordinates(p2.X, p2.Y);
            var vector = Vector.ByTwoPoints(p1Flat, p2Flat);
            var xAxis = Vector.XAxis();
            var zAxis = Vector.ZAxis();

            return vector.AngleAboutAxis(xAxis, zAxis.Reverse());
        }
        #endregion
    }
}
