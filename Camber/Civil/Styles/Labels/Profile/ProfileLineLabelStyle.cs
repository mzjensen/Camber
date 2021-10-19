#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.Profile
{
    public sealed class ProfileLineLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal ProfileLineLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static ProfileLineLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<ProfileLineLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new ProfileLineLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Profile Line Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ProfileLineLabelStyle ByName(string name)
        {
            return (ProfileLineLabelStyle)CreateByNameType(
                name, 
                LabelStyleCollections.ProfileLabelStyles.ToString() + "." + ProfileLabelStyles.LineLabelStyles.ToString(), 
                typeof(ProfileLineLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"ProfileLineLabelStyle(Name = {Name})";
        #endregion
    }
}
