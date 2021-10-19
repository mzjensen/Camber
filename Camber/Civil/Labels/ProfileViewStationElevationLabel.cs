#region references
using System;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acGeom = Autodesk.AutoCAD.Geometry;
using civApp = Autodesk.Civil.ApplicationServices;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using Autodesk.DesignScript.Geometry;
using AeccStationElevationLabel = Autodesk.Civil.DatabaseServices.StationElevationLabel;
using AeccProfile = Autodesk.Civil.DatabaseServices.Profile;
using DynamoServices;
using Camber.Civil.Styles.Objects;
#endregion

namespace Camber.Civil.Labels
{
    [RegisterForTrace]
    public sealed class ProfileViewStationElevationLabel : Label
    {
        #region properties
        internal AeccStationElevationLabel AeccStationElevationLabel => AcObject as AeccStationElevationLabel;

        /// <summary>
        /// Gets the Profile View that a Profile View Station Elevation Label belongs to.
        /// </summary>
        public ProfileView ProfileView { get; set; }

        /// <summary>
        /// Gets the elevation value of a Profile View Station Elevation Label in it's Profile View.
        /// </summary>
        public double Elevation => GetDouble();

        /// <summary>
        /// Gets the station value of a Profile View Station Elevation Label in it's Profile View.
        /// </summary>
        public double Station => GetDouble();

        /// <summary>
        /// Gets the first Profile object of a Profile View Station Elevation Label.
        /// </summary>
        public civDynNodes.Profile Profile1Object => GetProfile(true);

        /// <summary>
        /// Gets the second Profile object of a Profile View Station Elevation Label.
        /// </summary>
        public civDynNodes.Profile Profile2Object => GetProfile(false);
        #endregion

        #region constructors
        internal ProfileViewStationElevationLabel(AeccStationElevationLabel AeccStationOffsetLabel, ProfileView profileView, bool isDynamoOwned = false) : base(AeccStationOffsetLabel, isDynamoOwned)
        {
            ProfileView = profileView;
        }

        /// <summary>
        /// Create a Profile View Station Elevation Label by station and elevation.
        /// </summary>
        /// <param name="profileView"></param>
        /// <param name="station"></param>
        /// <param name="elevation"></param>
        /// <param name="labelStyle"></param>
        /// <param name="markerStyle"></param>
        /// <returns></returns>
        public static ProfileViewStationElevationLabel ByStationElevation(ProfileView profileView, double station, double elevation, ProfileViewStationElevationLabel labelStyle, MarkerStyle markerStyle)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;

            Point point = ProfileView.GetPointAtStationElevation(profileView, station, elevation);

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                civApp.CivilDocument cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                acDb.ObjectId labelId = acDynApp.ElementBinder.GetObjectIdFromTrace(ctx.Database);

                if (labelId.IsValid && !labelId.IsErased)
                {
                    AeccStationElevationLabel aeccLabel = (AeccStationElevationLabel)labelId.GetObject(acDb.OpenMode.ForWrite);
                    if (aeccLabel != null)
                    {
                        // Update the label's location
                        acGeom.Point3d oldLocation = aeccLabel.AnchorInfo.Location;
                        acGeom.Point3d newLocation = new acGeom.Point3d(point.X, point.Y, point.Z);
                        acGeom.Vector3d acVector = oldLocation.GetVectorTo(newLocation);
                        aeccLabel.TransformBy(acGeom.Matrix3d.Displacement(acVector));

                        // Update label style
                        aeccLabel.StyleId = labelStyle.InternalObjectId;

                        // Update marker style
                        aeccLabel.AnchorMarkerStyleId = markerStyle.InternalObjectId;
                    }
                }
                else
                {
                    // Create new label
                    acGeom.Point2d location = new acGeom.Point2d(point.X, point.Y);
                    labelId = AeccStationElevationLabel.Create(profileView.InternalObjectId, labelStyle.InternalObjectId, markerStyle.InternalObjectId, station, elevation);
                }

                var createdLabel = labelId.GetObject(acDb.OpenMode.ForRead) as AeccStationElevationLabel;
                if (createdLabel != null)
                {
                    return new ProfileViewStationElevationLabel(createdLabel, profileView, true);
                }
                return null;
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"ProfileViewStationElevationLabel(Station = {Station:F2}, Elevation = {Elevation:F2})";

        /// <summary>
        /// Gets the Profiles associated with a Profile View Station Elevation Label.
        /// Use True for Profile1 and False for Profile2.
        /// </summary>
        /// <param name="profileSelector">True = Profile1, False = Profile2</param>
        /// <returns></returns>
        private civDynNodes.Profile GetProfile(bool profileSelector)
        {
            try
            {
                acDb.ObjectId profileId = AeccStationElevationLabel.Profile1Id;

                if (!profileSelector)
                {
                    profileId = AeccStationElevationLabel.Profile2Id;
                }

                AeccProfile aeccProfile = (AeccProfile)profileId.GetObject(acDb.OpenMode.ForRead);
                civDynNodes.Alignment parentAlignment = ProfileView.Alignment;
                string profileName = aeccProfile.Name;

                return parentAlignment.ProfileByName(profileName);
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Sets the Profile objects for a Profile View Station Elevation Label.
        /// Use True to set Profile1 and False to set Profile2. 
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="profileSelector">True = Profile1, False = Profile2</param>
        /// <returns></returns>
        public ProfileViewStationElevationLabel SetProfile(civDynNodes.Profile profile, bool profileSelector)
        {            
            // Error checking
            if (profile.Alignment.Name != ProfileView.Alignment.Name)
            {
                throw new ArgumentException("The Profile is not associated with the same Alignment as the Station Elevation Label.");
            }
            
            bool openedForWrite = AeccEntity.IsWriteEnabled;
            if (!openedForWrite) AeccEntity.UpgradeOpen();
            
            if (profileSelector)
            {
                AeccStationElevationLabel.Profile1Id = profile.InternalObjectId;
            }
            else
            {
                AeccStationElevationLabel.Profile2Id = profile.InternalObjectId;
            }

            if (!openedForWrite) AeccEntity.DowngradeOpen();
            return this;
        }

        #endregion
    }
}
