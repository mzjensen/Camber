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
using DynamoServices;
using Camber.Utils;
using Dynamo.Graph.Nodes;
#endregion

namespace Camber.Civil
{
    [RegisterForTrace]
    public sealed class SectionView : CivilObject
    {
        #region properties
        internal AeccSectionView AeccSectionView => AcObject as AeccSectionView;

        /// <summary>
        /// Gets the Sample Line that the Section View is associated with.
        /// </summary>
        public SampleLine SampleLine => SampleLine.GetByObjectId(AeccSectionView.SampleLineId);
       
        /// <summary>
        /// Gets the maximum elevation of the Section View.
        /// </summary>
        public double MaxElevation => GetDouble("ElevationMax");

        /// <summary>
        /// Gets the minimum elevation of the Section View.
        /// </summary>
        public double MinElevation => GetDouble("ElevationMin");

        /// <summary>
        /// Gets whether the elevation range of the Section View is set to Automatic.
        /// </summary>
        public bool IsElevationRangeAutomatic => GetBool();

        /// <summary>
        /// Gets whether the offset range of the Section View is set to Automatic.
        /// </summary>
        public bool IsOffsetRangeAutomatic => GetBool();

        /// <summary>
        /// Gets the location of the Section View.
        /// </summary>
        public Point Location => GeometryConversions.AcPointToDynPoint(AeccSectionView.Location);

        /// <summary>
        /// Gets the left offset of the Section View.
        /// </summary>
        public double OffsetLeft => GetDouble();

        /// <summary>
        /// Gets the right offset of the Section View.
        /// </summary>
        public double OffsetRight => GetDouble();
        #endregion

        #region constructors
        internal SectionView(AeccSectionView aeccSectionView, bool isDynamoOwned = false) : base(aeccSectionView, isDynamoOwned) { }

        [SupressImportIntoVM]
        internal static SectionView GetByObjectId(acDb.ObjectId sectionViewId)
            => CivilObjectSupport.Get<SectionView, AeccSectionView>
            (sectionViewId, (sectionView) => new SectionView(sectionView));

        public static SectionView ByPoint(string name, SampleLine sampleLine, Point location)
        {
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
        /// Converts a Civil Object to a Section View.
        /// </summary>
        /// <param name="civilObject"></param>
        /// <returns></returns>
        [NodeCategory("Actions")]
        public static SectionView GetFromCivilObject(civDynNodes.CivilObject civilObject)
        {
            var document = acDynNodes.Document.Current;
            acDb.ObjectId oid = civilObject.InternalObjectId;
            using (acDynApp.DocumentContext ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                var aeccObject = ctx.Transaction.GetObject(oid, acDb.OpenMode.ForRead);
                if (aeccObject is AeccSectionView)
                {
                    return GetByObjectId(oid);
                }
                else
                {
                    throw new ArgumentException("Object is not a Section View.");
                }
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"SectionView(Name = {Name})";


        /// <summary>
        /// Gets the offset and elevation values of a point in the Section View.
        /// </summary>
        /// <param name="sectionView"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        [MultiReturn(new[] { "Offset", "Elevation" })]
        public static Dictionary<string, object> GetOffsetElevationAtPoint(SectionView sectionView, Point point)
        {
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
        public static Point GetPointAtOffsetElevation(SectionView sectionView, double offset, double elevation)
        {
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
        /// Sets the elevations of the Section View. The elevation range mode will be set to "User specified".
        /// </summary>
        /// <param name="minElevation"></param>
        /// <param name="maxElevation"></param>
        /// <returns></returns>
        public SectionView SetElevations(double minElevation, double maxElevation)
        {
            SetElevationRangeMode(false);
            SetValue(minElevation, "ElevationMin");
            SetValue(maxElevation, "ElevationMax");
            return this;
        }

        /// <summary>
        /// Sets the elevation range mode of the Section View.
        /// </summary>
        /// <param name="bool">True = Automatic, False = User specified</param>
        /// <returns></returns>
        public SectionView SetElevationRangeMode(bool @bool)
        {
            SetValue(@bool, "IsElevationRangeAutomatic");
            return this;
        }

        /// <summary>
        /// Sets the offset range mode of the Section View.
        /// </summary>
        /// <param name="bool">True = Automatic, False = User specified</param>
        /// <returns></returns>
        public SectionView SetOffsetRangeMode(bool @bool)
        {
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
            SetOffsetRangeMode(false);
            SetValue(-offsetLeft, "OffsetLeft");
            SetValue(offsetRight, "OffsetRight");
            return this;
        }


        /// <summary>
        /// Sets the location of the Section View.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public SectionView SetLocation(Point point)
        {
            acGeom.Point3d acPoint = (acGeom.Point3d)GeometryConversions.DynPointToAcPoint(point, true);
            SetValue(acPoint);
            return this;
        }
        #endregion
    }
}
