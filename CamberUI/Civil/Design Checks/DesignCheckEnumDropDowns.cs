#region references
using System.Collections.Generic;
using civDb = Autodesk.Civil.DatabaseServices;
using civDs = Autodesk.Civil.DataShortcuts;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using Camber.Civil.Styles;
using Camber.Civil.Styles.Objects;
using Autodesk.DesignScript.Runtime;
using Camber.Civil.Styles.Views;
#endregion

namespace Camber.UI
{
    [NodeName("Alignment Design Check Types")]
    [NodeCategory("Camber.Civil 3D.Design Checks")]
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
    [NodeCategory("Camber.Civil 3D.Design Checks")]
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
    [NodeCategory("Camber.Civil 3D.Design Checks.ProfileDesignCheckSet")]
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
}