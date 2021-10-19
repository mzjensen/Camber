#region references
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using civApp = Autodesk.Civil.ApplicationServices;
using civDb = Autodesk.Civil.DatabaseServices;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using Camber.Civil.Styles.Labels;
#endregion

namespace Camber.UI
{
    /// <summary>
    /// Generic UI dropdown node base class for Civil 3D Label Styles.
    /// This class populates a dropdown with all label styles of the specified type.
    /// </summary>
    public abstract class LabelStyleDropDownBase : DSDropDownBase
    {
        #region properties
        private string LabelStyleCollection { get; set; }
        private string LabelStyleType { get; set; }
        #endregion

        #region constructors
        /// <summary>
        /// Generic Civil 3D Label Style selection dropdown.
        /// </summary>
        /// <param name="outputName"></param>
        /// <param name="labelStyleCollection"></param>
        /// <param name="labelStyleType"></param>
        public LabelStyleDropDownBase(string outputName, string labelStyleCollection, string labelStyleType) : base(outputName)
        {
            LabelStyleCollection = labelStyleCollection;
            LabelStyleType = labelStyleType;
            PopulateDropDownItems();
        }

        /// <summary>
        /// JSON constructor for serializing/deserializing ports
        /// </summary>
        /// <param name="outputName"></param>
        /// <param name="labelStyleCollection"></param>
        /// <param name="labelStyleType"></param>
        /// <param name="inPorts"></param>
        /// <param name="outPorts"></param>
        [JsonConstructor]
        public LabelStyleDropDownBase(string outputName, string labelStyleCollection, string labelStyleType, IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(outputName)
        {
            LabelStyleCollection = labelStyleCollection;
            LabelStyleType = labelStyleType;
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

            AssociativeNode result = AstFactory.BuildFunctionCall<string, string, string, object>(LabelStyle.GetByNameType, new List<AssociativeNode>
            {
                AstFactory.BuildStringNode(Items[SelectedIndex].Name),
                AstFactory.BuildStringNode(LabelStyleCollection),
                AstFactory.BuildStringNode(LabelStyleType)
            }, null);

            AssociativeNode assign = (AssociativeNode)(object)AstFactory.BuildAssignment((AssociativeNode)(object)GetAstIdentifierForOutputIndex(0), result); 

            return new List<AssociativeNode> { assign };
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

                    var labelStyles = (civDb.Styles.LabelStyleCollection)GetNestedProperty(cdoc.Styles.LabelStyles, LabelStyleCollection);

                    foreach (acDb.ObjectId styleId in labelStyles)
                    {
                        if (!styleId.IsValid || styleId.IsErased || styleId.IsEffectivelyErased) { continue; }

                        var style = (civDb.Styles.LabelStyle)styleId.GetObject(acDb.OpenMode.ForRead, false, true);
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

        /// <summary>
        /// Gets nested object property values via reflection.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static object GetNestedProperty(object src, string propName)
        {
            if (src == null) throw new ArgumentException("Value cannot be null.", "src");
            if (propName == null) throw new ArgumentException("Value cannot be null.", "propName");

            if (propName.Contains("."))
            {
                var temp = propName.Split(new char[] { '.' }, 2);
                return GetNestedProperty(GetNestedProperty(src, temp[0]), temp[1]);
            }
            else
            {
                var prop = src.GetType().GetProperty(propName);
                return prop != null ? prop.GetValue(src, null) : null;
            }
        }
        #endregion
    }
}
