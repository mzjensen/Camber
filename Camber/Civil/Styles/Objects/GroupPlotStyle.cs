#region references
using System;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using civDb = Autodesk.Civil.DatabaseServices;
using AeccGroupPlotStyle = Autodesk.Civil.DatabaseServices.Styles.GroupPlotStyle;
#endregion

namespace Camber.Civil.Styles.Objects
{
    public sealed class GroupPlotStyle : Style
    {
        #region properties
        internal AeccGroupPlotStyle AeccGroupPlotStyle => AcObject as AeccGroupPlotStyle;

        /// <summary>
        /// Gets the value that specifies how the Section Views are aligned for a Group Plot Style.
        /// </summary>
        public string AlignType => AeccGroupPlotStyle.AlignType.ToString();

        /// <summary>
        /// Gets the value that specifies cell size as either uniform or uniform row/column for a Group Plot Style.
        /// </summary>
        public string CellSizeType => AeccGroupPlotStyle.CellSizeType.ToString();

        /// <summary>
        /// Gets the plotted distance between each successive page for a Group Plot Style.
        /// </summary>
        public double GapBetweenPages => GetDouble();

        /// <summary>
        /// Gets the value of horizontal major plot area grid for a Group Plot Style.
        /// </summary>
        public double GridHorizontalMajor => GetDouble();

        /// <summary>
        /// Gets the value of horizontal minor plot area grid for a Group Plot Style.
        /// </summary>
        public double GridHorizontalMinor => GetDouble();

        /// <summary>
        /// Gets the value of vertical major plot area grid for a Group Plot Style.
        /// </summary>
        public double GridVerticalMajor => GetDouble();

        /// <summary>
        /// Gets the value of vertical minor plot area grid for a Group Plot Style.
        /// </summary>
        public double GridVerticalMinor => GetDouble();

        /// <summary>
        /// Gets the maximum number of Section Views plotted in a column before moving on to the next column for a Group Plot Style.
        /// </summary>
        public int MaximumInColumn => GetInt();

        /// <summary>
        /// Gets the maximum number of Section Views plotted in a row before moving on to the next row for a Group Plot Style.
        /// </summary>
        public int MaximumInRow => GetInt();

        /// <summary>
        /// Gets the value that specifies whether to plot the Section Views by column or row for a Group Plot Style.
        /// </summary>
        public string PlotRule => AeccGroupPlotStyle.PlotRule.ToString();

        /// <summary>
        /// Gets the minimum number of major grid lines between columns of Section Views for a Group Plot Style.
        /// </summary>
        public double SpaceColumn => GetDouble();

        /// <summary>
        /// Gets the minimum number of major grid lines between rows of Section Views for a Group Plot Style.
        /// </summary>
        public double SpaceRow => GetDouble();

        /// <summary>
        /// Gets the value that specifies the starting corner type for plotting the Section Views in a Group Plot Style.
        /// </summary>
        public string StartCorner => AeccGroupPlotStyle.StartCorner.ToString();
        #endregion

        #region constructors
        internal GroupPlotStyle(AeccGroupPlotStyle aeccGroupPlotStyle, bool isDynamoOwned = false) : base(aeccGroupPlotStyle, isDynamoOwned) { }

        internal static GroupPlotStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<GroupPlotStyle, AeccGroupPlotStyle>
            (styleId, (style) => new GroupPlotStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"GroupPlotStyle(Name = {Name})";

        /// <summary>
        /// Sets the value that specifies how the Section Views are aligned for a Group Plot Style.
        /// </summary>
        /// <param name="alignType"></param>
        /// <returns></returns>
        public GroupPlotStyle SetAlignType(string alignType) => (GroupPlotStyle)SetValue(Enum.Parse(typeof(civDb.Styles.GroupPlotAlignType), alignType));

        /// <summary>
        /// Sets the value that specifies cell size as either uniform or uniform row/column for a Group Plot Style.
        /// </summary>
        /// <param name="useUniformForAll"></param>
        /// <returns></returns>
        public GroupPlotStyle SetCellSizeType(bool useUniformForAll)
        {
            var cellSizeType = civDb.Styles.GroupPlotCellSizeType.UniformPerRowOrColumn;
            if (useUniformForAll)
            {
                cellSizeType = civDb.Styles.GroupPlotCellSizeType.UniformForAll;
            }
            SetValue(cellSizeType);
            return this;
        }

        /// <summary>
        /// Sets the plotted distance between each successive page for a Group Plot Style. 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public GroupPlotStyle SetGapBetweenPages(double value) => (GroupPlotStyle)SetValue(value);

        /// <summary>
        /// Sets the value of horizontal major plot area grid for a Group Plot Style. 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public GroupPlotStyle SetGridHorizontalMajor(double value) => (GroupPlotStyle)SetValue(value);

        /// <summary>
        /// Sets the value of horizontal minor plot area grid for a Group Plot Style. 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public GroupPlotStyle SetGridHorizontalMinor(double value) => (GroupPlotStyle)SetValue(value);

        /// <summary>
        /// Sets the value of vertical major plot area grid for a Group Plot Style. 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public GroupPlotStyle SetGridVerticalMajor(double value) => (GroupPlotStyle)SetValue(value);

        /// <summary>
        /// Sets the value of vertical minor plot area grid for a Group Plot Style. 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public GroupPlotStyle SetGridVerticalMinor(double value) => (GroupPlotStyle)SetValue(value);

        /// <summary>
        /// Sets the maximum number of Section Views plotted in a column before moving on to the next column for a Group Plot Style.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public GroupPlotStyle SetMaximumInColumn(int value) => (GroupPlotStyle)SetValue(value);

        /// <summary>
        /// Sets the maximum number of Section Views plotted in a row before moving on to the next column for a Group Plot Style.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public GroupPlotStyle SetMaximumInRow(int value) => (GroupPlotStyle)SetValue(value);

        /// <summary>
        /// Sets the value that specifies whether to plot the Section Views by column or row for a Group Plot Style.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public GroupPlotStyle SetPlotRule(bool useByRows)
        {
            var plotRule = civDb.Styles.SectionViewPlotRuleType.ByColumns;
            if (useByRows)
            {
                plotRule = civDb.Styles.SectionViewPlotRuleType.ByRows;
            }
            SetValue(plotRule);
            return this;
        }

        /// <summary>
        /// Sets the minimum number of major grid lines between columns of Section Views for a Group Plot Style.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public GroupPlotStyle SetSpaceColumn(double value) => (GroupPlotStyle)SetValue(value);

        /// <summary>
        /// Sets the minimum number of major grid lines between rows of Section Views for a Group Plot Style.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public GroupPlotStyle SetSpaceRow(double value) => (GroupPlotStyle)SetValue(value);

        /// <summary>
        /// Sets the value that specifies the starting corner type for plotting the Section Views in a Group Plot Style.
        /// </summary>
        /// <param name="startCornerType"></param>
        /// <returns></returns>
        public GroupPlotStyle SetStartCorner(string startCornerType) => (GroupPlotStyle)SetValue(Enum.Parse(typeof(civDb.Styles.GroupPlotStartCornerType), startCornerType));
        #endregion
    }
}
