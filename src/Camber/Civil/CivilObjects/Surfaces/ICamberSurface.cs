using civDynNodes = Autodesk.Civil.DynamoNodes;

namespace Camber.Civil.CivilObjects.Surfaces
{
    internal interface ICamberSurface
    {
        civDynNodes.Surface AsSurface();
    }
}
