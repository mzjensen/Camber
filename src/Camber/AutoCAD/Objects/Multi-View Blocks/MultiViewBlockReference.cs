using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.DynamoNodes;
using Autodesk.DesignScript.Geometry;
using DynamoServices;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using AecMultiViewBlock = Autodesk.Aec.DatabaseServices.MultiViewBlockDefinition;
using AecMultiViewBlockReference = Autodesk.Aec.DatabaseServices.MultiViewBlockReference;

namespace Camber.AutoCAD.Objects.MultiViewBlocks
{
    [RegisterForTrace]
    public sealed class MultiViewBlockReference : Object
    {
        #region properties
        internal AecMultiViewBlockReference AecMultiViewBlockReference => AcObject as AecMultiViewBlockReference;

        /// <summary>
        /// Gets the Multi-View Block that defines a Multi-View Block Reference.
        /// </summary>
        public MultiViewBlock MultiViewBlock
        {
            get
            {
                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    var aecMvBlk = (AecMultiViewBlock) ctx.Transaction.GetObject(
                        AecMultiViewBlockReference.BlockDefId,
                        acDb.OpenMode.ForRead);
                    return new MultiViewBlock(aecMvBlk, false);
                }
            }
        }

        /// <summary>
        /// Gets the coordinate system of a Multi-View Block Reference.
        /// </summary>
        public CoordinateSystem CoordinateSystem => AutoCADUtility.MatrixToCoordinateSystem(AecMultiViewBlockReference.Ecs);
        #endregion

        #region constructors
        internal MultiViewBlockReference(AecMultiViewBlockReference aecMultiViewBlockReference, bool isDynamoOwned = false)
            : base(aecMultiViewBlockReference, isDynamoOwned) { }

        /// <summary>
        /// Creates a new Multi-View Block Reference by coordinate system.
        /// </summary>
        /// <param name="multiViewBlock"></param>
        /// <param name="coordinateSystem"></param>
        /// <returns></returns>
        public static MultiViewBlockReference ByCoordinateSystem(
            MultiViewBlock multiViewBlock,
            CoordinateSystem coordinateSystem,
            string layer,
            acDynNodes.Block block)
        {
            var document = acDynNodes.Document.Current;
            var res = CommonConstruct<MultiViewBlockReference, AecMultiViewBlockReference>(
                document,
                (ctx) =>
                {
                    AutoCADUtility.EnsureLayer(ctx, layer);

                    var bt = (BlockTable)ctx.Transaction.GetObject(
                        document.AcDocument.Database.BlockTableId,
                        OpenMode.ForRead);
                    var btr = (BlockTableRecord)ctx.Transaction.GetObject(
                        bt[block.Name], 
                        OpenMode.ForWrite);

                    var mvBlkRef = new AecMultiViewBlockReference();
                    mvBlkRef.SetDatabaseDefaults(document.AcDocument.Database);
                    mvBlkRef.SetToStandard(document.AcDocument.Database);
                    mvBlkRef.BlockDefId = multiViewBlock.InternalObjectId;
                    mvBlkRef.Layer = layer;
                    mvBlkRef.GeoEcs = AutoCADUtility.CooridnateSystemToMatrix(coordinateSystem);
                    btr.AppendEntity(mvBlkRef);
                    ctx.Transaction.AddNewlyCreatedDBObject(mvBlkRef, true);
                    btr.DowngradeOpen();
                    return mvBlkRef;
                },
                (ctx, mvBlkRef, existing) =>
                {
                    if (existing)
                    {
                        var bt = (BlockTable)ctx.Transaction.GetObject(
                            document.AcDocument.Database.BlockTableId,
                            OpenMode.ForRead);
                        var btr = (BlockTableRecord)ctx.Transaction.GetObject(
                            bt[block.Name],
                            OpenMode.ForRead);

                        if (mvBlkRef.BlockId != btr.ObjectId)
                        {
                            ObjectIdCollection ids = new ObjectIdCollection();
                            ids.Add(mvBlkRef.ObjectId);
                            btr.UpgradeOpen();
                            btr.AssumeOwnershipOf(ids);
                            btr.DowngradeOpen();
                        }

                        if (mvBlkRef.BlockDefId != multiViewBlock.InternalObjectId)
                        {
                            mvBlkRef.BlockDefId = multiViewBlock.InternalObjectId;
                        }

                        if (mvBlkRef.Layer != layer)
                        {
                            AutoCADUtility.EnsureLayer(ctx, layer);
                            mvBlkRef.Layer = layer;
                        }

                        if (mvBlkRef.GeoEcs != AutoCADUtility.CooridnateSystemToMatrix(coordinateSystem))
                        {
                            mvBlkRef.GeoEcs = AutoCADUtility.CooridnateSystemToMatrix(coordinateSystem);
                        }
                    }
                    return true;
                });
            return res;
        }
        #endregion

        #region methods
        public override string ToString() => $"MultiViewBlockReference(Multi-View Block = {MultiViewBlock.Name})";

        /// <summary>
        /// Sets the coordinate system of a Multi-View Block Reference.
        /// </summary>
        /// <param name="coordinateSystem"></param>
        /// <returns></returns>
        public MultiViewBlockReference SetCoordinateSystem(CoordinateSystem coordinateSystem)
        {
            using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
            {
                AecMultiViewBlockReference.UpgradeOpen();
                AecMultiViewBlockReference.GeoEcs = AutoCADUtility.CooridnateSystemToMatrix(coordinateSystem);
                AecMultiViewBlockReference.DowngradeOpen();
            }
            return this;
        }
        #endregion
    }
}
