using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using Newtonsoft.Json;
using ProtoCore.AST.AssociativeAST;
using System.Collections.Generic;
using System.Linq;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using civApp = Autodesk.Civil.ApplicationServices;
using civDb = Autodesk.Civil.DatabaseServices;

namespace Camber.UI
{
    [NodeName("Alignment Label Set Styles")]
    [NodeCategory("Camber.Civil 3D.CivilObjects.Alignment")]
    [NodeDescription("Select Alignment Label Set Style name.")]
    [IsDesignScriptCompatible]
    public class AlignmentLabelSetStyleNamesDropDown : DSDropDownBase
    {
        #region fields
        private const string _outputName = "labelSetStyleName";
        #endregion

        #region constructors
        /// <summary>
        /// Alignment label set style names dropdown
        /// </summary>
        public AlignmentLabelSetStyleNamesDropDown() : base(_outputName)
        {
            PopulateDropDownItems();
        }

        /// <summary>
        /// JSON constructor for serializing/deserializing ports
        /// </summary>
        /// <param name="inPorts"></param>
        /// <param name="outPorts"></param>
        [JsonConstructor]
        public AlignmentLabelSetStyleNamesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(_outputName, inPorts, outPorts)
        {
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
        /// Assign the selected label set style name to the output.
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
                var adoc = acDynNodes.Document.Current;
                
                using (acDynApp.DocumentContext ctx = new acDynApp.DocumentContext(adoc.AcDocument))
                {
                    var cdoc = civApp.CivilDocument.GetCivilDocument(adoc.AcDocument.Database);

                    foreach (var oid in cdoc.Styles.LabelSetStyles.AlignmentLabelSetStyles)
                    {
                        var style = (civDb.Styles.AlignmentLabelSetStyle) ctx.Transaction.GetObject(
                            oid,
                            acDb.OpenMode.ForRead);
                        var item = new DynamoDropDownItem(style.Name, style.Handle.ToString());
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
