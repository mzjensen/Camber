using Autodesk.DesignScript.Runtime;

namespace Camber.Civil.Styles.Objects
{
    [IsVisibleInDynamoLibrary(false)]
    public enum ProfileMarkerStyles
    {
        BeginPoint,
        EndPoint,
        HighPoint,
        LowPoint,
        ProfileMarkerSectionView,
        ThroughPoint,
        VCompoundCurveIntersectPoint,
        VCurveTangentIntersectPoint,
        VIntersectionPoint,
        VReverseCurveIntersectPoint,
        VTangentCurveIntersectPoint
    }
}
