#region references
using System;
using System.IO;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using AeccDataShortcuts = Autodesk.Civil.DataShortcuts.DataShortcuts;
using Autodesk.DesignScript.Runtime;
using Dynamo.Graph.Nodes;
using Camber.AutoCAD;
#endregion

namespace Camber.Civil.DataShortcuts
{
    [IsVisibleInDynamoLibrary(false)]
    public static class DataShortcuts
    {
        #region properties
        private const string RefreshCommand = "REFRESHSHORTCUTNODE ";
        #endregion

        #region methods
        /// <summary>
        /// Get working folder method for use internally.
        /// </summary>
        [SupressImportIntoVM]
        public static DataShortcutWorkingFolder GetDSWorkingFolder()
        {
            return new DataShortcutWorkingFolder(AeccDataShortcuts.GetWorkingFolder());
        }

        /// <summary>
        /// Gets the current Data Shortcuts Working Folder.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static DataShortcutWorkingFolder GetWorkingFolder(acDynNodes.Document document)
        {
            Refresh();
            return new DataShortcutWorkingFolder(AeccDataShortcuts.GetWorkingFolder());
        }

        /// <summary>
        /// Get current Data Shortcut Project for use internally.
        /// </summary>
        /// <param name="refresh"></param>
        /// <returns></returns>
        [SupressImportIntoVM]
        public static DataShortcutProject GetCurrentDSProject()
        {
            return new DataShortcutProject(AeccDataShortcuts.GetCurrentProjectFolder());
        }

        /// <summary>
        /// Gets the current Data Shortcut Project.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static DataShortcutProject GetCurrentProject(acDynNodes.Document document)
        {
            Refresh();
            return new DataShortcutProject(AeccDataShortcuts.GetCurrentProjectFolder());
        }

        /// <summary>
        /// Refresh method for use internally.
        /// </summary>
        [SupressImportIntoVM]
        public static void Refresh() => Document.SendCommand(acDynNodes.Document.Current, RefreshCommand, false);

        /// <summary>
        /// Refreshes the Data Shortcuts view in the Toolspace.
        /// </summary>
        /// <param name="runToggle"></param>
        /// <returns></returns>
        public static bool Refresh(bool runToggle = true)
        {
            try
            {
                Document.SendCommand(acDynNodes.Document.Current, RefreshCommand, false);
                return true;
            }
            catch
            {
                return false;
            }
        }
        

        /// <summary>
        /// Sets the current Data Shortcuts Working Folder for the application.
        /// </summary>
        /// <param name="directoryPath"></param>
        public static DataShortcutWorkingFolder SetWorkingFolder(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath)) { throw new ArgumentException("Folder path is null or empty."); }
            if (!Directory.Exists(directoryPath)) { throw new ArgumentException("Directory does not exist."); }

            try
            {
                AeccDataShortcuts.SetWorkingFolder(directoryPath);
                Refresh();
                return GetDSWorkingFolder();
            }
            catch { throw; }
        }


        /// <summary>
        /// Validates all Data Shortcuts.
        /// </summary>
        /// <param name="runToggle"></param>
        /// <returns></returns>
        [IsLacingDisabled]
        public static bool Validate(bool runToggle = true)
        {
            try
            {
                AeccDataShortcuts.Validate();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
