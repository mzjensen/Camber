#region references
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Autodesk.DesignScript.Runtime;
#endregion

namespace Camber.Utilities
{
    [SupressImportIntoVM]
    internal static class ReflectionUtilities
    {
        #region methods
        /// <summary>
        /// Get property from object.
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="defaultValue">Default return value in case of error</param>
        /// <returns></returns>
        public static object GetProperty
            (object obj, string propertyName, object defaultValue)
        {
            Type oType = obj?.GetType();
            try
            {
                return oType?
                    .GetProperty(propertyName,
                    BindingFlags.IgnoreCase
                    | BindingFlags.Public
                    | BindingFlags.Instance)?
                    .GetValue(obj) ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Get typed property from object.
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="defaultValue">Default return value in case of error</param>
        /// <returns></returns>
        public static T GetProperty<T>(object obj, string propertyName, T defaultValue)
        {
            Type oType = obj?.GetType();

            try
            {
                return
                    (T)(oType?.GetProperty
                    (propertyName,
                    BindingFlags.IgnoreCase
                    | BindingFlags.Public
                    | BindingFlags.Instance)?
                    .GetValue(obj) ?? defaultValue);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Returns instances of all the derived classes of a class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="constructorArgs"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class
        {
            List<T> objects = new List<T>();
            foreach (Type type in
                Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
            {
                try
                {
                    objects.Add((T)Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.NonPublic, null, constructorArgs, null));
                }
                catch { }
            }
            return objects;
        }

        /// <summary>
        /// Gets a nested property from object.
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="defaultValue">Default return value in case of error</param>
        /// <returns></returns>
        public static object GetNestedProperty(object obj, string propertyName, object defaultValue)
        {
            if (obj == null) { throw new ArgumentNullException("obj"); }
            if (propertyName == null) { throw new ArgumentNullException("propertyName"); }

            try
            {
                if (propertyName.Contains("."))
                {
                    var temp = propertyName.Split(new char[] { '.' }, 2);
                    return GetNestedProperty(GetNestedProperty(obj, temp[0], defaultValue), temp[1], defaultValue);
                }
                else
                {
                    var prop = obj.GetType().GetProperty(propertyName);
                    return prop.GetValue(obj, null) ?? null;
                }
            }
            catch { return defaultValue; }
        }


        /// <summary>
        /// Sets a nested property by name.
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="propertyName">Property name. Accepts compound property names with periods inbetween (e.g. "Property1.Property2")</param>
        /// <param name="value">Value</param>
        /// <returns></returns>
        public static object SetNestedProperty(object obj, string propertyName, object value)
        {
            if (obj == null) { throw new ArgumentNullException("obj"); }
            if (propertyName == null) { throw new ArgumentNullException("propertyName"); }

            try
            {
                string[] bits = propertyName.Split('.');
                for (int i = 0; i < bits.Length - 1; i++)
                {
                    PropertyInfo propertyToGet = obj.GetType().GetProperty(bits[i]);
                    obj = propertyToGet.GetValue(obj, null);
                }
                PropertyInfo propertyToSet = obj.GetType().GetProperty(bits.Last());
                propertyToSet.SetValue(obj, value, null);
                return obj;
            }
            catch { throw; }
            return null;
        }

        /// <summary>
        /// Invokes a method by name in the current assembly.
        /// </summary>
        /// <typeparam name="T">The type that the method is a member of.</typeparam>
        /// <param name="args"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public static object InvokeMethod<T>(object[] args, [CallerMemberName] string methodName = null)
        {
            Type objType = typeof(T);

            MethodInfo methodInfo = objType.GetMethod(methodName);
            Type returnType = methodInfo.ReturnType;
            
            try
            {
                object baseObject = Activator.CreateInstance(objType);
                return objType.InvokeMember(
                    methodName,
                    BindingFlags.Default | BindingFlags.InvokeMethod,
                    null,
                    baseObject,
                    args);
            }
            catch { }

            return null;
        }
        #endregion
    }
}