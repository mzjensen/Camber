#region references
using System;
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acGeom = Autodesk.AutoCAD.Geometry;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using civApp = Autodesk.Civil.ApplicationServices;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using AeccSampleLine = Autodesk.Civil.DatabaseServices.SampleLine;
using AeccSampleLineVertex = Autodesk.Civil.DatabaseServices.SampleLineVertex;
using Autodesk.DesignScript.Runtime;
using Autodesk.DesignScript.Geometry;
using DynamoServices;
using Camber.Utils;
using Dynamo.Graph.Nodes;
#endregion;

namespace Camber.Civil.CivilObjects
{
    [RegisterForTrace]
    public sealed class SampleLine : CivilObject
    {
        #region properties
        internal AeccSampleLine aeccSampleLine => AcObject as AeccSampleLine;

        /// <summary>
        /// Gets the Sample Line Group to which the Sample Line belongs.
        /// </summary>
        public SampleLineGroup SampleLineGroup => SampleLineGroup.GetByObjectId(aeccSampleLine.GroupId);

        /// <summary>
        /// Gets the boolean value which specifies whether the Sample Line is locked to a station.
        /// </summary>
        public bool LockToStation => GetBool();

        /// <summary>
        /// Gets the number assigned to the Sample Line.
        /// </summary>
        public int Number => GetInt();

        /// <summary>
        /// Gets the station of the Sample Line.
        /// </summary>
        public double Station => GetDouble();

        /// <summary>
        /// Gets the PolyCurve representation of the Sample Line.
        /// </summary>
        public PolyCurve Geometry
        {
            get
            {
                var dict = GetVertices();
                return PolyCurve.ByPoints((List<Point>)dict["Points"]);
            }
        }
        #endregion

        #region constructors
        internal SampleLine(AeccSampleLine aeccSampleLine, bool isDynamoOwned = false) : base(aeccSampleLine, isDynamoOwned) { }

        [SupressImportIntoVM]
        internal static SampleLine GetByObjectId(acDb.ObjectId sampleLineId)
            => CivilObjectSupport.Get<SampleLine, AeccSampleLine>
            (sampleLineId, (sampleLine) => new SampleLine(sampleLine));

        /// <summary>
        /// Creates a Sample Line with the specified vertex points.
        /// Besides the specified vertex points, the created Sample Line will have one more vertex
        /// at the intersection point with the Alignment.
        /// Points with non-zero Z coordinates will be projected to the X-Y plane.
        /// Currently does not implement element binding.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sampleLineGroup"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        public static SampleLine ByPoints(string name, SampleLineGroup sampleLineGroup, IList<Point> points)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            SampleLine sampleLine;

            acGeom.Point2dCollection acPoints = new acGeom.Point2dCollection();
            foreach (Point point in points)
            {
                acPoints.Add(new acGeom.Point2d(point.X, point.Y));
            }

            var existingSampleLines = sampleLineGroup.SampleLines;
            var existingNames = new List<string>();
            foreach (SampleLine existingSampleLine in existingSampleLines)
            {
                existingNames.Add(existingSampleLine.Name);
            }

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                if (existingNames.Contains(name))
                {
                    throw new ArgumentException("A Sample Line with that name already exists.");
                }
                else
                {
                    var id = AeccSampleLine.Create(name, sampleLineGroup.InternalObjectId, acPoints);
                    sampleLine = GetByObjectId(id);
                }
            }
            return sampleLine;
        }

        /// <summary>
        /// Returns a Sample Line at the specified station. A new Sample Line will be created if it does not already exist.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sampleLineGroup"></param>
        /// <param name="station"></param>
        /// <returns></returns>
        public static SampleLine ByStation(string name, SampleLineGroup sampleLineGroup, double station)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Sample Line name is null");
            }

            acDynNodes.Document document = acDynNodes.Document.Current;

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                civApp.CivilDocument cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                acDb.ObjectId sampleLineId = acDynApp.ElementBinder.GetObjectIdFromTrace(ctx.Database);

                if (sampleLineId.IsValid && !sampleLineId.IsErased)
                {
                    AeccSampleLine aeccSampleLine = (AeccSampleLine)sampleLineId.GetObject(acDb.OpenMode.ForWrite);
                    if (aeccSampleLine != null)
                    {
                        if (aeccSampleLine.GroupId == sampleLineGroup.InternalObjectId)
                        {
                            // Update properties
                            aeccSampleLine.Name = name;
                            aeccSampleLine.Station = station;
                        }
                        else
                        {
                            // If the group ID has changed, erase the old Sample Line and create a new one
                            aeccSampleLine.Erase();
                            sampleLineId = AeccSampleLine.Create(name, sampleLineGroup.InternalObjectId, station);
                        }
                    }
                }
                else
                {
                    // Create new Sample Line
                    sampleLineId = AeccSampleLine.Create(name, sampleLineGroup.InternalObjectId, station);
                }

                var createdSampleLine = sampleLineId.GetObject(acDb.OpenMode.ForRead) as AeccSampleLine;
                if (createdSampleLine != null)
                {
                    return new SampleLine(createdSampleLine, true);
                }
                return null;
            }
        }

        /// <summary>
        /// Converts a Civil Object to a Sample Line.
        /// </summary>
        /// <param name="civilObject"></param>
        /// <returns></returns>
        [NodeCategory("Actions")]
        public static SampleLine GetFromCivilObject(civDynNodes.CivilObject civilObject)
        {
            var document = acDynNodes.Document.Current;
            acDb.ObjectId oid = civilObject.InternalObjectId;
            using (acDynApp.DocumentContext ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                var aeccObject = ctx.Transaction.GetObject(oid, acDb.OpenMode.ForRead);
                if (aeccObject is AeccSampleLine)
                {
                    return GetByObjectId(oid);
                }
                else
                {
                    throw new ArgumentException("Object is not a Catchment.");
                }
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"SampleLine(Name = {Name}, Number = {Number}, Station = {Station:F2})";

        /// <summary>
        /// Gets a dictionary of the location, side, and offset index properties for a Sample Line Vertex.
        /// </summary>
        /// <param name="aeccSampleLineVertex"></param>
        /// <returns></returns>
        private static IDictionary<string, object> GetVertexInfo(AeccSampleLineVertex aeccSampleLineVertex)
        {
            acGeom.Point3d acPoint = aeccSampleLineVertex.Location;
            Point dynPoint = GeometryConversions.AcPointToDynPoint(acPoint);
            int offsetIndex = aeccSampleLineVertex.OffsetIndex;
            string side = aeccSampleLineVertex.Side.ToString();

            var dict = new Dictionary<string, object>()
            {
                {"Location",  dynPoint},
                {"Offset Index", offsetIndex },
                {"Side", side }
            };

            return dict;
        }

        /// <summary>
        /// Gets a Dictionary of data for the vertex points that define the Sample Line.
        /// The Offset Index values start at 0 for the vertex where the Sample Line intersects the Alignment
        /// and increase moving away (left or right) from the Alignment.
        /// </summary>
        /// <returns></returns>
        [MultiReturn(new[] { "Points", "Offset Indicies", "Sides" })]
        public Dictionary<string, object> GetVertices()
        {
            var points = new List<Point>();
            var offsetIndicies = new List<int>();
            var sides = new List<string>();

            foreach (AeccSampleLineVertex vertex in aeccSampleLine.Vertices)
            {
                var dict = GetVertexInfo(vertex);
                points.Add((Point)dict["Location"]);
                offsetIndicies.Add((int)dict["Offset Index"]);
                sides.Add((string)dict["Side"]);
            }
            return new Dictionary<string, object>
            {
                {"Points", points },
                {"Offset Indicies", offsetIndicies },
                {"Sides", sides }
            };
        }

        /// <summary>
        /// Sets the boolean value which specifies whether the Sample Line is locked to a station.
        /// </summary>
        /// <param name="station"></param>
        /// <returns></returns>
        public SampleLine SetLockToStation(bool @bool)
        {
            SetValue(@bool);
            return this;
        }

        /// <summary>
        /// Sets the station of the Sample Line.
        /// </summary>
        /// <param name="station"></param>
        /// <returns></returns>
        public SampleLine SetStation(double station)
        {
            SetValue(station);
            return this;
        }

        /// <summary>
        /// Sets the location of a vertex point at the given index.
        /// The index is not the same as the Offset Index, but rather an
        /// index of 0 corresponds to the vertex furthest to the left of the Alignment.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public SampleLine SetVertexLocation(int index, Point location)
        {
            acGeom.Point3d acPoint = new acGeom.Point3d(location.X, location.Y, location.Z);
            bool openedForWrite = AeccEntity.IsWriteEnabled;
            if (!openedForWrite) AeccEntity.UpgradeOpen();
            AeccSampleLineVertex vertex = aeccSampleLine.Vertices[index];
            vertex.Location = acPoint;
            if (!openedForWrite) AeccEntity.DowngradeOpen();
            return this;
        }
        #endregion
    }
}
