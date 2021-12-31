#region
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civApp = Autodesk.Civil.ApplicationServices;
using AeccCatchmentFlowSegmentLabel = Autodesk.Civil.DatabaseServices.FlowSegmentLabel;
using DynamoServices;
using System;
using Camber.Civil.Styles.Labels.Catchment;
using Camber.Civil.CivilObjects;
#endregion

namespace Camber.Civil.Labels
{
    [RegisterForTrace]
    public sealed class CatchmentFlowSegmentLabel : Label
    {
        #region properties
        internal AeccCatchmentFlowSegmentLabel AeccCatchmentFlowSegmentLabel => AcObject as AeccCatchmentFlowSegmentLabel;

        /// <summary>
        /// Gets the Catchment that a Catchment Flow Segment Label is associated with.
        /// </summary>
        public Catchment Catchment { get; set; }

        /// <summary>
        /// Gets the index of the Flow Segment within the Flow Path that a Catchment Flow Segment Label is associated with.
        /// </summary>
        public int FlowSegmentIndex => AeccCatchmentFlowSegmentLabel.FlowSegmentIndex;

        /// <summary>
        /// Gets the location of a Catchment Flow Segment Label along a Flow Path, expressed as a ratio between zero and one.
        /// </summary>
        public double Ratio => AeccCatchmentFlowSegmentLabel.Ratio;
        #endregion

        #region constructors
        internal CatchmentFlowSegmentLabel(AeccCatchmentFlowSegmentLabel AeccCatchmentFlowSegmentLabel, Catchment catchment, bool isDynamoOwned = false) : base(AeccCatchmentFlowSegmentLabel, isDynamoOwned)
        {
            Catchment = catchment;
        }

        /// <summary>
        /// Creates a Catchment Flow Segment Label by Catchment.
        /// </summary>
        /// <param name="catchment"></param>
        /// <param name="labelStyle"></param>
        /// <returns></returns>
        public static CatchmentFlowSegmentLabel ByCatchment(Catchment catchment, CatchmentFlowSegmentLabelStyle labelStyle, int flowSegmentIndex, double ratio = 0.5)
        {

            acDynNodes.Document document = acDynNodes.Document.Current;

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                civApp.CivilDocument cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                acDb.ObjectId labelId = acDynApp.ElementBinder.GetObjectIdFromTrace(ctx.Database);

                if (labelId.IsValid && !labelId.IsErased)
                {
                    AeccCatchmentFlowSegmentLabel aeccLabel = (AeccCatchmentFlowSegmentLabel)labelId.GetObject(acDb.OpenMode.ForWrite);
                    if (aeccLabel != null)
                    {
                        if (aeccLabel.Ratio != ratio || aeccLabel.FlowSegmentIndex != flowSegmentIndex)
                        {
                            // Update properties
                            aeccLabel.FlowSegmentIndex = flowSegmentIndex;
                            aeccLabel.Ratio = ratio;
                            aeccLabel.StyleId = labelStyle.InternalObjectId;
                        }
                    }
                }
                else
                {
                    // Create new label
                    var flowPath = catchment.AeccCatchment.GetFlowPath();
                    var flowSegment = flowPath.GetFlowSegmentAt(flowSegmentIndex);
                    labelId = AeccCatchmentFlowSegmentLabel.Create(flowSegment, labelStyle.InternalObjectId);
                }

                var createdLabel = labelId.GetObject(acDb.OpenMode.ForRead) as AeccCatchmentFlowSegmentLabel;
                if (createdLabel != null)
                {
                    return new CatchmentFlowSegmentLabel(createdLabel, catchment, true);
                }
                return null;
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"CatchmentFlowSegmentLabel(Flow Segment Index = {FlowSegmentIndex}, Ratio = {Ratio})";

        /// <summary>
        /// Sets the index of the Flow Segment within the Flow Path that the Catchment Flow Segment Label is associated with.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public CatchmentFlowSegmentLabel SetFlowSegmentIndex(int index)
        {
            SetValue(index);
            return this;
        }

        /// <summary>
        /// Sets the location of the Catchment Flow Segment Label along the Flow Path.
        /// </summary>
        /// <param name="ratio">The position along the Flow Segment between zero and one.</param>
        /// <returns></returns>
        public CatchmentFlowSegmentLabel SetRatio(double ratio)
        {
            if (ratio < 0 || ratio > 1)
            {
                throw new ArgumentException("Ratio must be greater or equal to zero and less than or equal to one.");
            }
            
            SetValue(ratio);
            return this;
        }
        #endregion
    }
}
