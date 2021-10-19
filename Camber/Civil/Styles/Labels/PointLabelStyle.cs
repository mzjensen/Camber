#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels
{
    public sealed class PointLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal PointLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static PointLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<PointLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new PointLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Point Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PointLabelStyle ByName(string name)
        {
            return (PointLabelStyle)CreateByNameType(
                name, 
                "PointLabelStyle.LabelStyles", 
                typeof(PointLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"PointLabelStyle(Name = {Name})";
        #endregion
    }
}
