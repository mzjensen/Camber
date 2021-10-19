#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLinkStyle = Autodesk.Civil.DatabaseServices.Styles.LinkStyle;
#endregion

namespace Camber.Civil.Styles.Objects
{
    public sealed class LinkStyle : Style
    {
        #region properties
        internal AeccLinkStyle AeccLinkStyle => AcObject as AeccLinkStyle;
        #endregion

        #region constructors
        internal LinkStyle(AeccLinkStyle aeccLinkStyle, bool isDynamoOwned = false) : base(aeccLinkStyle, isDynamoOwned) { }

        internal static LinkStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<LinkStyle, AeccLinkStyle>
            (styleId, (style) => new LinkStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"LinkStyle(Name = {Name})";
        #endregion
    }
}
