using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using Newtonsoft.Json;
using ProtoCore.AST.AssociativeAST;
using System.Collections.Generic;
using System.Linq;
using Camber.Properties;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;

namespace Camber.UI
{
    [NodeName("Text Styles")]
    [NodeCategory("Camber.AutoCAD.Objects")]
    [NodeDescription("Select text style.")]
    [IsDesignScriptCompatible]
    public class TextStylesDropDown : DSDropDownBase
    {
        #region fields
        private const string _outputName = "textStyle";
        #endregion

        #region constructors
        /// <summary>
        /// Text styles dropdown
        /// </summary>
        public TextStylesDropDown() : base(_outputName)
        {
            this.Info(string.Format(Resources.NODE_OBSOLETE_MIGRATION_MESSAGE, "Choose Text Style"), true);
            PopulateDropDownItems();
        }

        /// <summary>
        /// JSON constructor for serializing/deserializing ports
        /// </summary>
        /// <param name="inPorts"></param>
        /// <param name="outPorts"></param>
        [JsonConstructor]
        public TextStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(_outputName, inPorts, outPorts)
        {
            this.Info(string.Format(Resources.NODE_OBSOLETE_MIGRATION_MESSAGE, "Choose Text Style"), true);
            PopulateDropDownItems();
        }
        #endregion

        #region methods
        protected override SelectionState PopulateItemsCore(string currentSelection)
        {
            PopulateDropDownItems();
            return SelectionState.Restore;
        }

        /// <summary>
        /// Assign the selected text style to the output.
        /// </summary>
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            if (Items.Count == 0 || SelectedIndex == -1)
            {
                return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
            }

            // If the dropdown is still empty try to populate it again          
            if (Items.Count == 0 || Items.Count == -1)
            {
                PopulateItems();
            }

            #region return string
           // Get the name of the selected item
           var stringNode = AstFactory.BuildStringNode((string)Items[SelectedIndex].Name);

           // Assign the selected name to an actual enumeration value
           var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), stringNode);

            // Return the enumeration value
            return new List<AssociativeNode> { assign };
            #endregion
        }

        /// <summary>
        /// Populate the items in dropdown menu
        /// </summary>
        public void PopulateDropDownItems()
        {
            Items.Clear();

            try
            {
                using (acDynApp.DocumentContext ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    var textStyleTbl = (acDb.TextStyleTable)ctx.Transaction.GetObject(
                        ctx.Database.TextStyleTableId,
                        acDb.OpenMode.ForRead);

                    foreach (var oid in textStyleTbl)
                    {
                        var styleRecord = (acDb.TextStyleTableRecord)ctx.Transaction.GetObject(oid, acDb.OpenMode.ForRead);
                        if (string.IsNullOrWhiteSpace(styleRecord.Name))
                        {
                            continue;
                        }
                        var item = new DynamoDropDownItem(styleRecord.Name, styleRecord.Handle.ToString());
                        Items.Add(item);
                    }
                }
            }
            catch { }

            Items = Items.OrderBy(x => x.Name).ToObservableCollection();

            // Setting to -1 makes it so the node initially does not have anything selected.
            // Setting to 0 would select the first item in the list when the node is loaded.
            SelectedIndex = -1;
            
        }
        #endregion
    }
}
