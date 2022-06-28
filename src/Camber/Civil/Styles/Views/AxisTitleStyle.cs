#region references
using System.Reflection;
using System.Runtime.CompilerServices;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using AeccAxisTitleStyle = Autodesk.Civil.DatabaseServices.Styles.AxisTitleStyle;
#endregion


namespace Camber.Civil.Styles.Views
{
    public sealed class AxisTitleStyle
    {
        #region properties
        private AeccAxisTitleStyle AeccAxisTitleStyle { get; set; }

        /// <summary>
        /// Gets the Axis Style that an Axis Title Style belongs to.
        /// </summary>
        public AxisStyle ParentAxisStyle { get; set; }

        /// <summary>
        /// Gets the location of the title in relation to the view grid of an Axis Title Style.
        /// </summary>
        public string Location => AeccAxisTitleStyle.Location.ToString();

        /// <summary>
        /// Gets the horizontal offset from the location setting of an Axis Title Style.
        /// </summary>
        public double OffsetX => AeccAxisTitleStyle.OffsetX;

        /// <summary>
        /// Gets the vertical offset from the location setting of an Axis Title Style.
        /// </summary>
        public double OffsetY => AeccAxisTitleStyle.OffsetY;

        /// <summary>
        /// Gets the angle for the axis title of an Axis Title Style.
        /// </summary>
        public double Rotation => AeccAxisTitleStyle.Rotation;

        /// <summary>
        /// Gets the title text for the axis in an Axis Title Style.
        /// </summary>
        public string Text => AeccAxisTitleStyle.Text;

        /// <summary>
        /// Gets the axis title text height in plotted units of an Axis Title Style.
        /// </summary>
        public double TextHeight => AeccAxisTitleStyle.TextHeight;

        /// <summary>
        /// Gets the text style for the axis title of an Axis Title Style from the available AutoCAD text styles defined in the current drawing.
        /// </summary>
        public string TextStyle => AeccAxisTitleStyle.TextStyle;
        #endregion

        #region constructors
        internal AxisTitleStyle(AxisStyle parentAxisStyle, AeccAxisTitleStyle aeccAxisTitleStyle)
        {
            ParentAxisStyle = parentAxisStyle;
            AeccAxisTitleStyle = aeccAxisTitleStyle;
        }

        #endregion

        #region methods
        public override string ToString() => $"AxisTitleStyle";

        protected AxisTitleStyle SetValue(object value, [CallerMemberName] string methodName = null)
        {
            if (methodName.StartsWith("Set"))
            {
                methodName = methodName.Substring(3);
            }
            return SetValue(methodName, value);
        }

        protected AxisTitleStyle SetValue(string propertyName, object value)
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccParentStyle = ctx.Transaction.GetObject(ParentAxisStyle.ParentStyle.AeccStyle.ObjectId, acDb.OpenMode.ForWrite);
                    PropertyInfo propInfo = AeccAxisTitleStyle.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                    propInfo?.SetValue(AeccAxisTitleStyle, value);
                    return this;
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// Sets the location of the title in relation to the view grid of an Axis Title Style.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public AxisTitleStyle SetLocation(string location) => SetValue(location);

        /// <summary>
        /// Sets the horizontal offset from the location setting of an Axis Title Style.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public AxisTitleStyle SetOffsetX(double offset) => SetValue(offset);

        /// <summary>
        /// Sets the vertical offset from the location setting of an Axis Title Style.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public AxisTitleStyle SetOffsetY(double offset) => SetValue(offset);

        /// <summary>
        /// Sets the angle for the axis title of an Axis Title Style.
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public AxisTitleStyle SetRotation(double rotation) => SetValue(rotation);

        /// <summary>
        /// Sets the title text for the axis in an Axis Title Style.
        /// </summary>
        /// <param name="textString"></param>
        /// <returns></returns>
        public AxisTitleStyle SetText(string textString) => SetValue(textString);

        /// <summary>
        /// Sets the axis title text height in plotted units of an Axis Title Style.
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        public AxisTitleStyle SetTextHeight(double height) => SetValue(height);

        /// <summary>
        /// Sets the text style for the axis title of an Axis Title Style from the available AutoCAD text styles defined in the current drawing.
        /// </summary>
        /// <param name="textStyle"></param>
        /// <returns></returns>
        public AxisTitleStyle SetTextStyle(string textStyle) => SetValue(textStyle);
        #endregion
    }
}