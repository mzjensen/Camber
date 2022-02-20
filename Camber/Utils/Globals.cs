#region references
using System;
using acApp = Autodesk.AutoCAD.ApplicationServices;
#endregion

namespace Camber.Utils
{
    internal static class Globals
    {
        /// <summary>
        /// Gets the current running version of AutoCAD/Civil 3D.
        /// </summary>
        /// <returns></returns>
        internal static short GetCurrentVersion()
        {
            dynamic acadapp = acApp.Application.AcadApplication;
            string p = acadapp.Path;
            var numstr = p.Substring(p.Length - 4);
            var num = Convert.ToInt16(numstr);
            return num;
        }
    }
}
