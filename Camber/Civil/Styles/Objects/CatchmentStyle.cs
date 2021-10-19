#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using AeccCatchmentStyle = Autodesk.Civil.DatabaseServices.Styles.CatchmentStyle;
#endregion

namespace Camber.Civil.Styles.Objects
{
    public sealed class CatchmentStyle : Style
    {
        #region properties
        internal AeccCatchmentStyle AeccCatchmentStyle => AcObject as AeccCatchmentStyle;

        /// <summary>
        /// Gets the Marker Style for the discharge point of a Catchment Style.
        /// </summary>
        public MarkerStyle DischargePointMarkerStyle => GetMarkerStyleByType("DischargePointMarkerStyle");

        /// <summary>
        /// Gets the Marker Style for the flow segment start point of a Catchment Style.
        /// </summary>
        public MarkerStyle FlowSegmentStartPointMarkerStyle => GetMarkerStyleByType("FlowSegmentStartPointMarkerStyle");

        /// <summary>
        /// Gets the Marker Style for the hydraulically most-distant point of a Catchment Style.
        /// </summary>
        public MarkerStyle MostDistantPointMarkerStyle => GetMarkerStyleByType("MostDistantPointMarkerStyle");
        #endregion

        #region constructors
        internal CatchmentStyle(AeccCatchmentStyle aeccCatchmentStyle, bool isDynamoOwned = false) : base(aeccCatchmentStyle, isDynamoOwned) { }

        internal static CatchmentStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<CatchmentStyle, AeccCatchmentStyle>
            (styleId, (style) => new CatchmentStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"CatchmentStyle(Name = {Name})";

        /// <summary>
        /// Sets the Marker Style for the discharge point of a Catchment Style.
        /// </summary>
        /// <param name="markerStyle"></param>
        /// <returns></returns>
        public CatchmentStyle SetDischargePointMarkerStyle(MarkerStyle markerStyle) => SetMarkerStyleByType("DischargePointMarkerStyle", markerStyle);

        /// <summary>
        /// Sets the Marker Style for the flow segment start point of a Catchment Style.
        /// </summary>
        /// <param name="markerStyle"></param>
        /// <returns></returns>
        public CatchmentStyle SetFlowSegmentStartPointMarkerStyle(MarkerStyle markerStyle) => SetMarkerStyleByType("FlowSegmentStartPointMarkerStyle", markerStyle);

        /// <summary>
        /// Sets the Marker Style for the hydraulically most-distant point of a Catchment Style.
        /// </summary>
        /// <param name="markerStyle"></param>
        /// <returns></returns>
        public CatchmentStyle SetMostDistantPointMarkerStyle(MarkerStyle markerStyle) => SetMarkerStyleByType("MostDistantPointMarkerStyle", markerStyle);

        /// <summary>
        /// Gets a Marker Style by type within the Catchment Style definition.
        /// </summary>
        /// <param name="markerStyleType"></param>
        /// <returns></returns>
        protected MarkerStyle GetMarkerStyleByType(string markerStyleType)
        {
            try
            {
                var markerStyleId = (acDb.ObjectId)AeccCatchmentStyle.GetType().GetProperty(markerStyleType).GetValue(AeccCatchmentStyle, null);
                if (markerStyleId.IsValid && !markerStyleId.IsErased)
                {
                    return MarkerStyle.GetByObjectId(markerStyleId);
                }
                return null;
            }
            catch { throw; }
        }

        /// <summary>
        /// Sets a Marker Style by type within the Catchment Style definition.
        /// </summary>
        /// <param name="markerStyleType">The type of Marker Style to set</param>
        /// <param name="markerStyle">The new Marker Style</param>
        /// <returns></returns>
        protected CatchmentStyle SetMarkerStyleByType(string markerStyleType, MarkerStyle markerStyle)
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccStyle = ctx.Transaction.GetObject(AeccCatchmentStyle.ObjectId, acDb.OpenMode.ForWrite);
                    var markerStyleId = markerStyle.InternalObjectId;
                    if (markerStyleId.IsValid && !markerStyleId.IsErased)
                    {
                        aeccStyle.GetType().GetProperty(markerStyleType).SetValue(aeccStyle, markerStyleId, null);
                        return this;
                    }
                    return null;
                }
            }
            catch { throw; }
        }
        #endregion
    }
}
