#region references
using System.Reflection;
using System.Runtime.CompilerServices;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using Autodesk.DesignScript.Runtime;
using DynamoServices;
#endregion

namespace Camber.AutoCAD
{
    [RegisterForTrace]
    [IsVisibleInDynamoLibrary(false)]
    public class Object : acDynNodes.Object
    {
        #region properties
        internal acDb.Entity AcEntity => AcObject as acDb.Entity;
        protected const string NotApplicableMsg = "Not applicable";
        #endregion

        #region constructors
        public Object(acDb.Entity acEntity, bool isDynamoOwned = false) : base(acEntity, isDynamoOwned) { }
        #endregion

        #region methods
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
            return NotApplicableMsg;
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
    }
}