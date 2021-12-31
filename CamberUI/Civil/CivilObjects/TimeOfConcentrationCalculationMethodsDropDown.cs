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
    [NodeName("Time of Concentration Calculation Methods")]
    [NodeCategory("Camber.Civil 3D.CivilObjects.Catchment")]
    [NodeDescription("Select time of concentration calculation method.")]
    [IsDesignScriptCompatible]
    public class TimeOfConcentrationCalculationMethodsDropDown : DSDropDownBase
    {
        public TimeOfConcentrationCalculationMethodsDropDown() : base("calculationMethod") { }

        [JsonConstructor]
        public TimeOfConcentrationCalculationMethodsDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base("calculationMethod", inPorts, outPorts) { }

        protected override SelectionState PopulateItemsCore(string currentSelection)
        {
            Items.Clear();

            var newItems = new List<DynamoDropDownItem>()
            {
                new DynamoDropDownItem("User-defined", "CalculationMethodUserDefined"),
                new DynamoDropDownItem("TR-55", "CalculationMethodTR55")
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