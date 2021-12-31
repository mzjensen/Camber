#region references
using System.Collections.Generic;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using Camber.Civil.Styles.Labels;
#endregion

namespace Camber.UI
{
    [NodeName("Structure Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.StructureLabelStyle")]
    [NodeDescription("Select Structure Label Style.")]
    [IsDesignScriptCompatible]
    public class StructureLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "structureLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.StructureLabelStyles.ToString() + ".LabelStyles";
        private static string LabelStyleType = typeof(StructureLabelStyle).ToString();

        public StructureLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public StructureLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }
}
