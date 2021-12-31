#region references
using System.Linq;
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using civApp = Autodesk.Civil.ApplicationServices;
using civStyles = Autodesk.Civil.DatabaseServices.Styles;
using CoreNodeModels;
using ProtoCore.AST.AssociativeAST;
using Dynamo.Utilities;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using Camber.Civil.Styles;
#endregion

namespace Camber.UI
{
    /// <summary>
    /// Generic UI dropdown node base class for Civil 3D Styles.
    /// This class populates a dropdown with all styles of the specified type.
    /// </summary>
    public abstract class StyleDropDownBase : DSDropDownBase
    {
        #region properties
        private string StyleCollection { get; set; }
        private string StyleType { get; set; }
        #endregion

        #region constructors
        /// <summary>
        /// Generic Civil 3D Style selection dropdown.
        /// </summary>
        /// <param name="outputName"></param>
        /// <param name="styleCollection"></param>
        /// <param name="styleType"></param>
        public StyleDropDownBase(string outputName, string styleCollection, string styleType) : base(outputName)
        { 
            StyleCollection = styleCollection;
            StyleType = styleType;
            PopulateDropDownItems();
        }

        /// <summary>
        /// JSON constructor for serializing/deserializing ports
        /// </summary>
        /// <param name="outputName"></param>
        /// <param name="styleCollection"></param>
        /// <param name="styleType"></param>
        /// <param name="inPorts"></param>
        /// <param name="outPorts"></param>
        [JsonConstructor]
        public StyleDropDownBase(string outputName, string styleCollection, string styleType, IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(outputName, inPorts, outPorts)
        {
            StyleCollection = styleCollection;
            StyleType = styleType;
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
        /// Assign the selected style type to the output.
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
            //var stringNode = AstFactory.BuildStringNode((string)Items[SelectedIndex].Name);

            // Assign the selected name to an actual enumeration value
            //var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), stringNode);

            // Return the enumeration value
            //return new List<AssociativeNode> { assign };
            #endregion

            #region return instance
            AssociativeNode result = AstFactory.BuildFunctionCall<string, string, string, object>(Style.GetByNameType, new List<AssociativeNode>
            {
                AstFactory.BuildStringNode(Items[SelectedIndex].Name),
                AstFactory.BuildStringNode(StyleCollection),
                AstFactory.BuildStringNode(StyleType)
            }, null);

            AssociativeNode assign = (AssociativeNode)(object)AstFactory.BuildAssignment((AssociativeNode)(object)GetAstIdentifierForOutputIndex(0), result);

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
                    civApp.CivilDocument cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);

                    var styles = (civStyles.StyleCollectionBase)cdoc.Styles
                        .GetType()
                        .GetProperty(StyleCollection)
                        .GetValue(cdoc.Styles, null);

                    foreach (acDb.ObjectId styleId in styles)
                    {
                        if (!styleId.IsValid || styleId.IsErased || styleId.IsEffectivelyErased) { continue; }

                        var style = (civStyles.StyleBase)styleId.GetObject(acDb.OpenMode.ForRead, false, true);
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
