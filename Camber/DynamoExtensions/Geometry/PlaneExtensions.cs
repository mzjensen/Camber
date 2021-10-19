#region references
using Autodesk.DesignScript.Geometry;
#endregion

namespace Camber.DynamoExtensions.GeometryExtensions
{
    public class PlaneExtensions
    {
        #region constructors
        private PlaneExtensions() { }
        #endregion

        #region methods
        /// <summary>
        /// Checks to see if geometry lies in front of or behind a plane, with the front side being in the direction of the plane normal.
        /// Returns True for in front, False for behind, and Null if the geometry intersects with the plane.
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static bool? SideOfPlane(Geometry geometry, Plane plane)
        {
            if (geometry.DoesIntersect(plane) == false)
            {
                var ptOnPlane = plane.ClosestPointTo(geometry);
                var ptOnGeom = geometry.ClosestPointTo(plane);
                var vec = Vector.ByTwoPoints(ptOnPlane, ptOnGeom);
                var ang = vec.AngleAboutAxis(plane.Normal, plane.YAxis);
                if (ang == 180)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}
