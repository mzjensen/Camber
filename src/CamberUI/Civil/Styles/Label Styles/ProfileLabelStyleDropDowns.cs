#region references
using System.Collections.Generic;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using Camber.Civil.Styles.Labels;
using Camber.Civil.Styles.Labels.Profile;
#endregion

namespace Camber.UI
{
    [NodeName("Profile Curve Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Profile")]
    [NodeDescription("Select Profile Curve Label Style.")]
    [IsDesignScriptCompatible]
    public class ProfileCurveLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "profileCurveLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.ProfileLabelStyles.ToString() + "." + ProfileLabelStyles.CurveLabelStyles.ToString();
        private static string LabelStyleType = typeof(ProfileCurveLabelStyle).ToString();

        public ProfileCurveLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public ProfileCurveLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }

    [NodeName("Profile Line Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Profile")]
    [NodeDescription("Select Profile Line Label Style.")]
    [IsDesignScriptCompatible]
    public class ProfileLineLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "profileLineLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.ProfileLabelStyles.ToString() + "." + ProfileLabelStyles.LineLabelStyles.ToString();
        private static string LabelStyleType = typeof(ProfileLineLabelStyle).ToString();

        public ProfileLineLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public ProfileLineLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }

    [NodeName("Profile Grade Break Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Profile")]
    [NodeDescription("Select Profile Grade Break Label Style.")]
    [IsDesignScriptCompatible]
    public class ProfileGradeBreakLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "profileGradeBreakLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.ProfileLabelStyles.ToString() + "." + ProfileLabelStyles.GradeBreakLabelStyles.ToString();
        private static string LabelStyleType = typeof(ProfileGradeBreakLabelStyle).ToString();

        public ProfileGradeBreakLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public ProfileGradeBreakLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }

    [NodeName("Profile Horizontal Geometry Point Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Profile")]
    [NodeDescription("Select Profile Horizontal Geometry Point Label Style.")]
    [IsDesignScriptCompatible]
    public class ProfileHorizontalGeometryPointLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "profileHorizontalGeometryPointLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.ProfileLabelStyles.ToString() + "." + ProfileLabelStyles.HorizontalGeometryPointLabelStyles.ToString();
        private static string LabelStyleType = typeof(ProfileHorizontalGeometryPointLabelStyle).ToString();

        public ProfileHorizontalGeometryPointLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public ProfileHorizontalGeometryPointLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }

    [NodeName("Profile Major Station Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Profile")]
    [NodeDescription("Select Profile Major Station Label Style.")]
    [IsDesignScriptCompatible]
    public class ProfileMajorStationLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "profileMajorStationLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.ProfileLabelStyles.ToString() + "." + ProfileLabelStyles.MajorStationLabelStyles.ToString();
        private static string LabelStyleType = typeof(ProfileMajorStationLabelStyle).ToString();

        public ProfileMajorStationLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public ProfileMajorStationLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }

    [NodeName("Profile Minor Station Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Profile")]
    [NodeDescription("Select Profile Minor Station Label Style.")]
    [IsDesignScriptCompatible]
    public class ProfileMinorStationLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "profileMinorStationLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.ProfileLabelStyles.ToString() + "." + ProfileLabelStyles.MinorStationLabelStyles.ToString();
        private static string LabelStyleType = typeof(ProfileMinorStationLabelStyle).ToString();

        public ProfileMinorStationLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public ProfileMinorStationLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }
}
