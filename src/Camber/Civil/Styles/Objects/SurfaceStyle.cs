#region references
using Autodesk.DesignScript.Runtime;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccSurfaceStyle = Autodesk.Civil.DatabaseServices.Styles.SurfaceStyle;
#endregion

namespace Camber.Civil.Styles.Objects
{
    public sealed class SurfaceStyle : Style
    {
        #region properties
        internal AeccSurfaceStyle AeccSurfaceStyle => AcObject as AeccSurfaceStyle;

        [IsVisibleInDynamoLibrary(false)]
        public string DummyProperty => "DummyProperty";
        #endregion

        #region constructors
        internal SurfaceStyle(AeccSurfaceStyle aeccSurfaceStyle, bool isDynamoOwned = false) : base(aeccSurfaceStyle, isDynamoOwned) { }

        internal static SurfaceStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<SurfaceStyle, AeccSurfaceStyle>
            (styleId, (style) => new SurfaceStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"SurfaceStyle(Name = {Name})";
        #endregion
    }
}
