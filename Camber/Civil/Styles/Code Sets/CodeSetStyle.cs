#region references
using System;
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using AeccCodeSetStyle = Autodesk.Civil.DatabaseServices.Styles.CodeSetStyle;
using AeccCodeSetStyleItem = Autodesk.Civil.DatabaseServices.Styles.CodeSetStyleItem;
using Camber.Civil.Styles.Objects;
#endregion

namespace Camber.Civil.Styles.CodeSets
{
    public sealed class CodeSetStyle : Style
    {
        #region properties
        internal AeccCodeSetStyle AeccCodeSetStyle => AcObject as AeccCodeSetStyle;

        /// <summary>
        /// Gets all of the Code Set Style Items in a Code Set Style.
        /// </summary>
        public IList<CodeSetStyleItem> CodeSetStyleItems
        {
            get
            {
                var items = new List<CodeSetStyleItem>();
                foreach (AeccCodeSetStyleItem item in AeccCodeSetStyle)
                {
                    items.Add(new CodeSetStyleItem(item, this));
                }
                return items;
            }
        }
        #endregion

        #region constructors
        internal CodeSetStyle(AeccCodeSetStyle aeccCodeSetStyle, bool isDynamoOwned = false) : base(aeccCodeSetStyle, isDynamoOwned) { }

        internal static CodeSetStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<CodeSetStyle, AeccCodeSetStyle>
            (styleId, (style) => new CodeSetStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"CodeSetStyle(Name = {Name})";

        /// <summary>
        /// Adds a new point Code Set Style Item by code and Marker Style to a Code Set Style.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="markerStyle"></param>
        /// <returns></returns>
        public CodeSetStyle AddPointItem(string code, MarkerStyle markerStyle) => AddItem(code, markerStyle);

        /// <summary>
        /// Adds a new link Code Set Style Item by code and Marker Style to a Code Set Style.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="linkStyle"></param>
        /// <returns></returns>
        public CodeSetStyle AddLinkItem(string code, LinkStyle linkStyle) => AddItem(code, linkStyle);

        /// <summary>
        /// Adds a new shape Code Set Style Item by code and Marker Style to a Code Set Style.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="shapeStyle"></param>
        /// <returns></returns>
        public CodeSetStyle AddShapeItem(string code, ShapeStyle shapeStyle) => AddItem(code, shapeStyle);

        /// <summary>
        /// Adds a new Code Set Style Item by name and style to a Code Set Style.
        /// The input Style type must be a Marker Style, Link Style, or Shape Style.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="style">Marker Style, Link Style, or Shape Style.</param>
        /// <returns></returns>
        private CodeSetStyle AddItem(string code, Style style)
        {
            if (string.IsNullOrEmpty(code)) { throw new ArgumentNullException("Code is null or empty."); }
            if (!(style is MarkerStyle) && !(style is LinkStyle) && !(style is ShapeStyle))
            {
                throw new ArgumentException("Style type must be a Marker Style, Link Style, or Shape Style.");
            }

            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument.Database))
                {
                    var aeccCodeSetStyle = (AeccCodeSetStyle)ctx.Transaction.GetObject(InternalObjectId, acDb.OpenMode.ForWrite);
                    var newItem = aeccCodeSetStyle.Add(code, style.InternalObjectId);
                    return this;
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// Removes a Code Set Style Item with a given code from a Code Set Style.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public CodeSetStyle RemoveItem(string code)
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccCodeSet = ctx.Transaction.GetObject(InternalObjectId, acDb.OpenMode.ForWrite);
                    AeccCodeSetStyle.Remove(code);
                }
                return this;
            }
            catch { throw; }
        }
        #endregion
    }
}
