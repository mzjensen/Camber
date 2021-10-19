#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.Section
{
    public sealed class SectionMinorOffsetLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal SectionMinorOffsetLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static SectionMinorOffsetLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<SectionMinorOffsetLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new SectionMinorOffsetLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Section Minor Offset Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SectionMinorOffsetLabelStyle ByName(string name)
        {
            return (SectionMinorOffsetLabelStyle)CreateByNameType(
                name, 
                LabelStyleCollections.SectionLabelStyles.ToString() + "." + SectionLabelStyles.MinorOffsetLabelStyles.ToString(), 
                typeof(SectionMinorOffsetLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"SectionMinorOffsetLabelStyle(Name = {Name})";
        #endregion
    }
}
