#region references
using System.Collections.Generic;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using Camber.Civil.Styles.Labels;
using Camber.Civil.Styles.Labels.MatchLine;
#endregion

namespace Camber.UI
{
    [NodeName("Match Line Left Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Match Line")]
    [NodeDescription("Select Match Line Left Label Style.")]
    [IsDesignScriptCompatible]
    public class MatchLineLeftLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "matchLineLeftLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.MatchLineLabelStyles.ToString() + "." + MatchLineLabelStyles.LeftLabelStyles.ToString();
        private static string LabelStyleType = typeof(MatchLineLeftLabelStyle).ToString();

        public MatchLineLeftLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public MatchLineLeftLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }

    [NodeName("Match Line Right Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Match Line")]
    [NodeDescription("Select Match Line Right Label Style.")]
    [IsDesignScriptCompatible]
    public class MatchLineRightLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "matchLineRightLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.MatchLineLabelStyles.ToString() + "." + MatchLineLabelStyles.RightLabelStyles.ToString();
        private static string LabelStyleType = typeof(MatchLineRightLabelStyle).ToString();

        public MatchLineRightLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public MatchLineRightLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }
}
