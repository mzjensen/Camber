#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.Section
{
    public sealed class SectionSegmentLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal SectionSegmentLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static SectionSegmentLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<SectionSegmentLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new SectionSegmentLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Section Segment Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SectionSegmentLabelStyle ByName(string name)
        {
            return (SectionSegmentLabelStyle)CreateByNameType(
                name, 
                LabelStyleCollections.SectionLabelStyles.ToString() + "." + SectionLabelStyles.SegmentLabelStyles.ToString(), 
                typeof(SectionSegmentLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"SectionSegmentLabelStyle(Name = {Name})";
        #endregion
    }
}
