#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccSectionViewStyle = Autodesk.Civil.DatabaseServices.Styles.SectionViewStyle;
#endregion

namespace Camber.Civil.Styles.Views
{
    public sealed class SectionViewStyle : Style
    {
        #region properties
        internal AeccSectionViewStyle AeccSectionViewStyle => AcObject as AeccSectionViewStyle;

        /// <summary>
        /// Gets the Axis Style that specifies display properties for the bottom axis of a Section View.
        /// </summary>
        public AxisStyle BottomAxisStyle => new AxisStyle(this, AeccSectionViewStyle.BottomAxis);

        /// <summary>
        /// Gets the Axis Style that specifies display properties for the center axis of a Section View.
        /// </summary>
        public AxisStyle CenterAxisStyle => new AxisStyle(this, AeccSectionViewStyle.CenterAxis);

        /// <summary>
        /// Gets the View Style of a Section View, which specifies all view-related properties.
        /// </summary>
        public ViewStyle ViewStyle => new ViewStyle(this, AeccSectionViewStyle.GraphStyle);

        /// <summary>
        /// Gets the Grid Style of a Section View, which specifies all grid-related properties.
        /// </summary>
        public GridStyle GridStyle => new GridStyle(this, AeccSectionViewStyle.GridStyle);

        /// <summary>
        /// Gets the Axis Style that specifies display properties for the left axis of a Section View.
        /// </summary>
        public AxisStyle LeftAxisStyle => new AxisStyle(this, AeccSectionViewStyle.LeftAxis);

        /// <summary>
        /// Gets the Axis Style that specifies display properties for the right axis of a Section View.
        /// </summary>
        public AxisStyle RightAxisStyle => new AxisStyle(this, AeccSectionViewStyle.RightAxis);

        /// <summary>
        /// Gets the Axis Style that specifies display properties for the top axis of a Section View.
        /// </summary>
        public AxisStyle TopAxisStyle => new AxisStyle(this, AeccSectionViewStyle.TopAxis);
        #endregion

        #region constructors
        internal SectionViewStyle(AeccSectionViewStyle aeccSectionViewStyle, bool isDynamoOwned = false) : base(aeccSectionViewStyle, isDynamoOwned) { }

        internal static SectionViewStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<SectionViewStyle, AeccSectionViewStyle>
            (styleId, (style) => new SectionViewStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"SectionViewStyle(Name = {Name})";
        #endregion
    }
}
