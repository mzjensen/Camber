#region references
using System.Collections.Generic;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using Camber.Civil.Styles.Labels;
using Camber.Civil.Styles.Labels.Section;
#endregion

namespace Camber.UI
{
    [NodeName("Section Grade Break Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Section")]
    [NodeDescription("Select Section Grade Break Label Style.")]
    [IsDesignScriptCompatible]
    public class SectionGradeBreakLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "sectionGradeBreakLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.SectionLabelStyles.ToString() + "." + SectionLabelStyles.GradeBreakLabelStyles.ToString();
        private static string LabelStyleType = typeof(SectionGradeBreakLabelStyle).ToString();

        public SectionGradeBreakLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public SectionGradeBreakLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }

    [NodeName("Section Major Offset Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Section")]
    [NodeDescription("Select Section Major Offset Label Style.")]
    [IsDesignScriptCompatible]
    public class SectionMajorOffsetLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "sectionMajorOffsetLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.SectionLabelStyles.ToString() + "." + SectionLabelStyles.MajorOffsetLabelStyles.ToString();
        private static string LabelStyleType = typeof(SectionMajorOffsetLabelStyle).ToString();

        public SectionMajorOffsetLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public SectionMajorOffsetLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }

    [NodeName("Section Minor Offset Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Section")]
    [NodeDescription("Select Section Minor Offset Label Style.")]
    [IsDesignScriptCompatible]
    public class SectionMinorOffsetLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "sectionMinorOffsetLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.SectionLabelStyles.ToString() + "." + SectionLabelStyles.MinorOffsetLabelStyles.ToString();
        private static string LabelStyleType = typeof(SectionMinorOffsetLabelStyle).ToString();

        public SectionMinorOffsetLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public SectionMinorOffsetLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }

    [NodeName("Section Segment Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Section")]
    [NodeDescription("Select Section Segment Label Style.")]
    [IsDesignScriptCompatible]
    public class SectionSegmentLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "sectionSegmentLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.SectionLabelStyles.ToString() + "." + SectionLabelStyles.SegmentLabelStyles.ToString();
        private static string LabelStyleType = typeof(SectionSegmentLabelStyle).ToString();

        public SectionSegmentLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public SectionSegmentLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }
}
