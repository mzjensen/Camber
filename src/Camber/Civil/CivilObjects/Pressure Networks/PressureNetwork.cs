#region references
using System;
using System.Collections.Generic;
using System.Linq;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civApp = Autodesk.Civil.ApplicationServices;
using AeccPressureNetwork = Autodesk.Civil.DatabaseServices.PressurePipeNetwork;
using AeccPressurePipeRun = Autodesk.Civil.DatabaseServices.PressurePipeRun;
using Autodesk.DesignScript.Runtime;
using DynamoServices;
using Dynamo.Graph.Nodes;
using Camber.Civil.PressureNetworks.Parts;
using Camber.Civil.CivilObjects;
using Camber.Properties;

#endregion

namespace Camber.Civil.PressureNetworks
{
    [RegisterForTrace]
    public sealed class PressureNetwork : CivilObject
    {
        #region properties
        internal AeccPressureNetwork AeccPressureNetwork => AcObject as AeccPressureNetwork;
        private const string DocumentIsNullMessage = "Document is null.";
        private const string NameIsNullOrEmptyMessage = "Name is null or empty.";

        /// <summary>
        /// Gets the collection of Pressure Pipe Runs that are part of the Pressure Network.
        /// </summary>
        public IList<PressurePipeRun> PipeRuns
        {
            get
            {
                var runs = new List<PressurePipeRun>();
                foreach (AeccPressurePipeRun aeccRun in AeccPressureNetwork.PipeRuns)
                {
                    runs.Add(new PressurePipeRun(aeccRun, this));
                }
                return runs;
            }
        }
        #endregion

        #region constructors
        internal PressureNetwork(AeccPressureNetwork aeccPressureNetwork, bool isDynamoOwned = false) : base(aeccPressureNetwork, isDynamoOwned) { }

        [SupressImportIntoVM]
        internal static PressureNetwork GetByObjectId(acDb.ObjectId networkId)
            => CivilObjectSupport.Get<PressureNetwork, AeccPressureNetwork>
            (networkId, (network) => new PressureNetwork(network));
        #endregion

        #region methods
        public override string ToString() => $"PressureNetwork(Name = {Name})";
        #endregion

        #region deprecated
        /// <summary>
        /// Gets the collection of Pressure Appurtenances that belong to the Pressure Network.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PressureNetworks.PressureNetwork.Appurtenances",
            "Autodesk.Civil.DynamoNodes.PressureNetwork.Appurtenances")]
        public IList<PressureAppurtenance> Appurtenances
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "PressureNetwork.Appurtenances"));

                var appurtenances = new List<PressureAppurtenance>();
                foreach (acDb.ObjectId oid in AeccPressureNetwork.GetAppurtenanceIds())
                {
                    appurtenances.Add(PressureAppurtenance.GetByObjectId(oid));
                }
                return appurtenances;
            }
        }

        /// <summary>
        /// Creates a Pressure Network by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.Civil.PressureNetworks.PressureNetwork.ByName",
            "Autodesk.Civil.DynamoNodes.PressureNetwork.ByName")]
        public static PressureNetwork ByName(string name)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "PressureNetwork.ByName"));

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(NameIsNullOrEmptyMessage);
            }

            var document = acDynNodes.Document.Current;

            bool hasNetworkWithSameName = false;
            var res = CommonConstruct<PressureNetwork, AeccPressureNetwork>(
                document,
                (ctx) =>
                {
                    var existingNetworks = GetPressureNetworks(document, true);
                    if (existingNetworks.Any(obj => obj.Name == name))
                    {
                        hasNetworkWithSameName = true;
                        return null;
                    }
                    acDb.ObjectId oid = AeccPressureNetwork.Create(ctx.Database, name);
                    return ctx.Transaction.GetObject(oid, acDb.OpenMode.ForWrite) as AeccPressureNetwork;
                },
                (ctx, network, existing) =>
                {
                    if (existing)
                    {
                        var cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                        var existingNetworks = GetPressureNetworks(document, true);
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
                throw new Exception("The document already contains a Pressure Network with the same name.");
            }
            return res;
        }

        /// <summary>
        /// Gets the collection of Pressure Pipes that belong to the Pressure Network.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PressureNetworks.PressureNetwork.PressurePipes",
            "Autodesk.Civil.DynamoNodes.PressureNetwork.PressurePipes")]
        public IList<PressurePipe> PressurePipes
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "PressureNetwork.PressurePipes"));

                var pipes = new List<PressurePipe>();
                foreach (acDb.ObjectId oid in AeccPressureNetwork.GetPipeIds())
                {
                    pipes.Add(PressurePipe.GetByObjectId(oid));
                }
                return pipes;
            }
        }

        /// <summary>
        /// Gets the collection of Pressure Fittings that belong to the Pressure Network.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PressureNetworks.PressureNetwork.Fittings",
            "Autodesk.Civil.DynamoNodes.PressureNetwork.Fittings")]
        public IList<PressureFitting> Fittings
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "PressureNetwork.Fittings"));

                var fittings = new List<PressureFitting>();
                foreach (acDb.ObjectId oid in AeccPressureNetwork.GetFittingIds())
                {
                    fittings.Add(PressureFitting.GetByObjectId(oid));
                }
                return fittings;
            }
        }

        /// <summary>
        /// Gets the Pressure Pipe Networks in the document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="allowReference">Include data references?</param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.Civil.PressureNetworks.PressureNetwork.GetPressureNetworks",
            "Autodesk.Civil.DynamoNodes.PressureNetwork.GetPressureNetworks")]
        public static IList<PressureNetwork> GetPressureNetworks(acDynNodes.Document document, bool allowReference = false)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "PressureNetwork.GetPressureNetworks"));

            if (document is null)
            {
                throw new ArgumentNullException(DocumentIsNullMessage);
            }

            IList<PressureNetwork> networks = new List<PressureNetwork>();

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                civApp.CivilDocument cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);

                acDb.Transaction tr = ctx.Transaction;

                using (acDb.ObjectIdCollection networkIds = civApp.CivilDocumentPressurePipesExtension.GetPressurePipeNetworkIds(cdoc))
                {
                    foreach (acDb.ObjectId oid in networkIds)
                    {
                        if (!oid.IsValid || oid.IsErased || oid.IsEffectivelyErased)
                        {
                            continue;
                        }

                        if (tr.GetObject(oid, acDb.OpenMode.ForRead, false, true) is AeccPressureNetwork network)
                        {
                            if (allowReference || (!network.IsReferenceObject && !network.IsReferenceSubObject))
                            {
                                networks.Add(new PressureNetwork(network));
                            }
                        }
                    }
                }
            }
            return networks;
        }

        /// <summary>
        /// Gets a Pressure Network in the document by name.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="name"></param>
        /// <param name="allowReference">Include data references?</param>
        /// <returns></returns>
        [NodeCategory("Actions")]
        public static PressureNetwork GetPressureNetworkByName(acDynNodes.Document document, string name, bool allowReference = false)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MESSAGE, "PressureNetwork.GetPressureNetworkByName"));

            if (document is null)
            {
                throw new ArgumentNullException(DocumentIsNullMessage);
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(NameIsNullOrEmptyMessage);
            }

            return GetPressureNetworks(document, allowReference)
                .FirstOrDefault(item => item.Name.Equals
                    (name, StringComparison.OrdinalIgnoreCase));
        }
        #endregion
    }
}
