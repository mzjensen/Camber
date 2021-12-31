#region references
using System.Collections.Generic;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using Camber.Civil.Styles.Labels;
#endregion

namespace Camber.UI
{
    [NodeName("Point Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.PointLabelStyle")]
    [NodeDescription("Select Point Label Style.")]
    [IsDesignScriptCompatible]
    public class PointLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "pointLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.PointLabelStyles.ToString() + ".LabelStyles";
        private static string LabelStyleType = typeof(PointLabelStyle).ToString();

        public PointLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public PointLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }
}
