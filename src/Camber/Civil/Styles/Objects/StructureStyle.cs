using Autodesk.DesignScript.Runtime;
using System;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccStructureStyle = Autodesk.Civil.DatabaseServices.Styles.StructureStyle;
using civDb = Autodesk.Civil.DatabaseServices;

namespace Camber.Civil.Styles.Objects
{
    public sealed class StructureStyle : Style
    {
        #region properties
        internal AeccStructureStyle AeccStructureStyle => AcObject as AeccStructureStyle;
        #endregion

        #region constructors
        internal StructureStyle(AeccStructureStyle aeccStructureStyle, bool isDynamoOwned = false) : base(aeccStructureStyle, isDynamoOwned) { }

        internal static StructureStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<StructureStyle, AeccStructureStyle>
            (styleId, (style) => new StructureStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"StructureStyle(Name = {Name})";

        /// <summary>
        /// Gets a Structure Style plan property by name.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object GetPlanPropertyByName(string propertyName) => GetStructureStyleProperty(StructureViewDirection.PlanOption, propertyName);

        /// <summary>
        /// Gets a Structure Style profile property by name.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object GetProfilePropertyByName(string propertyName) => GetStructureStyleProperty(StructureViewDirection.ProfileOption, propertyName);

        /// <summary>
        /// Gets a Structure Style section property by name.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object GetSectionPropertyByName(string propertyName) => GetStructureStyleProperty(StructureViewDirection.SectionOption, propertyName);

        /// <summary>
        /// Gets a Structure Style model property by name.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object GetModelPropertyByName(string propertyName) => GetStructureStyleProperty(StructureViewDirection.ModelOption, propertyName);

        /// <summary>
        /// Sets a Structure Style plan property by name.
        /// </summary>
        /// <param name="propertyName">The name of the property to be set</param>
        /// <param name="value">The new property value</param>
        /// <returns></returns>
        public StructureStyle SetPlanPropertyByName(string propertyName, object value) => SetStructureStyleProperty(StructureViewDirection.PlanOption, propertyName, value);

        /// <summary>
        /// Sets a Structure Style profile property by name.
        /// </summary>
        /// <param name="propertyName">The name of the property to be set</param>
        /// <param name="value">The new property value</param>
        /// <returns></returns>
        public StructureStyle SetProfilePropertyByName(string propertyName, object value) => SetStructureStyleProperty(StructureViewDirection.ProfileOption, propertyName, value);

        /// <summary>
        /// Sets a Structure Style section property by name.
        /// </summary>
        /// <param name="propertyName">The name of the property to be set</param>
        /// <param name="value">The new property value</param>
        /// <returns></returns>
        public StructureStyle SetSectionPropertyByName(string propertyName, object value) => SetStructureStyleProperty(StructureViewDirection.SectionOption, propertyName, value);

        /// <summary>
        /// Sets a Structure Style model property by name.
        /// </summary>
        /// <param name="propertyName">The name of the property to be set</param>
        /// <param name="value">The new property value</param>
        /// <returns></returns>
        public StructureStyle SetModelPropertyByName(string propertyName, object value) => SetStructureStyleProperty(StructureViewDirection.ModelOption, propertyName, value);

        /// <summary>
        /// Set Structure Style properties.
        /// </summary>
        /// <param name="viewDirection"></param>
        /// <param name="propertyName">The name of the property to be set</param>
        /// <param name="value">The property value</param>
        /// <returns></returns>
        private StructureStyle SetStructureStyleProperty(StructureViewDirection viewDirection, string propertyName, object value)
        {
            const string invalidNameMessage = "Invalid property name.";

            switch (viewDirection)
            {
                case StructureViewDirection.ModelOption when propertyName == "ModelViewOptions":
                    value = Enum.Parse(typeof(civDb.Styles.StructureModelViewOptionType), (string)value);
                    break;
                case StructureViewDirection.ModelOption:
                {
                    if (propertyName == "SimpleSolidType")
                    {
                        value = Enum.Parse(typeof(civDb.Styles.StructureSimpleSolidType), (string)value);
                    }
                    break;
                }
                case StructureViewDirection.PlanOption when propertyName == "PlanViewOptions":
                    value = Enum.Parse(typeof(civDb.Styles.StructurePlanViewType), (string)value);
                    break;
                case StructureViewDirection.PlanOption:
                {
                    if (propertyName == "SizeType")
                    {
                        value = Enum.Parse(typeof(civDb.Styles.StructureSizeOptionsType), (string)value);
                    }
                    break;
                }
                case StructureViewDirection.ProfileOption:
                case StructureViewDirection.SectionOption:
                default:
                    switch (propertyName)
                    {
                        case "BlockInsertLocation":
                            value = Enum.Parse(typeof(civDb.Styles.StructureInsertionLocation), (string)value);
                            break;
                        case "SizeType":
                            value = Enum.Parse(typeof(civDb.Styles.StructureSizeOptionsType), (string)value);
                            break;
                        case "ViewOptions":
                            value = Enum.Parse(typeof(civDb.Styles.StructureViewType), (string)value);
                            break;
                    }
                    break;
            }

            using (var ctx = new Autodesk.AutoCAD.DynamoApp.Services.DocumentContext(AcDatabase))
            {
                var aeccStructureStyle = (AeccStructureStyle) ctx.Transaction.GetObject(InternalObjectId, acDb.OpenMode.ForWrite);
                var res = Utilities.ReflectionUtilities.SetNestedProperty(aeccStructureStyle, viewDirection + "." + propertyName, value);
                if (res == null) { throw new Exception("Value not set."); }
                return this;
            }
        }

        /// <summary>
        /// Get Structure Style properties.
        /// </summary>
        /// <param name="viewDirection"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private object GetStructureStyleProperty(StructureViewDirection viewDirection, string propertyName)
        {
            var propVal = Utilities.ReflectionUtilities.GetNestedProperty(AeccStructureStyle, viewDirection.ToString() + "." + propertyName, null);
            return propVal is Enum ? propVal.ToString() : propVal;
        }

        [IsVisibleInDynamoLibrary(false)]
        public enum StructureViewDirection
        {
            ModelOption,
            PlanOption,
            ProfileOption,
            SectionOption
        }
        #endregion
    }
}
