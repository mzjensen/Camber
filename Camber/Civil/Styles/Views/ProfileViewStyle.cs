#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccProfileViewStyle = Autodesk.Civil.DatabaseServices.Styles.ProfileViewStyle;
#endregion

namespace Camber.Civil.Styles.Views
{
    public sealed class ProfileViewStyle : Style
    {
        #region properties
        internal AeccProfileViewStyle AeccProfileViewStyle => AcObject as AeccProfileViewStyle;

        /// <summary>
        /// Gets the Axis Style that specifies display properties for the bottom axis of a Profile View.
        /// </summary>
        public AxisStyle BottomAxisStyle => new AxisStyle(this, AeccProfileViewStyle.BottomAxis);

        /// <summary>
        /// Gets the View Style of a Profile View, which specifies all view-related properties.
        /// </summary>
        public ViewStyle ViewStyle => new ViewStyle(this, AeccProfileViewStyle.GraphStyle);

        /// <summary>
        /// Gets the Grid Style of a Profile View, which specifies all grid-related properties.
        /// </summary>
        public GridStyle GridStyle => new GridStyle(this, AeccProfileViewStyle.GridStyle);

        /// <summary>
        /// Gets the Axis Style that specifies display properties for the left axis of a Profile View.
        /// </summary>
        public AxisStyle LeftAxisStyle => new AxisStyle(this, AeccProfileViewStyle.LeftAxis);

        /// <summary>
        /// Gets the Axis Style that specifies display properties for the right axis of a Profile View.
        /// </summary>
        public AxisStyle RightAxisStyle => new AxisStyle(this, AeccProfileViewStyle.RightAxis);

        /// <summary>
        /// Gets the Axis Style that specifies display properties for the top axis of a Profile View.
        /// </summary>
        public AxisStyle TopAxisStyle => new AxisStyle(this, AeccProfileViewStyle.TopAxis);
        #endregion

        #region constructors
        internal ProfileViewStyle(AeccProfileViewStyle aeccProfileViewStyle, bool isDynamoOwned = false) : base(aeccProfileViewStyle, isDynamoOwned) { }

        internal static ProfileViewStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<ProfileViewStyle, AeccProfileViewStyle>
            (styleId, (style) => new ProfileViewStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"ProfileViewStyle(Name = {Name})";
        #endregion
    }
}
