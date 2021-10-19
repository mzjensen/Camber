#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.Section
{
    public sealed class SectionGradeBreakLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal SectionGradeBreakLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static SectionGradeBreakLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<SectionGradeBreakLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new SectionGradeBreakLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Section Grade Break Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SectionGradeBreakLabelStyle ByName(string name)
        {
            return (SectionGradeBreakLabelStyle)CreateByNameType(
                name, 
                LabelStyleCollections.SectionLabelStyles.ToString() + "." + SectionLabelStyles.GradeBreakLabelStyles.ToString(), 
                typeof(SectionGradeBreakLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"SectionGradeBreakLabelStyle(Name = {Name})";
        #endregion
    }
}
