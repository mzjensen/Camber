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
using DynamoServices;
using Dynamo.Graph.Nodes;
using Camber.Utils;
#endregion

namespace Camber.Civil
{
    [RegisterForTrace]
    public sealed class ProfileView : CivilObjectExtensions
    {
        #region properties
        internal AeccProfileView AeccProfileView => AcObject as AeccProfileView;

        /// <summary>
        /// Gets the parent Alignment for the Profile View.
        /// </summary>
        public civDynNodes.Alignment Alignment
        {
            get
            {
                acDb.ObjectId alignmentId = AeccProfileView.AlignmentId;
                AeccAlignment aeccAlignment = (AeccAlignment)alignmentId.GetObject(acDb.OpenMode.ForRead);
                string name = aeccAlignment.Name;
                return civDynNodes.Selection.AlignmentByName(name, acDynNodes.Document.Current);
            }
        }

        /// <summary>
        /// Gets the maximum elevation of the Profile View.
        /// </summary>
        public double MaxElevation => GetDouble("ElevationMax");

        /// <summary>
        /// Gets the minimum elevation of the ProfileView.
        /// </summary>
        public double MinElevation => GetDouble("ElevationMin");

        /// <summary>
        /// Gets how the vertical range of the Profile View is specified.
        /// </summary>
        public string ElevationRangeMode => AeccProfileView.ElevationRangeMode.ToString();

        /// <summary>
        /// Gets the location of the Profile View.
        /// </summary>
        public Point Location => GeometryConversions.AcPointToDynPoint(AeccProfileView.Location);

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

        /// <summary>
        /// Gets the end station of the Profile View.
        /// </summary>
        public double EndStation => GetDouble("StationEnd");

        /// <summary>
        /// Gets the start station of the Profile View.
        /// </summary>
        public double StartStation => GetDouble("StationStart");

        /// <summary>
        /// Gets how the horizontal range of the Profile View is specified.
        /// </summary>
        public string StationRangeMode => AeccProfileView.StationRangeMode.ToString();
        #endregion

        #region constructors
        internal ProfileView(AeccProfileView aeccProfileView, bool isDynamoOwned = false) : base(aeccProfileView, isDynamoOwned) { }

        [SupressImportIntoVM]
        internal static ProfileView GetByObjectId(acDb.ObjectId profileViewId)
            => CivilObjectSupport.Get<ProfileView, AeccProfileView>
            (profileViewId, (profileView) => new ProfileView(profileView));

        /// <summary>
        /// Creates a Profile View by point.
        /// </summary>
        /// <param name="alignment"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static ProfileView ByPoint(civDynNodes.Alignment alignment, Point location)
        {   
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
        /// Converts a Civil Object to a Profile View.
        /// </summary>
        /// <param name="civilObject"></param>
        /// <returns></returns>
        [NodeCategory("Actions")]
        public static ProfileView GetFromCivilObject(civDynNodes.CivilObject civilObject)
        {
            var document = acDynNodes.Document.Current;
            acDb.ObjectId oid = civilObject.InternalObjectId;
            using (acDynApp.DocumentContext ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                var aeccObject = ctx.Transaction.GetObject(oid, acDb.OpenMode.ForRead);
                if (aeccObject is AeccProfileView)
                {
                    return GetByObjectId(oid);
                }
                else
                {
                    throw new ArgumentException("Object is not a Profile View.");
                }
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"ProfileView(Name = {Name}, Start Station = {StartStation:F2}, End Station = {EndStation:F2})";

        /// <summary>
        /// Gets the station and elevation values of a point in the Profile View.
        /// </summary>
        /// <param name="profileView"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        [MultiReturn(new[] { "Station", "Elevation" })]
        public static Dictionary<string, object> GetStationElevationAtPoint(ProfileView profileView, Point point)
        {
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
        /// Gets a point in the Profile View at the given station and elevation values.
        /// </summary>
        /// <param name="profileView"></param>
        /// <param name="station"></param>
        /// <param name="elevation"></param>
        /// <returns></returns>
        public static Point GetPointAtStationElevation(ProfileView profileView, double station, double elevation)
        {
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
        /// Sets the elevations for the Profile View. The elevation range mode will be set to "User specified".
        /// </summary>
        /// <param name="minElevation"></param>
        /// <param name="maxElevation"></param>
        /// <returns></returns>
        public ProfileView SetElevations(double minElevation, double maxElevation)
        {
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
        public ProfileView SetElevationRangeMode(bool modeToggle)
        {
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
        public ProfileView SetLocation(Point point)
        {
            var acPoint = new acGeom.Point3d(point.X, point.Y, point.Z);
            SetValue(acPoint);
            return this;
        }

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

        /// <summary>
        /// Sets the stations of the Profile View. The station range mode will be set to "User specified".
        /// </summary>
        /// <param name="startStation"></param>
        /// <param name="endStation"></param>
        /// <returns></returns>
        public ProfileView SetStations(double startStation, double endStation)
        {
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
        public ProfileView SetStationRangeMode(bool modeToggle)
        {
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
