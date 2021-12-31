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
    [NodeName("Structure Style Section Properties")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.StructureStyle")]
    [NodeDescription("Select Structure Style Section property name.")]
    [IsDesignScriptCompatible]
    public class StructureStyleSectionOptionsDropDown : DSDropDownBase
    {
        public StructureStyleSectionOptionsDropDown() : base("propertyName") { }

        [JsonConstructor]
        public StructureStyleSectionOptionsDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base("propertyName", inPorts, outPorts) { }

        protected override SelectionState PopulateItemsCore(string currentSelection)
        {
            Items.Clear();

            var newItems = new List<DynamoDropDownItem>()
            {
                new DynamoDropDownItem("Block Insert Location", "BlockInsertLocation"),
                new DynamoDropDownItem("Mask Connected Objects", "MaskConnectedObjects"),
                new DynamoDropDownItem("Size Type", "SizeType"),
                new DynamoDropDownItem("Symbol Block Name", "SymbolBlockName"),
                new DynamoDropDownItem("Symbol Block X Scale", "SymbolBlockXScale"),
                new DynamoDropDownItem("Symbol Block Y Scale", "SymbolBlockYScale"),
                new DynamoDropDownItem("Symbol Block Z Scale", "SymbolBlockZScale"),
                new DynamoDropDownItem("View Options", "ViewOptions"),
                new DynamoDropDownItem("X Size", "XSize"),
                new DynamoDropDownItem("X Size Percent", "XSizePercent"),
                new DynamoDropDownItem("Y Size", "YSize"),
                new DynamoDropDownItem("Y Size Percent", "YSizePercent")
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