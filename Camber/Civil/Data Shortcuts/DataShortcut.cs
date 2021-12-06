#region references
using System;
using System.Linq;
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using civDb = Autodesk.Civil.DatabaseServices;
using civDs = Autodesk.Civil.DataShortcuts;
using AeccPublishedItem = Autodesk.Civil.DataShortcuts.DataShortcuts.DataShortcutManager.PublishedItem;
using AeccExportableItem = Autodesk.Civil.DataShortcuts.DataShortcuts.DataShortcutManager.ExportableItem;
using Autodesk.DesignScript.Runtime;
using Dynamo.Graph.Nodes;
using Camber.AutoCAD.External;
#endregion

namespace Camber.Civil.DataShortcuts
{
    public sealed class DataShortcut
    {
        #region properties
        internal AeccPublishedItem AeccPublishedItem { get; set; }
        internal AeccExportableItem AeccExportableItem => 
            (AeccExportableItem)GetAllExportableItems().Where(
            item => item.DSEntityType == AeccPublishedItem.DSEntityType
            && item.Name == AeccPublishedItem.Name);

        protected const string InvalidCivilObjectMsg = "A Data Shortcut cannot be created for this type of Civil Object.";

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
        internal DataShortcut(AeccPublishedItem aeccPublishedItem)
        {
            AeccPublishedItem = aeccPublishedItem;
        }
        #endregion

        #region methods
        public override string ToString() => $"DataShortcut(Name = {Name}, Entity Type = {EntityType})";

        /// <summary>
        /// Creates new Data Shortcuts for a Civil Object in the current Project Folder.
        /// If the Civil Object is a Profile or Sample Line Group, a Data Shortcut will also be created for its parent Alignment.
        /// If the Civil Object is a Corridor, Data Shortcuts will be created for each of its horizontal and vertical baselines.
        /// </summary>
        /// <param name="civilObject"></param>
        /// <returns></returns>
        [NodeCategory("Create")]
        public static IList<DataShortcut> ByCivilObject(civDynNodes.CivilObject civilObject)
        {
            // Check if the object has already been published
            bool isExported = CivilObject.IsExportedAsDataShortcut(civilObject);
            if (isExported) { throw new ArgumentException("A Data Shortcut has already been created for this Civil Object."); }

            var dataShortcuts = new List<DataShortcut>();

            var allExItems = new List<AeccExportableItem>();
            var mainExItem = CivilObject.GetExportableItem(civilObject);
            allExItems.Add(mainExItem);

            // Check for parent item
            if (mainExItem.ParentItem != null)
            {
                allExItems.Add(mainExItem.ParentItem);
            }

            // Check for dependent items
            if (mainExItem.DependentItems != null)
            {
                foreach (var item in mainExItem.DependentItems)
                {
                    allExItems.Add(item);
                }
            }

            bool successfulExport = SetExportableItemPublishedState(mainExItem);
            if (successfulExport)
            {
                foreach (var exItem in allExItems)
                {
                    // Get corresponding PublishedItem for each ExportableItem
                    AeccPublishedItem pItem = GetAllPublishedItems().Where(
                        item => item.DSEntityType == exItem.DSEntityType
                        && item.Name == exItem.Name).FirstOrDefault();

                    dataShortcuts.Add(new DataShortcut(pItem));
                }
                return dataShortcuts;
            }
            return null;
        }

        /// <summary>
        /// Creates a reference to a Data Shortcut in the current document.
        /// </summary>
        /// <returns></returns>
        public IList<civDynNodes.CivilObject> CreateReference(acDynNodes.Document document)
        {
            var civilObjects = new List<civDynNodes.CivilObject>();

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
                        var entity = (civDb.Entity)ctx.Transaction.GetObject(oid, acDb.OpenMode.ForRead);
                        switch (entity)
                        {
                            case civDb.Alignment alignment:
                                civilObjects.Add(civDynNodes.Selection.AlignmentByName(entity.Name, document));
                                break;
                            case civDb.Network pipeNetwork:
                                civilObjects.Add(PipeNetworks.PipeNetwork.GetByObjectId(oid));
                                break;
                            case civDb.PressurePipeNetwork pressurePipeNetwork:
                                civilObjects.Add(PressureNetworks.PressureNetwork.GetByObjectId(oid));
                                break;
                            case civDb.Corridor corridor:
                                civilObjects.Add(civDynNodes.Selection.CorridorByName(entity.Name, document));
                                break;
                            case civDb.Profile profile:
                                var aeccProfile = (civDb.Profile)entity;
                                var align = (civDb.Alignment)ctx.Transaction.GetObject(aeccProfile.AlignmentId, acDb.OpenMode.ForRead);
                                civilObjects.Add(civDynNodes.Selection.AlignmentByName(align.Name, document).ProfileByName(aeccProfile.Name));
                                break;
                            case civDb.SampleLineGroup sampleLineGroup:
                                civilObjects.Add(SampleLineGroup.GetByObjectId(oid));
                                break;
                            case civDb.ViewFrameGroup viewFrameGroup:
                                civilObjects.Add(ViewFrameGroup.GetByObjectId(oid));
                                break;
                            case civDb.Surface surface:
                                civilObjects.Add(civDynNodes.Selection.SurfaceByName(entity.Name, document));
                                break;
                        }
                    }
                    return civilObjects;
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException(e.Message);
                }
            }
        }

        /// <summary>
        /// Creates a reference to a Data Shortcut in an External Document.
        /// </summary>
        /// <param name="externalDocument"></param>
        /// <param name="save">Save the External Document when complete?</param>
        /// <returns></returns>
        public bool CreateReference(ExternalDocument externalDocument, bool save)
        {
            using (var ctx = new acDynApp.DocumentContext(externalDocument.AcDatabase))
            {
                try
                {
                    var oids = civDs.DataShortcuts.CreateReference(
                        externalDocument.AcDatabase,
                        SourceLocation + @"\" + SourceFileName,
                        Name,
                        (civDs.DataShortcutEntityType)Enum.Parse(typeof(civDs.DataShortcutEntityType), EntityType));
                    if (save) {
                        externalDocument.Save();
                    }
                    if (oids.Count > 0) {
                        return true;
                    }
                    return false;
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException(e.Message);
                }
            }
        }

        /// <summary>
        /// Sets the state of an ExportableItem via the Data Shortcut Manager.
        /// </summary>
        /// <param name="aeccExportableItem"></param>
        /// <param name="isPublished"></param>
        /// <returns></returns>
        private static bool SetExportableItemPublishedState(AeccExportableItem aeccExportableItem)
        {
            try
            {
                // Create data shortcut manager
                bool isValidCreation = false;
                var manager = civDs.DataShortcuts.CreateDataShortcutManager(ref isValidCreation);

                if (isValidCreation)
                {
                    // Setting isSelected to false will not actually remove a data shortcut if it has already been created
                    // It essentially just unchecks the box in the "Create Data Shortcuts" window, although I don't see
                    // the point of this because saving the state of the DataShortcutManager when isSelected is true
                    // will create a data shortcut. So there's really never a time when passing false would do anything.
                    manager.SetSelectItemAtIndex(aeccExportableItem.Index, true);
                    civDs.DataShortcuts.SaveDataShortcutManager(ref manager);
                    DataShortcuts.Refresh();
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
