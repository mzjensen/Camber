#region references
using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using civDs = Autodesk.Civil.DataShortcuts;
using AeccDataShortcuts = Autodesk.Civil.DataShortcuts.DataShortcuts;
using Autodesk.DesignScript.Runtime;

#endregion

namespace Camber.Civil.DataShortcuts
{
    public sealed class ProjectFolder
    {
        #region fields
        private DirectoryInfo _directoryInfo;
        #endregion

        #region properties
        protected const string InvalidFolderNameMsg = "Folder name is null or empty.";

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
        /// Gets the full path of a Project Folder.
        /// </summary>
        public string Path => DirectoryInfo.FullName;

        /// <summary>
        /// Gets the directory name of a Project Folder.
        /// </summary>
        public string Name => DirectoryInfo.Name;

        /// <summary>
        /// Gets the description of a Project Folder.
        /// </summary>
        public string Description => AeccDataShortcuts.GetDescriptionDataShorcutProjectFolder(Name);

        /// <summary>
        /// Gets if a Project Folder is set as the current Project Folder for the application.
        /// </summary>
        public bool IsCurrent => AeccDataShortcuts.GetCurrentProjectFolder() == Name ? true : false;

        /// <summary>
        /// Gets the Data Shortcuts in a Project Folder.
        /// </summary>
        private IList<DataShortcut> DataShortcuts
        {
            get
            {
                bool isValidCreation = false;
                var manager = AeccDataShortcuts.CreateDataShortcutManager(ref isValidCreation);
                var dataShortcuts = new List<DataShortcut>();
                if (isValidCreation)
                {
                    var publishedItemsCount = manager.GetPublishedItemsCount();
                    for (int i = 0; i < publishedItemsCount; i++)
                    {
                        dataShortcuts.Add(new DataShortcut(manager.GetPublishedItemAt(i)));
                    }
                }
                return dataShortcuts;
            }
        }
        #endregion

        #region constructors
        internal ProjectFolder(string name)
        {
            DirectoryInfo = new DirectoryInfo(CivilApplication.WorkingFolder().Path + @"\" + name);
        }

        /// <summary>
        /// Creates a new Data Shortcuts Project Folder.
        /// </summary>
        /// <param name="name">The name of the folder.</param>
        /// <param name="description">The description of the folder.</param>
        /// <param name="templatePath">The full path to a project template. If left blank, a template will not be used.</param>
        /// <param name="setAsCurrent">Set the current project folder to the newly created one?</param>
        /// <param name="overwrite">Overwrite project folder if it already exists?</param>
        /// <returns></returns>
        public static ProjectFolder ByName(
            WorkingFolder workingFolder,
            string name,
            string description = "",
            string templatePath = "",
            bool setAsCurrent = false,
            bool overwrite = false)
        {
            if (string.IsNullOrEmpty(name)) { throw new ArgumentException(InvalidFolderNameMsg); }

            if (!overwrite && workingFolder.ContainsFolder(name))
            {
                throw new InvalidOperationException("A folder with that name already exists.");
            }

            try
            {
                // Check if template path is provided
                if (string.IsNullOrEmpty(templatePath))
                {
                    AeccDataShortcuts.CreateProjectFolder(name, description, setAsCurrent);
                }

                // Create the folder
                AeccDataShortcuts.CreateProjectFolder(name, description, templatePath, setAsCurrent);
                CivilApplication.RefreshDataShortcuts();

                // Check if the newly created folder exists
                if (!workingFolder.ContainsFolder(name))
                {
                    throw new InvalidOperationException("Failed to create Project Folder.");
                }
                return new ProjectFolder(name);
            }
            catch { throw; }
        }
        #endregion

        #region methods
        public override string ToString() => $"ProjectFolder(Name = {Name})";

        /// <summary>
        /// Sets a Project Folder as the current Data Shortcuts Project Folder.
        /// </summary>
        /// <param name="folderName"></param>
        [IsLacingDisabled]
        public ProjectFolder SetAsCurrent()
        {
            try
            {
                AeccDataShortcuts.SetCurrentProjectFolder(Name);
                CivilApplication.RefreshDataShortcuts();
                return this;
            }
            catch { throw; }
        }

        /// <summary>
        /// Sets the description of a Project Folder.
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public ProjectFolder SetDescription(string description)
        {
            string path = Path + @"\_Shortcuts\ShortcutsHistory.xml";

            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNodeList nodes = doc.SelectNodes("/History/ShortProjectID");

            foreach (XmlNode node in nodes)
            {
                XmlAttribute descAttr = node.Attributes["desc"];

                if (descAttr != null)
                {
                    string currentValue = descAttr.Value;
                    if (string.IsNullOrEmpty(currentValue))
                    {
                        descAttr.Value = description;
                    }
                }
            }
            doc.Save(path);
            return this;
        }

        /// <summary>
        /// Gets Data Shortcuts from the current Project Folder. Each input can be used as a filter.
        /// </summary>
        /// <param name="entityType">Filter by entity type.</param>
        /// <param name="name">Filter by object name.</param>
        /// <param name="description">Filter by object description.</param>
        /// <param name="sourceFileName">Filter by source file name.</param>
        /// <param name="sourceLocation">Filter by source file location.</param>
        /// <returns></returns>
        public static IList<DataShortcut> GetDataShortcuts(
            string entityType = "", 
            string name = "", 
            string description = "", 
            string sourceFileName = "", 
            string sourceLocation = "")
        {
            if (!string.IsNullOrEmpty(entityType) && !Enum.IsDefined(typeof(civDs.DataShortcutEntityType), entityType))
            {
                throw new ArgumentException("Invalid entity type.");
            }

            IList<DataShortcut> shortcuts = CivilApplication.ProjectFolder().DataShortcuts;

            IQueryable<DataShortcut> q = shortcuts.AsQueryable();

            if (!string.IsNullOrEmpty(entityType))
            {
                q = q.Where(p => p.EntityType == entityType);
            }
            if (!string.IsNullOrEmpty(name))
            {
                q = q.Where(p => p.Name == name);
            }
            if (!string.IsNullOrEmpty(description))
            {
                q = q.Where(p => p.Description == description);
            }
            if (!string.IsNullOrEmpty(sourceFileName))
            {
                q = q.Where(p => p.SourceFileName == sourceFileName);
            }
            if (!string.IsNullOrEmpty(sourceLocation))
            {
                q = q.Where(p => p.SourceLocation == sourceLocation);
            }

            return q.ToList();
        }
        #endregion
    }
}
