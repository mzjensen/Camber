#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.MatchLine
{
    public sealed class MatchLineRightLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal MatchLineRightLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static MatchLineRightLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<MatchLineRightLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new MatchLineRightLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Match Line Right Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static MatchLineRightLabelStyle ByName(string name)
        {
            return (MatchLineRightLabelStyle)CreateByNameType(
                name,
                LabelStyleCollections.MatchLineLabelStyles.ToString() + "." + MatchLineLabelStyles.RightLabelStyles.ToString(),
                typeof(MatchLineRightLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"MatchLineLeftLabelStyle(Name = {Name})";
        #endregion
    }
}
