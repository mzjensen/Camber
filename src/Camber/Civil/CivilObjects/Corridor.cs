#region references
using Camber.Civil.Intersections;
using Camber.Civil.Styles.Objects;
using Dynamo.Graph.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using AeccCorridor = Autodesk.Civil.DatabaseServices.Corridor;
using civDb = Autodesk.Civil.DatabaseServices;
using civDynNodes = Autodesk.Civil.DynamoNodes;
#endregion

namespace Camber.Civil.CivilObjects
{
    public static class Corridor
    {
        /// <summary>
        /// Gets a Dynamo-wrapped Corridor by Object ID.
        /// </summary>
        /// <param name="corrId"></param>
        /// <returns></returns>
        internal static civDynNodes.Corridor GetFromObjectId(acDb.ObjectId corrId)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            try
            {
                using (acDynApp.DocumentContext ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccCorr = ctx.Transaction.GetObject(corrId, acDb.OpenMode.ForWrite) as AeccCorridor;
                    return civDynNodes.Selection.CorridorByName(aeccCorr.Name, document);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        #region query methods
        /// <summary>
        /// Gets all of the Intersections that have Corridor Regions within a Corridor.
        /// </summary>
        /// <param name="corridor"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static IList<Intersection> Intersections(civDynNodes.Corridor corridor)
        {
            var intxs = Intersection.GetIntersections(acDynNodes.Document.Current);
            return intxs.Where(intx => intx.Corridor.Name == corridor.Name).ToList();
        }

        /// <summary>
        /// Gets the Corridor Surface associated with a Corridor.
        /// </summary>
        /// <param name="corridor"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static IList<CorridorSurface> CorridorSurfaces(civDynNodes.Corridor corridor)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            List<CorridorSurface> surfaces = new List<CorridorSurface>(); 

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                AeccCorridor aeccCorr = (AeccCorridor)ctx.Transaction.GetObject(
                    corridor.InternalObjectId,
                    acDb.OpenMode.ForRead);
                foreach (civDb.CorridorSurface surf in aeccCorr.CorridorSurfaces)
                {
                    surfaces.Add(new CorridorSurface(surf));
                }

                return surfaces;
            }
        }
        #endregion

        #region action methods
        /// <summary>
        /// Gets a Corridor Surface by name that is associated with a Corridor.
        /// </summary>
        /// <param name="corridor"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static CorridorSurface GetCorridorSurfaceByName(civDynNodes.Corridor corridor, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new InvalidOperationException("Surface name is null or empty.");
            }

            return CorridorSurfaces(corridor)
                .FirstOrDefault(item => item.Name.Equals
                    (name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Adds a new Corridor Surface to a Corridor with the option to specify Link and/or Feature Line Codes.
        /// </summary>
        /// <param name="corridor"></param>
        /// <param name="surfaceName"></param>
        /// <param name="linkCodes"></param>
        /// <param name="featureLineCodes"></param>
        /// <param name="style"></param>
        /// <param name="addLinkCodesAsBreaklines"></param>
        /// <param name="addExtentsAsBoundary"></param>
        /// <returns></returns>
        public static civDynNodes.Corridor AddCorridorSurface(
            civDynNodes.Corridor corridor,
            string surfaceName,
            List<string> linkCodes,
            List<string> featureLineCodes,
            SurfaceStyle style,
            bool addLinkCodesAsBreaklines = false,
            bool addExtentsAsBoundary = true)
        {
            if (string.IsNullOrEmpty(surfaceName))
            {
                throw new InvalidOperationException("Surface name is null or empty.");
            }

            using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
            {
                AeccCorridor aeccCorr = (AeccCorridor)ctx.Transaction.GetObject(
                    corridor.InternalObjectId,
                    acDb.OpenMode.ForWrite);

                if (aeccCorr.CorridorSurfaces.SurfaceNames().Contains(surfaceName))
                {
                    throw new InvalidOperationException("A Corridor Surface with that name already exists.");
                }

                try
                {
                    civDb.CorridorSurface newSurf = aeccCorr.CorridorSurfaces.Add(
                        surfaceName,
                        style.InternalObjectId);
                    civDb.Surface aeccSurf = (civDb.Surface)ctx.Transaction.GetObject(
                        newSurf.SurfaceId,
                        acDb.OpenMode.ForWrite);
                    foreach (string linkCode in linkCodes)
                    {
                        newSurf.AddLinkCode(linkCode, addLinkCodesAsBreaklines);
                    }
                    foreach (string featureLineCode in featureLineCodes)
                    {
                        newSurf.AddFeatureLineCode(featureLineCode);
                    }
                    if (addExtentsAsBoundary)
                    {
                        newSurf.Boundaries.AddCorridorExtentsBoundary("Corridor Boundary(1)");
                    }
                    aeccCorr.Rebuild();
                    aeccSurf.Rebuild();
                    return corridor;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
            }
        }
        #endregion
    }
}
