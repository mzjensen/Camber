#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.Surface
{
    public sealed class SurfaceWatershedLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal SurfaceWatershedLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static SurfaceWatershedLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<SurfaceWatershedLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new SurfaceWatershedLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Surface Watershed Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SurfaceWatershedLabelStyle ByName(string name)
        {
            return (SurfaceWatershedLabelStyle)CreateByNameType( 
                name, 
                LabelStyleCollections.SurfaceLabelStyles.ToString() + "." + SurfaceLabelStyles.WatershedLabelStyles.ToString(), 
                typeof(SurfaceWatershedLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"SurfaceWatershedLabelStyle(Name = {Name})";
        #endregion
    }
}
