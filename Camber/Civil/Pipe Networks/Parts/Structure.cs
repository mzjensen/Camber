#region references
using System;
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acGeom = Autodesk.AutoCAD.Geometry;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using civDb = Autodesk.Civil.DatabaseServices;
using AeccStructure = Autodesk.Civil.DatabaseServices.Structure;
using AeccPipe = Autodesk.Civil.DatabaseServices.Pipe;
using Autodesk.DesignScript.Runtime;
using Autodesk.DesignScript.Geometry;
using Camber.Utils;
#endregion

namespace Camber.Civil.PipeNetworks.Parts
{
    public sealed class Structure : Part
    {
        #region properties
        internal AeccStructure AeccStructure => AcObject as AeccStructure;

        /// <summary>
        /// Gets whether a Structure's rim elevation should be automatically adjusted when the reference Surface changes.
        /// </summary>
        public bool AutomaticRimSurfaceAdjustment => GetBool();

        /// <summary>
        /// Gets the value that determines how close to a Structure’s rim a Pipe can be positioned.
        /// This property only applies to two-tiered junction Structures.
        /// It is similar to Vertical Pipe Clearance, but only describes the transition zone between the access cylinder and the larger barrel cylinder.
        /// </summary>
        public double BarrelPipeClearance => GetDouble();

        /// <summary>
        /// Gets how a Structure's sump should be adjusted when the elevations of its connected Pipes are changed.
        /// </summary>
        public string ControlSumpBy => GetString();

        /// <summary>
        /// Gets the model or type of grate used for catch basin Structures.
        /// </summary>
        public string Cover => GetString();

        /// <summary>
        /// Gets the outer diameter or width of a Structure.
        /// </summary>
        public double OuterDiameterOrWidth => GetDouble("DiameterOrWidth");

        /// <summary>
        /// Gets the floor thickness of a Structure.
        /// </summary>
        public double FloorThickness => GetDouble();

        /// <summary>
        /// Gets the model or type of a Structure's frame.
        /// </summary>
        public string Frame => GetString();

        /// <summary>
        /// Gets the diameter of a Structure's frame.
        /// </summary>
        public double FrameDiameter => GetDouble();

        /// <summary>
        /// Gets the height of a Structure's frame.
        /// </summary>
        public double FrameHeight => GetDouble();

        /// <summary>
        /// Gets the model or type of a Structure's grate.
        /// </summary>
        public string Grate => GetString();

        /// <summary>
        /// Gets the thickness of the base of a headwall.
        /// This property only applies to inlet-outlet Structures.
        /// </summary>
        public double HeadwallBaseThickness => GetDouble();

        /// <summary>
        /// Gets the width of the base of a headwall.
        /// This property only applies to inlet-outlet Structures.
        /// </summary>
        public double HeadwallBaseWidth => GetDouble();

        /// <summary>
        /// Gets the overall height of a Structure.
        /// </summary>
        public double Height => GetDouble();

        /// <summary>
        /// Gets the inner diamter or width of a Structure.
        /// </summary>
        public double InnerDiameterOrWidth => GetDouble();

        /// <summary>
        /// Gets the inner length of a Structure.
        /// </summary>
        public double InnerLength => GetDouble();

        /// <summary>
        /// Gets the length of a Structure.
        /// </summary>
        public double Length => GetDouble();

        /// <summary>
        /// Gets the location of a Structure.
        /// </summary>
        public Point Location => GeometryConversions.AcPointToDynPoint(AeccStructure.Location);

        /// <summary>
        /// Gets the offset of a Structure from its reference Alignment.
        /// </summary>
        public double Offset => GetDouble();

        /// <summary>
        /// Gets the depth to the bottom of the lowest Pipe connected to a Structure.
        /// </summary>
        public double PipeLowestBottomDepth => GetDouble();

        /// <summary>
        /// Gets the depth to the top of uppermost Pipe connected to a Structure.
        /// </summary>
        public double PipeUpperTopDepth => GetDouble();

        /// <summary>
        /// Gets the rim elevation of a Structure.
        /// </summary>
        public double RimElevation => GetDouble();

        /// <summary>
        /// Gets the distance between the sump and rim of a Structure.
        /// </summary>
        public double RimToSumpHeight => GetDouble();

        /// <summary>
        /// Gets the rotation of a Structure.
        /// </summary>
        public double Rotation => GetDouble();

        /// <summary>
        /// Gets the station of a Structure along its reference Alignment.
        /// </summary>
        public double Station => GetDouble();

        /// <summary>
        /// Gets the sump depth of a Structure.
        /// </summary>
        public double SumpDepth => GetDouble();

        /// <summary>
        /// Gets the sump elevation of a Structure.
        /// </summary>
        public double SumpElevation => GetDouble();

        /// <summary>
        /// Gets a Structure's surface adjustment value.
        /// </summary>
        public double SurfaceAdjustmentValue => GetDouble();

        /// <summary>
        /// Gets the required clearance from the top outside of the uppermost pipe connected to a Structure to the Structure's rim.
        /// This is defined in the structure catalog and ensures that Pipes enter the Structure at an appropriate elevation.
        /// For example, this property prevents a Pipe from entering through the cone of a Structure.
        /// </summary>
        public double VerticalPipeClearance => GetDouble();
        #endregion

        #region constructors
        internal Structure(AeccStructure aeccStructure, bool isDynamoOwned = false) : base(aeccStructure, isDynamoOwned) { }

        [SupressImportIntoVM]
        internal static Structure GetByObjectId(acDb.ObjectId structureId)
            => CivilObjectSupport.Get<Structure, AeccStructure>
            (structureId, (structure) => new Structure(structure));

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
                    acDb.ObjectId oid = acDb.ObjectId.Null;
                    pipeNetwork.AeccPipeNetwork.AddStructure(
                        partFamily.InternalObjectId,
                        partSize.InternalObjectId,
                        (acGeom.Point3d)GeometryConversions.DynPointToAcPoint(point, true),
                        rotation,
                        ref oid,
                        applyRules);
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
        #endregion

        #region methods
        public override string ToString() => $"Structure(Name = {Name})";

        /// <summary>
        /// Gets the Pipes connected to a Structure.
        /// </summary>
        /// <returns></returns>
        [MultiReturn(new[] { "All", "Incoming", "Outgoing", "Bidirectional" })]
        public Dictionary<string, object> GetConnectedPipes()
        {
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

            bool openedForWrite = AeccStructure.IsWriteEnabled;
            if (!openedForWrite) AeccStructure.UpgradeOpen();
            AeccStructure.ConnectToPipe(pipe.InternalObjectId, position);
            if (!openedForWrite) AeccStructure.DowngradeOpen();
            return this;
        }

        /// <summary>
        /// Disconnects a Structure from a specified Pipe.
        /// </summary>
        /// <param name="pipe"></param>
        /// <returns></returns>
        public Structure DisconnectFromPipe(Pipe pipe)
        {
            bool openedForWrite = AeccStructure.IsWriteEnabled;
            if (!openedForWrite) AeccStructure.UpgradeOpen();
            AeccStructure.Disconnect(pipe.InternalObjectId);
            if (!openedForWrite) AeccStructure.DowngradeOpen();
            return this;
        }

        /// <summary>
        /// Determines if a point is within the region of a Structure.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsPointInsideStructureRegion(Point point) => AeccStructure.IsPointInsideStructureRegion((acGeom.Point3d)GeometryConversions.DynPointToAcPoint(point, true));

        /// <summary>
        /// Attempts to resize a Structure by Pipe depths.
        /// </summary>
        /// <returns></returns>
        public Structure ResizeByPipeDepths()
        {
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
    }
}
