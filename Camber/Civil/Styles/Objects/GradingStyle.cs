#region references
using System;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccGradingStyle = Autodesk.Civil.DatabaseServices.Styles.GradingStyle;
#endregion

namespace Camber.Civil.Styles.Objects
{
    public sealed class GradingStyle : Style
    {
        #region properties
        internal AeccGradingStyle AeccGradingStyle => AcObject as AeccGradingStyle;

        /// <summary>
        /// Gets the value that specifies the size of the marker for a Grading Style and how it is calculated.
        /// </summary>
        public string CenterMarkerSizeOption => AeccGradingStyle.CenterMarker.ToString();

        /// <summary>
        /// Gets the marker size value for a Grading Style.
        /// </summary>
        public double CenterMarkerSize
        {
            get
            {
                double retVal = 0;
                if (CenterMarkerSizeOption == "Fixed")
                {
                    retVal = GetDouble("FixedSize");
                }
                else if (CenterMarkerSizeOption == "Plotted")
                {
                    retVal = GetDouble("PlottedSize");
                }
                else if (CenterMarkerSizeOption == "PercentageOfScreen")
                {
                    retVal = GetDouble("PercentageOfScreen");
                }
                return retVal;
            }
        }

        /// <summary>
        /// Gets the Slope Pattern Style for a Grading Style.
        /// </summary>
        public SlopePatternStyle SlopePatternStyle
        {
            get
            {
                if (!AeccGradingStyle.SlopePatternStyleId.IsNull)
                {
                    return SlopePatternStyle.GetByObjectId(AeccGradingStyle.SlopePatternStyleId);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the maximum value of the slope range for a Grading Style.
        /// </summary>
        public double SlopeRangeMax => GetDouble();

        /// <summary>
        /// Gets the minimum value of the slope range for a Grading Style.
        /// </summary>
        public double SlopeRangeMin => GetDouble();

        /// <summary>
        /// Gets whether to use a slope pattern style for a Grading Style.
        /// </summary>
        public bool UseSlopePatternStyle => GetBool();

        /// <summary>
        /// Gets whether the slope pattern is applied to a limited range of slope values for a Grading Style.
        /// </summary>
        public bool UseSlopeRange => GetBool();
        #endregion

        #region constructors
        internal GradingStyle(AeccGradingStyle aeccGradingStyle, bool isDynamoOwned = false) : base(aeccGradingStyle, isDynamoOwned) { }

        internal static GradingStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<GradingStyle, AeccGradingStyle>
            (styleId, (style) => new GradingStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"GradingStyle(Name = {Name})";

        /// <summary>
        /// Sets the value that specifies the size of the marker for a Grading Style and how it is calculated.
        /// </summary>
        /// <param name="centerMarkerSizeOption"></param>
        /// <returns></returns>
        public GradingStyle SetCenterMarkerSizeOption(string centerMarkerSizeOption) => (GradingStyle)SetValue(
            Enum.Parse(typeof(Autodesk.Civil.DatabaseServices.Styles.CenterMarkerSizeType), 
                centerMarkerSizeOption), 
            "CenterMarker");


        /// <summary>
        /// Sets the marker size value for a Grading Style.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public GradingStyle SetCenterMarkerSize(double value)
        {
            if (CenterMarkerSizeOption == "Fixed")
            {
                SetValue(value, "FixedSize");
            }
            else if (CenterMarkerSizeOption == "Plotted")
            {
                SetValue(value, "PlottedSize");
            }
            else if (CenterMarkerSizeOption == "PercentageOfScreen")
            {
                SetValue(value, "PercentageOfScreen");
            }
            return this;
        }

        /// <summary>
        /// Sets the Slope Pattern Style for a Grading Style.
        /// </summary>
        /// <param name="slopePatternStyle"></param>
        /// <returns></returns>
        public GradingStyle SetSlopePatternStyle(SlopePatternStyle slopePatternStyle) => (GradingStyle)SetValue(slopePatternStyle.InternalObjectId, "SlopePatternStyleId");

        /// <summary>
        /// Sets the maximum value of the slope range for a Grading Style.
        /// </summary>
        /// <param name="value">Slope expressed as a decimal, e.g. for 2:1 use 0.5.</param>
        /// <returns></returns>
        public GradingStyle SetSlopeRangeMax(double value) => (GradingStyle)SetValue(value);

        /// <summary>
        /// Gets the minimum value of the slope range. 
        /// </summary>
        /// <param name="value">Slope expressed as a decimal, e.g. for 2:1 use 0.5.</param>
        /// <returns></returns>
        public GradingStyle SetSlopeRangeMin(double value) => (GradingStyle)SetValue(value);

        /// <summary>
        /// Sets the boolean value that specifies whether the slope pattern is applied to a limited range of slope values.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public GradingStyle SetUseSlopeRange(bool @bool) => (GradingStyle)SetValue(@bool);
        #endregion
    }
}
