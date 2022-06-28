#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.Surface
{
    public sealed class SurfaceContourLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal SurfaceContourLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static SurfaceContourLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<SurfaceContourLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new SurfaceContourLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Surface Contour Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SurfaceContourLabelStyle ByName(string name)
        {
            return (SurfaceContourLabelStyle)CreateByNameType( 
                name, 
                LabelStyleCollections.SurfaceLabelStyles.ToString() + "." + SurfaceLabelStyles.ContourLabelStyles.ToString(), 
                typeof(SurfaceContourLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"SurfaceContourLabelStyle(Name = {Name})";
        #endregion
    }
}
