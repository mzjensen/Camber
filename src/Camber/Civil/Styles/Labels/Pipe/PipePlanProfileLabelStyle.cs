#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.Pipe
{
    public sealed class PipePlanProfileLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal PipePlanProfileLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static PipePlanProfileLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<PipePlanProfileLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new PipePlanProfileLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Pipe Plan Profile Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PipePlanProfileLabelStyle ByName(string name)
        {
            return (PipePlanProfileLabelStyle)CreateByNameType(
                name, 
                LabelStyleCollections.PipeLabelStyles.ToString() + "." + PipeLabelStyles.PlanProfileLabelStyles.ToString(), 
                typeof(PipePlanProfileLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"PipePlanProfileLabelStyle(Name = {Name})";
        #endregion
    }
}
