using Autodesk.DesignScript.Geometry;
using Camber.Utilities.GeometryConversions;
using Dynamo.Graph.Nodes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using AcPolyline3d = Autodesk.AutoCAD.DatabaseServices.Polyline3d;

namespace Camber.AutoCAD.Objects
{
    public static class Polyline3D
    {
        #region query methods
        /// <summary>
        /// Gets the vertices of a 3D Polyline.
        /// </summary>
        /// <param name="polyline3d"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static List<Point> Vertices(this acDynNodes.Polyline3D polyline3d)
        {
            try
            {
                var points = new List<Point>();
                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    var acPline = (AcPolyline3d)ctx.Transaction.GetObject(polyline3d.InternalObjectId, acDb.OpenMode.ForWrite);

                    foreach (acDb.ObjectId oid in acPline)
                    {
                        var vertex = (acDb.PolylineVertex3d)ctx.Transaction.GetObject(oid, acDb.OpenMode.ForRead);
                        points.Add(GeometryConversions.AcPointToDynPoint(vertex.Position));
                    }
                }
                return points;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Gets the number of duplicate vertices in a 3D Polyline.
        ///  For example, if there are 3 vertices at the same location, this node would return '2'.
        ///  If the 3D Polyline is closed, the common start/end vertex is not considered a duplicate.
        /// </summary>
        /// <param name="polyline3d"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static int DuplicateVerticesCount(this acDynNodes.Polyline3D polyline3d) => GetDuplicateVertices(polyline3d).Count;

        /// <summary>
        /// Gets whether a 3D Polyline is closed or not.
        /// </summary>
        /// <param name="polyline3d"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static bool IsClosed(this acDynNodes.Polyline3D polyline3d) => GetBool(polyline3d, "Closed");

        /// <summary>
        /// Gets the total length of a 3D Polyline.
        /// </summary>
        /// <param name="polyline3d"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static double Length(this acDynNodes.Polyline3D polyline3d) => GetDouble(polyline3d);
        #endregion

        #region action methods
        /// <summary>
        /// Removes duplicate vertices from a 3D Polyline. If the 3D Polyline is closed, the shared start/end vertex is not removed.
        /// </summary>
        /// <param name="polyline3d"></param>
        /// <param name="keepFirst">If true, the first duplicate vertex is preserved from each group of duplicates. If false, the last duplicate vertex is preserved.</param>
        /// <param name="tolerance">Two vertices are considered equal if the distance between them is less than the tolerance.</param>
        public static acDynNodes.Polyline3D PruneDuplicateVertices(this acDynNodes.Polyline3D polyline3d, bool keepFirst = true, double tolerance = 0.001)
        {
            try
            {
                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    var dupes = GetDuplicateVertices(polyline3d, keepFirst, tolerance);
                    foreach (var vert in dupes)
                    {
                        vert.Vertex3d.UpgradeOpen();
                        vert.Vertex3d.Erase();
                    }

                }
                return polyline3d;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Sets if a 3D Polyline is closed or not.
        /// </summary>
        /// <param name="polyline3d"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static acDynNodes.Polyline3D SetIsClosed(this acDynNodes.Polyline3D polyline3d, bool value) =>
            SetValue(polyline3d, value, "Closed");
        #endregion

        #region private methods
        /// <summary>
        /// Gets a list of the duplicate vertices for a 3D Polyline.
        /// </summary>
        /// <remarks>If the 3D Polyline is closed, the shared start/end vertex is not considered a duplicate.</remarks>
        /// <param name="polyline3d"></param>
        /// <param name="tolerance">Two vertices are considered equal if the distance between them is less than the tolerance.</param>
        /// <param name="excludeFirst">
        /// If true, the first duplicate vertex is not included in each group of duplicates.
        /// If false, the first vertex is included and the last vertex is excluded.
        /// </param>
        /// <returns></returns>
        private static List<PolylineVertex> GetDuplicateVertices(
            this acDynNodes.Polyline3D polyline3d,
            bool excludeFirst = true,
            double tolerance = 0.001)
        {
            using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
            {
                var allVerts = new List<PolylineVertex>();
                var acPline = (AcPolyline3d)ctx.Transaction.GetObject(polyline3d.InternalObjectId, acDb.OpenMode.ForRead);

                foreach (acDb.ObjectId oid in acPline)
                {
                    var vertex = (acDb.PolylineVertex3d)ctx.Transaction.GetObject(oid, acDb.OpenMode.ForRead);
                    allVerts.Add(new PolylineVertex(vertex));
                }

                return PolylineUtilities.FindDuplicateVertices(allVerts, excludeFirst, tolerance);
            }
        }
        #endregion

        #region helper methods
        internal static string GetString(
            acDynNodes.Polyline3D polyline3d,
            [CallerMemberName] string propertyName = null)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                try
                {
                    var acMText = (AcPolyline3d)ctx.Transaction.GetObject(
                        polyline3d.InternalObjectId,
                        acDb.OpenMode.ForRead);
                    PropertyInfo propInfo = acMText.GetType().GetProperty(
                        propertyName,
                        BindingFlags.Public | BindingFlags.Instance);
                    if (propInfo != null)
                    {
                        var value = propInfo.GetValue(acMText).ToString();
                        if (string.IsNullOrEmpty(value))
                        {
                            return null;
                        }
                        else
                        {
                            return value;
                        }
                    }
                }
                catch { }
                return "Not applicable.";
            }
        }

        internal static bool GetBool(
            acDynNodes.Polyline3D polyline3d,
            [CallerMemberName] string propertyName = null)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                try
                {
                    var acMText = (AcPolyline3d)ctx.Transaction.GetObject(
                        polyline3d.InternalObjectId,
                        acDb.OpenMode.ForRead);
                    PropertyInfo propInfo = acMText.GetType().GetProperty(
                        propertyName,
                        BindingFlags.Public | BindingFlags.Instance);
                    if (propInfo != null)
                    {
                        return (bool)propInfo.GetValue(acMText);
                    }
                }
                catch { }
                return false;
            }
        }

        internal static double GetDouble(
            acDynNodes.Polyline3D polyline3d,
            [CallerMemberName] string propertyName = null)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                try
                {
                    var acMText = (AcPolyline3d)ctx.Transaction.GetObject(
                        polyline3d.InternalObjectId,
                        acDb.OpenMode.ForRead);
                    PropertyInfo propInfo = acMText.GetType().GetProperty(
                        propertyName,
                        BindingFlags.Public | BindingFlags.Instance);
                    if (propInfo != null)
                    {
                        return (double)propInfo.GetValue(acMText);
                    }
                }
                catch { }
                return double.NaN;
            }
        }

        internal static acDynNodes.Polyline3D SetValue(
            acDynNodes.Polyline3D polyline3d,
            object value, [CallerMemberName]
            string methodName = null)
        {
            if (methodName.StartsWith("Set"))
            {
                methodName = methodName.Substring(3);
            }
            return SetValue(polyline3d, methodName, value);
        }

        internal static acDynNodes.Polyline3D SetValue(
            acDynNodes.Polyline3D polyline3d,
            string propertyName,
            object value)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                try
                {
                    var acMText = (AcPolyline3d)ctx.Transaction.GetObject(
                        polyline3d.InternalObjectId,
                        acDb.OpenMode.ForWrite);
                    PropertyInfo propInfo = acMText.GetType().GetProperty(
                        propertyName,
                        BindingFlags.Public | BindingFlags.Instance);
                    propInfo?.SetValue(acMText, value);
                    return polyline3d;
                }
                catch { throw; }
            }
        }
        #endregion
    }
}
