using System;
using System.Collections.Generic;
using System.Linq;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using aecDb = Autodesk.Aec.DatabaseServices;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AecDisplayRepresentation = Autodesk.Aec.DatabaseServices.MultiViewBlockDisplayRepresentationDefinition;
using AecMultiViewBlock = Autodesk.Aec.DatabaseServices.MultiViewBlockDefinition;

namespace Camber.AutoCAD.Objects.MultiViewBlocks
{
    public sealed class DisplayRepresentation
    {
        #region properties
        internal AecDisplayRepresentation AecDisplayRepresentation { get; }
        
        /// <summary>
        /// Gets the Multi-View Block definition that a Display Representation is associated with.
        /// </summary>
        public MultiViewBlock MultiViewBlock { get; }

        /// <summary>
        /// Gets the name of a Display Representation.
        /// </summary>
        public string Name
        {
            get
            {
                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    var dispRep = (aecDb.DisplayRepresentation)ctx.Transaction.GetObject(
                        AecDisplayRepresentation.DisplayRepresentationId,
                        acDb.OpenMode.ForRead);
                    return dispRep.ViewTypeDisplayName;
                }
            }
        }

        /// <summary>
        /// Gets all of the View Blocks assigned to a Display Representation.
        /// </summary>
        public List<ViewBlock> ViewBlocks
        {
            get
            {
                acDynNodes.Document document = acDynNodes.Document.Current;
                List<ViewBlock> viewBlks = new List<ViewBlock>();

                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    foreach (aecDb.MultiViewBlockViewDefinition viewBlk in AecDisplayRepresentation.ViewDefinitions)
                    {
                        viewBlks.Add(new ViewBlock(viewBlk, this));
                    }
                }
                return viewBlks;
            }
        }
        #endregion

        #region constructors
        internal DisplayRepresentation(AecDisplayRepresentation aecDisplayRepresentation, MultiViewBlock multiViewBlock)
        {
            AecDisplayRepresentation = aecDisplayRepresentation;
            MultiViewBlock = multiViewBlock;
        }
        #endregion

        #region methods
        public override string ToString() => $"DisplayRepresentation(Name = {Name})";

        /// <summary>
        /// Adds a new View Block to a Display Representation.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="isVisibleFromTop"></param>
        /// <param name="isVisibleFromBottom"></param>
        /// <param name="isVisibleFromFront"></param>
        /// <param name="isVisibleFromBack"></param>
        /// <param name="isVisibleFromLeft"></param>
        /// <param name="isVisibleFromRight"></param>
        /// <param name="isVisibleFromOther"></param>
        /// <returns></returns>
        public DisplayRepresentation AddViewBlock(
            acDynNodes.Block block,
            bool isVisibleFromTop = true,
            bool isVisibleFromBottom = true,
            bool isVisibleFromFront = true,
            bool isVisibleFromBack = true,
            bool isVisibleFromLeft = true,
            bool isVisibleFromRight = true,
            bool isVisibleFromOther = true)
        {
            if (ViewBlocks.Any(b => b.Block.Name == block.Name))
            {
                throw new InvalidOperationException(
                    $"Block \"{block.Name}\" already exists for this Display Representation.");
            }
            
            var document = acDynNodes.Document.Current;

            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    MultiViewBlock.AecMultiViewBlock.UpgradeOpen();
                    var bt = (acDb.BlockTable) ctx.Transaction.GetObject(
                        document.AcDocument.Database.BlockTableId,
                        acDb.OpenMode.ForRead);
                    var blkId = bt[block.Name];

                    var viewBlk = new aecDb.MultiViewBlockViewDefinition()
                    {
                        BlockId = blkId
                    };
                    viewBlk.SetViewOn(aecDb.ViewDirection.Top, isVisibleFromTop);
                    viewBlk.SetViewOn(aecDb.ViewDirection.Bottom, isVisibleFromBottom);
                    viewBlk.SetViewOn(aecDb.ViewDirection.Front, isVisibleFromFront);
                    viewBlk.SetViewOn(aecDb.ViewDirection.Back, isVisibleFromBack);
                    viewBlk.SetViewOn(aecDb.ViewDirection.Left, isVisibleFromLeft);
                    viewBlk.SetViewOn(aecDb.ViewDirection.Right, isVisibleFromRight);
                    viewBlk.SetViewOn(aecDb.ViewDirection.Other, isVisibleFromOther);
                    AecDisplayRepresentation.ViewDefinitions.Add(viewBlk);
                    MultiViewBlock.AecMultiViewBlock.DowngradeOpen();
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
