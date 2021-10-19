#region references
using Autodesk.DesignScript.Runtime;
#endregion

namespace Camber.Civil.Styles.Labels
{
    [IsVisibleInDynamoLibrary(false)]
    public enum ComponentSettings
    {
        General,
        Block,
        Border,
        DirectionArrow,
        Line,
        Text,
        Tick
    }
}
