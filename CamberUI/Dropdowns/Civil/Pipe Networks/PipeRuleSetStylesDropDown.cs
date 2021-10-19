#region references
using System.Collections.Generic;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using Camber.Civil.PipeNetworks;
#endregion

namespace Camber.UI
{
    [NodeName("Pipe Rule Sets")]
    [NodeCategory("Camber.Civil 3D.Pipe Networks.RuleSet")]
    [NodeDescription("Select Pipe Rule Set.")]
    [IsDesignScriptCompatible]
    public class PipeRuleSetDropDown : StyleDropDownBase
    {
        private const string OutputName = "pipeRuleSet";
        private static string StyleType = typeof(RuleSet).ToString();
        private static string StyleCollection = DesignCheckSetCollections.AlignmentDesignCheckSets.ToString();

        public PipeRuleSetDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public PipeRuleSetDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }
}
