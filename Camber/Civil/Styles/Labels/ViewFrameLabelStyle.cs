#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels
{
    public sealed class ViewFrameLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal ViewFrameLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static ViewFrameLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<ViewFrameLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new ViewFrameLabelStyle(labelStyle));

        /// <summary>
        /// Creates a View Frame Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ViewFrameLabelStyle ByName(string name)
        {
            return (ViewFrameLabelStyle)CreateByNameType(
                name, 
                "ViewFrameLabelStyles.LabelStyles", 
                typeof(ViewFrameLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"ViewFrameLabelStyle(Name = {Name})";
        #endregion
    }
}
