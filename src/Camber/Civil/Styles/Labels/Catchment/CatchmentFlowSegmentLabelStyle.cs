#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.Catchment
{
    public sealed class CatchmentFlowSegmentLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal CatchmentFlowSegmentLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static CatchmentFlowSegmentLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<CatchmentFlowSegmentLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new CatchmentFlowSegmentLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Catchment Flow Segment Label Style by name.
        /// </summary>
        /// <param name="name">The style name</param>
        /// <returns></returns>
        public static CatchmentFlowSegmentLabelStyle ByName(string name)
        {
            return (CatchmentFlowSegmentLabelStyle)CreateByNameType(
                name,
                LabelStyleCollections.CatchmentLabelStyles.ToString() + "." + CatchmentLabelStyles.FlowSegmentLabelStyles.ToString(),
                typeof(CatchmentFlowSegmentLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"CatchmentFlowSegmentLabelStyle(Name = {Name})";
        #endregion
    }
}
