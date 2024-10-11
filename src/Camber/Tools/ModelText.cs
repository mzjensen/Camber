// Apache license 2.0

// DynamoText License

// Copyright 2013-2021 Autodesk

// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.

// Unless required by applicable law or agreed to in writing, software distributed under the License
// is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

// Modifications copyright 2021 Zachri Jensen
// Summary of modifications:
//   - Changed inputs of main constructor so that text can be created by coordinate system instead of by point.
//   - Added exception handling
//   - Added method to create 3D solids from text and remove "holes" for characters such as "8", "d", "A", etc.

#region references
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Autodesk.DesignScript.Geometry;
using Point = Autodesk.DesignScript.Geometry.Point;
using Vector = Autodesk.DesignScript.Geometry.Vector;
using Dynamo.Graph.Nodes;
using Camber.DynamoExtensions.GeometryExtensions;

#endregion

namespace Camber.Tools
{
    public class ModelText
    {
        #region constructor
        private ModelText() { }
        #endregion

        #region methods
        /// <summary>
        /// Creates 3D text using a Coordinate System. The width, height, and depth of the text will be oriented along the Coordinate System's X, Y, and Z axes, respectively.
        /// </summary>
        /// <param name="coordinateSystem"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="scale"></param>
        /// <param name="thickness"></param>
        /// <param name="textAlignment"></param>
        /// <param name="bold"></param>
        /// <param name="italic"></param>
        /// <returns></returns>
        [NodeCategory("Create")]
        public static IList<Solid> ByCoordinateSystem(
            CoordinateSystem coordinateSystem, 
            string text,
            string font, 
            double scale = 1, 
            double thickness = 1, 
            int textAlignment = 0, 
            bool bold = false, 
            bool italic = false
            )
        {
            // Error checking
            if (scale <= 0)
            {
                throw new ArgumentException("Scale must be greater than zero.");
            }

            if (thickness <= 0)
            {
                throw new ArgumentException("Thickness must be greater than zero.");
            }
            
            var polyCurves = new List<PolyCurve>();

            var fontFamily = new FontFamily(font);
            var fontStyle = FontStyles.Normal;
            var fontWeight = FontWeights.Medium;

            if (bold) fontWeight = FontWeights.Bold;
            if (italic) fontStyle = FontStyles.Italic;

            var formattedText = new FormattedText(
                text,
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface(
                    fontFamily,
                    fontStyle,
                    fontWeight,
                    FontStretches.Normal),
                1,
                System.Windows.Media.Brushes.Black // This brush does not matter since we use the geometry of the text.
                );

            formattedText.TextAlignment = (TextAlignment)(textAlignment);

            // Build the geometry object that represents the text.
            var textGeometry = formattedText.BuildGeometry(new System.Windows.Point(0, 0));
            foreach (var figure in textGeometry.GetFlattenedPathGeometry().Figures)
            {
                var lineSegments = new List<Line>();

                var init = figure.StartPoint;
                var a = figure.StartPoint;
                System.Windows.Point b;
                foreach (var segment in figure.GetFlattenedPathFigure().Segments)
                {
                    var lineSeg = segment as LineSegment;
                    if (lineSeg != null)
                    {
                        b = lineSeg.Point;
                        var crv = LineBetweenPoints(coordinateSystem, scale, a, b);
                        a = b;
                        lineSegments.Add(crv);
                    }

                    var plineSeg = segment as PolyLineSegment;
                    if (plineSeg != null)
                    {
                        foreach (var segPt in plineSeg.Points)
                        {
                            var crv = LineBetweenPoints(coordinateSystem, scale, a, segPt);
                            a = segPt;
                            lineSegments.Add(crv);
                        }
                    }  
                }
                polyCurves.Add(PolyCurve.ByJoinedCurves(lineSegments, 0.001D, false));
            }

            return SolidsFromTextOutlines(polyCurves, coordinateSystem.YAxis, thickness);
        }

        /// <summary>
        /// Creates a Dynamo Line between two System.Windows.Point objects.
        /// </summary>
        /// <param name="cs"></param>
        /// <param name="scale"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static Line LineBetweenPoints(CoordinateSystem cs, double scale, System.Windows.Point a, System.Windows.Point b)
        {
            Point origin = cs.Origin;
            CoordinateSystem fromCoordinateSystem = CoordinateSystem.ByOriginVectors(origin, Vector.XAxis(), Vector.ZAxis().Reverse());
            CoordinateSystem contextCoordinateSystem = cs;
            
            var pt1 = Point.ByCoordinates((a.X * scale) + origin.X, ((-a.Y + 1) * scale) + origin.Y, origin.Z);
            var pt2 = Point.ByCoordinates((b.X * scale) + origin.X, ((-b.Y + 1) * scale) + origin.Y, origin.Z);
            var crv = Line.ByStartPointEndPoint(pt1, pt2);
            crv = (Line)crv.Transform(fromCoordinateSystem, contextCoordinateSystem);
            return crv;
        }

        #region BUG: this isn't working with multiple holes in a single character (e.g. "8")
        /// <summary>
        /// Creates solids from text character outlines and accounts for characters that have "holes" in them.
        /// </summary>
        /// <param name="polyCurves"></param>
        /// <param name="direction"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        private static IList<Solid> SolidsFromTextOutlines(List<PolyCurve> polyCurves, Vector direction, double distance)
        {
            List<Solid> solids = new List<Solid>();
            List<int> solidsToDrop = new List<int>();

            // Extrude each polycurve as solid
            foreach (PolyCurve curve in polyCurves)
            {
                solids.Add(curve.ExtrudeAsSolid(direction, distance));
            }

            // Loop through each solid
            for (int i = 0; i < solids.Count; i++)
            {
                // The current iteration
                Solid currentSolid = solids[i];

                // Loop through all the solids again to check which one(s) can be subtracted from the current iteration
                for (int j = 0; j < solids.Count; j++)
                {
                    // Skip when index of inner loop equals that of outer loop (don't want to check the same solid against itself)
                    if (j == i)
                    {
                        j = j + 1;
                        continue;
                    }

                    // The solid to check
                    Solid checkerSolid = solids[j];

                    // Check for full containment and subtract if so
                    if (BoundingBoxExtensions.IsFullyContained(BoundingBox.ByGeometry(checkerSolid), BoundingBox.ByGeometry(currentSolid)))
                    {
                        try
                        {
                            currentSolid = currentSolid.Difference(checkerSolid);
                            // Save the index of the solid that was subtracted so we can drop it from the master list of solids later
                            solidsToDrop.Add(j);
                            solids[i] = currentSolid;
                        }
                        catch { }
                    }
                    else
                    {
                        // Go to next iteration if checker solid is not fully contained within current iteration
                        continue;
                    }
                }
            }

            // Drop the subtracted solids
            foreach (int index in solidsToDrop.OrderByDescending(x => x))
            {
                solids.RemoveAt(index);
            }

            return solids;
        }
        #endregion
        #endregion
    }
}