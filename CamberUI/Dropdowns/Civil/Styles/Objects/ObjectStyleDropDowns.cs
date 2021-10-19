#region references
using System.Collections.Generic;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using Camber.Civil.Styles.Objects;
#endregion

namespace Camber.UI
{
    [NodeName("Alignment Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.AlignmentStyle")]
    [NodeDescription("Select Alignment Style.")]
    [IsDesignScriptCompatible]
    public class AlignmentStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "alignmentStyle";
        private static string StyleType = typeof(AlignmentStyle).ToString();
        private static string StyleCollection = ObjectStyleCollections.AlignmentStyles.ToString();

        public AlignmentStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public AlignmentStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("Assembly Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.AssemblyStyle")]
    [NodeDescription("Select Assembly Style.")]
    [IsDesignScriptCompatible]
    public class AssemblyStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "assemblyStyle";
        private static string StyleType = typeof(AssemblyStyle).ToString();
        private static string StyleCollection = ObjectStyleCollections.AssemblyStyles.ToString();

        public AssemblyStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public AssemblyStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("Building Site Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.BuildingSiteStyle")]
    [NodeDescription("Select Building Site Style.")]
    [IsDesignScriptCompatible]
    public class BuildingSiteStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "buildingSiteStyle";
        private static string StyleType = typeof(BuildingSiteStyle).ToString();
        private static string StyleCollection = ObjectStyleCollections.BuildingSiteStyles.ToString();

        public BuildingSiteStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public BuildingSiteStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("Catchment Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.CatchmentStyle")]
    [NodeDescription("Select Catchment Style.")]
    [IsDesignScriptCompatible]
    public class CatchmentStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "catchmentStyle";
        private static string StyleType = typeof(CatchmentStyle).ToString();
        private static string StyleCollection = ObjectStyleCollections.CatchmentStyles.ToString();

        public CatchmentStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public CatchmentStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("Corridor Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.CorridorStyle")]
    [NodeDescription("Select Corridor Style.")]
    [IsDesignScriptCompatible]
    public class CorridorStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "corridorStyle";
        private static string StyleType = typeof(CorridorStyle).ToString();
        private static string StyleCollection = ObjectStyleCollections.CorridorStyles.ToString();

        public CorridorStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public CorridorStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("Feature Line Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.FeatureLineStyle")]
    [NodeDescription("Select Feature Line Style.")]
    [IsDesignScriptCompatible]
    public class FeatureLineStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "featureLineStyle";
        private static string StyleType = typeof(FeatureLineStyle).ToString();
        private static string StyleCollection = ObjectStyleCollections.FeatureLineStyles.ToString();

        public FeatureLineStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public FeatureLineStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("Grading Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.GradingStyle")]
    [NodeDescription("Select Grading Style.")]
    [IsDesignScriptCompatible]
    public class GradingStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "gradingStyle";
        private static string StyleType = typeof(GradingStyle).ToString();
        private static string StyleCollection = ObjectStyleCollections.GradingStyles.ToString();

        public GradingStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public GradingStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("Group Plot Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.GroupPlotStyle")]
    [NodeDescription("Select Group Plot Style.")]
    [IsDesignScriptCompatible]
    public class GroupPlotStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "groupPlotStyle";
        private static string StyleType = typeof(GroupPlotStyle).ToString();
        private static string StyleCollection = ObjectStyleCollections.GroupPlotStyles.ToString();

        public GroupPlotStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public GroupPlotStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("Interference Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.InterferenceStyle")]
    [NodeDescription("Select Interference Style.")]
    [IsDesignScriptCompatible]
    public class InterferenceStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "interferenceStyle";
        private static string StyleType = typeof(InterferenceStyle).ToString();
        private static string StyleCollection = ObjectStyleCollections.InterferenceStyles.ToString();

        public InterferenceStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public InterferenceStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("Intersection Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.IntersectionStyle")]
    [NodeDescription("Select Intersection Style.")]
    [IsDesignScriptCompatible]
    public class IntersectionStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "intersectionStyle";
        private static string StyleType = typeof(IntersectionStyle).ToString();
        private static string StyleCollection = ObjectStyleCollections.IntersectionStyles.ToString();

        public IntersectionStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public IntersectionStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("Link Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.LinkStyle")]
    [NodeDescription("Select Link Style.")]
    [IsDesignScriptCompatible]
    public class LinkStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "linkStyle";
        private static string StyleType = typeof(LinkStyle).ToString();
        private static string StyleCollection = ObjectStyleCollections.LinkStyles.ToString();

        public LinkStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public LinkStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("Marker Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.MarkerStyle")]
    [NodeDescription("Select Marker Style.")]
    [IsDesignScriptCompatible]
    public class MarkerStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "markerStyle";
        private static string StyleType = typeof(MarkerStyle).ToString();
        private static string StyleCollection = ObjectStyleCollections.MarkerStyles.ToString();

        public MarkerStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public MarkerStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("Mass Haul Line Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.MassHaulLineStyle")]
    [NodeDescription("Select Mass Haul Line Style.")]
    [IsDesignScriptCompatible]
    public class MassHaulLineStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "massHaulLineStyle";
        private static string StyleType = typeof(MassHaulLineStyle).ToString();
        private static string StyleCollection = ObjectStyleCollections.MassHaulLineStyles.ToString();

        public MassHaulLineStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public MassHaulLineStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("Match Line Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.MatchLineStyle")]
    [NodeDescription("Select Match Line Style.")]
    [IsDesignScriptCompatible]
    public class MatchLineStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "matchLineStyle";
        private static string StyleType = typeof(MatchLineStyle).ToString();
        private static string StyleCollection = ObjectStyleCollections.MatchLineStyles.ToString();

        public MatchLineStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public MatchLineStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("Parcel Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.ParcelStyle")]
    [NodeDescription("Select Parcel Style.")]
    [IsDesignScriptCompatible]
    public class ParcelStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "parcelStyle";
        private static string StyleType = typeof(ParcelStyle).ToString();
        private static string StyleCollection = ObjectStyleCollections.ParcelStyles.ToString();

        public ParcelStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public ParcelStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("Pipe Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.PipeStyle")]
    [NodeDescription("Select Pipe Style.")]
    [IsDesignScriptCompatible]
    public class PipeStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "pipeStyle";
        private static string StyleType = typeof(PipeStyle).ToString();
        private static string StyleCollection = ObjectStyleCollections.PipeStyles.ToString();

        public PipeStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public PipeStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("Profile Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.ProfileStyle")]
    [NodeDescription("Select Profile Style.")]
    [IsDesignScriptCompatible]
    public class ProfileStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "profileStyle";
        private static string StyleType = typeof(ProfileStyle).ToString();
        private static string StyleCollection = ObjectStyleCollections.ProfileStyles.ToString();

        public ProfileStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public ProfileStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("Sample Line Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.SampleLineStyle")]
    [NodeDescription("Select Sample Line Style.")]
    [IsDesignScriptCompatible]
    public class SampleLineStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "sampleLineStyle";
        private static string StyleType = typeof(SampleLineStyle).ToString();
        private static string StyleCollection = ObjectStyleCollections.SampleLineStyles.ToString();

        public SampleLineStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public SampleLineStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("Section Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.SectionStyle")]
    [NodeDescription("Select Section Style.")]
    [IsDesignScriptCompatible]
    public class SectionStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "sectionStyle";
        private static string StyleType = typeof(SectionStyle).ToString();
        private static string StyleCollection = ObjectStyleCollections.SectionStyles.ToString();

        public SectionStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public SectionStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("Shape Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.ShapeStyle")]
    [NodeDescription("Select Shape Style.")]
    [IsDesignScriptCompatible]
    public class ShapeStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "shapeStyle";
        private static string StyleType = typeof(ShapeStyle).ToString();
        private static string StyleCollection = ObjectStyleCollections.ShapeStyles.ToString();

        public ShapeStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public ShapeStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("Sheet Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.SheetStyle")]
    [NodeDescription("Select Sheet Style.")]
    [IsDesignScriptCompatible]
    public class SheetStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "sheetStyle";
        private static string StyleType = typeof(SheetStyle).ToString();
        private static string StyleCollection = ObjectStyleCollections.SheetStyles.ToString();

        public SheetStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public SheetStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("Slope Pattern Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.SlopePatternStyle")]
    [NodeDescription("Select Sheet Style.")]
    [IsDesignScriptCompatible]
    public class SlopePatternStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "slopePatternStyle";
        private static string StyleType = typeof(SlopePatternStyle).ToString();
        private static string StyleCollection = ObjectStyleCollections.SlopePatternStyles.ToString();

        public SlopePatternStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public SlopePatternStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("Structure Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.StructureStyle")]
    [NodeDescription("Select Structure Style.")]
    [IsDesignScriptCompatible]
    public class StructureStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "structureStyle";
        private static string StyleType = typeof(StructureStyle).ToString();
        private static string StyleCollection = ObjectStyleCollections.StructureStyles.ToString();

        public StructureStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public StructureStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("Surface Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.SurfaceStyle")]
    [NodeDescription("Select Surface Style.")]
    [IsDesignScriptCompatible]
    public class SurfaceStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "surfaceStyle";
        private static string StyleType = typeof(SurfaceStyle).ToString();
        private static string StyleCollection = ObjectStyleCollections.SurfaceStyles.ToString();

        public SurfaceStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public SurfaceStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("View Frame Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.ViewFrameStyle")]
    [NodeDescription("Select View Frame Style.")]
    [IsDesignScriptCompatible]
    public class ViewFrameStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "viewFrameStyle";
        private static string StyleType = typeof(ViewFrameStyle).ToString();
        private static string StyleCollection = ObjectStyleCollections.ViewFrameStyles.ToString();

        public ViewFrameStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public ViewFrameStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }
}