using System.Collections.Generic;
using Autodesk.DesignScript.Geometry;
using Camber.Utilities.GeometryConversions;
using AeccGridSurfaceCell = Autodesk.Civil.DatabaseServices.GridSurfaceCell;

namespace Camber.Civil.CivilObjects.Surfaces
{
    public sealed class GridSurfaceCell
    {
        #region properties
        internal AeccGridSurfaceCell AeccGridSurfaceCell { get; }

        /// <summary>
        /// Gets the index of the column where a Grid Surface Cell is located in a Grid Surface.
        /// </summary>
        public int ColumnIndex => AeccGridSurfaceCell.GridLocation.ColumnIndex;

        /// <summary>
        /// Gets the index of the row where a Grid Surface Cell is located in a Grid Surface.
        /// </summary>
        public int RowIndex => AeccGridSurfaceCell.GridLocation.RowIndex;

        /// <summary>
        /// Gets the Grid Surface that a Grid Surface Cell belongs to.
        /// </summary>
        public GridSurface Surface => GridSurface.GetByObjectId(AeccGridSurfaceCell.Surface.ObjectId);

        /// <summary>
        /// Gets the vertices at the corners of a Grid Surface Cell. The vertices are ordered clockwise starting from the top left corner.
        /// </summary>
        public GridSurfaceVertex[] Vertices
        {
            get
            {
                List<GridSurfaceVertex> vertices = new List<GridSurfaceVertex>();
                vertices.Add(new GridSurfaceVertex(AeccGridSurfaceCell.TopLeftVertex));
                vertices.Add(new GridSurfaceVertex(AeccGridSurfaceCell.BottomLeftVertex));
                vertices.Add(new GridSurfaceVertex(AeccGridSurfaceCell.BottomRightVertex));
                vertices.Add(new GridSurfaceVertex(AeccGridSurfaceCell.TopRightVertex));
                return vertices.ToArray();
            }
        }

        /// <summary>
        /// Gets the closed PolyCurve geometry of a Grid Surface Cell.
        /// </summary>
        public PolyCurve Geometry
        {
            get
            {
                List<Point> pnts = new List<Point>();
                pnts.Add(Vertices[0].Geometry);
                pnts.Add(Vertices[1].Geometry);
                pnts.Add(Vertices[2].Geometry);
                pnts.Add(Vertices[3].Geometry);
                return PolyCurve.ByPoints(pnts, true);
            }
        }
        #endregion

        #region constructors
        internal GridSurfaceCell(AeccGridSurfaceCell aeccGridSurfaceCell)
        {
            AeccGridSurfaceCell = aeccGridSurfaceCell;
        }
        #endregion

        #region methods
        public override string ToString() => $"{nameof(GridSurfaceCell)}(Row index = {RowIndex}, Column index = {ColumnIndex})";
        #endregion
    }
}
