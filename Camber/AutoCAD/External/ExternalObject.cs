#region references
using System.Reflection;
using System.Runtime.CompilerServices;
using acDb = Autodesk.AutoCAD.DatabaseServices;
#endregion

namespace Camber.AutoCAD.External
{
    public class ExternalObject : ExternalObjectBase
    {
        #region properties
        protected acDb.Entity AcEntity => AcObject as acDb.Entity;

        /// <summary>
        /// Gets the database handle of an External Object.
        /// </summary>
        public string Handle => GetString();

        /// <summary>
        /// Gets the name of the layer that an External Object is on.
        /// </summary>
        public string Layer => GetString();
        #endregion

        #region constructors
        protected ExternalObject(acDb.Entity ent) : base(ent) { }
        #endregion

        #region methods
        public override string ToString() => $"ExternalObject(Handle = {Handle})";

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
            return "Not applicable";
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

        protected ExternalObject SetValue(object value, [CallerMemberName] string methodName = null)
        {
            if (methodName.StartsWith("Set"))
            {
                methodName = methodName.Substring(3);
            }
            return SetValue(methodName, value);
        }

        protected ExternalObject SetValue(string propertyName, object value)
        {
            try
            {
                bool openedForWrite = AcEntity.IsWriteEnabled;
                if (!openedForWrite) AcEntity.UpgradeOpen();
                PropertyInfo propInfo = AcEntity.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                propInfo?.SetValue(AcEntity, value);
                if (!openedForWrite) AcEntity.DowngradeOpen();
                return this;
            }
            catch { throw; }
        }
        #endregion
    }
}
