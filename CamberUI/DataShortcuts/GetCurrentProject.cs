using System;
using System.Linq;
using System.Collections.Generic;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using Camber.Civil.DataShortcuts;
using Dynamo.Graph.Nodes;

namespace Camber.UI
{
    [NodeName("Get Current Data Shortcut Project")]
    [NodeDescription("Gets the current Data Shortcut Project.")]
    [NodeCategory("Camber.Civil 3D.Data Shortcuts")]
    [InPortNames("document")]
    [InPortTypes("Autodesk.AutoCAD.DynamoNodes.Document")]
    [InPortDescriptions("document")]
    [OutPortNames("dataShortcutProject")]
    [OutPortTypes("Camber.Civil.DataShortcuts.DataShortcutProject")]
    [OutPortDescriptions("dataShortcutProject")]

    [IsDesignScriptCompatible]
    public class GetCurrentProject : NodeModel
    {
        #region constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public GetCurrentProject()
        {
            RegisterAllPorts();
        }

        /// <summary>
        /// JSON constructor
        /// </summary>
        /// <param name="inPorts"></param>
        /// <param name="outPorts"></param>
        [JsonConstructor]
        private GetCurrentProject(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts) { }
        #endregion

        #region methods
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAsNodes)
        {
            if (!InPorts[0].Connectors.Any())
            {
                return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()) };
            }

            var functionNode =
                AstFactory.BuildFunctionCall(
                    new Func<acDynNodes.Document, DataShortcutProject>(DataShortcuts.GetCurrentProject),
                    new List<AssociativeNode> { inputAsNodes[0] });

            return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), functionNode) };
        }
        #endregion
    }
}