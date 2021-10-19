#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccSuperelevationDataBandStyle = Autodesk.Civil.DatabaseServices.Styles.SuperelevationDataBandStyle;
#endregion

namespace Camber.Civil.Styles.Bands
{
    public sealed class SuperelevationDataBandStyle : BandStyle
    {
        #region properties
        internal AeccSuperelevationDataBandStyle AeccSuperelevationDataBandStyle => AcObject as AeccSuperelevationDataBandStyle;
        #endregion

        #region constructors
        internal SuperelevationDataBandStyle(AeccSuperelevationDataBandStyle aeccSuperelevationDataBandStyle, bool isDynamoOwned = false) 
            : base(aeccSuperelevationDataBandStyle, isDynamoOwned) { }

        internal static SuperelevationDataBandStyle GetByObjectId(acDb.ObjectId superelevationDataBandStyleId)
            => StyleSupport.Get<SuperelevationDataBandStyle, AeccSuperelevationDataBandStyle>
            (superelevationDataBandStyleId, (superelevationDataBandStyle) => new SuperelevationDataBandStyle(superelevationDataBandStyle));
        #endregion

        #region methods
        public override string ToString() => $"SuperelevationDataBandStyle(Name = {Name})";
        #endregion
    }
}
