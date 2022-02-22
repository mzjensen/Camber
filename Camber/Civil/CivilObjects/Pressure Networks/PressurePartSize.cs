#region references
using AeccPressurePartSize = Autodesk.Civil.DatabaseServices.Styles.PressurePartSize;
#endregion

namespace Camber.Civil.PressureNetworks
{
    public class PressurePartSize
    {
        #region properties
        internal AeccPressurePartSize AeccPressurePartSize { get; set; }

        /// <summary>
        /// Gets the description of a Pressure Part Size.
        /// </summary>
        public string Description => AeccPressurePartSize.Description;

        /// <summary>
        /// Gets whether a Pressure Part Size is a Pipe, Fitting, or Appurtenance.
        /// </summary>
        public string Domain => AeccPressurePartSize.Domain.ToString();

        /// <summary>
        /// Gets the type of a Pressure Part Size.
        /// </summary>
        public string Type => AeccPressurePartSize.PartType.ToString();
        #endregion

        #region constructors
        internal PressurePartSize(AeccPressurePartSize aeccPressurePartSize)
        {
            AeccPressurePartSize = aeccPressurePartSize;
        }
        #endregion

        #region methods
        public override string ToString() => $"PressurePartSize(Domain = {Domain}, Type = {Type})";
        #endregion
    }
}
