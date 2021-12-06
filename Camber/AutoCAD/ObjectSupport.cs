#region references
using System;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using Autodesk.DesignScript.Runtime;
#endregion

namespace Camber.AutoCAD
{
    [SupressImportIntoVM]
    internal static class ObjectSupport
    {
        public static T Get<T, U>(acDb.ObjectId oid, Func<U, T> creator)
            where T : acDynNodes.Object
            where U : acDb.DBObject
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (acDynApp.DocumentContext ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                U acObject = ctx.Transaction.GetObject(oid, acDb.OpenMode.ForWrite) as U;
                return creator(acObject);
            }
        }
    }
}