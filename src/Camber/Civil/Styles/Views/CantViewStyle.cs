#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccCantViewStyle = Autodesk.Civil.DatabaseServices.Styles.CantViewStyle;
#endregion

namespace Camber.Civil.Styles.Views
{
    public sealed class CantViewStyle : Style
    {
        #region properties
        internal AeccCantViewStyle AeccCantViewStyle => AcObject as AeccCantViewStyle;

        /// <summary>
        /// Gets the spacing of major ticks on the bottom horizontal axis. 
        /// </summary>
        public double AxisBottomMajorTickInterval => GetDouble();

        /// <summary>
        /// Gets the spacing of major ticks on the top horizontal axis.
        /// </summary>
        public double AxisTopMajorTickInterval => GetDouble();

        /// <summary>
        /// Gets whether full height ticks are used or not.
        /// </summary>
        public bool UseFullHeightTick => GetBool();

        /// <summary>
        /// Gets whether to select the tick location at bottom.
        /// </summary>
        public bool UseSmallTicksAtBottom => GetBool();

        /// <summary>
        /// Gets whether to select the tick location at middle.
        /// </summary>
        public bool UseSmallTicksAtMiddle => GetBool();

        /// <summary>
        /// Gets whether to select the tick location at top.
        /// </summary>
        public bool UseSmallTicksAtTop => GetBool();

        /// <summary>
        /// Gets the vertical exaggeration.
        /// </summary>
        public double VerticalExaggeration => GetDouble();
        #endregion

        #region constructors
        internal CantViewStyle(AeccCantViewStyle aeccCantViewStyle, bool isDynamoOwned = false) : base(aeccCantViewStyle, isDynamoOwned) { }

        internal static CantViewStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<CantViewStyle, AeccCantViewStyle>
            (styleId, (style) => new CantViewStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"CantViewStyle(Name = {Name})";

        /// <summary>
        /// Sets the spacing of major ticks on the bottom horizontal axis. 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public CantViewStyle SetAxisBottomMajorTickInterval(double value) => (CantViewStyle)SetValue(value);


        /// <summary>
        /// Sets the spacing of major ticks on the top horizontal axis.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public CantViewStyle SetAxisTopMajorTickInterval(double value) => (CantViewStyle)SetValue(value);


        /// <summary>
        /// Sets whether full height ticks are used or not.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public CantViewStyle SetUseFullHeightTick(bool @bool) => (CantViewStyle)SetValue(@bool);


        /// <summary>
        /// Sets whether to select the tick location at bottom.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public CantViewStyle SetUseSmallTicksAtBottom(bool @bool) => (CantViewStyle)SetValue(@bool);


        /// <summary>
        /// Sets whether to select the tick location at middle.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public CantViewStyle SetUseSmallTicksAtMiddle(bool @bool) => (CantViewStyle)SetValue(@bool);


        /// <summary>
        /// Sets whether to select the tick location at top.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public CantViewStyle SetUseSmallTicksAtTop(bool @bool) => (CantViewStyle)SetValue(@bool);


        /// <summary>
        /// Sets the vertical exaggeration.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public CantViewStyle SetVerticalExaggeration(double value) => (CantViewStyle)SetValue(value);

        #endregion
    }
}
