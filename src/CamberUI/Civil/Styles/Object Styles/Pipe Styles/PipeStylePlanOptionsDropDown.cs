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
    [NodeName("Pipe Style Plan Properties")]
    [NodeCategory("Camber.Civil 3D.Styles.Object Styles.PipeStyle")]
    [NodeDescription("Select Pipe Style plan property name.")]
    [IsDesignScriptCompatible]
    public class PipeStylePlanOptionsDropDown : DSDropDownBase
    {
        public PipeStylePlanOptionsDropDown() : base("propertyName") { }

        [JsonConstructor]
        public PipeStylePlanOptionsDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base("propertyName", inPorts, outPorts) { }

        protected override SelectionState PopulateItemsCore(string currentSelection)
        {
            Items.Clear();

            var newItems = new List<DynamoDropDownItem>()
            {
                new DynamoDropDownItem("Align Hatch to Pipe", "AlignHatchToPipe"),
                new DynamoDropDownItem("Centerline Options", "CenterlineOptions"),
                new DynamoDropDownItem("Centerline Size", "CenterlineSize"),
                new DynamoDropDownItem("Centerline Size Percent", "CenterlineSizePercent"),
                new DynamoDropDownItem("End Line Size", "EndLineSize"),
                new DynamoDropDownItem("End Line Size Percent", "EndLineSizePercent"),
                new DynamoDropDownItem("End Size Options", "EndSizeOptions"),
                new DynamoDropDownItem("End Size Type", "EndSizeType"),
                new DynamoDropDownItem("Hatch Options", "HatchOptions"),
                new DynamoDropDownItem("Inner Diameter", "InnerDiameter"),
                new DynamoDropDownItem("Inner Diameter Percent", "InnerDiameterPercent"),
                new DynamoDropDownItem("Outer Diameter", "OuterDiameter"),
                new DynamoDropDownItem("Outer Diameter Percent", "OuterDiameterPercent"),
                new DynamoDropDownItem("Pipe-to-Pipe End Cleanup", "PipeToPipeEndCleanup"),
                new DynamoDropDownItem("Wall Size Options", "WallSizeOptions"),
                new DynamoDropDownItem("Wall Size Type", "WallSizeType")
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