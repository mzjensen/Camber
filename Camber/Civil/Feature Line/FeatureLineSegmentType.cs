#region references
using Autodesk.DesignScript.Runtime;
#endregion

namespace Camber.Civil
{
    [IsVisibleInDynamoLibrary(false)]
    public enum FeatureLineSegmentType
    {
        Line,
        Arc,
        Helix
    }
}
