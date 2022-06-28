#region references
using System;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using civDb = Autodesk.Civil.DatabaseServices;
using AeccPipeStyle = Autodesk.Civil.DatabaseServices.Styles.PipeStyle;
#endregion

namespace Camber.Civil.Styles.Objects
{
    public sealed class PipeStyle : Style
    {
        #region properties
        internal AeccPipeStyle AeccPipeStyle => AcObject as AeccPipeStyle;

        /// <summary>
        /// Gets the style for the inside walls of pipe crossings displayed in section view for a Pipe Style.
        /// </summary>
        public string SectionCrossingHatch => GetString();
        #endregion

        #region constructors
        internal PipeStyle(AeccPipeStyle aeccPipeStyle, bool isDynamoOwned = false) : base(aeccPipeStyle, isDynamoOwned) { }

        internal static PipeStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<PipeStyle, AeccPipeStyle>
            (styleId, (style) => new PipeStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"PipeStyle(Name = {Name})";

        /// <summary>
        /// Sets the style for the inside walls of pipe crossings displayed in section view for a Pipe Style.
        /// </summary>
        /// <param name="hatchOption"></param>
        /// <returns></returns>
        public PipeStyle SetSectionCrossingHatch(string hatchOption)
        {
            var aeccHatchType = Enum.Parse(typeof(civDb.Styles.PipeHatchType), hatchOption);
            SetValue(aeccHatchType);
            return this;
        }

        /// <summary>
        /// Gets a Pipe Style plan property by name.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object GetPlanPropertyByName(string propertyName) => GetPipeStyleProperty(true, propertyName);

        /// <summary>
        /// Gets a Pipe Style profile property by name.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object GetProfilePropertyByName(string propertyName) => GetPipeStyleProperty(false, propertyName);

        /// <summary>
        /// Sets a Pipe Style plan property by name.
        /// </summary>
        /// <param name="propertyName">The name of the property to be set</param>
        /// <param name="value">The new property value</param>
        /// <returns></returns>
        public PipeStyle SetPlanPropertyByName(string propertyName, object value) => SetPipeStyleProperty(true, propertyName, value);

        /// <summary>
        /// Sets a Pipe Style profile property by name.
        /// </summary>
        /// <param name="propertyName">The name of the property to be set</param>
        /// <param name="value">The new property value</param>
        /// <returns></returns>
        public PipeStyle SetProfilePropertyByName(string propertyName, object value) => SetPipeStyleProperty(false, propertyName, value);

        /// <summary>
        /// Set Pipe Style properties.
        /// </summary>
        /// <param name="usePlanOption">Set plan options if true, profile options if false</param>
        /// <param name="propertyName">The name of the property to be set</param>
        /// <param name="value">The new value</param>
        /// <returns></returns>
        protected PipeStyle SetPipeStyleProperty(bool usePlanOption, string propertyName, object value)
        {
            string invalidNameMessage = "Invalid property name.";
            string planOrProfile;
            if (usePlanOption)
            {
                if (propertyName == "CenterlineOptions")
                {
                    value = Enum.Parse(typeof(civDb.Styles.PipeCenterlineType), (string)value);
                }
                planOrProfile = "PlanOption";
            }
            else
            {
                if (propertyName == "CrossingHatch")
                {
                    value = Enum.Parse(typeof(civDb.Styles.PipeHatchType), (string)value);
                }
                planOrProfile = "ProfileOption";
            }

            switch (propertyName)
            {
                case "EndSizeOptions":
                    value = Enum.Parse(typeof(civDb.Styles.PipeUserDefinedType), (string)value);
                    break;
                case "EndSizeType":
                    value = Enum.Parse(typeof(civDb.Styles.PipeEndSizeType), (string)value);
                    break;
                case "HatchOptions":
                    value = Enum.Parse(typeof(civDb.Styles.PipeHatchType), (string)value);
                    break;
                case "WallSizeOptions":
                    value = Enum.Parse(typeof(civDb.Styles.PipeUserDefinedType), (string)value);
                    break;
                case "WallSizeType":
                    value = Enum.Parse(typeof(civDb.Styles.PipeWallSizeType), (string)value);
                    break;
                default:
                    throw new ArgumentException(invalidNameMessage);
            }

            bool openedForWrite = AeccPipeStyle.IsWriteEnabled;
            if (!openedForWrite) { AeccPipeStyle.UpgradeOpen(); }
            var res = Utilities.ReflectionUtilities.SetNestedProperty(AeccPipeStyle, planOrProfile + "." + propertyName, value);
            if (res == null) { throw new Exception("Value not set."); }
            if (!openedForWrite) { AeccPipeStyle.DowngradeOpen(); }
            return this;
        }

        /// <summary>
        /// Get Pipe Style properties.
        /// </summary>
        /// <param name="usePlanOption">Get plan options if true, profile options if false</param>
        /// <param name="propertyName">The name of the property to get</param>
        /// <returns></returns>
        protected object GetPipeStyleProperty(bool usePlanOption, string propertyName)
        {
            string planOrProfile = "PlanOption";
            if (!usePlanOption) { planOrProfile = "ProfileOption"; }

            var propVal = Utilities.ReflectionUtilities.GetNestedProperty(AeccPipeStyle, planOrProfile + "." + propertyName, null);
            if (propVal is Enum)
            {
                return propVal.ToString();
            }
            return propVal;
        }
        #endregion
    }
}
