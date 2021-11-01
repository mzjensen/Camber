#region references
using System;
using System.Linq;
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using civDs = Autodesk.Civil.DataShortcuts;
using AeccPublishedItem = Autodesk.Civil.DataShortcuts.DataShortcuts.DataShortcutManager.PublishedItem;
using AeccExportableItem = Autodesk.Civil.DataShortcuts.DataShortcuts.DataShortcutManager.ExportableItem;
using Autodesk.DesignScript.Runtime;

#endregion

namespace Camber.Civil.DataShortcuts
{
    public sealed class DataShortcut
    {
        #region properties
        internal AeccPublishedItem AeccPublishedItem { get; set; }
        internal AeccExportableItem AeccExportableItem { get; set; }
        protected const string InvalidCivilObjectMsg = "A Data Shortcut cannot be created for this type of Civil Object.";

        /// <summary>
        /// Gets the index of a Data Shortcut's PublishedItem from the list of PublishedItems (not the main list of ExportableItems).
        /// </summary>
        private int PublishedItemIndex => GetAllPublishedItems().IndexOf(AeccPublishedItem);

        /// <summary>
        /// Gets the description of a Data Shortcut.
        /// </summary>
        public string Description => AeccPublishedItem.Description;

        /// <summary>
        /// Gets the entity type of a Data Shortcut.
        /// </summary>
        public string EntityType => AeccPublishedItem.DSEntityType.ToString();

        /// <summary>
        /// Gets the value that indicates if a Data Shortcut is broken.
        /// </summary>
        public bool IsBroken => AeccPublishedItem.IsBroken;

        /// <summary>
        /// Gets the name of a Data Shortcut.
        /// </summary>
        public string Name => AeccPublishedItem.Name;

        /// <summary>
        /// Gets the source file name of a Data Shortcut.
        /// </summary>
        public string SourceFileName => AeccPublishedItem.SourceFileName;

        /// <summary>
        /// Gets the source location of a Data Shortcut.
        /// </summary>
        public string SourceLocation => AeccPublishedItem.SourceLocation;
        #endregion

        #region constructors
        internal DataShortcut(AeccPublishedItem aeccPublishedItem, AeccExportableItem aeccExportableItem)
        {
            AeccPublishedItem = aeccPublishedItem;
            AeccExportableItem = aeccExportableItem;
        }

        /// <summary>
        /// Creates a new Data Shortcut for a Civil Object in the current Project Folder.
        /// </summary>
        /// <param name="civilObject"></param>
        /// <returns></returns>
        public static DataShortcut ByCivilObject(civDynNodes.CivilObject civilObject)
        {
            // This doesn't work correctly because we need to check for the situation where multiple data shortcuts are created for parents and children.
            
            // Check if the object has already been published
            bool isExported = CivilObjectExtensions.IsExportedAsDataShortcut(civilObject);
            if (isExported) { throw new ArgumentException("A Data Shortcut has already been created for this Civil Object."); }

            var exportableItem = CivilObjectExtensions.GetExportableItem(civilObject);
            bool successfulExport = SetExportableItemPublishedState(exportableItem, true);
            if (successfulExport)
            {
                return CivilObjectExtensions.DataShortcut(civilObject);
            }
            return null;
        }
        #endregion

        #region methods
        public override string ToString() => $"DataShortcut(Name = {Name}, Entity Type = {EntityType})";

        /// <summary>
        /// Creates a reference to a Data Shortcut in the current drawing.
        /// </summary>
        /// <returns></returns>
        public IList<object> CreateReference()
        {
            var document = acDynNodes.Document.Current;
            var objects = new List<object>();

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                try
                {
                    var oids = civDs.DataShortcuts.CreateReference(
                        document.AcDocument.Database,
                        SourceLocation + @"\" + SourceFileName,
                        Name,
                        (civDs.DataShortcutEntityType)Enum.Parse(typeof(civDs.DataShortcutEntityType), EntityType));

                    foreach (acDb.ObjectId oid in oids)
                    {
                        var obj = ctx.Transaction.GetObject(oid, acDb.OpenMode.ForRead);
                        objects.Add(obj);
                    }
                    return objects;
                }
                catch { throw; }
            }
        }

        /// <summary>
        /// Removes a Data Shortcut from the current Project Folder. Returns true if the Data Shortcut was removed successfully and false otherwise. 
        /// </summary>
        /// <returns></returns>
        public bool Remove()
        {
            return !SetExportableItemPublishedState(AeccExportableItem, false);
        }

        /// <summary>
        /// Sets the state of an ExportableItem via the Data Shortcut Manager.
        /// </summary>
        /// <param name="aeccExportableItem"></param>
        /// <param name="isPublished"></param>
        /// <returns></returns>
        private static bool SetExportableItemPublishedState(AeccExportableItem aeccExportableItem, bool isPublished)
        {
            try
            {
                // Create data shortcut manager
                bool isValidCreation = false;
                var manager = civDs.DataShortcuts.CreateDataShortcutManager(ref isValidCreation);

                if (isValidCreation)
                {
                    manager.SetSelectItemAtIndex(aeccExportableItem.Index, isPublished);
                    civDs.DataShortcuts.SaveDataShortcutManager(ref manager);
                    CivilApplication.RefreshDataShortcuts();
                    return manager.IsItemAtIndexAlreadyPublished(aeccExportableItem.Index);
                }

                throw new InvalidOperationException("Failed to create Data Shortcut Manager.");
            }
            catch { throw; }
        }

        /// <summary>
        /// Gets the list of ExportableItems from the DataShortcutManager.
        /// </summary>
        [SupressImportIntoVM]
        public static IList<AeccExportableItem> GetAllExportableItems()
        {
            bool isValidCreation = false;
            var manager = civDs.DataShortcuts.CreateDataShortcutManager(ref isValidCreation);
            var exportableItems = new List<AeccExportableItem>();
            if (isValidCreation)
            {
                var exportableItemsCount = manager.GetExportableItemsCount();
                for (int i = 0; i < exportableItemsCount; i++)
                {
                    exportableItems.Add(manager.GetExportableItemAt(i));
                }
            }
            return exportableItems;
        }

        /// <summary>
        /// Gets the list of ExportableItems that are flagged as IsExported.
        /// </summary>
        /// <returns></returns>
        [SupressImportIntoVM]
        public static IList<AeccExportableItem> GetExportedItems()
        {
            return (IList<AeccExportableItem>)GetAllExportableItems().Where(x => x.IsExported);
        }

        /// <summary>
        /// Gets the list of PublishedItems from the DataShortcutManager.
        /// </summary>
        [SupressImportIntoVM]
        public static IList<AeccPublishedItem> GetAllPublishedItems()
        {
            bool isValidCreation = false;
            var manager = civDs.DataShortcuts.CreateDataShortcutManager(ref isValidCreation);
            var publishedItems = new List<AeccPublishedItem>();
            if (isValidCreation)
            {
                var publishedItemsCount = manager.GetPublishedItemsCount();
                for (int i = 0; i < publishedItemsCount; i++)
                {
                    publishedItems.Add(manager.GetPublishedItemAt(i));
                }
            }
            return publishedItems;
        }
        #endregion
    }
}
