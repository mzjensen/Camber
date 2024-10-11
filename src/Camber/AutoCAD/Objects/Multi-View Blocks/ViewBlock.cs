using System;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using AecViewBlock = Autodesk.Aec.DatabaseServices.MultiViewBlockViewDefinition;

namespace Camber.AutoCAD.Objects.MultiViewBlocks
{
    public sealed class ViewBlock
    {
        #region properties
        internal AecViewBlock AecViewBlock { get; }

        /// <summary>
        /// Gets the Display Representation that a View Block is associated with.
        /// </summary>
        public DisplayRepresentation DisplayRepresentation { get; }

        /// <summary>
        /// Gets the Block assigned to a View Block.
        /// </summary>
        public acDynNodes.Block Block
        {
            get
            {
                var document = acDynNodes.Document.Current;
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    if (AecViewBlock.BlockId != acDb.ObjectId.Null)
                    {
                        var btr = (acDb.BlockTableRecord)ctx.Transaction.GetObject(
                            AecViewBlock.BlockId,
                            acDb.OpenMode.ForRead);
                        return acDynNodes.Block.GetBlockByName(document, btr.Name);
                    }
                }
                return null;
            }
        }
        #endregion

        #region constructors
        internal ViewBlock(AecViewBlock aecViewBlock, DisplayRepresentation displayRepresentation)
        {
            AecViewBlock = aecViewBlock;
            DisplayRepresentation = displayRepresentation;
        }
        #endregion

        #region methods
        public override string ToString() => $"ViewBlock(Block = {Block.Name})";

        /// <summary>
        /// Determines if a View Block is visible when viewed in a given direction.
        /// </summary>
        /// <param name="viewDirection"></param>
        /// <returns></returns>
        public bool IsVisibleInViewDirection(string viewDirection)
        {
            Autodesk.Aec.DatabaseServices.ViewDirection viewDirectionType;
            if (!Enum.TryParse(viewDirection, out viewDirectionType))
            {
                throw new InvalidOperationException("Invalid View Block view direction.");
            }

            using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
            {
                return AecViewBlock.IsViewOn(viewDirectionType);
            }
        }

        /// <summary>
        /// Sets if a View Block should display when viewed in a given direction.
        /// </summary>
        /// <param name="viewDirection"></param>
        /// <param name="isVisible"></param>
        /// <returns></returns>
        public ViewBlock SetIsVisibleInViewDirection(string viewDirection, bool isVisible)
        {
            Autodesk.Aec.DatabaseServices.ViewDirection viewDirectionType;
            if (!Enum.TryParse(viewDirection, out viewDirectionType))
            {
                throw new InvalidOperationException("Invalid View Block view direction.");
            }

            try
            {
                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    DisplayRepresentation.MultiViewBlock.AecMultiViewBlock.UpgradeOpen();
                    AecViewBlock.SetViewOn(viewDirectionType, isVisible);
                    DisplayRepresentation.MultiViewBlock.AecMultiViewBlock.DowngradeOpen();
                }

                return this;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Toggles a View Block display in all view directions.
        /// </summary>
        /// <param name="isVisible"></param>
        /// <returns></returns>
        public ViewBlock SetIsVisibleInAllDirections(bool isVisible)
        {
            try
            {
                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    DisplayRepresentation.MultiViewBlock.AecMultiViewBlock.UpgradeOpen();
                    AecViewBlock.SetAllViews(isVisible);
                    DisplayRepresentation.MultiViewBlock.AecMultiViewBlock.DowngradeOpen();
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
