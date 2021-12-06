#region references
using System.Reflection;
using System.Runtime.CompilerServices;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using DynamoServices;
using Dynamo.Graph.Nodes;
using Autodesk.DesignScript.Runtime;
#endregion

namespace Camber.AutoCAD
{
    [RegisterForTrace]
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