#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccSlopePatternStyle = Autodesk.Civil.DatabaseServices.Styles.SlopePatternStyle;
#endregion

namespace Camber.Civil.Styles.Objects
{
    public sealed class SlopePatternStyle : Style
    {
        #region properties
        internal AeccSlopePatternStyle AeccSlopePatternStyle => AcObject as AeccSlopePatternStyle;

        /// <summary>
        /// Gets the minimum slope length on which to display the slope pattern in a Slope Pattern Style.
        /// </summary>
        public double MinDisplayLength => GetDouble();

        /// <summary>
        /// Gets the length of the Feature Line to display in a Slope Pattern Style.
        /// </summary>
        public double PreviewFeatureLength => GetDouble();

        /// <summary>
        /// Gets the slope value to display in a Slope Pattern Style.
        /// </summary>
        public double PreviewSlope => GetDouble();

        /// <summary>
        /// Gets the length of the slope to display in a Slope Pattern Style.
        /// </summary>
        public double PreviewSlopeLength => GetDouble();
        #endregion

        #region constructors
        internal SlopePatternStyle(AeccSlopePatternStyle aeccSlopePatternStyle, bool isDynamoOwned = false) : base(aeccSlopePatternStyle, isDynamoOwned) { }

        internal static SlopePatternStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<SlopePatternStyle, AeccSlopePatternStyle>
            (styleId, (style) => new SlopePatternStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"SlopePatternStyle(Name = {Name})";

        /// <summary>
        /// Sets the minimum slope length on which to display the slope pattern in a Slope Pattern Style.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SlopePatternStyle SetMinDisplayLength(double value) => (SlopePatternStyle)SetValue(value);

        /// <summary>
        /// Sets the length of the Feature Line to display in a Slope Pattern Style.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SlopePatternStyle SetPreviewFeatureLength(double value) => (SlopePatternStyle)SetValue(value);

        /// <summary>
        /// Sets the slope value to display in a Slope Pattern Style.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SlopePatternStyle SetPreviewSlope(double value) => (SlopePatternStyle)SetValue(value);

        /// <summary>
        /// Sets the length of the slope to display in a Slope Pattern Style.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SlopePatternStyle SetPreviewSlopeLength(double value) => (SlopePatternStyle)SetValue(value);
        #endregion
    }
}
