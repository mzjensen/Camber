#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccSectionDataBandStyle = Autodesk.Civil.DatabaseServices.Styles.SectionDataBandStyle;
#endregion

namespace Camber.Civil.Styles.Bands
{
    public sealed class SectionDataBandStyle : BandStyle
    {
        #region properties
        internal AeccSectionDataBandStyle AeccSectionDataBandStyle => AcObject as AeccSectionDataBandStyle;
        #endregion

        #region constructors
        internal SectionDataBandStyle(AeccSectionDataBandStyle aeccSectionDataBandStyle, bool isDynamoOwned = false) 
            : base(aeccSectionDataBandStyle, isDynamoOwned) { }

        internal static SectionDataBandStyle GetByObjectId(acDb.ObjectId sectionDataBandStyleId)
            => StyleSupport.Get<SectionDataBandStyle, AeccSectionDataBandStyle>
            (sectionDataBandStyleId, (sectionDataBandStyle) => new SectionDataBandStyle(sectionDataBandStyle));
        #endregion

        #region methods
        public override string ToString() => $"SectionDataBandStyle(Name = {Name})";
        #endregion
    }
}
