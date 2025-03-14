#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using aeccSite = Autodesk.Civil.DatabaseServices.Site;
using aeccParcel = Autodesk.Civil.DatabaseServices.Parcel;
using Dynamo.Graph.Nodes;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.DesignScript.Runtime;
using Autodesk.AutoCAD.Runtime;
using System.Security.Cryptography.Pkcs;
using System;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.DynamoApp.Services;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Aec.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using ProtoCore.AST.ImperativeAST;
#endregion

namespace Camber.Civil.CivilObjects
{
    public static class Parcel
    {
        #region properties
        /// <summary>
        /// Returns the site that a Parcel belongs to
        /// </summary>
        /// <param name="parcel"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static civDynNodes.Site Site(this civDynNodes.Parcel parcel)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            try
            {
                using (acDynApp.DocumentContext ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    foreach (civDynNodes.Site site in civDynNodes.Site.GetSites(document))
                    {
                        foreach (civDynNodes.Parcel childParcel in site.Parcels)
                            if (parcel.InternalObjectId.Equals(childParcel.InternalObjectId)) return site;
                    }
                }
            }
            catch { throw; }
            return null;
        }
        /// <summary>
        /// Returns the area of a parcel.
        /// </summary>
        /// <param name="parcel"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static double Area(this civDynNodes.Parcel parcel)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            double retVal = 0;
            try
            {
                using (acDynApp.DocumentContext ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    aeccParcel p = ctx.Transaction.GetObject(parcel.InternalObjectId, acDb.OpenMode.ForRead) as aeccParcel;
                    retVal = p.Area;
                }
            }
            catch { throw; }
            return retVal;
        }
        /// <summary>
        /// Returns the perimeter of the parcel
        /// </summary>
        /// <param name="parcel"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static double Perimeter(this civDynNodes.Parcel parcel)
        {
            double retVal = 0;
            try
            {
                IList<Autodesk.AutoCAD.DatabaseServices.Polyline> polys = ExtractPolylines(parcel);
                Polyline poly = polys[0];
                retVal = poly.Length;
            }
            catch { throw; }
            return retVal;
        }
        /// <summary>
        /// Returns a list of polycurves representing the exterior and interior boundaries of a parcel
        /// </summary>
        /// <param name="parcel"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        internal static IList<Autodesk.DesignScript.Geometry.PolyCurve> PolyCurves(this civDynNodes.Parcel parcel)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            IList<Autodesk.DesignScript.Geometry.PolyCurve> retVal = new List<Autodesk.DesignScript.Geometry.PolyCurve>();
            foreach (Polyline poly in ExtractPolylines(parcel))
            {
                retVal.Add(ConvertPolylineToPolyCurve(poly));
            }
            return retVal;
        }
        /// <summary>
        /// Returns a polycurve representation of the exterior boundary of a parcel
        /// </summary>
        /// <param name="parcel"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static Autodesk.DesignScript.Geometry.PolyCurve ExteriorCurve(this civDynNodes.Parcel parcel)
        {
            return PolyCurves(parcel)[0];
        }
        /// <summary>
        /// Returns a list of polycurves representing the interior boundaries of a parcel
        /// </summary>
        /// <param name="parcel"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static IList<Autodesk.DesignScript.Geometry.PolyCurve> InteriorCurves(this civDynNodes.Parcel parcel)
        {
            IList<Autodesk.DesignScript.Geometry.PolyCurve> retVal = PolyCurves(parcel);
            retVal.RemoveAt(0);
            return retVal;
        }

        /// <summary>
        /// Returns a polygon representation of the exterior boundary of a parcel
        /// </summary>
        /// <param name="parcel"></param>
        /// <param name="spacing">Distance along a curve to add points during decurving</param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static Autodesk.DesignScript.Geometry.Polygon ExteriorPolygon(this civDynNodes.Parcel parcel, double spacing = 1.0)
        {
            IList<Autodesk.DesignScript.Geometry.Curve> curves = new List<Autodesk.DesignScript.Geometry.Curve>();
            Polyline poly = ExtractPolylines(parcel)[0];

            // step thru the vertices and convert the segments to a new curve
            for (int i = 0; i < poly.NumberOfVertices; i++)
            {
                Point3d startPt3d = poly.GetPoint3dAt(i);
                Point3d endPt3d;
                if (i == poly.NumberOfVertices - 1)
                    endPt3d = poly.StartPoint;
                else
                    endPt3d = poly.GetPoint3dAt(i + 1);
                // if the current segment is a curve, decurve it
                if (poly.GetBulgeAt(i) != 0)
                {
                    double maxLength = poly.GetDistAtPoint(endPt3d);
                    double curDistance = poly.GetDistAtPoint(startPt3d);
                    while (curDistance < maxLength)
                    {
                        startPt3d = poly.GetPointAtDist(curDistance);
                        Autodesk.DesignScript.Geometry.Point startPt = Autodesk.DesignScript.Geometry.Point.ByCoordinates(
                            startPt3d.X,
                            startPt3d.Y,
                            startPt3d.Z);

                        curDistance += spacing;
                        if (curDistance > maxLength)
                            curDistance = maxLength;
                        endPt3d = poly.GetPointAtDist(curDistance);
                        Autodesk.DesignScript.Geometry.Point endPt = Autodesk.DesignScript.Geometry.Point.ByCoordinates(
                            endPt3d.X,
                            endPt3d.Y,
                            endPt3d.Z);
                        if (!startPt3d.Equals(endPt3d))
                        {
                            Autodesk.DesignScript.Geometry.Curve curLine = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(startPt, endPt);
                            curves.Add(curLine);
                        }
                    }
                }
                else
                {
                    Autodesk.DesignScript.Geometry.Point startPt = Autodesk.DesignScript.Geometry.Point.ByCoordinates(
                        startPt3d.X,
                        startPt3d.Y,
                        startPt3d.Z);

                    Autodesk.DesignScript.Geometry.Point endPt = Autodesk.DesignScript.Geometry.Point.ByCoordinates(
                        endPt3d.X,
                        endPt3d.Y,
                        endPt3d.Z);
                    if (!startPt3d.Equals(endPt3d))
                    {
                        Autodesk.DesignScript.Geometry.Curve curLine = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(startPt, endPt);
                        curves.Add(curLine);
                    }                    
                }
            }

            Autodesk.DesignScript.Geometry.PolyCurve finalCurve = Autodesk.DesignScript.Geometry.PolyCurve.ByJoinedCurves(curves, 0, false, 0);
            if (finalCurve.IsClosed == false)
            {
                finalCurve.CloseWithLine();
            }
            Autodesk.DesignScript.Geometry.Polygon pg = Autodesk.DesignScript.Geometry.Polygon.ByPoints(finalCurve.Points);
            return pg;
        }


        #endregion properties

        #region constructors
        /// <summary>
        /// Gets a Dynamo-wrapped Parcel by Object ID.
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        internal static civDynNodes.Parcel GetByObjectId(acDb.ObjectId oid)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            try
            {
                using (acDynApp.DocumentContext ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    foreach (civDynNodes.Site site in civDynNodes.Site.GetSites(document))
                    {
                        foreach (civDynNodes.Parcel parcel in site.Parcels)
                        {
                            if (parcel.InternalObjectId == oid)
                            {
                                return parcel;
                            }
                        }
                    }
                }
            }
            catch { throw; }
            return null;
        }
        #endregion constructors

        #region methods
        /// <summary>
        /// Converts an AutoCAD Polyline to a Dynamo PolyCurve
        /// </summary>
        /// <param name="polyline">AutoCAD Polyline</param>
        /// <returns>Dynamo PolyCurve object</returns>
        [IsVisibleInDynamoLibrary(false)]
        internal static Autodesk.DesignScript.Geometry.PolyCurve ConvertPolylineToPolyCurve(Autodesk.AutoCAD.DatabaseServices.Polyline polyline)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            Autodesk.DesignScript.Geometry.PolyCurve retVal = null;
            if (polyline != null)
            {
                // extract each segment
                Autodesk.DesignScript.Geometry.Curve[] curves; // = new Autodesk.DesignScript.Geometry.Curve[polyline.NumberOfVertices];
                if (polyline.Closed) { curves = new Autodesk.DesignScript.Geometry.Curve[polyline.NumberOfVertices - 1]; }
                else { curves = new Autodesk.DesignScript.Geometry.Curve[polyline.NumberOfVertices - 2]; }
                // convert segment into Dynamo curve or line
                int curIndex = 0;
                while (curIndex <= curves.Length)
                {
                    if (polyline.GetSegmentType(curIndex) == SegmentType.Arc)
                    {
                        // get the start and end points   
                        Autodesk.DesignScript.Geometry.Point startPt = Autodesk.DesignScript.Geometry.Point.ByCoordinates(
                            polyline.GetArcSegment2dAt(curIndex).StartPoint.X,
                            polyline.GetArcSegment2dAt(curIndex).StartPoint.Y,
                            0.0);
                        Autodesk.DesignScript.Geometry.Point endPt = Autodesk.DesignScript.Geometry.Point.ByCoordinates(
                            polyline.GetArcSegment2dAt(curIndex).EndPoint.X,
                            polyline.GetArcSegment2dAt(curIndex).EndPoint.Y,
                            0.0);
                        if (curIndex == curves.Length)
                            endPt = Autodesk.DesignScript.Geometry.Point.ByCoordinates(
                            polyline.GetArcSegment2dAt(curIndex).EndPoint.X,
                            polyline.GetArcSegment2dAt(curIndex).EndPoint.Y,
                            0.0);

                        // get the center point
                        Autodesk.DesignScript.Geometry.Point centerPt = Autodesk.DesignScript.Geometry.Point.ByCoordinates(
                            polyline.GetArcSegment2dAt(curIndex).Center.X,
                            polyline.GetArcSegment2dAt(curIndex).Center.Y,
                            0.0);

                        // get the mid point
                        Autodesk.AutoCAD.Geometry.CircularArc2d curArc = polyline.GetArcSegment2dAt(curIndex);

                        double curLen = curArc.GetLength(curArc.GetParameterOf(curArc.StartPoint), curArc.GetParameterOf(curArc.EndPoint));
                        double distToIndex = polyline.GetDistAtPoint(polyline.GetPoint3dAt(curIndex));
                        double distToMidPt = distToIndex + curLen / 2;
                        Point3d midPoint;
                        try
                        {
                            midPoint = polyline.GetPointAtDist(distToMidPt);
                        }
                        catch
                        {
                            distToMidPt = distToIndex - curLen / 2;
                            midPoint = polyline.GetPointAtDist(distToMidPt);
                        }
                        Autodesk.DesignScript.Geometry.Point midPt = Autodesk.DesignScript.Geometry.Point.ByCoordinates(midPoint.X, midPoint.Y, 0.0);

                        // creates the curve using start/end/center points
                        Autodesk.DesignScript.Geometry.Arc curCurveFromCen = Autodesk.DesignScript.Geometry.Arc.ByCenterPointStartPointEndPoint(centerPt, startPt, endPt);

                        // creates the curve using the start/mid/end points
                        Autodesk.DesignScript.Geometry.Arc curCurveFromMid = Autodesk.DesignScript.Geometry.Arc.ByThreePoints(startPt, midPt, endPt);

                        // decide which curve is the best match
                        if (curCurveFromMid.Length == curLen)
                            curves.SetValue(curCurveFromMid, curIndex);
                        else if (curCurveFromCen.Length == curLen)
                            curves.SetValue(curCurveFromCen, curIndex);
                        else
                        {
                            // if the neither curve matches, then lets try to best fit the curve to the points
                            IList<Autodesk.DesignScript.Geometry.Point> points = new List<Autodesk.DesignScript.Geometry.Point>()
                                {
                                    startPt,
                                    midPt,
                                    endPt
                                };

                            Autodesk.DesignScript.Geometry.Arc curCurveAlt = Autodesk.DesignScript.Geometry.Arc.ByBestFitThroughPoints(points);
                            curves.SetValue(curCurveAlt, curIndex);
                        }
                    }
                    else if (polyline.GetSegmentType(curIndex) == SegmentType.Line)
                    {
                        Autodesk.DesignScript.Geometry.Line curLine;
                        Autodesk.DesignScript.Geometry.Point startPt = Autodesk.DesignScript.Geometry.Point.ByCoordinates(
                            polyline.GetLineSegment2dAt(curIndex).StartPoint.X,
                            polyline.GetLineSegment2dAt(curIndex).StartPoint.Y,
                            0.0);
                        Autodesk.DesignScript.Geometry.Point endPt = Autodesk.DesignScript.Geometry.Point.ByCoordinates(
                            polyline.GetLineSegment2dAt(curIndex).EndPoint.X,
                            polyline.GetLineSegment2dAt(curIndex).EndPoint.Y,
                            0.0);
                        curLine = Autodesk.DesignScript.Geometry.Line.ByStartPointEndPoint(startPt, endPt);
                        curves.SetValue(curLine, curIndex);
                    }
                    curIndex = curIndex + 1;
                }
                try
                { retVal = Autodesk.DesignScript.Geometry.PolyCurve.ByJoinedCurves(curves, 0, false, 0); }
                catch { }
            }
            return retVal;
        }
        /// <summary>
        /// Creates a list of closed polylines representing the parcel
        /// </summary>
        /// <param name="parcel"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        internal static IList<Autodesk.AutoCAD.DatabaseServices.Polyline> ExtractPolylines(this civDynNodes.Parcel parcel)
        {
            IList<Autodesk.AutoCAD.DatabaseServices.Polyline> retVal = new List<Autodesk.AutoCAD.DatabaseServices.Polyline>();
            // get the COM object
            dynamic oParcel = parcel.InternalDBObject.AcadObject;
            // get the parcel loops from the COM object
            dynamic loops = oParcel.ParcelLoops;
            // loop the COM loops
            foreach (dynamic loop in loops)
            {
                // create a polyline for later conversion to polycurve
                acDb.Polyline poly = new acDb.Polyline();
                int count = loop.Count;
                for (int i = 0; i < count; i++)
                {
                    dynamic segElement = loop.Item(i);




                    double x = segElement.StartX;
                    double y = segElement.StartY;
                    double bulge = 0;
                    double radius = 0;
                    try
                    {
                        radius = (double)segElement.Radius;
                        bulge = (double)segElement.Bulge;
                    }
                    catch { }
                    poly.AddVertexAt(i, new Autodesk.AutoCAD.Geometry.Point2d(x, y), bulge, 0, 0);
                    if (i == count - 1)
                    {
                        bulge = 0;
                        try
                        {
                            bulge = (double)segElement.Bulge;
                        }
                        catch { }
                        poly.AddVertexAt(i + 1, new Autodesk.AutoCAD.Geometry.Point2d(segElement.EndX, segElement.EndY), bulge, 0, 0);
                    }
                }
                poly.Closed = true;
                retVal.Add(poly);
            }

            return retVal;
        }

        [IsVisibleInDynamoLibrary(true)]
        public static Dictionary<string, Dictionary<string, object>> GetUserDefinedProperties(this civDynNodes.Parcel parcel)
        {

            Dictionary<string, Dictionary<string, object>> dictionary = new Dictionary<string, Dictionary<string, object>>();
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (acDynApp.DocumentContext ctx = new acDynApp.DocumentContext(document.AcDocument))
                foreach (UDPClassification parcelUDPClassification in Autodesk.Civil.ApplicationServices.CivilDocument.GetCivilDocument(ctx.Database).ParcelUDPClassifications)
                {
                    Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
                    foreach (UDP uDP in parcelUDPClassification.UDPs)
                    {
                        if (uDP != null)
                        {
                            Autodesk.Civil.DatabaseServices.Parcel acParcel = ctx.Transaction.GetObject(parcel.InternalObjectId, OpenMode.ForRead) as Autodesk.Civil.DatabaseServices.Parcel;
                            object userDefinedPropertyValueByName = Autodesk.Civil.DynamoNodes.Parcel.GetUserDefinedPropertyValueByName(acParcel, parcelUDPClassification.Name, uDP.Name);
                            dictionary2.Add(uDP.Name, userDefinedPropertyValueByName);
                        }
                    }

                    dictionary.Add(parcelUDPClassification.Name, dictionary2);
                }

            return dictionary;

        }

        /// <summary>
        /// Applies the provided user defined data to the parcel
        /// </summary>
        /// <param name="parcel">The parcel to update</param>
        /// <param name="classificationGroup">The classification group name</param>
        /// <param name="propertyName">The user defined property key name</param>
        /// <param name="propertyValue">The user defined property value</param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(true)]
        public static civDynNodes.Parcel SetUserDefinedProperty(this civDynNodes.Parcel parcel, string classificationGroup, string propertyName, object propertyValue)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (acDynApp.DocumentContext ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                CivilDocument civilDocument = Autodesk.Civil.ApplicationServices.CivilDocument.GetCivilDocument(ctx.Database);
                // verify that the classification group has been applied to the parcel
                if ((propertyName != null) & (propertyValue != null))
                    if (civilDocument.ParcelUDPClassifications.Contains(classificationGroup))
                        foreach (UDPClassification parcelUDPClassification in Autodesk.Civil.ApplicationServices.CivilDocument.GetCivilDocument(ctx.Database).ParcelUDPClassifications)
                        {
                            foreach (UDP uDP in parcelUDPClassification.UDPs)
                            {
                                if (uDP != null)
                                {
                                    Autodesk.Civil.DatabaseServices.Parcel acParcel = ctx.Transaction.GetObject(parcel.InternalObjectId, OpenMode.ForWrite) as Autodesk.Civil.DatabaseServices.Parcel;
                                    // verify that the user defined properties exist in the classification group
                                    if (uDP.Name == propertyName)
                                        // verify data type provided is allowed for the UDP value
                                        if (!(uDP is UDPBoolean udp))
                                        {
                                            if (!(uDP is UDPDouble uDPDouble))
                                            {
                                                if (!(uDP is UDPEnumeration udp2))
                                                {
                                                    if (!(uDP is UDPInteger udp3))
                                                    {
                                                        if (uDP is UDPString udp4)
                                                        {
                                                            acParcel.SetUDPValue((UDPString)uDP, propertyValue.ToString());
                                                        }
                                                    }
                                                    else
                                                        acParcel.SetUDPValue((UDPInteger)uDP, (int)propertyValue);
                                                }
                                                else
                                                    acParcel.SetUDPValue((UDPEnumeration)uDP, propertyValue.ToString());
                                            }
                                            else
                                                acParcel.SetUDPValue((UDPDouble)uDP, (double)propertyValue);
                                        }
                                        else
                                            acParcel.SetUDPValue((UDPBoolean)uDP, (bool)propertyValue);
                                }



                            }
                        }
            }
            return parcel;
        }



        #endregion methods


    }
}
