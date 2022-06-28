#region references
using System;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using Autodesk.DesignScript.Runtime;
#endregion

namespace Camber.Civil.Styles
{
    [SupressImportIntoVM]
    internal static class StyleSupport
    {
        public static T Get<T, U>(acDb.ObjectId id, Func<U, T> creator)
            where T : acDynNodes.ObjectBase
            where U : acDb.DBObject
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (acDynApp.DocumentContext ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                U aeccObject = ctx.Transaction
                    .GetObject(id, acDb.OpenMode.ForWrite) as U;
                return creator(aeccObject);
            }
        }
    }
}