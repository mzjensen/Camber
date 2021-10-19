using Dynamo.Graph.Nodes;
using Camber.UI;

namespace Camber.Civil3D.Styles.StructureRuleSetStyle
{
    [NodeName("Structure Rule Sets")]
    [NodeDescription("Select Structure Rule Set.")]
    [IsDesignScriptCompatible]
    public class StructureRuleSetDropDown : StyleDropDownBase
    {
        public StructureRuleSetDropDown() : base("structureRuleSet", StyleCollections.StructureRuleSetStyles) { }
    }
}
