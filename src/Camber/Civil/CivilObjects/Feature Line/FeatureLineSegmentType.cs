#region references
using Autodesk.DesignScript.Runtime;
#endregion

namespace Camber.Civil.CivilObjects
{
    [IsVisibleInDynamoLibrary(false)]
    public enum FeatureLineSegmentType
    {
        Line,
        Arc,
        Helix
    }
}
