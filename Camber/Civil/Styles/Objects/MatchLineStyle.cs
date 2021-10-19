#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccMatchLineStyle = Autodesk.Civil.DatabaseServices.Styles.MatchLineStyle;
#endregion

namespace Camber.Civil.Styles.Objects
{
    public sealed class MatchLineStyle : Style
    {
        #region properties
        internal AeccMatchLineStyle AeccMatchLineStyle => AcObject as AeccMatchLineStyle;
        #endregion

        #region constructors
        internal MatchLineStyle(AeccMatchLineStyle aeccMatchLineStyle, bool isDynamoOwned = false) : base(aeccMatchLineStyle, isDynamoOwned) { }

        internal static MatchLineStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<MatchLineStyle, AeccMatchLineStyle>
            (styleId, (style) => new MatchLineStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"MatchLineStyle(Name = {Name})";
        #endregion
    }
}
