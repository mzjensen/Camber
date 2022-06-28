#region references
using System.Collections.Generic;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using civDb = Autodesk.Civil.DatabaseServices;
using Camber.Civil.Styles;
using Camber.Civil.Styles.CodeSets;
#endregion

namespace Camber.UI
{
    [NodeName("Code Set Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Code Set Styles")]
    [NodeDescription("Select Code Set Style.")]
    [IsDesignScriptCompatible]
    public class CodeSetStyleDropDown : StyleDropDownBase
    {
        private const string OutputName = "codeSetStyle";
        private static string StyleType = typeof(CodeSetStyle).ToString();
        private static string StyleCollection = StyleCollections.CodeSetStyles.ToString();

        public CodeSetStyleDropDown() : base(OutputName, StyleCollection, StyleType) { }

        [JsonConstructor]
        public CodeSetStyleDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, StyleCollection, StyleType, inPorts, outPorts) { }
    }

    [NodeName("Subentity Style Types")]
    [NodeCategory("Camber.Civil 3D.Styles.Code Set Styles.CodeSetStyleItem")]
    [NodeDescription("Select subentity style type.")]
    [IsDesignScriptCompatible]
    public class SubentityStyleTypesDropDown : EnumDropDownBase
    {
        private const string OutputName = "subentityStyleType";

        public SubentityStyleTypesDropDown() : base(OutputName, typeof(civDb.Styles.SubassemblySubentityStyleType)) { }

        [JsonConstructor]
        public SubentityStyleTypesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, typeof(civDb.Styles.SubassemblySubentityStyleType), inPorts, outPorts) { }
    }
}