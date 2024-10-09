#region references
using System;
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acGeom = Autodesk.AutoCAD.Geometry;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using civApp = Autodesk.Civil.ApplicationServices;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using AeccSectionView = Autodesk.Civil.DatabaseServices.SectionView;
using Autodesk.DesignScript.Runtime;
using Autodesk.DesignScript.Geometry;
using Camber.Properties;
using DynamoServices;
using Camber.Utilities.GeometryConversions;
#endregion

namespace Camber.Civil.CivilObjects
{
    [RegisterForTrace]
    public sealed class SectionView : CivilObject
    {
        #region properties
        internal AeccSectionView AeccSectionView => AcObject as AeccSectionView;
        #endregion

        #region constructors
        internal SectionView(AeccSectionView aeccSectionView, bool isDynamoOwned = false) : base(aeccSectionView, isDynamoOwned) { }

        [SupressImportIntoVM]
        internal static SectionView GetByObjectId(acDb.ObjectId sectionViewId)
            => CivilObjectSupport.Get<SectionView, AeccSectionView>
            (sectionViewId, (sectionView) => new SectionView(sectionView));
        #endregion

        #region methods
        public override string ToString() => $"SectionView(Name = {Name})";
        #endregion

        #region deprecated
        public static SectionView ByPoint(string name, SampleLine sampleLine, Point location)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MESSAGE, "SectionView.BySampleLine"));

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Section View name is null");
            }

            acDynNodes.Document document = acDynNodes.Document.Current;

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                civApp.CivilDocument cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                acDb.ObjectId sectionViewId = acDynApp.ElementBinder.GetObjectIdFromTrace(ctx.Database);
                acGeom.Point3d acPoint = (acGeom.Point3d)GeometryConversions.DynPointToAcPoint(location, true);

                if (sectionViewId.IsValid && !sectionViewId.IsErased)
                {
                    AeccSectionView aeccSectionView = (AeccSectionView)sectionViewId.GetObject(acDb.OpenMode.ForWrite);
                    if (aeccSectionView != null)
                    {
                        if (aeccSectionView.SampleLineId != sampleLine.InternalObjectId)
                        {
                            // If the sample line ID has changed, erase the old Section View and create a new one
                            aeccSectionView.Erase();
                            sectionViewId = AeccSectionView.Create(name, sampleLine.InternalObjectId, acPoint);
                        }
                        else
                        {
                            // Update properties
                            aeccSectionView.Location = acPoint;
                            aeccSectionView.Name = name;
                        }
                    }
                }
                else
                {
                    // Create new Section View
                    sectionViewId = AeccSectionView.Create(name, sampleLine.InternalObjectId, acPoint);
                }

                var createdSectionView = sectionViewId.GetObject(acDb.OpenMode.ForRead) as AeccSectionView;
                if (createdSectionView != null)
                {
                    return new SectionView(createdSectionView, true);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the offset and elevation values of a point in the Section View.
        /// </summary>
        /// <param name="sectionView"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.SectionView.GetOffsetElevationAtPoint",
            "Autodesk.Civil.DynamoNodes.SectionView.OffsetElevationAtPoint")]
        [MultiReturn(new[] { "Offset", "Elevation" })]
        public static Dictionary<string, object> GetOffsetElevationAtPoint(SectionView sectionView, Point point)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "SectionView.OffsetElevationAtPoint"));

            // Error checking
            if (sectionView is null)
            {
                throw new ArgumentNullException("Section View is null.");
            }

            double offset = 0.0;
            double elevation = 0.0;

            sectionView.AeccSectionView.FindOffsetAndElevationAtXY(point.X, point.Y, ref offset, ref elevation);

            return new Dictionary<string, object>
            {
                { "Offset", offset },
                { "Elevation", elevation }
            };
        }

        /// <summary>
        /// Gets a point in the Section View at the given offset and elevation values.
        /// </summary>
        /// <param name="sectionView"></param>
        /// <param name="offset"></param>
        /// <param name="elevation"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.SectionView.GetPointAtOffsetElevation",
            "Autodesk.Civil.DynamoNodes.SectionView.PointAtOffsetElevation")]
        public static Point GetPointAtOffsetElevation(SectionView sectionView, double offset, double elevation)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "SectionView.PointAtOffsetElevation"));

            // Error checking
            if (sectionView is null)
            {
                throw new ArgumentNullException("Section View is null.");
            }

            double x = 0.0;
            double y = 0.0;

            sectionView.AeccSectionView.FindXYAtOffsetAndElevation(offset, elevation, ref x, ref y);
            Point retPoint = Point.ByCoordinates(x, y);

            return retPoint;
        }

        /// <summary>
        /// Gets all Section Views in a Document.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static IList<SectionView> GetSectionViews(acDynNodes.Document document)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MESSAGE, "All Objects of Type"));

            List<SectionView> sectViews = new List<SectionView>();
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                var bt = (acDb.BlockTable)ctx.Transaction.GetObject(ctx.Database.BlockTableId, acDb.OpenMode.ForRead);
                var btr = (acDb.BlockTableRecord)ctx.Transaction.GetObject(
                    acDb.SymbolUtilityServices.GetBlockModelSpaceId(document.AcDocument.Database),
                    acDb.OpenMode.ForRead);
                foreach (acDb.ObjectId oid in btr)
                {
                    var obj = ctx.Transaction.GetObject(oid, acDb.OpenMode.ForRead);
                    if (obj is AeccSectionView)
                    {
                        sectViews.Add(SectionView.GetByObjectId(oid));
                    }
                }

                return sectViews;
            }
        }

        /// <summary>
        /// Gets whether the elevation range of the Section View is set to Automatic.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.SectionView.IsElevationRangeAutomatic",
            "Autodesk.Civil.DynamoNodes.SectionView.IsElevationRangeAutomatic")]
        public bool IsElevationRangeAutomatic
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "SectionView.IsElevationRangeAutomatic"));
                return GetBool();
            }
        }

        /// <summary>
        /// Gets whether the offset range of the Section View is set to Automatic.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.SectionView.IsOffsetRangeAutomatic",
            "Autodesk.Civil.DynamoNodes.SectionView.IsOffsetRangeAutomatic")]
        public bool IsOffsetRangeAutomatic
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "SectionView.IsOffsetRangeAutomatic"));
                return GetBool();
            }
        }

        /// <summary>
        /// Gets the location of the Section View.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.SectionView.Location",
            "Autodesk.AutoCAD.DynamoNodes.Object.Location")]
        public Point Location
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Object.Location"));
                return GeometryConversions.AcPointToDynPoint(AeccSectionView.Location);
            }
        }

        /// <summary>
        /// Gets the maximum elevation of the Section View.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.SectionView.MaxElevation",
            "Autodesk.Civil.DynamoNodes.SectionView.MaxElevation")]
        public double MaxElevation
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "SectionView.MaxElevation"));
                return GetDouble("ElevationMax");
            }
        }

        /// <summary>
        /// Gets the minimum elevation of the Section View.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.SectionView.MinElevation",
            "Autodesk.Civil.DynamoNodes.SectionView.MinElevation")]
        public double MinElevation
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "SectionView.MinElevation"));
                return GetDouble("ElevationMin");
            }
        }

        /// <summary>
        /// Gets the left offset of the Section View.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.SectionView.OffsetLeft",
            "Autodesk.Civil.DynamoNodes.SectionView.LeftOffset")]
        public double OffsetLeft
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "SectionView.LeftOffset"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the right offset of the Section View.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.SectionView.OffsetRight",
            "Autodesk.Civil.DynamoNodes.SectionView.RightOffset")]
        public double OffsetRight
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "SectionView.RightOffset"));
                return GetDouble();
            }
        }

        /// <summary>
        /// Gets the Sample Line that the Section View is associated with.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.SectionView.SampleLine",
            "Autodesk.Civil.DynamoNodes.SectionView.SampleLine")]
        public SampleLine SampleLine
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "SectionView.SampleLine"));
                return SampleLine.GetByObjectId(AeccSectionView.SampleLineId);
            }
        }

        /// <summary>
        /// Sets the elevation range mode of the Section View.
        /// </summary>
        /// <param name="bool">True = Automatic, False = User specified</param>
        /// <returns></returns>
        public SectionView SetElevationRangeMode(bool @bool)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MESSAGE, "SectionView.SetElevationRangeAutomatic"));
            
            SetValue(@bool, "IsElevationRangeAutomatic");
            return this;
        }

        /// <summary>
        /// Sets the elevations of the Section View. The elevation range mode will be set to "User specified".
        /// </summary>
        /// <param name="minElevation"></param>
        /// <param name="maxElevation"></param>
        /// <returns></returns>
        public SectionView SetElevations(double minElevation, double maxElevation)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MESSAGE, "SectionView.SetMinElevation and SectionView.SetMaxElevation"));

            SetElevationRangeMode(false);
            SetValue(minElevation, "ElevationMin");
            SetValue(maxElevation, "ElevationMax");
            return this;
        }

        /// <summary>
        /// Sets the location of the Section View.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.SectionView.SetLocation",
            "Autodesk.AutoCAD.DynamoNodes.Object.SetLocation")]
        public SectionView SetLocation(Point point)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Object.SetLocation"));

            acGeom.Point3d acPoint = (acGeom.Point3d)GeometryConversions.DynPointToAcPoint(point, true);
            SetValue(acPoint);
            return this;
        }

        /// <summary>
        /// Sets the offset range mode of the Section View.
        /// </summary>
        /// <param name="bool">True = Automatic, False = User specified</param>
        /// <returns></returns>
        public SectionView SetOffsetRangeMode(bool @bool)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "SectionView.SetOffsetRangeAutomatic"));

            SetValue(@bool, "IsOffsetRangeAutomatic");
            return this;
        }

        /// <summary>
        /// Sets the offset values of the Section View. The offset range mode will be set to "User specified".
        /// </summary>
        /// <param name="offsetLeft"></param>
        /// <param name="offsetRight"></param>
        /// <returns></returns>
        public SectionView SetOffsets(double offsetLeft, double offsetRight)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "SectionView.SetLeftOffset and SectionView.SetRightOffset"));

            SetOffsetRangeMode(false);
            SetValue(-offsetLeft, "OffsetLeft");
            SetValue(offsetRight, "OffsetRight");
            return this;
        }
        #endregion
    }
}
