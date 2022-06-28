#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels
{
    public sealed class IntersectionLocationLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal IntersectionLocationLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static IntersectionLocationLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<IntersectionLocationLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new IntersectionLocationLabelStyle(labelStyle));

        /// <summary>
        /// Creates an Intersection Location Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IntersectionLocationLabelStyle ByName(string name)
        {
            return (IntersectionLocationLabelStyle)CreateByNameType(
                name, 
                "IntersectionLabelStyles.LocationLabelStyles", 
                typeof(IntersectionLocationLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"IntersectionLocationLabelStyle(Name = {Name})";
        #endregion
    }
}
