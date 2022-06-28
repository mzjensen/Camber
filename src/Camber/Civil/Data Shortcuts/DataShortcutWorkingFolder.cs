#region references
using System;
using System.IO;
using System.Collections.Generic;
using AeccDataShortcuts = Autodesk.Civil.DataShortcuts.DataShortcuts;
using Autodesk.DesignScript.Runtime;
#endregion

namespace Camber.Civil.DataShortcuts
{
    public sealed class DataShortcutWorkingFolder
    {
        #region fields
        private DirectoryInfo _directoryInfo;
        #endregion

        #region properties
        [IsVisibleInDynamoLibrary(false)]
        public DirectoryInfo DirectoryInfo
        {
            get { return _directoryInfo; }
            set
            {
                if (!Directory.Exists(value.FullName)) { throw new ArgumentException("A directory does not exist at the input path."); }
                _directoryInfo = value;
            }
        }

        /// <summary>
        /// Gets the full path of a Data Shortcut Working Folder.
        /// </summary>
        public string Path => DirectoryInfo.FullName;

        /// <summary>
        /// Gets the directory name of a Data Shortcut Working Folder.
        /// </summary>
        public string Name => DirectoryInfo.Name;

        /// <summary>
        /// Gets all of the Data Shortcut Projects in a Data Shortcut Working Folder.
        /// </summary>
        public List<DataShortcutProject> DataShortcutProjects
        {
            get
            {
                string currentProject = null;
                List<string> otherProjects = new List<string>();
                List<string> allProjects = new List<string>();
                List<DataShortcutProject> projectFolders = new List<DataShortcutProject>();
                AeccDataShortcuts.GetAllProjectFolders(ref currentProject, ref otherProjects);
                allProjects.Add(currentProject);
                foreach (string otherProject in otherProjects)
                {
                    allProjects.Add(otherProject);
                }
                foreach (string project in allProjects)
                {
                    projectFolders.Add(new DataShortcutProject(project));
                }
                return projectFolders;
            }
        }
        #endregion

        #region constructors
        internal DataShortcutWorkingFolder(string path)
        {
            DirectoryInfo = new DirectoryInfo(path);
        }
        #endregion

        #region methods
        public override string ToString() => $"DataShortcutWorkingFolder(Name = {Name})";

        /// <summary>
        /// Determines if a folder exists within a Data Shortcut Working Folder.
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        public bool ContainsFolder(string folderName)
        {
            if (string.IsNullOrEmpty(folderName)) { throw new ArgumentException("Folder name is null or empty."); }
            
            return Directory.Exists(Path + "/" + folderName);
        }

        /// <summary>
        /// Gets a Data Shortcut Project by name from a Data Shortcut Working Folder if it exists.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public DataShortcutProject GetDataShortcutProjectByName(string projectName)
        {
            if (string.IsNullOrEmpty(projectName)) { throw new ArgumentException("Project name is null or empty."); }

            var folder = DataShortcutProjects.Find(x => x.Name == projectName);
            if (folder == null)
            {
                throw new InvalidOperationException("The Working Folder does not contain a project with the input name.");
            }
            return folder;
        }
        #endregion
    }
}
