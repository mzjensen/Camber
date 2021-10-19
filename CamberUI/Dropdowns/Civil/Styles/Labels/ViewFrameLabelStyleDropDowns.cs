#region references
using System.Collections.Generic;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using Camber.Civil.Styles.Labels;
#endregion

namespace Camber.UI
{
    [NodeName("View Frame Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.ViewFrameLabelStyle")]
    [NodeDescription("Select View Frame Label Style.")]
    [IsDesignScriptCompatible]
    public class ViewFrameLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "viewFrameLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.ViewFrameLabelStyles.ToString() + ".LabelStyles";
        private static string LabelStyleType = typeof(ViewFrameLabelStyle).ToString();

        public ViewFrameLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public ViewFrameLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }
}
