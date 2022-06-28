#region references
using AeccPressurePartConnection = Autodesk.Civil.DatabaseServices.PressurePartConnection;
using Autodesk.DesignScript.Geometry;
using Camber.Utilities.GeometryConversions;
#endregion

namespace Camber.Civil.PressureNetworks.Parts
{
    public sealed class PressurePartConnection
    {
        #region properties
        internal AeccPressurePartConnection AeccPressurePartConnection { get; set; }

        /// <summary>
        /// Gets the port index of the Pressure Part Connection.
        /// </summary>
        public int ConnectedIndex => AeccPressurePartConnection.ConnectedIndex;

        /// <summary>
        /// Gets the deflection of the Pressure Part Connection.
        /// </summary>
        public double Deflection => AeccPressurePartConnection.Deflection;

        /// <summary>
        /// Gets the units of the Pressure Part Connection's deflection .
        /// </summary>
        public string DeflectionUnit => AeccPressurePartConnection.DeflectionUnit;

        /// <summary>
        /// Gets the direction vector of the Pressure Part Connection.
        /// </summary>
        public Vector Direction => GeometryConversions.AcVectorToDynamoVector(AeccPressurePartConnection.Direction);

        /// <summary>
        /// Gets the engagement length of the Pressure Part Connection.
        /// </summary>
        public double EngagementLength => AeccPressurePartConnection.EngagementLength;

        /// <summary>
        /// Gets the units of the engagement length for the Pressure Part Connection.
        /// </summary>
        public string EngagementLengthUnit => AeccPressurePartConnection.EngagementLengthUnit;

        /// <summary>
        /// Gets the joint end type of the Pressure Part Connection.
        /// </summary>
        public string JointEndType => AeccPressurePartConnection.JointEndType;

        /// <summary>
        /// Gets the nominal diameter of the Pressure Part Connection.
        /// </summary>
        public double NominalDiameter => AeccPressurePartConnection.NominalDiameter;

        /// <summary>
        /// Gets the units of the nominal diameter of the Pressure Part Connection.
        /// </summary>
        public string NominalDiameterUnit => AeccPressurePartConnection.NominalDiameterUnit;

        /// <summary>
        /// Gets whether the Pressure Part Connection is open or closed.
        /// </summary>
        public bool IsOpen => AeccPressurePartConnection.Open;

        /// <summary>
        /// Gets the outside diameter of the Pressure Part Connection.
        /// </summary>
        public double OutsideDiameter => AeccPressurePartConnection.OutsideDiameter;

        /// <summary>
        /// Gets the units of the outside diameter of the Pressure Part Connection.
        /// </summary>
        public string OutsideDiameterUnit => AeccPressurePartConnection.OutsideDiameterUnit;

        /// <summary>
        /// Gets the position of the Pressure Part Connection.
        /// </summary>
        public Point Position => GeometryConversions.AcPointToDynPoint(AeccPressurePartConnection.Position);

        /// <summary>
        /// Gets the wall thickness of the Pressure Part Connection.
        /// </summary>
        public double WallThickness => AeccPressurePartConnection.WallThickness;

        /// <summary>
        /// Gets the units of the wall thickness of the Pressure Part Connection.
        /// </summary>
        public string WallThicknessUnit => AeccPressurePartConnection.WallThicknessUnit;
        #endregion

        #region constructors
        internal PressurePartConnection(AeccPressurePartConnection aeccPressurePartConnection)
        {
            AeccPressurePartConnection = aeccPressurePartConnection;
        }
        #endregion

        #region methods
        public override string ToString() => $"PressurePartConnection(Index = {ConnectedIndex})";
        #endregion
    }
}
