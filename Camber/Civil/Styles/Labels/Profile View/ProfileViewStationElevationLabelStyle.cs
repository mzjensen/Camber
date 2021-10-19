#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.ProfileView
{
    public sealed class ProfileViewStationElevationLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal ProfileViewStationElevationLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static ProfileViewStationElevationLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<ProfileViewStationElevationLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new ProfileViewStationElevationLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Profile View Station Elevation Label Style by name.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ProfileViewStationElevationLabelStyle ByName(string name)
        {
            return (ProfileViewStationElevationLabelStyle)CreateByNameType(
                name, 
                LabelStyleCollections.ProfileViewLabelStyles.ToString() + "." + ProfileViewLabelStyles.StationElevationLabelStyles.ToString(), 
                typeof(ProfileViewStationElevationLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"ProfileViewStationElevationLabelStyle(Name = {Name})";
        #endregion
    }
}
