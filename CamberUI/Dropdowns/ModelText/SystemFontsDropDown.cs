#region references
using System.Collections.Generic;
using System.Drawing.Text;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;
using System.Drawing;
using Autodesk.DesignScript.Runtime;
#endregion

namespace Camber.UI
{
    [NodeName("Fonts")]
    [NodeDescription("Select system font.")]
    [NodeCategory("Camber.Tools.ModelText")]
    [IsDesignScriptCompatible]
    [IsVisibleInDynamoLibrary(false)]
    public class SystemFontsDropDown : DSDropDownBase
    {
        #region constructors
        public SystemFontsDropDown() : base("font") { }

        [JsonConstructor]
        public SystemFontsDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base("font", inPorts, outPorts) { }
        #endregion

        #region methods
        protected override SelectionState PopulateItemsCore(string currentSelection)
        {
            Items.Clear();

            // Create list of all installed fonts and their names
            List<string> fonts = new List<string>();
            using (InstalledFontCollection fontsCollection = new InstalledFontCollection())
            {
                FontFamily[] fontFamilies = fontsCollection.Families;
                foreach (FontFamily font in fontFamilies)
                {
                    fonts.Add(font.Name);
                }
            }

            // Create list of dropdown items
            var newItems = new List<DynamoDropDownItem>();
            foreach (string font in fonts)
            {
                newItems.Add(new DynamoDropDownItem(font, font));
            }

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

            var stringNode = AstFactory.BuildStringNode(Items[SelectedIndex].Name);
            var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), stringNode);

            return new List<AssociativeNode> { assign };
        }
        #endregion
    }
}