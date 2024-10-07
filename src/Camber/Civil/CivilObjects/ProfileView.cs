#region references
using System;
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acGeom = Autodesk.AutoCAD.Geometry;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using civDb = Autodesk.Civil.DatabaseServices;
using civApp = Autodesk.Civil.ApplicationServices;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using AeccProfileView = Autodesk.Civil.DatabaseServices.ProfileView;
using AeccAlignment = Autodesk.Civil.DatabaseServices.Alignment;
using Autodesk.DesignScript.Runtime;
using Autodesk.DesignScript.Geometry;
using Camber.Properties;
using DynamoServices;
using Camber.Utilities.GeometryConversions;
#endregion

namespace Camber.Civil.CivilObjects
{
    [RegisterForTrace]
    public sealed class ProfileView : CivilObject
    {
        #region properties
        internal AeccProfileView AeccProfileView => AcObject as AeccProfileView;

        /// <summary>
        /// Gets the elevation range for splitting the Profile View.
        /// </summary>
        public double SplitHeight => GetDouble();

        /// <summary>
        /// Gets whether to split the Profile View into multiple segments.
        /// </summary>
        public bool SplitProfileView => GetBool();

        /// <summary>
        /// Gets the mode for splitting the Profile View.
        /// </summary>
        public string SplitStationMode => AeccProfileView.SplitStationMode.ToString();
        #endregion

        #region constructors
        internal ProfileView(AeccProfileView aeccProfileView, bool isDynamoOwned = false) : base(aeccProfileView, isDynamoOwned) { }

        [SupressImportIntoVM]
        internal static ProfileView GetByObjectId(acDb.ObjectId profileViewId)
            => CivilObjectSupport.Get<ProfileView, AeccProfileView>
            (profileViewId, (profileView) => new ProfileView(profileView));
        #endregion

        #region methods
        public override string ToString() => $"ProfileView(Name = {Name}, Start Station = {StartStation:F2}, End Station = {EndStation:F2})";

        /// <summary>
        /// Sets the elevation range for splitting the Profile View.
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        private ProfileView SetSplitHeight(double height)
        {
            SetValue(height);
            return this;
        }

        /// <summary>
        /// Sets whether to split the Profile View into multiple segments.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        private ProfileView SetSplitProfileView(bool @bool)
        {
            SetValue(@bool);
            return this;
        }

        /// <summary>
        /// Sets the mode for splitting the Profile View.
        /// </summary>
        /// <param name="modeToggle">True = Automatic, False = Manual</param>
        /// <returns></returns>
        private ProfileView SetSplitStationMode(bool modeToggle)
        {
            var mode = civDb.SplitStationType.Automatic;
            if (!modeToggle)
            {
                mode = civDb.SplitStationType.Manual;
            }

            SetValue(mode);
            return this;
        }
        #endregion

        #region deprecated
        /// <summary>
        /// Gets the parent Alignment for the Profile View.
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.ProfileView.Alignment",
            "Autodesk.Civil.DynamoNodes.ProfileView.Alignment")]
        public civDynNodes.Alignment Alignment
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "ProfileView.Alignment"));

                acDb.ObjectId alignmentId = AeccProfileView.AlignmentId;
                AeccAlignment aeccAlignment = (AeccAlignment)alignmentId.GetObject(acDb.OpenMode.ForRead);
                string name = aeccAlignment.Name;
                return civDynNodes.Selection.AlignmentByName(name, acDynNodes.Document.Current);
            }
        }

        /// <summary>
        /// Creates a Profile View by point.
        /// </summary>
        /// <param name="alignment"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.ProfileView.ByPoint",
            "Autodesk.Civil.DynamoNodes.ProfileView.ByAlignment")]
        public static ProfileView ByPoint(civDynNodes.Alignment alignment, Point location)
        {
            LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "ProfileView.ByAlignment"));

            acDynNodes.Document document = acDynNodes.Document.Current;

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                civApp.CivilDocument cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                acDb.ObjectId profileViewId = acDynApp.ElementBinder.GetObjectIdFromTrace(ctx.Database);
                acGeom.Point3d acPoint = (acGeom.Point3d)GeometryConversions.DynPointToAcPoint(location, true);

                if (profileViewId.IsValid && !profileViewId.IsErased)
                {
                    AeccProfileView aeccProfileView = (AeccProfileView)profileViewId.GetObject(acDb.OpenMode.ForWrite);
                    if (aeccProfileView != null)
                    {
                        if (aeccProfileView.AlignmentId != alignment.InternalObjectId)
                        {
                            // If the alignment ID has changed, erase the old Profile View and create a new one
                            aeccProfileView.Erase();
                            profileViewId = AeccProfileView.Create(alignment.InternalObjectId, acPoint);
                        }
                        else
                        {
                            // Update insertion point
                            aeccProfileView.Location = acPoint;
                        }
                    }
                }
                else
                {
                    // Create new Profile View
                    profileViewId = AeccProfileView.Create(alignment.InternalObjectId, acPoint);
                }

                var createdProfileView = profileViewId.GetObject(acDb.OpenMode.ForRead) as AeccProfileView;
                if (createdProfileView != null)
                {
                    return new ProfileView(createdProfileView, true);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the maximum elevation of the Profile View.
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.ProfileView.MaxElevation",
            "Autodesk.Civil.DynamoNodes.ProfileView.MaxElevation")]
        public double MaxElevation
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "ProfileView.MaxElevation"));
                return GetDouble("ElevationMax");
            }
        }

        /// <summary>
        /// Gets the minimum elevation of the ProfileView.
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.ProfileView.MinElevation",
            "Autodesk.Civil.DynamoNodes.ProfileView.MinElevation")]
        public double MinElevation
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "ProfileView.MinElevation"));
                return GetDouble("ElevationMin");
            }
        }

        /// <summary>
        /// Gets how the vertical range of the Profile View is specified.
        /// </summary>
        public string ElevationRangeMode
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MESSAGE, "ProfileView.IsElevationRangeAutomatic"));
                return AeccProfileView.ElevationRangeMode.ToString();
            }
        }

        /// <summary>
        /// Gets the location of the Profile View.
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.ProfileView.Location",
            "Autodesk.AutoCAD.DynamoNodes.Object.Location")]
        public Point Location
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Object.Location"));
                return GeometryConversions.AcPointToDynPoint(AeccProfileView.Location);
            }
        }

        /// <summary>
        /// Gets the end station of the Profile View.
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.ProfileView.EndStation",
            "Autodesk.Civil.DynamoNodes.ProfileView.EndStation")]
        public double EndStation
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "ProfileView.EndStation"));
                return GetDouble("StationEnd");
            }
        }

        /// <summary>
        /// Gets the start station of the Profile View.
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.ProfileView.StartStation",
            "Autodesk.Civil.DynamoNodes.ProfileView.StartStation")]
        public double StartStation
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "ProfileView.StartStation"));
                return GetDouble("StationStart");
            }
        }

        /// <summary>
        /// Gets how the horizontal range of the Profile View is specified.
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        public string StationRangeMode
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MESSAGE, "ProfileView.IsStationRangeAutomatic"));
                return AeccProfileView.StationRangeMode.ToString();
            }
        }

        /// <summary>
        /// Gets all of the Profile Views in a document.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static IList<ProfileView> GetAllProfileViews(acDynNodes.Document document)
        {
            LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MESSAGE, "All Objects of Type"));

            List<ProfileView> pViews = new List<ProfileView>();

            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var bt = (acDb.BlockTable)ctx.Transaction.GetObject(ctx.Database.BlockTableId, acDb.OpenMode.ForRead);
                    var btr = (acDb.BlockTableRecord)ctx.Transaction.GetObject(bt[acDb.BlockTableRecord.ModelSpace], acDb.OpenMode.ForRead);

                    foreach (acDb.ObjectId oid in btr)
                    {
                        var obj = ctx.Transaction.GetObject(oid, acDb.OpenMode.ForRead);
                        if (obj is AeccProfileView)
                        {
                            pViews.Add(GetByObjectId(oid));
                        }
                    }
                    return pViews;
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }

        }

        /// <summary>
        /// Gets a point in the Profile View at the given station and elevation values.
        /// </summary>
        /// <param name="profileView"></param>
        /// <param name="station"></param>
        /// <param name="elevation"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.ProfileView.GetPointAtStationElevation",
            "Autodesk.Civil.DynamoNodes.ProfileView.PointAtStationElevation")]
        public static Point GetPointAtStationElevation(ProfileView profileView, double station, double elevation)
        {
            LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "ProfileView.PointAtStationElevation"));

            // Error checking
            if (profileView is null)
            {
                throw new ArgumentNullException("Profile View is null.");
            }

            double x = 0.0;
            double y = 0.0;

            profileView.AeccProfileView.FindXYAtStationAndElevation(station, elevation, ref x, ref y);
            Point retPoint = Point.ByCoordinates(x, y);

            return retPoint;
        }

        /// <summary>
        /// Gets the station and elevation values of a point in the Profile View.
        /// </summary>
        /// <param name="profileView"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.ProfileView.GetStationElevationAtPoint",
            "Autodesk.Civil.DynamoNodes.ProfileView.StationElevationAtPoint")]
        [MultiReturn(new[] { "Station", "Elevation" })]
        public static Dictionary<string, object> GetStationElevationAtPoint(ProfileView profileView, Point point)
        {
            LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "ProfileView.StationElevationAtPoint"));

            // Error checking
            if (profileView is null)
            {
                throw new ArgumentNullException("Profile View is null.");
            }

            double station = 0.0;
            double elevation = 0.0;

            profileView.AeccProfileView.FindStationAndElevationAtXY(point.X, point.Y, ref station, ref elevation);

            return new Dictionary<string, object>
            {
                { "Station", station },
                { "Elevation", elevation }
            };
        }

        /// <summary>
        /// Sets the elevations for the Profile View. The elevation range mode will be set to "User specified".
        /// </summary>
        /// <param name="minElevation"></param>
        /// <param name="maxElevation"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        public ProfileView SetElevations(double minElevation, double maxElevation)
        {
            LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MESSAGE, "ProfileView.SetMaxElevation or ProfileView.SetMinElevation"));

            SetElevationRangeMode(false);
            SetValue(minElevation, "ElevationMin");
            SetValue(maxElevation, "ElevationMax");
            return this;
        }

        /// <summary>
        /// Sets the vertical range mode of the Profile View.
        /// </summary>
        /// <param name="modeToggle">True = Automatic, False = User specified</param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        public ProfileView SetElevationRangeMode(bool modeToggle)
        {
            LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MESSAGE, "ProfileView.SetElevationRangeAutomatic"));

            var mode = civDb.ElevationRangeType.Automatic;
            if (!modeToggle)
            {
                mode = civDb.ElevationRangeType.UserSpecified;
            }

            SetValue(mode);
            return this;
        }

        /// <summary>
        /// Sets the location of the Profile View.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.ProfileView.SetLocation",
            "Autodesk.AutoCAD.DynamoNodes.Object.SetLocation")]
        public ProfileView SetLocation(Point point)
        {
            LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Object.SetLocation"));

            var acPoint = new acGeom.Point3d(point.X, point.Y, point.Z);
            SetValue(acPoint);
            return this;
        }

        /// <summary>
        /// Sets the stations of the Profile View. The station range mode will be set to "User specified".
        /// </summary>
        /// <param name="startStation"></param>
        /// <param name="endStation"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        public ProfileView SetStations(double startStation, double endStation)
        {
            LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MESSAGE, "ProfileView.SetStartStation or ProfileView.SetEndStation"));

            SetStationRangeMode(false);
            SetValue(startStation, "StationStart");
            SetValue(endStation, "StationEnd");
            return this;
        }


        /// <summary>
        /// Sets the horizontal range mode of the Profile View.
        /// </summary>
        /// <param name="modeToggle">True = Automatic, False = User specified</param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        public ProfileView SetStationRangeMode(bool modeToggle)
        {
            LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MESSAGE, "ProfileView.SetStationRangeAutomatic"));

            var mode = civDb.StationRangeType.Automatic;

            if (!modeToggle)
            {
                mode = civDb.StationRangeType.UserSpecified;
            }

            SetValue(mode);
            return this;
        }
        #endregion
    }
}
