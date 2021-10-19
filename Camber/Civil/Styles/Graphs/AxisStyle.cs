#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using AeccAxisStyle = Autodesk.Civil.DatabaseServices.Styles.AxisStyle;
using System;
#endregion


namespace Camber.Civil.Styles.Graphs
{
    public sealed class AxisStyle
    {
        #region properties
        private AeccAxisStyle AeccAxisStyle { get; set; }

        /// <summary>
        /// Gets the Style that an Axis Style belongs to.
        /// </summary>
        public Style ParentStyle { get; set; }

        /// <summary>
        /// Gets the Axis Tick Style that specifies the appearance of horizontal geometry ticks on the horizontal axes of an Axis Style.
        /// </summary>
        public AxisTickStyle HorizontalGeometryTickStyle => new AxisTickStyle(this, AeccAxisStyle.HorizontalGeometryTickStyle);

        /// <summary>
        /// Gets the Axis Tick Style that specifies the appearance of major ticks on the vertical or horizontal axes of an Axis Style.
        /// </summary>
        public AxisTickStyle MajorTickStyle => new AxisTickStyle(this, AeccAxisStyle.MajorTickStyle);

        /// <summary>
        /// Gets the Axis Tick Style that specifies the appearance of minor ticks on the vertical or horizontal axes of an Axis Style.
        /// </summary>
        public AxisTickStyle MinorTickStyle => new AxisTickStyle(this, AeccAxisStyle.MajorTickStyle);

        /// <summary>
        /// Gets whether ticks and labels are placed at the start station on the horizontal axes of an Axis Style.
        /// </summary>
        public bool ShowTickAndLabel => AeccAxisStyle.ShowTickAndLabel;

        /// <summary>
        /// Gets the Axis Title Style that control the appearance of the axis title for profile or section views.
        /// </summary>
        public AxisTitleStyle TitleStyle => new AxisTitleStyle(this, AeccAxisStyle.TitleStyle);
        #endregion

        #region constructors
        internal AxisStyle(Style parentStyle, AeccAxisStyle aeccAxisStyle)
        {
            ParentStyle = parentStyle;
            AeccAxisStyle = aeccAxisStyle;
        }

        #endregion

        #region methods
        public override string ToString() => $"AxisStyle(Parent Style = {ParentStyle.Name})";

        /// <summary>
        /// Sets whether ticks and labels are placed at the start station on the horizontal axes of an Axis Style.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public AxisStyle SetShowTickAndLabel(bool @bool)
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccParentStyle = ctx.Transaction.GetObject(ParentStyle.AeccStyle.ObjectId, acDb.OpenMode.ForWrite);
                    AeccAxisStyle.ShowTickAndLabel = @bool;
                    return this;
                }
            }
            catch (InvalidOperationException)
            {
                throw new Exception(
                    "This value cannot be set under these circumstances:\r\n" +
                    "1. The Axis Style is obtained from a Mass Haul View Style or Section View Style.\r\n" +
                    "2. The Axis Position is top.");
            }
        }
        #endregion
    }
}