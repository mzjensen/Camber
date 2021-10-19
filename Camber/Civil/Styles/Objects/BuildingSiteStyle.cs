#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccBuildingSiteStyle = Autodesk.Civil.DatabaseServices.Styles.BuildingSiteStyle;
#endregion

namespace Camber.Civil.Styles.Objects
{
    public sealed class BuildingSiteStyle : Style
    {
        #region properties
        internal AeccBuildingSiteStyle AeccBuildingSiteStyle => AcObject as AeccBuildingSiteStyle;

        /// <summary>
        /// Gets the utility connection Marker Style of a Building Site Style.
        /// </summary>
        public MarkerStyle UtilityConnectionMarkerStyle => MarkerStyle.GetByObjectId(AeccBuildingSiteStyle.MarkerStyleId);
        #endregion

        #region constructors
        internal BuildingSiteStyle(AeccBuildingSiteStyle aeccBuildingSiteStyle, bool isDynamoOwned = false) : base(aeccBuildingSiteStyle, isDynamoOwned) { }

        internal static BuildingSiteStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<BuildingSiteStyle, AeccBuildingSiteStyle>
            (styleId, (style) => new BuildingSiteStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"BuildingSiteStyle(Name = {Name})";

        /// <summary>
        /// Sets the utility connection Marker Style of a Building Site Style.
        /// </summary>
        /// <param name="markerStyle"></param>
        /// <returns></returns>
        public BuildingSiteStyle SetUtilityConnectionMarkerStyle(MarkerStyle markerStyle) => (BuildingSiteStyle)SetValue(markerStyle.AeccMarkerStyle.ObjectId, "MarkerStyleId");
        #endregion
    }
}
