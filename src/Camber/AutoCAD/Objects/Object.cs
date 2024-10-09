#region references
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civDb = Autodesk.Civil.DatabaseServices;
using Dynamo.Graph.Nodes;
using Autodesk.DesignScript.Runtime;
using Camber.Civil.CivilObjects;
using Camber.Properties;
using Camber.Utilities;
using DynamoServices;
#endregion

namespace Camber.AutoCAD.Objects
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
        /// <param name="object"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        // BUG: for some reason this is giving a warning of "Converting objects to function pointers is not allowed."
        private static string Handle(acDynNodes.Object @object)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument.Database))
            {
                acDb.Entity acEnt = (acDb.Entity)ctx.Transaction.GetObject(@object.InternalObjectId, acDb.OpenMode.ForRead);
                return acEnt.Handle.ToString();
            }
        }

        /// <summary>
        /// Gets the Fields associated with an Object.
        /// </summary>
        /// <param name="object"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static List<Field> Fields(acDynNodes.Object @object)
        {
            var fields = new List<Field>();
            
            try
            {
                var document = acDynNodes.Document.Current;
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument.Database))
                {
                    var acEnt = (acDb.Entity) ctx.Transaction.GetObject(@object.InternalObjectId, acDb.OpenMode.ForRead);
                    
                    if (!acEnt.HasFields)
                    {
                        return fields;
                    }

                    var parentFieldId = acEnt.GetField();
                    var acParentField = (acDb.Field) ctx.Transaction.GetObject(parentFieldId, acDb.OpenMode.ForRead);

                    var childFields = acParentField.GetChildren();

                    foreach (var field in childFields)
                    {
                        fields.Add(new Field(field));
                    }

                    return fields;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Highlights an Object in the current document.
        /// </summary>
        /// <param name="object"></param>
        /// <returns></returns>
        public static acDynNodes.Object Highlight(acDynNodes.Object @object)
        {
            using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
            {
                acDb.Entity ent = (acDb.Entity)ctx.Transaction.GetObject(@object.InternalObjectId, acDb.OpenMode.ForRead);
                ent.Highlight();
            }
            return @object;
        }

        /// <summary>
        /// Selects Objects in the current document. This mimics the behavior of clicking on an object.
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        public static List<acDynNodes.Object> Select(List<acDynNodes.Object> @objects)
        {
            acDb.ObjectId[] oids = objects.Select(obj => obj.InternalObjectId).ToArray();

            var adoc = acDynNodes.Document.Current.AcDocument;
            var ed = adoc.Editor;
            try
            {
                ed.SetImpliedSelection(oids);
                ed.SelectImplied();
                return objects;
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
        public static acDynNodes.Object ConvertToCamberObject(acDynNodes.Object @object)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            if (@object == null)
            {
                return null;
            }
            using (var ctx = new acDynApp.DocumentContext(document?.AcDocument))
            {
                IEnumerable<acDynNodes.Object> assemblyObjects = null;
                var acObj = @object.InternalObjectId.GetObject(acDb.OpenMode.ForRead);

                if (acObj is civDb.Entity)
                {
                    assemblyObjects = ReflectionUtilities.GetEnumerableOfType<CivilObject>(new object[] { acObj, false });
                }
                else if (acObj is acDb.Entity)
                {
                    assemblyObjects = ReflectionUtilities.GetEnumerableOfType<Object>(new object[] { acObj, false });
                }

                if (assemblyObjects == null)
                {
                    throw new InvalidOperationException("Unable to get internal object.");
                }
                else
                {
                    if (assemblyObjects.Count() == 0)
                    {
                        return @object;
                    }
                    // If there are multiple objects, the first one should be the furthest down the inheritance hierarchy (i.e. the most derived)
                    return assemblyObjects.First();
                }
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

        #region deprecated
        /// <summary>
        /// Gets the color index assigned to an Object.
        /// </summary>
        /// <param name="object"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static int ColorIndex(acDynNodes.Object @object)
        {
            LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MESSAGE, "Object.Color"));

            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument.Database))
            {
                acDb.Entity acEnt = (acDb.Entity)ctx.Transaction.GetObject(@object.InternalObjectId, acDb.OpenMode.ForRead);
                return acEnt.ColorIndex;
            }
        }

        /// <summary>
        /// Sets the color of an Object by color index.
        /// 0 = ByBlock, 256 = ByLayer.
        /// </summary>
        /// <param name="object"></param>
        /// <param name="colorIndex"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.AutoCAD.Objects.Object.SetColor",
            "Autodesk.AutoCAD.DynamoNodes.Object.SetColor")]
        public static acDynNodes.Object SetColor(acDynNodes.Object @object, int colorIndex)
        {
            LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Object.SetColor"));

            acDynNodes.Document document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument.Database))
                {
                    acDb.Entity acEnt = (acDb.Entity)ctx.Transaction.GetObject(
                        @object.InternalObjectId,
                        acDb.OpenMode.ForWrite);
                    acEnt.ColorIndex = colorIndex;
                    return @object;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

        }
        #endregion
    }
}