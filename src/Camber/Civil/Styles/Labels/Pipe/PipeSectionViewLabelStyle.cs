#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.Pipe
{
    public sealed class PipeSectionViewLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal PipeSectionViewLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static PipeSectionViewLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<PipeSectionViewLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new PipeSectionViewLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Pipe Section View Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PipeSectionViewLabelStyle ByName(string name)
        {
            return (PipeSectionViewLabelStyle)CreateByNameType(
                name, 
                LabelStyleCollections.PipeLabelStyles.ToString() + "." + PipeLabelStyles.CrossSectionLabelStyles.ToString(), 
                typeof(PipeSectionViewLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"PipeSectionViewLabelStyle(Name = {Name})";
        #endregion
    }
}
