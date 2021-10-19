#region references
using System.Collections.Generic;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using Camber.Civil.Styles.Labels.General;
#endregion

namespace Camber.UI
{
    [NodeName("General Curve Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.General")]
    [NodeDescription("Select General Curve Label Style.")]
    [IsDesignScriptCompatible]
    public class GeneralCurveLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "generalCurveLabelStyle";
        private static string LabelStyleCollection = GeneralLabelStyles.GeneralCurveLabelStyles.ToString();
        private static string LabelStyleType = typeof(GeneralCurveLabelStyle).ToString();

        public GeneralCurveLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public GeneralCurveLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }

    [NodeName("General Line Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.General")]
    [NodeDescription("Select General Line Label Style.")]
    [IsDesignScriptCompatible]
    public class GeneralLineLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "generalLineLabelStyle";
        private static string LabelStyleCollection = GeneralLabelStyles.GeneralLineLabelStyles.ToString();
        private static string LabelStyleType = typeof(GeneralLineLabelStyle).ToString();

        public GeneralLineLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public GeneralLineLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }

    [NodeName("General Marker Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.General")]
    [NodeDescription("Select General Marker Label Style.")]
    [IsDesignScriptCompatible]
    public class GeneralMarkerLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "generalMarkerLabelStyle";
        private static string LabelStyleCollection = GeneralLabelStyles.GeneralMarkerLabelStyles.ToString();
        private static string LabelStyleType = typeof(GeneralMarkerLabelStyle).ToString();

        public GeneralMarkerLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public GeneralMarkerLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }

    [NodeName("General Note Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.General")]
    [NodeDescription("Select General Note Label Style.")]
    [IsDesignScriptCompatible]
    public class GeneralNoteLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "generalNoteLabelStyle";
        private static string LabelStyleCollection = GeneralLabelStyles.GeneralNoteLabelStyles.ToString();
        private static string LabelStyleType = typeof(GeneralNoteLabelStyle).ToString();

        public GeneralNoteLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public GeneralNoteLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }

    [NodeName("General Link Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.General")]
    [NodeDescription("Select General Link Label Style.")]
    [IsDesignScriptCompatible]
    public class GeneralLinkLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "generalLinkLabelStyle";
        private static string LabelStyleCollection = GeneralLabelStyles.GeneralLinkLabelStyles.ToString();
        private static string LabelStyleType = typeof(GeneralLinkLabelStyle).ToString();

        public GeneralLinkLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public GeneralLinkLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }

    [NodeName("General Shape Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.General")]
    [NodeDescription("Select General Shape Label Style.")]
    [IsDesignScriptCompatible]
    public class GeneralShapeLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "generalShapeLabelStyle";
        private static string LabelStyleCollection = GeneralLabelStyles.GeneralShapeLabelStyles.ToString();
        private static string LabelStyleType = typeof(GeneralShapeLabelStyle).ToString();

        public GeneralShapeLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public GeneralShapeLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }
}
