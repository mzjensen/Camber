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
    [NodeName("Material Quantity Types")]
    [NodeCategory("Camber.Civil 3D.Quantity Takeoff.QTOCriteria")]
    [NodeDescription("Select material quantity type.")]
    [IsDesignScriptCompatible]
    public class MaterialQuantityTypesDropDown : EnumDropDownBase
    {
        private const string OutputName = "quantityType";

        public MaterialQuantityTypesDropDown() : base(OutputName, typeof(civDb.MaterialQuantityType)) { }

        [JsonConstructor]
        public MaterialQuantityTypesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.MaterialQuantityType), inPorts, outPorts) { }
    }

    [NodeName("Material Conditions")]
    [NodeCategory("Camber.Civil 3D.Quantity Takeoff.QTOCriteriaData")]
    [NodeDescription("Select material condition.")]
    [IsDesignScriptCompatible]
    public class MaterialConditionsDropDown : EnumDropDownBase
    {
        private const string OutputName = "condition";

        public MaterialConditionsDropDown() : base(OutputName, typeof(civDb.MaterialConditionType)) { }

        [JsonConstructor]
        public MaterialConditionsDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.MaterialConditionType), inPorts, outPorts) { }
    }
}