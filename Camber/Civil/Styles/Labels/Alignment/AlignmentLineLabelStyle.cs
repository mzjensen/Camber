#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.Alignment
{
    public sealed class AlignmentLineLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal AlignmentLineLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static AlignmentLineLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<AlignmentLineLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new AlignmentLineLabelStyle(labelStyle));

        /// <summary>
        /// Creates an Alignment Line Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static AlignmentLineLabelStyle ByName(string name)
        {
            return (AlignmentLineLabelStyle)CreateByNameType(
                name,
                LabelStyleCollections.AlignmentLabelStyles.ToString() + "." + AlignmentLabelStyles.LineLabelStyles.ToString(),
                typeof(AlignmentLineLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"AlignmentLineLabelStyle(Name = {Name})";
        #endregion
    }
}
