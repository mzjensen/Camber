#region references
using System.Collections.Generic;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;

#endregion

namespace Camber.UI
{
    [NodeName("Text Alignments")]
    [NodeDescription("Select text alignment.")]
    [NodeCategory("Camber.Tools.ModelText")]
    [IsDesignScriptCompatible]
    public class TextAlignmentDropDown : DSDropDownBase
    {
        #region constructors
        public TextAlignmentDropDown() : base("textAlignment") { }

        [JsonConstructor]
        public TextAlignmentDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base("textAlignment", inPorts, outPorts) { }
        #endregion

        #region methods
        protected override SelectionState PopulateItemsCore(string currentSelection)
        {
            Items.Clear();
            var newItems = new List<DynamoDropDownItem>()
            {
                new DynamoDropDownItem("Left", 0),
                new DynamoDropDownItem("Right", 1),
                new DynamoDropDownItem("Center", 2)
                //new DynamoDropDownItem("Justify", 3)
            };

            Items.AddRange(newItems);

            SelectedIndex = -1;
            return SelectionState.Restore;
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            if (Items.Count == 0 || SelectedIndex == -1)
            {
                return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
            }

            var intNode = AstFactory.BuildIntNode((int)Items[SelectedIndex].Item);
            var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), intNode);

            return new List<AssociativeNode> { assign };
        }
        #endregion
    }
}