#region references
using System.Collections.Generic;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using Camber.Civil.Styles;
using Camber.Civil.GradingCriteria;

#endregion

namespace Camber.UI
{
    [NodeName("Grading Criteria Sets")]
    [NodeCategory("Camber.Civil 3D.Grading Criteria")]
    [NodeDescription("Select Grading Criteria Set.")]
    [IsDesignScriptCompatible]
    public class GradingCriteriaSetDropDown : StyleDropDownBase
    {
        private const string OutputName = "gradingCriteriaSet";
        private static string StyleType = typeof(GradingCriteriaSet).ToString();
        private static string StyleCollection = StyleCollections.GradingCriteriaSets.ToString();

        public GradingCriteriaSetDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public GradingCriteriaSetDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }
}