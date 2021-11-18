#region references
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using AeccDataShortcuts = Autodesk.Civil.DataShortcuts.DataShortcuts;
using Autodesk.DesignScript.Runtime;
#endregion

namespace Camber.Civil.DataShortcuts
{
    public sealed class WorkingFolder
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
        /// Gets the full path of a Working Folder.
        /// </summary>
        public string Path => DirectoryInfo.FullName;

        /// <summary>
        /// Gets the directory name of a Working Folder.
        /// </summary>
        public string Name => DirectoryInfo.Name;

        /// <summary>
        /// Gets all of the Project Folders in a Working Folder.
        /// </summary>
        public List<ProjectFolder> ProjectFolders
        {
            get
            {
                string currentProject = null;
                List<string> otherProjects = new List<string>();
                List<string> allProjects = new List<string>();
                List<ProjectFolder> projectFolders = new List<ProjectFolder>();
                AeccDataShortcuts.GetAllProjectFolders(ref currentProject, ref otherProjects);
                allProjects.Add(currentProject);
                foreach (string otherProject in otherProjects)
                {
                    allProjects.Add(otherProject);
                }
                foreach (string project in allProjects)
                {
                    projectFolders.Add(new ProjectFolder(project));
                }
                return projectFolders;
            }
        }
        #endregion

        #region constructors
        internal WorkingFolder(string path)
        {
            DirectoryInfo = new DirectoryInfo(path);
        }
        #endregion

        #region methods
        public override string ToString() => $"WorkingFolder(Name = {Name})";

        /// <summary>
        /// Determines if a folder exists within a Working Folder.
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public bool ContainsFolder(string folderName)
        {
            if (string.IsNullOrEmpty(folderName)) { throw new ArgumentException("Folder name is null or empty."); }
            
            return Directory.Exists(Path + "/" + folderName);
        }

        /// <summary>
        /// Gets a Project Folder by name from a Working Folder if it exists.
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public ProjectFolder GetProjectFolderByName(string folderName)
        {
            if (string.IsNullOrEmpty(folderName)) { throw new ArgumentException("Folder name is null or empty."); }

            var folder = ProjectFolders.Find(x => x.Name == folderName);
            if (folder == null)
            {
                throw new InvalidOperationException("The Working Folder does not contain a Project Folder with the input name.");
            }
            return folder;
        }
        #endregion
    }
}
