#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccProfileViewBandSetStyle = Autodesk.Civil.DatabaseServices.Styles.ProfileViewBandSetStyle;
#endregion

namespace Camber.Civil.Styles.BandSets
{
    public sealed class ProfileViewBandSetStyle : BandSetStyle
    {
        #region properties
        internal AeccProfileViewBandSetStyle AeccProfileViewBandSetStyle => AcObject as AeccProfileViewBandSetStyle;
        #endregion

        #region constructors
        internal ProfileViewBandSetStyle(AeccProfileViewBandSetStyle aeccProfileViewBandSetStyle, bool isDynamoOwned = false) : base(aeccProfileViewBandSetStyle, isDynamoOwned) { }

        internal static ProfileViewBandSetStyle GetByObjectId(acDb.ObjectId profileViewBandSetStyleId)
            => StyleSupport.Get<ProfileViewBandSetStyle, AeccProfileViewBandSetStyle>
            (profileViewBandSetStyleId, (profileViewBandSetStyle) => new ProfileViewBandSetStyle(profileViewBandSetStyle));
        #endregion

        #region methods
        public override string ToString() => $"ProfileViewBandSetStyle(Name = {Name})";
        #endregion
    }
}
