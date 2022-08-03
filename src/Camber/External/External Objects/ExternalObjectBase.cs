using Autodesk.DesignScript.Runtime;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using acDb = Autodesk.AutoCAD.DatabaseServices;

namespace Camber.External.ExternalObjects
{
    [IsVisibleInDynamoLibrary(false)]
    public class ExternalObjectBase
    {
        #region properties
        protected acDb.DBObject AcObject { get; set; }
        protected acDb.ObjectId AcObjectId { get; set; }
        protected acDb.Database AcDatabase { get; set; }
        public acDb.DBObject InternalDBObject => AcObject;
        public acDb.ObjectId InternalObjectId => AcObjectId;
        #endregion

        #region constructors
        protected ExternalObjectBase(acDb.DBObject obj)
        {
            AcObject = obj;
            AcObjectId = obj.ObjectId;
            AcDatabase = obj.Database;
        }
        #endregion

        #region methods
        protected double GetDouble([CallerMemberName] string propertyName = null)
        {
            try
            {
                PropertyInfo propInfo = AcObject.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (propInfo != null)
                {
                    return (double)propInfo.GetValue(AcObject);
                }
            }
            catch { }
            return double.NaN;
        }

        protected string GetString([CallerMemberName] string propertyName = null)
        {
            try
            {
                PropertyInfo propInfo = AcObject.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (propInfo != null)
                {
                    var value = propInfo.GetValue(AcObject).ToString();
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
            return "Not applicable";
        }

        protected int GetInt([CallerMemberName] string propertyName = null)
        {
            try
            {
                PropertyInfo propInfo = AcObject.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (propInfo != null)
                {
                    return (int)propInfo.GetValue(AcObject);
                }
            }
            catch { }
            return int.MinValue;
        }

        protected bool GetBool([CallerMemberName] string propertyName = null)
        {
            try
            {
                PropertyInfo propInfo = AcObject.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

                if (propInfo != null)
                {
                    return (bool)propInfo.GetValue(AcObject);
                }
            }
            catch { }
            return false;
        }

        protected dynamic SetValue(object value, [CallerMemberName] string methodName = null)
        {
            if (methodName.StartsWith("Set"))
            {
                methodName = methodName.Substring(3);
            }
            return SetValue(methodName, value);
        }

        protected dynamic SetValue(string propertyName, object value)
        {
            try
            {
                using(var tr = AcDatabase.TransactionManager.StartOpenCloseTransaction())
                {
                    acDb.DBObject obj = tr.GetObject(AcObject.ObjectId, acDb.OpenMode.ForWrite);
                    PropertyInfo propInfo = obj.GetType()
                    .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                    propInfo?.SetValue(obj, value);
                    return this;
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
