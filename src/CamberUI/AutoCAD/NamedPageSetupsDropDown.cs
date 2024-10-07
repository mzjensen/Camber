using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using Newtonsoft.Json;
using ProtoCore.AST.AssociativeAST;
using System.Collections.Generic;
using System.Linq;
using Autodesk.DesignScript.Runtime;
using Camber.Properties;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;

namespace Camber.UI
{
    [NodeName("Named Page Setups")]
    [NodeCategory("Camber.AutoCAD.Layout")]
    [NodeDescription("Select named page setup.")]
    [IsDesignScriptCompatible]
    [IsVisibleInDynamoLibrary(false)]
    [NodeMigrationMapping(
        "Camber.UI.NamedPageSetupsDropDown",
        "Autodesk.AutoCAD.DynamoNodes.ListNamedPageSetup")]
    public class NamedPageSetupsDropDown : DSDropDownBase
    {
        #region fields
        private const string _outputName = "pageSetupName";
        #endregion

        #region constructors
        /// <summary>
        /// Named page setups dropdown
        /// </summary>
        public NamedPageSetupsDropDown() : base(_outputName)
        {
            this.Warning(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Choose Named Page Setup"), true);
            PopulateDropDownItems();
        }

        /// <summary>
        /// JSON constructor for serializing/deserializing ports
        /// </summary>
        /// <param name="inPorts"></param>
        /// <param name="outPorts"></param>
        [JsonConstructor]
        public NamedPageSetupsDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) 
            : base(_outputName, inPorts, outPorts)
        {
            this.Warning(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Choose Named Page Setup"), true);
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
        /// Assign the selected named page setup to the output.
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
                    acDb.DBDictionary plSets = (acDb.DBDictionary)ctx.Transaction.GetObject(
                        ctx.Database.PlotSettingsDictionaryId,
                        acDb.OpenMode.ForRead);

                    foreach (var entry in plSets)
                    {
                        var plSet = (acDb.PlotSettings)ctx.Transaction.GetObject(entry.Value, acDb.OpenMode.ForRead);
                        if (plSet.ModelType)
                        {
                            continue;
                        }
                        var item = new DynamoDropDownItem(plSet.PlotSettingsName, plSet.Handle.ToString());
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
