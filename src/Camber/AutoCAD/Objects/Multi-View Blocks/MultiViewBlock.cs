using System;
using Autodesk.AutoCAD.DynamoNodes;
using System.Collections.Generic;
using System.Threading;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using aecDb = Autodesk.Aec.DatabaseServices;
using AecMultiViewBlock = Autodesk.Aec.DatabaseServices.MultiViewBlockDefinition;
using AecMultiViewBlockReference = Autodesk.Aec.DatabaseServices.MultiViewBlockReference;

namespace Camber.AutoCAD.Objects.MultiViewBlocks
{
    public  sealed class MultiViewBlock : ObjectBase
    {
        #region properties
        internal AecMultiViewBlock AecMultiViewBlock => AcObject as AecMultiViewBlock;

        /// <summary>
        /// Gets the name of a Multi-View Block.
        /// </summary>
        public string Name => AecMultiViewBlock.Name;

        /// <summary>
        /// Gets the description of a Multi-View Block.
        /// </summary>
        public string Description => AecMultiViewBlock.Description;

        /// <summary>
        /// Gets if a Multi-View Block is locked.
        /// </summary>
        public bool IsLocked => AecMultiViewBlock.IsLocked;

        /// <summary>
        /// Gets all of the Blocks referenced in a Multi-View Block definition.
        /// </summary>
        public List<acDynNodes.Block> ReferencedBlocks
        {
            get
            {
                acDynNodes.Document document = acDynNodes.Document.Current;
                List<acDynNodes.Block> blks = new List<acDynNodes.Block>();

                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    foreach (acDb.ObjectId oid in AecMultiViewBlock.GetAllBlocksReferenced())
                    {
                        var acBlk = (acDb.BlockTableRecord) ctx.Transaction.GetObject(oid, acDb.OpenMode.ForRead);
                        blks.Add(acDynNodes.Block.GetBlockByName(document, acBlk.Name));
                    }
                }
                return blks;
            }
        }

        /// <summary>
        /// Gets the Block defined as an interference Block in a Multi-View Block definition.
        /// </summary>
        public acDynNodes.Block InterferenceBlock
        {
            get
            {
                acDynNodes.Document document = acDynNodes.Document.Current;

                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    if (AecMultiViewBlock.InterferenceBlockId != acDb.ObjectId.Null)
                    {
                        var acBlk = (acDb.BlockTableRecord)ctx.Transaction.GetObject(
                            AecMultiViewBlock.InterferenceBlockId,
                            acDb.OpenMode.ForRead);
                        return acDynNodes.Block.GetBlockByName(document, acBlk.Name);
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets all of the Display Representations for a Multi-View Block.
        /// </summary>
        public List<DisplayRepresentation> DisplayRepresentations
        {
            get
            {
                acDynNodes.Document document = acDynNodes.Document.Current;
                List<DisplayRepresentation> dispReps = new List<DisplayRepresentation>();

                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    foreach (aecDb.MultiViewBlockDisplayRepresentationDefinition dispRep in AecMultiViewBlock.DisplayRepresentationDefinitions)
                    {
                        dispReps.Add(new DisplayRepresentation(dispRep, this));
                    }
                }
                return dispReps;
            }
        }

        /// <summary>
        /// Gets the Multi-View Block References of a Multi-View Block.
        /// </summary>
        public List<MultiViewBlockReference> MultiViewBlockReferences
        {
            get
            {
                var document = acDynNodes.Document.Current;
                var mvBlkRefs = new List<MultiViewBlockReference>();

                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    // Strange that there isn't a method to get the MVBlock Ref IDs of an MVBlock like there is for normal Blocks...
                    var bt = (acDb.BlockTable)ctx.Transaction.GetObject(AcDatabase.BlockTableId, acDb.OpenMode.ForRead);
                    foreach (var btrOid in bt)
                    {
                        var btr = (acDb.BlockTableRecord) ctx.Transaction.GetObject(btrOid, acDb.OpenMode.ForRead);
                        foreach (var oid in btr)
                        {
                            var obj = ctx.Transaction.GetObject(oid, acDb.OpenMode.ForRead);
                            if (obj is AecMultiViewBlockReference mvBlkRef)
                            {
                                mvBlkRefs.Add(new MultiViewBlockReference(mvBlkRef, false));
                            }
                        }
                    }
                }
                return mvBlkRefs;
            }
        }
        #endregion

        #region constructors
        internal MultiViewBlock(AecMultiViewBlock aecMultiViewBlock, bool isDynamoOwned = false)
            : base(aecMultiViewBlock, isDynamoOwned) { }
        #endregion

        #region methods
        public override string ToString() => $"MultiViewBlock(Name = {Name})";

        /// <summary>
        /// Sets if a Multi-View Block is locked.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public MultiViewBlock SetIsLocked(bool @bool)
        {
            using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
            {
                AecMultiViewBlock.UpgradeOpen();
                AecMultiViewBlock.IsLocked = @bool;
                AecMultiViewBlock.DowngradeOpen();
            }
            return this;
        }

        /// <summary>
        /// Sets the description of a Multi-View Block.
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public MultiViewBlock SetDescription(string description)
        {
            using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
            {
                AecMultiViewBlock.UpgradeOpen();
                AecMultiViewBlock.Description = description;
                AecMultiViewBlock.DowngradeOpen();
            }
            return this;
        }

        /// <summary>
        /// Sets the interference Block for a Multi-View Block.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public MultiViewBlock SetInterferenceBlock(acDynNodes.Block block)
        {
            var document = acDynNodes.Document.Current;

            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    AecMultiViewBlock.UpgradeOpen();
                    var bt = (acDb.BlockTable) ctx.Transaction.GetObject(
                        document.AcDocument.Database.BlockTableId,
                        acDb.OpenMode.ForRead);
                    var blkId = bt[block.Name];
                    if (blkId != acDb.ObjectId.Null)
                    {
                        AecMultiViewBlock.InterferenceBlockId = blkId;
                    }
                }

                return this;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
        #endregion
    }
}
