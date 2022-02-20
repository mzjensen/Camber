#region references
using System;
using System.Reflection;
using Camber.Utils;
#endregion

namespace Camber.Civil.DataShortcuts
{
    internal static class DSAssemblyLoader
    {
        /// <summary>
        /// Attempts to load the AeccDataShortcutMgd assembly.
        /// </summary>
        /// <returns></returns>
        internal static Assembly LoadDSAssembly()
        {
            string appVersion = Globals.GetCurrentVersion().ToString();
            string path = "C:\\Program Files\\Autodesk\\AutoCAD " + appVersion + "\\C3D\\AeccDataShortcutMgd.dll";
            try
            {
                return Assembly.LoadFrom(path);
            }
            catch
            {
                return null;
            }
        }
    }
}
