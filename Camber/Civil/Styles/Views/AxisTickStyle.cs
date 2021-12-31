#region references
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using civDb = Autodesk.Civil.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using AeccAxisTickStyle = Autodesk.Civil.DatabaseServices.Styles.AxisTickStyle;
#endregion


namespace Camber.Civil.Styles.Views
{
    public sealed class AxisTickStyle
    {
        #region properties
        private AeccAxisTickStyle AeccAxisTickStyle { get; set; }

        /// <summary>
        /// Gets the Axis Style that an Axis Tick Style belongs to.
        /// </summary>
        public AxisStyle ParentAxisStyle { get; set; }

        /// <summary>
        /// Gets the spacing of ticks on the axes of an Axis Tick Style. The spacing is defined in actual ground units.
        /// </summary>
        public double Interval => AeccAxisTickStyle.Interval;

        /// <summary>
        /// Gets the justification (alignment) of the title in an Axis Tick Style.
        /// </summary>
        public string Justification => AeccAxisTickStyle.Justification.ToString();

        /// <summary>
        /// Gets the label text for the tick in an Axis Tick Style.
        /// </summary>
        public string LabelText => AeccAxisTickStyle.LabelText;

        /// <summary>
        /// Gets the horizontal offset for the tick label from the bottom of the tick in an Axis Tick Style.
        /// </summary>
        public double OffsetX => AeccAxisTickStyle.OffsetX;

        /// <summary>
        /// Gets the vertical offset for the tick label from the bottom of the tick in an Axis Tick Style. 
        /// </summary>
        public double OffsetY => AeccAxisTickStyle.OffsetY;

        /// <summary>
        /// Gets the rotation angle for the tick label in an Axis Tick Style.
        /// </summary>
        public double Rotation => AeccAxisTickStyle.Rotation;

        /// <summary>
        /// Gets the height of the axis ticks in an Axis Tick Style. The units are specified in plotted units. 
        /// </summary>
        public double Size => AeccAxisTickStyle.Size;

        /// <summary>
        /// Gets the height of the text used to label ticks in an Axis Tick Style. The units are specified in plotted units.
        /// </summary>
        public double TextHeight => AeccAxisTickStyle.TextHeight;

        /// <summary>
        /// Gets the text style for an Axis Tick Style tick label from the available AutoCAD text styles defined in the current drawing.
        /// </summary>
        public string TextStyle => AeccAxisTickStyle.TextStyle;
        #endregion

        #region constructors
        internal AxisTickStyle(AxisStyle parentAxisStyle, AeccAxisTickStyle aeccAxisTickStyle)
        {
            ParentAxisStyle = parentAxisStyle;
            AeccAxisTickStyle = aeccAxisTickStyle;
        }

        #endregion

        #region methods
        public override string ToString() => $"AxisTickStyle";

        protected AxisTickStyle SetValue(object value, [CallerMemberName] string methodName = null)
        {
            if (methodName.StartsWith("Set"))
            {
                methodName = methodName.Substring(3);
            }
            return SetValue(methodName, value);
        }

        protected AxisTickStyle SetValue(string propertyName, object value)
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccParentStyle = ctx.Transaction.GetObject(ParentAxisStyle.ParentStyle.AeccStyle.ObjectId, acDb.OpenMode.ForWrite);
                    PropertyInfo propInfo = AeccAxisTickStyle.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                    propInfo?.SetValue(AeccAxisTickStyle, value);
                    return this;
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// Sets the spacing of ticks on the axes of an Axis Tick Style. The spacing is defined in actual ground units.
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public AxisTickStyle SetInterval(double interval) => SetValue(interval);

        /// <summary>
        /// Sets the justification (alignment) of the title in an Axis Tick Style.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public AxisTickStyle SetJustification(string value) => SetValue(Enum.Parse(typeof(civDb.Styles.AxisTickJustificationType), value));

        /// <summary>
        /// Sets the label text for the tick in an Axis Tick Style.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public AxisTickStyle SetLabelText(string value) => SetValue(value);

        /// <summary>
        /// Sets the horizontal offset for the tick label from the bottom of the tick in an Axis Tick Style.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public AxisTickStyle SetOffsetX(double offset) => SetValue(offset);

        /// <summary>
        /// Sets the vertical offset for the tick label from the bottom of the tick in an Axis Tick Style.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public AxisTickStyle SetOffsetY(double offset) => SetValue(offset);

        /// <summary>
        /// Sets the rotation angle for the tick label in an Axis Tick Style.
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public AxisTickStyle SetRotation(double rotation) => SetValue(rotation);

        /// <summary>
        /// Sets the height of the axis ticks in an Axis Tick Style. The units are specified in plotted units. 
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public AxisTickStyle SetSize(double size) => SetValue(size);

        /// <summary>
        /// Sets the height of the text used to label ticks in an Axis Tick Style. The units are specified in plotted units.
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        public AxisTickStyle SetTextHeight(double height) => SetValue(height);

        /// <summary>
        /// Sets the text style for an Axis Tick Style tick label from the available AutoCAD text styles defined in the current drawing.
        /// </summary>
        /// <param name="styleName"></param>
        /// <returns></returns>
        public AxisTickStyle SetTextStyle(string styleName) => SetValue(styleName);
        #endregion
    }
}