using Autodesk.Civil.Settings;
using Autodesk.DesignScript.Runtime;
using Camber.AutoCAD.Objects;
using Camber.AutoCAD.Objects.MultiViewBlocks;
using Dynamo.Graph.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Civil;
using acApp = Autodesk.AutoCAD.ApplicationServices;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using aecDb = Autodesk.Aec.DatabaseServices;
using civApp = Autodesk.Civil.ApplicationServices;

namespace Camber.AutoCAD
{
    public static class Document
    {
        #region query methods
        /// <summary>
        /// Gets if a Document is a named file on the disk (as opposed to a new drawing that has not yet been saved).
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static bool IsNamedDrawing(this acDynNodes.Document document)
        {
            return document.AcDocument.IsNamedDrawing;
        }

        /// <summary>
        /// Gets if a Document is read-only.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static bool IsReadOnly(this acDynNodes.Document document)
        {
            return document.AcDocument.IsReadOnly;
        }

        /// <summary>
        /// Gets the version of a Document when it was last saved with the current session.
        /// Returns null if the Document has not been saved with the current session.
        /// Returns 0 for any version prior to 2004.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static int? LastSavedVersion(this acDynNodes.Document document)
            => GetDatabaseVersionInfo(document.AcDocument.Database, true);

        /// <summary>
        /// Gets the version of a Document when it was first opened.
        /// Returns 0 for any version prior to 2004.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static int? OriginalVersion(this acDynNodes.Document document)
            => GetDatabaseVersionInfo(document.AcDocument.Database, false);

        /// <summary>
        /// Gets the Multi-View Block definitions in a Document.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static List<MultiViewBlock> MultiViewBlocks(this acDynNodes.Document document)
        {
            List<MultiViewBlock> mvBlks = new List<MultiViewBlock>();

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                var mvDict = new aecDb.DictionaryMultiViewBlockDefinition(ctx.Database);
                foreach (acDb.ObjectId oid in mvDict.Records)
                {
                    var mvBlk = (aecDb.MultiViewBlockDefinition) ctx.Transaction.GetObject(oid, acDb.OpenMode.ForRead);
                    mvBlks.Add(new MultiViewBlock(mvBlk, false));
                }
            }
            return mvBlks;
        }

        /// <summary>
        /// Gets the Xrefs in a Document.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static List<Xref> Xrefs(this acDynNodes.Document document)
        {
            List<Xref> xrefs = new List<Xref>();
            var blks = document.Blocks;
            var xrefBlks = blks.Where(b => b.IsFromExternalReference());

            foreach (var blk in xrefBlks)
            {
                foreach (var blkRef in blk.BlockReferences)
                {
                    xrefs.Add(new Xref((acDb.BlockReference)blkRef.InternalDBObject, false));
                }
            }
            return xrefs;
        }

        /// <summary>
        /// Gets the units and zone settings for a Document.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        [MultiReturn(
            "Drawing Units",
            "Angular Units",
            "Coordinate System Code",
            "Imperial to Metric Conversion",
            "Drawing Scale",
            "Scale Objects from Other Drawings",
            "Set AutoCAD Variables to Match")]
        public static Dictionary<string, object> UnitsZoneSettings(this acDynNodes.Document document)
        {
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                var cdoc = civApp.CivilDocument.GetCivilDocument(document.AcDocument.Database);
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
        #endregion

        #region action methods
        /// <summary>
        /// Executes a particular command string. Returns true if the command was executed successfully and false otherwise.
        /// Ensure that a space or \n is included at the end of any string that you wish to be executed.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="echoInCommandLine">Show the command in the command line?</param>
        public static bool SendCommand(this acDynNodes.Document document, string command, bool echoInCommandLine = true)
        {
            if (string.IsNullOrEmpty(command)) { throw new ArgumentException("Command string is null or empty."); }

            try
            {
                document.AcDocument.SendStringToExecute(command, false, false, echoInCommandLine);
                return true;
            }
            catch
            {
                throw;
                return false;
            }
        }

        /// <summary>
        /// Gets the value of a system variable.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="variableName">The name of the system variable.</param>
        /// <returns></returns>
        public static object GetSystemVariable(this acDynNodes.Document document, string variableName)
        {
            return acApp.Application.GetSystemVariable(variableName);
        }

        /// <summary>
        /// Sets the value of a system variable.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="variableName">The name of the system variable.</param>
        /// <param name="newValue">The new value to assign.</param>
        /// <returns></returns>
        public static acDynNodes.Document SetSystemVariable(this acDynNodes.Document document, string variableName, object newValue)
        {
            // AutoCAD needs 16-bit integers, but from Dynamo they come as 64-bit.
            // Without this check, an eInvalidInput exception will be thrown when trying to set integer values.
            if (newValue is long) { newValue = Convert.ToInt16(newValue); }
            
            try
            {
                acApp.Application.SetSystemVariable(variableName, newValue);
                return document;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        /// <summary>
        /// Gets a Multi-View Block definition in a Document by name.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static MultiViewBlock MultiViewBlockByName(this acDynNodes.Document document, string name)
        {
            if (document is null)
            {
                throw new InvalidOperationException("Document cannot be null.");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new InvalidOperationException("Name is null or empty.");
            }

            return MultiViewBlocks(document)
                .FirstOrDefault(item => item.Name.Equals
                    (name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Sets the units (feet or meters) for a Document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="useFeet">If true, the units will be set to feet. If false, they will be set to metric.</param>
        /// <returns></returns>
        public static acDynNodes.Document SetDrawingUnits(this acDynNodes.Document document, bool useFeet)
        {
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var cdoc = civApp.CivilDocument.GetCivilDocument(document.AcDocument.Database);
                    var settings = cdoc.Settings.DrawingSettings.UnitZoneSettings;
                    var units = DrawingUnitType.Meters;
                    if (useFeet)
                    {
                        units = DrawingUnitType.Feet;
                    }
                    settings.DrawingUnits = units;
                }
                return document;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Sets the angular units for a Document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="angularUnitType">Degrees, Grads, or Radians</param>
        /// <returns></returns>
        public static acDynNodes.Document SetAngularUnits(this acDynNodes.Document document, string angularUnitType)
        {
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var cdoc = civApp.CivilDocument.GetCivilDocument(document.AcDocument.Database);
                    var settings = cdoc.Settings.DrawingSettings.UnitZoneSettings;
                    if (!Enum.TryParse(angularUnitType, out AngleUnitType unitType))
                    {
                        throw new InvalidOperationException("Invalid angular units type.");
                    }
                    settings.AngularUnits = unitType;
                }
                return document;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Sets the coordinate system code for a Document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static acDynNodes.Document SetCoordinateSystemCode(this acDynNodes.Document document, string code)
        {
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var cdoc = civApp.CivilDocument.GetCivilDocument(document.AcDocument.Database);
                    var settings = cdoc.Settings.DrawingSettings.UnitZoneSettings;
                    settings.CoordinateSystemCode = code;
                }
                return document;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Sets the imperial to metric conversion type for a Document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="useInternationalFoot">
        /// If true, the conversion type will be set to International Foot.
        ///  If false, it will be set to US Survey Foot.
        /// </param>
        /// <returns></returns>
        public static acDynNodes.Document SetImperialToMetricConversion(this acDynNodes.Document document, bool useInternationalFoot)
        {
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var cdoc = civApp.CivilDocument.GetCivilDocument(document.AcDocument.Database);
                    var settings = cdoc.Settings.DrawingSettings.UnitZoneSettings;
                    var conversionType = ImperialToMetricConversionType.UsSurveyFoot;
                    if (useInternationalFoot)
                    {
                        conversionType = ImperialToMetricConversionType.InternationalFoot;
                    }
                    settings.ImperialToMetricConversion = conversionType;
                }
                return document;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Sets the scale for a Document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static acDynNodes.Document SetDrawingScale(this acDynNodes.Document document, double scale)
        {
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var cdoc = civApp.CivilDocument.GetCivilDocument(document.AcDocument.Database);
                    var settings = cdoc.Settings.DrawingSettings.UnitZoneSettings;
                    settings.DrawingScale = scale;
                }
                return document;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Toggles whether to scale objects when inserted from other drawings into a Document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="scaleObjects"></param>
        /// <returns></returns>
        public static acDynNodes.Document SetScaleObjectsFromOtherDrawings(this acDynNodes.Document document, bool scaleObjects)
        {
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var cdoc = civApp.CivilDocument.GetCivilDocument(document.AcDocument.Database);
                    var settings = cdoc.Settings.DrawingSettings.UnitZoneSettings;
                    settings.ScaleObjectsFromOtherDrawings = scaleObjects;
                }
                return document;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Toggles whether to set AutoCAD variables to match for a Document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="matchVariables"></param>
        /// <returns></returns>
        public static acDynNodes.Document SetAutoCADVariablesToMatch(this acDynNodes.Document document, bool matchVariables)
        {
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var cdoc = civApp.CivilDocument.GetCivilDocument(document.AcDocument.Database);
                    var settings = cdoc.Settings.DrawingSettings.UnitZoneSettings;
                    settings.MatchAutoCADVariables = matchVariables;
                }
                return document;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
        #endregion

        #region helper methods
        /// <summary>
        /// Gets the DWG version of a database.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="getLastSaved">
        /// True = get the version that was last saved by the current session.
        /// False = get the original file version when the drawing was opened and prior to saving.
        /// </param>
        /// <returns>
        /// Returns null if the database has not been saved in the current session.
        /// Returns 0 if the DWG version is prior to 2004.
        /// </returns>
        internal static int? GetDatabaseVersionInfo(this acDb.Database database, bool getLastSaved)
        {
            acDb.DwgVersion dwgVersion;

            if (getLastSaved)
            {
                // The version that was last saved by the current session.
                // This will return MC0To0 if the drawing has not been saved in the current session,
                // even if the drawing has previously been saved to disk.
                dwgVersion = database.LastSavedAsVersion;
            }
            else
            {
                // The version of the file when it was opened.
                // This will return AC1032 if the database in the current session has never been saved.
                dwgVersion = database.OriginalFileVersion;
            }

            switch (dwgVersion)
            {
                case acDb.DwgVersion.AC1032:
                    return 2018;
                case acDb.DwgVersion.AC1027:
                    return 2013;
                case acDb.DwgVersion.AC1024:
                    return 2010;
                case acDb.DwgVersion.AC1021:
                    return 2007;
                case acDb.DwgVersion.AC1800:
                    return 2004;
                case acDb.DwgVersion.MC0To0:
                    return null;
                default:
                    return 0;
            }
        }
        #endregion
    }
}
