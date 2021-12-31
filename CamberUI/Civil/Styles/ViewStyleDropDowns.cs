#region references
using System.Collections.Generic;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using Camber.Civil.Styles.Views;
#endregion

namespace Camber.UI
{
    [NodeName("Cant View Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.View Styles")]
    [NodeDescription("Select Cant View Style.")]
    [IsDesignScriptCompatible]
    public class CantViewStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "cantViewStyle";
        private static string StyleType = typeof(CantViewStyle).ToString();
        private static string StyleCollection = ViewStyleCollections.CantViewStyles.ToString();

        public CantViewStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public CantViewStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("Mass Haul View Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.View Styles")]
    [NodeDescription("Select Mass Haul View Style.")]
    [IsDesignScriptCompatible]
    public class MassHaulViewStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "massHaulViewStyle";
        private static string StyleType = typeof(MassHaulViewStyle).ToString();
        private static string StyleCollection = ViewStyleCollections.MassHaulViewStyles.ToString();

        public MassHaulViewStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public MassHaulViewStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("Profile View Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.View Styles")]
    [NodeDescription("Select Profile View Style.")]
    [IsDesignScriptCompatible]
    public class ProfileViewStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "profileViewStyle";
        private static string StyleType = typeof(ProfileViewStyle).ToString();
        private static string StyleCollection = ViewStyleCollections.ProfileViewStyles.ToString();

        public ProfileViewStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public ProfileViewStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("Section View Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.View Styles")]
    [NodeDescription("Select Section View Style.")]
    [IsDesignScriptCompatible]
    public class SectionViewStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "sectionViewStyle";
        private static string StyleType = typeof(SectionViewStyle).ToString();
        private static string StyleCollection = ViewStyleCollections.SectionViewStyles.ToString();

        public SectionViewStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public SectionViewStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }


    [NodeName("Superelevation View Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.View Styles")]
    [NodeDescription("Select Superelevation View Style.")]
    [IsDesignScriptCompatible]
    public class SuperelevationViewStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "superelevationViewStyle";
        private static string StyleType = typeof(SuperelevationViewStyle).ToString();
        private static string StyleCollection = ViewStyleCollections.SuperelevationViewStyles.ToString();

        public SuperelevationViewStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public SuperelevationViewStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }
}