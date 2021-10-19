#region
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civApp = Autodesk.Civil.ApplicationServices;
using AeccGeneralSegmentLabel = Autodesk.Civil.DatabaseServices.GeneralSegmentLabel;
using DynamoServices;
using Camber.Civil.Styles.Labels.General;
#endregion

namespace Camber.Civil.Labels
{
    [RegisterForTrace]
    public sealed class GeneralSegmentLabel : Label
    {
        #region properties
        internal AeccGeneralSegmentLabel AeccGeneralSegmentLabel => AcObject as AeccGeneralSegmentLabel;

        /// <summary>
        /// Gets the ratio that determines the relative position of the General Segment Label on its object.
        /// </summary>
        public double Ratio => GetDouble();
        #endregion

        #region constructors
        internal GeneralSegmentLabel(AeccGeneralSegmentLabel AeccGeneralNoteLabel, bool isDynamoOwned = false) : base(AeccGeneralNoteLabel, isDynamoOwned) { }

        /// <summary>
        /// Creates a General Segment Label by object. The object type must be a Line, Arc, Polyline, or Feature Line.
        /// For Line and Arc objects, the ratio should be in the range of [0,1].
        /// For Polyline and Feature Line objects with n segments, the ratio should be in the range of [0,n].
        /// </summary>
        /// <param name="obj">Line, Arc, Polyline, or Feature Line.</param>
        /// <param name="ratio"></param>
        /// <param name="lineLabelStyle"></param>
        /// <param name="curveLabelStyle"></param>
        /// <returns></returns>
        public static GeneralSegmentLabel ByObject(acDynNodes.Object obj, double ratio, GeneralLineLabelStyle lineLabelStyle, GeneralCurveLabelStyle curveLabelStyle)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                civApp.CivilDocument cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                acDb.ObjectId labelId = acDynApp.ElementBinder.GetObjectIdFromTrace(ctx.Database);

                if (labelId.IsValid && !labelId.IsErased)
                {
                    AeccGeneralSegmentLabel aeccLabel = (AeccGeneralSegmentLabel)labelId.GetObject(acDb.OpenMode.ForWrite);
                    if (aeccLabel != null)
                    {
                        // If the label object ID has changed, erase the old label and create a new one
                        if (obj.InternalObjectId != aeccLabel.FeatureId)
                        {
                            aeccLabel.Erase();
                            labelId = AeccGeneralSegmentLabel.Create(obj.InternalObjectId, ratio, lineLabelStyle.InternalObjectId, curveLabelStyle.InternalObjectId);
                        }
                        else
                        {
                            // Update the ratio
                            aeccLabel.Ratio = ratio;

                            // Update line label style
                            aeccLabel.LineLabelStyleId = lineLabelStyle.InternalObjectId;

                            // Update curve label style
                            aeccLabel.CurveLabelStyleId = curveLabelStyle.InternalObjectId;
                        }
                    }
                }
                else
                {
                    // Create new label
                    labelId = AeccGeneralSegmentLabel.Create(obj.InternalObjectId, ratio, lineLabelStyle.InternalObjectId, curveLabelStyle.InternalObjectId);
                }

                var createdLabel = labelId.GetObject(acDb.OpenMode.ForRead) as AeccGeneralSegmentLabel;
                if (createdLabel != null)
                {
                    return new GeneralSegmentLabel(createdLabel, true);
                }
                return null;
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"GeneralSegmentLabel(Ratio = {Ratio:F2})";

        /// <summary>
        /// Sets the ratio that determines the relative position of a General Segment Label on its object.
        /// </summary>
        /// <param name="ratio"></param>
        /// <returns></returns>
        public GeneralSegmentLabel SetRatio(double ratio)
        {
            SetValue(ratio);
            return this;
        }
        #endregion
    }
}
