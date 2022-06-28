using Autodesk.Civil;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Camber.Utilities.GeometryConversions;
using DSCore;
using Dynamo.Graph.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acGeom = Autodesk.AutoCAD.Geometry;
using AeccSurface = Autodesk.Civil.DatabaseServices.Surface;
using civDb = Autodesk.Civil.DatabaseServices;
using civDynNodes = Autodesk.Civil.DynamoNodes;

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

        #region action methods
        /// <summary>
        /// Performs an elevation analysis on a Surface. 
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="minElevations">List of minimum elevations in each range.</param>
        /// <param name="maxElevations">List of maximum elevations in each range.</param>
        /// <param name="colors">List of color schemes to assign to each elevation range.</param>
        /// <returns></returns>
        public static civDynNodes.Surface ElevationAnalysis(
            this civDynNodes.Surface surface, 
            List<double> minElevations, 
            List<double> maxElevations, 
            List<Color> colors) 
            => RunMinMaxAnalysis(surface, SurfaceMinMaxAnalysisType.Elevations, minElevations, maxElevations, colors);

        /// <summary>
        /// Performs a direction analysis on a Surface. 
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="minDirections">List of minimum directions (degrees) in each range.</param>
        /// <param name="maxDirections">List of maximum directions (degrees) in each range.</param>
        /// <param name="colors">List of color schemes to assign to each direction range.</param>
        /// <returns></returns>
        public static civDynNodes.Surface DirectionAnalysis(
            this civDynNodes.Surface surface,
            List<double> minDirections,
            List<double> maxDirections,
            List<Color> colors)
            => RunMinMaxAnalysis(surface, SurfaceMinMaxAnalysisType.Directions, minDirections, maxDirections, colors);

        /// <summary>
        /// Performs a slope analysis on a Surface. 
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="minSlopes">List of minimum slopes (percent) in each range.</param>
        /// <param name="maxSlopes">List of minimum slopes (percent) in each range.</param>
        /// <param name="colors">List of color schemes to assign to each slope range.</param>
        /// <returns></returns>
        public static civDynNodes.Surface SlopeAnalysis(
            this civDynNodes.Surface surface,
            List<double> minSlopes,
            List<double> maxSlopes,
            List<Color> colors)
            => RunMinMaxAnalysis(surface, SurfaceMinMaxAnalysisType.Slopes, minSlopes, maxSlopes, colors);

        /// <summary>
        /// Performs a slope  arrow analysis on a Surface.
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="minSlopes">List of minimum slopes (percent) in each range.</param>
        /// <param name="maxSlopes">List of minimum slopes (percent) in each range.</param>
        /// <param name="colors">List of color schemes to assign to each slope range.</param>
        /// <returns></returns>
        public static civDynNodes.Surface SlopeArrowAnalysis(
            this civDynNodes.Surface surface,
            List<double> minSlopes,
            List<double> maxSlopes,
            List<Color> colors)
            => RunMinMaxAnalysis(surface, SurfaceMinMaxAnalysisType.SlopeArrows, minSlopes, maxSlopes, colors);

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
        /// Creates one or more PolyCurves that represent the flow of water along a Surface from a given start point.
        /// If the point is on a peak, multiple curves are created. If the location is on a flat area, no curves are created. Otherwise, only one curve is created.
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="startPoint"></param>
        /// <param name="create3DCurves">True = create 3D PolyCurves, False = create 2D PolyCurves</param>
        /// <returns></returns>
        public static List<PolyCurve> CreateWaterDrop(
            this civDynNodes.Surface surface,
            Point startPoint,
            bool create3DCurves)
        {
            List<PolyCurve> pcurves = new List<PolyCurve>();

            try
            {
                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    AeccSurface aeccSurf = surface.GetAeccSurface(acDb.OpenMode.ForRead);

                    acGeom.Point2d location = (acGeom.Point2d)GeometryConversions.DynPointToAcPoint(startPoint, false);

                    var dropType = WaterdropObjectType.Polyline2D;
                    if (create3DCurves)
                    {
                        dropType = WaterdropObjectType.Polyline3D;
                    }

                    acDb.ObjectIdCollection plineIds = aeccSurf.Analysis.CreateWaterdrop(location, dropType);

                    foreach (acDb.ObjectId plineId in plineIds)
                    {
                        List<Point> dynPnts = new List<Point>();
                        if (dropType == WaterdropObjectType.Polyline2D)
                        {
                            var pline = (acDb.Polyline)ctx.Transaction.GetObject(plineId, acDb.OpenMode.ForWrite);
                            for (int i = 0; i < pline.NumberOfVertices; i++)
                            {
                                dynPnts.Add(GeometryConversions.AcPointToDynPoint(pline.GetPoint3dAt(i)));
                            }
                            pline.Erase();
                        }
                        else
                        {
                            var pline3d = (acDb.Polyline3d)ctx.Transaction.GetObject(plineId, acDb.OpenMode.ForWrite);
                            foreach (acDb.ObjectId vertexId in pline3d)
                            {
                                var vertex = (acDb.PolylineVertex3d) ctx.Transaction.GetObject(
                                        vertexId,
                                        acDb.OpenMode.ForRead);
                                dynPnts.Add(GeometryConversions.AcPointToDynPoint(vertex.Position));
                            }
                            pline3d.Erase();
                        }
                        pcurves.Add(PolyCurve.ByPoints(dynPnts));
                    }
                }
                return pcurves;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Calculates the volume between a Surface and a datum elevation within an area defined by a Polygon.
        /// </summary>
        /// <param name="surface"></param>
        /// <param name="polygon"></param>
        /// <param name="datumElevation"></param>
        /// <returns></returns>
        [MultiReturn(new[] { "Cut Volume", "Fill Volume", "Net Volume" })]
        public static Dictionary<string, object> GetBoundedVolumes(
            this civDynNodes.Surface surface,
            Polygon polygon,
            double datumElevation)
        {
            try
            {
                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    acGeom.Point3dCollection pnts = GeometryConversions.DynPolygonToAcPoint3dCollection(polygon, true);
                    var aeccSurf = surface.GetAeccSurface(acDb.OpenMode.ForRead);
                    var volInfo = aeccSurf.GetBoundedVolumes(pnts, datumElevation);
                    return new Dictionary<string, object>
                    {
                        {"Cut Volume", volInfo.Cut},
                        {"Fill Volume", volInfo.Fill},
                        {"Net Volume", volInfo.Net}
                    };
                }
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

        private static civDynNodes.Surface RunMinMaxAnalysis(
            civDynNodes.Surface surface,
            SurfaceMinMaxAnalysisType analysisType,
            List<double> minValues,
            List<double> maxValues,
            List<Color> colors)
        {
            // Check list lengths
            if (new IList[] { minValues, maxValues, colors }.All(list => list.Count != minValues.Count))
            {
                throw new InvalidOperationException("All lists must have the same number of items.");
            }

            try
            {
                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    AeccSurface aeccSurf = surface.GetAeccSurface(acDb.OpenMode.ForWrite);

                    int steps = minValues.Count;

                    switch (analysisType)
                    {
                        case SurfaceMinMaxAnalysisType.Directions:
                            civDb.SurfaceAnalysisDirectionData[] directionData = new civDb.SurfaceAnalysisDirectionData[steps];
                            for (var i = 0; i < steps; i++)
                            {
                                Autodesk.AutoCAD.Colors.Color acColor = Autodesk.AutoCAD.Colors.Color.FromRgb(
                                    colors[i].Red,
                                    colors[i].Green,
                                    colors[i].Blue);
                                directionData[i] = new civDb.SurfaceAnalysisDirectionData(
                                    Utilities.MathUtilities.DegreesToRadians(minValues[i]),
                                    Utilities.MathUtilities.DegreesToRadians(maxValues[i]),
                                    acColor);
                            }
                            aeccSurf.Analysis.SetDirectionData(directionData);
                            break;
                        case SurfaceMinMaxAnalysisType.Elevations:
                            civDb.SurfaceAnalysisElevationData[] elevationData = new civDb.SurfaceAnalysisElevationData[steps];
                            for (var i = 0; i < steps; i++)
                            {
                                Autodesk.AutoCAD.Colors.Color acColor = Autodesk.AutoCAD.Colors.Color.FromRgb(
                                    colors[i].Red,
                                    colors[i].Green,
                                    colors[i].Blue);
                                elevationData[i] = new civDb.SurfaceAnalysisElevationData(
                                    minValues[i],
                                    maxValues[i],
                                    acColor);
                            }
                            aeccSurf.Analysis.SetElevationData(elevationData);
                            break;
                        case SurfaceMinMaxAnalysisType.SlopeArrows:
                            civDb.SurfaceAnalysisSlopeArrowData[] arrowData = new civDb.SurfaceAnalysisSlopeArrowData[steps];
                            for (var i = 0; i < steps; i++)
                            {
                                Autodesk.AutoCAD.Colors.Color acColor = Autodesk.AutoCAD.Colors.Color.FromRgb(
                                    colors[i].Red,
                                    colors[i].Green,
                                    colors[i].Blue);
                                arrowData[i] = new civDb.SurfaceAnalysisSlopeArrowData(
                                    minValues[i],
                                    maxValues[i],
                                    acColor);
                            }
                            aeccSurf.Analysis.SetSlopeArrowData(arrowData);
                            break;
                        case SurfaceMinMaxAnalysisType.Slopes:
                            civDb.SurfaceAnalysisSlopeData[] slopeData = new civDb.SurfaceAnalysisSlopeData[steps];
                            for (var i = 0; i < steps; i++)
                            {
                                Autodesk.AutoCAD.Colors.Color acColor = Autodesk.AutoCAD.Colors.Color.FromRgb(
                                    colors[i].Red,
                                    colors[i].Green,
                                    colors[i].Blue);
                                slopeData[i] = new civDb.SurfaceAnalysisSlopeData(
                                    minValues[i],
                                    maxValues[i],
                                    acColor);
                            }
                            aeccSurf.Analysis.SetSlopeData(slopeData);
                            break;
                    }
                }
                return surface;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
        #endregion

        [IsVisibleInDynamoLibrary(false)]
        public enum SurfaceMinMaxAnalysisType
        {
            Directions,
            Elevations,
            Slopes,
            SlopeArrows
        }
    }
}
