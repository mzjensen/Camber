#region references
using System.Reflection;
using System.Runtime.CompilerServices;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using AeccGraphStyle = Autodesk.Civil.DatabaseServices.Styles.GraphStyle;
#endregion


namespace Camber.Civil.Styles.Views
{
    /// <summary>
    /// Wraps the API 'GraphStyle' class
    /// </summary>
    public sealed class ViewStyle
    {
        #region properties
        private AeccGraphStyle AeccGraphStyle { get; set; }

        /// <summary>
        /// Gets the Style that a View Style belongs to.
        /// </summary>
        public Style ParentStyle { get; set; }

        /// <summary>
        /// Gets the plotted size of the horizontal scale of a View Style.
        /// </summary>
        public double CurrentHorizontalScale => AeccGraphStyle.CurrentHorizontalScale;

        /// <summary>
        /// Gets the direction of the view grid of a View Style.
        /// </summary>
        public string Direction => AeccGraphStyle.Direction.ToString();

        /// <summary>
        /// Gets the View Title Style of a View Style, which specifies title text display properties.
        /// </summary>
        public ViewTitleStyle TitleStyle => new ViewTitleStyle(this, AeccGraphStyle.TitleStyle);

        /// <summary>
        /// Gets the vertical exaggeration factor of a View Style.
        /// </summary>
        public double VerticalExaggeration => AeccGraphStyle.VerticalExaggeration;

        /// <summary>
        /// Gets the vertical scale of a View Style.
        /// </summary>
        public double VerticalScale => AeccGraphStyle.VerticalScale;
        #endregion

        #region constructors
        internal ViewStyle(Style parentStyle, AeccGraphStyle aeccGraphStyle)
        {
            ParentStyle = parentStyle;
            AeccGraphStyle = aeccGraphStyle;
        }

        #endregion

        #region methods
        public override string ToString() => $"ViewStyle(Parent Style = {ParentStyle.Name})";

        protected ViewStyle SetValue(object value, [CallerMemberName] string methodName = null)
        {
            if (methodName.StartsWith("Set"))
            {
                methodName = methodName.Substring(3);
            }
            return SetValue(methodName, value);
        }

        protected ViewStyle SetValue(string propertyName, object value)
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccParentStyle = ctx.Transaction.GetObject(ParentStyle.AeccStyle.ObjectId, acDb.OpenMode.ForWrite);
                    PropertyInfo propInfo = AeccGraphStyle.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                    propInfo?.SetValue(AeccGraphStyle, value);
                    return this;
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// Sets the direction of the view grid of a View Style.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public ViewStyle SetDirection(string direction) => SetValue(direction);

        /// <summary>
        /// Sets the vertical exaggeration factor of a View Style.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ViewStyle SetVerticalExaggeration(double value) => SetValue(value);

        /// <summary>
        /// Sets the vertical scale of a View Style.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ViewStyle SetVerticalScale(double value) => SetValue(value);
        #endregion
    }
}