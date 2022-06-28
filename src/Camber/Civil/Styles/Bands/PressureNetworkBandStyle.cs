#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccPressureNetworkBandStyle = Autodesk.Civil.DatabaseServices.Styles.PressureNetworkBandStyle;
#endregion

namespace Camber.Civil.Styles.Bands
{
    public sealed class PressureNetworkBandStyle : BandStyle
    {
        #region properties
        internal AeccPressureNetworkBandStyle AeccPressureNetworkBandStyle => AcObject as AeccPressureNetworkBandStyle;
        #endregion

        #region constructors
        internal PressureNetworkBandStyle(AeccPressureNetworkBandStyle aeccPressureNetworkBandStyle, bool isDynamoOwned = false) 
            : base(aeccPressureNetworkBandStyle, isDynamoOwned) { }

        internal static PressureNetworkBandStyle GetByObjectId(acDb.ObjectId pressureNetworkBandStyleId)
            => StyleSupport.Get<PressureNetworkBandStyle, AeccPressureNetworkBandStyle>
            (pressureNetworkBandStyleId, (pressureNetworkBandStyle) => new PressureNetworkBandStyle(pressureNetworkBandStyle));
        #endregion

        #region methods
        public override string ToString() => $"PressureNetworkBandStyle(Name = {Name})";
        #endregion
    }
}
