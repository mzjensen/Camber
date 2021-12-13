#region references
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using DynamoServices;
using Dynamo.Graph.Nodes;
using Autodesk.DesignScript.Runtime;
using Camber.Utils;
#endregion

namespace Camber.AutoCAD
{
    public class Object : acDynNodes.Object
    {
        #region properties
        internal acDb.Entity AcEntity => AcObject as acDb.Entity;
        #endregion

        #region constructors
        internal Object(acDb.Entity acEntity, bool isDynamoOwned = false) : base(acEntity, isDynamoOwned) { }
        #endregion

        #region methods
        /// <summary>
        /// Gets the handle assigned to an Object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static string Handle(acDynNodes.Object obj)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument.Database))
            {
                acDb.Entity acEnt = (acDb.Entity)ctx.Transaction.GetObject(obj.InternalObjectId, acDb.OpenMode.ForRead);
                return acEnt.Handle.ToString();
            }
        }

        /// <summary>
        /// Gets the color index assigned to an Object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static int ColorIndex(acDynNodes.Object obj)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument.Database))
            {
                acDb.Entity acEnt = (acDb.Entity)ctx.Transaction.GetObject(obj.InternalObjectId, acDb.OpenMode.ForRead);
                return acEnt.ColorIndex;
            }
        }

        /// <summary>
        /// Highlights an Object in the current document.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static acDynNodes.Object Highlight(acDynNodes.Object obj)
        {
            using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
            {
                acDb.Entity ent = (acDb.Entity)ctx.Transaction.GetObject(obj.InternalObjectId, acDb.OpenMode.ForRead);
                ent.Highlight();
            }
            return obj;
        }

        /// <summary>
        /// Selects Objects in the current document. This mimics the behavior of clicking on an object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<acDynNodes.Object> Select(List<acDynNodes.Object> objs)
        {
            acDb.ObjectId[] oids = objs.Select(obj => obj.InternalObjectId).ToArray();

            var adoc = acDynNodes.Document.Current.AcDocument;
            var ed = adoc.Editor;
            try
            {
                ed.SetImpliedSelection(oids);
                ed.SelectImplied();
                return objs;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        /// <summary>
        /// Converts a Dynamo object to its valid Camber object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        public static acDynNodes.Object ConvertToCamberObject(acDynNodes.Object obj)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document?.AcDocument))
            {
                var acObj = obj.InternalObjectId.GetObject(acDb.OpenMode.ForRead);
                IEnumerable<Object> assemblyObjects = ReflectionUtils.GetEnumerableOfType<Object>(new object[] { acObj, false });
                if (assemblyObjects.Count() > 1)
                {
                    throw new InvalidOperationException("Multiple object types found.");
                }
                else if (assemblyObjects.Count() == 0)
                {
                    throw new InvalidOperationException("Not implemented.");
                }
                return assemblyObjects.First();
            }
        }

        #region helper methods
        protected double GetDouble([CallerMemberName] string propertyName = null)
        {
            try
            {
                PropertyInfo propInfo = AcEntity.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (propInfo != null)
                {
                    return (double)propInfo.GetValue(AcEntity);
                }
            }
            catch { }
            return double.NaN;
        }

        protected string GetString([CallerMemberName] string propertyName = null)
        {
            try
            {
                PropertyInfo propInfo = AcEntity.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (propInfo != null)
                {
                    var value = propInfo.GetValue(AcEntity).ToString();
                    if (string.IsNullOrEmpty(value))
                    {
                        return null;
                    }
                    else
                    {
                        return value;
                    }
                }
            }
            catch { }
            return "Not applicable.";
        }

        protected int GetInt([CallerMemberName] string propertyName = null)
        {
            try
            {
                PropertyInfo propInfo = AcEntity.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (propInfo != null)
                {
                    return (int)propInfo.GetValue(AcEntity);
                }
            }
            catch { }
            return int.MinValue;
        }

        protected bool GetBool([CallerMemberName] string propertyName = null)
        {
            try
            {
                PropertyInfo propInfo = AcEntity.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

                if (propInfo != null)
                {
                    return (bool)propInfo.GetValue(AcEntity);
                }
            }
            catch { }
            return false;
        }

        protected bool SetValue(object value, [CallerMemberName] string methodName = null)
        {
            if (methodName.StartsWith("Set"))
            {
                methodName = methodName.Substring(3);
            }
            return SetValue(methodName, value);
        }

        protected bool SetValue(string propertyName, object value)
        {
            try
            {
                bool openedForWrite = AcEntity.IsWriteEnabled;
                if (!openedForWrite) AcEntity.UpgradeOpen();
                PropertyInfo propInfo = AcEntity.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                propInfo?.SetValue(AcEntity, value);
                if (!openedForWrite) AcEntity.DowngradeOpen();
                return true;
            }
            catch { }
            return false;
        }
        #endregion
        #endregion
    }
}