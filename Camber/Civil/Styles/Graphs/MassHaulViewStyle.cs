#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccMassHaulViewStyle = Autodesk.Civil.DatabaseServices.Styles.MassHaulViewStyle;
#endregion

namespace Camber.Civil.Styles.Graphs
{
    public sealed class MassHaulViewStyle : Style
    {
        #region properties
        internal AeccMassHaulViewStyle AeccMassHaulViewStyle => AcObject as AeccMassHaulViewStyle;
        /// <summary>
        /// Gets the Axis Style that specifies display properties for the bottom axis of a Mass Haul View.
        /// </summary>
        public AxisStyle BottomAxisStyle => new AxisStyle(this, AeccMassHaulViewStyle.BottomAxis);

        /// <summary>
        /// Gets the Graph Style of a Mass Haul View, which specifies all graph-related properties.
        /// </summary>
        public GraphStyle GraphStyle => new GraphStyle(this, AeccMassHaulViewStyle.GraphStyle);

        /// <summary>
        /// Gets the Grid Style of a Mass Haul View, which specifies all grid-related properties.
        /// </summary>
        public GridStyle GridStyle => new GridStyle(this, AeccMassHaulViewStyle.GridStyle);

        /// <summary>
        /// Gets the Axis Style that specifies display properties for the left axis of a Mass Haul View.
        /// </summary>
        public AxisStyle LeftAxisStyle => new AxisStyle(this, AeccMassHaulViewStyle.LeftAxis);

        /// <summary>
        /// Gets the Axis Style that specifies display properties for the left axis of a Mass Haul View.
        /// </summary>
        public AxisStyle MiddleAxisStyle => new AxisStyle(this, AeccMassHaulViewStyle.MiddleAxis);

        /// <summary>
        /// Gets the Axis Style that specifies display properties for the right axis of a Mass Haul View.
        /// </summary>
        public AxisStyle RightAxisStyle => new AxisStyle(this, AeccMassHaulViewStyle.RightAxis);

        /// <summary>
        /// Gets the Axis Style that specifies display properties for the top axis of a Mass Haul View.
        /// </summary>
        public AxisStyle TopAxisStyle => new AxisStyle(this, AeccMassHaulViewStyle.TopAxis);
        #endregion

        #region constructors
        internal MassHaulViewStyle(AeccMassHaulViewStyle aeccMassHaulViewStyle, bool isDynamoOwned = false) : base(aeccMassHaulViewStyle, isDynamoOwned) { }

        internal static MassHaulViewStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<MassHaulViewStyle, AeccMassHaulViewStyle>
            (styleId, (style) => new MassHaulViewStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"MassHaulViewStyle(Name = {Name})";
        #endregion
    }
}
