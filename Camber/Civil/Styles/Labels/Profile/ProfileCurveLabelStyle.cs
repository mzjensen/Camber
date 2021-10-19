#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.Profile
{
    public sealed class ProfileCurveLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal ProfileCurveLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static ProfileCurveLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<ProfileCurveLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new ProfileCurveLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Profile Curve Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ProfileCurveLabelStyle ByName(string name)
        {
            return (ProfileCurveLabelStyle)CreateByNameType(
                name, 
                LabelStyleCollections.ProfileLabelStyles.ToString() + "." + ProfileLabelStyles.CurveLabelStyles.ToString(), 
                typeof(ProfileCurveLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"ProfileCurveLabelStyle(Name = {Name})";
        #endregion
    }
}
