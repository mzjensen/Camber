#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.ProfileView
{
    public sealed class ProfileViewDepthLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal ProfileViewDepthLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static ProfileViewDepthLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<ProfileViewDepthLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new ProfileViewDepthLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Profile View Depth Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ProfileViewDepthLabelStyle ByName(string name)
        {
            return (ProfileViewDepthLabelStyle)CreateByNameType(
                name, 
                LabelStyleCollections.ProfileViewLabelStyles.ToString() + "." + ProfileViewLabelStyles.DepthLabelStyles.ToString(), 
                typeof(ProfileViewDepthLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"ProfileViewDepthLabelStyle(Name = {Name})";
        #endregion
    }
}
