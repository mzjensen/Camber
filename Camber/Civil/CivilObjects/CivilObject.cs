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
using AeccExportableItem = Autodesk.Civil.DataShortcuts.DataShortcuts.DataShortcutManager.ExportableItem;
using AeccPublishedItem = Autodesk.Civil.DataShortcuts.DataShortcuts.DataShortcutManager.PublishedItem;
using Autodesk.DesignScript.Runtime;
using DynamoServices;
using Dynamo.Graph.Nodes;
using Camber.Civil.DataShortcuts;
using Camber.Civil.Styles;
using Camber.Civil.Toolspace;
using Camber.Utilities;
#endregion

namespace Camber.Civil.CivilObjects
{
    [RegisterForTrace]
    public class CivilObject : civDynNodes.CivilObject
    {
        #region properties
        internal civDb.Entity AeccEntity => AcObject as civDb.Entity;
        protected const string NotApplicableMsg = "Not applicable";
        protected const string NotReferenceEntityMsg = "Civil Object is not a Data Shortcut reference entity.";
        #endregion

        #region constructors
        internal CivilObject(civDb.Entity entity, bool isDynamoOwned = false) : base(entity, isDynamoOwned) { }
        #endregion

        #region methods
        /// <summary>
        /// Gets the Style of a Civil Object.
        /// </summary>
        /// <param name="civilObject"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static Style Style(civDynNodes.CivilObject civilObject)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document?.AcDocument))
            {
                var cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                acDb.ObjectId oid = civilObject.InternalObjectId;
                var aeccEntity = (civDb.Entity)oid.GetObject(acDb.OpenMode.ForRead);
                var styleId = aeccEntity.StyleId;
                var aeccStyle = styleId.GetObject(acDb.OpenMode.ForRead);
                IEnumerable<Style> styles = ReflectionUtilities.GetEnumerableOfType<Style>(new object[] { aeccStyle, false });
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
        [NodeCategory("Query")]
        public static bool IsReference(civDynNodes.CivilObject civilObject)
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
        /// Gets the Toolspace Folder that a Civil Object is contained within.
        /// Returns null if the Civil Object is not contained in a Folder.
        /// </summary>
        /// <param name="civilObject"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static Folder Folder(civDynNodes.CivilObject civilObject)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                acDb.ObjectId oid = civilObject.InternalObjectId;
                var aeccEntity = (civDb.Entity)oid.GetObject(acDb.OpenMode.ForRead);

                if (aeccEntity.FolderId != acDb.ObjectId.Null)
                {
                    civDb.Folder folder = (civDb.Folder)ctx.Transaction.GetObject(
                        aeccEntity.FolderId,
                        acDb.OpenMode.ForWrite);
                    return new Folder(folder, false);
                }
                return null;
            }
        }

        /// <summary>
        /// Determines if a Civil Object can be exported as a Data Shortcut.
        /// </summary>
        /// <param name="civilObject"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static bool IsExportableAsDataShortcut(civDynNodes.CivilObject civilObject)
        {
            var DSEntityType = GetDataShortcutEntityType(civilObject);
            if (DSEntityType is civDs.DataShortcutEntityType.Unknown)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets if a Data Shortcut has been created for a Civil Object. 
        /// </summary>
        /// <param name="civilObject"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static bool IsExportedAsDataShortcut(civDynNodes.CivilObject civilObject)
        {
            var exItem = GetExportableItem(civilObject);
            if (exItem != null)
            {
                return exItem.IsExported;
            }
            return false;
        }

        /// <summary>
        /// Gets a Civil Object's Data Shortcut if one has been created.
        /// </summary>
        /// <param name="civilObject"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static DataShortcut DataShortcut(civDynNodes.CivilObject civilObject)
        {
            if (IsExportedAsDataShortcut(civilObject))
            {
                var exItem = GetExportableItem(civilObject);
                var pItems = DataShortcuts.DataShortcut.GetAllPublishedItems();
                AeccPublishedItem pItem = pItems.Where(
                    item => item.DSEntityType == exItem.DSEntityType
                    && item.Name == exItem.Name).FirstOrDefault();

                return new DataShortcut(pItem);
            }
            return null;
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
        /// <param name="pathToSource">The full path to the specified drawing to use as the new source drawing.</param>
        /// <param name="repairOthers">Attempt to repair other Civil Objects with broken references?</param>
        /// <returns></returns>
        public static civDynNodes.CivilObject RepairBrokenReference(civDynNodes.CivilObject civilObject, string pathToSource, bool repairOthers = false)
        {
            if (!IsReference(civilObject)) { throw new ArgumentException(NotReferenceEntityMsg); }
            if (string.IsNullOrEmpty(pathToSource)) { throw new ArgumentException("Path to target drawing is null or empty."); }

            // Check if file exists
            if (!File.Exists(pathToSource)) { throw new ArgumentException("The specified drawing does not exist or the path is invalid."); }

            // Check if file is a DWG
            var extension = Path.GetExtension(pathToSource);
            if (string.IsNullOrEmpty(extension)) { throw new ArgumentException("The specified path does not point to a .DWG file."); }

            try
            {
                bool repaired = civDs.DataShortcuts.RepairBrokenDRef(civilObject.InternalObjectId, pathToSource, repairOthers);
                if (repaired)
                {
                    return civilObject;
                }
                throw new Exception("The reference for the specified Civil Object could not be repaired.");
            }
            catch { throw; }
        }

        /// <summary>
        /// Gets reference information for a Civil Object whose source is a Data Shortcut.
        /// </summary>
        /// <param name="civilObject"></param>
        /// <returns></returns>
        [MultiReturn(new[] { "Name", "Type", "Is Broken", "Source Drawing" })]
        public static Dictionary<string, object> GetReferenceInfo(civDynNodes.CivilObject civilObject)
        {
            if (!IsReference(civilObject)) { throw new ArgumentException(NotReferenceEntityMsg); }

            var document = acDynNodes.Document.Current;

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                var aeccEntity = (civDb.Entity)ctx.Transaction.GetObject(civilObject.InternalObjectId, acDb.OpenMode.ForRead);
                var refInfo = aeccEntity.GetReferenceInfo();

                return new Dictionary<string, object>
                {
                    { "Name", refInfo.Name },
                    { "Type", refInfo.Type.ToString() },
                    { "Is Broken", !refInfo.IsSourceDrawingExistent },
                    { "Source Drawing", refInfo.SourceDrawing }
                };
            }
        }

        /// <summary>
        /// Gets a Civil Object's associated Exportable Item if it exists.
        /// </summary>
        /// <param name="civilObject"></param>
        /// <returns></returns>
        [SupressImportIntoVM]
        public static AeccExportableItem GetExportableItem(civDynNodes.CivilObject civilObject)
        {
            // Get list of all exportable items
            var exItems = DataShortcuts.DataShortcut.GetAllExportableItems();

            // Get DS entity type of Civil Object
            var DSEntityType = GetDataShortcutEntityType(civilObject);

            AeccExportableItem output = null;

            foreach (AeccExportableItem exItem in exItems)
            {
                if (exItem.Name == civilObject.Name && exItem.DSEntityType == DSEntityType)
                {
                    // Profiles and Sample Line Groups are the only exportable items that can have parents.
                    if (exItem.DSEntityType == civDs.DataShortcutEntityType.Profile
                        || exItem.DSEntityType == civDs.DataShortcutEntityType.SampleLineGroup)
                    {
                        civDynNodes.Alignment parentAlignment;

                        if (exItem.DSEntityType == civDs.DataShortcutEntityType.Profile)
                        {
                            var profile = (civDynNodes.Profile)civilObject;
                            parentAlignment = profile.Alignment;
                        }
                        else
                        {
                            var slg = (SampleLineGroup)civilObject;
                            parentAlignment = slg.Alignment;
                        }

                        if (exItem.ParentItem.Name == parentAlignment.Name)
                        {
                            output = exItem;
                        }
                    }
                    output = exItem;
                }
            }
            return output;
        }

        /// <summary>
        /// Gets the Data Shortcut entity type of a Civil Object.
        /// </summary>
        /// <param name="civilObject"></param>
        /// <returns></returns>
        [SupressImportIntoVM]
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