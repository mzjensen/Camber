#region references
using System;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccAlignmentStyle = Autodesk.Civil.DatabaseServices.Styles.AlignmentStyle;
using DynamoServices;
#endregion

namespace Camber.Civil.Styles.Objects
{
    [RegisterForTrace]
    public sealed class AlignmentStyle : Style
    {
        #region properties
        internal AeccAlignmentStyle AeccAlignmentStyle => AcObject as AeccAlignmentStyle;

        /// <summary>
        /// Gets whether curve snapping is enabled when changing the curve radius value of an Alignment Style.
        /// </summary>
        public bool EnableRadiusSnap => GetBool();

        /// <summary>
        /// Gets the even increment value by which the curve radius can change using snapping in an Alignment Style.
        /// </summary>
        public double RadiusSnapValue => GetDouble();

        #endregion

        #region constructors
        internal AlignmentStyle(AeccAlignmentStyle aeccAlignmentStyle, bool isDynamoOwned = false) : base(aeccAlignmentStyle, isDynamoOwned) { }

        internal static AlignmentStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<AlignmentStyle, AeccAlignmentStyle>
            (styleId, (style) => new AlignmentStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"AlignmentStyle(Name = {Name})";

        /// <summary>
        /// Gets a Marker Style by type from an Alignment Style definition.
        /// </summary>
        /// <param name="markerStyleType"></param>
        /// <returns></returns>
        public MarkerStyle GetMarkerStyleByType(string markerStyleType)
        {
            string name = markerStyleType.ToString() + "MarkerStyle";
            if(!Enum.IsDefined(typeof(AlignmentMarkerStyles), markerStyleType)) { throw new ArgumentException("Invalid marker style type."); }

            try
            {
                var markerStyleId = (acDb.ObjectId)AeccAlignmentStyle.GetType().GetProperty(name).GetValue(AeccAlignmentStyle, null);
                
                // Need to check for null ID in this case because it is possible to set a marker style to '<none>' in the UI.
                if (markerStyleId.IsValid && !markerStyleId.IsErased)
                {
                    return MarkerStyle.GetByObjectId(markerStyleId);
                }
                return null;
            }
            catch { throw; }
            
        }

        /// <summary>
        /// Sets a Marker Style by type in an Alignment Style definition.
        /// </summary>
        /// <param name="markerStyleType">The type of Marker Style to set</param>
        /// <param name="markerStyle">The new Marker Style</param>
        /// <returns></returns>
        public AlignmentStyle SetMarkerStyleByType(string markerStyleType, MarkerStyle markerStyle)
        {
            string name = markerStyleType.ToString() + "MarkerStyle";
            if (!Enum.IsDefined(typeof(AlignmentMarkerStyles), markerStyleType)) { throw new ArgumentException("Invalid marker style type."); }

            try
            {
                var markerStyleId = markerStyle.InternalObjectId;
                if (markerStyleId.IsValid && !markerStyleId.IsErased)
                {
                    AeccAlignmentStyle.GetType().GetProperty(name).SetValue(AeccAlignmentStyle, markerStyleId, null);
                    return this;
                }
                return null;
            }
            catch { throw; }
        }

        /// <summary>
        /// Sets whether curve snapping is enabled when changing the curve radius value of an Alignment Style.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public AlignmentStyle SetRadiusSnapValue(double value) => (AlignmentStyle)SetValue(value);


        /// <summary>
        /// Sets the even increment value by which the curve radius can change using snapping in an Alignment Style.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public AlignmentStyle SetEnableRadiusSnap(bool @bool) => (AlignmentStyle)SetValue(@bool);
        #endregion
    }
}
