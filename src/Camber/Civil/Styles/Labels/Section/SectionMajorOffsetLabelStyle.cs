#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.Section
{
    public sealed class SectionMajorOffsetLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal SectionMajorOffsetLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static SectionMajorOffsetLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<SectionMajorOffsetLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new SectionMajorOffsetLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Section Major Offset Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SectionMajorOffsetLabelStyle ByName(string name)
        {
            return (SectionMajorOffsetLabelStyle)CreateByNameType(
                name, 
                LabelStyleCollections.SectionLabelStyles.ToString() + "." + SectionLabelStyles.MajorOffsetLabelStyles.ToString(), 
                typeof(SectionMajorOffsetLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"SectionMajorOffsetLabelStyle(Name = {Name})";
        #endregion
    }
}
