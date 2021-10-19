using Dynamo.Graph.Nodes;
using Camber.UI;

namespace Camber.Civil3D.Styles.ProfileViewBandSetStyle
{
    [NodeName("Profile View Band Set")]
    [NodeDescription("Select Profile View Band Set.")]
    [IsDesignScriptCompatible]
    public class ProfileViewBandSetDropDown : StyleDropDownBase
    {
        public ProfileViewBandSetDropDown() : base("profileViewBandSet", StyleCollections.ProfileViewBandSetStyles) { }
    }
}
