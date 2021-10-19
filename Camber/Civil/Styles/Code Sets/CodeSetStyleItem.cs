#region references
using System;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civDb = Autodesk.Civil.DatabaseServices;
using AeccCodeSetStyleItem = Autodesk.Civil.DatabaseServices.Styles.CodeSetStyleItem;
using System.Runtime.CompilerServices;
using System.Reflection;
using Camber.Civil.Styles.Objects;
#endregion


namespace Camber.Civil.Styles.CodeSets
{
    public sealed class CodeSetStyleItem
    {
        #region properties
        private AeccCodeSetStyleItem AeccCodeSetStyleItem { get; set; }

        /// <summary>
        /// Gets the Code Set Style that a Code Set Style Item belongs to.
        /// </summary>
        public CodeSetStyle CodeSetStyle { get; private set; }

        /// <summary>
        /// Gets the classification of a Code Set Style Item.
        /// </summary>
        public string Classification => AeccCodeSetStyleItem.Classification;

        /// <summary>
        /// Gets the code of a Code Set Style Item.
        /// </summary>
        public string Code => AeccCodeSetStyleItem.Code;

        /// <summary>
        /// Gets the Marker, Link, or Shape Style assigned to a Code Set Style Item.
        /// </summary>
        public Style CodeStyle
        {
            get
            {
                var styleType = AeccCodeSetStyleItem.StyleType;
                if (styleType is civDb.Styles.SubassemblySubentityStyleType.MarkerType)
                {
                    return MarkerStyle.GetByObjectId(AeccCodeSetStyleItem.CodeStyleId);
                }
                else if (styleType is civDb.Styles.SubassemblySubentityStyleType.LinkType)
                {
                    throw new Exception("Not implemented.");
                }
                else if (styleType is civDb.Styles.SubassemblySubentityStyleType.ShapeType)
                {
                    throw new Exception("Not implemented.");
                }
                else
                {
                    throw new Exception("Invalid style type.");
                }
            }
        }

        /// <summary>
        /// Gets the description of a Code Set Style Item.
        /// </summary>
        public string Description => AeccCodeSetStyleItem.Description;

        /// <summary>
        /// Gets the Feature Line Style of a Code Set Style Item.
        /// </summary>
        public FeatureLineStyle FeatureLineStyle
        {
            get
            {
                var oid = AeccCodeSetStyleItem.FeatureLineStyleId;
                if (oid.IsNull) { return null; }
                return FeatureLineStyle.GetByObjectId(oid);
            }
        }

        /// <summary>
        /// Gets the item type of a Code Set Style Item.
        /// </summary>
        public string ItemType => AeccCodeSetStyleItem.ItemType.ToString();

        /// <summary>
        /// Gets the list of Pay Items assigned to a Code Set Style Item.
        /// </summary>
        public string[] PayItems => AeccCodeSetStyleItem.PayItems;

        /// <summary>
        /// Gets the render material style assigned to a Code Set Style Item.
        /// </summary>
        public string RenderMaterialStyle
        {
            get
            {
                try
                {
                    return AeccCodeSetStyleItem.RenderMaterialStyleName;
                }
                catch { return null; }
            }
        }

        /// <summary>
        /// Gets the subassembly subentity style type of a Code Set Style Item.
        /// </summary>
        public string SubentityStyleType => AeccCodeSetStyleItem.StyleType.ToString();
        #endregion

        #region constructors
        internal CodeSetStyleItem(AeccCodeSetStyleItem aeccCodeSetStyleItem, CodeSetStyle codeSetStyle)
        {
            AeccCodeSetStyleItem = aeccCodeSetStyleItem;
            CodeSetStyle = codeSetStyle;
        }
        #endregion

        #region methods
        public override string ToString() => $"CodeSetStyleItem(Code = {Code})";

        protected CodeSetStyleItem SetValue(object value, [CallerMemberName] string methodName = null)
        {
            if (methodName.StartsWith("Set"))
            {
                methodName = methodName.Substring(3);
            }
            return SetValue(methodName, value);
        }

        protected CodeSetStyleItem SetValue(string propertyName, object value)
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccCodeSet = ctx.Transaction.GetObject(CodeSetStyle.InternalObjectId, acDb.OpenMode.ForWrite);
                    PropertyInfo propInfo = AeccCodeSetStyleItem.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                    propInfo?.SetValue(AeccCodeSetStyleItem, value);
                    return this;
                }
            }
            catch (Exception e) { throw e.InnerException; }
        }

        /// <summary>
        /// Sets the classification of a Code Set Style Item.
        /// </summary>
        /// <param name="classification"></param>
        /// <returns></returns>
        public CodeSetStyleItem SetClassification(string classification) => SetValue(classification);

        /// <summary>
        /// Sets the code of a Code Set Style Item.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public CodeSetStyleItem SetCode(string code) => SetValue(code);

        /// <summary>
        /// Sets the Marker, Link, or Shape Style assigned to a Code Set Style Item.
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        public CodeSetStyleItem SetCodeStyle(Style style) => SetValue(style.InternalObjectId, "CodeStyleId");

        /// <summary>
        /// Sets the description of a Code Set Style Item.
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public CodeSetStyleItem SetDescription(string description) => SetValue(description);

        /// <summary>
        /// Sets the Feature Line Style of a Code Set Style Item.
        /// </summary>
        /// <param name="featureLineStyle"></param>
        /// <returns></returns>
        public CodeSetStyleItem SetFeatureLineStyle(FeatureLineStyle featureLineStyle) => SetValue(featureLineStyle.InternalObjectId, "FeatureLineStyleId");

        /// <summary>
        /// Sets the list of Pay Items assigned to a Code Set Style Item.
        /// </summary>
        /// <param name="payItems"></param>
        /// <returns></returns>
        public CodeSetStyleItem SetPayItems(string[] payItems) => SetValue(payItems);

        /// <summary>
        /// Sets the render material style assigned to a Code Set Style Item.
        /// Use an empty string to set the render material to "<none>".
        /// </summary>
        /// <param name="renderMaterial"></param>
        /// <returns></returns>
        public CodeSetStyleItem SetRenderMaterialStyle(string renderMaterial) => SetValue((object)renderMaterial, "RenderMaterialStyleName");
        #endregion
    }
}
