#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccSectionStyle = Autodesk.Civil.DatabaseServices.Styles.SectionStyle;
#endregion

namespace Camber.Civil.Styles.Objects
{
    public sealed class SectionStyle : Style
    {
        #region properties
        internal AeccSectionStyle AeccSectionStyle => AcObject as AeccSectionStyle;
        #endregion

        #region constructors
        internal SectionStyle(AeccSectionStyle aeccSectionStyle, bool isDynamoOwned = false) : base(aeccSectionStyle, isDynamoOwned) { }

        internal static SectionStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<SectionStyle, AeccSectionStyle>
            (styleId, (style) => new SectionStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"SectionStyle(Name = {Name})";
        #endregion
    }
}
