#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccCorridorStyle = Autodesk.Civil.DatabaseServices.Styles.CorridorStyle;
#endregion

namespace Camber.Civil.Styles.Objects
{
    public sealed class CorridorStyle : Style
    {
        #region properties
        internal AeccCorridorStyle AeccCorridorStyle => AcObject as AeccCorridorStyle;

        #endregion

        #region constructors
        internal CorridorStyle(AeccCorridorStyle aeccCorridorStyle, bool isDynamoOwned = false) : base(aeccCorridorStyle, isDynamoOwned) { }

        internal static CorridorStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<CorridorStyle, AeccCorridorStyle>
            (styleId, (style) => new CorridorStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"CorridorStyle(Name = {Name})";
        #endregion
    }
}
