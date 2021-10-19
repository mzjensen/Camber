#region references
using System.Collections.Generic;
using civDb = Autodesk.Civil.DatabaseServices;
using civDs = Autodesk.Civil.DataShortcuts;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using Camber.Civil.Styles;
using Camber.Civil.Styles.Objects;
using Camber.Civil.Styles.Graphs;
using Autodesk.DesignScript.Runtime;
#endregion

namespace Camber.UI
{
    [NodeName("Alignment Marker Style Types")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.AlignmentStyle")]
    [NodeDescription("Select Alignment Marker Style type.")]
    [IsDesignScriptCompatible]
    public class AlignmentStyleAlignmentMarkerStyleDropDown : EnumDropDownBase
    {
        private const string OutputName = "markerStyleType";

        public AlignmentStyleAlignmentMarkerStyleDropDown() : base(OutputName, typeof(AlignmentMarkerStyles), true) { }

        [JsonConstructor]
        public AlignmentStyleAlignmentMarkerStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(AlignmentMarkerStyles), inPorts, outPorts) { }
    }

    [NodeName("Display Style View Directions")]
    [NodeCategory("Camber.Civil 3D.Styles.Style")]
    [NodeDescription("Select Display Style view direction.")]
    [IsDesignScriptCompatible]
    public class DisplayStyleViewDirectionsDropDown : EnumDropDownBase
    {
        private const string OutputName = "viewDirection";

        public DisplayStyleViewDirectionsDropDown() : base(OutputName, typeof(DisplayStyleViewDirections)) { }

        [JsonConstructor]
        public DisplayStyleViewDirectionsDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(DisplayStyleViewDirections), inPorts, outPorts) { }
    }

    [NodeName("Center Marker Size Options")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.GradingStyle")]
    [NodeDescription("Select Center Marker size option.")]
    [IsDesignScriptCompatible]
    public class GradingStyleCenterMarkerSizeTypeDropDown : EnumDropDownBase
    {
        private const string OutputName = "centerMarkerSizeOption";

        public GradingStyleCenterMarkerSizeTypeDropDown() : base(OutputName, typeof(civDb.Styles.CenterMarkerSizeType), true) { }

        [JsonConstructor]
        public GradingStyleCenterMarkerSizeTypeDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.Styles.CenterMarkerSizeType), inPorts, outPorts) { }
    }

    [NodeName("Group Plot Align Types")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.GroupPlotStyle")]
    [NodeDescription("Select Group Plot align type.")]
    [IsDesignScriptCompatible]
    public class GroupPlotAlignmentTypeDropDown : EnumDropDownBase
    {
        private const string OutputName = "alignType";

        public GroupPlotAlignmentTypeDropDown() : base(OutputName, typeof(civDb.Styles.GroupPlotAlignType)) { }

        [JsonConstructor]
        public GroupPlotAlignmentTypeDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.Styles.GroupPlotAlignType), inPorts, outPorts) { }
    }

    [NodeName("Group Plot Style Start Corner Types")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.GroupPlotStyle")]
    [NodeDescription("Select Group Plot Style start corner type.")]
    [IsDesignScriptCompatible]
    public class GroupPlotStartCornersDropDown : EnumDropDownBase
    {
        private const string OutputName = "startCornerType";

        public GroupPlotStartCornersDropDown() : base(OutputName, typeof(civDb.Styles.GroupPlotStartCornerType), true) { }

        [JsonConstructor]
        public GroupPlotStartCornersDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.Styles.GroupPlotStartCornerType), inPorts, outPorts) { }
    }

    [NodeName("Custom Marker Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.MarkerStyle")]
    [NodeDescription("Select custom marker style.")]
    [IsDesignScriptCompatible]
    public class MarkerStyleCustomMarkerStylesDropDown : EnumDropDownBase
    {
        private const string OutputName = "customMarkerStyle";

        public MarkerStyleCustomMarkerStylesDropDown() : base(OutputName, typeof(civDb.Styles.CustomMarkerType), true) { }

        [JsonConstructor]
        public MarkerStyleCustomMarkerStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.Styles.CustomMarkerType), inPorts, outPorts) { }
    }

    [NodeName("Custom Marker Superimpose Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.MarkerStyle")]
    [NodeDescription("Select custom marker superimpose style.")]
    [IsDesignScriptCompatible]
    public class MarkerStyleCustomMarkerSuperimposeTypeDropDown : EnumDropDownBase
    {
        private const string OutputName = "superimposeStyle";

        public MarkerStyleCustomMarkerSuperimposeTypeDropDown() : base(OutputName, typeof(civDb.Styles.CustomMarkerSuperimposeType), true) { }

        [JsonConstructor]
        public MarkerStyleCustomMarkerSuperimposeTypeDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.Styles.CustomMarkerSuperimposeType), inPorts, outPorts) { }
    }

    [NodeName("Marker Orientation Types")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.MarkerStyle")]
    [NodeDescription("Select marker orientation type.")]
    [IsDesignScriptCompatible]
    public class MarkerStyleMarkerOrientationTypeDropDown : EnumDropDownBase
    {
        private const string OutputName = "orientationType";

        public MarkerStyleMarkerOrientationTypeDropDown() : base(OutputName, typeof(civDb.Styles.MarkerOrientationType), true) { }

        [JsonConstructor]
        public MarkerStyleMarkerOrientationTypeDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.Styles.MarkerOrientationType), inPorts, outPorts) { }
    }

    [NodeName("Marker Size Types")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.MarkerStyle")]
    [NodeDescription("Select marker size type.")]
    [IsDesignScriptCompatible]
    public class MarkerStyleMarkerSizeTypeDropDown : EnumDropDownBase
    {
        private const string OutputName = "sizeType";

        public MarkerStyleMarkerSizeTypeDropDown() : base(OutputName, typeof(civDb.Styles.MarkerSizeType), true) { }

        [JsonConstructor]
        public MarkerStyleMarkerSizeTypeDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.Styles.MarkerSizeType), inPorts, outPorts) { }
    }

    [NodeName("Profile Marker Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.ProfileStyle")]
    [NodeDescription("Select Alignment Marker Style.")]
    [IsDesignScriptCompatible]
    public class ProfileStyleProfileMarkerStyleDropDown : EnumDropDownBase
    {
        private const string OutputName = "profileMarkerStyle";

        public ProfileStyleProfileMarkerStyleDropDown() : base(OutputName, typeof(ProfileMarkerStyles), true) { }

        [JsonConstructor]
        public ProfileStyleProfileMarkerStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(ProfileMarkerStyles), inPorts, outPorts) { }
    }

    [NodeName("Pipe Style Centerline Options")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.PipeStyle")]
    [NodeDescription("Select Pipe Style centerline option.")]
    [IsDesignScriptCompatible]
    public class PipeStyleCenterlineTypeDropDown : EnumDropDownBase
    {
        private const string OutputName = "centerlineOption";

        public PipeStyleCenterlineTypeDropDown() : base(OutputName, typeof(civDb.Styles.PipeCenterlineType), true) { }

        [JsonConstructor]
        public PipeStyleCenterlineTypeDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.Styles.PipeCenterlineType), inPorts, outPorts) { }
    }

    [NodeName("Pipe Style Centerline Width Options")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.PipeStyle")]
    [NodeDescription("Select Pipe Style centerline width option.")]
    [IsDesignScriptCompatible]
    public class PipeStyleCenterlineWidthTypeDropDown : EnumDropDownBase
    {
        private const string OutputName = "centerlineWidthOption";

        public PipeStyleCenterlineWidthTypeDropDown() : base(OutputName, typeof(civDb.Styles.PipeCenterlineWidthType), true) { }

        [JsonConstructor]
        public PipeStyleCenterlineWidthTypeDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.Styles.PipeCenterlineWidthType), inPorts, outPorts) { }
    }

    [NodeName("Pipe Style Size Options")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.PipeStyle")]
    [NodeDescription("Select Pipe Style size option.")]
    [IsDesignScriptCompatible]
    public class PipeStyleSizeOptionsDropDown : EnumDropDownBase
    {
        private const string OutputName = "sizeOption";

        public PipeStyleSizeOptionsDropDown() : base(OutputName, typeof(civDb.Styles.PipeUserDefinedType), true) { }

        [JsonConstructor]
        public PipeStyleSizeOptionsDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.Styles.PipeUserDefinedType), inPorts, outPorts) { }
    }

    [NodeName("Pipe Style Hatch Options")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.PipeStyle")]
    [NodeDescription("Select Pipe Style hatch option.")]
    [IsDesignScriptCompatible]
    public class PipeStyleHatchOptionsDropDown : EnumDropDownBase
    {
        private const string OutputName = "hatchOption";

        public PipeStyleHatchOptionsDropDown() : base(OutputName, typeof(civDb.Styles.PipeHatchType), true) { }

        [JsonConstructor]
        public PipeStyleHatchOptionsDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.Styles.PipeHatchType), inPorts, outPorts) { }
    }


    [NodeName("Pipe Style Wall Size Options")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.PipeStyle")]
    [NodeDescription("Select Pipe Style wall size option.")]
    [IsDesignScriptCompatible]
    public class PipeStyleWallSizeOptionsDropDown : EnumDropDownBase
    {
        private const string OutputName = "wallSizeOption";

        public PipeStyleWallSizeOptionsDropDown() : base(OutputName, typeof(civDb.Styles.PipeWallSizeType), true) { }

        [JsonConstructor]
        public PipeStyleWallSizeOptionsDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.Styles.PipeWallSizeType), inPorts, outPorts) { }
    }

    [NodeName("Structure Style Block Insertion Locations")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.StructureStyle")]
    [NodeDescription("Select Structure Style block insertion location options.")]
    [IsDesignScriptCompatible]
    public class StructureStyleWallBlockInsertLocationDropDown : EnumDropDownBase
    {
        private const string OutputName = "insertionLocation";

        public StructureStyleWallBlockInsertLocationDropDown() : base(OutputName, typeof(civDb.Styles.StructureInsertionLocation)) { }

        [JsonConstructor]
        public StructureStyleWallBlockInsertLocationDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.Styles.StructureInsertionLocation), inPorts, outPorts) { }
    }

    [NodeName("Structure Style Size Options")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.StructureStyle")]
    [NodeDescription("Select Structure Style size options.")]
    [IsDesignScriptCompatible]
    public class StructureStyleSizeOptionsDropDown : EnumDropDownBase
    {
        private const string OutputName = "sizeOption";

        public StructureStyleSizeOptionsDropDown() : base(OutputName, typeof(civDb.Styles.StructureSizeOptionsType), true) { }

        [JsonConstructor]
        public StructureStyleSizeOptionsDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.Styles.StructureSizeOptionsType), inPorts, outPorts) { }
    }


    [NodeName("Structure Style Profile Section View Options")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.StructureStyle")]
    [NodeDescription("Select Structure Style profile or section view options.")]
    [IsDesignScriptCompatible]
    public class StructureStyleViewOptionsDropDown : EnumDropDownBase
    {
        private const string OutputName = "profileSectionViewOption";

        public StructureStyleViewOptionsDropDown() : base(OutputName, typeof(civDb.Styles.StructureViewType), true) { }

        [JsonConstructor]
        public StructureStyleViewOptionsDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.Styles.StructureViewType), inPorts, outPorts) { }
    }

    [NodeName("Structure Style Plan View Options")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.StructureStyle")]
    [NodeDescription("Select Structure Style plan view options.")]
    [IsDesignScriptCompatible]
    public class StructureStylePlanViewOptionsDropDown : EnumDropDownBase
    {
        private const string OutputName = "planViewOption";

        public StructureStylePlanViewOptionsDropDown() : base(OutputName, typeof(civDb.Styles.StructurePlanViewType), true) { }

        [JsonConstructor]
        public StructureStylePlanViewOptionsDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.Styles.StructurePlanViewType), inPorts, outPorts) { }
    }

    [NodeName("Structure Style Model View Options")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.StructureStyle")]
    [NodeDescription("Select Structure Style model view options.")]
    [IsDesignScriptCompatible]
    public class StructureStyleModelViewOptionsDropDown : EnumDropDownBase
    {
        private const string OutputName = "modelViewOption";

        public StructureStyleModelViewOptionsDropDown() : base(OutputName, typeof(civDb.Styles.StructureModelViewOptionType), true) { }

        [JsonConstructor]
        public StructureStyleModelViewOptionsDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.Styles.StructureModelViewOptionType), inPorts, outPorts) { }
    }


    [NodeName("Structure Style Simple Solid Types")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.StructureStyle")]
    [NodeDescription("Select Structure Style simple solid type.")]
    [IsDesignScriptCompatible]
    public class StructureStyleSimpleSolidTypesDropDown : EnumDropDownBase
    {
        private const string OutputName = "simpleSolidType";

        public StructureStyleSimpleSolidTypesDropDown() : base(OutputName, typeof(civDb.Styles.StructureSimpleSolidType), true) { }

        [JsonConstructor]
        public StructureStyleSimpleSolidTypesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.Styles.StructureSimpleSolidType), inPorts, outPorts) { }
    }

    [NodeName("Axis Tick Style Justifications")]
    [NodeCategory("Camber.Civil 3D.Styles.Graphs.AxisTickStyle")]
    [NodeDescription("Select Axis Tick Style justification.")]
    [IsDesignScriptCompatible]
    public class AxisTickStyleJustificationTypesDropDown : EnumDropDownBase
    {
        private const string OutputName = "justification";

        public AxisTickStyleJustificationTypesDropDown() : base(OutputName, typeof(civDb.Styles.AxisTickJustificationType), true) { }

        [JsonConstructor]
        public AxisTickStyleJustificationTypesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.Styles.AxisTickJustificationType), inPorts, outPorts) { }
    }


    [NodeName("Graph Style Directions")]
    [NodeCategory("Camber.Civil 3D.Styles.Graphs.GraphStyle")]
    [NodeDescription("Select Graph Style direction type.")]
    [IsDesignScriptCompatible]
    public class GraphStyleDirectionsDropDown : EnumDropDownBase
    {
        private const string OutputName = "direction";

        public GraphStyleDirectionsDropDown() : base(OutputName, typeof(civDb.Styles.GraphDirectionType), true) { }

        [JsonConstructor]
        public GraphStyleDirectionsDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.Styles.GraphDirectionType), inPorts, outPorts) { }
    }

    [NodeName("Graph Style Title Justifications")]
    [NodeCategory("Camber.Civil 3D.Styles.Graphs.GraphStyle")]
    [NodeDescription("Select Graph Style title justification options.")]
    [IsDesignScriptCompatible]
    public class GraphStyleTitleJustificationsDropDown : EnumDropDownBase
    {
        private const string OutputName = "justification";

        public GraphStyleTitleJustificationsDropDown() : base(OutputName, typeof(civDb.Styles.GraphTitleJustificationType), true) { }

        [JsonConstructor]
        public GraphStyleTitleJustificationsDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.Styles.GraphTitleJustificationType), inPorts, outPorts) { }
    }

    [NodeName("Grid Style Title Locations")]
    [NodeCategory("Camber.Civil 3D.Styles.Graphs.GridStyle")]
    [NodeDescription("Select Grid Style title location option.")]
    [IsDesignScriptCompatible]
    public class GridStyleTitleLocationsDropDown : EnumDropDownBase
    {
        private const string OutputName = "location";

        public GridStyleTitleLocationsDropDown() : base(OutputName, typeof(civDb.Styles.GraphTitleLocationType)) { }

        [JsonConstructor]
        public GridStyleTitleLocationsDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.Styles.GraphTitleLocationType), inPorts, outPorts) { }
    }

    [NodeName("Material Quantity Types")]
    [NodeCategory("Camber.Civil 3D.Styles.Quantity Takeoff.QTOCriteria")]
    [NodeDescription("Select material quantity type.")]
    [IsDesignScriptCompatible]
    public class MaterialQuantityTypesDropDown : EnumDropDownBase
    {
        private const string OutputName = "quantityType";

        public MaterialQuantityTypesDropDown() : base(OutputName, typeof(civDb.MaterialQuantityType)) { }

        [JsonConstructor]
        public MaterialQuantityTypesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.MaterialQuantityType), inPorts, outPorts) { }
    }

    [NodeName("Material Conditions")]
    [NodeCategory("Camber.Civil 3D.Styles.Quantity Takeoff.QTOCriteriaData")]
    [NodeDescription("Select material condition.")]
    [IsDesignScriptCompatible]
    public class MaterialConditionsDropDown : EnumDropDownBase
    {
        private const string OutputName = "condition";

        public MaterialConditionsDropDown() : base(OutputName, typeof(civDb.MaterialConditionType)) { }

        [JsonConstructor]
        public MaterialConditionsDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.MaterialConditionType), inPorts, outPorts) { }
    }

    [NodeName("Subentity Style Types")]
    [NodeCategory("Camber.Civil 3D.Styles.Code Sets.CodeSetStyleItem")]
    [NodeDescription("Select subentity style type.")]
    [IsDesignScriptCompatible]
    public class SubentityStyleTypesDropDown : EnumDropDownBase
    {
        private const string OutputName = "subentityStyleType";

        public SubentityStyleTypesDropDown() : base(OutputName, typeof(civDb.Styles.SubassemblySubentityStyleType)) { }

        [JsonConstructor]
        public SubentityStyleTypesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.Styles.SubassemblySubentityStyleType), inPorts, outPorts) { }
    }

    [NodeName("Data Shortcut Entity Types")]
    [NodeCategory("Camber.Civil 3D.DataShortcuts")]
    [NodeDescription("Select data shortcut entity type.")]
    [IsDesignScriptCompatible]
    [IsVisibleInDynamoLibrary(false)]
    public class DataShortcutEntityTypesDropDown : EnumDropDownBase
    {
        private const string OutputName = "entityType";

        public DataShortcutEntityTypesDropDown() : base(OutputName, typeof(civDs.DataShortcutEntityType), true) { }

        [JsonConstructor]
        public DataShortcutEntityTypesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDs.DataShortcutEntityType), inPorts, outPorts) { }
    }

    [NodeName("Object Style Types")]
    [NodeCategory("Camber.Civil 3D.Styles.Style")]
    [NodeDescription("Select Object Style type.")]
    [IsDesignScriptCompatible]
    public class ObjectStyleTypesDropDown : EnumDropDownBase
    {
        private const string OutputName = "styleType";

        public ObjectStyleTypesDropDown() : base(OutputName, typeof(ObjectStyleCollections), true, true) { }

        [JsonConstructor]
        public ObjectStyleTypesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(ObjectStyleCollections), inPorts, outPorts) { }
    }

    [NodeName("Graph Style Types")]
    [NodeCategory("Camber.Civil 3D.Styles.Style")]
    [NodeDescription("Select Graph Style type.")]
    [IsDesignScriptCompatible]
    public class GraphStyleTypesDropDown : EnumDropDownBase
    {
        private const string OutputName = "styleType";

        public GraphStyleTypesDropDown() : base(OutputName, typeof(GraphStyleCollections), true, true) { }

        [JsonConstructor]
        public GraphStyleTypesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(GraphStyleCollections), inPorts, outPorts) { }
    }

    [NodeName("Alignment Design Check Types")]
    [NodeCategory("Camber.Civil 3D.Styles.Design Checks")]
    [NodeDescription("Select Alignment Design Check type.")]
    [IsDesignScriptCompatible]
    public class AlignmentDesignCheckTypesDropDown : EnumDropDownBase
    {
        private const string OutputName = "checkType";

        public AlignmentDesignCheckTypesDropDown() : base(OutputName, typeof(Autodesk.Civil.AlignmentDesignCheckType), true) { }

        [JsonConstructor]
        public AlignmentDesignCheckTypesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(Autodesk.Civil.AlignmentDesignCheckType), inPorts, outPorts) { }
    }

    [NodeName("Profile Design Check Types")]
    [NodeCategory("Camber.Civil 3D.Styles.Design Checks")]
    [NodeDescription("Select Profile Design Check type.")]
    [IsDesignScriptCompatible]
    public class ProfileDesignCheckTypesDropDown : EnumDropDownBase
    {
        private const string OutputName = "checkType";

        public ProfileDesignCheckTypesDropDown() : base(OutputName, typeof(Autodesk.Civil.ProfileDesignCheckType), true) { }

        [JsonConstructor]
        public ProfileDesignCheckTypesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(Autodesk.Civil.ProfileDesignCheckType), inPorts, outPorts) { }
    }

    [NodeName("Profile Design Check Curve Types")]
    [NodeCategory("Camber.Civil 3D.Styles.Design Checks.ProfileDesignCheckSet")]
    [NodeDescription("Select Profile Design Check curve type.")]
    [IsDesignScriptCompatible]
    public class ProfileDesignCheckCurveTypesDropDown : EnumDropDownBase
    {
        private const string OutputName = "curveCheckType";

        public ProfileDesignCheckCurveTypesDropDown() : base(OutputName, typeof(civDb.Styles.ProfileDesignCheckCurveType), true) { }

        [JsonConstructor]
        public ProfileDesignCheckCurveTypesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.Styles.ProfileDesignCheckCurveType), inPorts, outPorts) { }
    }


    [NodeName("Grading Distance Projection Types")]
    [NodeCategory("Camber.Civil 3D.Styles.Grading Criteria.GradingCriteria")]
    [NodeDescription("Select grading distance projection type.")]
    [IsDesignScriptCompatible]
    public class GradingDistanceProjectionTypesDropDown : EnumDropDownBase
    {
        private const string OutputName = "projectionType";

        public GradingDistanceProjectionTypesDropDown() : base(OutputName, typeof(Autodesk.Civil.GradingDistanceProjectionType), true) { }

        [JsonConstructor]
        public GradingDistanceProjectionTypesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(Autodesk.Civil.GradingDistanceProjectionType), inPorts, outPorts) { }
    }

    [NodeName("Grading Elevation Projection Types")]
    [NodeCategory("Camber.Civil 3D.Styles.Grading Criteria.GradingCriteria")]
    [NodeDescription("Select grading elevation projection type.")]
    [IsDesignScriptCompatible]
    public class GradingElevationProjectionTypesDropDown : EnumDropDownBase
    {
        private const string OutputName = "projectionType";

        public GradingElevationProjectionTypesDropDown() : base(OutputName, typeof(Autodesk.Civil.GradingElevationProjectionType), true) { }

        [JsonConstructor]
        public GradingElevationProjectionTypesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(Autodesk.Civil.GradingElevationProjectionType), inPorts, outPorts) { }
    }


    [NodeName("Grading Slope Format Types")]
    [NodeCategory("Camber.Civil 3D.Styles.Grading Criteria.GradingCriteria")]
    [NodeDescription("Select grading slope format type.")]
    [IsDesignScriptCompatible]
    public class GradingSlopeFormatTypesDropDown : EnumDropDownBase
    {
        private const string OutputName = "formatType";

        public GradingSlopeFormatTypesDropDown() : base(OutputName, typeof(Autodesk.Civil.GradingSlopeFormatType)) { }

        [JsonConstructor]
        public GradingSlopeFormatTypesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(Autodesk.Civil.GradingSlopeFormatType), inPorts, outPorts) { }
    }


    [NodeName("Grading Interior Corner Overlap Solution Types")]
    [NodeCategory("Camber.Civil 3D.Styles.Grading Criteria.GradingCriteria")]
    [NodeDescription("Select grading interior corner overlap solution type.")]
    [IsDesignScriptCompatible]
    public class GradingInteriorOverlapSolutionTypesDropDown : EnumDropDownBase
    {
        private const string OutputName = "overlapSolution";

        public GradingInteriorOverlapSolutionTypesDropDown() : base(OutputName, typeof(Autodesk.Civil.GradingInteriorCornerOverlapSolutionType), true) { }

        [JsonConstructor]
        public GradingInteriorOverlapSolutionTypesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(Autodesk.Civil.GradingInteriorCornerOverlapSolutionType), inPorts, outPorts) { }
    }

    [NodeName("Grading Relative Elevation Projection Types")]
    [NodeCategory("Camber.Civil 3D.Styles.Grading Criteria.GradingCriteria")]
    [NodeDescription("Select grading relative elevation projection type.")]
    [IsDesignScriptCompatible]
    public class GradingRelativeElevationProjectionTypesDropDown : EnumDropDownBase
    {
        private const string OutputName = "projectionType";

        public GradingRelativeElevationProjectionTypesDropDown() : base(OutputName, typeof(Autodesk.Civil.GradingRelativeElevationProjectionType)) { }

        [JsonConstructor]
        public GradingRelativeElevationProjectionTypesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(Autodesk.Civil.GradingRelativeElevationProjectionType), inPorts, outPorts) { }
    }

    [NodeName("Grading Search Orders")]
    [NodeCategory("Camber.Civil 3D.Styles.Grading Criteria.GradingCriteria")]
    [NodeDescription("Select grading search order.")]
    [IsDesignScriptCompatible]
    public class GradingSearchOrderTypesDropDown : EnumDropDownBase
    {
        private const string OutputName = "searchOrder";

        public GradingSearchOrderTypesDropDown() : base(OutputName, typeof(Autodesk.Civil.GradingSearchOrderType), true) { }

        [JsonConstructor]
        public GradingSearchOrderTypesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(Autodesk.Civil.GradingSearchOrderType), inPorts, outPorts) { }
    }


    [NodeName("Grading Surface Projection Types")]
    [NodeCategory("Camber.Civil 3D.Styles.Grading Criteria.GradingCriteria")]
    [NodeDescription("Select grading surface projection type.")]
    [IsDesignScriptCompatible]
    public class GradingSurfaceProjectionTypesDropDown : EnumDropDownBase
    {
        private const string OutputName = "projectionType";

        public GradingSurfaceProjectionTypesDropDown() : base(OutputName, typeof(Autodesk.Civil.GradingSurfaceProjectionType), true) { }

        [JsonConstructor]
        public GradingSurfaceProjectionTypesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(Autodesk.Civil.GradingSurfaceProjectionType), inPorts, outPorts) { }
    }

    [NodeName("Grading Target Types")]
    [NodeCategory("Camber.Civil 3D.Styles.Grading Criteria.GradingCriteria")]
    [NodeDescription("Select grading target type.")]
    [IsDesignScriptCompatible]
    public class GradingTargetTypesDropDown : EnumDropDownBase
    {
        private const string OutputName = "targetType";

        public GradingTargetTypesDropDown() : base(OutputName, typeof(Autodesk.Civil.GradingTargetType), true) { }

        [JsonConstructor]
        public GradingTargetTypesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(Autodesk.Civil.GradingTargetType), inPorts, outPorts) { }
    }

    [NodeName("Label Style Component Types")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.LabelStyle")]
    [NodeDescription("Select Label Style component type.")]
    [IsDesignScriptCompatible]
    public class LabelStyleComponentTypesDropDown : EnumDropDownBase
    {
        private const string OutputName = "componentType";

        public LabelStyleComponentTypesDropDown() : base(OutputName, typeof(civDb.Styles.LabelStyleComponentType), true) { }

        [JsonConstructor]
        public LabelStyleComponentTypesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.Styles.LabelStyleComponentType), inPorts, outPorts) { }
    }

    [NodeName("Reference Text Component Types")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.LabelStyle")]
    [NodeDescription("Select reference text component type.")]
    [IsDesignScriptCompatible]
    public class ReferenceTextComponentTypesDropDown : EnumDropDownBase
    {
        private const string OutputName = "componentType";

        public ReferenceTextComponentTypesDropDown() : base(OutputName, typeof(civDb.Styles.ReferenceTextComponentSelectedType), true) { }

        [JsonConstructor]
        public ReferenceTextComponentTypesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.Styles.ReferenceTextComponentSelectedType), inPorts, outPorts) { }
    }

    [NodeName("Text For Each Component Types")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.LabelStyle")]
    [NodeDescription("Select text for each component type.")]
    [IsDesignScriptCompatible]
    public class TextForEachComponentTypesDropDown : EnumDropDownBase
    {
        private const string OutputName = "componentType";

        public TextForEachComponentTypesDropDown() : base(OutputName, typeof(civDb.Styles.TextForEachComponentSelectedType), true) { }

        [JsonConstructor]
        public TextForEachComponentTypesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.Styles.TextForEachComponentSelectedType), inPorts, outPorts) { }
    }
}