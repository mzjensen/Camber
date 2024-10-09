#region references
using System;
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acGeom = Autodesk.AutoCAD.Geometry;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civDb = Autodesk.Civil.DatabaseServices;
using AeccStructure = Autodesk.Civil.DatabaseServices.Structure;
using AeccStructureLabel = Autodesk.Civil.DatabaseServices.StructureLabel;
using Autodesk.DesignScript.Runtime;
using Autodesk.DesignScript.Geometry;
using Camber.Civil.Labels;
using Camber.Civil.CivilObjects;
using Camber.Properties;
using Camber.Utilities.GeometryConversions;
using DynamoServices;
#endregion

namespace Camber.Civil.PipeNetworks.Parts
{
    public sealed class Structure : Part
    {
        #region properties
        internal AeccStructure AeccStructure => AcObject as AeccStructure;

        /// <summary>
        /// Gets the Structure Plan Labels associated with a Structure.
        /// </summary>
        public IList<StructurePlanLabel> StructurePlanLabels
        {
            get
            {
                var labels = new List<StructurePlanLabel>();
                foreach (acDb.ObjectId oid in AeccStructureLabel.GetAvailableLabelIds(InternalObjectId))
                {
                    labels.Add(StructurePlanLabel.GetByObjectId(oid));
                }
                return labels;
            }
        }
        #endregion

        #region constructors
        internal Structure(AeccStructure aeccStructure, bool isDynamoOwned = false) : base(aeccStructure, isDynamoOwned) { }

        [SupressImportIntoVM]
        internal static Structure GetByObjectId(acDb.ObjectId structureId)
            => CivilObjectSupport.Get<Structure, AeccStructure>
            (structureId, (structure) => new Structure(structure));
        #endregion

        #region methods
        public override string ToString() => $"Structure(Name = {Name})";

        /// <summary>
        /// Connects a Structure to a specified Pipe.
        /// </summary>
        /// <param name="pipe"></param>
        /// <param name="connectToStart">True = connect to Pipe start, False = connect to Pipe end</param>
        /// <returns></returns>
        public Structure ConnectToPipe(Pipe pipe, bool connectToStart)
        {
            var position = civDb.ConnectorPositionType.End;
            if (connectToStart)
            {
                position = civDb.ConnectorPositionType.Start;
            }
            
            using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
            {
                var aeccPipe = (civDb.Pipe) ctx.Transaction.GetObject(pipe.InternalObjectId, acDb.OpenMode.ForWrite);
                AeccStructure.UpgradeOpen();
                AeccStructure.ConnectToPipe(pipe.InternalObjectId, position);
                AeccStructure.DowngradeOpen();
                aeccPipe.DowngradeOpen();
            }
            return this;
        }

        /// <summary>
        /// Disconnects a Structure from a specified Pipe.
        /// </summary>
        /// <param name="pipe"></param>
        /// <returns></returns>
        public Structure DisconnectFromPipe(Pipe pipe)
        {
            using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
            {
                var aeccPipe = (civDb.Pipe)ctx.Transaction.GetObject(pipe.InternalObjectId, acDb.OpenMode.ForWrite);
                AeccStructure.UpgradeOpen();
                AeccStructure.Disconnect(pipe.InternalObjectId);
                AeccStructure.DowngradeOpen();
                aeccPipe.DowngradeOpen();
            }

            return this;
        }

        /// <summary>
        /// Determines if a point is within the region of a Structure.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsPointInsideStructureRegion(Point point) => AeccStructure.IsPointInsideStructureRegion((acGeom.Point3d)GeometryConversions.DynPointToAcPoint(point, true));

        /// <summary>
        /// Resize a Structure by rim and sump elevations.
        /// </summary>
        /// <param name="partFamily"></param>
        /// <param name="rimElevation"></param>
        /// <param name="sumpElevation"></param>
        /// <returns></returns>
        public Structure Resize(PartFamily partFamily, double rimElevation, double sumpElevation)
        {
            try
            {
                bool openedForWrite = AeccStructure.IsWriteEnabled;
                if (!openedForWrite) AeccStructure.UpgradeOpen();
                AeccStructure.ResizeJunctionStructure(partFamily.GUID, rimElevation, sumpElevation);
                if (!openedForWrite) AeccStructure.DowngradeOpen();
                return this;
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region deprecated
        /// <summary>
        /// Gets whether a Structure's rim elevation should be automatically adjusted when the reference Surface changes.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Structure.AutomaticRimSurfaceAdjustment",
            "Autodesk.Civil.DynamoNodes.Structure.AutomaticSurfaceAdjustment")]
        public bool AutomaticRimSurfaceAdjustment
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Structure.AutomaticSurfaceAdjustment"));
                return GetBool();
            }
        }

        /// <summary>
        /// Gets the value that determines how close to a Structure’s rim a Pipe can be positioned.
        /// This property only applies to two-tiered junction Structures.
        /// It is similar to Vertical Pipe Clearance, but only describes the transition zone between the access cylinder and the larger barrel cylinder.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Structure.BarrelPipeClearance",
            "Autodesk.Civil.DynamoNodes.Structure.BarrelPipeClearance")]
        public double BarrelPipeClearance
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Structure.BarrelPipeClearance"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets how a Structure's sump should be adjusted when the elevations of its connected Pipes are changed.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Structure.ControlSumpBy",
            "Autodesk.Civil.DynamoNodes.Structure.SumpControlMethod")]
        public string ControlSumpBy
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Structure.SumpControlMethod"));
                return GetString();
            }
        }

        /// <summary>
        /// Gets the model or type of grate used for catch basin Structures.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Structure.Cover",
            "Autodesk.Civil.DynamoNodes.Structure.Cover")]
        public string Cover
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Structure.Cover"));
                return GetString();
            }
        }

        /// <summary>
        /// Gets the outer diameter or width of a Structure.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Structure.OuterDiameterOrWidth",
            "Autodesk.Civil.DynamoNodes.Structure.OuterDiameterOrWidth")]
        public double OuterDiameterOrWidth
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Structure.OuterDiameterOrWidth"));
                return GetDouble("DiameterOrWidth");
            }
        }

        /// <summary>
        /// Gets the floor thickness of a Structure.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Structure.FloorThickness",
            "Autodesk.Civil.DynamoNodes.Structure.FloorThickness")]
        public double FloorThickness
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Structure.FloorThickness"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the model or type of a Structure's frame.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Structure.Frame",
            "Autodesk.Civil.DynamoNodes.Structure.Frame")]
        public string Frame
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Structure.Frame"));
                return GetString();
            }
        }

        /// <summary>
        /// Gets the diameter of a Structure's frame.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Structure.FrameDiameter",
            "Autodesk.Civil.DynamoNodes.Structure.FrameDiameter")]
        public double FrameDiameter
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Structure.FrameDiameter"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the height of a Structure's frame.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Structure.FrameHeight",
            "Autodesk.Civil.DynamoNodes.Structure.FrameHeight")]
        public double FrameHeight
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Structure.FrameHeight"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the model or type of a Structure's grate.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Structure.Grate",
            "Autodesk.Civil.DynamoNodes.Structure.Grate")]
        public string Grate
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Structure.Grate"));
                return GetString();
            }
        }

        /// <summary>
        /// Gets the thickness of the base of a headwall.
        /// This property only applies to inlet-outlet Structures.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Structure.HeadwallBaseThickness",
            "Autodesk.Civil.DynamoNodes.Structure.HeadwallBaseThickness")]
        public double HeadwallBaseThickness
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Structure.HeadwallBaseThickness"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the width of the base of a headwall.
        /// This property only applies to inlet-outlet Structures.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Structure.HeadwallBaseWidth",
            "Autodesk.Civil.DynamoNodes.Structure.HeadwallBaseWidth")]
        public double HeadwallBaseWidth
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Structure.HeadwallBaseWidth"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the overall height of a Structure.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Structure.Height",
            "Autodesk.Civil.DynamoNodes.Structure.Height")]
        public double Height
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Structure.Height"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the inner diamter or width of a Structure.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Structure.InnerDiameterOrWidth",
            "Autodesk.Civil.DynamoNodes.Structure.InnerDiameterOrWidth")]
        public double InnerDiameterOrWidth
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Structure.InnerDiameterOrWidth"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the inner length of a Structure.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Structure.InnerLength",
            "Autodesk.Civil.DynamoNodes.Structure.InnerLength")]
        public double InnerLength
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Structure.InnerLength"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the length of a Structure.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Structure.Length",
            "Autodesk.Civil.DynamoNodes.Structure.Length")]
        public double Length
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Structure.Length"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the location of a Structure.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Structure.Location",
            "Autodesk.AutoCAD.DynamoNodes.Object.Location")]
        public Point Location
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Object.Location"));
                return GeometryConversions.AcPointToDynPoint(AeccStructure.Location);
            }
        }

        /// <summary>
        /// Gets the offset of a Structure from its reference Alignment.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Structure.Offset",
            "Autodesk.Civil.DynamoNodes.Structure.Offset")]
        public double Offset
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Structure.Offset"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the depth to the bottom of the lowest Pipe connected to a Structure.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Structure.PipeLowestBottomDepth",
            "Autodesk.Civil.DynamoNodes.Structure.DepthToLowestPipeBottom")]
        public double PipeLowestBottomDepth
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Structure.DepthToLowestPipeBottom"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the depth to the top of uppermost Pipe connected to a Structure.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Structure.PipeUpperTopDepth",
            "Autodesk.Civil.DynamoNodes.Structure.DepthToUpperPipeTop")]
        public double PipeUpperTopDepth
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Structure.DepthToUpperPipeTop"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the rim elevation of a Structure.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Structure.RimElevation",
            "Autodesk.Civil.DynamoNodes.Structure.RimElevation")]
        public double RimElevation
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Structure.RimElevation"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the distance between the sump and rim of a Structure.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Structure.RimToSumpHeight",
            "Autodesk.Civil.DynamoNodes.Structure.RimToSumpHeight")]
        public double RimToSumpHeight
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Structure.RimToSumpHeight"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the rotation of a Structure.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Structure.Rotation",
            "Autodesk.AutoCAD.DynamoNodes.Object.Rotation")]
        public double Rotation
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Object.Rotation"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the station of a Structure along its reference Alignment.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Structure.Station",
            "Autodesk.Civil.DynamoNodes.Structure.Station")]
        public double Station
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Structure.Station"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the sump depth of a Structure.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Structure.SumpDepth",
            "Autodesk.Civil.DynamoNodes.Structure.SumpDepth")]
        public double SumpDepth
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Structure.SumpDepth"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the sump elevation of a Structure.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Structure.SumpElevation",
            "Autodesk.Civil.DynamoNodes.Structure.SumpElevation")]
        public double SumpElevation
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Structure.SumpElevation"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets a Structure's surface adjustment value.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Structure.SurfaceAdjustmentValue",
            "Autodesk.Civil.DynamoNodes.Structure.SurfaceAdjustmentValue")]
        public double SurfaceAdjustmentValue
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Structure.SurfaceAdjustmentValue"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the required clearance from the top outside of the uppermost pipe connected to a Structure to the Structure's rim.
        /// This is defined in the structure catalog and ensures that Pipes enter the Structure at an appropriate elevation.
        /// For example, this property prevents a Pipe from entering through the cone of a Structure.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Structure.VerticalPipeClearance",
            "Autodesk.Civil.DynamoNodes.Structure.VerticalPipeClearance")]
        public double VerticalPipeClearance
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Structure.VerticalPipeClearance"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Attempts to resize a Structure by Pipe depths.
        /// </summary>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Structure.ResizeByPipeDepths",
            "Autodesk.Civil.DynamoNodes.Structure.ResizeByPipeDepths")]
        public Structure ResizeByPipeDepths()
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Structure.ResizeByPipeDepths"));

            bool openedForWrite = AeccStructure.IsWriteEnabled;
            if (!openedForWrite) AeccStructure.UpgradeOpen();
            bool res = AeccStructure.ResizeByPipeDepths();
            if (res)
            {
                if (!openedForWrite) AeccStructure.DowngradeOpen();
                return this;
            }
            throw new Exception("Failed to resize Structure.");
        }

        /// <summary>
        /// Adds a new Structure to a Pipe Network.
        /// </summary>
        /// <param name="pipeNetwork"></param>
        /// <param name="point"></param>
        /// <param name="partFamily"></param>
        /// <param name="partSize"></param>
        /// <param name="rotation"></param>
        /// <param name="applyRules"></param>
        /// <returns></returns>
        public static Structure ByPoint(PipeNetwork pipeNetwork, Point point, PartFamily partFamily, PartSize partSize, double rotation = 0.0, bool applyRules = false)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MESSAGE, "Structure.ByPoint"));

            if (pipeNetwork is null)
            {
                throw new ArgumentNullException("Pipe Network is null.");
            }

            if (partFamily is null)
            {
                throw new ArgumentNullException("Part Family is null.");
            }

            if (partSize is null)
            {
                throw new ArgumentNullException("Part Size is null.");
            }

            var document = acDynNodes.Document.Current;

            var res = CommonConstruct<Structure, AeccStructure>(
                document,
                (ctx) =>
                {
                    pipeNetwork.AeccPipeNetwork.UpgradeOpen();
                    acDb.ObjectId oid = acDb.ObjectId.Null;
                    pipeNetwork.AeccPipeNetwork.AddStructure(
                        partFamily.InternalObjectId,
                        partSize.InternalObjectId,
                        (acGeom.Point3d)GeometryConversions.DynPointToAcPoint(point, true),
                        rotation,
                        ref oid,
                        applyRules);
                    pipeNetwork.AeccPipeNetwork.DowngradeOpen();
                    return ctx.Transaction.GetObject(oid, acDb.OpenMode.ForWrite) as AeccStructure;
                },
                (ctx, structure, existing) =>
                {
                    if (existing)
                    {
                        if (structure.PartFamilyName != partFamily.Name || structure.PartSizeName != partSize.Name)
                        {
                            return false;
                        }
                        structure.Location = (acGeom.Point3d)GeometryConversions.DynPointToAcPoint(point, true);
                        structure.Rotation = rotation;
                        if (applyRules)
                        {
                            structure.ApplyRules();
                        }
                    }
                    return true;
                });
            return res;
        }

        /// <summary>
        /// Gets the Pipes connected to a Structure.
        /// </summary>
        /// <returns></returns>
        [MultiReturn(new[] { "All", "Incoming", "Outgoing", "Bidirectional" })]
        public Dictionary<string, object> GetConnectedPipes()
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MESSAGE, "Structure.Connections"));

            var allPipes = new List<Pipe>();
            var incomingPipes = new List<Pipe>();
            var outgoingPipes = new List<Pipe>();
            var bidirectionalPipes = new List<Pipe>();
            for (int i = 0; i < AeccStructure.ConnectedPartCount; i++)
            {
                var pipe = Pipe.GetByObjectId(AeccStructure.get_ConnectedPipe(i));
                allPipes.Add(pipe);
                if (AeccStructure.IsConnectedPipeFlowingIn(i))
                {
                    incomingPipes.Add(pipe);
                }
                else if (AeccStructure.IsConnectedPipeFlowingOut(i))
                {
                    outgoingPipes.Add(pipe);
                }
                else
                {
                    bidirectionalPipes.Add(pipe);
                }
            }

            return new Dictionary<string, object>
            {
                { "All", allPipes },
                { "Incoming", incomingPipes },
                { "Outgoing", outgoingPipes },
                { "Bidirectional", bidirectionalPipes }
            };
        }
        #endregion
    }
}
