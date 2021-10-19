#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.Profile
{
    public sealed class ProfileHorizontalGeometryPointLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal ProfileHorizontalGeometryPointLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static ProfileHorizontalGeometryPointLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<ProfileHorizontalGeometryPointLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new ProfileHorizontalGeometryPointLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Profile Horizontal Geometry Point Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ProfileHorizontalGeometryPointLabelStyle ByName(string name)
        {
            return (ProfileHorizontalGeometryPointLabelStyle)CreateByNameType(
                name, 
                LabelStyleCollections.ProfileLabelStyles.ToString() + "." + ProfileLabelStyles.HorizontalGeometryPointLabelStyles.ToString(), 
                typeof(ProfileHorizontalGeometryPointLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"ProfileHorizontalGeometryPointLabelStyle(Name = {Name})";
        #endregion
    }
}
