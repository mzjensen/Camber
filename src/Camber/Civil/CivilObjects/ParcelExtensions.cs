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
#endregion

namespace Camber.Civil.CivilObjects
{
    public static class Parcel
    {
        #region properties
        /// <summary>
        /// Returns the site that a arcel belongs to
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
        [NodeCategory("Query")]
        public static IList<Autodesk.DesignScript.Geometry.PolyCurve> PolyCurves(this civDynNodes.Parcel parcel)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            IList<Autodesk.DesignScript.Geometry.PolyCurve> retVal = new List<Autodesk.DesignScript.Geometry.PolyCurve>();
            foreach (Polyline poly in ExtractPolylines(parcel))
            {
                retVal.Add(ConvertPolylineToPolyCurve(poly));
            }
            //// get the COM object
            //dynamic oParcel = parcel.InternalDBObject.AcadObject;
            //// get the parcel loops from the COM object
            //dynamic loops = oParcel.ParcelLoops;
            //// loop the COM loops
            //foreach (dynamic loop in loops)
            //{
            //    // create a polyline for later conversion to polycurve
            //    acDb.Polyline poly = new acDb.Polyline();
            //    int count = loop.Count;
            //    for (int i = 0; i < count; i++)
            //    {
            //        dynamic segElements = loop.Item(i);
            //        double x = segElements.StartX;
            //        double y = segElements.StartY;
            //        double bulge = 0;
            //        try
            //        {
            //            bulge = (double)segElements.Bulge;
            //        }
            //        catch { }
            //        poly.AddVertexAt(i, new Autodesk.AutoCAD.Geometry.Point2d(x, y), bulge, 0, 0);
            //    }
            //    poly.Closed = true;
            //    retVal.Add(ConvertPolylineToPolyCurve(poly));
            //    poly.Erase();
            //}

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
                if (polyline.Closed) { curves = new Autodesk.DesignScript.Geometry.Curve[polyline.NumberOfVertices]; }
                else { curves = new Autodesk.DesignScript.Geometry.Curve[polyline.NumberOfVertices - 1]; }
                // convert segment into Dynamo curve or line
                int curIndex = 0;
                while (curIndex <= polyline.NumberOfVertices)
                {
                    if (polyline.GetSegmentType(curIndex) == SegmentType.Arc)
                    {
                        Autodesk.DesignScript.Geometry.Arc curCurve;
                        Autodesk.DesignScript.Geometry.Point centerPt = Autodesk.DesignScript.Geometry.Point.ByCoordinates(
                            polyline.GetArcSegment2dAt(curIndex).Center.X,
                            polyline.GetArcSegment2dAt(curIndex).Center.Y,
                            0.0);
                        Autodesk.DesignScript.Geometry.Point startPt = Autodesk.DesignScript.Geometry.Point.ByCoordinates(
                            polyline.GetArcSegment2dAt(curIndex).StartPoint.X,
                            polyline.GetArcSegment2dAt(curIndex).StartPoint.Y,
                            0.0);
                        Autodesk.DesignScript.Geometry.Point endPt = Autodesk.DesignScript.Geometry.Point.ByCoordinates(
                            polyline.GetArcSegment2dAt(curIndex).EndPoint.X,
                            polyline.GetArcSegment2dAt(curIndex).EndPoint.Y,
                            0.0);
                        curCurve = Autodesk.DesignScript.Geometry.Arc.ByCenterPointStartPointEndPoint(centerPt, endPt, startPt);
                        curves.SetValue(curCurve, curIndex);
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
                retVal = Autodesk.DesignScript.Geometry.PolyCurve.ByJoinedCurves(curves, 0, false, 0);
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
                    dynamic segElements = loop.Item(i);
                    double x = segElements.StartX;
                    double y = segElements.StartY;
                    double bulge = 0;
                    try
                    {
                        bulge = (double)segElements.Bulge;
                    }
                    catch { }
                    poly.AddVertexAt(i, new Autodesk.AutoCAD.Geometry.Point2d(x, y), bulge, 0, 0);
                }
                poly.Closed = true;
                retVal.Add(poly);
            }

            return retVal;
        }

        #endregion methods


    }
}
