#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccHorizontalGeometryBandStyle = Autodesk.Civil.DatabaseServices.Styles.HorizontalGeometryBandStyle;
#endregion

namespace Camber.Civil.Styles.Bands
{
    public sealed class HorizontalGeometryBandStyle : BandStyle
    {
        #region properties
        internal AeccHorizontalGeometryBandStyle AeccHorizontalGeometryBandStyle => AcObject as AeccHorizontalGeometryBandStyle;
        #endregion

        #region constructors
        internal HorizontalGeometryBandStyle(AeccHorizontalGeometryBandStyle aeccHorizontalGeometryBandStyle, bool isDynamoOwned = false) 
            : base(aeccHorizontalGeometryBandStyle, isDynamoOwned) { }

        internal static HorizontalGeometryBandStyle GetByObjectId(acDb.ObjectId horizontalGeometryBandStyleId)
            => StyleSupport.Get<HorizontalGeometryBandStyle, AeccHorizontalGeometryBandStyle>
            (horizontalGeometryBandStyleId, (horizontalGeometryBandStyle) => new HorizontalGeometryBandStyle(horizontalGeometryBandStyle));
        #endregion

        #region methods
        public override string ToString() => $"HorizontalGeometryBandStyle(Name = {Name})";
        #endregion
    }
}
