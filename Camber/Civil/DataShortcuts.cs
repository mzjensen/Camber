#region references
using System;
using System.Collections.Generic;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using AeccDataShortcut = Autodesk.Civil.DataShortcuts.DataShortcuts;
#endregion

namespace Camber.Civil
{
    // Need to expand this out after the 2022.1 update.
    // The DLL that comes with C3D 2022 does not have the latest and greatest for the Data Shortcuts API, which was included in the 2021.3 update.
    // https://help.autodesk.com/view/CIV3D/2020/ENU/?guid=299173e1-d4f1-a572-539d-f800a22b7cd8
    public abstract class DataShortcuts
    {
        #region properties
        #endregion

        #region constructors
        internal DataShortcuts() { }
        #endregion

        #region methods
        /// <summary>
        /// Gets the current Data Shortcuts project folder.
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentProjectFolder() => AeccDataShortcut.GetCurrentProjectFolder();

        /// <summary>
        /// Gets the other Data Shortcut project folders within the working folder.
        /// </summary>
        /// <returns></returns>
        public static IList<string> GetOtherProjectFolders() => AeccDataShortcut.GetOtherProjectFolders();

        /// <summary>
        /// Gets the description of the current Data Shortcuts project folder.
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentProjectFolderDescription() => AeccDataShortcut.GetDescriptionDataShorcutProjectFolder();

        /// <summary>
        /// Gets the description of a Data Shortcuts project folder by name.
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static string GetProjectFolderDescription(string folderName) => AeccDataShortcut.GetDescriptionDataShorcutProjectFolder(folderName);

        /// <summary>
        /// Gets the Data Shortcuts working folder.
        /// </summary>
        /// <returns></returns>
        public static string GetWorkingFolder() => AeccDataShortcut.GetWorkingFolder();

        /// <summary>
        /// Sets the Data Shortcuts working folder.
        /// </summary>
        /// <param name="folderPath"></param>
        public static void SetWorkingFolder(string folderPath) => AeccDataShortcut.SetWorkingFolder(folderPath);


        /// <summary>
        /// Sets the current Data Shortcuts project folder by name.
        /// </summary>
        /// <param name="folderName"></param>
        public static void SetCurrentProjectFolderstring(string folderName) => AeccDataShortcut.SetCurrentProjectFolder(folderName);

        /// <summary>
        /// Validates Data Shortcuts.
        /// </summary>
        public static void Validate() => AeccDataShortcut.Validate();

        /// <summary>
        /// Creates a new Data Shortcuts project folder in the current working folder.
        /// </summary>
        /// <param name="folderName">The name of the folder.</param>
        /// <param name="folderDescription">The description of the folder.</param>
        /// <param name="templatePath">The full path to a project template. If left blank, a template will not be used.</param>
        /// <param name="setAsCurrent">Set the current project folder to the newly created one?</param>
        public static void CreateProjectFolder(string folderName, string folderDescription = "", string templatePath = "", bool setAsCurrent = false)
        {
            // TODO: add some type of check in here to return true if the folder was successfully created, or false if not.
            // Also add a boolean to allow overwriting an existing folder with the same name.
            
            if (string.IsNullOrEmpty(folderName)) { throw new ArgumentException("Folder name is null or empty."); }
            try
            {
                if (string.IsNullOrEmpty(templatePath))
                {
                    AeccDataShortcut.CreateProjectFolder(folderName, folderDescription, setAsCurrent);
                }
                AeccDataShortcut.CreateProjectFolder(folderName, folderDescription, templatePath, setAsCurrent);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }
        #endregion
    }
}
