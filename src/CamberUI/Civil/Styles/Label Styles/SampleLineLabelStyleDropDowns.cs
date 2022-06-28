#region references
using System.Collections.Generic;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using Camber.Civil.Styles.Labels;
#endregion

namespace Camber.UI
{
    [NodeName("Sample Line Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.SampleLineLabelStyle")]
    [NodeDescription("Select Sample Line Label Style.")]
    [IsDesignScriptCompatible]
    public class SampleLineLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "sampleLineLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.SampleLineLabelStyles.ToString() + ".LabelStyles";
        private static string LabelStyleType = typeof(SampleLineLabelStyle).ToString();

        public SampleLineLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public SampleLineLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }
}
