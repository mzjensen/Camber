#region references

using acDb = Autodesk.AutoCAD.DatabaseServices;
using acGeom = Autodesk.AutoCAD.Geometry;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using AcDimension = Autodesk.AutoCAD.DatabaseServices.Dimension;
using DynamoServices;
using Autodesk.DesignScript.Runtime;
using Camber.Utilities;
#endregion

namespace Camber.AutoCAD.Objects.Dimensions
{
    [RegisterForTrace]
    public class Dimension : Object
    {
        #region properties
        internal AcDimension AcDimension => AcObject as AcDimension;

        /// <summary>
        /// Gets the style of a Dimension.
        /// </summary>
        public string DimensionStyle => GetString("DimensionStyleName");

        /// <summary>
        /// Gets the current measurement of a Dimension.
        /// </summary>
        public double Measurement => GetDouble();

        /// <summary>
        /// Gets the prefix of a Dimension.
        /// </summary>
        public string Prefix => GetString();

        /// <summary>
        /// Gets the suffix of a Dimension.
        /// </summary>
        public string Suffix => GetString();

        /// <summary>
        /// Gets the user-supplied annotation text string of a Dimension.
        /// </summary>
        public string TextOverride => GetString("DimensionText");

        /// <summary>
        /// Gets the rotation angle (in degrees) to the horizontal axis used by the text in a Dimension.
        /// </summary>
        public double TextRotation => MathUtilities.RadiansToDegrees(AcDimension.TextRotation);
        #endregion

        #region constructors
        [SupressImportIntoVM]
        internal static Dimension GetByObjectId(acDb.ObjectId dimId)
            => ObjectSupport.Get<Dimension, AcDimension>
            (dimId, (dimension) => new Dimension(dimension));

        internal Dimension(AcDimension acDim, bool isDynamoOwned = false) : base(acDim, isDynamoOwned) { }
        #endregion

        #region methods
        public override string ToString() => $"Dimension(Measurement = {Measurement:F3})";

        /// <summary>
        /// Sets the style of a Dimension.
        /// </summary>
        /// <param name="styleName"></param>
        /// <returns></returns>
        public Dimension SetDimensionStyle(string styleName)
        {
            SetValue((object)styleName, "DimensionStyleName");
            return this;
        }

        /// <summary>
        /// Sets the prefix of a Dimension.
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public Dimension SetPrefix(string prefix)
        {
            SetValue((object)prefix);
            return this;
        }

        /// <summary>
        /// Sets the suffix of a Dimension.
        /// </summary>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public Dimension SetSuffix(string suffix)
        {
            SetValue((object)suffix);
            return this;
        }

        /// <summary>
        /// Sets the user-supplied annotation text string of a Dimension. 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public Dimension SetTextOverride(string text)
        {
            SetValue((object)text, "DimensionText");
            return this;
        }

        /// <summary>
        /// Sets the rotation angle (in degrees) to the horizontal axis used by the text in a Dimension.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public Dimension SetTextRotation(double angle)
        {
            SetValue(MathUtilities.DegreesToRadians(angle));
            return this;
        }
        #endregion
    }
}
