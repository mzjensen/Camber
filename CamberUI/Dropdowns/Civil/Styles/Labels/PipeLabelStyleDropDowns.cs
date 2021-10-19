#region references
using System.Collections.Generic;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using Camber.Civil.Styles.Labels;
using Camber.Civil.Styles.Labels.Pipe;
#endregion

namespace Camber.UI
{
    [NodeName("Pipe Plan Profile Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Pipe")]
    [NodeDescription("Select Pipe Plan Profile Label Style.")]
    [IsDesignScriptCompatible]
    public class PipePlanProfileLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "pipePlanProfileLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.PipeLabelStyles.ToString() + "." + PipeLabelStyles.PlanProfileLabelStyles.ToString();
        private static string LabelStyleType = typeof(PipePlanProfileLabelStyle).ToString();

        public PipePlanProfileLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public PipePlanProfileLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }

    [NodeName("Pipe Profile View Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Pipe")]
    [NodeDescription("Select Pipe Profile View Label Style.")]
    [IsDesignScriptCompatible]
    public class PipeProfileViewLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "pipeProfileViewLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.PipeLabelStyles.ToString() + "." + PipeLabelStyles.CrossProfileLabelStyles.ToString();
        private static string LabelStyleType = typeof(PipeProfileViewLabelStyle).ToString();

        public PipeProfileViewLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public PipeProfileViewLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }

    [NodeName("Pipe Section View Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Pipe")]
    [NodeDescription("Select Pipe Section View Label Style.")]
    [IsDesignScriptCompatible]
    public class PipeSectionViewLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "pipeSectionViewLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.PipeLabelStyles.ToString() + "." + PipeLabelStyles.CrossSectionLabelStyles.ToString();
        private static string LabelStyleType = typeof(PipeSectionViewLabelStyle).ToString();

        public PipeSectionViewLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public PipeSectionViewLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }
}
