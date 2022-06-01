using Autodesk.DesignScript.Runtime;
using civDynNodes = Autodesk.Civil.DynamoNodes;

namespace Camber.Civil.CivilObjects.Surfaces
{
    [SupressImportIntoVM]
    internal interface ICamberSurface
    {
        civDynNodes.Surface AsSurface();
    }
}
