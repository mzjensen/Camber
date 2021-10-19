#region references
using System;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccProfileStyle = Autodesk.Civil.DatabaseServices.Styles.ProfileStyle;
#endregion

namespace Camber.Civil.Styles.Objects
{
    public sealed class ProfileStyle : Style
    {
        #region properties
        internal AeccProfileStyle AeccProfileStyle => AcObject as AeccProfileStyle;

        /// <summary>
        /// Gets the curve tessellation distance defined in a Profile Style, which is used for 3D chain visualization of a Profile.
        /// </summary>
        public double ChainTessellationDistance3D => GetDouble();
        #endregion

        #region constructors
        internal ProfileStyle(AeccProfileStyle aeccProfileStyle, bool isDynamoOwned = false) : base(aeccProfileStyle, isDynamoOwned) { }

        internal static ProfileStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<ProfileStyle, AeccProfileStyle>
            (styleId, (style) => new ProfileStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"ProfileStyle(Name = {Name})";

        /// <summary>
        /// Gets a Marker Style by type from a Profile Style definition.
        /// </summary>
        /// <param name="markerStyleType"></param>
        /// <returns></returns>
        public MarkerStyle GetMarkerStyleByType(string markerStyleType)
        {
            string name = markerStyleType.ToString() + "MarkerStyle";
            if (!Enum.IsDefined(typeof(ProfileMarkerStyles), markerStyleType)) { throw new ArgumentException("Invalid marker style type."); }

            try
            {
                var markerStyleId = (acDb.ObjectId)AeccProfileStyle.GetType().GetProperty(name).GetValue(AeccProfileStyle, null);

                // Need to check for null ID in this case because it is possible to set a marker style to '<none>' in the UI.
                if (markerStyleId.IsValid && !markerStyleId.IsErased)
                {
                    return MarkerStyle.GetByObjectId(markerStyleId);
                }
                return null;
            }
            catch { throw; }
        }

        /// <summary>
        /// Sets a Marker Style by type in a Profile Style definition.
        /// </summary>
        /// <param name="markerStyleType">The type of Marker Style to set</param>
        /// <param name="markerStyle">The new Marker Style</param>
        /// <returns></returns>
        public ProfileStyle SetMarkerStyleByType(string markerStyleType, MarkerStyle markerStyle)
        {
            string name = markerStyleType.ToString() + "MarkerStyle";
            if (!Enum.IsDefined(typeof(ProfileMarkerStyles), markerStyleType)) { throw new ArgumentException("Invalid marker style type."); }

            try
            {
                var markerStyleId = markerStyle.InternalObjectId;
                if (markerStyleId.IsValid && !markerStyleId.IsErased)
                {
                    AeccProfileStyle.GetType().GetProperty(name).SetValue(AeccProfileStyle, markerStyleId, null);
                    return this;
                }
                return null;
            }
            catch { throw; }
        }

        /// <summary>
        /// Sets the curve tessellation distance defined in a Profile Style, which is used for 3D chain visualization of a Profile.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ProfileStyle SetChainTessellationDistance3D(double value) => (ProfileStyle)SetValue(value);
        #endregion
    }
}
