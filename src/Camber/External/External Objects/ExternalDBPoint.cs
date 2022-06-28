using System;
using Autodesk.AutoCAD.Geometry;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Camber.Utilities.GeometryConversions;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AcDBPoint = Autodesk.AutoCAD.DatabaseServices.DBPoint;

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

        /// <summary>
        /// Gets the thickness value of a DBPoint.
        /// The thickness is the point's dimension along its normal vector direction (sometimes called the extrusion direction).
        /// </summary>
        public double Thickness => AcEntity.Thickness;
        #endregion

        #region constructors
        [SupressImportIntoVM]
        internal static ExternalDBPoint GetByObjectId(acDb.ObjectId oid)
            => Get<ExternalDBPoint, AcDBPoint>
            (oid, (dbPoint) => new ExternalDBPoint(dbPoint));

        internal ExternalDBPoint(AcDBPoint acDbPoint) : base(acDbPoint) { }

        /// <summary>
        /// Creates a new External DBPoint by coordinates.
        /// </summary>
        /// <param name="block">The destination block for the DBPoint.</param>
        /// <param name="layer">The layer that the DBPoint will reside on.</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static ExternalDBPoint ByCoordinates(ExternalBlock block, string layer, double x, double y, double z)
        {
            if (string.IsNullOrEmpty(layer)) { throw new InvalidOperationException("Layer is null or empty."); }

            ExternalDBPoint retPnt;

            acDb.Database destDb = block.AcBlock.Database;
            ExternalDocument destDoc = new ExternalDocument(destDb, destDb.Filename);

            using (var tr = destDb.TransactionManager.StartTransaction())
            {
                try
                {
                    AcDBPoint acDbPnt = new AcDBPoint(new Point3d(x, y, z));
                    acDb.BlockTableRecord btr = (acDb.BlockTableRecord)tr.GetObject(block.InternalObjectId, acDb.OpenMode.ForWrite);
                    btr.AppendEntity(acDbPnt);
                    tr.AddNewlyCreatedDBObject(acDbPnt, true);
                    ExternalDocument.EnsureLayer(destDoc, layer);
                    acDbPnt.Layer = layer;
                    retPnt = new ExternalDBPoint(acDbPnt);
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException(e.Message);
                }
                tr.Commit();
            }
            return retPnt;
        }

        /// <summary>
        /// Creates a new External DBPoint from a Dynamo point.
        /// </summary>
        /// <param name="block">The destination block for the DBPoint.</param>
        /// <param name="layer">The layer that the DBPoint will reside on.</param>
        /// <param name="point">The Dynamo point.</param>
        /// <returns></returns>
        public static ExternalDBPoint ByPoint(ExternalBlock block, string layer, Point point) =>
            ByCoordinates(block, layer, point.X, point.Y, point.Z);
        #endregion

        #region methods
        public override string ToString() => $"ExternalDBPoint(X = {X:F3}, Y = {Y:F3}, Z = {Z:F3})";

        [IsVisibleInDynamoLibrary(false)]
        public override Geometry Geometry() => GeometryConversions.AcPointToDynPoint(AcEntity.Position);

        /// <summary>
        /// Sets the position of an External DBPoint.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public ExternalDBPoint SetPosition(double x, double y, double z)
        {
            try
            {
                AcEntity.UpgradeOpen();
                var newPnt = new Point3d(x, y, z);
                AcEntity.Position = newPnt;
                AcEntity.DowngradeOpen();
                return this;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        /// <summary>
        /// Sets the thickness value for an External DBPoint.
        /// </summary>
        /// <param name="thickness">The new thickness value.</param>
        /// <returns></returns>
        public ExternalDBPoint SetThickness(double thickness)
        {
            try
            {
                AcEntity.UpgradeOpen();
                AcEntity.Thickness = thickness;
                AcEntity.DowngradeOpen();
                return this;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }
        #endregion
    }
}
