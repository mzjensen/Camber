﻿using System;
using System.Linq;
using System.Collections.Generic;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using Dynamo.Graph.Nodes;

namespace Camber.UI
{
    [NodeName("Convert to Camber Object")]
    [NodeDescription("Converts a Dynamo Object to a Camber Object.")]
    [NodeCategory("Camber")]
    [InPortNames("object")]
    [InPortTypes("Object")]
    [InPortDescriptions("Object")]
    [OutPortNames("Object")]
    [OutPortTypes("Object")]
    [OutPortDescriptions("Object")]

    [IsDesignScriptCompatible]
    public class ConvertToCamberObject : NodeModel
    {
        #region constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public ConvertToCamberObject()
        {
            RegisterAllPorts();
        }

        /// <summary>
        /// JSON constructor
        /// </summary>
        /// <param name="inPorts"></param>
        /// <param name="outPorts"></param>
        [JsonConstructor]
        private ConvertToCamberObject(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts) { }
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
                    new Func<acDynNodes.Object, acDynNodes.Object>(AutoCAD.Objects.Object.ConvertToCamberObject),
                    new List<AssociativeNode> { inputAsNodes[0] });

            return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), functionNode) };
        }
        #endregion
    }
}