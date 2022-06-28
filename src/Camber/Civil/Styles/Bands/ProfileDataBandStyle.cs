#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccProfileDataBandStyle = Autodesk.Civil.DatabaseServices.Styles.ProfileDataBandStyle;
#endregion

namespace Camber.Civil.Styles.Bands
{
    public sealed class ProfileDataBandStyle : BandStyle
    {
        #region properties
        internal AeccProfileDataBandStyle AeccProfileDataBandStyle => AcObject as AeccProfileDataBandStyle;
        #endregion

        #region constructors
        internal ProfileDataBandStyle(AeccProfileDataBandStyle aeccPipeNetworkBandStyle, bool isDynamoOwned = false) 
            : base(aeccPipeNetworkBandStyle, isDynamoOwned) { }

        internal static ProfileDataBandStyle GetByObjectId(acDb.ObjectId profileDataBandStyleId)
            => StyleSupport.Get<ProfileDataBandStyle, AeccProfileDataBandStyle>
            (profileDataBandStyleId, (profileDataBandStyle) => new ProfileDataBandStyle(profileDataBandStyle));
        #endregion

        #region methods
        public override string ToString() => $"ProfileDataBandStyle(Name = {Name})";
        #endregion
    }
}
