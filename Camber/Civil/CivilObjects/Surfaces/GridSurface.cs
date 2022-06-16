using Autodesk.DesignScript.Runtime;
using Camber.Civil.Styles.Objects;
using DynamoServices;
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Geometry;
using Camber.Utilities.GeometryConversions;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using AeccGridSurface = Autodesk.Civil.DatabaseServices.GridSurface;
using civDb = Autodesk.Civil.DatabaseServices;
using civDynNodes = Autodesk.Civil.DynamoNodes;

namespace Camber.Civil.CivilObjects.Surfaces
{
    public sealed class GridSurface : CivilObject, ICamberSurface
    {
        #region properties
        internal AeccGridSurface AeccGridSurface => AcObject as AeccGridSurface;

        /// <summary>
        /// Gets the gridlines of a Grid Surface.
        /// </summary>
        public List<PolyCurve> Gridlines
        {
            get
            {
                List<PolyCurve> pcurves = new List<PolyCurve>();

                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    var oids = AeccGridSurface.ExtractGridded(Autodesk.Civil.SurfaceExtractionSettingsType.Plan);
                    foreach (acDb.ObjectId oid in oids)
                    {
                        var pline = (acDb.Polyline3d)ctx.Transaction.GetObject(oid, acDb.OpenMode.ForRead);
                        pcurves.Add(GeometryConversions.AcPolyline3dToDynPolyCurve(pline));
                    }
                }
                return pcurves;
            }
        }
        #endregion

        #region constructors
        [SupressImportIntoVM]
        internal static GridSurface GetByObjectId(acDb.ObjectId gridSurfId)
            => CivilObjectSupport.Get<GridSurface, AeccGridSurface>
                (gridSurfId, (aeccGridSurface) => new GridSurface(aeccGridSurface));

        internal GridSurface(AeccGridSurface aeccGridSurface, bool isDynamoOwned = false) 
            : base(aeccGridSurface, isDynamoOwned)
        {

        }
        #endregion

        #region methods
        public override string ToString() => $"{nameof(GridSurface)}(Name = {Name})";

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
