using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.Civil;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Camber.Civil.Styles.Objects;
using Camber.Utilities.GeometryConversions;
using DynamoServices;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using AeccGridSurface = Autodesk.Civil.DatabaseServices.GridSurface;
using civDb = Autodesk.Civil.DatabaseServices;
using civDynNodes = Autodesk.Civil.DynamoNodes;

namespace Camber.Civil.CivilObjects.Surfaces
{
    [RegisterForTrace]
    public sealed class GridSurface : CivilObject, ICamberSurface
    {
        #region properties
        internal AeccGridSurface AeccGridSurface => AcObject as AeccGridSurface;

        /// <summary>
        /// Gets the gridlines of a Grid Surface.
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        public List<PolyCurve> Gridlines
        {
            get
            {
                List<PolyCurve> pcurves = new List<PolyCurve>();

                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    var oids = AeccGridSurface.ExtractGridded(SurfaceExtractionSettingsType.Plan);
                    foreach (acDb.ObjectId oid in oids)
                    {
                        var pline = (acDb.Polyline3d)ctx.Transaction.GetObject(oid, acDb.OpenMode.ForRead);
                        pcurves.Add(GeometryConversions.AcPolyline3dToDynPolyCurve(pline));
                    }
                }
                return pcurves;
            }
        }

        /// <summary>
        /// Gets the geometry of a Grid Surface as a Mesh.
        /// </summary>
        public Mesh Mesh
        {
            get
            {
                Mesh retMesh;
                try
                {
                    var indices = new List<IndexGroup>();
                    var vertexPositions = new List<Point>();
                    foreach (GridSurfaceCell cell in GetCells(true))
                    {
                        uint uint32 = Convert.ToUInt32(vertexPositions.Count);
                        vertexPositions.Add(cell.Vertices[0].Geometry);
                        vertexPositions.Add(cell.Vertices[1].Geometry);
                        vertexPositions.Add(cell.Vertices[2].Geometry);
                        vertexPositions.Add(cell.Vertices[3].Geometry);
                        IndexGroup indexGroup = IndexGroup.ByIndices(uint32, uint32 + 1U, uint32 + 2U, uint32 + 3U);
                        indices.Add(indexGroup);
                    }
                    retMesh = Mesh.ByPointsFaceIndices(vertexPositions, indices);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
                return retMesh;
            }
        }
        #endregion

        #region constructors
        [SupressImportIntoVM]
        internal static GridSurface GetByObjectId(acDb.ObjectId gridSurfId)
            => CivilObjectSupport.Get<GridSurface, AeccGridSurface>
                (gridSurfId, aeccGridSurface => new GridSurface(aeccGridSurface));

        internal GridSurface(AeccGridSurface aeccGridSurface, bool isDynamoOwned = false) 
            : base(aeccGridSurface, isDynamoOwned)
        {
        }

        /// <summary>
        /// Creates a new Grid Surface by name. The units for the X and Y spacing and orientation are taken from the settings in SettingsCmdCreateSurface.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="surfaceName"></param>
        /// <param name="spacingX"></param>
        /// <param name="spacingY"></param>
        /// <param name="orientation"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public static GridSurface ByName(
            acDynNodes.Document document,
            string surfaceName,
            double spacingX,
            double spacingY,
            double orientation,
            SurfaceStyle style)
        {
            if (string.IsNullOrEmpty(surfaceName))
            {
                throw new InvalidOperationException("Surface name is null or empty.");
            }

            if (spacingX <= 0 || spacingY <= 0)
            {
                throw new InvalidOperationException("The X and Y spacing must be greater than zero.");
            }

            bool hasSurfaceWithSameName = false;
            var res = CommonConstruct<GridSurface, AeccGridSurface>(
                document,
                ctx =>
                {
                    if (civDynNodes.Selection.Surfaces(document).Any(s => s.Name == surfaceName))
                    {
                        hasSurfaceWithSameName = true;
                        return null;
                    }

                    try
                    {
                        var surfId = AeccGridSurface.Create(
                            surfaceName,
                            spacingX,
                            spacingY,
                            orientation,
                            style.InternalObjectId);
                        return (AeccGridSurface)ctx.Transaction.GetObject(surfId, acDb.OpenMode.ForRead);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(ex.Message);
                    }
                },
                (ctx, surf, existing) =>
                {
                    if (existing)
                    {
                        if (surf.Name != surfaceName && civDynNodes.Selection.Surfaces(document).All(obj => obj.Name != surfaceName))
                        {
                            surf.Name = surfaceName;
                        }
                        else if (surf.Name != surfaceName && civDynNodes.Selection.Surfaces(document).Any(obj => obj.Name == surfaceName))
                        {
                            hasSurfaceWithSameName = true;
                            return false;
                        }
                        surf.StyleId = style.InternalObjectId;
                    }
                    return true;
                });
            if (hasSurfaceWithSameName)
            {
                throw new InvalidOperationException("A Surface with the same name already exists");
            }
            return res;
        }

        /// <summary>
        /// Creates a new Grid Surface by importing from a DEM file.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="demFilePath">The path to a .tif, .dem, .asc, .txt, or .adf file.</param>
        /// <param name="style"></param>
        /// <param name="surfaceName">The surface will take the name of the file unless otherwise specified.</param>
        /// <returns></returns>
        public static GridSurface ImportFromDEM(acDynNodes.Document document,
            string demFilePath,
            SurfaceStyle style,
            string surfaceName = "")
        {
            if (string.IsNullOrEmpty(demFilePath))
            {
                throw new InvalidOperationException("The DEM file path is null or empty.");
            }

            if (!File.Exists(demFilePath))
            {
                throw new InvalidOperationException("Invalid file path.");
            }

            var ext = Path.GetExtension(demFilePath);

            if (ext != ".tif" && ext != ".dem" && ext != ".asc" && ext != ".txt" && ext != ".adf")
            {
                throw new InvalidOperationException("The specified file is not an acceptable DEM file format.");
            }

            if (surfaceName == "")
            {
                surfaceName = Path.GetFileNameWithoutExtension(demFilePath);
            }

            if (civDynNodes.Selection.Surfaces(document).Any(s => s.Name == surfaceName))
            {
                throw new InvalidOperationException("A surface with the same name already exists.");
            }

            try
            {
                var surfId = AeccGridSurface.CreateFromDEM(demFilePath, style.InternalObjectId);
                var surf = GetByObjectId(surfId);
                surf.SetName(surfaceName);
                return surf;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"{nameof(GridSurface)}(Name = {Name})";

        /// <summary>
        /// Gets the cells of a Grid Surface.
        /// </summary>
        /// <param name="includeInvisible">Include invisible cells?</param>
        /// <returns></returns>
        public List<GridSurfaceCell> GetCells(bool includeInvisible = false)
        {
            List<GridSurfaceCell> cells = new List<GridSurfaceCell>();

            foreach (civDb.GridSurfaceCell cell in AeccGridSurface.GetCells(includeInvisible))
            {
                cells.Add(new GridSurfaceCell(cell));
            }
            return cells;
        }

        /// <summary>
        /// Gets the vertices of a Grid Surface.
        /// </summary>
        /// <param name="includeInvisible">Include invisible vertices?</param>
        /// <returns></returns>
        public List<GridSurfaceVertex> GetVertices(bool includeInvisible = false)
        {
            List<GridSurfaceVertex> vertices = new List<GridSurfaceVertex>();

            foreach (civDb.GridSurfaceVertex vertex in AeccGridSurface.GetVertices(includeInvisible))
            {
                vertices.Add(new GridSurfaceVertex(vertex));
            }
            return vertices;
        }

        /// <summary>
        /// "Converts" a Grid Surface to a more-generic Surface so it can be used with the out-of-the-box nodes for Surfaces.
        /// </summary>
        /// <returns></returns>
        public civDynNodes.Surface AsSurface() => civDynNodes.Selection.SurfaceByName(Name, acDynNodes.Document.Current);
        #endregion
    }
}
