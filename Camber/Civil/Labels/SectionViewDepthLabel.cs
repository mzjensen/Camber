#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acGeom = Autodesk.AutoCAD.Geometry;
using civApp = Autodesk.Civil.ApplicationServices;
using Autodesk.DesignScript.Geometry;
using AeccSectionViewDepthLabel = Autodesk.Civil.DatabaseServices.SectionViewDepthLabel;
using Camber.Civil.Styles.Labels.SectionView;
using DynamoServices;
using Camber.Utils;
using Camber.Civil.CivilObjects;
#endregion

namespace Camber.Civil.Labels
{
    [RegisterForTrace]
    public sealed class SectionViewDepthLabel : Label
    {
        #region properties
        internal AeccSectionViewDepthLabel AeccSectionViewDepthLabel => AcObject as AeccSectionViewDepthLabel;

        /// <summary>
        /// Gets the Section View that a Section View Depth Label belongs to.
        /// </summary>
        public SectionView SectionView { get; set; }

        /// <summary>
        /// Gets the start point of a Section View Depth Label.
        /// </summary>
        public Point StartPoint => GeometryConversions.AcPointToDynPoint(AeccSectionViewDepthLabel.StartPoint);

        /// <summary>
        /// Gets the end point of a Section View Depth Label.
        /// </summary>
        public Point EndPoint => GeometryConversions.AcPointToDynPoint(AeccSectionViewDepthLabel.EndPoint);
        #endregion

        #region constructors
        internal SectionViewDepthLabel(AeccSectionViewDepthLabel AeccSectionViewDepthLabel, SectionView sectionView, bool isDynamoOwned = false) : base(AeccSectionViewDepthLabel, isDynamoOwned)
        {
            SectionView = sectionView;
        }

        /// <summary>
        /// Create a Section View Depth Label by two points.
        /// </summary>
        /// <param name="sectionView"></param>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="labelStyle"></param>
        /// <returns></returns>
        public static SectionViewDepthLabel ByTwoPoints(SectionView sectionView, Point startPoint, Point endPoint, SectionViewDepthLabelStyle labelStyle)
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
                    AeccSectionViewDepthLabel aeccLabel = (AeccSectionViewDepthLabel)labelId.GetObject(acDb.OpenMode.ForWrite);
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
                    labelId = AeccSectionViewDepthLabel.Create(sectionView.InternalObjectId, acStartPoint, acEndPoint, labelStyle.InternalObjectId);
                }

                var createdLabel = labelId.GetObject(acDb.OpenMode.ForRead) as AeccSectionViewDepthLabel;
                if (createdLabel != null)
                {
                    return new SectionViewDepthLabel(createdLabel, sectionView, true);
                }
                return null;
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"SectionViewDepthLabel(Section View = {SectionView.Name})";

        /// <summary>
        /// Sets the start and end points of a Section View Depth Label.
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public SectionViewDepthLabel SetPoints(Point startPoint, Point endPoint)
        {            
            SetValue((acGeom.Point2d)GeometryConversions.DynPointToAcPoint(startPoint, false), "StartPoint");
            SetValue((acGeom.Point2d)GeometryConversions.DynPointToAcPoint(endPoint, false), "EndPoint");
            return this;
        }
        #endregion
    }
}
