#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.Profile
{
    public sealed class ProfileGradeBreakLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal ProfileGradeBreakLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static ProfileGradeBreakLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<ProfileGradeBreakLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new ProfileGradeBreakLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Profile Grade Break Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ProfileGradeBreakLabelStyle ByName(string name)
        {
            return (ProfileGradeBreakLabelStyle)CreateByNameType(
                name, 
                LabelStyleCollections.ProfileLabelStyles.ToString() + "." + ProfileLabelStyles.GradeBreakLabelStyles.ToString(), 
                typeof(ProfileGradeBreakLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"ProfileGradeBreakLabelStyle(Name = {Name})";
        #endregion
    }
}
