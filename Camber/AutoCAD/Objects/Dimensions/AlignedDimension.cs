#region references
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Camber.Utilities.GeometryConversions;
using DynamoServices;
using System;
using AcAlignedDimension = Autodesk.AutoCAD.DatabaseServices.AlignedDimension;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acGeom = Autodesk.AutoCAD.Geometry;
#endregion

namespace Camber.AutoCAD.Objects.Dimensions
{
    [RegisterForTrace]
    public class AlignedDimension : Dimension
    {
        #region properties
        internal AcAlignedDimension AcAlignedDimension => AcObject as AcAlignedDimension;

        /// <summary>
        /// Gets the first point of an Aligned Dimension.
        /// </summary>
        public Point Point1 => GeometryConversions.AcPointToDynPoint(AcAlignedDimension.XLine1Point);

        /// <summary>
        /// Gets the second point of an Aligned Dimension.
        /// </summary>
        public Point Point2 => GeometryConversions.AcPointToDynPoint(AcAlignedDimension.XLine2Point);
        #endregion

        #region constructors
        [SupressImportIntoVM]
        internal static AlignedDimension GetByObjectId(acDb.ObjectId dimId)
            => ObjectSupport.Get<AlignedDimension, AcAlignedDimension>
            (dimId, (dimension) => new AlignedDimension(dimension));

        internal AlignedDimension(AcAlignedDimension acDim, bool isDynamoOwned = false) : base(acDim, isDynamoOwned) { }

        /// <summary>
        /// Creates a new Aligned Dimension by two points and offset distance.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="offset"></param>
        /// <param name="block"></param>
        /// <param name="text"></param>
        /// <param name="dimensionStyle"></param>
        /// <returns></returns>
        public static AlignedDimension ByTwoPoints(
            Point point1,
            Point point2,
            double offset,
            acDynNodes.Block block, 
            string text = "",
            string dimensionStyle = "")
        {
            acDynNodes.Document document = acDynNodes.Document.Current;

            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    acDb.BlockTable bt = (acDb.BlockTable)ctx.Transaction.GetObject(
                        document.AcDocument.Database.BlockTableId,
                        acDb.OpenMode.ForWrite);
                    acDb.BlockTableRecord btr = (acDb.BlockTableRecord)ctx.Transaction.GetObject(
                        bt[block.Name],
                        acDb.OpenMode.ForWrite);

                    acDb.ObjectId styleId = acDb.ObjectId.Null;
                    acDb.DimStyleTable dst = (acDb.DimStyleTable)ctx.Transaction.GetObject(
                        document.AcDocument.Database.DimStyleTableId,
                        acDb.OpenMode.ForRead);
                    if (dimensionStyle != "" && dst.Has(dimensionStyle))
                    {
                        styleId = dst[dimensionStyle];
                    }
                    else if (dimensionStyle == "")
                    {
                        styleId = document.AcDocument.Database.Dimstyle;
                    }
                    else
                    {
                        throw new InvalidOperationException("A dimension style with that name does not exist.");
                    }

                    AcAlignedDimension dim = new AcAlignedDimension(
                        (acGeom.Point3d)GeometryConversions.DynPointToAcPoint(point1),
                        (acGeom.Point3d)GeometryConversions.DynPointToAcPoint(point2),
                        ComputeOffsetPoint(point1, point2, offset),
                        text,
                        styleId);
                    btr.AppendEntity(dim);
                    ctx.Transaction.AddNewlyCreatedDBObject(dim, true);
                    return new AlignedDimension(dim, true);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"AlignedDimension(Measurement = {Measurement:F3})";

        /// <summary>
        /// Computes the offset point for an Aligned Dimension.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private static acGeom.Point3d ComputeOffsetPoint(Point point1, Point point2, double offset)
        {
            Point p1 = Point.ByCoordinates(point1.X, point1.Y, 0);
            Point p2 = Point.ByCoordinates(point2.X, point2.Y, 0);
            Line line = Line.ByStartPointEndPoint(p1, p2);

            if (offset == 0)
            {
                return (acGeom.Point3d)GeometryConversions.DynPointToAcPoint(line.PointAtParameter(0.5));
            }

            Vector normal = line.NormalAtParameter();
            Line offsetLine = Line.ByStartPointDirectionLength(p1, normal, offset);
            return (acGeom.Point3d)GeometryConversions.DynPointToAcPoint(offsetLine.EndPoint);
        }
        #endregion
    }
}
