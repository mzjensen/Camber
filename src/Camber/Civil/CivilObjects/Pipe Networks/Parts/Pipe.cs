#region references
using System;
using System.Collections.Generic;
using System.Linq;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acGeom = Autodesk.AutoCAD.Geometry;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using AeccPipe = Autodesk.Civil.DatabaseServices.Pipe;
using AeccPipeLabel = Autodesk.Civil.DatabaseServices.PipeLabel;
using Autodesk.DesignScript.Runtime;
using Autodesk.DesignScript.Geometry;
using DynamoServices;
using Camber.Civil.Labels;
using Camber.Civil.CivilObjects;
using Camber.Properties;
using Camber.Utilities.GeometryConversions;
#endregion

namespace Camber.Civil.PipeNetworks.Parts
{
    [RegisterForTrace]
    public sealed class Pipe : Part
    {
        #region properties
        internal AeccPipe AeccPipe => AcObject as AeccPipe;

        /// <summary>
        /// Gets the current flow direction of a Pipe relative to its start and end points.
        /// </summary>
        public string FlowDirection => GetString();

        /// <summary>
        /// Gets the method that determines how a Pipe will behave when resized.
        /// </summary>
        public string ResizeBehavior => GetString("HoldOnResizeType");

        /// <summary>
        /// Gets a Pipe's subentity type.
        /// </summary>
        public string SubEntityType => GetString();

        /// <summary>
        /// Gets the Pipe Plan Labels associated with a Pipe.
        /// </summary>
        public IList<PipePlanLabel> PipePlanLabels
        {
            get
            {
                var labels = new List<PipePlanLabel>();
                foreach (acDb.ObjectId oid in AeccPipeLabel.GetAvailableLabelIds(InternalObjectId))
                {
                    labels.Add(PipePlanLabel.GetByObjectId(oid));
                }
                return labels;
            }
        }
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
        /// Performs a cover check on a Pipe relative to its reference Surface.
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

        #region deprecated
        /// <summary>
        /// Gets the bearing of a Pipe. For straight Pipes, the returned value is the horizontal bearing.
        /// For curved Pipes, the returned bearing is the chord bearing.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Pipe.Bearing",
            "Autodesk.Civil.DynamoNodes.Pipe.Bearing")]
        public double Bearing
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Pipe.Bearing"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the depth of cover at a Pipe's start point, measured from the top outside edge of the Pipe to its reference Surface.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Pipe.CoverStart",
            "Autodesk.Civil.DynamoNodes.Pipe.StartCover")]
        public double CoverStart
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Pipe.StartCover"));
                return GetDouble("CoverOfStartPoint");
            }
        }

        /// <summary>
        /// Gets the cover at a Pipe's end point, measured from the top outside edge of the Pipe to its reference Surface.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Pipe.CoverEnd",
            "Autodesk.Civil.DynamoNodes.Pipe.EndCover")]
        public double CoverEnd
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Pipe.EndCover"));
                return GetDouble("CoverOfEndpoint");
            }
        }

        /// <summary>
        /// Gets a Pipe's cross sectional shape.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Pipe.CrossSectionalShape",
            "Autodesk.Civil.DynamoNodes.Pipe.CrossSectionalShape")]
        public string CrossSectionalShape
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Pipe.CrossSectionalShape"));
                return GetString();
            }
        }

        /// <summary>
        /// Gets the end point of a Pipe at the centerline.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Pipe.EndPoint",
            "Autodesk.Civil.DynamoNodes.Pipe.EndPoint")]
        public Point EndPoint
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Pipe.EndPoint"));
                return GeometryConversions.AcPointToDynPoint(AeccPipe.EndPoint);
            }
        }

        /// <summary>
        /// Gets the offset of a Pipe's end point from its reference Alignment.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Pipe.EndOffset",
            "Autodesk.Civil.DynamoNodes.Pipe.EndOffset")]
        public double EndOffset
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Pipe.EndOffset"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the station of a Pipe's end point along its reference Alignment.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Pipe.EndStation",
            "Autodesk.Civil.DynamoNodes.Pipe.EndStation")]
        public double EndStation
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Pipe.EndStation"));
                return GetDouble("EndStation");
            }
        }

        /// <summary>
        /// Gets a Pipe's end Structure if it exists.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Pipe.EndStructure",
            "Autodesk.Civil.DynamoNodes.Pipe.EndStructure")]
        public Structure EndStructure
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Pipe.EndStructure"));
                return Structure.GetByObjectId(AeccPipe.EndStructureId) ?? null;
            }
        }

        /// <summary>
        /// Gets the downstream elevation of the energy grade line for a Pipe.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Pipe.EGLDown",
            "Autodesk.Civil.DynamoNodes.Pipe.EnergyGradeLineDown")]
        public double EGLDown
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Pipe.EnergyGradeLineDown"));
                return GetDouble("EnergyGradeLineDown");
            }
        }

        /// <summary>
        /// Gets the upstream elevation of the energy grade line for a Pipe.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Pipe.EGLUp",
            "Autodesk.Civil.DynamoNodes.Pipe.EnergyGradeLineUp")]
        public double EGLUp
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Pipe.EnergyGradeLineUp"));
                return GetDouble("EnergyGradeLineUp");
            }
        }

        /// <summary>
        /// Gets the method that is used to determine the flow direction of a Pipe.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Pipe.FlowDirectionMethod",
            "Autodesk.Civil.DynamoNodes.Pipe.FlowDirectionMethod")]
        public string FlowDirectionMethod
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Pipe.FlowDirectionMethod"));
                return GetString();
            }
        }

        /// <summary>
        /// Gets the flow rate value assigned to a Pipe.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Pipe.FlowRate",
            "Autodesk.Civil.DynamoNodes.Pipe.FlowRate")]
        public double FlowRate
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Pipe.FlowRate"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the downstream elevation of the hydraulic grade line for a Pipe.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Pipe.HGLDown",
            "Autodesk.Civil.DynamoNodes.Pipe.HydraulicGradeLineDown")]
        public double HGLDown
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Pipe.HydraulicGradeLineDown"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the upstream elevation of the hydraulic grade line for a Pipe.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Pipe.HGLUp",
            "Autodesk.Civil.DynamoNodes.Pipe.HydraulicGradeLineUp")]
        public double HGLUp
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Pipe.HydraulicGradeLineUp"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the inner diameter or width of a Pipe.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Pipe.InnerDiameterOrWidth",
            "Autodesk.Civil.DynamoNodes.Pipe.InnerDiameterOrWidth")]
        public double InnerDiameterOrWidth
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Pipe.InnerDiameterOrWidth"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the inner height of a Pipe.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Pipe.InnerHeight",
            "Autodesk.Civil.DynamoNodes.Pipe.InnerHeight")]
        public double InnerHeight
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Pipe.InnerHeight"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the junction loss value assigned to a Pipe.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Pipe.JunctionLoss",
            "Autodesk.Civil.DynamoNodes.Pipe.JunctionLoss")]
        public double JunctionLoss
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Pipe.JunctionLoss"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the 2D length of a Pipe measured from the centers of its start and end Structures. 
        /// </summary>
        public double Length2DCenterToCenter
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MESSAGE, "Pipe.Length"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the 2D length of a Pipe measured from the inside edges of its start and end Structures. 
        /// </summary>
        public double Length2DToInsideEdge
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MESSAGE, "Pipe.Length"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the 3D length of a Pipe measured from the centers of its start and end Structures. 
        /// </summary>
        public double Length3DCenterToCenter
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MESSAGE, "Pipe.Length"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the 3D length of a Pipe measured from the inside edges of its start and end Structures. 
        /// </summary>
        public double Length3DToInsideEdge
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MESSAGE, "Pipe.Length"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the maximum depth of cover along the entire length of a Pipe, measured from the top outside of the Pipe to its reference Surface.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Pipe.CoverMax",
            "Autodesk.Civil.DynamoNodes.Pipe.MaxCover")]
        public double CoverMax
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Pipe.MaxCover"));
                return GetDouble("MaximumCover");
            }
        }

        /// <summary>
        /// Gets the minimum depth of cover along the entire length of a Pipe, measured from the top outside of the Pipe to its reference Surface.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Pipe.CoverMin",
            "Autodesk.Civil.DynamoNodes.Pipe.MinCover")]
        public double CoverMin
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Pipe.MinCover"));
                return GetDouble("MinimumCover");
            }
        }

        /// <summary>
        /// Gets the outer diameter or width of a Pipe.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Pipe.OuterDiameterOrWidth",
            "Autodesk.Civil.DynamoNodes.Pipe.OuterDiameterOrWidth")]
        public double OuterDiameterOrWidth
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Pipe.OuterDiameterOrWidth"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the outer height of a Pipe.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Pipe.OuterHeight",
            "Autodesk.Civil.DynamoNodes.Pipe.OuterHeight")]
        public double OuterHeight
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Pipe.OuterHeight"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the radius of a Pipe.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Pipe.Radius",
            "Autodesk.Civil.DynamoNodes.Pipe.Radius")]
        public double Radius
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Pipe.Radius"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the return period value assigned to a Pipe.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Pipe.ReturnPeriod",
            "Autodesk.Civil.DynamoNodes.Pipe.ReturnPeriod")]
        public int ReturnPeriod
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Pipe.ReturnPeriod"));
                return GetInt();
            }
        }

        /// <summary>
        /// Gets the slope of a Pipe in absolute value.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Pipe.Slope",
            "Autodesk.Civil.DynamoNodes.Pipe.Slope")]
        public double Slope
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Pipe.Slope"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the start point of a Pipe at the centerline.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Pipe.StartPoint",
            "Autodesk.Civil.DynamoNodes.Pipe.StartPoint")]
        public Point StartPoint
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Pipe.StartPoint"));
                return GeometryConversions.AcPointToDynPoint(AeccPipe.StartPoint);
            }
        }

        /// <summary>
        /// Gets the offset of a Pipe's start point from its reference Alignment.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Pipe.StartOffset",
            "Autodesk.Civil.DynamoNodes.Pipe.StartOffset")]
        public double StartOffset
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Pipe.StartOffset"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the station of a Pipe's end point along its reference Alignment.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Pipe.StartStation",
            "Autodesk.Civil.DynamoNodes.Pipe.StartStation")]
        public double StartStation
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Pipe.StartStation"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets a Pipe's start Structure if it exists.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Pipe.StartStructure",
            "Autodesk.Civil.DynamoNodes.Pipe.StartStructure")]
        public Structure StartStructure
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Pipe.StartStructure"));
                return Structure.GetByObjectId(AeccPipe.StartStructureId) ?? null;
            }
        }

        /// <summary>
        /// Resize a Pipe to a new inner diameter or width.
        /// </summary>
        /// <param name="newSize">Inner diameter or width</param>
        /// <param name="useClosestSize"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Pipe.ResizeByInnerDimension",
            "Autodesk.Civil.DynamoNodes.Pipe.Resize")]
        public Pipe ResizeByInnerDimension(double newSize, bool useClosestSize = true)
        {
            LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Pipe.Resize"));

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
            LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MESSAGE, "Pipe.SetSlope"));

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
            LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MESSAGE, "Pipe.SetSlope"));

            bool openedForWrite = AeccPipe.IsWriteEnabled;
            if (!openedForWrite) AeccPipe.UpgradeOpen();
            AeccPipe.SetSlopeHoldStart(slope);
            if (!openedForWrite) AeccPipe.DowngradeOpen();
            return this;
        }
        #endregion
    }
}
