#region references
using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using civDs = Autodesk.Civil.DataShortcuts;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using AeccDataShortcuts = Autodesk.Civil.DataShortcuts.DataShortcuts;
using Camber.AutoCAD;
using Autodesk.DesignScript.Runtime;
using Camber.AutoCAD.External;
#endregion

namespace Camber.Civil.DataShortcuts
{
    public sealed class DataShortcutProject
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
        /// Gets the folder path of a Data Shortcut Project.
        /// </summary>
        public string Path => DirectoryInfo.FullName;

        /// <summary>
        /// Gets the ID of a Data Shortcut Project.
        /// </summary>
        public string ID => AeccDataShortcuts.GetDSProjectId(Path);

        /// <summary>
        /// Gets the name of a Data Shortcut Project.
        /// </summary>
        public string Name => DirectoryInfo.Name;

        /// <summary>
        /// Gets the description of a Data Shortcut Project.
        /// </summary>
        public string Description => AeccDataShortcuts.GetDescriptionDataShorcutProjectFolder(Name);

        /// <summary>
        /// Gets if a Data Shortcut Project is set as the current project.
        /// </summary>
        public bool IsCurrent => AeccDataShortcuts.GetCurrentProjectFolder() == Name ? true : false;

        /// <summary>
        /// Gets the Data Shortcuts in a Data Shortcut Project.
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
        internal DataShortcutProject(string name)
        {
            DirectoryInfo = new DirectoryInfo(Civil.DataShortcuts.DataShortcuts.GetDSWorkingFolder().Path + @"\" + name);
        }

        /// <summary>
        /// Creates a new Data Shortcut Project and folder.
        /// </summary>
        /// <param name="name">The name of the project.</param>
        /// <param name="description">The description of the project.</param>
        /// <param name="templatePath">The full path to a project template. If left blank, a template will not be used.</param>
        /// <param name="setAsCurrent">Set the newly-created project as the current project?</param>
        /// <param name="overwrite">Overwrite project folder if it already exists?</param>
        /// <returns></returns>
        public static DataShortcutProject ByName(
            DataShortcutWorkingFolder workingFolder,
            string name,
            string description = "",
            string templatePath = "",
            bool setAsCurrent = false,
            bool overwrite = false)
        {
            if (string.IsNullOrEmpty(name)) { throw new ArgumentException(InvalidFolderNameMsg); }

            if (!overwrite && workingFolder.ContainsFolder(name))
            {
                throw new InvalidOperationException("A project with that name already exists.");
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
                Civil.DataShortcuts.DataShortcuts.Refresh();

                // Check if the newly created folder exists
                if (!workingFolder.ContainsFolder(name))
                {
                    throw new InvalidOperationException("Failed to create project.");
                }
                return new DataShortcutProject(name);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"DataShortcutProject(Name = {Name})";

        /// <summary>
        /// Sets a Data Shortcut Project as the current project.
        /// </summary>
        public DataShortcutProject SetAsCurrent()
        {
            try
            {
                AeccDataShortcuts.SetCurrentProjectFolder(Name);
                Civil.DataShortcuts.DataShortcuts.Refresh();
                return this;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        /// <summary>
        /// Sets the description of a Data Shortcut Project.
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        private DataShortcutProject SetDescription(string description)
        {
            // While this should work in theory, the returned value for the project description
            // does not change after manually editing the description in the XML file.

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
        /// Gets Data Shortcuts from the current Data Shortcut Project. Each input can be used as a filter.
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

            IList<DataShortcut> shortcuts = Civil.DataShortcuts.DataShortcuts.GetCurrentDSProject().DataShortcuts;

            IQueryable<DataShortcut> q = shortcuts.AsQueryable();

            if (!string.IsNullOrEmpty(entityType))
            {
                q = q.Where(p => p.EntityType == entityType);
            }
            if (!string.IsNullOrEmpty(name))
            {
                q = q.Where(p => p.Name.Contains(name));
            }
            if (!string.IsNullOrEmpty(description))
            {
                q = q.Where(p => p.Description.Contains(description));
            }
            if (!string.IsNullOrEmpty(sourceFileName))
            {
                q = q.Where(p => p.SourceFileName.Contains(sourceFileName));
            }
            if (!string.IsNullOrEmpty(sourceLocation))
            {
                q = q.Where(p => p.SourceLocation.Contains(sourceLocation));
            }

            return q.ToList();
        }

        /// <summary>
        /// Associates a Data Shortcut Project to the current document.
        /// </summary>
        /// <param name="save">Save current document after association is complete?</param>
        /// <returns></returns>
        public DataShortcutProject Associate(acDynNodes.Document document, bool save)
        {
            if (save && !Document.IsNamedDrawing(document))
            {
                throw new InvalidOperationException("The current drawing is a new drawing and has not yet been saved. Please save the drawing and try again.");
            }

            try
            {
                AeccDataShortcuts.AssociateDSProject(ID, document.AcDocument.Database, save);
                Civil.DataShortcuts.DataShortcuts.Refresh();
                return this;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        /// <summary>
        /// Associates a Data Shortcut Project to an External Document.
        /// </summary>
        /// <param name="externalDocument"></param>
        /// <param name="save">Save External Document after association is complete?</param>
        /// <returns></returns>
        public DataShortcutProject Associate(ExternalDocument externalDocument, bool save)
        {
            try
            {
                AeccDataShortcuts.AssociateDSProject(ID, externalDocument.AcDatabase, save);
                return this;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }
        #endregion
    }
}
