#region references
using System.Reflection;
using System.Runtime.CompilerServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using AeccDisplayStyle = Autodesk.Civil.DatabaseServices.Styles.DisplayStyle;
#endregion


namespace Camber.Civil.Styles
{
    public sealed class DisplayStyle
    {
        #region properties
        private AeccDisplayStyle AeccDisplayStyle { get; set; }
        
        /// <summary>
        /// Gets the Style that a Display Style belongs to.
        /// </summary>
        public Style ParentStyle { get; set; }

        /// <summary>
        /// Gets the Layer of a Display Style.
        /// </summary>
        public acDynNodes.Layer Layer => acDynNodes.Document.Current.LayerByName(AeccDisplayStyle.Layer);

        /// <summary>
        /// Gets the linetype of a Display Style.
        /// </summary>
        public string Linetype => AeccDisplayStyle.Linetype;

        /// <summary>
        /// Gets the linetype scale of a Display Style.
        /// </summary>
        public double LinetypeScale => AeccDisplayStyle.LinetypeScale;

        /// <summary>
        /// Gets the plot style of a Display Style.
        /// </summary>
        public string PlotStyle => AeccDisplayStyle.PlotStyle;

        /// <summary>
        /// Gets the visibilty of a Display Style.
        /// </summary>
        public bool IsVisible => AeccDisplayStyle.Visible;

        /// <summary>
        /// Gets the view direction of a Display Style.
        /// </summary>
        public string ViewDirection { get; set; }

        /// <summary>
        /// Gets the name of a Display Style.
        /// </summary>
        public string Name { get; set; }
        #endregion

        #region constructors
        internal DisplayStyle(Style parentStyle, AeccDisplayStyle aeccDisplayStyle) 
        {
            ParentStyle = parentStyle;
            AeccDisplayStyle = aeccDisplayStyle;
        }

        #endregion

        #region methods
        public override string ToString() => $"DisplayStyle(Name = {Name}, View Direction = {ViewDirection})";
        
        protected DisplayStyle SetValue(object value, [CallerMemberName] string methodName = null)
        {
            if (methodName.StartsWith("Set"))
            {
                methodName = methodName.Substring(3);
            }
            return SetValue(methodName, value);
        }

        private DisplayStyle SetValue(string propertyName, object value)
        {
            try
            {
                bool openedForWrite = ParentStyle.AeccStyle.IsWriteEnabled;
                if (!openedForWrite) ParentStyle.AeccStyle.UpgradeOpen();
                PropertyInfo propInfo = AeccDisplayStyle.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                propInfo?.SetValue(AeccDisplayStyle, value);
                if (!openedForWrite) ParentStyle.AeccStyle.DowngradeOpen();
                return this;
            }
            catch { return null; }
        }

        /// <summary>
        /// Sets the layer of a Display Style.
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public DisplayStyle SetLayer(string layer) => SetValue(layer);

        /// <summary>
        /// Sets the linetype of a Display Style.
        /// </summary>
        /// <param name="linetype"></param>
        /// <returns></returns>
        public DisplayStyle SetLinetype(string linetype) => SetValue(linetype);

        /// <summary>
        /// Sets the linetype scale of a Display Style.
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        public DisplayStyle SetLinetypeScale(double scale) => SetValue(scale);

        /// <summary>
        /// Sets the plot style of a Display Style.
        /// </summary>
        /// <param name="plotStyle"></param>
        /// <returns></returns>
        public DisplayStyle SetPlotStyle(string plotStyle) => SetValue(plotStyle);

        /// <summary>
        /// Sets the visibility of a Display Style.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public DisplayStyle SetVisibility(bool @bool) => SetValue(@bool, "Visible");
        #endregion
    }
}