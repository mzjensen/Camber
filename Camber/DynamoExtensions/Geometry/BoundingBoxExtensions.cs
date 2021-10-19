#region references
using System;
using Autodesk.DesignScript.Geometry;
#endregion

namespace Camber.DynamoExtensions.GeometryExtensions
{
    public class BoundingBoxExtensions
    {
        #region constructors
        private BoundingBoxExtensions() { }
        #endregion

        #region methods
        /// <summary>
        /// Checks if a Bounding Box is fully contained within another Bounding Box.
        /// </summary>
        /// <param name="checkThisBox"></param>
        /// <param name="withinThisBox"></param>
        /// <returns></returns>
        public static bool IsFullyContained(BoundingBox checkThisBox, BoundingBox withinThisBox)
        {
            if (IsEqualTo(checkThisBox, withinThisBox))
            {
                throw new ArgumentException("The two Bounding Boxes are identical.");
            }

            Point minPoint = checkThisBox.MinPoint;
            Point maxPoint = checkThisBox.MaxPoint;

            if (withinThisBox.Contains(minPoint) && withinThisBox.Contains(maxPoint))
            {
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Checks if a Bounding Box is partially contained within another Bounding Box.
        /// Returns false if the Bounding Box is fully contained or not at all contained within the other Bounding Box.
        /// </summary>
        /// <param name="checkThisBox"></param>
        /// <param name="withinThisBox"></param>
        /// <returns></returns>
        public static bool IsPartiallyContained(BoundingBox checkThisBox, BoundingBox withinThisBox)
        {
            if (IsEqualTo(checkThisBox, withinThisBox))
            {
                throw new ArgumentException("The two Bounding Boxes are identical.");
            }

            Point minPoint = checkThisBox.MinPoint;
            Point maxPoint = checkThisBox.MaxPoint;

            return withinThisBox.Contains(minPoint) ^ withinThisBox.Contains(maxPoint);
        }

        /// <summary>
        /// Checks if two Bounding Boxes are identical.
        /// </summary>
        /// <param name="boundingBox"></param>
        /// <param name="otherBoundingBox"></param>
        /// <returns></returns>
        public static bool IsEqualTo(BoundingBox boundingBox, BoundingBox otherBoundingBox)
        {
            Point minPoint = boundingBox.MinPoint;
            Point maxPoint = boundingBox.MaxPoint;
            Point otherMinPoint = otherBoundingBox.MinPoint;
            Point otherMaxPoin = otherBoundingBox.MaxPoint;

            if (minPoint.IsAlmostEqualTo(otherMinPoint) && maxPoint.IsAlmostEqualTo(otherMaxPoin))
            {
                return true;
            }

            return false;
        }
        #endregion
    }
}
