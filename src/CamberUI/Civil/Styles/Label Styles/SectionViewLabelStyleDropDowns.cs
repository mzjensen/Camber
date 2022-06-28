#region references
using System.Collections.Generic;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using Camber.Civil.Styles.Labels;
using Camber.Civil.Styles.Labels.SectionView;
#endregion

namespace Camber.UI
{
    [NodeName("Section View Depth Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Section View")]
    [NodeDescription("Select Section View Depth Label Style.")]
    [IsDesignScriptCompatible]
    public class SectionViewDepthLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "sectionViewDepthLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.SectionViewLabelStyles.ToString() + "." + SectionViewLabelStyles.GradeLabelStyles.ToString();
        private static string LabelStyleType = typeof(SectionViewDepthLabelStyle).ToString();

        public SectionViewDepthLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public SectionViewDepthLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }

    [NodeName("Section View Offset Elevation Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Section View")]
    [NodeDescription("Select Section View Offset Elevation Label Style.")]
    [IsDesignScriptCompatible]
    public class SectionViewOffsetElevationLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "sectionViewOffsetElevationLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.SectionViewLabelStyles.ToString() + "." + SectionViewLabelStyles.OffsetElevationLabelStyles.ToString();
        private static string LabelStyleType = typeof(SectionViewOffsetElevationLabelStyle).ToString();

        public SectionViewOffsetElevationLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public SectionViewOffsetElevationLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }
}
