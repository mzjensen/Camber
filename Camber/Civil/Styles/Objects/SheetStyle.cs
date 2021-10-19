#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccSheetStyle = Autodesk.Civil.DatabaseServices.Styles.SheetStyle;
#endregion

namespace Camber.Civil.Styles.Objects
{
    public sealed class SheetStyle : Style
    {
        #region properties
        internal AeccSheetStyle AeccSheetStyle => AcObject as AeccSheetStyle;

        /// <summary>
        /// Gets the bottom page margin of a Sheet Style.
        /// </summary>
        public double PageMarginBottom => GetDouble();

        /// <summary>
        /// Gets the left page margin of a Sheet Style.
        /// </summary>
        public double PageMarginLeft => GetDouble();

        /// <summary>
        /// Gets the right page margin of a Sheet Style.
        /// </summary>
        public double PageMarginRight => GetDouble();

        /// <summary>
        /// Gets the top page margin of a Sheet Style.
        /// </summary>
        public double PageMarginTop => GetDouble();
        #endregion

        #region constructors
        internal SheetStyle(AeccSheetStyle aeccSheetStyle, bool isDynamoOwned = false) : base(aeccSheetStyle, isDynamoOwned) { }

        internal static SheetStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<SheetStyle, AeccSheetStyle>
            (styleId, (style) => new SheetStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"SheetStyle(Name = {Name})";

        /// <summary>
        /// Sets the bottom page margin of a Sheet Style.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SheetStyle SetPageMarginBottom(double value) => (SheetStyle)SetValue(value);

        /// <summary>
        /// Sets the left page margin of a Sheet Style.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SheetStyle SetPageMarginLeft(double value) => (SheetStyle)SetValue(value);

        /// <summary>
        /// Sets the right page margin of a Sheet Style.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SheetStyle SetPageMarginRight(double value) => (SheetStyle)SetValue(value);

        /// <summary>
        /// Sets the top page margin of a Sheet Style.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SheetStyle SetPageMarginTop(double value) => (SheetStyle)SetValue(value);
        #endregion
    }
}
