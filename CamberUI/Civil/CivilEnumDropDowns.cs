#region references
using System.Collections.Generic;
using civDs = Autodesk.Civil.DataShortcuts;
using civDb = Autodesk.Civil.DatabaseServices;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using Camber.Civil;
#endregion

namespace Camber.UI
{
    [NodeName("Data Shortcut Entity Types")]
    [NodeCategory("Camber.Civil 3D.Data Shortcuts.DataShortcutProject")]
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

    [NodeName("Pressure Part Domains")]
    [NodeCategory("Camber.Civil 3D.CivilObjects.Pressure Networks.PressurePartsList")]
    [NodeDescription("Select Pressure Part domain.")]
    [IsDesignScriptCompatible]
    public class PressurePartDomainDropDown : EnumDropDownBase
    {
        private const string OutputName = "pressurePartDomain";

        public PressurePartDomainDropDown() : base(OutputName, typeof(civDb.PressurePartDomainType)) { }

        [JsonConstructor]
        public PressurePartDomainDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.PressurePartDomainType), inPorts, outPorts) { }
    }

    [NodeName("Pressure Part Types")]
    [NodeCategory("Camber.Civil 3D.CivilObjects.Pressure Networks.PressurePartsList")]
    [NodeDescription("Select Pressure Part type.")]
    [IsDesignScriptCompatible]
    public class PressurePartTypeDropDown : EnumDropDownBase
    {
        private const string OutputName = "pressurePartType";

        public PressurePartTypeDropDown() : base(OutputName, typeof(civDb.PressurePartType), true) { }

        [JsonConstructor]
        public PressurePartTypeDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.PressurePartType), inPorts, outPorts) { }
    }

    [NodeName("Folder Categories")]
    [NodeCategory("Camber.Civil 3D.Folder")]
    [NodeDescription("Select Folder category.")]
    [IsDesignScriptCompatible]
    public class FolderCategoriesDropDown : EnumDropDownBase
    {
        private const string OutputName = "category";

        public FolderCategoriesDropDown() : base(OutputName, typeof(Folder.FolderCategory), true) { }

        [JsonConstructor]
        public FolderCategoriesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(Folder.FolderCategory), inPorts, outPorts) { }
    }
}