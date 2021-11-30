#region references
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acApp = Autodesk.AutoCAD.ApplicationServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using AcDatabase = Autodesk.AutoCAD.DatabaseServices.Database;
using Dynamo.Graph.Nodes;
#endregion

namespace Camber.AutoCAD
{
    public class ExternalDocument
    {
        #region properties
        internal AcDatabase AcDatabase { get; private set; }
        internal FileInfo FileInfo { get; private set; }
        private const string InvalidDirectoryPathMessage = "Directory path is null or empty.";
        private const string InvalidFileNameMessage = "File name is null or empty.";
        private const string InvalidFilePathMessage = "File path is null or empty.";
        private const string InvalidFileExtensionMessage = "File name should have a .DWG, .DWT, .DWS, or .DXF extension.";
        private const string InvalidTemplateFileExtensionMessage = "File name should have a .DWG, .DWT, or .DWS extension.";
        private const string FileExistsMessage = "A file with the same name already exists and will not be overwritten.";

        public string Name => FileInfo.Name;
        public string Directory => FileInfo.DirectoryName;
        public string Path => FileInfo.FullName;
        public string Extension => FileInfo.Extension;
        public bool NeedsRecovery => AcDatabase.NeedsRecovery;
        public int ObjectsCount => AcDatabase.ApproxNumObjects;
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
        /// <param name="overwrite">Overwrite if a file of the same name already exists in the specified directory?</param>
        /// <param name="readOnly">True = load file as read-only, False = load file with read/write access.</param>
        /// <returns></returns>
        public static ExternalDocument CreateAndLoad(string directoryPath, string fileName, string templateFilePath, bool overwrite = false, bool readOnly = true)
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
                return LoadFromFile(filePath, readOnly);
            }
            catch { throw; }
        }

        /// <summary>
        /// Loads an External Document from an existing file.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <param name="readOnly">True = load file as read-only, False = load file with read/write access.</param>
        /// <returns></returns>
        [NodeCategory("Actions")]
        public static ExternalDocument LoadFromFile(string filePath, bool readOnly = true)
        {
            if (string.IsNullOrEmpty(filePath)) { throw new ArgumentNullException(InvalidFilePathMessage); }

            FileShare openMode = FileShare.ReadWrite;
            if (readOnly) { openMode = FileShare.Read; }
            
            try
            {
                var db = new AcDatabase(false, true);
                db.ReadDwgFile(filePath, openMode, true, null);
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
        /// Saves an External Document as a new file.
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
            catch { throw; }
        }

        /// <summary>
        /// Saves an External Document.
        /// </summary>
        /// <returns></returns>
        public ExternalDocument Save()
        {
            try
            {
                AcDatabase.SaveAs(Path, acDb.DwgVersion.Current);
                return this;
            }
            catch { throw; }
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