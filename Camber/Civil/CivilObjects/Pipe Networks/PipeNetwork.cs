#region references
using System;
using System.Collections.Generic;
using System.Linq;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civApp = Autodesk.Civil.ApplicationServices;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using AeccPipeNetwork = Autodesk.Civil.DatabaseServices.Network;
using AeccPipe = Autodesk.Civil.DatabaseServices.Pipe;
using AeccStructure = Autodesk.Civil.DatabaseServices.Structure;
using Autodesk.DesignScript.Runtime;
using DynamoServices;
using Dynamo.Graph.Nodes;
using Camber.Civil.PipeNetworks.Parts;
using Camber.Civil.CivilObjects;
#endregion

namespace Camber.Civil.PipeNetworks
{
    [RegisterForTrace]
    public sealed class PipeNetwork : CivilObject
    {
        #region properties
        internal AeccPipeNetwork AeccPipeNetwork => AcObject as AeccPipeNetwork;
        private const string NameIsNullOrEmptyMessage = "Name is null or empty.";
        private const string DocumentIsNullMessage = "Document is null.";

        /// <summary>
        /// Gets a Pipe Network's section layer name.
        /// </summary>
        public string ModifiedPipeNetworkSectionLayer => GetString("ModifiedPipeNetworkSectionLayerName");

        /// <summary>
        /// Gets a Pipe Network's modified Pipe plan layer name, with suffix or prefix.
        /// Returns the same as PipePlanLayerName if there is no suffix or prefix.
        /// </summary>
        public string ModifiedPipePlanLayer => GetString("ModifiedPipePlanLayerName");

        /// <summary>
        /// Gets a Pipe Network's modified Pipe profile layer name, with suffix or prefix.
        /// Returns the same as PipeProfileLayerName if there is no suffix or prefix.
        /// </summary>
        public string ModifiedPipeProfileLayer => GetString("ModifiedPipeProfileLayerName");

        /// <summary>
        /// Gets a Pipe Network's modified Structure plan layer name, with suffix or prefix.
        /// Returns the same as StructurePlanLayerName if there is no suffix or prefix.
        /// </summary>
        public string ModifiedStructurePlanLayer => GetString("ModifiedStructurePlanLayerName");

        /// <summary>
        /// Gets a Pipe Network's modified Structure profile layer name, with suffix or prefix.
        /// Returns the same as StructureProfileLayerName if there is no suffix or prefix.
        /// </summary>
        public string ModifiedStructureProfileLayer => GetString("ModifiedStructureProfileLayerName");
        
        /// <summary>
        /// Gets the Parts List assigned to a Pipe Network.
        /// </summary>
        public PartsList PartsList => PartsList.GetByObjectId(AeccPipeNetwork.PartsListId);

        /// <summary>
        /// Gets Pipe Network's Pipe naming template string.
        /// </summary>
        public string PipeNameTemplate => GetString();

        /// <summary>
        /// Gets a Pipe Network's section layer name.
        /// </summary>
        public string PipeNetworkSectionLayer => GetString("PipeNetworkSectionLayerName");

        /// <summary>
        /// Gets a Pipe Network's Pipe plan layer name.
        /// </summary>
        public string PipePlanLayer => GetString("PipePlanLayerName");

        /// <summary>
        /// Gets a Pipe Network's Pipe profile layer name.
        /// </summary>
        public string PipeProfileLayer => GetString("PipeProfileLayerName");

        /// <summary>
        /// Gets the reference Alignment for a Pipe Network.
        /// </summary>
        public civDynNodes.Alignment ReferenceAlignment
        {
            get
            {
                try
                {
                    return civDynNodes.Selection.AlignmentByName(AeccPipeNetwork.ReferenceAlignmentName, acDynNodes.Document.Current);
                }
                catch
                {
                    return null;
                }
            }
        }
        
        /// <summary>
        /// Gets the reference Surface for a Pipe Network.
        /// </summary>
        public civDynNodes.Surface ReferenceSurface
        {
            get
            {
                try 
                {
                    return civDynNodes.Selection.SurfaceByName(AeccPipeNetwork.ReferenceSurfaceName, acDynNodes.Document.Current);
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets a Pipe Network's Structure naming template string.
        /// </summary>
        public string StructureNameTemplate => GetString();

        /// <summary>
        /// Gets a Pipe Network's Structure plan layer name.
        /// </summary>
        public string StructurePlanLayer => GetString("StructurePlanLayerName");

        /// <summary>
        /// Gets a Pipe Network's Structure profile layer name.
        /// </summary>
        public string StructureProfileLayer => GetString("StructureProfileLayerName");

        /// <summary>
        /// Gets all of the Pipes in a Pipe Network.
        /// </summary>
        public IList<Pipe> Pipes
        {
            get
            {
                var pipes = new List<Pipe>();
                acDb.ObjectIdCollection pipeIds = AeccPipeNetwork.GetPipeIds();
                foreach (acDb.ObjectId oid in pipeIds)
                {
                    pipes.Add(Pipe.GetByObjectId(oid));
                }
                return pipes;
            }
        }

        /// <summary>
        /// Gets all of the Structures in a Pipe Network.
        /// </summary>
        public IList<Structure> Structures
        {
            get
            {
                var structures = new List<Structure>();
                acDb.ObjectIdCollection structureIds = AeccPipeNetwork.GetStructureIds();
                foreach (acDb.ObjectId oid in structureIds)
                {
                    structures.Add(Structure.GetByObjectId(oid));
                }
                return structures;
            }
        }
        #endregion

        #region constructors
        internal PipeNetwork(AeccPipeNetwork aeccPipeNetwork, bool isDynamoOwned = false) : base(aeccPipeNetwork, isDynamoOwned) { }

        [SupressImportIntoVM]
        internal static PipeNetwork GetByObjectId(acDb.ObjectId networkId)
            => CivilObjectSupport.Get<PipeNetwork, AeccPipeNetwork>
            (networkId, (network) => new PipeNetwork(network));

        /// <summary>
        /// Creates a Pipe Network by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PipeNetwork ByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(NameIsNullOrEmptyMessage);
            }

            var document = acDynNodes.Document.Current;

            bool hasNetworkWithSameName = false;
            var res = CommonConstruct<PipeNetwork, AeccPipeNetwork>(
                document,
                (ctx) =>
                {
                    var cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                    var existingNetworks = GetPipeNetworks(document, true);
                    if (existingNetworks.Any(obj => obj.Name == name))
                    {
                        hasNetworkWithSameName = true;
                        return null;
                    }
                    acDb.ObjectId oid = AeccPipeNetwork.Create(cdoc, ref name);
                    return ctx.Transaction.GetObject(oid, acDb.OpenMode.ForWrite) as AeccPipeNetwork;
                },
                (ctx, network, existing) =>
                {
                    if (existing)
                    {
                        var cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                        var existingNetworks = GetPipeNetworks(document, true);
                        if (network.Name != name && !existingNetworks.Any(obj => obj.Name == name))
                        {
                            network.Name = name;
                        }
                        else if (network.Name != name && existingNetworks.Any(obj => obj.Name == name))
                        {
                            hasNetworkWithSameName = true;
                            return false;
                        }
                    }
                    return true;
                });
            if (hasNetworkWithSameName)
            {
                throw new Exception("The document already contains a Pipe Network with the same name.");
            }
            return res;
        }

        /// <summary>
        /// Gets a Pipe Network in the document by name.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="name"></param>
        /// <param name="allowReference">Include data references?</param>
        /// <returns></returns>
        [NodeCategory("Actions")]
        public static PipeNetwork GetPipeNetworkByName(acDynNodes.Document document, string name, bool allowReference = false)
        {
            if (document is null)
            {
                throw new ArgumentNullException(DocumentIsNullMessage);
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(NameIsNullOrEmptyMessage);
            }

            return GetPipeNetworks(document, allowReference)
                .FirstOrDefault(item => item.Name.Equals
                (name, StringComparison.OrdinalIgnoreCase));
        }
        #endregion

        #region methods
        public override string ToString() => $"PipeNetwork(Name = {Name})";

        /// <summary>
        /// Gets the Pipe Networks in the document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="allowReference">Include data references?</param>
        /// <returns></returns>
        public static IList<PipeNetwork> GetPipeNetworks(acDynNodes.Document document, bool allowReference = false)
        {
            if (document is null)
            {
                throw new ArgumentNullException(DocumentIsNullMessage);
            }

            IList<PipeNetwork> networks = new List<PipeNetwork>();

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                civApp.CivilDocument cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);

                acDb.Transaction tr = ctx.Transaction;

                using (acDb.ObjectIdCollection networkIds = cdoc.GetPipeNetworkIds())
                {
                    foreach (acDb.ObjectId oid in networkIds)
                    {
                        if (!oid.IsValid || oid.IsErased || oid.IsEffectivelyErased)
                        {
                            continue;
                        }

                        if (tr.GetObject(oid, acDb.OpenMode.ForRead, false, true) is AeccPipeNetwork network)
                        {
                            if (allowReference || (!network.IsReferenceObject && !network.IsReferenceSubObject))
                            {
                                networks.Add(new PipeNetwork(network));
                            }
                        }
                    }
                }
            }
            return networks;
        }

        /// <summary>
        /// Finds the shortest path between two Parts in the same Pipe Network.
        /// </summary>
        /// <param name="startPart"></param>
        /// <param name="endPart"></param>
        /// <returns></returns>
        [MultiReturn(new[] { "Parts", "Path Length" })]
        public static Dictionary<string, object> FindShortestNetworkPath(Part startPart, Part endPart)
        {
            if (startPart.PipeNetwork.Name != endPart.PipeNetwork.Name)
            {
                throw new InvalidOperationException("The Parts must be in the same Pipe Network.");
            }
            
            double pathLength = 0.0;
            var parts = new List<Part>();

            var document = acDynNodes.Document.Current;
            using(var ctx = new acDynApp.DocumentContext(document.AcDocument.Database))
            {
                var pathParts = AeccPipeNetwork.FindShortestNetworkPath(startPart.InternalObjectId, endPart.InternalObjectId, ref pathLength);
                foreach (acDb.ObjectId oid in pathParts)
                {
                    var aeccPart = ctx.Transaction.GetObject(oid, acDb.OpenMode.ForRead, false);
                    if (aeccPart is AeccPipe)
                    {
                        parts.Add(Pipe.GetByObjectId(oid));
                    }
                    else if (aeccPart is AeccStructure)
                    {
                        parts.Add(Structure.GetByObjectId(oid));
                    }
                    else
                    {
                        throw new InvalidOperationException("Part is not a Pipe or Structure.");
                    }
                }
                if (startPart.Name != endPart.Name)
                {
                    parts.Add(endPart);
                }
            }
            return new Dictionary<string, object>
            {
                { "Parts", parts },
                { "Path Length", pathLength }
            };
        }
        #endregion
    }
}
