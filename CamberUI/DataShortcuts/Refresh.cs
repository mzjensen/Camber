using System;
using System.Linq;
using System.Collections.Generic;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;
using Camber.Civil.DataShortcuts;
using Dynamo.Graph.Nodes;

namespace Camber.UI
{
    [NodeName("Refresh Data Shortcuts")]
    [NodeDescription("Refreshes the Data Shortcuts view in the Toolspace.")]
    [NodeCategory("Camber.Civil 3D.Data Shortcuts")]
    [InPortNames("runToggle")]
    [InPortTypes("bool")]
    [InPortDescriptions("A boolean input that can be changed to force the node to run.")]
    [OutPortNames("bool")]
    [OutPortTypes("bool")]
    [OutPortDescriptions("bool")]

    [IsDesignScriptCompatible]
    public class Refresh : NodeModel
    {
        #region constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public Refresh()
        {
            RegisterAllPorts();
        }

        /// <summary>
        /// JSON constructor
        /// </summary>
        /// <param name="inPorts"></param>
        /// <param name="outPorts"></param>
        [JsonConstructor]
        private Refresh(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts) { }
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
                    new Func<bool, bool>(DataShortcuts.Refresh),
                    new List<AssociativeNode> { inputAsNodes[0] });

            return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), functionNode) };
        }
        #endregion
    }
}