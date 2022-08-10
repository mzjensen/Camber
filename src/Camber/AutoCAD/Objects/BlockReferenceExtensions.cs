using Dynamo.Graph.Nodes;
using System.Collections.Generic;
using AcAttRef = Autodesk.AutoCAD.DatabaseServices.AttributeReference;
using AcBlockReference = Autodesk.AutoCAD.DatabaseServices.BlockReference;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;

namespace Camber.AutoCAD.Objects
{
    public static class BlockReference
    {
        #region query methods
        /// <summary>
        /// Gets the Attribute References in a Block Reference.
        /// </summary>
        /// <param name="blockReference"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static List<AttributeReference> AttributeReferences(this acDynNodes.BlockReference blockReference)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            var attRefs = new List<AttributeReference>();

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                var acBlkRef = (AcBlockReference)ctx.Transaction.GetObject(blockReference.InternalObjectId, acDb.OpenMode.ForRead);
                foreach (AcAttRef attRef in acBlkRef.AttributeCollection)
                {
                    attRefs.Add(new AttributeReference(attRef));
                }
            }
            return attRefs;
        }
        #endregion
    }
}
