#region references
using CoreNodeModels;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using System.Text;
#endregion

namespace Camber.UI
{
    /// <summary>
    /// Generic UI dropdown node base class for enumerations.
    /// This class populates a dropdown with all enumeration values of the specified type.
    /// </summary>
    public abstract class EnumDropDownBase : DSDropDownBase
    {
        #region properties
        /// <summary>
        /// The type of enumeration to display.
        /// </summary>
        public Type EnumerationType { get; set; }

        /// <summary>
        /// Specifies whether to add spaces between capital letters.
        /// </summary>
        public bool AddSpaces { get; set; }

        /// <summary>
        /// Specifies whether to drop the last character of each enum name
        /// </summary>
        public bool DropLastCharacter { get; set; }
        #endregion

        #region constructors
        /// <summary>
        /// Generic enumeration selection dropdown
        /// </summary>
        /// <param name="name"></param>
        /// <param name="enumerationType"></param>
        /// <param name="addSpaces"></param>
        /// <param name="dropLastCharacter"></param>
        protected EnumDropDownBase(string name, Type enumerationType, bool addSpaces = false, bool dropLastCharacter = false) : base(name)
        { 
            EnumerationType = enumerationType;
            AddSpaces = addSpaces;
            DropLastCharacter = dropLastCharacter;
            PopulateDropDownItems(); 
        }

        /// <summary>
        /// JSON constructor for serializing/deserializing ports
        /// </summary>
        /// <param name="outputName"></param>
        /// <param name="enumerationType"></param>
        /// <param name="inPorts"></param>
        /// <param name="outPorts"></param>
        /// <param name="addSpaces"></param>
        /// <param name="dropLastCharacter"></param>
        [JsonConstructor]
        public EnumDropDownBase(
            string outputName, 
            Type enumerationType, 
            IEnumerable<PortModel> inPorts, 
            IEnumerable<PortModel> outPorts, 
            bool addSpaces = false, 
            bool dropLastCharacter = false) 
            : base(outputName)
        {
            EnumerationType = enumerationType;
            AddSpaces = addSpaces;
            DropLastCharacter = dropLastCharacter;
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
        /// Populate the items in dropdown menu
        /// </summary>
        public void PopulateDropDownItems()
        {
            if (EnumerationType != null)
            {
                // Clear the dropdown list
                Items.Clear();

                // Get all enumeration names and add them to the dropdown menu
                foreach (string enumName in Enum.GetNames(EnumerationType))
                {
                    string displayName = enumName;
                    if (AddSpaces) { displayName = AddSpacesBetweenCapitals(enumName, true, DropLastCharacter); }
                    Items.Add(new DynamoDropDownItem(displayName, enumName));
                }

                Items = Items.OrderBy(x => x.Name).ToObservableCollection();

                // Setting to -1 makes it so the node initially does not have anything selected.
                // Setting to 0 would select the first item in the list when the node is loaded.
                SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Assign the selected enumeration value to the output
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
                if (EnumerationType != null && Enum.GetNames(EnumerationType).Length > 0)
                {
                    PopulateItems();
                }
            }

            var stringNode = AstFactory.BuildStringNode((string)Items[SelectedIndex].Item);
            var assign = AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), stringNode);
            return new List<AssociativeNode> { assign };
        }

        /// <summary>
        /// Adds spaces between capital letters in a string.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="preserveAcronyms"></param>
        /// <param name="dropLastCharacter"></param>
        /// <returns></returns>
        public string AddSpacesBetweenCapitals(string text, bool preserveAcronyms, bool dropLastCharacter = false)
        {
            if (string.IsNullOrWhiteSpace(text)) { return string.Empty; }
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                         i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                        newText.Append(' ');
                newText.Append(text[i]);
            }
            if (dropLastCharacter) { newText = newText.Remove(newText.Length - 1, 1); }
            return newText.ToString();
        }
    }
    #endregion
}
