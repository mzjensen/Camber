using Autodesk.DesignScript.Runtime;

namespace Camber.Civil.Styles.Objects
{
    [IsVisibleInDynamoLibrary(false)]
    public enum AlignmentMarkerStyles
    {
        BeginPoint,
        CompoundCurveIntersectPoint,
        CurveLineIntersectPoint,
        CurveSpiralIntersectPoint,
        EndPoint,
        IntersectionPoint,
        LineCurveIntersectPoint,
        LineSpiralIntersectPoint,
        MidPoint,
        ReverseCurveIntersectPoint,
        ReverseSpiralIntersectPoint,
        SpiralCurveIntersectPoint,
        SpiralLineIntersectPoint,
        SpiralSpiralIntersectPoint,
        StationReferencePoint,
        ThroughPoint
    }
}
