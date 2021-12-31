#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccMassHaulLineStyle = Autodesk.Civil.DatabaseServices.Styles.MassHaulLineStyle;
#endregion

namespace Camber.Civil.Styles.Objects
{
    public sealed class MassHaulLineStyle : Style
    {
        #region properties
        internal AeccMassHaulLineStyle AeccMassHaulLineStyle => AcObject as AeccMassHaulLineStyle;

        /// <summary>
        /// Gets the free haul option for a Mass Haul Line Style, which determines how to show the free haul in the view.
        /// </summary>
        public string FreeHaulOption => AeccMassHaulLineStyle.FreeHaulOption.ToString();
        #endregion

        #region constructors
        internal MassHaulLineStyle(AeccMassHaulLineStyle aeccMassHaulLineStyle, bool isDynamoOwned = false) : base(aeccMassHaulLineStyle, isDynamoOwned) { }

        internal static MassHaulLineStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<MassHaulLineStyle, AeccMassHaulLineStyle>
            (styleId, (style) => new MassHaulLineStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"MassHaulLineStyle(Name = {Name})";

        /// <summary>
        /// Sets the free haul option for a Mass Haul Line Style, which determines how to show the free haul in the view.
        /// </summary>
        /// <param name="useGradePoint">True = Grade Point, False = Balance Point</param>
        /// <returns></returns>
        public MassHaulLineStyle SetFreeHaulOption(bool useGradePoint)
        {
            var freeHaulOption = Autodesk.Civil.FreeHaulDisplayType.BalancePoint;
            if (useGradePoint)
            {
                freeHaulOption = Autodesk.Civil.FreeHaulDisplayType.GradePoint;
            }
            SetValue(freeHaulOption);
            return this;
        }
        #endregion
    }
}
