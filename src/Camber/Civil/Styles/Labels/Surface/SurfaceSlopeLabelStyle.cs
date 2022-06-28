#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.Surface
{
    public sealed class SurfaceSlopeLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal SurfaceSlopeLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static SurfaceSlopeLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<SurfaceSlopeLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new SurfaceSlopeLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Surface Slope Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SurfaceSlopeLabelStyle ByName(string name)
        {
            return (SurfaceSlopeLabelStyle)CreateByNameType( 
                name, 
                LabelStyleCollections.SurfaceLabelStyles.ToString() + "." + SurfaceLabelStyles.SlopeLabelStyles.ToString(), 
                typeof(SurfaceSlopeLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"SurfaceSlopeLabelStyle(Name = {Name})";
        #endregion
    }
}
