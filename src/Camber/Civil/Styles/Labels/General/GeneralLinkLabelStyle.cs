#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.General
{
    public sealed class GeneralLinkLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal GeneralLinkLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static GeneralLinkLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<GeneralLinkLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new GeneralLinkLabelStyle(labelStyle));

        /// <summary>
        /// Creates a General Link Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GeneralLinkLabelStyle ByName(string name)
        {
            return (GeneralLinkLabelStyle)CreateByNameType(
                name, 
                GeneralLabelStyles.GeneralLinkLabelStyles.ToString(),  
                typeof(GeneralLinkLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"GeneralLinkLabelStyle(Name = {Name})";
        #endregion
    }
}
