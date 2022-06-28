#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccIntersectionStyle = Autodesk.Civil.DatabaseServices.Styles.IntersectionStyle;
#endregion

namespace Camber.Civil.Styles.Objects
{
    public sealed class IntersectionStyle : Style
    {
        #region properties
        internal AeccIntersectionStyle AeccIntersectionStyle => AcObject as AeccIntersectionStyle;

        /// <summary>
        /// Gets the Marker Style for an Intersection Style.
        /// </summary>
        public MarkerStyle MarkerStyle
        {
            get
            {
                var markerStyleId = AeccIntersectionStyle.MarkerStyleId;
                if (markerStyleId.IsValid && !markerStyleId.IsErased)
                {
                    return MarkerStyle.GetByObjectId(markerStyleId);
                }
                return null;
            }
        }

        #endregion

        #region constructors
        internal IntersectionStyle(AeccIntersectionStyle aeccIntersectionStyle, bool isDynamoOwned = false) : base(aeccIntersectionStyle, isDynamoOwned) { }

        internal static IntersectionStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<IntersectionStyle, AeccIntersectionStyle>
            (styleId, (style) => new IntersectionStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"IntersectionStyle(Name = {Name})";
        #endregion
    }
}
