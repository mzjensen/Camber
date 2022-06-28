#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.Surface
{
    public sealed class SurfaceElevationLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal SurfaceElevationLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static SurfaceElevationLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<SurfaceElevationLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new SurfaceElevationLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Surface Elevation Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SurfaceElevationLabelStyle ByName(string name)
        {
            return (SurfaceElevationLabelStyle)CreateByNameType(
                name, 
                LabelStyleCollections.SurfaceLabelStyles.ToString() + "." + SurfaceLabelStyles.SpotElevationLabelStyles.ToString(), 
                typeof(SurfaceElevationLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"SurfaceElevationLabelStyle(Name = {Name})";
        #endregion
    }
}
