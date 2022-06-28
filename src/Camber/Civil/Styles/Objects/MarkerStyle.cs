#region references
using System;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acGeom = Autodesk.AutoCAD.Geometry;
using civDb = Autodesk.Civil.DatabaseServices;
using Autodesk.DesignScript.Geometry;
using AeccMarkerStyle = Autodesk.Civil.DatabaseServices.Styles.MarkerStyle;
using Camber.Utilities.GeometryConversions;
#endregion

namespace Camber.Civil.Styles.Objects
{
    public sealed class MarkerStyle : Style
    {
        #region properties
        internal AeccMarkerStyle AeccMarkerStyle => AcObject as AeccMarkerStyle;

        /// <summary>
        /// Gets the long value specifying a custom marker style type.
        /// </summary>
        public string CustomMarkerStyle => AeccMarkerStyle.CustomMarkerStyle.ToString();

        /// <summary>
        /// Gets the value that specifies the shape superimposed on the custom point style marker as either none, circle, square, or both square and circle.
        /// </summary>
        public string CustomMarkerSuperimposeStyle => AeccMarkerStyle.CustomMarkerSuperimposeStyle.ToString();

        /// <summary>
        /// Gets the fixed scale of the marker represented as a Dynamo Point. The X, Y, and Z coordinates of the point correspond with the appropriate scale factors.
        /// </summary>
        public Point MarkerFixedScale => GeometryConversions.AcPointToDynPoint(AeccMarkerStyle.MarkerFixedScale);

        /// <summary>
        /// Gets the marker rotation angle from a Marker Style.
        /// </summary>
        public double MarkerRotationAngle => GetDouble();

        /// <summary>
        /// Gets the marker size from a Marker Style in drawing units.
        /// </summary>
        public double MarkerSize => GetDouble();

        /// <summary>
        /// Gets the marker symbol name from a Marker Style.
        /// </summary>
        public string MarkerSymbolName => GetString();

        /// <summary>
        /// Gets the marker type from a Marker Style (point, symbol, vertical line, or custom).
        /// </summary>
        public string MarkerType => AeccMarkerStyle.MarkerType.ToString();

        /// <summary>
        /// Gets the value that defines how the marker rotation angle is applied in a Marker Style.
        /// </summary>
        public string Orientation => AeccMarkerStyle.Orientation.ToString();

        /// <summary>
        /// Gets the values that defines how the marker is sized in a Marker Style.
        /// </summary>
        public string SizeType => AeccMarkerStyle.SizeType.ToString();
        #endregion

        #region constructors
        internal MarkerStyle(AeccMarkerStyle aeccMarkerStyle, bool isDynamoOwned = false) : base(aeccMarkerStyle, isDynamoOwned) { }

        internal static MarkerStyle GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<MarkerStyle, AeccMarkerStyle>
            (styleId, (style) => new MarkerStyle(style));
        #endregion

        #region methods
        public override string ToString() => $"MarkerStyle(Name = {Name})";

        /// <summary>
        /// Sets the long value specifying a custom marker style type.
        /// </summary>
        /// <param name="customMarkerStyle"></param>
        /// <returns></returns>
        public MarkerStyle SetCustomMarkerStyle(string customMarkerStyle) => (MarkerStyle)SetValue(Enum.Parse(typeof(civDb.Styles.CustomMarkerType), customMarkerStyle));

        /// <summary>
        /// Sets the value that specifies the shape superimposed on the custom point style marker as either none, circle, square, or both square and circle.
        /// </summary>
        /// <param name="superimposeStyle"></param>
        /// <returns></returns>
        public MarkerStyle SetCustomMarkerSuperimposeStyle(string superimposeStyle) => (MarkerStyle)SetValue(
            Enum.Parse(typeof(civDb.Styles.CustomMarkerSuperimposeType), 
                superimposeStyle));

        /// <summary>
        /// Sets the fixed scale of the marker in a Marker Style by X, Y, and Z scale factors.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public MarkerStyle SetMarkerFixedScale(double x, double y, double z) => (MarkerStyle)SetValue(new acGeom.Point3d(x, y, z));

        /// <summary>
        /// Sets the marker rotation angle for a Marker Style.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public MarkerStyle SetMarkerRotationAngle(double angle) => (MarkerStyle)SetValue(angle);

        /// <summary>
        /// Sets the marker size for a Marker Style in drawing units.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public MarkerStyle SetMarkerSize(double size) => (MarkerStyle)SetValue(size);

        /// <summary>
        /// Sets the marker symbol name for a Marker Style.
        /// </summary>
        /// <param name="symbolName"></param>
        /// <returns></returns>
        public MarkerStyle SetMarkerSymbolName(string symbolName) => (MarkerStyle)SetValue(symbolName);

        /// <summary>
        /// Sets the marker type from a Marker Style (point, symbol, vertical line, or custom).
        /// </summary>
        /// <param name="markerType"></param>
        /// <returns></returns>
        public MarkerStyle SetMarkerType(string markerType) => (MarkerStyle)SetValue(Enum.Parse(typeof(civDb.Styles.MarkerDisplayType), markerType));

        /// <summary>
        /// Sets the value that defines how the marker rotation angle is applied in a Marker Style.
        /// </summary>
        /// <param name="orientationType"></param>
        /// <returns></returns>
        public MarkerStyle SetOrientation(string orientationType) => (MarkerStyle)SetValue(Enum.Parse(typeof(civDb.Styles.MarkerOrientationType), orientationType));

        /// <summary>
        /// Sets the values that defines how the marker is sized in a Marker Style.
        /// </summary>
        /// <param name="sizeType"></param>
        /// <returns></returns>
        public MarkerStyle SetSizeType(string sizeType) => (MarkerStyle)SetValue(Enum.Parse(typeof(civDb.Styles.MarkerSizeType), sizeType));
        #endregion
    }
}
