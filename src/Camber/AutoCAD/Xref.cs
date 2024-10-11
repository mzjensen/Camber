using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Camber.AutoCAD.Objects;
using Camber.Utilities.GeometryConversions;
using DynamoServices;
using System;
using System.IO;
using System.Linq;
using Autodesk.AutoCAD.DynamoNodes;
using AcBlock = Autodesk.AutoCAD.DatabaseServices.BlockTableRecord;
using AcBlockReference = Autodesk.AutoCAD.DatabaseServices.BlockReference;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acGeom = Autodesk.AutoCAD.Geometry;

namespace Camber.AutoCAD
{
    [RegisterForTrace]
    public sealed class Xref : acDynNodes.ObjectBase
    {
        private const string FileNotExistsMsg = "A valid file does not exist at the specified path.";
        private const string InvalidFileTypeMsg = "The file at the specified path is not a DWG file.";
        private const string AlreadyExistsMsg = "An Xref or Block with the same name already exists.";
        
        #region properties
        internal AcBlockReference AcBlockReference => AcObject as AcBlockReference;

        /// <summary>
        /// Gets the Block Reference of an Xref, which is how the Xref is handled internally by AutoCAD.
        /// </summary>
        public acDynNodes.BlockReference BlockReference =>
            (acDynNodes.BlockReference) acDynNodes.SelectionByQuery
                .GetObjectByObjectHandle(AcBlockReference.Handle
                .ToString());

        /// <summary>
        /// Gets the name of an Xref.
        /// </summary>
        public string Name => AcBlockReference.Name;

        /// <summary>
        /// Gets the path of the source file that defines an Xref. This may be a relative or full path.
        /// </summary>
        public string Path
        {
            get
            {
                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    var btr = (AcBlock) ctx.Transaction.GetObject(
                        AcBlockReference.BlockTableRecord,
                        acDb.OpenMode.ForRead);
                    return btr.PathName;
                }
            }
        }

        /// <summary>
        /// Gets if an Xref is attached as an overlay or not.
        /// </summary>
        public bool IsOverlay
        {
            get
            {
                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    var btr = (AcBlock)ctx.Transaction.GetObject(
                        AcBlockReference.BlockTableRecord,
                        acDb.OpenMode.ForRead);
                    return btr.IsFromOverlayReference;
                }
            }
        }

        /// <summary>
        /// Gets the insertion point, rotation, and scale factors of an Xref in the form of a coordinate system.
        /// </summary>
        public CoordinateSystem CoordinateSystem => AcBlockReference.BlockTransform.ToDyn();

        /// <summary>
        /// Gets the full path of the source file that defines an Xref. 
        /// </summary>
        public string FullPath
        {
            get
            {
                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    var btr = (AcBlock)ctx.Transaction.GetObject(
                        AcBlockReference.BlockTableRecord,
                        acDb.OpenMode.ForRead);
                    var db = btr.GetXrefDatabase(true);
                    return db.Filename;
                }
            }
        }
        #endregion

        #region constructors
        internal Xref(AcBlockReference acBlockReference, bool isDynamoOwned = false) : base(acBlockReference, isDynamoOwned) { }

        /// <summary>
        /// Creates a new external reference to a DWG file.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="block">The destination Block to insert the Xref into</param>
        /// <param name="filePath"></param>
        /// <param name="overlay">true = overlay, false = attach</param>
        /// <param name="coordinateSystem">Defines the insertion point, rotation, and scale factors</param>
        /// <param name="name">The name to assign to the Xref. By default it will be the name of the file.</param>
        /// <param name="layer">The layer to place the Xref on. By default it will be placed on the current layer.</param>
        /// <returns></returns>
        public static Xref InsertFromFile(
            acDynNodes.Document document,
            acDynNodes.Block block,
            string filePath,
            bool overlay,
            [DefaultArgument("CoordinateSystem.Identity()")]
            CoordinateSystem coordinateSystem,
            string name = "",
            string layer = "")
        {
            bool hasBlockWithSameName = false;
            bool fileNotExists = false;
            bool fileInvalidExtension = false;

            var res = CommonConstruct<Xref, AcBlockReference>(
                document,
                (ctx) =>
                {
                    if (name == "")
                    {
                        name = System.IO.Path.GetFileNameWithoutExtension(filePath);
                    }

                    if (document.Blocks.Any(obj => obj.Name == name && !obj.IsFromExternalReference()))
                    {
                        hasBlockWithSameName = true;
                        return null;
                    }

                    if (!File.Exists(filePath))
                    {
                        fileNotExists = true;
                        return null;
                    }

                    if (System.IO.Path.GetExtension(filePath).ToLower() != ".dwg")
                    {
                        fileInvalidExtension = true;
                        return null;
                    }

                    try
                    {
                        acGeom.Point3d insPnt =
                            (acGeom.Point3d) GeometryConversions.DynPointToAcPoint(coordinateSystem.Origin);

                        var xrefId = overlay
                            ? ctx.Database.OverlayXref(filePath, name)
                            : ctx.Database.AttachXref(filePath, name);

                        AcBlockReference blkRef = new AcBlockReference(insPnt, xrefId);
                        blkRef.BlockTransform = coordinateSystem.ToAc();

                        AcBlock btr = (AcBlock) ctx.Transaction.GetObject(
                            block.GetObjectId(),
                            acDb.OpenMode.ForWrite);

                        btr.AppendEntity(blkRef);
                        ctx.Transaction.AddNewlyCreatedDBObject(blkRef, true);

                        if (layer != "")
                        {
                            acDynNodes.AutoCADUtility.EnsureLayer(ctx, layer);
                            blkRef.Layer = layer;
                        }

                        return new Xref(blkRef, true).AcBlockReference;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                },
                (ctx, xref, existing) =>
                {
                    if (existing)
                    {
                        xref.UpgradeOpen();
                        var btr = (AcBlock)ctx.Transaction.GetObject(xref.BlockTableRecord, acDb.OpenMode.ForWrite);

                        if (name == "")
                        {
                            name = System.IO.Path.GetFileNameWithoutExtension(filePath);
                        }

                        // Update path
                        if (btr.PathName != filePath)
                        {
                            btr.PathName = filePath;
                            ctx.Database.ReloadXrefs(new acDb.ObjectIdCollection(new[] { btr.ObjectId }));
                        }

                        // Update name
                        if (xref.Name != name && document.Blocks.All(obj => obj.Name != name))
                        {
                            btr.Name = name;
                            ctx.Database.ReloadXrefs(new acDb.ObjectIdCollection(new[] { btr.ObjectId }));
                        }
                        else if (xref.Name != name && document.Blocks.Any(obj => obj.Name == name))
                        {
                            hasBlockWithSameName = true;
                            return false;
                        }

                        // Update host block
                        if (xref.BlockName != block.Name)
                        {
                            var hostBlk = block.GetBlockTableRecord();
                            hostBlk.UpgradeOpen();
                            hostBlk.AssumeOwnershipOf(new acDb.ObjectIdCollection(new[] { xref.ObjectId }));
                            hostBlk.DowngradeOpen();
                        }

                        // Update block transform
                        xref.BlockTransform = coordinateSystem.ToAc();

                        // Update overlay/attach
                        if (btr.IsFromOverlayReference != overlay)
                        {
                            btr.IsFromOverlayReference = overlay;
                            ctx.Database.ReloadXrefs(new acDb.ObjectIdCollection(new[] { btr.ObjectId }));
                        }

                        // Update layer
                        if (!string.Equals(xref.Layer, layer, StringComparison.CurrentCultureIgnoreCase))
                        {
                            acDynNodes.AutoCADUtility.EnsureLayer(ctx, layer);
                            xref.Layer = layer;
                        }

                        xref.DowngradeOpen();
                    }
                    return true;
                });

            if (hasBlockWithSameName)
            {
                throw new Exception(AlreadyExistsMsg);
            }

            if (fileNotExists)
            {
                throw new Exception(FileNotExistsMsg);
            }

            if (fileInvalidExtension)
            {
                throw new Exception(InvalidFileTypeMsg);
            }

            return res;
        }
        #endregion

        #region methods
        public override string ToString() => $"Xref(Name = {Name})";

        /// <summary>
        /// Override from ObjectBase to include Xref detach on Dispose().
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        public override void Dispose()
        {
            if (acDynApp.DisposeLogic.DisableDispose)
            {
                return;
            }

            acDynApp.LifecycleManager<string> instance = acDynApp.LifecycleManager<string>.Instance;

            if (instance.IsEmpty())
            {
                return;
            }
            bool isDeleted = instance.IsAutoCADDeleted(this.Handle);
            instance.UnRegisterAssociation(Handle, (object)this, this.IsDynamoOwned);

            if (this.IsDynamoOwned && !isDeleted && instance.GetDynamoOwnedRegisterCount(this.Handle) == 1)
            {
                try
                {
                    if (!string.IsNullOrEmpty(this.Handle))
                    {
                        acDynNodes.Document.Current.AcDocument.Database.DetachXref(this.AcBlockReference.BlockTableRecord);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine("Error deleting object: " + ex.ToString());
                }
            }
            ObjectHandle = (string)null;
        }

        /// <summary>
        /// Detaches an Xref and any nested Xrefs.
        /// </summary>
        public void Detach()
        {
            var document = acDynNodes.Document.Current;

            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    ctx.Database.DetachXref(AcBlockReference.BlockTableRecord);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Unloads an Xref.
        /// </summary>
        /// <returns></returns>
        public Xref Unload()
        {
            var document = acDynNodes.Document.Current;

            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    ctx.Database.UnloadXrefs(new acDb.ObjectIdCollection(new[] { AcBlockReference.BlockTableRecord }));
                }

                return this;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Reloads an Xref.
        /// </summary>
        /// <returns></returns>
        public Xref Reload()
        {
            var document = acDynNodes.Document.Current;

            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    ctx.Database.ReloadXrefs(new acDb.ObjectIdCollection(new[] { AcBlockReference.BlockTableRecord }));
                }

                return this;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Binds an Xref to the current drawing and returns its Block Reference.
        /// </summary>
        /// <returns></returns>
        public acDynNodes.BlockReference Bind()
        {
            var document = acDynNodes.Document.Current;

            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    ctx.Database.BindXrefs(new acDb.ObjectIdCollection(new[] { AcBlockReference.BlockTableRecord }), false);
                }

                return (acDynNodes.BlockReference) acDynNodes.SelectionByQuery.GetObjectByObjectHandle(AcBlockReference.Handle.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Sets the name of an Xref.
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        public Xref SetName(string newName)
        {
            var document = acDynNodes.Document.Current;
            
            if (document.Blocks.Any(obj => obj.Name == newName))
            {
                throw new Exception(AlreadyExistsMsg);
            }

            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var btr = (AcBlock) ctx.Transaction.GetObject(
                        AcBlockReference.BlockTableRecord,
                        acDb.OpenMode.ForWrite);
                    btr.Name = newName;
                    ctx.Database.ReloadXrefs(new acDb.ObjectIdCollection(new[] { btr.ObjectId }));
                }

                return this;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Sets the source file path of an Xref.
        /// </summary>
        /// <param name="newPath"></param>
        /// <returns></returns>
        public Xref SetPath(string newPath)
        {
            var document = acDynNodes.Document.Current;

            if (!File.Exists(newPath))
            {
                throw new Exception(FileNotExistsMsg);
            }

            if (System.IO.Path.GetExtension(newPath).ToLower() != ".dwg")
            {
                throw new Exception(InvalidFileTypeMsg);
            }

            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var btr = (AcBlock)ctx.Transaction.GetObject(
                        AcBlockReference.BlockTableRecord,
                        acDb.OpenMode.ForWrite);
                    if (btr.PathName != newPath)
                    {
                        btr.PathName = newPath;
                        ctx.Database.ReloadXrefs(new acDb.ObjectIdCollection(new[] { btr.ObjectId }));
                    }
                }

                return this;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Sets if an Xref is attached as an overlay or not.
        /// </summary>
        /// <param name="isOverlay"></param>
        /// <returns></returns>
        public Xref SetIsOverlay(bool isOverlay)
        {
            var document = acDynNodes.Document.Current;

            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var btr = (AcBlock)ctx.Transaction.GetObject(
                        AcBlockReference.BlockTableRecord,
                        acDb.OpenMode.ForWrite);
                    if (btr.IsFromOverlayReference != isOverlay)
                    {
                        btr.IsFromOverlayReference = isOverlay;
                        ctx.Database.ReloadXrefs(new acDb.ObjectIdCollection(new[] { btr.ObjectId }));
                    }
                }

                return this;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Sets the insertion point, rotation, and scale factors of an Xref in the form of a coordinate system.
        /// </summary>
        /// <param name="coordinateSystem"></param>
        /// <returns></returns>
        public Xref SetCoordinateSystem(CoordinateSystem coordinateSystem)
        {
            var document = acDynNodes.Document.Current;

            try
            {
                AcBlockReference.UpgradeOpen();
                AcBlockReference.BlockTransform = coordinateSystem.ToAc();
                AcBlockReference.DowngradeOpen();
                return this;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
