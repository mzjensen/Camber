#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.Projection
{
    public sealed class ProfileViewProjectionLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal ProfileViewProjectionLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static ProfileViewProjectionLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<ProfileViewProjectionLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new ProfileViewProjectionLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Profile View Projection Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ProfileViewProjectionLabelStyle ByName(string name)
        {
            return (ProfileViewProjectionLabelStyle)CreateByNameType(
                name, 
                LabelStyleCollections.ProjectionLabelStyles.ToString() + "." + ProjectionLabelStyles.ProfileViewProjectionLabelStyles.ToString(), 
                typeof(ProfileViewProjectionLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"ProfileViewProjectionLabelStyle(Name = {Name})";
        #endregion
    }
}
