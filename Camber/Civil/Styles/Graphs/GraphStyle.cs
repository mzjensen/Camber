#region references
using System.Reflection;
using System.Runtime.CompilerServices;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using AeccGraphStyle = Autodesk.Civil.DatabaseServices.Styles.GraphStyle;
#endregion


namespace Camber.Civil.Styles.Graphs
{
    public sealed class GraphStyle
    {
        #region properties
        private AeccGraphStyle AeccGraphStyle { get; set; }

        /// <summary>
        /// Gets the Style that a Graph Style belongs to.
        /// </summary>
        public Style ParentStyle { get; set; }

        /// <summary>
        /// Gets the plotted size of the horizontal scale of a Graph Style.
        /// </summary>
        public double CurrentHorizontalScale => AeccGraphStyle.CurrentHorizontalScale;

        /// <summary>
        /// Gets the direction of the view grid of a Graph Style.
        /// </summary>
        public string Direction => AeccGraphStyle.Direction.ToString();

        /// <summary>
        /// Gets the Graph Title Style of a Graph Style, which specifies title text display properties.
        /// </summary>
        public GraphTitleStyle TitleStyle => new GraphTitleStyle(this, AeccGraphStyle.TitleStyle);

        /// <summary>
        /// Gets the vertical exaggeration factor of a Graph Style.
        /// </summary>
        public double VerticalExaggeration => AeccGraphStyle.VerticalExaggeration;

        /// <summary>
        /// Gets the vertical scale of a Graph Style.
        /// </summary>
        public double VerticalScale => AeccGraphStyle.VerticalScale;
        #endregion

        #region constructors
        internal GraphStyle(Style parentStyle, AeccGraphStyle aeccGraphStyle)
        {
            ParentStyle = parentStyle;
            AeccGraphStyle = aeccGraphStyle;
        }

        #endregion

        #region methods
        public override string ToString() => $"GraphStyle(Parent Style = {ParentStyle.Name})";

        protected GraphStyle SetValue(object value, [CallerMemberName] string methodName = null)
        {
            if (methodName.StartsWith("Set"))
            {
                methodName = methodName.Substring(3);
            }
            return SetValue(methodName, value);
        }

        protected GraphStyle SetValue(string propertyName, object value)
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
        /// Sets the direction of the view grid of a Graph Style.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public GraphStyle SetDirection(string direction) => SetValue(direction);

        /// <summary>
        /// Sets the vertical exaggeration factor of a Graph Style.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public GraphStyle SetVerticalExaggeration(double value) => SetValue(value);

        /// <summary>
        /// Sets the vertical scale of a Graph Style.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public GraphStyle SetVerticalScale(double value) => SetValue(value);
        #endregion
    }
}