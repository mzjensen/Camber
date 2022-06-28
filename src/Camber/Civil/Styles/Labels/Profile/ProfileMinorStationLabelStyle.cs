#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.Profile
{
    public sealed class ProfileMinorStationLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal ProfileMinorStationLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static ProfileMinorStationLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<ProfileMinorStationLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new ProfileMinorStationLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Profile Minor Station Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ProfileMinorStationLabelStyle ByName(string name)
        {
            return (ProfileMinorStationLabelStyle)CreateByNameType(
                name, 
                LabelStyleCollections.ProfileLabelStyles.ToString() + "." + ProfileLabelStyles.MinorStationLabelStyles.ToString(), 
                typeof(ProfileMinorStationLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"ProfileMinorStationLabelStyle(Name = {Name})";
        #endregion
    }
}
