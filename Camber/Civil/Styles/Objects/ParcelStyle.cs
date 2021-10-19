#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccParcelStyle = Autodesk.Civil.DatabaseServices.Styles.ParcelStyle;
#endregion

namespace Camber.Civil.Styles.Objects
{
    public sealed class ParcelStyle : Style
    {
        #region properties
        internal AeccParcelStyle AeccParcelStyle => AcObject as AeccParcelStyle;

        /// <summary>
        /// Gets the parcel name template string for a Parcel Style.
        /// </summary>
        public string NameTemplate => GetString();

        /// <summary>
        /// Gets the boolean value that enables the use of the width of boundary area parcel setting for a Parcel Style.
        /// </summary>
        public bool ObservePatternFillDistance => GetBool();

        /// <summary>
        /// Gets the Marker Style for Parcel Segments designated in a Parcel Style.
        /// </summary>
        public MarkerStyle ParcelSegmentsMarkerStyle
        {
            get
            {
                var markerStyleId = AeccParcelStyle.ParcelSegmentsMarkerStyle;
                if (markerStyleId.IsValid && !markerStyleId.IsErased)
                {
                    return MarkerStyle.GetByObjectId(markerStyleId);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the width of the boundary area parcel setting for a Parcel Style, which is the offset distance from the parcel boundary.
        /// </summary>
        public double PatternFillDistance => GetDouble();
        #endregion

        #region constructors
        internal ParcelStyle(AeccParcelStyle aeccParcelStyle, bool isDynamoOwned = false) : base(aeccParcelStyle, isDynamoOwned) { }

        internal static ParcelStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<ParcelStyle, AeccParcelStyle>
            (styleId, (style) => new ParcelStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"ParcelStyle(Name = {Name})";

        /// <summary>
        /// Sets the parcel name template string for a Parcel Style.
        /// </summary>
        /// <param name="templateString"></param>
        /// <returns></returns>
        public ParcelStyle SetNameTemplate(string templateString) => (ParcelStyle)SetValue(templateString);

        /// <summary>
        /// Sets the boolean value that enables the use of the width of boundary area parcel setting for a Parcel Style.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public ParcelStyle SetObservePatternFillDistance(bool @bool) => (ParcelStyle)SetValue(@bool);

        /// <summary>
        /// Sets the Marker Style for Parcel Segments designated in a Parcel Style.
        /// </summary>
        /// <param name="markerStyle"></param>
        /// <returns></returns>
        public ParcelStyle SetParcelSegmentsMarkerStyle(MarkerStyle markerStyle) => (ParcelStyle)SetValue(markerStyle.AeccMarkerStyle.ObjectId);

        /// <summary>
        /// Sets the width of the boundary area parcel setting for a Parcel Style, which is the offset distance from the parcel boundary.
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public ParcelStyle SetPatternFillDistance(double distance) => (ParcelStyle)SetValue(distance);
        #endregion
    }
}