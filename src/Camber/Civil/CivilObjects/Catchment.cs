﻿#region references
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Camber.Civil.Labels;
using Camber.Civil.PipeNetworks;
using Camber.Civil.PipeNetworks.Parts;
using Camber.Civil.Styles.Objects;
using Camber.Utilities.GeometryConversions;
using DynamoServices;
using System;
using System.Collections.Generic;
using System.Linq;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acGeom = Autodesk.AutoCAD.Geometry;
using AeccCatchment = Autodesk.Civil.DatabaseServices.Catchment;
using AeccCatchmentAreaLabel = Autodesk.Civil.DatabaseServices.CatchmentLabel;
using AeccCatchmentFlowSegmentLabel = Autodesk.Civil.DatabaseServices.FlowSegmentLabel;
using civDb = Autodesk.Civil.DatabaseServices;
using civDynNodes = Autodesk.Civil.DynamoNodes;
#endregion

namespace Camber.Civil.CivilObjects
{
    [RegisterForTrace]
    public sealed class Catchment : CivilObject
    {
        #region properties
        internal AeccCatchment AeccCatchment => AcObject as AeccCatchment;

        /// <summary>
        /// Gets the Catchment's parent Catchment Group.
        /// </summary>
        public CatchmentGroup CatchmentGroup => CatchmentGroup.GetByObjectId(AeccCatchment.ContainingGroupId);

        /// <summary>
        /// Gets the 2D area of the Catchment.
        /// </summary>
        public double Area => GetDouble("Area2d");

        /// <summary>
        /// Gets the discharge point of the Catchment.
        /// </summary>
        public Point DischargePoint
        {
            get
            {
                try
                {
                    return GeometryConversions.AcPointToDynPoint(AeccCatchment.DischargePoint);
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets a boolean value that identifies if a Catchment is exclusionary or not.
        /// </summary>
        public bool Exclusionary => GetBool();

        /// <summary>
        /// Gets the hydrologically most distant length of a Catchment.
        /// </summary>
        public double HydrologicallyMostDistantLength => GetDouble();

        /// <summary>
        /// Gets the hydrologically most distant point in a Catchment.
        /// </summary>
        public Point HydrologicallyMostDistantPoint => GeometryConversions.AcPointToDynPoint(AeccCatchment.HydrologicallyMostDistantPoint);

        /// <summary>
        /// Gets the 2D perimeter of a Catchment.
        /// </summary>
        public double Perimeter => GetDouble("Perimeter2d");

        /// <summary>
        /// Gets the runoff coefficient for a Catchment.
        /// </summary>
        public double RunoffCoefficient => GetDouble();

        /// <summary>
        /// Gets the time of concentration for a Catchment.
        /// </summary>
        public double TimeOfConcentration => GetDouble();

        /// <summary>
        /// Gets the time of concentration calculation method for a Catchment.
        /// </summary>
        public string TimeOfConcentrationCalculationMethod => AeccCatchment.TimeOfConcentrationCalculationMethod.ToString();

        /// <summary>
        /// Gets a Catchment's 2D boundary Polygon.
        /// </summary>
        public Polygon Boundary2D => GeometryConversions.AcPointCollectionToDynPolygon(AeccCatchment.BoundaryPolyline2d);

        /// <summary>
        /// Gets a Catchment's 3D boundary Polygon.
        /// </summary>
        public Polygon Boundary3D => GeometryConversions.AcPointCollectionToDynPolygon(AeccCatchment.BoundaryPolyline3d);

        /// <summary>
        /// Gets the reference Surface for a Catchment.
        /// </summary>
        public civDynNodes.Surface ReferenceSurface
        {
            get
            {
                try
                {
                    return civDynNodes.Selection.SurfaceByName(AeccCatchment.ReferenceSurfaceName, acDynNodes.Document.Current);
                }
                catch { return null; }
            }
        }

        /// <summary>
        /// Gets the reference Pipe Network for a Catchment.
        /// </summary>
        public PipeNetwork ReferencePipeNetwork
        {
            get
            {
                try
                {
                    return PipeNetwork.GetByObjectId(AeccCatchment.ReferencePipeNetworkId);
                }
                catch { return null; }
            }
        }

        /// <summary>
        /// Gets the reference Structure for a Catchment.
        /// </summary>
        public Structure ReferenceStructure
        {
            get
            {
                try
                {
                    return Structure.GetByObjectId(AeccCatchment.ReferencePipeNetworkStructureId);
                }
                catch { return null; }
            }
        }

        /// <summary>
        /// Gets the available Flow Segment Labels for a Catchment.
        /// </summary>
        public List<CatchmentFlowSegmentLabel> FlowSegmentLabels
        {
            get
            {
                var labels = new List<CatchmentFlowSegmentLabel>();
                foreach (acDb.ObjectId aeccLabelId in AeccCatchment.GetAvailableFlowSegmentLabelIds())
                {
                    AeccCatchmentFlowSegmentLabel aeccLabel = (AeccCatchmentFlowSegmentLabel)aeccLabelId.GetObject(acDb.OpenMode.ForWrite);
                    labels.Add(new CatchmentFlowSegmentLabel(aeccLabel, false));
                }
                return labels;
            }
        }

        /// <summary>
        /// Gets the available Area Labels for a Catchment.
        /// </summary>
        public List<CatchmentAreaLabel> AreaLabels
        {
            get
            {
                var labels = new List<CatchmentAreaLabel>();
                foreach (acDb.ObjectId aeccLabelId in AeccCatchment.GetAvailableCatchmentLabelIds())
                {
                    AeccCatchmentAreaLabel aeccLabel = (AeccCatchmentAreaLabel)aeccLabelId.GetObject(acDb.OpenMode.ForWrite);
                    labels.Add(new CatchmentAreaLabel(aeccLabel, false));
                }
                return labels;
            }
        }
        #endregion

        #region constructors
        internal Catchment(AeccCatchment aeccCatchment, bool isDynamoOwned = false) : base(aeccCatchment, isDynamoOwned) { }

        [SupressImportIntoVM]
        internal static Catchment GetByObjectId(acDb.ObjectId catchmentId)
            => CivilObjectSupport.Get<Catchment, AeccCatchment>
            (catchmentId, (catchment) => new Catchment(catchment));

        /// <summary>
        /// Creates a Catchment by name and boundary Polygon.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="catchmentGroup"></param>
        /// <param name="boundary"></param>
        /// <param name="surface"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public static Catchment ByPolygon(string name, CatchmentGroup catchmentGroup, Polygon boundary, civDynNodes.Surface surface, CatchmentStyle style)
        {
            // This is interesting because the API requires a surface as an input,
            // although in the UI you can create a Catchment from an object with no surface associated to it.

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name is null or empty.");
            }

            if (boundary.SelfIntersections().Length > 0)
            {
                throw new ArgumentException("Boundary Polygon cannot have self-intersections.");
            }

            var document = acDynNodes.Document.Current;
            acGeom.Point3dCollection acBoundary = GeometryConversions.DynPolygonToAcPoint3dCollection(boundary, true);

            bool hasCatchmentWithSameName = false;
            var res = CommonConstruct<Catchment, AeccCatchment>(
                document,
                (ctx) =>
                {
                    if (catchmentGroup.Catchments.Any(obj => obj.Name == name))
                    {
                        hasCatchmentWithSameName = true;
                        return null;
                    }
                    var oid = AeccCatchment.Create(
                        name,
                        style.InternalObjectId,
                        catchmentGroup.InternalObjectId,
                        surface.InternalObjectId,
                        acBoundary);
                    return ctx.Transaction.GetObject(oid, acDb.OpenMode.ForWrite) as AeccCatchment;
                },
                (ctx, catchment, existing) =>
                {
                    if (existing)
                    {
                        if (catchment.Name != name && !catchmentGroup.Catchments.Any(obj => obj.Name == name))
                        {
                            catchment.Name = name;
                        }
                        else if (catchment.Name != name && catchmentGroup.Catchments.Any(obj => obj.Name == name))
                        {
                            hasCatchmentWithSameName = true;
                            return false;
                        }
                        catchment.StyleId = style.InternalObjectId;
                        catchment.ReferenceSurfaceId = surface.InternalObjectId;
                        catchment.BoundaryPolyline3d = acBoundary;
                    }
                    return true;
                });
            if (hasCatchmentWithSameName)
            {
                throw new Exception("The Catchment Group already contains a Catchment with the same name.");
            }
            return res;
        }
        #endregion

        #region methods
        public override string ToString() => $"Catchment(Name = {Name})";

        /// <summary>
        /// Sets the runoff coefficient for a Catchment.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Catchment SetRunoffCoefficient(double value)
        {
            if (value < 0.01 || value > 1.0)
            {
                throw new ArgumentException("Value must be greater than or equal to 0.01 and less than or equal to 1.");
            }

            SetValue(value);
            return this;
        }

        /// <summary>
        /// Sets the time of concentration for a Catchment.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Catchment SetTimeOfConcentration(double value)
        {
            SetValue(value);
            return this;
        }

        /// <summary>
        /// Sets the time of concentration calculation method for a Catchment.
        /// </summary>
        /// <param name="calculationMethod"></param>
        /// <returns></returns>
        public Catchment SetTimeOfConcentrationCalculationMethod(string calculationMethod)
        {
            if (!Enum.IsDefined(typeof(civDb.TimeOfConcentrationCalculationMethod), calculationMethod))
            {
                throw new ArgumentException("Invalid calculation method.");
            }

            SetValue((civDb.TimeOfConcentrationCalculationMethod)Enum.Parse(typeof(civDb.TimeOfConcentrationCalculationMethod), calculationMethod));
            return this;
        }

        /// <summary>
        /// Sets a Catchment's boundary Polygon.
        /// </summary>
        /// <param name="boundary"></param>
        /// <returns></returns>
        public Catchment SetBoundaryPolygon(Polygon boundary)
        {
            SetValue("BoundaryPolyline3d", GeometryConversions.DynPolygonToAcPoint3dCollection(boundary));
            return this;
        }

        /// <summary>
        /// Sets the reference Surface for a Catchment.
        /// </summary>
        /// <param name="surface"></param>
        /// <returns></returns>
        public Catchment SetReferenceSurface(civDynNodes.Surface surface)
        {
            SetValue("ReferenceSurfaceId", surface.InternalObjectId);
            return this;
        }

        /// <summary>
        /// Sets the reference Structure for a Catchment.
        /// </summary>
        /// <param name="structure"></param>
        /// <returns></returns>
        public Catchment SetReferenceStructure(Structure structure)
        {
            SetValue("ReferencePipeNetworkStructureId", structure.InternalObjectId);
            return this;
        }

        /// <summary>
        /// Sets a Catchment's Flow Path.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public Catchment SetFlowPath(List<Point> points)
        {
            acGeom.Point3dCollection acPoints = new acGeom.Point3dCollection();
            foreach (Point dynPoint in points)
            {
                acPoints.Add(new acGeom.Point3d(dynPoint.X, dynPoint.Y, dynPoint.Z));
            }

            bool openedForWrite = AeccCatchment.IsWriteEnabled;
            if (!openedForWrite) AeccEntity.UpgradeOpen();
            AeccCatchment.SetFlowPath(acPoints);
            if (!openedForWrite) AeccEntity.DowngradeOpen();

            return this;
        }
        #endregion
    }
}
