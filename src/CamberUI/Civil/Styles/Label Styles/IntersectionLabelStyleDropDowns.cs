#region references
using System.Collections.Generic;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using Camber.Civil.Styles.Labels;
#endregion

namespace Camber.UI
{
    [NodeName("Intersection Location Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.IntersectionLocationLabelStyle")]
    [NodeDescription("Select Intersection Location Label Style.")]
    [IsDesignScriptCompatible]
    public class IntersectionLocationLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "intersectionLocationLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.IntersectionLabelStyles.ToString() + ".LocationLabelStyles";
        private static string LabelStyleType = typeof(IntersectionLocationLabelStyle).ToString();

        public IntersectionLocationLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public IntersectionLocationLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }
}
