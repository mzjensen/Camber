#region references
using System;
using System.IO;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using AeccDataShortcuts = Autodesk.Civil.DataShortcuts.DataShortcuts;
using Autodesk.DesignScript.Runtime;
using Dynamo.Graph.Nodes;
using Camber.Civil.DataShortcuts;
using Camber.AutoCAD;
#endregion

namespace Camber.Civil
{
    public static class CivilApplication
    {
        #region properties
        private const string RefreshCommand = "REFRESHSHORTCUTNODE ";
        #endregion

        #region methods
        /// <summary>
        /// Get working folder method for use internally.
        /// </summary>
        [SupressImportIntoVM]
        public static WorkingFolder WorkingFolder()
        {
            return new WorkingFolder(AeccDataShortcuts.GetWorkingFolder());
        }
        
        /// <summary>
        /// Gets the current Data Shortcuts Working Folder for the application.
        /// </summary>
        /// <param name="runToggle">A boolean input that can be changed to force the node to run.</param>
        /// <returns></returns>
        [NodeCategory("Query")]
        [IsLacingDisabled]
        public static WorkingFolder WorkingFolder(bool runToggle = true)
        {
            RefreshDataShortcuts();
            return new WorkingFolder(AeccDataShortcuts.GetWorkingFolder());
        }

        /// <summary>
        /// Get current project folder method for use internally.
        /// </summary>
        /// <param name="refresh"></param>
        /// <returns></returns>
        [SupressImportIntoVM]
        public static ProjectFolder ProjectFolder()
        {
            return new ProjectFolder(AeccDataShortcuts.GetCurrentProjectFolder());
        }

        /// <summary>
        /// Gets the current Data Shortcuts Project Folder for the application.
        /// </summary>
        /// <param name="runToggle">A boolean input that can be changed to force the node to run.</param>
        /// <returns></returns>
        [NodeCategory("Query")]
        [IsLacingDisabled]
        public static ProjectFolder ProjectFolder(bool runToggle = true)
        {
            RefreshDataShortcuts();
            return new ProjectFolder(AeccDataShortcuts.GetCurrentProjectFolder());
        }

        /// <summary>
        /// Refresh method for use internally.
        /// </summary>
        [SupressImportIntoVM]
        public static void RefreshDataShortcuts() => Document.SendCommand(acDynNodes.Document.Current, RefreshCommand, false);

        /// <summary>
        /// Refreshes the Data Shortcuts view in the Toolspace.
        /// </summary>
        /// <param name="runToggle">A boolean input that can be changed to force the node to run.</param>
        [IsLacingDisabled]
        public static void RefreshDataShortcuts(bool runToggle = true) => Document.SendCommand(acDynNodes.Document.Current, RefreshCommand, false);

        /// <summary>
        /// Sets the current Data Shortcuts Working Folder for the application.
        /// </summary>
        /// <param name="directoryPath"></param>
        [IsLacingDisabled]
        public static WorkingFolder SetWorkingFolder(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath)) { throw new ArgumentException("Folder path is null or empty."); }
            if (!Directory.Exists(directoryPath)) { throw new ArgumentException("Directory does not exist."); }

            try
            {
                AeccDataShortcuts.SetWorkingFolder(directoryPath);
                RefreshDataShortcuts();
                return WorkingFolder();
            }
            catch { throw; }
        }


        /// <summary>
        /// Validates all Data Shortcuts.
        /// </summary>
        /// <param name="runToggle">A boolean input that can be changed to force the node to run.</param>
        [IsLacingDisabled]
        public static void ValidateDataShortcuts(bool runToggle = true) => AeccDataShortcuts.Validate();
        #endregion
    }
}
