#region references
using System.Collections.Generic;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using Camber.Civil.Styles.Labels;
using Camber.Civil.Styles.Labels.Catchment;
#endregion

namespace Camber.UI
{
    [NodeName("Catchment Area Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Catchment")]
    [NodeDescription("Select Catchment Area Label Style.")]
    [IsDesignScriptCompatible]
    public class CatchmentAreaLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "catchmentAreaLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.CatchmentLabelStyles.ToString() + "." + CatchmentLabelStyles.AreaLabelStyles.ToString();
        private static string LabelStyleType = typeof(CatchmentAreaLabelStyle).ToString();

        public CatchmentAreaLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public CatchmentAreaLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }

    [NodeName("Catchment Flow Segment Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Catchment")]
    [NodeDescription("Select Catchment Flow Segment Label Style.")]
    [IsDesignScriptCompatible]
    public class CatchmentFlowSegmentLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "catchmentFlowSegmentLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.CatchmentLabelStyles.ToString() + "." + CatchmentLabelStyles.FlowSegmentLabelStyles.ToString();
        private static string LabelStyleType = typeof(CatchmentFlowSegmentLabelStyle).ToString();

        public CatchmentFlowSegmentLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public CatchmentFlowSegmentLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }
}
