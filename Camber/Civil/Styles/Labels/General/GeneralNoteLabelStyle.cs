#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.General
{
    public sealed class GeneralNoteLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal GeneralNoteLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static GeneralNoteLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<GeneralNoteLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new GeneralNoteLabelStyle(labelStyle));

        /// <summary>
        /// Creates a General Note Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GeneralNoteLabelStyle ByName(string name)
        {
            return (GeneralNoteLabelStyle)CreateByNameType(
                name, 
                GeneralLabelStyles.GeneralNoteLabelStyles.ToString(),  
                typeof(GeneralNoteLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"GeneralNoteLabelStyle(Name = {Name})";
        #endregion
    }
}
