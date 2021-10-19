#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.Profile
{
    public sealed class ProfileMajorStationLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal ProfileMajorStationLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static ProfileMajorStationLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<ProfileMajorStationLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new ProfileMajorStationLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Profile Major Station Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ProfileMajorStationLabelStyle ByName(string name)
        {
            return (ProfileMajorStationLabelStyle)CreateByNameType(
                name, 
                LabelStyleCollections.ProfileLabelStyles.ToString() + "." + ProfileLabelStyles.MajorStationLabelStyles.ToString(), 
                typeof(ProfileMajorStationLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"ProfileMajorStationLabelStyle(Name = {Name})";
        #endregion
    }
}
