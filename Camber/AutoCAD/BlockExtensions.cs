#region references
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using DynamoServices;
using Dynamo.Graph.Nodes;
#endregion

namespace Camber.AutoCAD
{
    [RegisterForTrace]
    public class BlockExtensions
    {
        #region properties
        #endregion

        #region constructors
        private BlockExtensions() { }
        #endregion

        #region methods
        /// <summary>
        /// Gets the Block's Attribute Definitions.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static List<AttributeDefinition> AttributeDefinitions(acDynNodes.Block block)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            var attDefs = new List<AttributeDefinition>();

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                var t = ctx.Transaction;
                var bt = (acDb.BlockTable)t.GetObject(ctx.Database.BlockTableId, acDb.OpenMode.ForRead);
                var btr = (acDb.BlockTableRecord)t.GetObject(bt[block.Name], acDb.OpenMode.ForRead);
                if (btr.HasAttributeDefinitions)
                {
                    foreach (acDb.ObjectId oid in btr)
                    {
                        var obj = oid.GetObject(acDb.OpenMode.ForRead);
                        if (obj is acDb.AttributeDefinition)
                        {
                            attDefs.Add(AttributeDefinition.GetByObjectId(oid));
                        }
                    }
                }
            }
            return attDefs;
        }
        #endregion
    }
}
