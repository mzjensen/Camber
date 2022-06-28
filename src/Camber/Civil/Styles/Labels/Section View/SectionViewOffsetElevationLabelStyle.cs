#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.SectionView
{
    public sealed class SectionViewOffsetElevationLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal SectionViewOffsetElevationLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static SectionViewOffsetElevationLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<SectionViewOffsetElevationLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new SectionViewOffsetElevationLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Section View Offset Elevation Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SectionViewOffsetElevationLabelStyle ByName(string name)
        {
            return (SectionViewOffsetElevationLabelStyle)CreateByNameType(
                name, 
                LabelStyleCollections.SectionViewLabelStyles.ToString() + "." + SectionViewLabelStyles.OffsetElevationLabelStyles.ToString(), 
                typeof(SectionViewOffsetElevationLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"SectionViewOffsetElevationLabelStyle(Name = {Name})";
        #endregion
    }
}
