#region references
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using civDb = Autodesk.Civil.DatabaseServices;
using civApp = Autodesk.Civil.ApplicationServices;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using DynamoServices;
using Camber.Civil.Styles;
using Camber.Utils;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Camber.Civil
{
    [RegisterForTrace]
    public class CivilObjectExtensions : civDynNodes.CivilObject
    {
        #region properties
        internal civDb.Entity AeccEntity => AcObject as civDb.Entity;
        protected const string NotApplicableMsg = "Not applicable";
        //protected bool IsReference => AeccEntity.IsReferenceObject || AeccEntity.IsReferenceSubObject;
        #endregion

        #region constructors
        internal CivilObjectExtensions(civDb.Entity entity, bool isDynamoOwned = false) : base(entity, isDynamoOwned) { }
        #endregion

        #region methods
        /// <summary>
        /// Gets the Style of a Civil Object.
        /// </summary>
        /// <param name="civilObject"></param>
        /// <returns></returns>
        public static Style GetStyle(civDynNodes.CivilObject civilObject)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document?.AcDocument))
            {
                var cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                acDb.ObjectId oid = civilObject.InternalObjectId;
                var aeccEntity = (civDb.Entity)oid.GetObject(acDb.OpenMode.ForRead);
                var styleId = aeccEntity.StyleId;
                var aeccStyle = styleId.GetObject(acDb.OpenMode.ForRead);
                IEnumerable<Style> styles = ReflectionUtils.GetEnumerableOfType<Style>(new object[] { aeccStyle, false });
                if (styles.Count() > 1)
                {
                    throw new Exception("Multiple Styles found.");
                }
                else if (styles.Count() == 0)
                {
                    throw new Exception("No Style found.");
                }
                return styles.First();
            }
        }

        /// <summary>
        /// Gets whether the Civil Object is a reference object.
        /// A reference object is located in another drawing and linked using a data shortcut or through Autodesk Vault.
        /// </summary>
        /// <param name="civilObject"></param>
        /// <returns></returns>
        public static bool GetIsReference(civDynNodes.CivilObject civilObject)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document?.AcDocument))
            {
                var cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                acDb.ObjectId oid = civilObject.InternalObjectId;
                var aeccEntity = (civDb.Entity)oid.GetObject(acDb.OpenMode.ForRead);
                if (aeccEntity.IsReferenceObject || aeccEntity.IsReferenceSubObject)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Sets the Style of a Civil Object.
        /// </summary>
        /// <param name="civilObject"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public static civDynNodes.CivilObject SetStyle(civDynNodes.CivilObject civilObject, Style style)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document?.AcDocument))
            {
                var cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                acDb.ObjectId oid = civilObject.InternalObjectId;
                var aeccEntity = (civDb.Entity)oid.GetObject(acDb.OpenMode.ForWrite);
                try
                {
                    aeccEntity.StyleId = style.InternalObjectId;
                    return civilObject;
                }
                catch
                {
                    throw new Exception("Style is not valid for this type of object.");
                }
            }
        }

        protected double GetDouble([CallerMemberName] string propertyName = null)
        {
            try
            {
                PropertyInfo propInfo = AeccEntity.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (propInfo != null)
                {
                    return (double)propInfo.GetValue(AeccEntity);
                }
            }
            catch { }
            return double.NaN;
        }

        protected string GetString([CallerMemberName] string propertyName = null)
        {
            try
            {
                PropertyInfo propInfo = AeccEntity.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (propInfo != null)
                {
                    var value = propInfo.GetValue(AeccEntity).ToString();
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
                PropertyInfo propInfo = AeccEntity.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (propInfo != null)
                {
                    return (int)propInfo.GetValue(AeccEntity);
                }
            }
            catch { }
            return int.MinValue;
        }

        protected bool GetBool([CallerMemberName] string propertyName = null)
        {
            try
            {
                PropertyInfo propInfo = AeccEntity.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

                if (propInfo != null)
                {
                    return (bool)propInfo.GetValue(AeccEntity);
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
                bool openedForWrite = AeccEntity.IsWriteEnabled;
                if (!openedForWrite) AeccEntity.UpgradeOpen();
                PropertyInfo propInfo = AeccEntity.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                propInfo?.SetValue(AeccEntity, value);
                if (!openedForWrite) AeccEntity.DowngradeOpen();
                return true;
            }
            catch { }
            return false;
        }
        #endregion
    }
}