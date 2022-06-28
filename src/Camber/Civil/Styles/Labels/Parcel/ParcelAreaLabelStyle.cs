#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.Parcel
{
    public sealed class ParcelAreaLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal ParcelAreaLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static ParcelAreaLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<ParcelAreaLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new ParcelAreaLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Parcel Area Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ParcelAreaLabelStyle ByName(string name)
        {
            return (ParcelAreaLabelStyle)CreateByNameType(
                name, 
                LabelStyleCollections.ParcelLabelStyles.ToString() + "." + ParcelLabelStyles.AreaLabelStyles.ToString(), 
                typeof(ParcelAreaLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"ParcelAreaLabelStyle(Name = {Name})";
        #endregion
    }
}
