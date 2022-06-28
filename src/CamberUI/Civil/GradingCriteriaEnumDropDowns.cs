#region references
using System.Collections.Generic;
using civDb = Autodesk.Civil.DatabaseServices;
using civDs = Autodesk.Civil.DataShortcuts;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;

#endregion

namespace Camber.UI
{
    [NodeName("Grading Distance Projection Types")]
    [NodeCategory("Camber.Civil 3D.Grading Criteria.GradingCriteria")]
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
    [NodeCategory("Camber.Civil 3D.Grading Criteria.GradingCriteria")]
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
    [NodeCategory("Camber.Civil 3D.Grading Criteria.GradingCriteria")]
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
    [NodeCategory("Camber.Civil 3D.Grading Criteria.GradingCriteria")]
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
    [NodeCategory("Camber.Civil 3D.Grading Criteria.GradingCriteria")]
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
    [NodeCategory("Camber.Civil 3D.Grading Criteria.GradingCriteria")]
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
    [NodeCategory("Camber.Civil 3D.Grading Criteria.GradingCriteria")]
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
    [NodeCategory("Camber.Civil 3D.Grading Criteria.GradingCriteria")]
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
}