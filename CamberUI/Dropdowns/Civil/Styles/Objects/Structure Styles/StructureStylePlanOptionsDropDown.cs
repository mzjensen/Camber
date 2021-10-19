#region references
using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;
using System.Linq;
#endregion

namespace Camber.UI
{
    [NodeName("Structure Style Plan Properties")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.StructureStyle")]
    [NodeDescription("Select Structure Style plan property name.")]
    [IsDesignScriptCompatible]
    public class StructureStylePlanOptionsDropDown : DSDropDownBase
    {
        public StructureStylePlanOptionsDropDown() : base("propertyName") { }

        [JsonConstructor]
        public StructureStylePlanOptionsDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base("propertyName", inPorts, outPorts) { }

        protected override SelectionState PopulateItemsCore(string currentSelection)
        {
            Items.Clear();

            var newItems = new List<DynamoDropDownItem>()
            {
                new DynamoDropDownItem("Mask Connected Objects", "MaskConnectedObjects"),
                new DynamoDropDownItem("Plan View Options", "PlanViewOptions"),
                new DynamoDropDownItem("Size", "Size"),
                new DynamoDropDownItem("Size Percent", "SizePercent"),
                new DynamoDropDownItem("Size Type", "SizeType"),
                new DynamoDropDownItem("Symbol Block Name", "SymbolBlockName"),
                new DynamoDropDownItem("Symbol Block X Scale", "SymbolBlockXScale"),
                new DynamoDropDownItem("Symbol Block Y Scale", "SymbolBlockYScale"),
                new DynamoDropDownItem("Symbol Block Z Scale", "SymbolBlockZScale")
            };

            Items.AddRange(newItems);
            Items = Items.OrderBy(x => x.Name).ToObservableCollection();

            SelectedIndex = -1;
            return SelectionState.Restore;
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            if (Items.Count == 0 || SelectedIndex == -1)
            {
                return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
            }

            var stringNode = AstFactory.BuildStringNode((string)Items[SelectedIndex].Item);
            var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), stringNode);

            return new List<AssociativeNode> { assign };
        }
    }
}