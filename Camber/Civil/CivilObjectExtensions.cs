#region references
using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using civDb = Autodesk.Civil.DatabaseServices;
using civDs = Autodesk.Civil.DataShortcuts;
using civApp = Autodesk.Civil.ApplicationServices;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using ExportableItem = Autodesk.Civil.DataShortcuts.DataShortcuts.DataShortcutManager.ExportableItem;
using Autodesk.DesignScript.Runtime;
using DynamoServices;
using Camber.Civil.Styles;
using Camber.Utils;
#endregion

namespace Camber.Civil
{
    [RegisterForTrace]
    public class CivilObjectExtensions : civDynNodes.CivilObject
    {
        #region properties
        internal civDb.Entity AeccEntity => AcObject as civDb.Entity;
        protected const string NotApplicableMsg = "Not applicable";
        protected const string NotReferenceEntityMsg = "Civil Object is not a Data Shortcut reference entity.";
        #endregion

        #region constructors
        internal CivilObjectExtensions(civDb.Entity entity, bool isDynamoOwned = false) : base(entity, isDynamoOwned) { }
        #endregion

        #region methods
        /// <summary>
        /// Gets the Style of a Civil Object.
        /// </summary>
        /// <param name="civilObject"></param>
        /// <returns></returns>
        public static Style GetStyle(civDynNodes.CivilObject civilObject)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document?.AcDocument))
            {
                var cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                acDb.ObjectId oid = civilObject.InternalObjectId;
                var aeccEntity = (civDb.Entity)oid.GetObject(acDb.OpenMode.ForRead);
                var styleId = aeccEntity.StyleId;
                var aeccStyle = styleId.GetObject(acDb.OpenMode.ForRead);
                IEnumerable<Style> styles = ReflectionUtils.GetEnumerableOfType<Style>(new object[] { aeccStyle, false });
                if (styles.Count() > 1)
                {
                    throw new Exception("Multiple Styles found.");
                }
                else if (styles.Count() == 0)
                {
                    throw new Exception("No Style found.");
                }
                return styles.First();
            }
        }

        /// <summary>
        /// Gets whether the Civil Object is a reference object.
        /// A reference object is located in another drawing and linked using a data shortcut or through Autodesk Vault.
        /// </summary>
        /// <param name="civilObject"></param>
        /// <returns></returns>
        public static bool GetIsReference(civDynNodes.CivilObject civilObject)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document?.AcDocument))
            {
                var cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                acDb.ObjectId oid = civilObject.InternalObjectId;
                var aeccEntity = (civDb.Entity)oid.GetObject(acDb.OpenMode.ForRead);
                if (aeccEntity.IsReferenceObject || aeccEntity.IsReferenceSubObject)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Sets the Style of a Civil Object.
        /// </summary>
        /// <param name="civilObject"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public static civDynNodes.CivilObject SetStyle(civDynNodes.CivilObject civilObject, Style style)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document?.AcDocument))
            {
                var cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                acDb.ObjectId oid = civilObject.InternalObjectId;
                var aeccEntity = (civDb.Entity)oid.GetObject(acDb.OpenMode.ForWrite);
                try
                {
                    aeccEntity.StyleId = style.InternalObjectId;
                    return civilObject;
                }
                catch
                {
                    throw new Exception("Style is not valid for this type of object.");
                }
            }
        }

        /// <summary>
        /// Attempts to repair a broken reference to a Civil Object by specifying a path to a new source drawing.
        /// Includes a flag to optionally attempt to repair all other Civil Objects with broken references from the same source drawing.
        /// </summary>
        /// <param name="civilObject"></param>
        /// <param name="pathToSourceDwg">The full path to the specified drawing to use as the new source drawing.</param>
        /// <param name="repairOthers">Attempt to repair other Civil Objects with broken references?</param>
        /// <returns></returns>
        public static civDynNodes.CivilObject RepairBrokenReference(civDynNodes.CivilObject civilObject, string pathToSourceDwg, bool repairOthers = false)
        {
            if (!GetIsReference(civilObject)) { throw new ArgumentException(NotReferenceEntityMsg); }
            if (string.IsNullOrEmpty(pathToSourceDwg)) { throw new ArgumentException("Path to target drawing is null or empty."); }

            // Check if file exists
            if (!File.Exists(pathToSourceDwg)) { throw new ArgumentException("The specified drawing does not exist or the path is invalid."); }

            // Check if file is a DWG
            var extension = Path.GetExtension(pathToSourceDwg);
            if (string.IsNullOrEmpty(extension)) { throw new ArgumentException("The specified path does not point to a .DWG file."); }

            try
            {
                bool repaired = civDs.DataShortcuts.RepairBrokenDRef(civilObject.InternalObjectId, pathToSourceDwg, repairOthers);
                if (repaired) 
                { 
                    return civilObject; 
                }
                throw new Exception("The reference for the specified Civil Object could not be repaired.");
            }
            catch { throw; }
        }

        /// <summary>
        /// Gets the Data Shortcut reference information for a Civil Object.
        /// </summary>
        /// <param name="civilObject"></param>
        /// <returns></returns>
        [MultiReturn(new[] { "Name", "Type", "Is Broken", "Source Drawing", "Handle High", "Handle Low" })]
        public static Dictionary<string, object> GetReferenceInfo(civDynNodes.CivilObject civilObject)
        {
            if (!GetIsReference(civilObject)) { throw new ArgumentException(NotReferenceEntityMsg); }

            var document = acDynNodes.Document.Current;

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                var aeccEntity = (civDb.Entity)ctx.Transaction.GetObject(civilObject.InternalObjectId, acDb.OpenMode.ForRead);
                var refInfo = aeccEntity.GetReferenceInfo();

                return new Dictionary<string, object>
                {
                    { "Name", refInfo.Name },
                    { "Type", refInfo.Type.ToString() },
                    { "Is Broken", refInfo.IsSourceDrawingExistent },
                    { "Source Drawing", refInfo.SourceDrawing },
                    { "Handle High", refInfo.HandleHigh },
                    { "Handle Low", refInfo.HandleLow }
                };
            }
        }

        /// <summary>
        /// Gets the data shortcut entity type of a Civil Object.
        /// </summary>
        /// <param name="civilObject"></param>
        /// <returns></returns>
        public static civDs.DataShortcutEntityType GetDataShortcutEntityType(civDynNodes.CivilObject civilObject)
        {
            var document = acDynNodes.Document.Current;
            civDs.DataShortcutEntityType DSEntityType = civDs.DataShortcutEntityType.Unknown;
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                var aeccObj = ctx.Transaction.GetObject(civilObject.InternalObjectId, acDb.OpenMode.ForRead);
                
                switch (aeccObj)
                {
                    case civDb.Alignment alignment:
                        DSEntityType = civDs.DataShortcutEntityType.Alignment;
                        break;
                    case civDb.Network pipeNetwork:
                        DSEntityType = civDs.DataShortcutEntityType.PipeNetwork;
                        break;
                    case civDb.PressurePipeNetwork pressurePipeNetwork:
                        DSEntityType = civDs.DataShortcutEntityType.PressurePipeNetwork;
                        break;
                    case civDb.Corridor corridor:
                        DSEntityType = civDs.DataShortcutEntityType.Corridor;
                        break;
                    case civDb.Profile profile:
                        DSEntityType = civDs.DataShortcutEntityType.Profile;
                        break;
                    case civDb.SampleLineGroup sampleLineGroup:
                        DSEntityType = civDs.DataShortcutEntityType.SampleLineGroup;
                        break;
                    case civDb.ViewFrameGroup viewFrameGroup:
                        DSEntityType = civDs.DataShortcutEntityType.ViewFrameGroup;
                        break;
                    case civDb.Surface surface:
                        DSEntityType = civDs.DataShortcutEntityType.Surface;
                        break;
                }
                return DSEntityType;
            }
        }

        /// <summary>
        /// Determines if a Civil Object can be published as a data shortcut.
        /// </summary>
        /// <param name="civilObject"></param>
        /// <returns></returns>
        public static bool IsExportable(civDynNodes.CivilObject civilObject)
        {
            var DSEntityType = GetDataShortcutEntityType(civilObject);
            if (DSEntityType is civDs.DataShortcutEntityType.Unknown)
            {
                return false;
            }
            return true;
        }

        // TODO
        public static ExportableItem GetExportableItem(civDynNodes.CivilObject civilObject)
        {
            // I think Profiles and Sample Line Groups are the only exportable items that can have parents.
            
            bool isExportable = IsExportable(civilObject);
            if (!isExportable) { throw new ArgumentException("A data shortcut cannot be created for this type of object."); }
            
            var exportableItems = DataShortcuts.DataShortcuts.GetExportableItems();
            var DSEntityType = GetDataShortcutEntityType(civilObject);

            foreach (ExportableItem item in exportableItems)
            {
                if (item.Name == civilObject.Name && item.DSEntityType == DSEntityType)
                {
                    return null;
                }
            }
            return null;
        }


        protected double GetDouble([CallerMemberName] string propertyName = null)
        {
            try
            {
                PropertyInfo propInfo = AeccEntity.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (propInfo != null)
                {
                    return (double)propInfo.GetValue(AeccEntity);
                }
            }
            catch { }
            return double.NaN;
        }

        protected string GetString([CallerMemberName] string propertyName = null)
        {
            try
            {
                PropertyInfo propInfo = AeccEntity.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (propInfo != null)
                {
                    var value = propInfo.GetValue(AeccEntity).ToString();
                    if (string.IsNullOrEmpty(value))
                    {
                        return null;
                    }
                    else
                    {
                        return value;
                    }
                }
            }
            catch { }
            return NotApplicableMsg;
        }

        protected int GetInt([CallerMemberName] string propertyName = null)
        {
            try
            {
                PropertyInfo propInfo = AeccEntity.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (propInfo != null)
                {
                    return (int)propInfo.GetValue(AeccEntity);
                }
            }
            catch { }
            return int.MinValue;
        }

        protected bool GetBool([CallerMemberName] string propertyName = null)
        {
            try
            {
                PropertyInfo propInfo = AeccEntity.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

                if (propInfo != null)
                {
                    return (bool)propInfo.GetValue(AeccEntity);
                }
            }
            catch { }
            return false;
        }

        protected bool SetValue(object value, [CallerMemberName] string methodName = null)
        {
            if (methodName.StartsWith("Set"))
            {
                methodName = methodName.Substring(3);
            }
            return SetValue(methodName, value);
        }

        protected bool SetValue(string propertyName, object value)
        {
            try
            {
                bool openedForWrite = AeccEntity.IsWriteEnabled;
                if (!openedForWrite) AeccEntity.UpgradeOpen();
                PropertyInfo propInfo = AeccEntity.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                propInfo?.SetValue(AeccEntity, value);
                if (!openedForWrite) AeccEntity.DowngradeOpen();
                return true;
            }
            catch { }
            return false;
        }
        #endregion
    }
}