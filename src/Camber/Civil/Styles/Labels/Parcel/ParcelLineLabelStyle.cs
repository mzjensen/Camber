#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.Parcel
{
    public sealed class ParcelLineLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal ParcelLineLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static ParcelLineLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<ParcelLineLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new ParcelLineLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Parcel Line Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ParcelLineLabelStyle ByName(string name)
        {
            return (ParcelLineLabelStyle)CreateByNameType(
                name, 
                LabelStyleCollections.ParcelLabelStyles.ToString() + "." + ParcelLabelStyles.LineLabelStyles.ToString(), 
                typeof(ParcelLineLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"ParcelLineLabelStyle(Name = {Name})";
        #endregion
    }
}
