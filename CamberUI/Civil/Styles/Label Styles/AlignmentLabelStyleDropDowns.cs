#region references
using System.Collections.Generic;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using Camber.Civil.Styles.Labels;
using Camber.Civil.Styles.Labels.Alignment;
#endregion

namespace Camber.UI
{
    [NodeName("Alignment Station Offset Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Alignment")]
    [NodeDescription("Select Alignment Station Offset Label Style.")]
    [IsDesignScriptCompatible]
    public class AlignmentStationOffsetLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "alignmentStationOffsetLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.AlignmentLabelStyles.ToString() + "." + AlignmentLabelStyles.StationOffsetLabelStyles.ToString();
        private static string LabelStyleType = typeof(AlignmentStationOffsetLabelStyle).ToString();

        public AlignmentStationOffsetLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public AlignmentStationOffsetLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }

    [NodeName("Alignment Line Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Alignment")]
    [NodeDescription("Select Alignment Line Label Style.")]
    [IsDesignScriptCompatible]
    public class AlignmentLineLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "alignmentLineLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.AlignmentLabelStyles.ToString() + "." + AlignmentLabelStyles.LineLabelStyles.ToString();
        private static string LabelStyleType = typeof(AlignmentLineLabelStyle).ToString();

        public AlignmentLineLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public AlignmentLineLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }

    [NodeName("Alignment Curve Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Alignment")]
    [NodeDescription("Select Alignment Curve Label Style.")]
    [IsDesignScriptCompatible]
    public class AlignmentCurveLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "alignmentCurveLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.AlignmentLabelStyles.ToString() + "." + AlignmentLabelStyles.CurveLabelStyles.ToString();
        private static string LabelStyleType = typeof(AlignmentCurveLabelStyle).ToString();

        public AlignmentCurveLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public AlignmentCurveLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }
}
