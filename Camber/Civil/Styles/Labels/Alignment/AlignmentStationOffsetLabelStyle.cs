#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.Alignment
{
    public sealed class AlignmentStationOffsetLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal AlignmentStationOffsetLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static AlignmentStationOffsetLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<AlignmentStationOffsetLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new AlignmentStationOffsetLabelStyle(labelStyle));

        /// <summary>
        /// Creates an Alignment Station Offset Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static AlignmentStationOffsetLabelStyle ByName(string name)
        {
            return (AlignmentStationOffsetLabelStyle)CreateByNameType(
                name,
                LabelStyleCollections.AlignmentLabelStyles.ToString() + "." + AlignmentLabelStyles.StationOffsetLabelStyles.ToString(),
                typeof(AlignmentStationOffsetLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"AlignmentStationOffsetLabelStyle(Name = {Name})";
        #endregion
    }
}
