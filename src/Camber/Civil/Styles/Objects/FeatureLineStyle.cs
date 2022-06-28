#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccFeatureLineStyle = Autodesk.Civil.DatabaseServices.Styles.FeatureLineStyle;
#endregion

namespace Camber.Civil.Styles.Objects
{
    public sealed class FeatureLineStyle : Style
    {
        #region properties
        internal AeccFeatureLineStyle AeccFeatureLineStyle => AcObject as AeccFeatureLineStyle;

        /// <summary>
        /// Gets the beginning vertex Marker Style of a Feature Line Style.
        /// </summary>
        public MarkerStyle BeginningVertexMarkerStyle => GetMarkerStyleByType("ProfileBeginningVertexMarkerStyleId");

        /// <summary>
        /// Gets the ending vertex Marker Style of a Feature Line Style.
        /// </summary>
        public MarkerStyle EndingVertexMarkerStyle => GetMarkerStyleByType("ProfileEndingVertexMarkerStyleId");

        /// <summary>
        /// Gets the internal vertex Marker Style of a Feature Line Style.
        /// </summary>
        public MarkerStyle InternalVertexMarkerStyle => GetMarkerStyleByType("ProfileInternalVertexMarkerStyleId");

        /// <summary>
        /// Gets the crossing Marker Style of a Feature Line Style.
        /// </summary>
        public MarkerStyle CrossingMarkerStyle => GetMarkerStyleByType("SectionMarkerStyleId");
        #endregion

        #region constructors
        internal FeatureLineStyle(AeccFeatureLineStyle aeccFeatureLineStyle, bool isDynamoOwned = false) : base(aeccFeatureLineStyle, isDynamoOwned) { }

        internal static FeatureLineStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<FeatureLineStyle, AeccFeatureLineStyle>
            (styleId, (style) => new FeatureLineStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"FeatureLineStyle(Name = {Name})";

        /// <summary>
        /// Sets the beginning vertex Marker Style of a Feature Line Style.
        /// </summary>
        /// <param name="markerStyle"></param>
        /// <returns></returns>
        public FeatureLineStyle SetBeginningVertexMarkerStyle(MarkerStyle markerStyle) => SetMarkerStyleByType("ProfileBeginningVertexMarkerStyleId", markerStyle);


        /// <summary>
        /// Sets the ending vertex Marker Style of a Feature Line Style.
        /// </summary>
        /// <param name="markerStyle"></param>
        /// <returns></returns>
        public FeatureLineStyle SetEndingVertexMarkerStyle(MarkerStyle markerStyle) => SetMarkerStyleByType("ProfileEndingVertexMarkerStyleId", markerStyle);


        /// <summary>
        /// Sets the internal vertex Marker Style of a Feature Line Style.
        /// </summary>
        /// <param name="markerStyle"></param>
        /// <returns></returns>
        public FeatureLineStyle SetInternalVertexMarkerStyle(MarkerStyle markerStyle) => SetMarkerStyleByType("ProfileInternalVertexMarkerStyleId", markerStyle);


        /// <summary>
        /// Sets the crossing Marker Style of a Feature Line Style.
        /// </summary>
        /// <param name="markerStyle"></param>
        /// <returns></returns>
        public FeatureLineStyle SetCrossingMarkerStyle(MarkerStyle markerStyle) => SetMarkerStyleByType("SectionMarkerStyleId", markerStyle);



        /// <summary>
        /// Gets a Marker Style by type within the Feature Line Style definition.
        /// </summary>
        /// <param name="markerStyleType"></param>
        /// <returns></returns>
        protected MarkerStyle GetMarkerStyleByType(string markerStyleType)
        {
            try
            {
                var markerStyleId = (acDb.ObjectId)AeccFeatureLineStyle.GetType().GetProperty(markerStyleType).GetValue(AeccFeatureLineStyle, null);
                if (markerStyleId.IsValid && !markerStyleId.IsErased)
                {
                    return MarkerStyle.GetByObjectId(markerStyleId);
                }
                return null;
            }
            catch { throw; }
        }

        /// <summary>
        /// Sets a Marker Style by type within the Feature Line Style definition.
        /// </summary>
        /// <param name="markerStyleType">The type of Marker Style to set</param>
        /// <param name="markerStyle">The new Marker Style</param>
        /// <returns></returns>
        protected FeatureLineStyle SetMarkerStyleByType(string markerStyleType, MarkerStyle markerStyle)
        {
            try
            {
                var markerStyleId = markerStyle.AeccMarkerStyle.ObjectId;
                if (markerStyleId.IsValid && !markerStyleId.IsErased)
                {
                    AeccFeatureLineStyle.GetType().GetProperty(markerStyleType).SetValue(AeccFeatureLineStyle, markerStyleId, null);
                    return this;
                }
                return null;
            }
            catch { throw; }
        }
        #endregion
    }
}
