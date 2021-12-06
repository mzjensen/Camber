#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using AeccAlignment = Autodesk.Civil.DatabaseServices.Alignment;
using DynamoServices;
#endregion

namespace Camber.Civil
{
    [RegisterForTrace]
    public sealed class Alignment
    {
        #region properties
        #endregion

        #region constructors
        private Alignment() { }
        #endregion

        #region methods
        /// <summary>
        /// Gets a Dynamo-wrapped Alignment by Object ID.
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        internal static civDynNodes.Alignment GetFromObjectId(acDb.ObjectId oid)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            try
            {
                using (acDynApp.DocumentContext ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccAlign = ctx.Transaction.GetObject(oid, acDb.OpenMode.ForWrite) as AeccAlignment;
                    return civDynNodes.Selection.AlignmentByName(aeccAlign.Name, document);
                }
            }
            catch { throw; }
        }
        #endregion
    }
}
