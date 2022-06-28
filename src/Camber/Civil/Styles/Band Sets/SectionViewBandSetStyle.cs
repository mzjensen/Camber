#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccSectionViewBandSetStyle = Autodesk.Civil.DatabaseServices.Styles.SectionViewBandSetStyle;
#endregion

namespace Camber.Civil.Styles.BandSets
{
    public sealed class SectionViewBandSetStyle : BandSetStyle
    {
        #region properties
        internal AeccSectionViewBandSetStyle AeccSectionViewBandSetStyle => AcObject as AeccSectionViewBandSetStyle;
        #endregion

        #region constructors
        internal SectionViewBandSetStyle(AeccSectionViewBandSetStyle aeccSectionViewBandSetStyle, bool isDynamoOwned = false) : base(aeccSectionViewBandSetStyle, isDynamoOwned) { }

        internal static SectionViewBandSetStyle GetByObjectId(acDb.ObjectId sectionViewBandSetStyleId)
            => StyleSupport.Get<SectionViewBandSetStyle, AeccSectionViewBandSetStyle>
            (sectionViewBandSetStyleId, (sectionViewBandSetStyle) => new SectionViewBandSetStyle(sectionViewBandSetStyle));
        #endregion

        #region methods
        public override string ToString() => $"SectionViewBandSetStyle(Name = {Name})";
        #endregion
    }
}
