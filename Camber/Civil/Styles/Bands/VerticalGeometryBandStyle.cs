#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccVerticalGeometryBandStyle = Autodesk.Civil.DatabaseServices.Styles.VerticalGeometryBandStyle;
#endregion

namespace Camber.Civil.Styles.Bands
{
    public sealed class VerticalGeometryBandStyle : BandStyle
    {
        #region properties
        internal AeccVerticalGeometryBandStyle AeccVerticalGeometryBandStyle => AcObject as AeccVerticalGeometryBandStyle;
        #endregion

        #region constructors
        internal VerticalGeometryBandStyle(AeccVerticalGeometryBandStyle aeccVerticalGeometryBandStyle, bool isDynamoOwned = false) 
            : base(aeccVerticalGeometryBandStyle, isDynamoOwned) { }

        internal static VerticalGeometryBandStyle GetByObjectId(acDb.ObjectId verticalGeometryBandStyleId)
            => StyleSupport.Get<VerticalGeometryBandStyle, AeccVerticalGeometryBandStyle>
            (verticalGeometryBandStyleId, (verticalGeometryBandStyle) => new VerticalGeometryBandStyle(verticalGeometryBandStyle));
        #endregion

        #region methods
        public override string ToString() => $"VerticalGeometryBandStyle(Name = {Name})";
        #endregion
    }
}
