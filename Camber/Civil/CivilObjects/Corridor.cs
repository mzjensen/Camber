#region references
using System.Linq;
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using AeccCorridor = Autodesk.Civil.DatabaseServices.Corridor;
using Dynamo.Graph.Nodes;
using Camber.Civil.Intersections;
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
            catch { throw; }
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
        #endregion
    }
}
