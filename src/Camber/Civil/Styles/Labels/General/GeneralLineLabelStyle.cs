#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.General
{
    public sealed class GeneralLineLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal GeneralLineLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static GeneralLineLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<GeneralLineLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new GeneralLineLabelStyle(labelStyle));

        /// <summary>
        /// Creates a General Line Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GeneralLineLabelStyle ByName(string name)
        {
            return (GeneralLineLabelStyle)CreateByNameType(
                name, 
                GeneralLabelStyles.GeneralLineLabelStyles.ToString(),  
                typeof(GeneralLineLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"GeneralLineLabelStyle(Name = {Name})";
        #endregion
    }
}
