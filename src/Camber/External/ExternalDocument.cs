using Autodesk.Civil.Settings;
using Autodesk.DesignScript.Runtime;
using Camber.External.ExternalObjects;
using Dynamo.Graph.Nodes;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using acApp = Autodesk.AutoCAD.ApplicationServices;
using AcDatabase = Autodesk.AutoCAD.DatabaseServices.Database;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using civApp = Autodesk.Civil.ApplicationServices;

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
                    var bt = (acDb.BlockTable)t.GetObject(AcDatabase.BlockTableId, acDb.OpenMode.ForRead);
                    var btr = (acDb.BlockTableRecord)t.GetObject(bt[acDb.BlockTableRecord.ModelSpace], acDb.OpenMode.ForRead);
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
                var blocks = new List<ExternalBlock>();
                var t = AcDatabase.TransactionManager.StartTransaction();
                using (t)
                {
                    var bt = (acDb.BlockTable)t.GetObject(AcDatabase.BlockTableId, acDb.OpenMode.ForRead);
                    foreach (var oid in bt)
                    {
                        var btr = (acDb.BlockTableRecord)t.GetObject(oid, acDb.OpenMode.ForRead);
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
                var layers = new List<ExternalLayer>();
                var t = AcDatabase.TransactionManager.StartTransaction();
                using (t)
                {
                    var lt = (acDb.LayerTable)t.GetObject(
                        AcDatabase.LayerTableId, 
                        acDb.OpenMode.ForRead);
                    foreach (var oid in lt)
                    {
                        var ltr = (acDb.LayerTableRecord)t.GetObject(
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

        /// <summary>
        /// Gets all of the named page setups in an External Document.
        /// </summary>
        public IList<string> NamedPageSetups
        {
            get
            {
                var setups = new List<string>();
                try
                {
                    using (var tr = AcDatabase.TransactionManager.StartTransaction())
                    {
                        var plSets = (acDb.DBDictionary) tr.GetObject(AcDatabase.PlotSettingsDictionaryId,
                            acDb.OpenMode.ForRead);

                        foreach (var entry in plSets)
                        {
                            var plSet = (acDb.PlotSettings) tr.GetObject(entry.Value, acDb.OpenMode.ForRead);
                            if (plSet.ModelType)
                            {
                                continue;
                            }

                            setups.Add(plSet.PlotSettingsName);
                        }
                    }
                    return setups;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
            }
        }

        /// <summary>
        /// Gets the list of available coordinate system codes in an External Document.
        /// </summary>
        public List<string> CoordinateSystemCodes
        {
            get
            {
                var codes = SettingsUnitZone.GetAllCodes().ToList();
                codes.Sort();
                return codes;
            }
        }
        #endregion

        #region constructors
        internal ExternalDocument(AcDatabase acDatabase, string filePath)
        {
            try
            {
                var fileInfo = new FileInfo(filePath);
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
        /// <returns></returns>
        public static ExternalDocument CreateAndLoad(
            string directoryPath,
            string fileName,
            string templateFilePath,
            bool overwrite = false)
        {
            FileCreationChecks(directoryPath, fileName, templateFilePath, overwrite);

            try
            {
                var filePath = directoryPath + "\\" + fileName;

                // Using false here for buildDefaultDrawing because we are reading from a template file.
                using (var db = new AcDatabase(false, true))
                {
                    db.ReadDwgFile(templateFilePath, FileShare.Read, true, null);
                    db.SaveAs(filePath, acDb.DwgVersion.Current);
                }
                return LoadFromFile(filePath);
            }
            catch { throw; }
        }

        /// <summary>
        /// Loads an External Document from an existing file.
        ///  Please note that this node does not prevent other applications
        ///  from opening and/or modifying the file at the same time. 
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <returns></returns>
        [NodeCategory("Actions")]
        public static ExternalDocument LoadFromFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(InvalidFilePathMessage);
            }

            if (!File.Exists(filePath))
            {
                throw new ArgumentException("Invalid file path.");
            }

            if (!HasValidExtension(filePath))
            {
                throw new ArgumentException(InvalidFileExtensionMessage);
            }

            try
            {
                var db = new AcDatabase(false, true);
                db.ReadDwgFile(filePath, acDb.FileOpenMode.OpenForReadAndAllShare, true, null);
                return new ExternalDocument(db, filePath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
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
                var filePath = directoryPath + "\\" + fileName;

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
                var newFileInfo = new FileInfo(AcDatabase.Filename);
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
            try
            {
                AcDatabase.SaveAs(Path, acDb.DwgVersion.Current);
                return this;
            }
            catch
            {
                throw new InvalidOperationException("The file is currently in use by another application and cannot be saved.");
            }
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
        /// Gets an External Layout in an External Document by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ExternalLayout LayoutByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new InvalidOperationException("Layout name is null or empty.");
            }

            return Layouts(true).FirstOrDefault(
                item => item.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets all of the External Layouts in an External Document.
        /// </summary>
        /// <param name="includeModel">Include Model Space?</param>
        /// <returns></returns>
        public IList<ExternalLayout> Layouts(bool includeModel = false)
        {
            var layouts = new List<ExternalLayout>();
            try
            {
                using (var tr = AcDatabase.TransactionManager.StartTransaction())
                {
                    var layoutDict = (acDb.DBDictionary)tr.GetObject(AcDatabase.LayoutDictionaryId, acDb.OpenMode.ForRead);
                    foreach (var layoutEntry in layoutDict)
                    {
                        if (layoutEntry.Key.ToUpper() == "MODEL" && !includeModel)
                        {
                            continue;
                        }

                        var acLayout = (acDb.Layout)tr.GetObject(layoutEntry.Value, acDb.OpenMode.ForRead);

                        layouts.Add(new ExternalLayout(acLayout));
                    }
                    return layouts;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
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
                var lt = (acDb.LayerTable)tr.GetObject(externalDocument.AcDatabase.LayerTableId, acDb.OpenMode.ForWrite);
                if (!lt.Has(layer))
                {
                    var ltr = new acDb.LayerTableRecord();
                    ltr.Name = layer;
                    lt.Add(ltr);
                    tr.AddNewlyCreatedDBObject(ltr, true);
                }
                tr.Commit();
            }
        }

        /// <summary>
        /// Gets the units and zone settings for an External Document.
        /// </summary>
        /// <returns></returns>
        [MultiReturn(
            "Drawing Units",
            "Angular Units",
            "Coordinate System Code",
            "Imperial to Metric Conversion",
            "Drawing Scale",
            "Scale Objects from Other Drawings",
            "Set AutoCAD Variables to Match")]
        public Dictionary<string, object> GetUnitsZoneSettings()
        {
            using (var tr = AcDatabase.TransactionManager.StartTransaction())
            {
                var cdoc = civApp.CivilDocument.GetCivilDocument(AcDatabase);
                var settings = cdoc.Settings.DrawingSettings.UnitZoneSettings;
                return new Dictionary<string, object>
                {
                    { "Drawing Units", settings.DrawingUnits.ToString() },
                    { "Angular Units", settings.AngularUnits.ToString() },
                    { "Coordinate System Code", settings.CoordinateSystemCode },
                    { "Imperial to Metric Conversion", settings.ImperialToMetricConversion.ToString() },
                    { "Drawing Scale", settings.DrawingScale },
                    { "Scale Objects from Other Drawings", settings.ScaleObjectsFromOtherDrawings },
                    { "Set AutoCAD Variables to Match", settings.MatchAutoCADVariables }
                };
            }

        }

        /// <summary>
        /// Sets the units (feet or meters) for an External Document.
        /// </summary>
        /// <param name="useFeet">If true, the units will be set to feet. If false, they will be set to metric.</param>
        /// <returns></returns>
        public ExternalDocument SetDrawingUnits(bool useFeet)
        {
            try
            {
                using (var tr = AcDatabase.TransactionManager.StartTransaction())
                {
                    var cdoc = civApp.CivilDocument.GetCivilDocument(AcDatabase);
                    var settings = cdoc.Settings.DrawingSettings.UnitZoneSettings;
                    var units = DrawingUnitType.Meters;
                    if (useFeet)
                    {
                        units = DrawingUnitType.Feet;
                    }
                    settings.DrawingUnits = units;
                }
                return this;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Sets the angular units for an External Document.
        /// </summary>
        /// <param name="angularUnitType">Degree, Grad, or Radian</param>
        /// <returns></returns>
        public ExternalDocument SetAngularUnits(string angularUnitType)
        {
            try
            {
                using (var tr = AcDatabase.TransactionManager.StartTransaction())
                {
                    var cdoc = civApp.CivilDocument.GetCivilDocument(AcDatabase);
                    var settings = cdoc.Settings.DrawingSettings.UnitZoneSettings;
                    if (!Enum.TryParse(angularUnitType, out Autodesk.Civil.AngleUnitType unitType))
                    {
                        throw new InvalidOperationException("Invalid angular units type.");
                    }
                    settings.AngularUnits = unitType;
                }
                return this;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Sets the coordinate system code for an External Document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ExternalDocument SetCoordinateSystemCode(string code)
        {
            try
            {
                using (var tr = AcDatabase.TransactionManager.StartTransaction())
                {
                    var cdoc = civApp.CivilDocument.GetCivilDocument(AcDatabase);
                    var settings = cdoc.Settings.DrawingSettings.UnitZoneSettings;
                    settings.CoordinateSystemCode = code;
                }
                return this;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Sets the imperial to metric conversion type for an External Document.
        /// </summary>
        /// <param name="useInternationalFoot">
        /// If true, the conversion type will be set to International Foot.
        ///  If false, it will be set to US Survey Foot.
        /// </param>
        /// <returns></returns>
        public ExternalDocument SetImperialToMetricConversion(bool useInternationalFoot)
        {
            try
            {
                using (var tr = AcDatabase.TransactionManager.StartTransaction())
                {
                    var cdoc = civApp.CivilDocument.GetCivilDocument(AcDatabase);
                    var settings = cdoc.Settings.DrawingSettings.UnitZoneSettings;
                    var conversionType = ImperialToMetricConversionType.UsSurveyFoot;
                    if (useInternationalFoot)
                    {
                        conversionType = ImperialToMetricConversionType.InternationalFoot;
                    }
                    settings.ImperialToMetricConversion = conversionType;
                }
                return this;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Sets the scale for an External Document.
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        public ExternalDocument SetDrawingScale(double scale)
        {
            try
            {
                using (var tr = AcDatabase.TransactionManager.StartTransaction())
                {
                    var cdoc = civApp.CivilDocument.GetCivilDocument(AcDatabase);
                    var settings = cdoc.Settings.DrawingSettings.UnitZoneSettings;
                    settings.DrawingScale = scale;
                }
                return this;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Toggles whether to scale objects when inserted from other drawings into an External Document.
        /// </summary>
        /// <param name="scaleObjects"></param>
        /// <returns></returns>
        public ExternalDocument SetScaleObjectsFromOtherDrawings(bool scaleObjects)
        {
            try
            {
                using (var tr = AcDatabase.TransactionManager.StartTransaction())
                {
                    var cdoc = civApp.CivilDocument.GetCivilDocument(AcDatabase);
                    var settings = cdoc.Settings.DrawingSettings.UnitZoneSettings;
                    settings.ScaleObjectsFromOtherDrawings = scaleObjects;
                }
                return this;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Toggles whether to set AutoCAD variables to match for an External Document.
        /// </summary>
        /// <param name="matchVariables"></param>
        /// <returns></returns>
        public ExternalDocument SetAutoCADVariablesToMatch(bool matchVariables)
        {
            try
            {
                using (var tr = AcDatabase.TransactionManager.StartTransaction())
                {
                    var cdoc = civApp.CivilDocument.GetCivilDocument(AcDatabase);
                    var settings = cdoc.Settings.DrawingSettings.UnitZoneSettings;
                    settings.MatchAutoCADVariables = matchVariables;
                }
                return this;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
        #endregion

        #region private methods
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
            var fileInfo = new FileInfo(templateFilePath);
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
        /// Determines if a file is locked or not.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>
        /// true if the file is currently being written to,
        /// is being processed by another thread,
        /// or does not exist;
        /// false if not locked.</returns>
        private static bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                return true;
            }

            return false;
        }
        #endregion
    }
}