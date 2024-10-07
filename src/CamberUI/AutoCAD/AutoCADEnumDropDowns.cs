#region references
using System.Collections.Generic;
using Autodesk.DesignScript.Runtime;
using Camber.Properties;
using Newtonsoft.Json;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using aecDb = Autodesk.Aec.DatabaseServices;
using Dynamo.Graph.Nodes;
#endregion

namespace Camber.UI
{
    [NodeName("Block Units")]
    [NodeCategory("Camber.AutoCAD.Objects.Block")]
    [NodeDescription("Select Block units.")]
    [IsDesignScriptCompatible]
    [IsVisibleInDynamoLibrary(false)]
    public class BlockUnitsDropDown : EnumDropDownBase
    {
        private const string OutputName = "units";

        public BlockUnitsDropDown() : base(OutputName, typeof(acDb.UnitsValue))
        {
            this.Warning(string.Format(Resources.NODE_DEPRECATED_MESSAGE, "Units"), true);
        }

        [JsonConstructor]
        public BlockUnitsDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(acDb.UnitsValue), inPorts, outPorts)
        {
            this.Warning(string.Format(Resources.NODE_DEPRECATED_MESSAGE, "Units"), true);
        }
    }

    [NodeName("View Block View Directions")]
    [NodeCategory("Camber.AutoCAD.Objects.Multi-View Blocks.ViewBlock")]
    [NodeDescription("Select View Block view direction type.")]
    [IsDesignScriptCompatible]
    public class ViewDirectionsDropDown : EnumDropDownBase
    {
        private const string OutputName = "viewDirection";

        public ViewDirectionsDropDown() : base(OutputName, typeof(aecDb.ViewDirection)) { }

        [JsonConstructor]
        public ViewDirectionsDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(aecDb.ViewDirection), inPorts, outPorts) { }
    }
}