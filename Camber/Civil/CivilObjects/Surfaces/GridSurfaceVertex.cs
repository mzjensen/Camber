using Autodesk.DesignScript.Geometry;
using Camber.Utilities.GeometryConversions;
using AeccGridSurfaceVertex = Autodesk.Civil.DatabaseServices.GridSurfaceVertex;

namespace Camber.Civil.CivilObjects.Surfaces
{
    public sealed class GridSurfaceVertex
    {
        #region properties
        internal AeccGridSurfaceVertex AeccGridSurfaceVertex { get; }

        /// <summary>
        /// Gets the column index of a Grid Surface Vertex.
        /// </summary>
        public int ColumnIndex => AeccGridSurfaceVertex.GridLocation.ColumnIndex;

        /// <summary>
        /// Gets the row index of a Grid Surface Vertex.
        /// </summary>
        public int RowIndex => AeccGridSurfaceVertex.GridLocation.RowIndex;

        /// <summary>
        /// Gets the Grid Surface that a Grid Surface Vertex belongs to.
        /// </summary>
        public GridSurface Surface => GridSurface.GetByObjectId(AeccGridSurfaceVertex.Surface.ObjectId);

        /// <summary>
        /// Gets the Dynamo point geometry of a Grid Surface Vertex.
        /// </summary>
        public Point Geometry => GeometryConversions.AcPointToDynPoint(AeccGridSurfaceVertex.Location);
        #endregion

        #region constructors
        internal GridSurfaceVertex(AeccGridSurfaceVertex aeccGridSurfaceVertex)
        {
            AeccGridSurfaceVertex = aeccGridSurfaceVertex;
        }
        #endregion

        #region methods
        public override string ToString() => $"{nameof(GridSurfaceVertex)}(Row index = {RowIndex}, Column index = {ColumnIndex})";
        #endregion
    }
}
