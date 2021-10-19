#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using civStyles = Autodesk.Civil.DatabaseServices.Styles;
using AeccInterferenceStyle = Autodesk.Civil.DatabaseServices.Styles.InterferenceStyle;
#endregion

namespace Camber.Civil.Styles.Objects
{
    public sealed class InterferenceStyle : Style
    {
        #region properties
        internal AeccInterferenceStyle AeccInterferenceStyle => AcObject as AeccInterferenceStyle;

        /// <summary>
        /// Gets the absolute model size for an Interference Style.
        /// </summary>
        public double AbsoluteModelSize => GetDouble();

        /// <summary>
        /// Gets the drawing scale model size for an Interference Style.
        /// </summary>
        public double DrawingScaleModelSize => GetDouble();

        /// <summary>
        /// Gets the Marker Style for an Interference Style.
        /// </summary>
        public MarkerStyle MarkerStyle
        {
            get
            {
                var markerStyleId = AeccInterferenceStyle.MarkerStyle;
                if (markerStyleId.IsValid && !markerStyleId.IsErased)
                {
                    return MarkerStyle.GetByObjectId(markerStyleId);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the model options for an Interference Style.
        /// </summary>
        public string ModelOptions => AeccInterferenceStyle.ModelOptions.ToString();

        /// <summary>
        /// Gets the model size options for an Interference Style.
        /// </summary>
        public string ModelSizeOptions => AeccInterferenceStyle.ModelSizeOptions.ToString();

        /// <summary>
        /// Gets the interference size type for an Interference Style.
        /// </summary>
        public string ModelSizeType => AeccInterferenceStyle.ModelSizeType.ToString();
        #endregion

        #region constructors
        internal InterferenceStyle(AeccInterferenceStyle aeccInterferenceStyle, bool isDynamoOwned = false) : base(aeccInterferenceStyle, isDynamoOwned) { }

        internal static InterferenceStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<InterferenceStyle, AeccInterferenceStyle>
            (styleId, (style) => new InterferenceStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"InterferenceStyle(Name = {Name})";

        /// <summary>
        /// Sets the absolute model size for an Interference Style.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public InterferenceStyle SetAbsoluteModelSize(double size) => (InterferenceStyle)SetValue(size);

        /// <summary>
        /// Sets the drawing scale model size for an Interference Style.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public InterferenceStyle SetDrawingScaleModelSize(double size) => (InterferenceStyle)SetValue(size);

        /// <summary>
        /// Sets the Marker Style for an Interference Style.
        /// </summary>
        /// <param name="markerStyle"></param>
        /// <returns></returns>
        public InterferenceStyle SetMarkerStyle(MarkerStyle markerStyle) => (InterferenceStyle)SetValue(markerStyle.AeccMarkerStyle.ObjectId);

        /// <summary>
        /// Sets the model options for an Interference Style.
        /// </summary>
        /// <param name="useTrueSolid">True = show true interference solid, False = show as sphere</param>
        /// <returns></returns>
        public InterferenceStyle SetModelOptions(bool useTrueSolid)
        {
            var modelOptions = civStyles.InterferenceModelType.Sphere;
            if (useTrueSolid)
            {
                modelOptions = civStyles.InterferenceModelType.TrueSolid;
            }
            SetValue(modelOptions);
            return this;
        }

        /// <summary>
        /// Sets the model size options for an Interference Style.
        /// </summary>
        /// <param name="useAbsoluteUnits">True = use size in absolute units, False = use size in drawing units</param>
        /// <returns></returns>
        public InterferenceStyle SetModelSizeOptions(bool useAbsoluteUnits)
        {
            var modelSizeOptions = civStyles.InterferenceModelSizeOptionType.UseDrawingUnits;
            if (useAbsoluteUnits)
            {
                modelSizeOptions = civStyles.InterferenceModelSizeOptionType.UseAbsoluteUnits;
            }
            SetValue(modelSizeOptions);
            return this;
        }

        /// <summary>
        /// Sets the interference size type for an Interference Style.
        /// </summary>
        /// <param name="useSolidExtents">True = diameter by true solid extents, False = user-specified diameter</param>
        /// <returns></returns>
        public InterferenceStyle SetModelSizeType(bool useSolidExtents)
        {
            var modelSizeType = civStyles.InterferenceModelSizeType.UserSpecified;
            if (useSolidExtents)
            {
                modelSizeType = civStyles.InterferenceModelSizeType.SolidExtents;
            }
            SetValue(modelSizeType);
            return this;
        }
        #endregion
    }
}
