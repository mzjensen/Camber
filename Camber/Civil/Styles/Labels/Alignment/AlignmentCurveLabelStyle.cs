#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.Alignment
{
    public sealed class AlignmentCurveLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal AlignmentCurveLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static AlignmentCurveLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<AlignmentCurveLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new AlignmentCurveLabelStyle(labelStyle));

        /// <summary>
        /// Creates an Alignment Curve Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static AlignmentCurveLabelStyle ByName(string name)
        {
            return (AlignmentCurveLabelStyle)CreateByNameType(
                name,
                LabelStyleCollections.AlignmentLabelStyles.ToString() + "." + AlignmentLabelStyles.CurveLabelStyles.ToString(),
                typeof(AlignmentCurveLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"AlignmentCurveLabelStyle(Name = {Name})";
        #endregion
    }
}
