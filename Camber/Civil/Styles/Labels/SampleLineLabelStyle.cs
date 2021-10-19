#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels
{
    public sealed class SampleLineLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal SampleLineLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static SampleLineLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<SampleLineLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new SampleLineLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Sample Line Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SampleLineLabelStyle ByName(string name)
        {
            return (SampleLineLabelStyle)CreateByNameType(
                name, 
                "SampleLineLabelStyles.LabelStyles", 
                typeof(SampleLineLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"SampleLineLabelStyle(Name = {Name})";
        #endregion
    }
}
