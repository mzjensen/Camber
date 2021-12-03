#region references
using System;
using System.IO;
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AcDatabase = Autodesk.AutoCAD.DatabaseServices.Database;
using Autodesk.DesignScript.Runtime;
using Dynamo.Graph.Nodes;
#endregion

namespace Camber.AutoCAD.External
{
    public sealed class ExternalDocument : IDisposable
    {
        #region properties
        protected AcDatabase AcDatabase { get; private set; }
        protected FileInfo FileInfo { get; private set; }
        
        private const string InvalidDirectoryPathMessage = "Directory path is null or empty.";
        private const string InvalidFileNameMessage = "File name is null or empty.";
        private const string InvalidFilePathMessage = "File path is null or empty.";
        private const string InvalidFileExtensionMessage = "File name should have a .DWG, .DWT, .DWS, or .DXF extension.";
        private const string InvalidTemplateFileExtensionMessage = "File name should have a .DWG, .DWT, or .DWS extension.";
        private const string FileExistsMessage = "A file with the same name already exists and will not be overwritten.";

        /// <summary>
        /// Gets the name of an External Document.
        /// </summary>
        public string Name => FileInfo.Name;

        /// <summary>
        /// Gets the directory path where an External Document is located.
        /// </summary>
        public string Directory => FileInfo.DirectoryName;

        /// <summary>
        /// Gets the full path to an External Document.
        /// </summary>
        public string Path => FileInfo.FullName;

        /// <summary>
        /// Gets the file extension of an External Document.
        /// </summary>
        public string Extension => FileInfo.Extension;

        /// <summary>
        /// Gets if an External Document needs to be recovered.
        /// </summary>
        public bool NeedsRecovery => AcDatabase.NeedsRecovery;

        /// <summary>
        /// Gets the number of objects in the database of an External Document.
        /// </summary>
        public int ObjectsCount => AcDatabase.ApproxNumObjects;

        /// <summary>
        /// Gets the External Blocks in an External Document.
        /// </summary>
        public IList<ExternalBlock> Blocks
        {
            get
            {
                List<ExternalBlock> blocks = new List<ExternalBlock>();
                acDb.Transaction t = AcDatabase.TransactionManager.StartTransaction();
                using (t)
                {
                    acDb.BlockTable bt = (acDb.BlockTable)t.GetObject(AcDatabase.BlockTableId, acDb.OpenMode.ForRead);
                    foreach (acDb.ObjectId oid in bt)
                    {
                        acDb.BlockTableRecord btr = (acDb.BlockTableRecord)t.GetObject(oid, acDb.OpenMode.ForRead);
                        if (btr != null)
                        {
                            blocks.Add(new ExternalBlock(btr));
                        }
                    }
                }
                return blocks;
            }
        }
        #endregion

        #region constructors
        internal ExternalDocument(AcDatabase acDatabase, string fileName)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(fileName);
                AcDatabase = acDatabase;
                FileInfo = fileInfo;
            }
            catch { throw; }
        }

        /// <summary>
        /// Creates and loads a new External Document from a template file.
        /// </summary>
        /// <param name="directoryPath">The path to a directory location for the new file.</param>
        /// <param name="fileName">The name of the new file. Be sure to append the desired file extension to the name.</param>
        /// <param name="templateFilePath">The path to a .DWG, .DWT, or .DWS file.</param>
        /// <param name="lock">True = file can be edited by other applications, False = file can only be opened as read-only by other applications.</param>
        /// <param name="overwrite">Overwrite if a file of the same name already exists in the specified directory?</param>
        /// <returns></returns>
        public static ExternalDocument CreateAndLoad(
            string directoryPath, 
            string fileName, 
            string templateFilePath, 
            bool @lock, 
            bool overwrite = false)
        {
            FileCreationChecks(directoryPath, fileName, templateFilePath, overwrite);

            try
            {
                string filePath = directoryPath + "\\" + fileName;
                using (var db = new AcDatabase(false, true))
                {
                    db.ReadDwgFile(templateFilePath, FileShare.Read, true, null);
                    db.SaveAs(filePath, acDb.DwgVersion.Current);
                }
                return LoadFromFile(filePath, @lock);
            }
            catch { throw; }
        }

        /// <summary>
        /// Loads an External Document from an existing file.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <param name="lock">True = file can be edited by other applications, False = file can only be opened as read-only by other applications.</param>
        /// <returns></returns>
        [NodeCategory("Actions")]
        public static ExternalDocument LoadFromFile(string filePath, bool @lock)
        {
            if (string.IsNullOrEmpty(filePath)) { throw new ArgumentNullException(InvalidFilePathMessage); }

            FileShare access = FileShare.ReadWrite;
            if (@lock) { access = FileShare.Read; }

            try
            {
                var db = new AcDatabase(false, true);
                db.ReadDwgFile(filePath, access, true, null);
                return new ExternalDocument(db, filePath);
            }
            catch
            {
                throw new InvalidOperationException("The file is currently being edited by another application and cannot be loaded.");
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"ExternalDocument(Name = {Name})";

        /// <summary>
        /// Implementation of IDisposable
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        public void Dispose() { AcDatabase = null; }

        /// <summary>
        /// Creates a new External Document from a template file.
        /// </summary>
        /// <param name="directoryPath">The path to a directory location for the new file.</param>
        /// <param name="fileName">The name of the new file. Be sure to append the desired file extension to the name.</param>
        /// <param name="templateFilePath">The path to a .DWG, .DWT, or .DWS file.</param>
        /// <param name="overwrite">Overwrite if a file of the same name already exists in the specified directory?</param>
        /// <returns></returns>
        [NodeCategory("Create")]
        public static void Create(string directoryPath, string fileName, string templateFilePath, bool overwrite = false)
        {
            FileCreationChecks(directoryPath, fileName, templateFilePath, overwrite);

            try
            {
                string filePath = directoryPath + "\\" + fileName;
                using (var db = new AcDatabase(false, true))
                {
                    db.ReadDwgFile(templateFilePath, FileShare.Read, true, null);
                    db.SaveAs(filePath, acDb.DwgVersion.Current);
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// Attempts to save an External Document as a new file.
        /// </summary>
        /// <param name="directoryPath">The path to a directory location for the new file.</param>
        /// <param name="fileName">The name of the new file. Be sure to append the desired file extension to the name.</param>
        /// <param name="overwrite">Overwrite if a file of the same name already exists in the specified directory?</param>
        /// <returns></returns>
        public ExternalDocument SaveAs(string directoryPath, string fileName, bool overwrite = false)
        {
            // Check inputs
            if (string.IsNullOrEmpty(directoryPath)) { throw new ArgumentException(InvalidDirectoryPathMessage); }
            if (string.IsNullOrEmpty(fileName)) { throw new ArgumentException(InvalidFileNameMessage); }

            // Check if directory exists
            if (!System.IO.Directory.Exists(directoryPath))
            {
                throw new ArgumentNullException("Invalid directory.");
            }

            // Check extensions
            if (!HasValidExtension(fileName)) { throw new ArgumentException(InvalidFileExtensionMessage); }

            // Check if file with same name already exists
            if (File.Exists(directoryPath + "\\" + fileName)
                && overwrite == false)
            {
                throw new InvalidOperationException(FileExistsMessage);
            }

            try
            {
                AcDatabase.SaveAs(directoryPath + "\\" + fileName, acDb.DwgVersion.Current);
                FileInfo newFileInfo = new FileInfo(AcDatabase.Filename);
                FileInfo = newFileInfo;
                return this;
            }
            catch
            {
                throw new InvalidOperationException("Cannot save file.");
            }
        }

        /// <summary>
        /// Attempts to save an External Document.
        /// </summary>
        /// <returns></returns>
        public ExternalDocument Save()
        {
            AcDatabase.SaveAs(Path, acDb.DwgVersion.Current);
            return this;
        }

        /// <summary>
        /// Check file name for valid extensions.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static bool HasValidExtension(string fileName)
        {
            // Check extensions
            if (!fileName.Contains(".dwg")
                && !fileName.Contains(".dwt")
                && !fileName.Contains(".dws")
                && !fileName.Contains(".dxf"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Base method for checking various items when creating new files.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="fileName"></param>
        /// <param name="templateFilePath"></param>
        /// <param name="overwrite"></param>
        private static void FileCreationChecks(string directoryPath, string fileName, string templateFilePath, bool overwrite = false)
        {
            // Check inputs
            if (string.IsNullOrEmpty(directoryPath)) { throw new ArgumentException(InvalidDirectoryPathMessage); }
            if (string.IsNullOrEmpty(fileName)) { throw new ArgumentException(InvalidFileNameMessage); }
            if (!File.Exists(templateFilePath)) { throw new ArgumentException("A valid file does not exist at the template file path."); }

            // Check for valid DWT file
            FileInfo fileInfo = new FileInfo(templateFilePath);
            if (fileInfo.Extension != ".dwg"
                && fileInfo.Extension != ".dwt"
                && fileInfo.Extension != ".dws")
            {
                throw new ArgumentException(InvalidTemplateFileExtensionMessage);
            }

            // Check extension for new file
            if (!HasValidExtension(fileName)) { throw new ArgumentException(InvalidFileExtensionMessage); }

            // Check if file with same name already exists
            if (File.Exists(directoryPath + "\\" + fileName)
                && overwrite == false)
            {
                throw new InvalidOperationException(FileExistsMessage);
            }
        }
        #endregion
    }
}