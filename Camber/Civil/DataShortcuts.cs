#region references
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
        public static string GetCurrentProjectFolder(acDynNodes.Document document) => AeccDataShortcut.GetCurrentProjectFolder();


        /// <summary>
        /// Gets the other Data Shortcut project folders within the working folder.
        /// </summary>
        /// <returns></returns>
        public static IList<string> GetOtherProjectFolders(acDynNodes.Document document) => AeccDataShortcut.GetOtherProjectFolders();

        /// <summary>
        /// Gets the description of the current Data Shortcuts project folder.
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentProjectFolderDescription(acDynNodes.Document document) => AeccDataShortcut.GetDescriptionDataShorcutProjectFolder();

        /// <summary>
        /// Gets the description of a Data Shortcuts project folder by name.
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static string GetProjectFolderDescription(acDynNodes.Document document, string folderName) => AeccDataShortcut.GetDescriptionDataShorcutProjectFolder(folderName);

        /// <summary>
        /// Gets the Data Shortcuts working folder.
        /// </summary>
        /// <returns></returns>
        public static string GetWorkingFolder(acDynNodes.Document document) => AeccDataShortcut.GetWorkingFolder();

        /// <summary>
        /// Sets the Data Shortcuts working folder.
        /// </summary>
        /// <param name="path"></param>
        public static void SetWorkingFolder(acDynNodes.Document document, string path) => AeccDataShortcut.SetWorkingFolder(path);


        /// <summary>
        /// Sets the current Data Shortcuts project folder by name.
        /// </summary>
        /// <param name="folderName"></param>
        public static void SetCurrentProjectFolder(acDynNodes.Document document, string folderName) => AeccDataShortcut.SetCurrentProjectFolder(folderName);

        /// <summary>
        /// Validates Data Shortcuts.
        /// </summary>
        public static void Validate(acDynNodes.Document document) => AeccDataShortcut.Validate();
        #endregion
    }
}
