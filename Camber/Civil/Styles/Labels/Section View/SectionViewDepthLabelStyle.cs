#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.SectionView
{
    public sealed class SectionViewDepthLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal SectionViewDepthLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static SectionViewDepthLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<SectionViewDepthLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new SectionViewDepthLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Section View Depth Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SectionViewDepthLabelStyle ByName(string name)
        {
            return (SectionViewDepthLabelStyle)CreateByNameType(
                name, 
                LabelStyleCollections.SectionViewLabelStyles.ToString() + "." + SectionViewLabelStyles.GradeLabelStyles.ToString(), 
                typeof(SectionViewDepthLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"SectionViewDepthLabelStyle(Name = {Name})";
        #endregion
    }
}
