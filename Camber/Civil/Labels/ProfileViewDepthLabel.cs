#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acGeom = Autodesk.AutoCAD.Geometry;
using civApp = Autodesk.Civil.ApplicationServices;
using Autodesk.DesignScript.Geometry;
using AeccProfileViewDepthLabel = Autodesk.Civil.DatabaseServices.ProfileViewDepthLabel;
using Camber.Civil.Styles.Labels.ProfileView;
using DynamoServices;
using Camber.Utils;
#endregion

namespace Camber.Civil.Labels
{
    [RegisterForTrace]
    public sealed class ProfileViewDepthLabel : Label
    {
        #region properties
        internal AeccProfileViewDepthLabel AeccProfileViewDepthLabel => AcObject as AeccProfileViewDepthLabel;

        /// <summary>
        /// Gets the Profile View that the Profile View Depth Label belongs to.
        /// </summary>
        public ProfileView ProfileView { get; set; }

        /// <summary>
        /// Gets the start point of the Profile View Depth Label.
        /// </summary>
        public Point StartPoint => GeometryConversions.AcPointToDynPoint(AeccProfileViewDepthLabel.StartPoint);

        /// <summary>
        /// Gets the end point of the Profile View Depth Label.
        /// </summary>
        public Point EndPoint => GeometryConversions.AcPointToDynPoint(AeccProfileViewDepthLabel.EndPoint);
        #endregion

        #region constructors
        internal ProfileViewDepthLabel(AeccProfileViewDepthLabel AeccProfileViewDepthLabel, ProfileView profileView, bool isDynamoOwned = false) : base(AeccProfileViewDepthLabel, isDynamoOwned)
        {
            ProfileView = profileView;
        }

        /// <summary>
        /// Creates a Profile View Depth Label by two points
        /// </summary>
        /// <param name="profileView"></param>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="labelStyle"></param>
        /// <returns></returns>
        public static ProfileViewDepthLabel ByTwoPoints(ProfileView profileView, Point startPoint, Point endPoint, ProfileViewDepthLabelStyle labelStyle)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                civApp.CivilDocument cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                acDb.ObjectId labelId = acDynApp.ElementBinder.GetObjectIdFromTrace(ctx.Database);

                var acStartPoint = (acGeom.Point2d)GeometryConversions.DynPointToAcPoint(startPoint, false);
                var acEndPoint = (acGeom.Point2d)GeometryConversions.DynPointToAcPoint(endPoint, false);

                if (labelId.IsValid && !labelId.IsErased)
                {
                    AeccProfileViewDepthLabel aeccLabel = (AeccProfileViewDepthLabel)labelId.GetObject(acDb.OpenMode.ForWrite);
                    if (aeccLabel != null)
                    {
                        // Update the start and end points
                        aeccLabel.StartPoint = acStartPoint;
                        aeccLabel.EndPoint = acEndPoint;

                        // Update label style
                        aeccLabel.StyleId = labelStyle.InternalObjectId;
                    }
                }
                else
                {
                    // Create new label
                    labelId = AeccProfileViewDepthLabel.Create(profileView.InternalObjectId, labelStyle.InternalObjectId, acStartPoint, acEndPoint);
                }

                var createdLabel = labelId.GetObject(acDb.OpenMode.ForRead) as AeccProfileViewDepthLabel;
                if (createdLabel != null)
                {
                    return new ProfileViewDepthLabel(createdLabel, profileView, true);
                }
                return null;
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"ProfileViewDepthLabel(Profile View = {ProfileView.Name})";

        /// <summary>
        /// Sets the start and end points of a Profile View Depth Label.
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public ProfileViewDepthLabel SetPoints(Point startPoint, Point endPoint)
        {            
            SetValue((acGeom.Point2d)GeometryConversions.DynPointToAcPoint(startPoint, false), "StartPoint");
            SetValue((acGeom.Point2d)GeometryConversions.DynPointToAcPoint(endPoint, false), "EndPoint");
            return this;
        }

        #endregion
    }
}
