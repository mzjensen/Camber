#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.General
{
    public sealed class GeneralCurveLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal GeneralCurveLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static GeneralCurveLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<GeneralCurveLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new GeneralCurveLabelStyle(labelStyle));

        /// <summary>
        /// Creates a General Curve Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GeneralCurveLabelStyle ByName(string name)
        {
            return (GeneralCurveLabelStyle)CreateByNameType(
                name, 
                GeneralLabelStyles.GeneralCurveLabelStyles.ToString(),  
                typeof(GeneralCurveLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"GeneralCurveLabelStyle(Name = {Name})";
        #endregion
    }
}
