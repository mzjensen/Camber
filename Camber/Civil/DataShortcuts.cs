#region references
using System;
using System.IO;
using System.Collections.Generic;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using AeccDataShortcut = Autodesk.Civil.DataShortcuts.DataShortcuts;
using Autodesk.DesignScript.Runtime;
using Camber.AutoCAD;
#endregion

namespace Camber.Civil.DataShortcuts
{
    public abstract class DataShortcuts
    {
        #region properties
        protected const string InvalidFolderNameMsg = "Folder name is null or empty.";
        protected const string RefreshCommand = "REFRESHSHORTCUTNODE ";
        #endregion

        #region constructors
        internal DataShortcuts() { }
        #endregion

        #region methods
        /// <summary>
        /// Gets the project folders within the current working folder. Both the current project folder and all other project folders are returned.
        /// </summary>
        /// <param name="runToggle">A boolean input that can be changed to force the node to run.</param>
        /// <returns></returns>
        [IsLacingDisabled]
        [MultiReturn(new[] { "Current", "Others" })]
        public static Dictionary<string, object> GetProjectFolders(bool runToggle = true)
        {
            DocumentExtensions.SendCommand(acDynNodes.Document.Current, RefreshCommand, false);
            return new Dictionary<string, object>
            {
                { "Current", AeccDataShortcut.GetCurrentProjectFolder() },
                { "Others", AeccDataShortcut.GetOtherProjectFolders() }
            };
        }

        /// <summary>
        /// Gets the description of the current Data Shortcuts project folder.
        /// </summary>
        /// <param name="runToggle">A boolean input that can be changed to force the node to run.</param>
        /// <returns></returns>
        [IsLacingDisabled]
        public static string GetCurrentProjectFolderDescription(bool runToggle = true) => AeccDataShortcut.GetDescriptionDataShorcutProjectFolder();

        /// <summary>
        /// Gets the description of a Data Shortcuts project folder by name within the current working folder.
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        [IsLacingDisabled]
        public static string GetProjectFolderDescription(string folderName)
        {
            if (string.IsNullOrEmpty(folderName)) { throw new ArgumentException("Project folder name is null or empty."); }

            try
            {
                return AeccDataShortcut.GetDescriptionDataShorcutProjectFolder(folderName);
            }
            catch { throw; }
        }

        /// <summary>
        /// Gets the Data Shortcuts working folder.
        /// </summary>
        /// <param name="runToggle">A boolean input that can be changed to force the node to run.</param>
        /// <returns></returns>
        [IsLacingDisabled]
        public static string GetWorkingFolder(bool runToggle = true)
        {
            try
            {
                return AeccDataShortcut.GetWorkingFolder();
            }
            catch { throw; }
        } 

        /// <summary>
        /// Sets the Data Shortcuts working folder. Returns true if the working folder was set successfully and false otherwise.
        /// </summary>
        /// <param name="folderPath"></param>
        [IsLacingDisabled]
        public static bool SetWorkingFolder(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath)) { throw new ArgumentException("Folder path is null or empty."); }

            // Check if folder path is valid
            try
            {
                Path.GetFullPath(folderPath);
            }
            catch { throw; }

            // Disallow relative paths
            if (!Path.IsPathRooted(folderPath)) { throw new ArgumentException("Folder path cannot be relative."); }

            try
            {
                AeccDataShortcut.SetWorkingFolder(folderPath);

                // Check if working folder was set successfully
                string path = AeccDataShortcut.GetWorkingFolder();
                if (path == folderPath)
                {
                    return true;
                }
                return false;
            }
            catch { throw; } 
        }

        /// <summary>
        /// Sets the current Data Shortcuts project folder by name. The project folder must exist within the working folder.
        /// Returns true if the project folder is set successfully and false otherwise.
        /// </summary>
        /// <param name="folderName"></param>
        [IsLacingDisabled]
        public static bool SetCurrentProjectFolder(string folderName)
        {
            if (string.IsNullOrEmpty(folderName)) { throw new ArgumentException(InvalidFolderNameMsg); }

            try
            {
                AeccDataShortcut.SetCurrentProjectFolder(folderName);
                DocumentExtensions.SendCommand(acDynNodes.Document.Current, RefreshCommand, false);

                // Check if the folder was set correctly
                string dir = AeccDataShortcut.GetCurrentProjectFolder();
                string dirName = new DirectoryInfo(dir).Name;
                if (dirName == folderName)
                {
                    return true;
                }
                return false;
            }
            catch { throw; }
        }

        /// <summary>
        /// Validates all Data Shortcuts.
        /// </summary>
        /// <param name="runToggle">A boolean input that can be changed to force the node to run.</param>
        [IsLacingDisabled]
        public static void Validate(bool runToggle = true) => AeccDataShortcut.Validate();

        /// <summary>
        /// Creates a new Data Shortcuts project folder in the current working folder.
        /// Returns true if the folder is successfully created and false otherwise.
        /// </summary>
        /// <param name="folderName">The name of the folder.</param>
        /// <param name="folderDescription">The description of the folder.</param>
        /// <param name="templatePath">The full path to a project template. If left blank, a template will not be used.</param>
        /// <param name="setAsCurrent">Set the current project folder to the newly created one?</param>
        /// <param name="overwrite">Overwrite project folder if it already exists?</param>
        public static bool CreateProjectFolder(
            string folderName,
            string folderDescription = "",
            string templatePath = "",
            bool setAsCurrent = false,
            bool overwrite = false)
        {
            if (string.IsNullOrEmpty(folderName)) { throw new ArgumentException(InvalidFolderNameMsg); }

            if (!overwrite && Directory.Exists(AeccDataShortcut.GetWorkingFolder() + "/" + folderName))
            {
                throw new InvalidOperationException("Folder already exists.");
            }

            try
            {
                // Check if template path is provided
                if (string.IsNullOrEmpty(templatePath))
                {
                    AeccDataShortcut.CreateProjectFolder(folderName, folderDescription, setAsCurrent);
                }

                // Create the folder
                AeccDataShortcut.CreateProjectFolder(folderName, folderDescription, templatePath, setAsCurrent);
                DocumentExtensions.SendCommand(acDynNodes.Document.Current, RefreshCommand, false);

                // Check if the newly created folder exists
                if (!Directory.Exists(AeccDataShortcut.GetWorkingFolder() + "/" + folderName))
                {
                    return false;
                }
                return true;
            }
            catch { throw; }
        }

        /// <summary>
        /// Gets the list of ExportableItems from the DataShortcutManager.
        /// </summary>
        [IsVisibleInDynamoLibrary(true)]
        public static IList<AeccDataShortcut.DataShortcutManager.ExportableItem> GetExportableItems()
        {
            bool isValidCreation = false;
            var manager = AeccDataShortcut.CreateDataShortcutManager(ref isValidCreation);
            var exportableItems = new List<AeccDataShortcut.DataShortcutManager.ExportableItem>();
            if (isValidCreation)
            {
                var exportableItemsCount = manager.GetExportableItemsCount();
                for (int i = 0; i < exportableItemsCount; i++)
                {
                    exportableItems.Add(manager.GetExportableItemAt(i));
                }
            }
            return exportableItems;
        }
        #endregion
    }
}
