#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccSampleLineStyle = Autodesk.Civil.DatabaseServices.Styles.SampleLineStyle;
#endregion

namespace Camber.Civil.Styles.Objects
{
    public sealed class SampleLineStyle : Style
    {
        #region properties
        internal AeccSampleLineStyle AeccSampleLineStyle => AcObject as AeccSampleLineStyle;
        #endregion

        #region constructors
        internal SampleLineStyle(AeccSampleLineStyle aeccSampleLineStyle, bool isDynamoOwned = false) : base(aeccSampleLineStyle, isDynamoOwned) { }

        internal static SampleLineStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<SampleLineStyle, AeccSampleLineStyle>
            (styleId, (style) => new SampleLineStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"SampleLineStyle(Name = {Name})";
        #endregion
    }
}
