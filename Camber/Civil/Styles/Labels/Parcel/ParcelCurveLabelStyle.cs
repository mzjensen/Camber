#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.Parcel
{
    public sealed class ParcelCurveLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal ParcelCurveLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static ParcelCurveLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<ParcelCurveLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new ParcelCurveLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Parcel Curve Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ParcelCurveLabelStyle ByName(string name)
        {
            return (ParcelCurveLabelStyle)CreateByNameType(
                name, 
                LabelStyleCollections.ParcelLabelStyles.ToString() + "." + ParcelLabelStyles.CurveLabelStyles.ToString(), 
                typeof(ParcelCurveLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"ParcelCurveLabelStyle(Name = {Name})";
        #endregion
    }
}
