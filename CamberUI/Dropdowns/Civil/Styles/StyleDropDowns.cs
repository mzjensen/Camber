#region references
using System.Collections.Generic;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using Camber.Civil.Styles;
using Camber.Civil.Styles.CodeSets;
using Camber.Civil.Styles.GradingCriteria;
using Camber.Civil.Styles.QTO;
#endregion

namespace Camber.UI
{
    [NodeName("Code Set Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Code Sets")]
    [NodeDescription("Select Code Set Style.")]
    [IsDesignScriptCompatible]
    public class CodeSetStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "codeSetStyle";
        private static string StyleType = typeof(CodeSetStyle).ToString();
        private static string StyleCollection = StyleCollections.CodeSetStyles.ToString();

        public CodeSetStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public CodeSetStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("Grading Criteria Sets")]
    [NodeCategory("Camber.Civil 3D.Styles.Grading Criteria")]
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

    [NodeName("Quantity Takeoff Criterias")]
    [NodeCategory("Camber.Civil 3D.Styles.Quantity Takeoff")]
    [NodeDescription("Select Quantity Takeoff Criteria.")]
    [IsDesignScriptCompatible]
    public class QTOCriteriaDropDown : StyleDropDownBase
    {
        private const string OutputName = "qTOCriteria";
        private static string StyleType = typeof(QTOCriteria).ToString();
        private static string StyleCollection = StyleCollections.QuantityTakeoffCriterias.ToString();

        public QTOCriteriaDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public QTOCriteriaDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }
}