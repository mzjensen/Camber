#region references
using System.Reflection;
using System.Runtime.CompilerServices;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using AeccGraphTitleStyle = Autodesk.Civil.DatabaseServices.Styles.GraphTitleStyle;
#endregion


namespace Camber.Civil.Styles.Views
{
    public sealed class ViewTitleStyle
    {
        #region properties
        private AeccGraphTitleStyle AeccGraphTitleStyle { get; set; }

        /// <summary>
        /// Gets the View Style that a View Title Style belongs to.
        /// </summary>
        public ViewStyle ParentViewStyle { get; set; }

        /// <summary>
        /// Gets whether a border line is drawn around the title block of a View Title Style.
        /// </summary>
        public bool Border => AeccGraphTitleStyle.Border;

        /// <summary>
        /// Gets the gap between the border and view title text in plotted units of a View Title Style.
        /// </summary>
        public double BorderGap => AeccGraphTitleStyle.BorderGap;

        /// <summary>
        /// Gets the justification (alignment) of the title in a View Title Style.
        /// </summary>
        public string Justification => AeccGraphTitleStyle.Justification.ToString();

        /// <summary>
        /// Gets the location of the title in relation to the view grid of a View Title Style.
        /// </summary>
        public string Location => AeccGraphTitleStyle.Location.ToString();

        /// <summary>
        /// Gets the horizontal offset from the location setting of a View Title Style.
        /// </summary>
        public double OffsetX => AeccGraphTitleStyle.OffsetX;

        /// <summary>
        /// Gets the vertical offset from the location setting of a View Title Style.
        /// </summary>
        public double OffsetY => AeccGraphTitleStyle.OffsetY;

        /// <summary>
        /// Gets the title text for the axis in a View Title Style.
        /// </summary>
        public string Text => AeccGraphTitleStyle.Text;

        /// <summary>
        /// Gets the axis title text height in plotted units of a View Title Style.
        /// </summary>
        public double TextHeight => AeccGraphTitleStyle.TextHeight;

        /// <summary>
        /// Gets the text style for the axis title of a View Title Style from the available AutoCAD text styles defined in the current drawing.
        /// </summary>
        public string TextStyle => AeccGraphTitleStyle.TextStyle;
        #endregion

        #region constructors
        internal ViewTitleStyle(ViewStyle parentViewStyle, AeccGraphTitleStyle aeccGraphTitleStyle)
        {
            ParentViewStyle = parentViewStyle;
            AeccGraphTitleStyle = aeccGraphTitleStyle;
        }

        #endregion

        #region methods
        public override string ToString() => $"ViewTitleStyle";

        protected ViewTitleStyle SetValue(object value, [CallerMemberName] string methodName = null)
        {
            if (methodName.StartsWith("Set"))
            {
                methodName = methodName.Substring(3);
            }
            return SetValue(methodName, value);
        }

        protected ViewTitleStyle SetValue(string propertyName, object value)
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccParentStyle = ctx.Transaction.GetObject(ParentViewStyle.ParentStyle.AeccStyle.ObjectId, acDb.OpenMode.ForWrite);
                    PropertyInfo propInfo = AeccGraphTitleStyle.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                    propInfo?.SetValue(AeccGraphTitleStyle, value);
                    return this;
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// Sets whether a border line is drawn around the title block of a View Title Style.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public ViewTitleStyle SetBorder(bool @bool) => SetValue(@bool);

        /// <summary>
        /// Sets the gap between the border and view title text in plotted units of a View Title Style.
        /// </summary>
        /// <param name="gap"></param>
        /// <returns></returns>
        public ViewTitleStyle SetBorderGap(double gap) => SetValue(gap);

        /// <summary>
        /// Sets the justification (alignment) of the title in a View Title Style.
        /// </summary>
        /// <param name="justification"></param>
        /// <returns></returns>
        public ViewTitleStyle SetJustification(string justification) => SetValue(justification);

        /// <summary>
        /// Sets the location of the title in relation to the view grid of a View Title Style.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public ViewTitleStyle SetLocation(string location) => SetValue(location);

        /// <summary>
        /// Sets the horizontal offset from the location setting of a View Title Style.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public ViewTitleStyle SetOffsetX(double offset) => SetValue(offset);

        /// <summary>
        /// Sets the vertical offset from the location setting of a View Title Style.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public ViewTitleStyle SetOffsetY(double offset) => SetValue(offset);

        /// <summary>
        /// Sets the title text for the axis in a View Title Style.
        /// </summary>
        /// <param name="textString"></param>
        /// <returns></returns>
        public ViewTitleStyle SetText(string textString) => SetValue(textString);

        /// <summary>
        /// Sets the axis title text height in plotted units of a View Title Style.
        /// </summary>
        /// <param name="textHeight"></param>
        /// <returns></returns>
        public ViewTitleStyle SetTextHeight(double textHeight) => SetValue(textHeight);

        /// <summary>
        /// Sets the text style for the axis title of a View Title Style from the available AutoCAD text styles defined in the current drawing.
        /// </summary>
        /// <param name="textStyle"></param>
        /// <returns></returns>
        public ViewTitleStyle SetTextStyle(string textStyle) => SetValue(textStyle);
        #endregion
    }
}