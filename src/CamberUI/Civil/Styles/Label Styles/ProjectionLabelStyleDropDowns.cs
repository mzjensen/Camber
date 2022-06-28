#region references
using System.Collections.Generic;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using Camber.Civil.Styles.Labels;
using Camber.Civil.Styles.Labels.Projection;
#endregion

namespace Camber.UI
{
    [NodeName("Profile View Projection Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Projection")]
    [NodeDescription("Select Profile View Projection Label Style.")]
    [IsDesignScriptCompatible]
    public class ProfileViewProjectionLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "profileViewProjectionLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.ProjectionLabelStyles.ToString() + "." + ProjectionLabelStyles.ProfileViewProjectionLabelStyles.ToString();
        private static string LabelStyleType = typeof(ProfileViewProjectionLabelStyle).ToString();

        public ProfileViewProjectionLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public ProfileViewProjectionLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }

    [NodeName("Section View Projection Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Projection")]
    [NodeDescription("Select Section View Projection Label Style.")]
    [IsDesignScriptCompatible]
    public class SectionViewProjectionLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "sectionViewProjectionLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.ProjectionLabelStyles.ToString() + "." + ProjectionLabelStyles.SectionViewProjectionLabelStyles.ToString();
        private static string LabelStyleType = typeof(SectionViewProjectionLabelStyle).ToString();

        public SectionViewProjectionLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public SectionViewProjectionLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }
}
