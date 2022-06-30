#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccShapeStyle = Autodesk.Civil.DatabaseServices.Styles.ShapeStyle;
#endregion

namespace Camber.Civil.Styles.Objects
{
    public sealed class ShapeStyle : Style
    {
        #region properties
        internal AeccShapeStyle AeccShapeStyle => AcObject as AeccShapeStyle;

        /// <summary>
        /// Dummy public property so that the class gets imported.
        /// </summary>
        public object DummyProperty => null;
        #endregion

        #region constructors
        internal ShapeStyle(AeccShapeStyle aeccShapeStyle, bool isDynamoOwned = false) : base(aeccShapeStyle, isDynamoOwned) { }

        internal static ShapeStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<ShapeStyle, AeccShapeStyle>
            (styleId, (style) => new ShapeStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"ShapeStyle(Name = {Name})";
        #endregion
    }
}
