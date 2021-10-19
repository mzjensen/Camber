#region references
using System;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using civDb = Autodesk.Civil.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using AeccQTOCriteriaData = Autodesk.Civil.DatabaseServices.Styles.QuantityTakeoffCriteriaData;
using DynamoServices;
#endregion


namespace Camber.Civil.Styles.QTO
{
    [RegisterForTrace]
    public sealed class QTOCriteriaData
    {
        #region properties
        private AeccQTOCriteriaData AeccQTOCriteriaData { get; set; }

        /// <summary>
        /// Gets the QTO Criteria Item that a QTO Criteria Data object belongs to.
        /// </summary>
        public QTOCriteriaItem CriteriaItem { get; private set; }

        /// <summary>
        /// Gets the condition of a QTO Criteria Data object.
        /// </summary>
        public string Condition => AeccQTOCriteriaData.Condition.ToString();

        /// <summary>
        /// Gets the type of a QTO Criteria Data object. 
        /// </summary> 
        public string ItemType => AeccQTOCriteriaData.ItemType.ToString();
        /// <summary>
        /// Gets the name of a Quantity Takeoff Criteria Data object.
        /// </summary>
        public string Name => AeccQTOCriteriaData.Name;
        #endregion

        #region constructors
        internal QTOCriteriaData(AeccQTOCriteriaData aeccQTOCriteriaData, QTOCriteriaItem QuantityTakeoffCriteriaItem)
        {
            AeccQTOCriteriaData = aeccQTOCriteriaData;
            CriteriaItem = QuantityTakeoffCriteriaItem;
        }
        #endregion

        #region methods
        public override string ToString() => $"QTOCriteriaData(Name = {Name}, Type = {ItemType}, Condition = {Condition})";

        /// <summary>
        /// Sets the condition of a QTO Criteria Data object. 
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public QTOCriteriaData SetCondition(string condition)
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccCriteria = ctx.Transaction.GetObject(CriteriaItem.Criteria.InternalObjectId, acDb.OpenMode.ForWrite);
                    AeccQTOCriteriaData.Condition = (civDb.MaterialConditionType)Enum.Parse(typeof(civDb.MaterialConditionType), condition);
                    return this;
                }
            }
            catch (Exception e) { throw e.InnerException; }
        }
        #endregion
    }
}