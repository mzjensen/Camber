﻿#region references
using System.Collections.Generic;
using Autodesk.Civil;
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

    [NodeName("Overhang Correction Types")]
    [NodeCategory("Camber.Civil 3D.CorridorSurface")]
    [NodeDescription("Select Corridor Surface overhang correction type.")]
    [IsDesignScriptCompatible]
    public class OverhangCorrectionTypesDropDown : EnumDropDownBase
    {
        private const string OutputName = "overhangCorrectionType";

        public OverhangCorrectionTypesDropDown() : base(OutputName, typeof(civDb.OverhangCorrectionType), true) { }

        [JsonConstructor]
        public OverhangCorrectionTypesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.OverhangCorrectionType), inPorts, outPorts) { }
    }

    [NodeName("Angular Unit Types")]
    [NodeCategory("Camber.AutoCAD.Document")]
    [NodeDescription("Select angular unit type.")]
    [IsDesignScriptCompatible]
    public class AngularUnitTypesDropDown : EnumDropDownBase
    {
        private const string OutputName = "angularUnitType";

        public AngularUnitTypesDropDown() : base(OutputName, typeof(Autodesk.Civil.AngleUnitType)) { }

        [JsonConstructor]
        public AngularUnitTypesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(Autodesk.Civil.AngleUnitType), inPorts, outPorts) { }
    }

    [NodeName("Surface Boundary Types")]
    [NodeCategory("Camber.Civil 3D.CivilObjects.Surfaces.Surface")]
    [NodeDescription("Select Surface boundary type.")]
    [IsDesignScriptCompatible]
    public class SurfaceBoundaryTypesDropDown : EnumDropDownBase
    {
        private const string OutputName = "boundaryType";

        public SurfaceBoundaryTypesDropDown() : base(OutputName, typeof(SurfaceBoundaryType), true) { }

        [JsonConstructor]
        public SurfaceBoundaryTypesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(SurfaceBoundaryType), inPorts, outPorts, true) { }
    }
}