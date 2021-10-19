#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccAssemblyStyle = Autodesk.Civil.DatabaseServices.Styles.AssemblyStyle;
#endregion

namespace Camber.Civil.Styles.Objects
{
    public sealed class AssemblyStyle : Style
    {
        #region properties
        internal AeccAssemblyStyle AeccAssemblyStyle => AcObject as AeccAssemblyStyle;

        /// <summary>
        /// Gets the Marker Style for the insertion point component of an Assembly Style.
        /// </summary>
        public MarkerStyle InsertionPointMarkerStyle => GetMarkerStyleByComponent("MarkerStyleAtAssemblyOriginId");

        /// <summary>
        /// Gets the Marker Style for the baseline component of an Assembly Style.
        /// </summary>
        public MarkerStyle BaselineMarkerStyle => GetMarkerStyleByComponent("MarkerStyleAtMainBaselineId");

        /// <summary>
        /// Gets the Marker Style for the baseline point component of an Assembly Style.
        /// </summary>
        public MarkerStyle BaselinePointMarkerStyle => GetMarkerStyleByComponent("MarkerStyleAtMainBaselineOriginId");

        /// <summary>
        /// Gets the Marker Style for the offset component of an Assembly Style.
        /// </summary>
        public MarkerStyle OffsetMarkerStyle => GetMarkerStyleByComponent("MarkerStyleAtOffsetBaselineId");
        
        /// <summary>
        /// Gets the Marker Style for the offset point component of an Assembly Style.
        /// </summary>
        public MarkerStyle OffsetPointMarkerStyle => GetMarkerStyleByComponent("MarkerStyleAtOffsetBaselineOriginId");
        #endregion

        #region constructors
        internal AssemblyStyle(AeccAssemblyStyle aeccAssemblyStyle, bool isDynamoOwned = false) : base(aeccAssemblyStyle, isDynamoOwned) { }

        internal static AssemblyStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<AssemblyStyle, AeccAssemblyStyle>
            (styleId, (style) => new AssemblyStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"AssemblyStyle(Name = {Name})";

        /// <summary>
        /// Gets a Marker Style by component from an Assembly Style.
        /// </summary>
        /// <param name="markerStyleComponent"></param>
        /// <returns></returns>
        private MarkerStyle GetMarkerStyleByComponent(string markerStyleComponent)
        {
            //TODO: This is returning really weird names for the marker styles.
            var markerStyleId = (acDb.ObjectId)AeccAssemblyStyle.GetType().GetProperty(markerStyleComponent).GetValue(AeccAssemblyStyle, null);
            if (markerStyleId.IsValid && !markerStyleId.IsErased)
            {
                return MarkerStyle.GetByObjectId(markerStyleId);
            }
            return null;
        }
        #endregion
    }
}
