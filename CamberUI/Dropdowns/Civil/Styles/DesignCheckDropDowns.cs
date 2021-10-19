#region references
using System.Collections.Generic;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using Camber.Civil.Styles.DesignChecks;
#endregion

namespace Camber.UI
{
    [NodeName("Alignment Design Check Sets")]
    [NodeCategory("Camber.Civil 3D.Styles.Design Checks")]
    [NodeDescription("Select Alignment Design Check Set.")]
    [IsDesignScriptCompatible]
    public class AlignmentDesignCheckSetDropDown : StyleDropDownBase
    {
        private const string OutputName = "alignmentDesignCheckSet";
        private static string StyleType = typeof(AlignmentDesignCheckSet).ToString();
        private static string StyleCollection = DesignCheckSetCollections.AlignmentDesignCheckSets.ToString();

        public AlignmentDesignCheckSetDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public AlignmentDesignCheckSetDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("Profile Design Check Sets")]
    [NodeCategory("Camber.Civil 3D.Styles.Design Checks")]
    [NodeDescription("Select Profile Design Check Set.")]
    [IsDesignScriptCompatible]
    public class ProfileDesignCheckSetDropDown : StyleDropDownBase
    {
        private const string OutputName = "profileDesignCheckSet";
        private static string StyleType = typeof(ProfileDesignCheckSet).ToString();
        private static string StyleCollection = DesignCheckSetCollections.ProfileDesignCheckSets.ToString();

        public ProfileDesignCheckSetDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public ProfileDesignCheckSetDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }
}