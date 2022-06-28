#region references
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Camber.Utilities.GeometryConversions;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AcDBPoint = Autodesk.AutoCAD.DatabaseServices.DBPoint;
#endregion

namespace Camber.External.ExternalObjects
{
    public sealed class ExternalDBPoint : ExternalObject
    {
        #region properties
        internal AcDBPoint AcEntity => AcObject as AcDBPoint;

        /// <summary>
        /// Gets the X coordinate of a DBPoint.
        /// </summary>
        public double X => AcEntity.Position.X;

        /// <summary>
        /// Gets the Y coordinate of a DBPoint.
        /// </summary>
        public double Y => AcEntity.Position.Y;

        /// <summary>
        /// Gets the Z coordinate of a DBPoint.
        /// </summary>
        public double Z => AcEntity.Position.Z;
        #endregion

        #region constructors
        [SupressImportIntoVM]
        internal static ExternalDBPoint GetByObjectId(acDb.ObjectId oid)
            => Get<ExternalDBPoint, AcDBPoint>
            (oid, (dbPoint) => new ExternalDBPoint(dbPoint));

        internal ExternalDBPoint(AcDBPoint acDbPoint) : base(acDbPoint) { }
        #endregion

        #region methods
        public override string ToString() => $"ExternalDBPoint(X = {X:F3}, Y = {Y:F3}, Z = {Z:F3})";

        [IsVisibleInDynamoLibrary(false)]
        public override Geometry Geometry() => GeometryConversions.AcPointToDynPoint(AcEntity.Position);
        #endregion
    }
}
