#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccSuperelevationViewStyle = Autodesk.Civil.DatabaseServices.Styles.SuperelevationViewStyle;
#endregion

namespace Camber.Civil.Styles.Graphs
{
    public sealed class SuperelevationViewStyle : Style
    {
        #region properties
        internal AeccSuperelevationViewStyle AeccSuperelevationViewStyle => AcObject as AeccSuperelevationViewStyle;

        /// <summary>
        /// Gets the spacing of major ticks on the horizontal bottom axis of a Superelevation View.
        /// </summary>
        public double AxisBottomMajorTickInterval => GetDouble();

        /// <summary>
        /// Gets the spacing of major ticks at the horizontal top axis of a Superelevation View.
        /// </summary>
        public double AxisTopMajorTickInterval => GetDouble();

        /// <summary>
        /// Gets whether to use full-height ticks in a Superelevation View.
        /// </summary>
        public bool UseFullHeightTick => GetBool();

        /// <summary>
        /// Gets whether to use small ticks at the bottom of a Superelevation View.
        /// </summary>
        public bool UseSmallTicksAtBottom => GetBool();

        /// <summary>
        /// Gets whether to use small ticks at the middle of a Superelevation View.
        /// </summary>
        public bool UseSmallTicksAtMiddle => GetBool();

        /// <summary>
        /// Gets whether to use small ticks at the top of a Superelevation View.
        /// </summary>
        public bool UseSmallTicksAtTop => GetBool();

        /// <summary>
        /// Gets the vertical height unit of a Superelevation View.
        /// </summary>
        public double VerticalHeightUnit => GetDouble();
        #endregion

        #region constructors
        internal SuperelevationViewStyle(AeccSuperelevationViewStyle aeccSuperelevationViewStyle, bool isDynamoOwned = false) : base(aeccSuperelevationViewStyle, isDynamoOwned) { }

        internal static SuperelevationViewStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<SuperelevationViewStyle, AeccSuperelevationViewStyle>
            (styleId, (style) => new SuperelevationViewStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"SuperelevationViewStyle(Name = {Name})";

        /// <summary>
        /// Sets the spacing of major ticks on the horizontal bottom axis of a Superelevation View.
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public SuperelevationViewStyle SetAxisBottomMajorTickInterval(double interval) => (SuperelevationViewStyle)SetValue(interval);

        /// <summary>
        /// Sets the spacing of major ticks at the horizontal top axis of a Superelevation View.
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public SuperelevationViewStyle SetAxisTopMajorTickInterval(double interval) => (SuperelevationViewStyle)SetValue(interval);

        /// <summary>
        /// Sets whether to use full-height ticks in a Superelevation View.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public SuperelevationViewStyle SetUseFullHeightTick(bool @bool) => (SuperelevationViewStyle)SetValue(@bool);

        /// <summary>
        /// Sets whether to use small ticks at the bottom of a Superelevation View.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public SuperelevationViewStyle SetUseSmallTicksAtBottom(bool @bool) => (SuperelevationViewStyle)SetValue(@bool);

        /// <summary>
        /// Sets whether to use small ticks at the middle of a Superelevation View.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public SuperelevationViewStyle SetUseSmallTicksAtMiddle(bool @bool) => (SuperelevationViewStyle)SetValue(@bool);

        /// <summary>
        /// Sets whether to use small ticks at the top of a Superelevation View.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public SuperelevationViewStyle SetUseSmallTicksAtTop(bool @bool) => (SuperelevationViewStyle)SetValue(@bool);

        /// <summary>
        /// Sets the vertical height unit of a Superelevation View.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public SuperelevationViewStyle SetVerticalHeightUnit(bool @bool) => (SuperelevationViewStyle)SetValue(@bool);
        #endregion
    }
}