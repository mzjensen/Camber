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
    [NodeName("Structure Style Model Properties")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.StructureStyle")]
    [NodeDescription("Select Structure Style model property name.")]
    [IsDesignScriptCompatible]
    public class StructureStyleModelOptionsDropDown : DSDropDownBase
    {
        public StructureStyleModelOptionsDropDown() : base("propertyName") { }

        [JsonConstructor]
        public StructureStyleModelOptionsDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base("propertyName", inPorts, outPorts) { }

        protected override SelectionState PopulateItemsCore(string currentSelection)
        {
            Items.Clear();

            var newItems = new List<DynamoDropDownItem>()
            {
                new DynamoDropDownItem("Model View Options", "ModelViewOptions"),
                new DynamoDropDownItem("Simple Solid Type", "SimpleSolidType")
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