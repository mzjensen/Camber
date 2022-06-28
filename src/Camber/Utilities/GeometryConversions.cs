#region references
using System;
using System.Collections;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using acGeom = Autodesk.AutoCAD.Geometry;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
#endregion

namespace Camber.Utilities.GeometryConversions
{
    [IsVisibleInDynamoLibrary(false)]
    public static class GeometryConversions
    {
        #region points
        /// <summary>
        /// Converts an AutoCAD Point2d to Dynamo Point.
        /// </summary>
        /// <param name="acPoint2d"></param>
        /// <returns></returns>
        public static Point AcPointToDynPoint(acGeom.Point2d acPoint2d)
        {
            return Point.ByCoordinates(acPoint2d.X, acPoint2d.Y);
        }

        /// <summary>
        /// Converts an AutoCAD Point3d to Dynamo Point.
        /// </summary>
        /// <param name="acPoint3d"></param>
        /// <returns></returns>
        public static Point AcPointToDynPoint(acGeom.Point3d acPoint3d)
        {
            return Point.ByCoordinates(acPoint3d.X, acPoint3d.Y, acPoint3d.Z);
        }


        /// <summary>
        /// Converts a Dynamo Point into an AutoCAD Point2d or Point3d
        /// </summary>
        /// <param name="dynPoint"></param>
        /// <param name="returnPoint3d">If false, return Point2d</param>
        /// <returns></returns>
        public static object DynPointToAcPoint(Point dynPoint, bool returnPoint3d = true)
        {
            if (returnPoint3d)
            {
                return new acGeom.Point3d(dynPoint.X, dynPoint.Y, dynPoint.Z);
            }
            else
            {
                return new acGeom.Point2d(dynPoint.X, dynPoint.Y);
            }
        }


        /// <summary>
        /// Converts a list of Dynamo Points to an AutoCAD Point3dCollection or Point2dCollection.
        /// </summary>
        /// <param name="dynPoints"></param>
        /// <returns></returns>
        public static object DynPointsToAcPointCollection(List<Point> dynPoints, bool returnPoint3dCollection = true)
        {
            bool anyPointIs3d = dynPoints.Exists(x => x.Z != 0);
            if (anyPointIs3d)
            {
                var collection = new acGeom.Point3dCollection();
                foreach (Point point in dynPoints)
                {
                    collection.Add(new acGeom.Point3d(point.X, point.Y, point.Z));
                }
                return collection;
            }
            else
            {
                if (returnPoint3dCollection)
                {
                    var collection = new acGeom.Point3dCollection();
                    foreach (Point point in dynPoints)
                    {
                        collection.Add(new acGeom.Point3d(point.X, point.Y, point.Z));
                    }
                    return collection;
                }
                else
                {
                    var collection = new acGeom.Point2dCollection();
                    foreach (Point point in dynPoints)
                    {
                        collection.Add(new acGeom.Point2d(point.X, point.Y));
                    }
                    return collection;
                }
            }
        }

        /// <summary>
        /// Converts an AutoCAD Point2dCollection to a list of Dynamo Points.
        /// </summary>
        /// <param name="acPoint2dCollection"></param>
        /// <returns></returns>
        public static List<Point> AcPointCollectionToDynPoints(acGeom.Point2dCollection acPoint2dCollection)
        {
            List<Point> dynPoints = new List<Point>();
            foreach (acGeom.Point2d acPoint in acPoint2dCollection)
            {
                dynPoints.Add(Point.ByCoordinates(acPoint.X, acPoint.Y));
            }
            return dynPoints;
        }

        /// <summary>
        /// Converts an AutoCAD Point3dCollection to a list of Dynamo Points.
        /// </summary>
        /// <param name="acPoint3dCollection"></param>
        /// <returns></returns>
        public static List<Point> AcPointCollectionToDynPoints(acGeom.Point3dCollection acPoint3dCollection)
        {
            List<Point> dynPoints = new List<Point>();
            foreach (acGeom.Point3d acPoint in acPoint3dCollection)
            {
                dynPoints.Add(Point.ByCoordinates(acPoint.X, acPoint.Y, acPoint.Z));
            }
            return dynPoints;
        }
        #endregion

        #region vectors
        /// <summary>
        /// Converts an AutoCAD Vector2d or Vector3d to Dynamo Vector.
        /// </summary>
        /// <param name="acVector">Vector2d or Vector3d</param>
        /// <returns></returns>
        public static Vector AcVectorToDynamoVector(object acVector)
        {
            if (!(acVector is acGeom.Vector2d) & !(acVector is acGeom.Vector3d))
            {
                throw new ArgumentException("Input object is not a Vector2d or Vector3d.");
            }

            if (acVector is acGeom.Vector2d)
            {
                var vec = (acGeom.Vector2d)acVector;
                return Vector.ByCoordinates(vec.X, vec.Y, 0);
            }
            else if (acVector is acGeom.Vector3d)
            {
                var vec = (acGeom.Vector3d)acVector;
                return Vector.ByCoordinates(vec.X, vec.Y, vec.Z);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Converts a Dynamo Vector to AutoCAD Vector2d or Vector3d.
        /// </summary>
        /// <param name="dynVector"></param>
        /// <returns></returns>
        public static object DynamoVectorToAcVector(Vector dynVector, bool returnVector3d = true)
        {
            if (dynVector.Z == 0)
            {
                if (returnVector3d)
                {
                    return new acGeom.Vector3d(dynVector.X, dynVector.Y, dynVector.Z);
                }
                else
                {
                    return new acGeom.Vector2d(dynVector.X, dynVector.Y);
                }
            }
            else
            {
                return new acGeom.Vector3d(dynVector.X, dynVector.Y, dynVector.Z);
            }
        }
        #endregion

        #region curves
        /// <summary>
        /// Converts an AutoCAD Line to Dynamo Line.
        /// </summary>
        /// <param name="acLine"></param>
        /// <returns></returns>
        public static Line AcLineToDynLine(acDb.Line acLine)
        {
            return Line.ByStartPointEndPoint(AcPointToDynPoint(acLine.StartPoint), AcPointToDynPoint(acLine.EndPoint));
        }

        /// <summary>
        /// Converts a Dynamo Line to AutoCAD Line.
        /// </summary>
        /// <param name="dynLine"></param>
        /// <returns></returns>
        public static acDb.Line DynLineToAcLine(Line dynLine)
        {
            return new acDb.Line((acGeom.Point3d)DynPointToAcPoint(dynLine.StartPoint), (acGeom.Point3d)DynPointToAcPoint(dynLine.EndPoint));
        }

        /// <summary>
        /// Converts a Dynamo Line to an AutoCAD LineSegment.
        /// </summary>
        /// <param name="dynLine"></param>
        /// <param name="returnLineSegment3d"></param>
        /// <returns></returns>
        public static object DynLineToAcLineSegment(Line dynLine, bool returnLineSegment3d = true)
        {
            var points = new List<Point>()
            {
                dynLine.StartPoint,
                dynLine.EndPoint
            };

            bool anyPointIs3d = points.Exists(x => x.Z != 0);
            if (anyPointIs3d)
            {
                var start = new acGeom.Point3d(dynLine.StartPoint.X, dynLine.StartPoint.Y, dynLine.StartPoint.Z);
                var end = new acGeom.Point3d(dynLine.EndPoint.X, dynLine.EndPoint.Y, dynLine.EndPoint.Z);
                return new acGeom.LineSegment3d(start, end);
            }
            else
            {
                if (returnLineSegment3d)
                {
                    var start = new acGeom.Point3d(dynLine.StartPoint.X, dynLine.StartPoint.Y, dynLine.StartPoint.Z);
                    var end = new acGeom.Point3d(dynLine.EndPoint.X, dynLine.EndPoint.Y, dynLine.EndPoint.Z);
                    return new acGeom.LineSegment3d(start, end);
                }
                else
                {
                    var start = new acGeom.Point2d(dynLine.StartPoint.X, dynLine.StartPoint.Y);
                    var end = new acGeom.Point2d(dynLine.EndPoint.X, dynLine.EndPoint.Y);
                    return new acGeom.LineSegment2d(start, end);
                }
            }
        }

        /// <summary>
        /// Converts a Dynamo Arc to AutoCAD Arc.
        /// </summary>
        /// <param name="dynArc"></param>
        /// <returns></returns>
        public static acDb.Arc DynArcToAcArc(Arc dynArc)
        {
            double endAngle = dynArc.SweepAngle - dynArc.StartAngle;
            return new acDb.Arc((acGeom.Point3d)DynPointToAcPoint(dynArc.CenterPoint), dynArc.Radius, dynArc.StartAngle, endAngle);
        }

        /// <summary>
        /// Converts an AutoCAD Arc to Dynamo Arc.
        /// </summary>
        /// <param name="acArc"></param>
        /// <returns></returns>
        public static Arc AcArcToDynArc(acDb.Arc acArc)
        {
            return Arc.ByCenterPointStartPointEndPoint(AcPointToDynPoint(acArc.Center), AcPointToDynPoint(acArc.StartPoint), AcPointToDynPoint(acArc.EndPoint));
        }

        /// <summary>
        /// Converts a Dynamo Circle to AutoCAD Circle.
        /// </summary>
        /// <param name="dynCircle"></param>
        /// <returns></returns>
        public static acDb.Circle DynCircleToAcCircle(Circle dynCircle)
        {
            return new acDb.Circle((acGeom.Point3d)DynPointToAcPoint(dynCircle.CenterPoint), (acGeom.Vector3d)DynamoVectorToAcVector(dynCircle.Normal), dynCircle.Radius);
        }

        /// <summary>
        /// Converts and AutoCAD Point2dCollection to Dynamo Polygon.
        /// </summary>
        /// <param name="acPoint2dCollection"></param>
        /// <returns></returns>
        public static Polygon AcPointCollectionToDynPolygon(acGeom.Point2dCollection acPoint2dCollection)
        {
            return Polygon.ByPoints(AcPointCollectionToDynPoints(acPoint2dCollection));
        }

        /// <summary>
        /// Converts and AutoCAD Point3dCollection to Dynamo Polygon.
        /// </summary>
        /// <param name="acPoint3dCollection"></param>
        /// <returns></returns>
        public static Polygon AcPointCollectionToDynPolygon(acGeom.Point3dCollection acPoint3dCollection)
        {
            return Polygon.ByPoints(AcPointCollectionToDynPoints(acPoint3dCollection));
        }

        /// <summary>
        /// Converts a Dynamo Polygon to AutoCAD Point3dCollection.
        /// </summary>
        /// <param name="dynPolygon"></param>
        /// <param name="sameStartAndEnd"></param>
        /// <returns></returns>
        public static acGeom.Point3dCollection DynPolygonToAcPoint3dCollection(Polygon dynPolygon, bool sameStartAndEnd = false)
        {
            acGeom.Point3dCollection acCollection = (acGeom.Point3dCollection)DynPointsToAcPointCollection(dynPolygon.Points.ToList(), true);

            if (sameStartAndEnd)
            {
                acCollection.Add(new acGeom.Point3d(acCollection[0].X, acCollection[0].Y, acCollection[0].Z));
            }

            return acCollection;
        }

        /// <summary>
        /// Converts an AutoCAD Polyline3d to Dynamo PolyCurve.
        /// </summary>
        /// <param name="polyline3d"></param>
        /// <returns></returns>
        public static PolyCurve AcPolyline3dToDynPolyCurve(acDb.Polyline3d polyline3d)
        {
            bool connectLastToFirst = polyline3d.Closed;

            List<Point> dynPnts = new List<Point>();
            foreach (acDb.PolylineVertex3d vert in polyline3d)
            {
                dynPnts.Add(AcPointToDynPoint(vert.Position));
            }
            return PolyCurve.ByPoints(dynPnts, connectLastToFirst);
        }
        #endregion

        #region solids
        /// <summary>
        /// Converts an AutoCAD Solid3d to Dynamo solid via SAT export/import.
        /// </summary>
        /// <param name="acSolid"></param>
        /// <returns></returns>
        public static Solid AcSolidToDynSolid(acDb.Solid3d acSolid)
        {
            // Create temp path
            string satPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".sat");
            acDb.DBObjectCollection entitiesOutToFile = new acDb.DBObjectCollection();
            
            try
            {
                entitiesOutToFile.Add(acSolid);
                // Export bodies to SAT
                acDb.Body.AcisOut(satPath, entitiesOutToFile);
                
                // Import SAT as Dynamo geometry
                Geometry[] satGeom = Geometry.ImportFromSAT(satPath);

                if (satGeom != null && satGeom.Length == 1)
                {
                    double scaleFactor = GetSATScaleFactor(satPath);
                    if (scaleFactor != 1.0)
                    {
                        Solid dynSolid = (Solid)satGeom[0].Scale(scaleFactor);
                        satGeom[0].Dispose();
                        return dynSolid;
                    }
                    return (Solid)satGeom[0];
                }

                if (satGeom != null)
                {
                    Geometry[] tempArray = satGeom;
                    for (int i = 0; i < tempArray.Length; i++)
                    {
                        tempArray[i]?.Dispose();
                    }
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                try
                {
                    entitiesOutToFile.Dispose();
                    if (File.Exists(satPath))
                    {
                        File.Delete(satPath);
                    }
                }
                catch { }
            }
            return null;
        }

        /// <summary>
        /// Gets the scale factor from the hdader of an SAT file.
        /// The first value in the third line of the file gives the number of millimeters
        /// represented by each unit of the model.
        /// </summary>
        /// <param name="satFilePath"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        public static double GetSATScaleFactor(string satFilePath)
        {
            try
            {
                using (StreamReader streamReader = new StreamReader(satFilePath))
                {
                    streamReader.ReadLine();
                    streamReader.ReadLine();
                    string text = streamReader.ReadLine();
                    if (!string.IsNullOrEmpty(text))
                    {
                        string[] headerArray = text.Split(' ');
                        if (headerArray != null && headerArray.Length != 0)
                        {
                            double num = Convert.ToDouble(headerArray[0]);
                            if (num > 0.0)
                            {
                                return 1000.0 / num;
                            }
                        }
                    }
                    return 1.0;
                }
            }
            catch
            {
                throw new InvalidOperationException("Invalid SAT file");
            }
        }
        #endregion
    }
}
