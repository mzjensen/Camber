#region references
using System;
using System.Collections.Generic;
using System.Linq;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acGeom = Autodesk.AutoCAD.Geometry;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using AeccPipe = Autodesk.Civil.DatabaseServices.Pipe;
using Autodesk.DesignScript.Runtime;
using Autodesk.DesignScript.Geometry;
using DynamoServices;
using Camber.Utils;
#endregion

namespace Camber.Civil.PipeNetworks.Parts
{
    [RegisterForTrace]
    public sealed class Pipe : Part
    {
        #region properties
        internal AeccPipe AeccPipe => AcObject as AeccPipe;

        /// <summary>
        /// Gets the bearing of the Pipe. For straight Pipes, the returned value is the horizontal bearing.
        /// For curved Pipes, the returned bearing is the chord bearing.
        /// </summary>
        public double Bearing => GetDouble();

        /// <summary>
        /// Gets the depth of cover at the Pipe's start point, measured from the top outside edge of the Pipe to its reference Surface.
        /// </summary>
        public double CoverStart => GetDouble("CoverOfStartPoint");

        /// <summary>
        /// Gets the cover at the Pipe's end point, measured from the top outside edge of the Pipe to its reference Surface.
        /// </summary>
        public double CoverEnd => GetDouble("CoverOfEndpoint");

        /// <summary>
        /// Gets the Pipe's cross sectional shape.
        /// </summary>
        public string CrossSectionalShape => GetString();

        /// <summary>
        /// Gets the end point of the Pipe at the centerline.
        /// </summary>
        public Point EndPoint => GeometryConversions.AcPointToDynPoint(AeccPipe.EndPoint);

        /// <summary>
        /// Gets the offset of the Pipe's end point from the Pipe's reference Alignment.
        /// </summary>
        public double EndOffset => GetDouble();

        /// <summary>
        /// Gets the station of the Pipe's end point along the Pipe's reference Alignment.
        /// </summary>
        public double EndStation => GetDouble("EndStation");

        /// <summary>
        /// Gets the Pipe's end Structure if it exists.
        /// </summary>
        public Structure EndStructure => Structure.GetByObjectId(AeccPipe.EndStructureId) ?? null;

        /// <summary>
        /// Gets the downstream elevation of the energy grade line for the Pipe.
        /// </summary>
        public double EGLDown => GetDouble("EnergyGradeLineDown");

        /// <summary>
        /// Gets the upstream elevation of the energy grade line for the Pipe.
        /// </summary>
        public double EGLUp => GetDouble("EnergyGradeLineUp");

        /// <summary>
        /// Gets the current flow direction of the Pipe relative to its start and end points.
        /// </summary>
        public string FlowDirection => GetString();

        /// <summary>
        /// Gets the method that is used to determine the flow direction of the Pipe.
        /// </summary>
        public string FlowDirectionMethod => GetString();

        /// <summary>
        /// Gets the flow rate value assigned to the Pipe.
        /// </summary>
        public double FlowRate => GetDouble();

        /// <summary>
        /// Gets the method that determines how the Pipe will behave when resized.
        /// </summary>
        public string ResizeBehavior => GetString("HoldOnResizeType");

        /// <summary>
        /// Gets the downstream elevation of the hydraulic grade line for the Pipe.
        /// </summary>
        public double HGLDown => GetDouble();

        /// <summary>
        /// Gets the upstream elevation of the hydraulic grade line for the Pipe.
        /// </summary>
        public double HGLUp => GetDouble();

        /// <summary>
        /// Gets the inner diameter or width of the Pipe.
        /// </summary>
        public double InnerDiameterOrWidth => GetDouble();

        /// <summary>
        /// Gets the inner height of the Pipe.
        /// </summary>
        public double InnerHeight => GetDouble();

        /// <summary>
        /// Gets the junction loss value assigned to the Pipe.
        /// </summary>
        public double JunctionLoss => GetDouble();

        /// <summary>
        /// Gets the 2D length of the Pipe measured from the centers of its start and end Structures. 
        /// </summary>
        public double Length2DCenterToCenter => GetDouble();

        /// <summary>
        /// Gets the 2D length of the Pipe measured from the inside edges of its start and end Structures. 
        /// </summary>
        public double Length2DToInsideEdge => GetDouble();

        /// <summary>
        /// Gets the 3D length of the Pipe measured from the centers of its start and end Structures. 
        /// </summary>
        public double Length3DCenterToCenter => GetDouble();

        /// <summary>
        /// Gets the 3D length of the Pipe measured from the inside edges of its start and end Structures. 
        /// </summary>
        public double Length3DToInsideEdge => GetDouble();

        /// <summary>
        /// Gets the maximum depth of cover along the entire length of the Pipe, measured from the top outside of the Pipe to its reference Surface.
        /// </summary>
        public double CoverMax => GetDouble("MaximumCover");

        /// <summary>
        /// Gets the minimum depth of cover along the entire length of the Pipe, measured from the top outside of the Pipe to its reference Surface.
        /// </summary>
        public double CoverMin => GetDouble("MinimumCover");

        /// <summary>
        /// Gets the outer diameter or width of the Pipe.
        /// </summary>
        public double OuterDiameterOrWidth => GetDouble();

        /// <summary>
        /// Gets the outer height of the Pipe.
        /// </summary>
        public double OuterHeight => GetDouble();

        /// <summary>
        /// Gets the radius of the Pipe.
        /// </summary>
        public double Radius => GetDouble();

        /// <summary>
        /// Gets the return period value assigned to the Pipe.
        /// </summary>
        public int ReturnPeriod => GetInt();

        /// <summary>
        /// Gets the slope of the Pipe in absolute value.
        /// </summary>
        public double Slope => GetDouble();

        /// <summary>
        /// Gets the start point of the Pipe at the centerline.
        /// </summary>
        public Point StartPoint => GeometryConversions.AcPointToDynPoint(AeccPipe.StartPoint);

        /// <summary>
        /// Gets the offset of the Pipe's start point from the Pipe's reference Alignment.
        /// </summary>
        public double StartOffset => GetDouble();

        /// <summary>
        /// Gets the station of the Pipe's end point along the Pipe's reference Alignment.
        /// </summary>
        public double StartStation => GetDouble();

        /// <summary>
        /// Gets the Pipe's start Structure if it exists.
        /// </summary>
        public Structure StartStructure => Structure.GetByObjectId(AeccPipe.StartStructureId) ?? null;

        /// <summary>
        /// Gets the Pipe's subentity type.
        /// </summary>
        public string SubEntityType => GetString();
        #endregion

        #region constructors
        internal Pipe(AeccPipe aeccPipe, bool isDynamoOwned = false) : base(aeccPipe, isDynamoOwned) { }

        [SupressImportIntoVM]
        internal static Pipe GetByObjectId(acDb.ObjectId pipeId)
            => CivilObjectSupport.Get<Pipe, AeccPipe>
            (pipeId, (pipe) => new Pipe(pipe));

        // This doesn't work.
        [IsVisibleInDynamoLibrary(false)]
        public static Pipe ByCurve(Curve curve, PipeNetwork pipeNetwork, PartFamily partFamily, PartSize partSize, bool applyRules = false)
        {
            if (pipeNetwork is null)
            {
                throw new ArgumentNullException("Pipe Network is null.");
            }

            var document = acDynNodes.Document.Current;

            var res = CommonConstruct<Pipe, AeccPipe>(
                document,
                (ctx) =>
                {
                    acDb.ObjectId oid = acDb.ObjectId.Null;
                    if (curve is Line)
                    {
                        pipeNetwork.AeccPipeNetwork.AddLinePipe(
                            partFamily.InternalObjectId,
                            partSize.InternalObjectId,
                            (acGeom.LineSegment3d)GeometryConversions.DynLineToAcLineSegment((Line)curve, true),
                            ref oid,
                            applyRules);
                    }
                    else if (curve is Arc)
                    {
                        // Kinda stuck here. Is an Arc actually the right geometry type to use?
                        return null;
                    }
                    else
                    {
                        throw new ArgumentException("Curve must be a Line or Arc.");
                    }
                    
                    return ctx.Transaction.GetObject(oid, acDb.OpenMode.ForWrite) as AeccPipe;
                },
                (ctx, pipe, existing) =>
                {
                    if (existing)
                    {
                        // What do we do here? Update the start and end points? 
                    }
                    return true;
                });
            return res;
        }
        #endregion

        #region methods
        public override string ToString() => $"Pipe(Name = {Name})";

        /// <summary>
        /// Resize a Pipe to a new inner diameter or width.
        /// </summary>
        /// <param name="newSize">Inner diameter or width</param>
        /// <param name="useClosestSize"></param>
        /// <returns></returns>
        public Pipe ResizeByInnerDimension(double newSize, bool useClosestSize = true)
        {
            bool openedForWrite = AeccPipe.IsWriteEnabled;
            if (!openedForWrite) AeccPipe.UpgradeOpen();
            AeccPipe.ResizeByInnerDiameterOrWidth(newSize, useClosestSize);
            if (!openedForWrite) AeccPipe.DowngradeOpen();
            return this;
        }

        /// <summary>
        /// Sets the slope of a Pipe from its end point towards its start point.
        /// </summary>
        /// <param name="slope"></param>
        /// <returns></returns>
        public Pipe SetSlopeHoldEnd(double slope)
        {
            bool openedForWrite = AeccPipe.IsWriteEnabled;
            if (!openedForWrite) AeccPipe.UpgradeOpen();
            AeccPipe.SetSlopeHoldEnd(slope);
            if (!openedForWrite) AeccPipe.DowngradeOpen();
            return this;
        }

        /// <summary>
        /// Sets the slope of a Pipe from its start point towards its end point.
        /// </summary>
        /// <param name="slope"></param>
        /// <returns></returns>
        public Pipe SetSlopeHoldStart(double slope)
        {
            bool openedForWrite = AeccPipe.IsWriteEnabled;
            if (!openedForWrite) AeccPipe.UpgradeOpen();
            AeccPipe.SetSlopeHoldStart(slope);
            if (!openedForWrite) AeccPipe.DowngradeOpen();
            return this;
        }

        /// <summary>
        /// Performs a cover check on the Pipe relative to its reference Surface.
        /// </summary>
        /// <param name="minCover"></param>
        /// <param name="maxCover"></param>
        /// <returns></returns>
        [MultiReturn(new[] {
            "Is Min Cover Violated?",
            "Is Max Cover Violated?",
            "Min Cover Violation Locations",
            "Max Cover Violation Locations",
            "Min Cover Violation Depths",
            "Max Cover Violation Depths" })]
        public Dictionary<string, object> CoverCheck(double minCover, double maxCover)
        {
            // Not sure if I like this method.
            // 1. It always returns negative values for the differences, even if the pipe is above the surface.
            // 2. It appears to always measure the differences relative to the top outer edge of the pipe,
            // so they won't really be correct if the pipe is above the surface.
            
            acGeom.Point3d[] minPoints = new acGeom.Point3d[] { };
            acGeom.Point3d[] maxPoints = new acGeom.Point3d[] { };
            double[] minDepths = new double[] { };
            double[] maxDepths = new double[] { };
            double[] minParams = new double[] { };
            double[] maxParams = new double[] { };

            bool isMinViolated = AeccPipe.IsMinCoverViolated(minCover, ref minPoints, ref minDepths, ref minParams);
            bool isMaxViolated = AeccPipe.IsMaxCoverViolated(maxCover, ref maxPoints, ref maxDepths, ref maxParams);

            List<acGeom.Point3d> minAcPoints = minPoints.ToList();
            List<acGeom.Point3d> maxAcPoints = maxPoints.ToList();
            List<Point> minDynPoints = new List<Point>();
            List<Point> maxDynPoints = new List<Point>();
            
            foreach (acGeom.Point3d acPoint in minAcPoints)
            {
                minDynPoints.Add(GeometryConversions.AcPointToDynPoint(acPoint));
            }

            foreach (acGeom.Point3d acPoint in maxAcPoints)
            {
                maxDynPoints.Add(GeometryConversions.AcPointToDynPoint(acPoint));
            }

            return new Dictionary<string, object>
            {
                { "Is Min Cover Violated?", isMinViolated },
                { "Is Max Cover Violated?", isMaxViolated },
                { "Min Cover Violation Locations", minDynPoints },
                { "Max Cover Violation Locations", maxDynPoints },
                { "Min Cover Violation Depths", minDepths.ToList() },
                { "Max Cover Violation Depths", maxDepths.ToList() }
            };
        }
        #endregion
    }
}
