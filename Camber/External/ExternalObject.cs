#region references
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using Autodesk.DesignScript.Runtime;
#endregion

namespace Camber.External
{
    public class ExternalObject : ExternalObjectBase
    {
        #region properties
        internal acDb.Entity AcEntity => AcObject as acDb.Entity;

        /// <summary>
        /// Gets the color index assigned to an External Object.
        /// </summary>
        public int ColorIndex => GetInt();

        /// <summary>
        /// Gets the database handle of an External Object.
        /// </summary>
        public string Handle => GetString();

        /// <summary>
        /// Gets the name of the layer that an External Object is on.
        /// </summary>
        public string Layer => GetString();

        /// <summary>
        /// Gets the linetype assigned to an External Object.
        /// </summary>
        public string Linetype => GetString();

        /// <summary>
        /// Gets the scale of the linetype assigned to an External Object.
        /// </summary>
        public double LinetypeScale => GetDouble();

        /// <summary>
        /// Gets the material assigned to an External Object.
        /// </summary>
        public string Material => GetString();

        /// <summary>
        /// Gets the name of the plot style assigned to an External Object.
        /// </summary>
        public string PlotStyle => GetString("PlotStyleName");

        /// <summary>
        /// Gets the underlying Autodesk.AutoCAD.DatabaseServices type of an External Object.
        /// </summary>
        public string Type
        {
            get
            {
                const string prefix = "Autodesk.AutoCAD.DatabaseServices.";
                string typeString = AcEntity.GetType().ToString();

                if (typeString.Contains(prefix))
                {
                    typeString = typeString.Substring(prefix.Length);
                }
                return typeString;
            }
        }
        #endregion

        #region constructors
        internal ExternalObject(acDb.Entity ent) : base(ent) { }
        #endregion

        #region methods
        public override string ToString() => $"ExternalObject(Handle = {Handle})";

        /// <summary>
        /// Sets the layer that an External Object is on.
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public ExternalObject SetLayer(string layer) => SetValue(layer);

        /// <summary>
        /// Sets the linetype for an External Object.
        /// </summary>
        /// <param name="linetype"></param>
        /// <returns></returns>
        public ExternalObject SetLinetype(string linetype) => SetValue(linetype);

        /// <summary>
        /// Sets the scale of the linetype for an External Object.
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        public ExternalObject SetLinetypeScale(double scale) => SetValue(scale);

        /// <summary>
        /// Sets the material assigned to an External Object.
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        public ExternalObject SetMaterial(string material) => SetValue(material);

        /// <summary>
        /// Sets the plot style of an External Object.
        /// </summary>
        /// <param name="plotstyle"></param>
        /// <returns></returns>
        public ExternalObject SetPlotStyle(string plotstyle) => SetValue((object)plotstyle, "PlotStyleName");


        /// <summary>
        /// Deletes an External Object.
        /// </summary>
        public void Delete()
        {
            try
            {
                using (var tr = AcEntity.Database.TransactionManager.StartTransaction())
                {
                    acDb.Entity ent = (acDb.Entity)tr.GetObject(InternalObjectId, acDb.OpenMode.ForWrite);
                    ent.Erase();
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        #region helper methods
        [SupressImportIntoVM]
        public static T Get<T, U>(acDb.ObjectId oid, Func<U, T> creator)
            where T : ExternalObject
            where U : acDb.DBObject
        {
            acDb.Transaction t = oid.Database.TransactionManager.StartTransaction();
            using (t)
            {
                U acObject = t.GetObject(oid, acDb.OpenMode.ForWrite) as U;
                return creator(acObject);
            }
        }
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
            acDb.Transaction t = AcDatabase.TransactionManager.StartTransaction();
            using (t)
            {
                try
                {
                    acDb.Entity ent = (acDb.Entity)t.GetObject(InternalObjectId, acDb.OpenMode.ForWrite);
                    PropertyInfo propInfo = ent.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                    propInfo?.SetValue(ent, value);
                    return this;
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException(e.Message);
                }
            }
        }
        #endregion
        #endregion
    }
}
