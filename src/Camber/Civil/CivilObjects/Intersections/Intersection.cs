#region references
using System;
using System.Collections.Generic;
using System.Linq;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civDb = Autodesk.Civil.DatabaseServices;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using AeccIntersection = Autodesk.Civil.DatabaseServices.Intersection;
using Autodesk.DesignScript.Runtime;
using Autodesk.DesignScript.Geometry;
using Camber.Utilities.GeometryConversions;
using Camber.Civil.Labels;
using Autodesk.AutoCAD.Runtime;
using Dynamo.Graph.Nodes;
using Camber.Civil.CivilObjects;
#endregion

namespace Camber.Civil.Intersections
{
    public sealed class Intersection : CivilObject
    {

        #region properties
        internal AeccIntersection AeccIntersection => AcObject as AeccIntersection;

        /// <summary>
        /// Gets the Corridor that an Intersection belongs to.
        /// </summary>
        public civDynNodes.Corridor Corridor
        {
            get
            {
                var document = acDynNodes.Document.Current;

                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    civDb.Corridor aeccCorr = (civDb.Corridor)ctx.Transaction.GetObject(
                        AeccIntersection.CorridorId,
                        acDb.OpenMode.ForRead);
                    if (aeccCorr != null)
                    {
                        return civDynNodes.Selection.CorridorByName(aeccCorr.Name, document);
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the corridor grade option for the Corridors created in the area of an Intersection.
        /// </summary>
        public string CorridorType => GetString("GradeRuleType");

        /// <summary>
        /// Gets the location of an Intersection.
        /// </summary>
        public Point Location => GeometryConversions.AcPointToDynPoint(AeccIntersection.Location);

        /// <summary>
        /// Gets the driving direction of the roadways in an Intersection.
        /// </summary>
        private string RoadwayDrivingDirection => GetString();

        /// <summary>
        /// Gets the Intersection Location Labels associated with an Intersection.
        /// </summary>
        public IList<IntersectionLocationLabel> Labels
        {
            get
            {
                var labels = new List<IntersectionLocationLabel>();
                foreach (acDb.ObjectId aeccLabelId in AeccIntersection.GetIntersectionLocaitonLabelIds())
                {
                    civDb.IntersectionLocationLabel aeccLabel =
                        (civDb.IntersectionLocationLabel)aeccLabelId.GetObject(acDb.OpenMode.ForWrite);
                    labels.Add(new IntersectionLocationLabel(aeccLabel, false));
                }
                return labels;
            }
        }
        #endregion

        #region constructors
        [SupressImportIntoVM]
        internal static Intersection GetByObjectId(acDb.ObjectId intxId)
            => CivilObjectSupport.Get<Intersection, AeccIntersection>
            (intxId, (intx) => new Intersection(intx));

        internal Intersection(AeccIntersection aeccIntx, bool isDynamoOwned = false)
            : base(aeccIntx, isDynamoOwned) { }

        /// <summary>
        /// Gets an Intersection in a Document by name.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        [NodeCategory("Actions")]
        public static Intersection GetIntersectionByName(acDynNodes.Document document, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new InvalidOperationException("Intersection name is null or empty.");
            }

            return GetIntersections(document)
                .FirstOrDefault(item => item.Name.Equals
                (name, StringComparison.OrdinalIgnoreCase));
        }
        #endregion

        #region methods
        public override string ToString() => $"Intersection(Name = {Name})";

        /// <summary>
        /// Gets all of the Intersections in a Document.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static IList<Intersection> GetIntersections(acDynNodes.Document document)
        {
            List<Intersection> intxs = new List<Intersection>();

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                acDb.BlockTable bt = (acDb.BlockTable)ctx.Transaction.GetObject(
                    document.AcDocument.Database.BlockTableId,
                    acDb.OpenMode.ForRead);
                acDb.BlockTableRecord btr = (acDb.BlockTableRecord)ctx.Transaction.GetObject(
                    bt[acDb.BlockTableRecord.ModelSpace],
                    acDb.OpenMode.ForRead);

                foreach (acDb.ObjectId oid in btr)
                {
                    if (oid.ObjectClass == RXObject.GetClass(typeof(AeccIntersection)))
                    {
                        intxs.Add(GetByObjectId(oid));
                    }
                }
            }
            return intxs;
        }

        /// <summary>
        /// Sets the driving direction of the roadways in an Intersection.
        /// </summary>
        /// <param name="directionToggle">True = right side, False = left side</param>
        /// <returns></returns>
        private Intersection SetDrivingDirection(bool directionToggle)
        {
            var direction = Autodesk.Civil.DrivingDirectionType.RightSideOfTheRoad;

            if (!directionToggle)
            {
                direction = Autodesk.Civil.DrivingDirectionType.LeftSideOfTheRoad;
            }

            SetValue(direction, "RoadwayDrivingDirection");
            return this;
        }
        #endregion
    }
}
