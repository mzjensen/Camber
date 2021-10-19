#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.Pipe
{
    public sealed class PipeProfileViewLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal PipeProfileViewLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static PipeProfileViewLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<PipeProfileViewLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new PipeProfileViewLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Pipe Profile View Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PipeProfileViewLabelStyle ByName(string name)
        {
            return (PipeProfileViewLabelStyle)CreateByNameType(
                name, 
                LabelStyleCollections.PipeLabelStyles.ToString() + "." + PipeLabelStyles.CrossProfileLabelStyles.ToString(), 
                typeof(PipeProfileViewLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"PipeProfileViewLabelStyle(Name = {Name})";
        #endregion
    }
}
