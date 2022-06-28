#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.Projection
{
    public sealed class SectionViewProjectionLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal SectionViewProjectionLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static SectionViewProjectionLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<SectionViewProjectionLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new SectionViewProjectionLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Section View Projection Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SectionViewProjectionLabelStyle ByName(string name)
        {
            return (SectionViewProjectionLabelStyle)CreateByNameType(
                name, 
                LabelStyleCollections.ProjectionLabelStyles.ToString() + "." + ProjectionLabelStyles.SectionViewProjectionLabelStyles.ToString(), 
                typeof(SectionViewProjectionLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"SectionViewProjectionLabelStyle(Name = {Name})";
        #endregion
    }
}
