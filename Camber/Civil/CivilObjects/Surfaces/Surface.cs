using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Civil;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using civDb = Autodesk.Civil.DatabaseServices;
using Autodesk.DesignScript.Runtime;
using Autodesk.DesignScript.Geometry;
using acGeom = Autodesk.AutoCAD.Geometry;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using Camber.Utilities.GeometryConversions;
using Dynamo.Graph.Nodes;
using AeccSurface = Autodesk.Civil.DatabaseServices.Surface;

namespace Camber.Civil.CivilObjects.Surfaces
{
    public static class Surface
    {
        #region query methods
        /// <summary>
        /// Gets whether a Surface is automatically rebuilt when its definition is changed.
        /// </summary>
        /// <param name="surface"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static bool AutoRebuild(this civDynNodes.Surface surface) => surface.GetBoolProperty();

        /// <summary>
        /// Gets a boolean value that specifies whether a Surface has a snapshot.
        /// </summary>
        /// <param name="surface"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static bool HasSnapshot(this civDynNodes.Surface surface) => surface.GetBoolProperty();

        /// <summary>
        /// Gets whether a Surface is out-of-date.
        /// </summary>
        /// <param name="surface"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static bool IsOutOfDate(this civDynNodes.Surface surface) => surface.GetBoolProperty();

        /// <summary>
        /// Gets whether the snapshot of a Surface is out-of-date.
        /// </summary>
        /// <param name="surface"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static bool IsSnapshotOutOfDate(this civDynNodes.Surface surface) => surface.GetBoolProperty();

        /// <summary>
        /// Gets whether the Civil 3D GUI shows a Surface as locked.
        /// </summary>
        /// <param name="surface"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static bool IsLocked(this civDynNodes.Surface surface) => surface.GetBoolProperty("Lock");
        #endregion

        #region create methods
        #endregion

        #region action methods
        /// <summary>
        /// Sets whether to automatically rebuild a Surface when its definition is changed.
        /// </summary>
        /// <param name="surface"></param>
        /// <returns></returns>
        public static civDynNodes.Surface SetAutoRebuild(this civDynNodes.Surface surface, bool @bool) =>
            surface.SetProperty(@bool);

        /// <summary>
        /// Sets whether the Civil 3D GUI shows a Surface as locked.
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="bool"></param>
        /// <returns></returns>
        public static civDynNodes.Surface SetIsLocked(this civDynNodes.Surface surface, bool @bool) => 
            surface.SetProperty(@bool, "Lock");

        /// <summary>
        /// Creates one or more Polylines that represent the flow of water along a Surface from a given start startPoint.
        /// If the location is on a peak, multiple Polylines are created.
        /// If the location is on a flat area, no objects are created.
        /// Otherwise, only one Polyline is created.
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="startPoint"></param>
        /// <param name="create3DPolylines">Create 3D Polylines or 2D Polylines?</param>
        /// <param name="layer">If blank, the Polyline(s) will be created on the same layer as the Surface</param>
        /// <returns></returns>
        public static List<acDynNodes.Object> CreateWaterDrop(
            this civDynNodes.Surface surface,
            Point startPoint,
            bool create3DPolylines,
            string layer = "")
        {
            List<acDynNodes.Object> dynObjs = new List<acDynNodes.Object>();

            try
            {
                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    AeccSurface aeccSurf = surface.GetAeccSurface(acDb.OpenMode.ForRead);
                    
                    acGeom.Point2d location = (acGeom.Point2d)GeometryConversions.DynPointToAcPoint(startPoint, false);
                    
                    var dropType = WaterdropObjectType.Polyline2D;
                    if (create3DPolylines)
                    {
                        dropType = WaterdropObjectType.Polyline3D;
                    }

                    acDb.ObjectIdCollection oids = aeccSurf.Analysis.CreateWaterdrop(location, dropType);
                    
                    foreach (acDb.ObjectId oid in oids)
                    {
                        var acObj = (acDb.Entity) ctx.Transaction.GetObject(oid, acDb.OpenMode.ForWrite);
                        if (!string.IsNullOrEmpty(layer))
                        {
                            acDynNodes.AutoCADUtility.EnsureLayer(ctx, layer);
                            acObj.Layer = layer;
                        }

                        dynObjs.Add(acDynNodes.SelectionByQuery.GetObjectByObjectHandle(oid.Handle.ToString()));
                    }
                }
                return dynObjs;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
        #endregion

        #region internal methods
        internal static AeccSurface GetAeccSurface(this civDynNodes.Surface surface, acDb.OpenMode openMode)
        {
            using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
            {
                var oid = surface.InternalObjectId;
                return (AeccSurface) ctx.Transaction.GetObject(oid, openMode);
            }
        }

        /// <summary>
        /// Gets a property of type double from the wrapped surface object.
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        /// <remarks>
        /// It is the responsibility of the caller to use this method with the correct type.
        /// </remarks>
        internal static double GetDoubleProperty(
            this civDynNodes.Surface surface,
            [CallerMemberName] string propertyName = null)
        {
            try
            {
                AeccSurface aeccSurface = surface.GetAeccSurface(acDb.OpenMode.ForRead);
                PropertyInfo propInfo = aeccSurface
                    .GetType()
                    .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (propInfo != null)
                {
                    return (double)propInfo.GetValue(aeccSurface);
                }
                return double.NaN;
            }
            catch
            {
                throw new InvalidOperationException($"Unable to get property \"{propertyName}\".");
            }
        }

        /// <inheritdoc cref="GetDoubleProperty"/>
        /// <summary>
        /// Gets a property of type int from the wrapped surface object.
        /// </summary>
        internal static int GetIntProperty(
            this civDynNodes.Surface surface,
            [CallerMemberName] string propertyName = null)
        {
            try
            {
                AeccSurface aeccSurface = surface.GetAeccSurface(acDb.OpenMode.ForRead);
                PropertyInfo propInfo = aeccSurface
                    .GetType()
                    .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (propInfo != null)
                {
                    return (int)propInfo.GetValue(aeccSurface);
                }
                return int.MinValue;
            }
            catch
            {
                throw new InvalidOperationException($"Unable to get property \"{propertyName}\".");
            }
        }

        /// <inheritdoc cref="GetDoubleProperty"/>
        /// <summary>
        /// Gets a property of type string from the wrapped surface object.
        /// </summary>
        internal static string GetStringProperty(
            this civDynNodes.Surface surface,
            [CallerMemberName] string propertyName = null)
        {
            try
            {
                AeccSurface aeccSurface = surface.GetAeccSurface(acDb.OpenMode.ForRead);
                PropertyInfo propInfo = aeccSurface
                    .GetType()
                    .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (propInfo != null)
                {
                    var value = propInfo.GetValue(aeccSurface).ToString();
                    if (string.IsNullOrEmpty(value))
                    {
                        return null;
                    }
                    return value;
                }
                return null;
            }
            catch
            {
                throw new InvalidOperationException($"Unable to get property \"{propertyName}\".");
            }
        }

        /// <inheritdoc cref="GetDoubleProperty"/>
        /// <summary>
        /// Gets a property of type bool from the wrapped surface object.
        /// </summary>
        internal static bool GetBoolProperty(
            this civDynNodes.Surface surface, 
            [CallerMemberName] string propertyName = null)
        {
            try
            {
                AeccSurface aeccSurface = surface.GetAeccSurface(acDb.OpenMode.ForRead);
                PropertyInfo propInfo = aeccSurface
                    .GetType()
                    .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

                return (bool)propInfo.GetValue(aeccSurface);
            }
            catch
            {
                throw new InvalidOperationException($"Unable to get property \"{propertyName}\".");
            }
        }

        /// <summary>
        /// Sets a property value for the wrapped surface object.
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        internal static civDynNodes.Surface SetProperty(
            this civDynNodes.Surface surface,
            object value,
            [CallerMemberName] string propertyName = null)
        {
            if (propertyName.StartsWith("Set"))
            {
                propertyName = propertyName.Substring(3);
            }
            var success = SetPropertyValue(surface, value, propertyName);
            if (!success)
            {
                throw new InvalidOperationException($"Unable to set property \"{propertyName}\".");
            }
            return surface;
        }

        #endregion

        #region private methods
        private static bool SetPropertyValue(
            civDynNodes.Surface surface, 
            object value, 
            string propertyName)
        {
            try
            {
                AeccSurface aeccSurf = surface.GetAeccSurface(acDb.OpenMode.ForWrite);
                PropertyInfo propInfo = aeccSurf.GetType()
                    .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                propInfo?.SetValue(aeccSurf, value);
                aeccSurf.DowngradeOpen();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
