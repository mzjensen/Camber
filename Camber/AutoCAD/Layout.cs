#region references
using Autodesk.AutoCAD.DynamoNodes;
using Autodesk.DesignScript.Runtime;
using Camber.External;
using Dynamo.Graph.Nodes;
using DynamoServices;
using System;
using System.Collections.Generic;
using System.Linq;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using AcLayout = Autodesk.AutoCAD.DatabaseServices.Layout;
#endregion

namespace Camber.AutoCAD
{
    [RegisterForTrace]
    public sealed class Layout : ObjectBase
    {
        #region properties

        private const string NameExistsMsg = "A Layout with the same name already exists.";
        internal AcLayout AcLayout => AcObject as AcLayout;

        /// <summary>
        /// Gets the Block associated with a Layout.
        /// </summary>
        public acDynNodes.Block Block
        {
            get
            {
                acDynNodes.Document document = acDynNodes.Document.Current;
                ;
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    try
                    {
                        var bt = (acDb.BlockTable)ctx.Transaction.GetObject(
                            ctx.Database.BlockTableId,
                            acDb.OpenMode.ForRead);
                        var btr = (acDb.BlockTableRecord)ctx.Transaction.GetObject(
                            AcLayout.BlockTableRecordId,
                            acDb.OpenMode.ForRead);
                        return acDynNodes.AutoCADUtility.GetBlockByName(btr.Name, document.AcDocument);
                    }
                    catch (Exception e)
                    {
                        throw new InvalidOperationException(e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the name of a Layout.
        /// </summary>
        public string Name => AcLayout.LayoutName;

        /// <summary>
        /// Gets the tab order of a Layout.
        /// </summary>
        public int TabOrder => AcLayout.TabOrder;

        #endregion

        #region constructors
        internal Layout(AcLayout acLayout, bool isDynamoOwned = false) 
            : base(acLayout, isDynamoOwned) { }

        /// <summary>
        /// Creates a new Layout by name.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Layout ByName(acDynNodes.Document document, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new InvalidOperationException("Layout name is null or empty.");
            }

            bool hasLayoutWithSameName = false;
            var res = CommonConstruct<Layout, AcLayout>(
                document,
                (ctx) =>
                {
                    if (acDb.LayoutManager.Current.LayoutExists(name))
                    {
                        hasLayoutWithSameName = true;
                        return null;
                    }

                    using (acDb.Transaction t = ctx.Transaction.TransactionManager.StartTransaction())
                    {
                        acDb.ObjectId layId = acDb.LayoutManager.Current.CreateLayout(name);
                        AcLayout acLayout = (AcLayout)t.GetObject(
                            layId,
                            acDb.OpenMode.ForWrite);
                        t.Commit();
                        return acLayout;
                    }
                },
                (ctx, layout, existing) =>
                {
                    if (existing)
                    {
                        if (layout.LayoutName != name && !acDb.LayoutManager.Current.LayoutExists(name))
                        {
                            layout.LayoutName = name;
                            document.AcDocument.Editor.Regen();
                        }
                        else if (layout.LayoutName != name && acDb.LayoutManager.Current.LayoutExists(name))
                        {
                            hasLayoutWithSameName = true;
                            return false;
                        }
                    }
                    return true;
                });
            if (hasLayoutWithSameName)
            {
                throw new InvalidOperationException(NameExistsMsg);
            }
            return res;
        }

        /// <summary>
        /// Imports a Layout from an External Document.
        /// </summary>
        /// <param name="externalDocument"></param>
        /// <param name="layoutName"></param>
        /// <returns></returns>
        public static Layout Import(ExternalDocument externalDocument, string layoutName)
        {
            if (string.IsNullOrWhiteSpace(layoutName))
            {
                throw new InvalidOperationException("Invalid layout name.");
            }

            if (GetLayoutByName(acDynNodes.Document.Current, layoutName) != null)
            {
                throw new InvalidOperationException("A Layout with that name already exists in the current document.");
            }

            acDb.Database curDb = acDynNodes.Document.Current.AcDocument.Database;
            acDb.Database exDb = externalDocument.AcDatabase;
            
            // External drawing transaction
            using (acDb.Transaction exTr = exDb.TransactionManager.StartTransaction())
            {
                acDb.DBDictionary layoutsEx = (acDb.DBDictionary)exTr.GetObject(
                        exDb.LayoutDictionaryId, 
                        acDb.OpenMode.ForRead);

                if (!layoutsEx.Contains(layoutName))
                {
                    throw new InvalidOperationException("A Layout with that name does not exist in the external drawing.");
                }
                
                // Get the layout and block objects from the external drawing
                AcLayout layEx = layoutsEx.GetAt(layoutName).GetObject(acDb.OpenMode.ForRead) as AcLayout;
                acDb.BlockTableRecord blkBlkRecEx = (acDb.BlockTableRecord)exTr.GetObject(
                        layEx.BlockTableRecordId,
                        acDb.OpenMode.ForRead);

                // Get the objects from the block associated with the layout
                acDb.ObjectIdCollection idCol = new acDb.ObjectIdCollection();
                foreach (acDb.ObjectId id in blkBlkRecEx)
                {
                    idCol.Add(id);
                }

                // Current drawing transaction
                using (var ctx = new acDynApp.DocumentContext(curDb))
                {
                    // Get the block table and create a new block,
                    // then copy the objects between drawings
                    acDb.BlockTable blkTbl = (acDb.BlockTable)ctx.Transaction.GetObject(
                        curDb.BlockTableId, 
                        acDb.OpenMode.ForWrite);

                    using (acDb.BlockTableRecord blkBlkRec = new acDb.BlockTableRecord())
                    {
                        int layoutCount = layoutsEx.Count - 1;

                        blkBlkRec.Name = "*Paper_Space" + layoutCount.ToString();
                        blkTbl.Add(blkBlkRec);
                        ctx.Transaction.AddNewlyCreatedDBObject(blkBlkRec, true);
                        exDb.WblockCloneObjects(
                            idCol,
                            blkBlkRec.ObjectId,
                            new acDb.IdMapping(),
                            acDb.DuplicateRecordCloning.Ignore,
                            false);

                        // Create a new layout and then copy properties between drawings
                        acDb.DBDictionary layouts = (acDb.DBDictionary)ctx.Transaction.GetObject(
                            curDb.LayoutDictionaryId,
                            acDb.OpenMode.ForWrite);

                        using (AcLayout lay = new AcLayout())
                        {
                            lay.LayoutName = layoutName;
                            lay.AddToLayoutDictionary(curDb, blkBlkRec.ObjectId);
                            ctx.Transaction.AddNewlyCreatedDBObject(lay, true);
                            lay.CopyFrom(layEx);

                            acDb.DBDictionary plSets = (acDb.DBDictionary)ctx.Transaction.GetObject(
                                    curDb.PlotSettingsDictionaryId,
                                    acDb.OpenMode.ForRead);

                            // Check to see if a named page setup was assigned to the layout,
                            // if so then copy the page setup settings
                            if (lay.PlotSettingsName != "")
                            {
                                // Check to see if the page setup exists
                                if (!plSets.Contains(lay.PlotSettingsName))
                                {
                                    ctx.Transaction.GetObject(
                                        curDb.PlotSettingsDictionaryId, 
                                        acDb.OpenMode.ForWrite);

                                    using (acDb.PlotSettings plSet = new acDb.PlotSettings(lay.ModelType))
                                    {
                                        plSet.PlotSettingsName = lay.PlotSettingsName;
                                        plSet.AddToPlotSettingsDictionary(curDb);
                                        ctx.Transaction.AddNewlyCreatedDBObject(plSet, true);

                                        acDb.DBDictionary plSetsEx = (acDb.DBDictionary)exTr.GetObject(
                                            exDb.PlotSettingsDictionaryId,
                                            acDb.OpenMode.ForRead);

                                        acDb.PlotSettings plSetEx =
                                            plSetsEx.GetAt(lay.PlotSettingsName).GetObject(
                                                           acDb.OpenMode.ForRead) as acDb.PlotSettings;

                                        plSet.CopyFrom(plSetEx);
                                    }
                                }
                            }
                        }
                    }
                    acDynNodes.Document.Current.AcDocument.Editor.Regen();
                    //ctx.Transaction.Commit();
                }
                exTr.Abort();
                return GetLayoutByName(acDynNodes.Document.Current, layoutName);
            }
        }

        /// <summary>
        /// Gets a Layout in a Document by name.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [NodeCategory("Actions")]
        public static Layout GetLayoutByName(acDynNodes.Document document, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new InvalidOperationException("Layout name is null or empty.");
            }

            return GetLayouts(document, true).FirstOrDefault(
                item => item.Name.Equals(
                    name, 
                    StringComparison.OrdinalIgnoreCase));
        }
        #endregion

        #region methods
        public override string ToString() => $"Layout(Name = {Name})";

        /// <summary>
        /// Override from ObjectBase to include Editor regen on Dispose().
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        public override void Dispose()
        {
            if (acDynApp.DisposeLogic.DisableDispose)
                return;
            acDynApp.LifecycleManager<string> instance = acDynApp.LifecycleManager<string>.Instance;
            if (instance.IsEmpty())
            {
                return;
            }
            bool flag = instance.IsAutoCADDeleted(this.Handle);
            instance.UnRegisterAssociation(Handle, (object)this, this.IsDynamoOwned);
            if (this.IsDynamoOwned && !flag && instance.GetDynamoOwnedRegisterCount(this.Handle) == 0)
            {
                try
                {
                    if (!string.IsNullOrEmpty(this.Handle))
                    {
                        this.Delete();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine("Error deleting object: " + ex.ToString());
                }
            }
            ObjectHandle = (string)null;
            acDynNodes.Document.Current.AcDocument.Editor.Regen();
        }

        /// <summary>
        /// Gets all of the Layouts in a Document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="includeModel">Include Model Space?</param>
        /// <returns></returns>
        public static IList<Layout> GetLayouts(acDynNodes.Document document, bool includeModel = false)
        {
            List<Layout> layouts = new List<Layout>();

            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    acDb.DBDictionary layoutDict = (acDb.DBDictionary)ctx.Transaction.GetObject(
                        document.AcDocument.Database.LayoutDictionaryId,
                        acDb.OpenMode.ForRead);
                    foreach (acDb.DBDictionaryEntry layoutEntry in layoutDict)
                    {
                        if (layoutEntry.Key.ToUpper() == "MODEL" && !includeModel)
                        {
                            continue;
                        }

                        var acLayout = (acDb.Layout)ctx.Transaction.GetObject(
                            layoutEntry.Value,
                            acDb.OpenMode.ForRead);

                        layouts.Add(new Layout(acLayout, false));
                    }

                    return layouts;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Sets the name of a Layout.
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        public Layout SetName(string newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                throw new InvalidOperationException("Name is null or empty.");
            }

            if (acDb.LayoutManager.Current.LayoutExists(newName))
            {
                throw new InvalidOperationException(NameExistsMsg);
            }

            if (this.Name.ToUpper() == "MODEL")
            {
                throw new InvalidOperationException("The Model Space layout cannot be renamed.");
            }

            AcLayout.UpgradeOpen();
            AcLayout.LayoutName = newName;
            AcLayout.DowngradeOpen();
            acDynNodes.Document.Current.AcDocument.Editor.Regen();
            return this;
        }

        /// <summary>
        /// Reorders Layout tabs based on the ordering of the input list.
        /// </summary>
        /// <param name="layouts"></param>
        /// <returns></returns>
        public static IList<Layout> Reorder(IList<Layout> layouts)
        {
            if (layouts.Any(layout => layout.Name.ToUpper() == "MODEL"))
            {
                throw new InvalidOperationException("The Model Space tab cannot be reordered.");
            }

            var docLayouts = GetLayouts(
                acDynNodes.Document.Current, 
                false);

            if (layouts.Count != docLayouts.Count)
            {
                throw new InvalidOperationException(
                    "The number of input Layouts should match the total number of " +
                    "Layouts in the Document (excluding Model Space).");
            }

            //int maxTabOrder = docLayouts.Max(layout => layout.TabOrder);
            foreach (Layout layout in layouts)
            {
                layout.AcLayout.UpgradeOpen();
                layout.AcLayout.TabOrder = layouts.IndexOf(layout) + 1;
                layout.AcLayout.DowngradeOpen();
            }
            acDynNodes.Document.Current.AcDocument.Editor.Regen();
            return layouts;
        }
        #endregion
    }
}
