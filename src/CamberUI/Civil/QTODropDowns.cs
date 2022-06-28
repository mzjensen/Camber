#region references
using System.Collections.Generic;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using Camber.Civil.Styles;
using Camber.Civil.QTO;

#endregion

namespace Camber.UI
{
    [NodeName("Quantity Takeoff Criterias")]
    [NodeCategory("Camber.Civil 3D.Quantity Takeoff")]
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