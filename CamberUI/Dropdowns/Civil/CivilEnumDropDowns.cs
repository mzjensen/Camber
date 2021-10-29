#region references
using System.Collections.Generic;
using civDs = Autodesk.Civil.DataShortcuts;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
#endregion

namespace Camber.UI
{
    [NodeName("Data Shortcut Entity Types")]
    [NodeCategory("Camber.Civil 3D.DataShortcuts")]
    [NodeDescription("Select Data Shortcut entity type.")]
    [IsDesignScriptCompatible]
    public class DataShortcutEntityTypesDropDown : EnumDropDownBase
    {
        private const string OutputName = "entityType";

        public DataShortcutEntityTypesDropDown() : base(OutputName, typeof(civDs.DataShortcutEntityType), true) { }

        [JsonConstructor]
        public DataShortcutEntityTypesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDs.DataShortcutEntityType), inPorts, outPorts) { }
    }
}