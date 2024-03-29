﻿#region references
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Camber.Civil.PressureNetworks.Parts;
using Camber.Utilities.GeometryConversions;
using System;
using System.Collections.Generic;
using System.Linq;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acGeom = Autodesk.AutoCAD.Geometry;
using AeccPressureAppurtenance = Autodesk.Civil.DatabaseServices.PressureAppurtenance;
using AeccPressureFitting = Autodesk.Civil.DatabaseServices.PressureFitting;
using AeccPressurePipe = Autodesk.Civil.DatabaseServices.PressurePipe;
using AeccPressurePipeRun = Autodesk.Civil.DatabaseServices.PressurePipeRun;
using civDb = Autodesk.Civil.DatabaseServices;
using civDynNodes = Autodesk.Civil.DynamoNodes;
#endregion

namespace Camber.Civil.PressureNetworks
{
    public sealed class PressurePipeRun
    {
        #region properties
        internal AeccPressurePipeRun AeccPressurePipeRun { get; set; }

        /// <summary>
        /// Gets the Pressure Network that the Pressure Pipe Run belongs to.
        /// </summary>
        public PressureNetwork PressureNetwork { get; set; }

        /// <summary>
        /// Gets the Alignment that is used as the horizontal path for the Pressure Pipe Run.
        /// </summary>
        public civDynNodes.Alignment Alignment => CivilObjects.Alignment.GetByObjectId(AeccPressurePipeRun.AlignmentId);

        /// <summary>
        /// Gets the name of the Pressure Pipe Run.
        /// </summary>
        public string Name => AeccPressurePipeRun.Name;

        /// <summary>
        /// Gets the Profile that is used as the vertical path for the Pressure Pipe Run.
        /// </summary>
        public civDynNodes.Profile Profile
        {
            get
            {
                if (AeccPressurePipeRun.ProfileId.IsNull)
                {
                    return null;
                }
                return CivilObjects.Profile.GetFromObjectId(AeccPressurePipeRun.ProfileId);
            }
        }

        /// <summary>
        /// Gets the Profile that is used as the reference profile for the Pressure Pipe Run.
        /// </summary>
        public civDynNodes.Profile ReferenceProfile
        {
            get
            {
                if (AeccPressurePipeRun.ReferenceProfileId.IsNull)
                {
                    return null;
                }
                return CivilObjects.Profile.GetFromObjectId(AeccPressurePipeRun.ReferenceProfileId);
            }
        }  

        /// <summary>
        /// Gets the vertical distance that the Pressure Pipe Run is offset from its reference Profile.
        /// </summary>
        public double VerticalOffset => AeccPressurePipeRun.VerticalOffset;

        /// <summary>
        /// Gets the value that determines how the parts in the Pressure Pipe Run follow the vertical path (i.e. Profile).
        /// </summary>
        public string VerticalOffsetType => AeccPressurePipeRun.VerticalOffsetType.ToString();

        /// <summary>
        /// Gets the collection of Pressure Parts that belong to the Pressure Pipe Run.
        /// </summary>
        public IList<PressurePart> Parts
        {
            get
            {
                var document = acDynNodes.Document.Current;
                var parts = new List<PressurePart>();
                using (acDynApp.DocumentContext ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    foreach (acDb.ObjectId partId in AeccPressurePipeRun.GetPartIds())
                    {
                        var aeccObject = ctx.Transaction.GetObject(partId, acDb.OpenMode.ForRead);
                        if (aeccObject is AeccPressurePipe)
                        {
                            parts.Add(PressurePipe.GetByObjectId(partId));
                        }
                        else if (aeccObject is AeccPressureAppurtenance)
                        {
                            parts.Add(PressureAppurtenance.GetByObjectId(partId));
                        }
                        else if (aeccObject is AeccPressureFitting)
                        {
                            parts.Add(PressureFitting.GetByObjectId(partId));
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                return parts;
            }
        }
        #endregion

        #region constructors
        internal PressurePipeRun(AeccPressurePipeRun aeccPressurePipeRun, PressureNetwork pressureNetwork)
        {
            AeccPressurePipeRun = aeccPressurePipeRun;
            PressureNetwork = pressureNetwork;
        }

        /// <summary>
        /// Creates a new Pressure Pipe Run by Object. Currently supports Alignments or Polylines as input.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="object">Alignment or Polyline to define the horizontal path of the Pipe Run.</param>
        /// <param name="pressurePartSize"></param>
        /// <param name="surfaceOffset">Vertical offset from Pressure Network reference surface.</param>
        /// <param name="autoAddFittings">Automatically add Fittings?</param>
        /// <returns></returns>
        public static PressurePipeRun ByObject(
            PressureNetwork pressureNetwork,
            string name,
            acDynNodes.Object @object,
            PressurePartSize pressurePartSize,
            double surfaceOffset,
            bool autoAddFittings = true)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new InvalidOperationException("Name is null or empty.");
            }

            if (pressurePartSize.AeccPressurePartSize.Domain != civDb.PressurePartDomainType.Pipe)
            {
                throw new InvalidOperationException("Part Size must be in the Pipe domain.");
            }

            if (pressureNetwork.PipeRuns.Any(p => p.Name == name))
            {
                throw new InvalidOperationException("A Pipe Run with the same name already exists.");
            }

            using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
            {
                
                try
                {
                    acDb.Polyline acPline = null;

                    if (@object is acDynNodes.Polyline)
                    {
                        acPline = (acDb.Polyline)@object.InternalDBObject;
                    }
                    else if (@object is civDynNodes.Alignment)
                    {
                        civDb.Alignment aeccAlign = (civDb.Alignment)ctx.Transaction.GetObject(
                            @object.InternalObjectId,
                            acDb.OpenMode.ForRead);
                        acPline = (acDb.Polyline)ctx.Transaction.GetObject(
                            aeccAlign.GetPolyline(),
                            acDb.OpenMode.ForRead);
                    }
                    else
                    {
                        throw new InvalidOperationException("Object must be an Alignment or Polyline.");
                    }
                    pressureNetwork.AeccPressureNetwork.PipeRuns.createPipeRun(
                        name,
                        acPline,
                        pressurePartSize.AeccPressurePartSize,
                        surfaceOffset,
                        autoAddFittings);
                    return pressureNetwork.PipeRuns.FirstOrDefault(x => x.Name == name);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"PressurePipeRun(Name = {Name})";

        /// <summary>
        /// Adds a horizontal bend to the Pressure Pipe Run at the specified point.
        /// Only the X and Y coordinates of the input point are used.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public PressurePipeRun AddHorizontalBend(Point point)
        {
            bool openedForWrite = PressureNetwork.AeccPressureNetwork.IsWriteEnabled;
            if (!openedForWrite) PressureNetwork.AeccPressureNetwork.UpgradeOpen();
            AeccPressurePipeRun.AddBendByPI((acGeom.Point2d)GeometryConversions.DynPointToAcPoint(point, false));
            if (!openedForWrite) PressureNetwork.AeccPressureNetwork.DowngradeOpen();
            return this;
        }

        /// <summary>
        /// Adds a vertical bend to the Pressure Pipe Run at the specified station and elevation.
        /// If there is no PVI at the specified station and elevation, then a new PVI will be added to the Profile.
        /// </summary>
        /// <param name="station"></param>
        /// <param name="elevation"></param>
        /// <returns></returns>
        public PressurePipeRun AddVerticalBend(double station, double elevation)
        {
            bool openedForWrite = PressureNetwork.AeccPressureNetwork.IsWriteEnabled;
            if (!openedForWrite) PressureNetwork.AeccPressureNetwork.UpgradeOpen();
            AeccPressurePipeRun.AddVerticalBendByPVI(station, elevation);
            if (!openedForWrite) PressureNetwork.AeccPressureNetwork.DowngradeOpen();
            return this;
        }

        /// <summary>
        /// Breaks a Pressure Pipe Run at the specified point.
        /// The original run is retained from the start point to the break point,
        /// and a new run is created from the break point to the original end point.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="newPipeRunName"></param>
        /// <returns></returns>
        [MultiReturn(new[] { "Original Run", "New Run" })]
        public Dictionary<string, object> Break(Point point, string newPipeRunName)
        {
            if (string.IsNullOrEmpty(newPipeRunName))
            {
                throw new ArgumentNullException("Name is null or empty.");
            }

            bool openedForWrite = PressureNetwork.AeccPressureNetwork.IsWriteEnabled;
            if (!openedForWrite) PressureNetwork.AeccPressureNetwork.UpgradeOpen();
            var newRun = AeccPressurePipeRun.Break((acGeom.Point2d)GeometryConversions.DynPointToAcPoint(point, false), newPipeRunName);
            if (!openedForWrite) PressureNetwork.AeccPressureNetwork.DowngradeOpen();
            return new Dictionary<string, object>
            {
                { "Original Run", this },
                { "New Run", newRun }
            };
        }

        /// <summary>
        /// Makes a Pressure Pipe Run follow a Profile with a specified vertical offset.
        /// If "isDynamic" is set to True, the pipe run will update accordingly when the reference Profile is changed. 
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="offsetValue"></param>
        /// <param name="offsetType">True = Cut Length, False = Offset at Fittings</param>
        /// <param name="isDynamic"></param>
        /// <returns></returns>
        public PressurePipeRun FollowProfile(civDynNodes.Profile profile, double offsetValue, bool offsetType, bool isDynamic = true)
        {
            var offsetEnum = civDb.PressurePipeRunVerticalOffsetType.OffsetAtFitting;
            if (offsetType)
            {
                offsetEnum = civDb.PressurePipeRunVerticalOffsetType.CutLength;
            }
            bool openedForWrite = PressureNetwork.AeccPressureNetwork.IsWriteEnabled;
            if (!openedForWrite) PressureNetwork.AeccPressureNetwork.UpgradeOpen();
            AeccPressurePipeRun.FollowProfile(profile.InternalObjectId, offsetEnum, offsetValue, isDynamic);
            if (!openedForWrite) PressureNetwork.AeccPressureNetwork.DowngradeOpen();
            return this;
        }

        /// <summary>
        /// Merges two Pressure Pipe Runs together.
        /// </summary>
        /// <param name="otherRun">The run to be merged.</param>
        /// <returns>The combined run.</returns>
        public PressurePipeRun Merge(PressurePipeRun otherRun)
        {
            bool openedForWrite = PressureNetwork.AeccPressureNetwork.IsWriteEnabled;
            if (!openedForWrite) PressureNetwork.AeccPressureNetwork.UpgradeOpen();
            AeccPressurePipeRun.Merge(otherRun.AeccPressurePipeRun);
            if (!openedForWrite) PressureNetwork.AeccPressureNetwork.DowngradeOpen();
            return this;
        }

        /// <summary>
        /// Removes a horizontal bend from the Pressure Pipe Run nearest to the specified point.
        /// Only the X and Y coordinates of the input point are used.
        /// The original bend and PI are removed and the two connected pipes are merged into one.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public PressurePipeRun RemoveHorizontalBend(Point point)
        {
            bool openedForWrite = PressureNetwork.AeccPressureNetwork.IsWriteEnabled;
            if (!openedForWrite) PressureNetwork.AeccPressureNetwork.UpgradeOpen();
            AeccPressurePipeRun.RemoveBendByPI((acGeom.Point2d)GeometryConversions.DynPointToAcPoint(point, false));
            if (!openedForWrite) PressureNetwork.AeccPressureNetwork.DowngradeOpen();
            return this;
        }

        /// <summary>
        /// Removes a vertical bend from the Pressure Pipe Run nearest to the specified station and elevation.
        /// The original vertical bend and PVI are removed, and the two connected pipes are merged into one.
        /// </summary>
        /// <param name="station"></param>
        /// <param name="elevation"></param>
        /// <returns></returns>
        public PressurePipeRun RemoveVerticalBend(double station, double elevation)
        {
            bool openedForWrite = PressureNetwork.AeccPressureNetwork.IsWriteEnabled;
            if (!openedForWrite) PressureNetwork.AeccPressureNetwork.UpgradeOpen();
            AeccPressurePipeRun.RemoveVerticalBendByPVI(station, elevation);
            if (!openedForWrite) PressureNetwork.AeccPressureNetwork.DowngradeOpen();
            return this;
        }

        /// <summary>
        /// Breaks the dynamic relationship between the Pressure Pipe Run and the reference Profile.
        /// </summary>
        /// <returns></returns>
        public PressurePipeRun ResetReferenceProfile()
        {
            bool openedForWrite = PressureNetwork.AeccPressureNetwork.IsWriteEnabled;
            if (!openedForWrite) PressureNetwork.AeccPressureNetwork.UpgradeOpen();
            AeccPressurePipeRun.ResetReferenceProfile();
            if (!openedForWrite) PressureNetwork.AeccPressureNetwork.DowngradeOpen();
            return this;
        }

        /// <summary>
        /// Sets how the Parts in the Pressure Pipe Run follow the Profile.
        /// </summary>
        /// <param name="offsetType">True = Cut Length, False = Offset at Fittings</param>
        /// <returns></returns>
        public PressurePipeRun SetVerticalOffsetType(bool offsetType)
        {
            var offsetEnum = civDb.PressurePipeRunVerticalOffsetType.OffsetAtFitting;
            if (offsetType)
            {
                offsetEnum = civDb.PressurePipeRunVerticalOffsetType.CutLength;
            }
            bool openedForWrite = PressureNetwork.AeccPressureNetwork.IsWriteEnabled;
            if (!openedForWrite) PressureNetwork.AeccPressureNetwork.UpgradeOpen();
            AeccPressurePipeRun.SetVerticalOffsetType(offsetEnum);
            if (!openedForWrite) PressureNetwork.AeccPressureNetwork.DowngradeOpen();
            return this;
        }
        #endregion
    }
}
