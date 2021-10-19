using Dynamo.Graph.Nodes;
using Camber.UI;

namespace Camber.Civil3D.Styles.SectionViewBandSetStyle
{
    [NodeName("Section View Band Sets")]
    [NodeDescription("Select Section View Band Set.")]
    [IsDesignScriptCompatible]
    public class SectionViewBandSetDropDown : StyleDropDownBase
    {
        public SectionViewBandSetDropDown() : base("sectionViewBandSet", StyleCollections.SectionViewBandSetStyles) { }
    }
}
