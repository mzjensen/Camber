#region references
using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccGridStyle = Autodesk.Civil.DatabaseServices.Styles.GridStyle;
using System;
#endregion


namespace Camber.Civil.Styles.Views
{
    public sealed class GridStyle
    {
        #region properties
        private AeccGridStyle AeccGridStyle { get; set; }

        /// <summary>
        /// Gets the Style that a Grid Style belongs to.
        /// </summary>
        public Style ParentStyle { get; set; }

        /// <summary>
        /// Gets whether horizontal Profile View grid lines are drawn to the highest of all displayed Profiles.
        /// </summary>
        public bool HorizontalClipToHighestProfile => AeccGridStyle.HorizontalGridOptions.ClipToHighestProfile;

        /// <summary>
        /// Gets whether horizontal Profile View grid lines are not drawn in the padding areas.
        /// </summary>
        public bool HorizontalOmitGridInPaddingAreas => AeccGridStyle.HorizontalGridOptions.OmitGridInPaddingAreas;

        /// <summary>
        /// Gets whether to clip horizontal grid lines at a selected Profile.
        /// If true, horizontal grid lines are drawn below the Profile line, but not above.
        /// If false, grid lines are drawn for the full height of the grid.
        /// </summary>
        public bool HorizontalUseClipGrid => AeccGridStyle.HorizontalGridOptions.UseClipGrid;

        /// <summary>
        /// Gets whether vertical Profile View grid lines are drawn to the highest of all displayed Profiles.
        /// </summary>
        public bool VerticalClipToHighestProfile => AeccGridStyle.HorizontalGridOptions.ClipToHighestProfile;

        /// <summary>
        /// Gets whether vertical Profile View grid lines are not drawn in the padding areas.
        /// </summary>
        public bool VerticalOmitGridInPaddingAreas => AeccGridStyle.HorizontalGridOptions.OmitGridInPaddingAreas;

        /// <summary>
        /// Gets whether to clip vertical grid lines at a selected Profile.
        /// If true, vertical grid lines are drawn below the Profile line, but not above.
        /// If false, grid lines are drawn for the full height of the grid.
        /// </summary>
        public bool VerticalUseClipGrid => AeccGridStyle.HorizontalGridOptions.UseClipGrid;
        #endregion

        #region constructors
        internal GridStyle(Style parentStyle, AeccGridStyle aeccGridStyle)
        {
            ParentStyle = parentStyle;
            AeccGridStyle = aeccGridStyle;
        }
        #endregion

        #region methods
        public override string ToString() => $"GridStyle(Parent Style = {ParentStyle.Name})";

        protected GridStyle SetValue(object value, [CallerMemberName] string methodName = null)
        {
            if (methodName.StartsWith("Set"))
            {
                methodName = methodName.Substring(3);
            }
            return SetValue(methodName, value);
        }

        protected GridStyle SetValue(string propertyName, object value)
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccParentStyle = ctx.Transaction.GetObject(ParentStyle.AeccStyle.ObjectId, acDb.OpenMode.ForWrite);
                    PropertyInfo propInfo = AeccGridStyle.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                    propInfo?.SetValue(AeccGridStyle, value);
                    return this;
                }
            }
            catch { throw; }
        }

        protected GridStyle SetNamedValue(bool useHorizontal, string innerPropertyName, object value)
        {
            string outerPropName = "HorizontalGridOptions";
            if (!useHorizontal) { outerPropName = "VerticalGridOptions"; }

            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccParentStyle = ctx.Transaction.GetObject(ParentStyle.AeccStyle.ObjectId, acDb.OpenMode.ForWrite);
                    Utils.ReflectionUtils.SetNestedProperty(AeccGridStyle, outerPropName + "." + innerPropertyName, value);
                    return this;
                }
            }
            catch (Exception e) { throw e.InnerException; }
        }

        /// <summary>
        /// Gets the axis offset values of a Grid Style.
        /// </summary>
        /// <returns></returns>
        [MultiReturn(new[] { "Above", "Bottom", "Left", "Right" })]
        public Dictionary<string, object> GetAxisOffsets()
        {
            return new Dictionary<string, object>
                {
                    { "Above", AeccGridStyle.AxisOffsetAbove },
                    { "Bottom", AeccGridStyle.AxisOffsetBottom },
                    { "Left", AeccGridStyle.AxisOffsetLeft },
                    { "Right", AeccGridStyle.AxisOffsetRight }
                };
        }

        /// <summary>
        /// Gets grid padding values of a Grid Style.
        /// </summary>
        /// <returns></returns>
        [MultiReturn(new[] { "Above", "Bottom", "Left", "Right" })]
        public Dictionary<string, object> GetGridPadding()
        {
            return new Dictionary<string, object>
                {
                    { "Above", AeccGridStyle.GridPaddingAbove },
                    { "Bottom", AeccGridStyle.GridPaddingBottom },
                    { "Left", AeccGridStyle.GridPaddingLeft },
                    { "Right", AeccGridStyle.GridPaddingRight }
                };
        }

        /// <summary>
        /// Sets the axis offset values of a Grid Style.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public GridStyle SetAxisOffsets(string location, double value) => SetValue(value, "AxisOffset" + location);

        /// <summary>
        /// Sets the grid padding values of a Grid Style.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public GridStyle SetGridPadding(string location, double value) => SetValue(value, "GridPadding" + location);

        /// <summary>
        /// Sets whether horizontal Profile View grid lines are drawn to the highest of all displayed Profiles.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public GridStyle SetHorizontalClipToHighestProfile(bool @bool) => SetNamedValue(true, "ClipToHighestProfile", @bool);

        /// <summary>
        /// Sets whether horizontal Profile View grid lines are not drawn in the padding areas.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public GridStyle SetHorizontalOmitGridInPaddingAreas(bool @bool) => SetNamedValue(true, "OmitGridInPaddingAreas", @bool);

        /// <summary>
        /// Sets whether to clip horizontal grid lines at a selected Profile.
        /// If true, horizontal grid lines are drawn below the Profile line, but not above.
        /// If false, grid lines are drawn for the full height of the grid. 
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public GridStyle SetHorizontalUseClipGrid(bool @bool) => SetNamedValue(true, "UseClipGrid", @bool);

        /// <summary>
        /// Sets whether vertical Profile View grid lines are drawn to the highest of all displayed Profiles.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public GridStyle SetVerticalClipToHighestProfile(bool @bool) => SetNamedValue(false, "ClipToHighestProfile", @bool);

        /// <summary>
        /// Sets whether vertical Profile View grid lines are not drawn in the padding areas.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public GridStyle SetVerticalOmitGridInPaddingAreas(bool @bool) => SetNamedValue(false, "OmitGridInPaddingAreas", @bool);

        /// <summary>
        /// Sets whether to clip vertical grid lines at a selected Profile.
        /// If true, vertical grid lines are drawn below the Profile line, but not above.
        /// If false, grid lines are drawn for the full height of the grid. 
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public GridStyle SetVerticalUseClipGrid(bool @bool) => SetNamedValue(false, "UseClipGrid", @bool);
        #endregion
    }
}