#region references
using System.Collections.Generic;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using Camber.Civil.Styles.Labels;
using Camber.Civil.Styles.Labels.Surface;
#endregion

namespace Camber.UI
{
    [NodeName("Surface Elevation Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Surface")]
    [NodeDescription("Select Surface Elevation Label Style.")]
    [IsDesignScriptCompatible]
    public class SurfaceElevationLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "surfaceElevationLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.SurfaceLabelStyles.ToString() + "." + SurfaceLabelStyles.SpotElevationLabelStyles.ToString();
        private static string LabelStyleType = typeof(SurfaceElevationLabelStyle).ToString();

        public SurfaceElevationLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public SurfaceElevationLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }

    [NodeName("Surface Slope Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Surface")]
    [NodeDescription("Select Surface Slope Label Style.")]
    [IsDesignScriptCompatible]
    public class SurfaceSlopeLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "surfaceSlopeLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.SurfaceLabelStyles.ToString() + "." + SurfaceLabelStyles.SlopeLabelStyles.ToString();
        private static string LabelStyleType = typeof(SurfaceSlopeLabelStyle).ToString();

        public SurfaceSlopeLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public SurfaceSlopeLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }

    [NodeName("Surface Contour Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Surface")]
    [NodeDescription("Select Surface Contour Label Style.")]
    [IsDesignScriptCompatible]
    public class SurfaceContourLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "surfaceContourLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.SurfaceLabelStyles.ToString() + "." + SurfaceLabelStyles.ContourLabelStyles.ToString();
        private static string LabelStyleType = typeof(SurfaceContourLabelStyle).ToString();

        public SurfaceContourLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public SurfaceContourLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }

    [NodeName("Surface Watershed Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Surface")]
    [NodeDescription("Select Surface Watershed Label Style.")]
    [IsDesignScriptCompatible]
    public class SurfaceWatershedLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "surfaceWatershedLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.SurfaceLabelStyles.ToString() + "." + SurfaceLabelStyles.WatershedLabelStyles.ToString();
        private static string LabelStyleType = typeof(SurfaceWatershedLabelStyle).ToString();

        public SurfaceWatershedLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public SurfaceWatershedLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }
}
