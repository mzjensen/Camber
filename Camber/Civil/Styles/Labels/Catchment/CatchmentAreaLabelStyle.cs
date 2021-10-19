#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.Catchment
{
    public sealed class CatchmentAreaLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal CatchmentAreaLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static CatchmentAreaLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<CatchmentAreaLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new CatchmentAreaLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Catchment Area Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static CatchmentAreaLabelStyle ByName(string name)
        {
            return (CatchmentAreaLabelStyle)CreateByNameType(
                name,
                LabelStyleCollections.CatchmentLabelStyles.ToString() + "." + CatchmentLabelStyles.AreaLabelStyles.ToString(),
                typeof(CatchmentAreaLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"CatchmentAreaLabelStyle(Name = {Name})";
        #endregion
    }
}
