#region references
using System;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using AeccEntity = Autodesk.Civil.DatabaseServices.Entity;
using Autodesk.DesignScript.Runtime;
#endregion

namespace Camber.Civil.CivilObjects
{
    [SupressImportIntoVM]
    internal static class CivilObjectSupport
    {
        public static T Get<T, U>(acDb.ObjectId id, Func<U, T> creator)
            where T : civDynNodes.CivilObject
            where U : AeccEntity
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (acDynApp.DocumentContext ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                U aeccObject = ctx.Transaction.GetObject(id, acDb.OpenMode.ForWrite) as U;
                return creator(aeccObject);
            }
        }

        /// <summary>
        /// Checks whether the Entity is a reference object. A reference object is located in another drawing, and linked using a data shortcut or through Autodesk Vault.
        /// </summary>
        /// <param name="entity"></param>
        public static void CheckForReference(AeccEntity entity)
        {
            if (entity.IsReferenceObject || entity.IsReferenceSubObject)
            {
                throw new InvalidOperationException("Object is referenced and cannot be changed!");
            }
        }
    }
}