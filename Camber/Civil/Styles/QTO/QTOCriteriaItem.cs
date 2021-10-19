#region references
using System;
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using civDb = Autodesk.Civil.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using AeccQTOCriteriaItem = Autodesk.Civil.DatabaseServices.Styles.QuantityTakeoffCriteriaItem;
using DynamoServices;
using System.Runtime.CompilerServices;
using System.Reflection;
#endregion


namespace Camber.Civil.Styles.QTO
{
    [RegisterForTrace]
    public sealed class QTOCriteriaItem
    {
        #region properties
        private AeccQTOCriteriaItem AeccQTOCriteriaItem { get; set; }

        /// <summary>
        /// Gets the QTO Criteria that a QTO Criteria Item belongs to.
        /// </summary>
        public QTOCriteria Criteria { get; private set; }

        /// <summary>
        /// Gets the expansion or swell of the cut material for a QTO Criteria Item.
        /// </summary>
        public double CutFactor => AeccQTOCriteriaItem.CutFactor;

        /// <summary>
        /// Gets the contraction or shrinkage of the fill material for a QTO Criteria Item.
        /// </summary>
        public double FillFactor => AeccQTOCriteriaItem.FillFactor;

        /// <summary>
        /// Gets the material name for a QTO Criteria Item.
        /// </summary>
        public string MaterialName => AeccQTOCriteriaItem.MaterialName;

        /// <summary>
        /// Gets the material quantity type for a QTO Criteria Item.
        /// </summary>
        public string QuantityType => AeccQTOCriteriaItem.QuantityType.ToString();

        /// <summary>
        /// Gets the usability factor used to calculate how much cut material can be reused as fill for a QTO Criteria Item.
        /// </summary>
        public double ReFillFactor => AeccQTOCriteriaItem.ReFillFactor;

        /// <summary>
        /// Gets the Quantity Takeoff Criteria Data for a QTO Criteria Item.
        /// </summary>
        public IList<QTOCriteriaData> CriteriaData
        {
            get
            {
                var dataItems = new List<QTOCriteriaData>();
                for (int i = 0; i < AeccQTOCriteriaItem.Count; i++)
                {
                    dataItems.Add(new QTOCriteriaData(AeccQTOCriteriaItem[i], this));
                }
                return dataItems;
            }
        }
        #endregion

        #region constructors
        internal QTOCriteriaItem(AeccQTOCriteriaItem aeccQTOCriteriaItem, QTOCriteria QuantityTakeoffCriteria)
        {
            AeccQTOCriteriaItem = aeccQTOCriteriaItem;
            Criteria = QuantityTakeoffCriteria;
        }
        #endregion

        #region methods
        public override string ToString() => $"QTOCriteriaItem(Material Name = {MaterialName}, Type = {QuantityType})";

        protected QTOCriteriaItem SetValue(object value, [CallerMemberName] string methodName = null)
        {
            if (methodName.StartsWith("Set"))
            {
                methodName = methodName.Substring(3);
            }
            return SetValue(methodName, value);
        }

        protected QTOCriteriaItem SetValue(string propertyName, object value)
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccCriteria = ctx.Transaction.GetObject(Criteria.AeccQuantityTakeoffCriteria.ObjectId, acDb.OpenMode.ForWrite);
                    PropertyInfo propInfo = AeccQTOCriteriaItem.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                    propInfo?.SetValue(AeccQTOCriteriaItem, value);
                    return this;
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// Adds a new corridor shape QTO Criteria Data with default values to a QTO Criteria Item.
        /// </summary>
        /// <param name="shapeName"></param>
        /// <returns></returns>
        public QTOCriteriaItem AddCorridorShape(string shapeName)
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccCriteria = ctx.Transaction.GetObject(Criteria.InternalObjectId, acDb.OpenMode.ForWrite);
                    AeccQTOCriteriaItem.AddCorridorShape(shapeName);
                    return this;
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// Adds a new surface QTO Criteria Data with default values to a QTO Criteria Item.
        /// </summary>
        /// <param name="surface"></param>
        /// <returns></returns>
        public QTOCriteriaItem AddSurface(civDynNodes.Surface surface)
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccCriteria = ctx.Transaction.GetObject(Criteria.InternalObjectId, acDb.OpenMode.ForWrite);
                    AeccQTOCriteriaItem.AddSurface(surface.Name);
                    return this;
                }
            }
            catch { throw; }
        }


        /// <summary>
        /// Remove a corridor shape QTO Criteria Data from a QTO Criteria Item.
        /// </summary>
        /// <param name="shapeName"></param>
        /// <returns></returns>
        public QTOCriteriaItem RemoveCorridorShape(string shapeName)
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccCriteria = ctx.Transaction.GetObject(Criteria.InternalObjectId, acDb.OpenMode.ForWrite);
                    AeccQTOCriteriaItem.RemoveCorridorShape(shapeName);
                    return this;
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// Removes a surface QTO Criteria Data from a QTO Criteria Item.
        /// </summary>
        /// <param name="surface"></param>
        /// <returns></returns>
        public QTOCriteriaItem RemoveSurface(civDynNodes.Surface surface)
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccCriteria = ctx.Transaction.GetObject(Criteria.InternalObjectId, acDb.OpenMode.ForWrite);
                    AeccQTOCriteriaItem.RemoveSurface(surface.Name);
                    return this;
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// Sets the expansion or swell of the cut material for a QTO Criteria Item.
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        public QTOCriteriaItem SetCutFactor(double factor) => SetValue(factor);

        /// <summary>
        /// Sets the contraction or shrinkage of the fill material for a QTO Criteria Item.
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        public QTOCriteriaItem SetFillFactor(double factor) => SetValue(factor);

        /// <summary>
        /// Sets the material name for a QTO Criteria Item.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public QTOCriteriaItem SetMaterialName(string name) => SetValue(name);

        /// <summary>
        /// Sets the material quantity type for a QTO Criteria Item.
        /// </summary>
        /// <param name="quantityType"></param>
        /// <returns></returns>
        public QTOCriteriaItem SetQuantityType(string quantityType) => SetValue(Enum.Parse(typeof(civDb.MaterialQuantityType), quantityType));

        /// <summary>
        /// Sets the usability factor used to calculate how much cut material can be reused as fill for a QTO Criteria Item.
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        public QTOCriteriaItem SetReFillFactor(double factor) => SetValue(factor);
        #endregion
    }
}
