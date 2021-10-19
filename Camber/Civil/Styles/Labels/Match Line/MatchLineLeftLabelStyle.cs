#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.MatchLine
{
    public sealed class MatchLineLeftLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal MatchLineLeftLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static MatchLineLeftLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<MatchLineLeftLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new MatchLineLeftLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Match Line Left Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static MatchLineLeftLabelStyle ByName(string name)
        {
            return (MatchLineLeftLabelStyle)CreateByNameType(
                name,
                LabelStyleCollections.MatchLineLabelStyles.ToString() + "." + MatchLineLabelStyles.LeftLabelStyles.ToString(),
                typeof(MatchLineLeftLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"MatchLineLeftLabelStyle(Name = {Name})";
        #endregion
    }
}
