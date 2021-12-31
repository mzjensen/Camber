#region references
using System.Collections.Generic;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using Camber.Civil.Styles.Labels;
using Camber.Civil.Styles.Labels.ProfileView;
#endregion

namespace Camber.UI
{
    [NodeName("Profile View Depth Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Profile View")]
    [NodeDescription("Select Profile View Depth Label Style.")]
    [IsDesignScriptCompatible]
    public class ProfileViewDepthLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "profileViewDepthLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.ProfileViewLabelStyles.ToString() + "." + ProfileViewLabelStyles.DepthLabelStyles.ToString();
        private static string LabelStyleType = typeof(ProfileViewDepthLabelStyle).ToString();

        public ProfileViewDepthLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public ProfileViewDepthLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }

    [NodeName("Profile View Station Elevation Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Profile View")]
    [NodeDescription("Select Profile View Station Elevation Label Style.")]
    [IsDesignScriptCompatible]
    public class ProfileViewStationElevationLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "profileViewStationElevationLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.ProfileViewLabelStyles.ToString() + "." + ProfileViewLabelStyles.StationElevationLabelStyles.ToString();
        private static string LabelStyleType = typeof(ProfileViewStationElevationLabelStyle).ToString();

        public ProfileViewStationElevationLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public ProfileViewStationElevationLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }
}
