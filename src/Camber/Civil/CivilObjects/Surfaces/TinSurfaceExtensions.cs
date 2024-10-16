﻿using Autodesk.DesignScript.Runtime;
using Camber.Civil.Styles.Objects;
using Dynamo.Graph.Nodes;
using System;
using System.Collections.Generic;
using System.IO;
using Autodesk.DesignScript.Geometry;
using Camber.Properties;
using Camber.Utilities.GeometryConversions;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using AeccTinSurface = Autodesk.Civil.DatabaseServices.TinSurface;
using civDb = Autodesk.Civil.DatabaseServices;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using acGeom = Autodesk.AutoCAD.Geometry;
using DynamoServices;

namespace Camber.Civil.CivilObjects.Surfaces
{
    public static class TinSurface
    {
        #region query methods
        #endregion

        #region create methods
        /// <summary>
        /// Creates a new TIN Surface by importing a TIN file.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="filePath">The full path to a .tin file.</param>
        /// <returns></returns>
        [NodeCategory("Create")]
        public static civDynNodes.Surface ImportFromTIN(acDynNodes.Document document, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new InvalidOperationException("File path is null or empty.");
            }

            if (Path.GetExtension(filePath).ToLower() != ".tin")
            {
                throw new InvalidOperationException("Invalid file type.");
            }
            
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    acDb.ObjectId surfId = AeccTinSurface.CreateFromTin(document.AcDocument.Database, filePath);
                    var surf = (AeccTinSurface) ctx.Transaction.GetObject(surfId, acDb.OpenMode.ForRead);
                    return civDynNodes.Selection.SurfaceByName(surf.Name, document);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Creates a new TIN Surface by importing from a LandXML file.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="newSurfaceName">The name to give the new TIN Surface</param>
        /// <param name="filePath">The full path to the LandXML file</param>
        /// <param name="surfaceNameInFile">The name of the TIN Surface as defined in the LandXML file</param>
        /// <returns></returns>
        [NodeCategory("Create")]
        // Leaving this node out because the API function will create an empty surface if the surface name isn't found in the LandXML, which is undesirable behavior.
        private static civDynNodes.Surface ImportFromLandXML(
            acDynNodes.Document document,
            string newSurfaceName,
            string filePath,
            string surfaceNameInFile)
        {
            if (string.IsNullOrEmpty(newSurfaceName))
            {
                throw new InvalidOperationException("New surface name is null or empty.");
            }

            if (string.IsNullOrEmpty(filePath))
            {
                throw new InvalidOperationException("The LandXML file path is null or empty.");
            }

            if (string.IsNullOrWhiteSpace(surfaceNameInFile))
            {
                throw new InvalidOperationException("The provided name for the surface in the LandXML file is null or empty.");
            }
            
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    AeccTinSurface.CreateFromLandXML(
                        ctx.Database,
                        newSurfaceName,
                        filePath,
                        surfaceNameInFile);

                    return civDynNodes.Selection.SurfaceByName(newSurfaceName, document);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
        #endregion

        #region action methods
        /// <summary>
        /// Pastes a Surface into a TIN Surface.
        /// </summary>
        /// <param name="surfaceToPasteInto">TIN Surface</param>
        /// <param name="surfaceToPaste">TIN Surface, Grid Surface, TIN Volume Surface, or Grid Volume Surface</param>
        /// <returns></returns>
        public static civDynNodes.Surface PasteSurface(
            this civDynNodes.TinSurface surfaceToPasteInto,
            civDynNodes.Surface surfaceToPaste)
        {
            try
            {
                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    var aeccSurf = (AeccTinSurface)surfaceToPasteInto.InternalDBObject;
                    aeccSurf.UpgradeOpen();
                    aeccSurf.PasteSurface(surfaceToPaste.InternalObjectId);
                    aeccSurf.DowngradeOpen();
                }
                return surfaceToPasteInto;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Adds DEM file data to a TIN Surface with custom null elevation and coordinate system information.
        /// If the DEM file coordinate system is different from the current coordinate system of the drawing, you can specify a coordinate system for the DEM file.
        /// The coordinate system you specify for the DEM file should match the data defined in the DEM file itself.
        /// An empty string input for the coordinate system code means that no transformation is needed.
        /// </summary>
        /// <param name="tinSurface"></param>
        /// <param name="filePath">The path to the DEM file</param>
        /// <param name="coordinateSystemCode">The coordinate system code to transform the data in the DEM file.</param>
        /// <param name="customNullElevation"></param>
        /// <returns></returns>
        public static civDynNodes.Surface AddDEMFile(
            this civDynNodes.TinSurface tinSurface, 
            string filePath,
            [DefaultArgument("null")] double? customNullElevation,
            string coordinateSystemCode = "")
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new InvalidOperationException("File path is null or empty.");
            }

            var ext = Path.GetExtension(filePath).ToLower();

            if (ext != ".dem" && ext != ".tif" && ext != ".asc" && ext != ".txt" && ext != ".adf")
            {
                throw new InvalidOperationException("Invalid file type.");
            }

            bool useCustomNullElevation = true;
            if (customNullElevation == null)
            {
                useCustomNullElevation = false;
                customNullElevation = 1000000000000000000000000.000;
            }

            try
            {
                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    var aeccSurf = (AeccTinSurface) tinSurface.InternalDBObject;
                    aeccSurf.UpgradeOpen();
                    aeccSurf.DEMFilesDefinition.AddDEMFile(
                        filePath,
                        coordinateSystemCode,
                        useCustomNullElevation,
                        (double)customNullElevation);
                    aeccSurf.DowngradeOpen();
                }
                return tinSurface;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Adds point data to a TIN Surface from Block References. 
        /// </summary>
        /// <param name="tinSurface"></param>
        /// <param name="blockReferences"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static civDynNodes.Surface AddBlocks(
            this civDynNodes.TinSurface tinSurface, 
            List<acDynNodes.BlockReference> blockReferences,
            string description = "")
        {
            acDb.ObjectIdCollection blkIds = new acDb.ObjectIdCollection();

            foreach (acDynNodes.BlockReference blkRef in blockReferences)
            {
                blkIds.Add(blkRef.InternalObjectId);
            }
            
            try
            {
                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    var aeccSurf = (AeccTinSurface)tinSurface.InternalDBObject;
                    aeccSurf.UpgradeOpen();
                    aeccSurf.DrawingObjectsDefinition.AddFromBlocks(blkIds, description);
                    aeccSurf.DowngradeOpen();
                }
                return tinSurface;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Adds point data to a TIN Surface from 3D Faces. 
        /// </summary>
        /// <param name="tinSurface"></param>
        /// <param name="faces"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static civDynNodes.Surface Add3DFaces(
            this civDynNodes.TinSurface tinSurface,
            List<acDynNodes.Face> faces,
            string description = "",
            bool maintainEdges = false
            )
        {
            acDb.ObjectIdCollection faceIds = new acDb.ObjectIdCollection();

            foreach (acDynNodes.Face face in faces)
            {
                faceIds.Add(face.InternalObjectId);
            }

            try
            {
                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    var aeccSurf = (AeccTinSurface)tinSurface.InternalDBObject;
                    aeccSurf.UpgradeOpen();
                    aeccSurf.DrawingObjectsDefinition.AddFrom3DFaces(faceIds, maintainEdges, description);
                    aeccSurf.DowngradeOpen();
                }
                return tinSurface;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Gets the triangles of a TIN Surface within a boundary.
        /// </summary>
        /// <param name="tinSurface"></param>
        /// <param name="polyline"></param>
        /// <returns></returns>
        public static List<PolyCurve> GetTrianglesWithinBoundary(
            this civDynNodes.TinSurface tinSurface,
            acDynNodes.Polyline polyline
        )
        {
            try
            {
                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    var aeccSurf = (AeccTinSurface)tinSurface.InternalDBObject;
                    var plineIds = new acDb.ObjectIdCollection(new[] { polyline.InternalObjectId });
                    var verts = aeccSurf.GetVerticesInsidePolylines(plineIds);

                    return GetConnectedTriangles(verts);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Gets the triangles of a TIN Surface within a boundary.
        /// </summary>
        /// <param name="tinSurface"></param>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static List<PolyCurve> GetTrianglesWithinBoundary(
            this civDynNodes.TinSurface tinSurface,
            Polygon polygon
        )
        {
            if (polygon.SelfIntersections().Length > 0)
            {
                throw new ArgumentException("Boundary Polygon cannot have self-intersections.");
            }

            acGeom.Point3dCollection bndyPnts = GeometryConversions.DynPolygonToAcPoint3dCollection(polygon, true);
            List<PolyCurve> pcurves = new List<PolyCurve>();

            try
            {
                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    var aeccSurf = (AeccTinSurface)tinSurface.InternalDBObject;
                    var verts = aeccSurf.GetVerticesInsideBorder(bndyPnts);

                    return GetConnectedTriangles(verts);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
        #endregion

        #region private methods
        /// <summary>
        /// Gets all triangles connected to a TIN Surface vertex.
        /// </summary>
        /// <param name="verts"></param>
        /// <returns></returns>
        private static List<PolyCurve> GetConnectedTriangles(civDb.TinSurfaceVertex[] verts)
        {
            List<PolyCurve> pcurves = new List<PolyCurve>();

            foreach (civDb.TinSurfaceVertex vert in verts)
            {
                foreach (civDb.TinSurfaceTriangle tri in vert.Triangles)
                {
                    List<Point> pnts = new List<Point>()
                    {
                        GeometryConversions.AcPointToDynPoint(tri.Vertex1.Location),
                        GeometryConversions.AcPointToDynPoint(tri.Vertex2.Location),
                        GeometryConversions.AcPointToDynPoint(tri.Vertex3.Location)
                    };
                    pcurves.Add(PolyCurve.ByPoints(pnts, true));
                }
            }
            return pcurves;
        }
        #endregion

        #region obsolete
        /// <summary>
        /// Creates a new empty TIN Surface by name and style.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="name"></param>
        /// <param name="surfaceStyle"></param>
        /// <returns></returns>
        [NodeCategory("Create")]
        public static civDynNodes.Surface ByName(
            acDynNodes.Document document,
            string name,
            SurfaceStyle surfaceStyle)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_OBSOLETE_MESSAGE, "TinSurface.ByName"));

            if (civDynNodes.Selection.SurfaceByName(name, document) != null)
            {
                throw new InvalidOperationException("A TIN Surface with the same name already exists.");
            }

            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    AeccTinSurface.Create(name, surfaceStyle.InternalObjectId);
                    return civDynNodes.Selection.SurfaceByName(name, document);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
        #endregion
    }
}
