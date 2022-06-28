#region references
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using acApp = Autodesk.AutoCAD.ApplicationServices;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using AcDatabase = Autodesk.AutoCAD.DatabaseServices.Database;
using Autodesk.DesignScript.Runtime;
using Camber.External.ExternalObjects;
using Dynamo.Graph.Nodes;
#endregion

namespace Camber.External
{
    public sealed class ExternalDocument : IDisposable
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
        /// Gets the version of an ExternalDocument when it was last saved with the current session.
        /// Returns null if the ExternalDocument has not been saved with the current session.
        /// Returns 0 for any version prior to 2004.
        /// </summary>
        public int? LastSavedVersion 
            => AutoCAD.Document.GetDatabaseVersionInfo(
            AcDatabase, 
            true);

        /// <summary>
        /// Gets the version of an ExternalDocument when it was first loaded.
        /// Returns 0 for any version prior to 2004.
        /// </summary>
        public int? OriginalVersion
            => AutoCAD.Document.GetDatabaseVersionInfo(
                AcDatabase, 
                false);

        /// <summary>
        /// Gets if an External Document needs to be recovered.
        /// </summary>
        public bool NeedsRecovery => AcDatabase.NeedsRecovery;

        /// <summary>
        /// Gets the number of objects in the database of an External Document.
        /// </summary>
        public int ObjectsCount => AcDatabase.ApproxNumObjects;

        /// <summary>
        /// Gets the Model Space block of an External Document.
        /// </summary>
        public ExternalBlock ModelSpace
        {
            get
            {
                ExternalBlock retBlk = null;
                using (var t = AcDatabase.TransactionManager.StartTransaction())
                {
                    acDb.BlockTable bt = (acDb.BlockTable)t.GetObject(AcDatabase.BlockTableId, acDb.OpenMode.ForRead);
                    acDb.BlockTableRecord btr = (acDb.BlockTableRecord)t.GetObject(bt[acDb.BlockTableRecord.ModelSpace], acDb.OpenMode.ForRead);
                    return new ExternalBlock(btr);
                }
            }
        }

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

        /// <summary>
        /// Gets the External Layers in an External Document.
        /// </summary>
        public IList<ExternalLayer> Layers
        {
            get
            {
                List<ExternalLayer> layers = new List<ExternalLayer>();
                acDb.Transaction t = AcDatabase.TransactionManager.StartTransaction();
                using (t)
                {
                    acDb.LayerTable lt = (acDb.LayerTable)t.GetObject(
                        AcDatabase.LayerTableId, 
                        acDb.OpenMode.ForRead);
                    foreach (acDb.ObjectId oid in lt)
                    {
                        acDb.LayerTableRecord ltr = (acDb.LayerTableRecord)t.GetObject(
                            oid, 
                            acDb.OpenMode.ForRead);
                        if (ltr != null)
                        {
                            layers.Add(new ExternalLayer(ltr));
                        }
                    }
                }
                return layers;
            }
        }
        #endregion

        #region constructors
        internal ExternalDocument(AcDatabase acDatabase, string filePath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
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

                // Using false here for buildDefaultDrawing because we are reading from a template file.
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
        /// <param name="lock">True = file can only be opened as read-only by other applications, False = file can be edited by other applications.</param>
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

                // Using false here for buildDefaultDrawing because we are reading from a template file.
                using (var db = new AcDatabase(false, true))
                {
                    db.ReadDwgFile(templateFilePath, FileShare.Read, true, null);
                    db.SaveAs(filePath, acDb.DwgVersion.Current);
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// Saves a copy of an External Document under a new name. Returns the original External Document, not the newly-created copy.
        /// </summary>
        /// <param name="directoryPath">The path to a directory location for the new file.</param>
        /// <param name="fileName">The name of the new file. Be sure to append the desired file extension to the name.</param>
        /// <param name="overwrite">Overwrite if a file of the same name already exists in the specified directory?</param>
        /// <returns></returns>
        public ExternalDocument SaveCopy(string directoryPath, string fileName, bool overwrite = false)
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
        /// Gets an External Block from an External Document by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ExternalBlock BlockByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException("Invalid name.");
            }

            return Blocks.FirstOrDefault(item => item.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets an External Layer from an External Document by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ExternalLayer LayerByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException("Invalid name.");
            }

            return Layers.FirstOrDefault(item => item.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the value of a system variable.
        /// </summary>
        /// <param name="variableName">The name of the system variable.</param>
        /// <returns></returns>
        public object GetSystemVariable(string variableName)
        {
            acDb.HostApplicationServices.WorkingDatabase = AcDatabase;
            var sysVar = acApp.Application.GetSystemVariable(variableName);
            acDb.HostApplicationServices.WorkingDatabase = acDynNodes.Document.Current.AcDocument.Database;
            return sysVar;
        }

        /// <summary>
        /// Sets the value of a system variable.
        /// </summary>
        /// <param name="variableName">The name of the system variable.</param>
        /// <param name="newValue">The new value to assign.</param>
        /// <returns></returns>
        public ExternalDocument SetSystemVariable(string variableName, object newValue)
        {
            // AutoCAD needs 16-bit integers, but from Dynamo they come as 64-bit.
            // Without this check, an eInvalidInput exception will be thrown when trying to set integer values.
            if (newValue is long) { newValue = Convert.ToInt16(newValue); }

            try
            {
                acDb.HostApplicationServices.WorkingDatabase = AcDatabase;
                acApp.Application.SetSystemVariable(variableName, newValue);
                acDb.HostApplicationServices.WorkingDatabase = acDynNodes.Document.Current.AcDocument.Database;
                return this;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
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
        /// Helper method for checking various items when creating new files.
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

        /// <summary>
        /// Ensures that a specified layer exists in an External Document. If not, create a new layer.
        /// </summary>
        /// <param name="externalDocument"></param>
        /// <param name="layer"></param>
        [IsVisibleInDynamoLibrary(false)]
        public static void EnsureLayer(ExternalDocument externalDocument, string layer)
        {
            if (string.IsNullOrEmpty(layer)) { throw new ArgumentNullException("layer"); }

            using (var tr = externalDocument.AcDatabase.TransactionManager.StartTransaction())
            {
                acDb.LayerTable lt = (acDb.LayerTable)tr.GetObject(externalDocument.AcDatabase.LayerTableId, acDb.OpenMode.ForWrite);
                if (!lt.Has(layer))
                {
                    acDb.LayerTableRecord ltr = new acDb.LayerTableRecord();
                    ltr.Name = layer;
                    lt.Add(ltr);
                    tr.AddNewlyCreatedDBObject(ltr, true);
                }
                tr.Commit();
            }
        }
        #endregion
    }
}