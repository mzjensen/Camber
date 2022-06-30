using acDb = Autodesk.AutoCAD.DatabaseServices;
using acGeom = Autodesk.AutoCAD.Geometry;

namespace Camber.AutoCAD.Objects
{
    internal sealed class PolylineVertex
    {
        #region properties
        /// <summary>
        /// Gets the index of this <see cref="PolylineVertex"/>.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Gets the <see cref="acGeom.Point3d"/> of this <see cref="PolylineVertex"/>.
        /// </summary>
        public acGeom.Point3d AcPoint { get; }

        /// <summary>
        /// Gets the <see cref="acDb.PolylineVertex3d"/> of this <see cref="PolylineVertex"/>, if applicable.
        /// </summary>
        public acDb.PolylineVertex3d Vertex3d { get; }
        #endregion

        #region constructors
        /// <summary>
        /// Creates a new <see cref="PolylineVertex"/> by index and <see cref="acGeom.Point3d"/>.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="acPoint">The AutoCAD Point3d.</param>
        internal PolylineVertex(int index, acGeom.Point3d acPoint)
        {
            Index = index;
            AcPoint = acPoint;
            Vertex3d = null;
        }

        /// <summary>
        /// Creates a new <see cref="PolylineVertex"/> by <see cref="acDb.PolylineVertex3d"/>.
        /// </summary>
        /// <param name="vertex3d">The PolylineVertex3d</param>
        internal PolylineVertex(acDb.PolylineVertex3d vertex3d)
        {
            AcPoint = vertex3d.Position;
            Vertex3d = vertex3d;
        }
        #endregion
    }
}
