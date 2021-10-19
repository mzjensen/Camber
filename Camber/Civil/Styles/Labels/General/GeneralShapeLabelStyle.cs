#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.General
{
    public sealed class GeneralShapeLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal GeneralShapeLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static GeneralShapeLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<GeneralShapeLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new GeneralShapeLabelStyle(labelStyle));

        /// <summary>
        /// Creates a General Shape Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GeneralShapeLabelStyle ByName(string name)
        {
            return (GeneralShapeLabelStyle)CreateByNameType(
                name, 
                GeneralLabelStyles.GeneralShapeLabelStyles.ToString(),  
                typeof(GeneralShapeLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"GeneralShapeLabelStyle(Name = {Name})";
        #endregion
    }
}
