using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Camber.Utilities.GeometryConversions;
using Dynamo.Graph.Nodes;
using DynamoServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Camber.Properties;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using AcPolyline = Autodesk.AutoCAD.DatabaseServices.Polyline;

namespace Camber.AutoCAD.Objects
{
    public static class Polyline
    {
        #region query methods
        /// <summary>
        /// Gets the vertices of a Polyline.
        /// </summary>
        /// <param name="polyline"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static List<Point> Vertices(this acDynNodes.Polyline polyline)
        {
            try
            {
                var vertices = new List<Point>();
                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    var acPline =
                        (AcPolyline) ctx.Transaction.GetObject(polyline.InternalObjectId, acDb.OpenMode.ForRead);

                    for (int i = 0; i < acPline.NumberOfVertices; i++)
                    {
                        vertices.Add(GeometryConversions.AcPointToDynPoint(acPline.GetPoint3dAt(i)));
                    }
                }
                return vertices;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Gets the number of duplicate vertices in a Polyline.
        ///  For example, if there are 3 vertices at the same location, this node would return 2.
        ///  If the Polyline is closed, the common start/end vertex is not considered a duplicate.
        /// </summary>
        /// <param name="polyline"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static int DuplicateVerticesCount(this acDynNodes.Polyline polyline) => polyline.GetDuplicateVertices().Count;

        /// <summary>
        /// Gets the total length of a Polyline.
        /// </summary>
        /// <param name="polyline"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static double Length(this acDynNodes.Polyline polyline) => GetDouble(polyline);
        #endregion

        #region action methods
        /// <summary>
        /// Removes duplicate vertices from a Polyline. If the Polyline is closed, the shared start/end vertex is not removed.
        /// </summary>
        /// <param name="polyline"></param>
        /// <param name="keepFirst">If true, the first duplicate vertex is preserved from each group of duplicates. If false, the last duplicate vertex is preserved.</param>
        /// <param name="tolerance">Two vertices are considered equal if the distance between them is less than the tolerance.</param>
        /// <returns></returns>
        public static acDynNodes.Polyline PruneDuplicateVertices(this acDynNodes.Polyline polyline, bool keepFirst = true, double tolerance = 0.001)
        {
            try
            {
                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    var acPline =
                        (AcPolyline) ctx.Transaction.GetObject(polyline.InternalObjectId, acDb.OpenMode.ForWrite);

                    var duplicatesToRemove = polyline.GetDuplicateVertices(keepFirst, tolerance);

                    int counter = 0;
                    foreach (var item in duplicatesToRemove)
                    {
                        acPline.RemoveVertexAt((item.Index - counter));
                        counter += 1;
                    }
                }
                return polyline;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
        #endregion

        #region private methods
        /// <summary>
        /// Gets a list of the duplicate vertices for a Polyline.
        /// </summary>
        /// <remarks>If the Polyline is closed, the shared start/end vertex is not considered a duplicate.</remarks>
        /// <param name="polyline"></param>
        /// <param name="tolerance">Two vertices are considered equal if the distance between them is less than the tolerance.</param>
        /// <param name="excludeFirst">
        /// If true, the first duplicate vertex is not included in each group of duplicates.
        /// If false, the first vertex is included and the last vertex is excluded.
        /// </param>
        /// <returns></returns>
        private static List<PolylineVertex> GetDuplicateVertices(
            this acDynNodes.Polyline polyline,
            bool excludeFirst = true,
            double tolerance = 0.001)
        {
            using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
            {
                var acPline = (AcPolyline)ctx.Transaction.GetObject(polyline.InternalObjectId, acDb.OpenMode.ForRead);
                var allVerts = new List<PolylineVertex>();

                for (int i = 0; i < acPline.NumberOfVertices; i++)
                {
                    allVerts.Add(new PolylineVertex(i, acPline.GetPoint3dAt(i)));
                }

                return PolylineUtilities.FindDuplicateVertices(allVerts, excludeFirst, tolerance);
            }
        }
        #endregion

        #region helper methods
        internal static string GetString(
            acDynNodes.Polyline polyline,
            [CallerMemberName] string propertyName = null)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new Autodesk.AutoCAD.DynamoApp.Services.DocumentContext(document.AcDocument))
            {
                try
                {
                    var acMText = (AcPolyline)ctx.Transaction.GetObject(
                        polyline.InternalObjectId,
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
            acDynNodes.Polyline polyline,
            [CallerMemberName] string propertyName = null)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new Autodesk.AutoCAD.DynamoApp.Services.DocumentContext(document.AcDocument))
            {
                try
                {
                    var acMText = (AcPolyline)ctx.Transaction.GetObject(
                        polyline.InternalObjectId,
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
            acDynNodes.Polyline polyline,
            [CallerMemberName] string propertyName = null)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new Autodesk.AutoCAD.DynamoApp.Services.DocumentContext(document.AcDocument))
            {
                try
                {
                    var acMText = (AcPolyline)ctx.Transaction.GetObject(
                        polyline.InternalObjectId,
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

        internal static acDynNodes.Polyline SetValue(
            acDynNodes.Polyline polyline,
            object value, [CallerMemberName]
            string methodName = null)
        {
            if (methodName.StartsWith("Set"))
            {
                methodName = methodName.Substring(3);
            }
            return SetValue(polyline, methodName, value);
        }

        internal static acDynNodes.Polyline SetValue(
            acDynNodes.Polyline polyline,
            string propertyName,
            object value)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new Autodesk.AutoCAD.DynamoApp.Services.DocumentContext(document.AcDocument))
            {
                try
                {
                    var acMText = (AcPolyline)ctx.Transaction.GetObject(
                        polyline.InternalObjectId,
                        acDb.OpenMode.ForWrite);
                    PropertyInfo propInfo = acMText.GetType().GetProperty(
                        propertyName,
                        BindingFlags.Public | BindingFlags.Instance);
                    propInfo?.SetValue(acMText, value);
                    return polyline;
                }
                catch { throw; }
            }
        }
        #endregion

        #region deprecated
        /// <summary>
        /// Gets the elevation of a Polyline measured from the world XY plane.
        /// </summary>
        /// <param name="polyline"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        [NodeMigrationMapping(
            "Camber.AutoCAD.Objects.Polyline.Elevation",
            "Autodesk.AutoCAD.DynamoNodes.Polyline.Elevation")]
        [NodeCategory("Query")]
        public static double Elevation(this acDynNodes.Polyline polyline)
        {
            LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Polyline.Elevation"));
            return GetDouble(polyline);
        }

        /// <summary>
        /// Gets the global width of a Polyline.
        /// </summary>
        /// <param name="polyline"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        [NodeMigrationMapping(
            "Camber.AutoCAD.Objects.Polyline.GlobalWidth",
            "Autodesk.AutoCAD.DynamoNodes.Polyline.GlobalWidth")]
        [NodeCategory("Query")]
        public static double GlobalWidth(this acDynNodes.Polyline polyline)
        {
            LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Polyline.GlobalWidth"));
            return GetDouble(polyline, "ConstantWidth");
        }

        /// <summary>
        /// Gets whether a Polyline is closed or not.
        /// </summary>
        /// <param name="polyline"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        [NodeMigrationMapping(
            "Camber.AutoCAD.Objects.Polyline.IsClosed",
            "Autodesk.AutoCAD.DynamoNodes.Curve.IsClosed")]
        [NodeCategory("Query")]
        public static bool IsClosed(this acDynNodes.Polyline polyline)
        {
            LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Curve.IsClosed"));
            return GetBool(polyline, "Closed");
        }

        /// <summary>
        /// Sets the elevation of a Polyline measured from the world XY plane.
        /// </summary>
        /// <param name="polyline"></param>
        /// <param name="elevation"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        [NodeMigrationMapping(
            "Camber.AutoCAD.Objects.Polyline.SetElevation",
            "Autodesk.AutoCAD.DynamoNodes.Polyline.SetElevation")]
        public static acDynNodes.Polyline SetElevation(this acDynNodes.Polyline polyline, double elevation)
        {
            LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Polyline.SetElevation"));
            return SetValue(polyline, elevation);
        }

        /// <summary>
        /// Sets the global width of a Polyline.
        /// </summary>
        /// <param name="polyline"></param>
        /// <param name="width">A positive width value.</param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        [NodeMigrationMapping(
            "Camber.AutoCAD.Objects.Polyline.SetGlobalWidth",
            "Autodesk.AutoCAD.DynamoNodes.Polyline.SetGlobalWidth")]
        public static acDynNodes.Polyline SetGlobalWidth(this acDynNodes.Polyline polyline, double width)
        {
            LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Polyline.SetGlobalWidth"));

            if (width < 0)
            {
                throw new InvalidOperationException("Input width must be positive.");
            }

            return SetValue(polyline, width, "ConstantWidth");
        }

        /// <summary>
        /// Sets whether a Polyline is closed or not.
        /// </summary>
        /// <param name="polyline"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        [NodeMigrationMapping(
            "Camber.AutoCAD.Objects.Polyline.SetIsClosed",
            "Autodesk.AutoCAD.DynamoNodes.Curve.SetClosed")]
        public static acDynNodes.Polyline SetIsClosed(this acDynNodes.Polyline polyline, bool value)
        {
            LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Curve.SetClosed"));
            return SetValue(polyline, value, "Closed");
        }
        #endregion
    }
}
