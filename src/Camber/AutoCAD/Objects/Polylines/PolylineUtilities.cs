using System.Collections.Generic;
using System.Linq;
using acGeom = Autodesk.AutoCAD.Geometry;

namespace Camber.AutoCAD.Objects
{
    internal static class PolylineUtilities
    {
        /// <summary>
        /// Determines which vertices in a list of <see cref="PolylineVertex"/> are duplicates.
        /// </summary>
        /// <param name="vertices">The vertices to check.</param>
        /// <param name="tolerance">Two vertices are considered equal if the distance between them is less than the tolerance.</param>
        /// <param name="excludeFirst">
        /// If true, the first duplicate vertex is not included in each group of duplicates.
        /// If false, the first vertex is included and the last vertex is excluded.
        /// </param>
        /// <returns>A list of duplicate vertices.</returns>
        internal static List<PolylineVertex> FindDuplicateVertices(
        List<PolylineVertex> vertices,
        bool excludeFirst,
        double tolerance)
        {
            var dupeGroups = new List<List<PolylineVertex>>();
            var checkedIndices = new List<int>();

            for (int i = 0; i < vertices.Count; i++)
            {
                var currentVert = vertices[i];
                var dupes = new List<PolylineVertex>();

                if (checkedIndices.Contains(i))
                {
                    // Skip the current point if we've already determined that it's a duplicate
                    continue;
                }

                // Loop through every other point
                for (int j = 0; j < vertices.Count; j++)
                {
                    if (j == i)
                    {
                        // Skip if current point and compare point have the same index, since that would always be a duplicate
                        continue;
                    }

                    var compareVert = vertices[j];
                    if (currentVert.AcPoint.IsEqualTo(compareVert.AcPoint, new acGeom.Tolerance(tolerance, tolerance)))
                    {
                        dupes.Add(compareVert);
                        checkedIndices.Add(j);
                    }
                }

                if (dupes.Count == 0)
                {
                    continue;
                }
                if (!excludeFirst)
                {
                    dupes.Reverse();
                    dupes.RemoveAt(0);
                    dupes.Add(currentVert);
                    dupes.Reverse();
                }
                dupeGroups.Add(dupes);
                checkedIndices.Add(i);
            }
            return dupeGroups.SelectMany(x => x).ToList();
        }
    }
}
