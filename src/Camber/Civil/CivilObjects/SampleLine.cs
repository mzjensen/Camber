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
using Camber.Properties;
using DynamoServices;
using Camber.Utilities.GeometryConversions;
using Dynamo.Graph.Nodes;
#endregion;

namespace Camber.Civil.CivilObjects
{
    [RegisterForTrace]
    public sealed class SampleLine : CivilObject
    {
        #region properties
        internal AeccSampleLine AeccSampleLine => AcObject as AeccSampleLine;
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

            foreach (AeccSampleLineVertex vertex in AeccSampleLine.Vertices)
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
            AeccSampleLineVertex vertex = AeccSampleLine.Vertices[index];
            vertex.Location = acPoint;
            if (!openedForWrite) AeccEntity.DowngradeOpen();
            return this;
        }
        #endregion

        #region deprecated
        /// <summary>
        /// Returns a Sample Line at the specified station. A new Sample Line will be created if it does not already exist.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sampleLineGroup"></param>
        /// <param name="station"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        public static SampleLine ByStation(string name, SampleLineGroup sampleLineGroup, double station)
        {
            LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MESSAGE, "SampleLine.ByStationSwathWidths"));

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
        /// Gets the PolyCurve representation of the Sample Line.
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.SampleLine.Geometry",
            "Autodesk.AutoCAD.DynamoNodes.Object.Geometry")]
        public PolyCurve Geometry
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Object.Geometry"));
                var dict = GetVertices();
                return PolyCurve.ByPoints((List<Point>)dict["Points"]);
            }
        }

        /// <summary>
        /// Gets the boolean value which specifies whether the Sample Line is locked to a station.
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.SampleLine.LockToStation",
            "Autodesk.Civil.DynamoNodes.SampleLine.IsLockedToStation")]
        public bool LockToStation
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "SampleLine.IsLockedToStation"));
                return GetBool();
            }
        }

        /// <summary>
        /// Gets the number assigned to the Sample Line.
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.SampleLine.Number",
            "Autodesk.Civil.DynamoNodes.SampleLine.Number")]
        public int Number
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "SampleLine.Number"));
                return GetInt();
            }
        }

        /// <summary>
        /// Gets the Sample Line Group to which the Sample Line belongs.
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.SampleLine.SampleLineGroup",
            "Autodesk.Civil.DynamoNodes.SampleLine.SampleLineGroup")]
        public SampleLineGroup SampleLineGroup
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "SampleLine.SampleLineGroup"));
                return SampleLineGroup.GetByObjectId(AeccSampleLine.GroupId);
            }
        }

        /// <summary>
        /// Gets the Section Views associated with a Sample Line.
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.SampleLine.SectionViews",
            "Autodesk.Civil.DynamoNodes.SampleLine.SectionViews")]
        public IList<SectionView> SectionViews
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "SampleLine.SectionViews"));
                List<SectionView> sectViews = new List<SectionView>();
                foreach (acDb.ObjectId oid in AeccSampleLine.GetSectionViewIds())
                {
                    sectViews.Add(SectionView.GetByObjectId(oid));
                }

                return sectViews;
            }
        }

        /// <summary>
        /// Sets the boolean value which specifies whether the Sample Line is locked to a station.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.SampleLine.SetLockToStation",
            "Autodesk.Civil.DynamoNodes.SampleLine.SetLockedToStation")]
        public SampleLine SetLockToStation(bool @bool)
        {
            LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "SampleLine.SetLockedToStation"));
            SetValue(@bool);
            return this;
        }

        /// <summary>
        /// Sets the station of the Sample Line.
        /// </summary>
        /// <param name="station"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.SampleLine.SetStation",
            "Autodesk.Civil.DynamoNodes.SampleLine.SetStation")]
        public SampleLine SetStation(double station)
        {
            LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "SampleLine.SetStation"));
            SetValue(station);
            return this;
        }

        /// <summary>
        /// Gets the station of the Sample Line.
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.SampleLine.Station",
            "Autodesk.Civil.DynamoNodes.SampleLine.Station")]
        public double Station
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "SampleLine.Station"));
                return GetDouble();
            }
        }

        #endregion
    }
}
