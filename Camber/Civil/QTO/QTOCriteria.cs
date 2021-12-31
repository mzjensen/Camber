#region references
using System.Collections.Generic;
using System.Linq;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using AeccQTOCriteria = Autodesk.Civil.DatabaseServices.Styles.QuantityTakeoffCriteria;
using DynamoServices;
using Camber.Civil.Styles;
#endregion

namespace Camber.Civil.QTO
{
    [RegisterForTrace]
    public class QTOCriteria : Style
    {
        #region properties
        internal AeccQTOCriteria AeccQuantityTakeoffCriteria => AcObject as AeccQTOCriteria;

        /// <summary>
        /// Gets the QTO Criteria Items that belong to a QTO Criteria.
        /// </summary>
        public IList<QTOCriteriaItem> CriteriaItems
        {
            get
            {
                var items = new List<QTOCriteriaItem>();
                for (int i = 0; i < AeccQuantityTakeoffCriteria.Count; i++)
                {
                    items.Add(new QTOCriteriaItem(AeccQuantityTakeoffCriteria[i], this));
                }
                return items;
            }
        }
        #endregion

        #region constructors
        internal QTOCriteria(AeccQTOCriteria aeccQTOCriteria, bool isDynamoOwned = false) : base(aeccQTOCriteria, isDynamoOwned) { }

        internal static QTOCriteria GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<QTOCriteria, AeccQTOCriteria>
            (styleId, (style) => new QTOCriteria(style));
        #endregion

        #region methods
        public override string ToString() => $"QTOCriteria(Name = {Name})";

        /// <summary>
        /// Adds a new empty QTO Criteria Item with default values to a QTO Criteria.
        /// </summary>
        /// <param name="materialName"></param>
        /// <returns></returns>
        public QTOCriteria AddItem(string materialName)
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccCriteria = ctx.Transaction.GetObject(InternalObjectId, acDb.OpenMode.ForWrite);
                    AeccQuantityTakeoffCriteria.AddMaterial(materialName);
                }
                return this;
            }
            catch { throw; }
        }

        /// <summary>
        /// Removes all of the QTO Criteria Items from a QTO Criteria.
        /// </summary>
        /// <returns></returns>
        public QTOCriteria Clear()
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccCriteria = ctx.Transaction.GetObject(InternalObjectId, acDb.OpenMode.ForWrite);
                    AeccQuantityTakeoffCriteria.Clear();
                }
                return this;
            }
            catch { throw; }
        }

        /// <summary>
        /// Removes a QTO Criteria Item by name from a QTO Criteria.
        /// </summary>
        /// <param name="materialName"></param>
        /// <returns></returns>
        public QTOCriteria RemoveItem(string materialName)
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccCriteria = ctx.Transaction.GetObject(InternalObjectId, acDb.OpenMode.ForWrite);
                    int index = CriteriaItems.IndexOf(CriteriaItems.Where(p => p.MaterialName == materialName).FirstOrDefault());
                    AeccQuantityTakeoffCriteria.RemoveAt(index);
                }
                return this;
            }
            catch { throw; }
        }
        #endregion
    }
}
