#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccViewFrameStyle = Autodesk.Civil.DatabaseServices.Styles.ViewFrameStyle;
#endregion

namespace Camber.Civil.Styles.Objects
{
    public sealed class ViewFrameStyle : Style
    {
        #region properties
        internal AeccViewFrameStyle AeccViewFrameStyle => AcObject as AeccViewFrameStyle;

        #endregion

        #region constructors
        internal ViewFrameStyle(AeccViewFrameStyle aeccViewFrameStyle, bool isDynamoOwned = false) : base(aeccViewFrameStyle, isDynamoOwned) { }

        internal static ViewFrameStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<ViewFrameStyle, AeccViewFrameStyle>
            (styleId, (style) => new ViewFrameStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"ViewFrameStyle(Name = {Name})";
        #endregion
    }
}
