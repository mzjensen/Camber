#region references
using System.Collections.Generic;
using Newtonsoft.Json;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using Dynamo.Graph.Nodes;
#endregion

namespace Camber.UI
{
    [NodeName("Block Units")]
    [NodeCategory("Camber.AutoCAD.BlockExtensions")]
    [NodeDescription("Select Block units.")]
    [IsDesignScriptCompatible]
    public class BlockUnitsDropDown : EnumDropDownBase
    {
        private const string OutputName = "units";

        public BlockUnitsDropDown() : base(OutputName, typeof(acDb.UnitsValue)) { }

        [JsonConstructor]
        public BlockUnitsDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(acDb.UnitsValue), inPorts, outPorts) { }
    }
}