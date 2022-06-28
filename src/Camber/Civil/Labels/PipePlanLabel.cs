#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civApp = Autodesk.Civil.ApplicationServices;
using AeccPipeLabel = Autodesk.Civil.DatabaseServices.PipeLabel;
using Autodesk.DesignScript.Runtime;
using DynamoServices;
using Camber.Civil.PipeNetworks.Parts;
using Camber.Civil.Styles.Labels.Pipe;
using Camber.Civil.CivilObjects;

#endregion

namespace Camber.Civil.Labels
{
    [RegisterForTrace]
    public sealed class PipePlanLabel : Label
    {
        #region properties
        internal AeccPipeLabel AeccPipeLabel => AcObject as AeccPipeLabel;

        /// <summary>
        /// Gets the Pipe that a Pipe Plan Label is associated with.
        /// </summary>
        public Pipe Pipe => Pipe.GetByObjectId(AeccPipeLabel.FeatureId);

        /// <summary>
        /// Gets the ratio that determines the relative position of the Pipe Plan Label on its Pipe.
        /// </summary>
        public double Ratio => GetDouble();
        #endregion

        #region constructors
        internal PipePlanLabel(
            AeccPipeLabel AeccPipeLabel, 
            bool isDynamoOwned = false) 
            : base(AeccPipeLabel, isDynamoOwned) { }

        [SupressImportIntoVM]
        internal static PipePlanLabel GetByObjectId(acDb.ObjectId labelId)
            => CivilObjectSupport.Get<PipePlanLabel, AeccPipeLabel>
            (labelId, (label) => new PipePlanLabel(label));

        /// <summary>
        /// Creates a Pipe Plan Label by Pipe. The ratio should be in the range of [0,1].
        /// </summary>
        /// <param name="pipe"></param>
        /// <param name="ratio"></param>
        /// <param name="labelStyle"></param>
        /// <returns></returns>
        public static PipePlanLabel ByPipe(
            Pipe pipe, 
            double ratio, 
            PipePlanProfileLabelStyle labelStyle)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                civApp.CivilDocument cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                acDb.ObjectId labelId = acDynApp.ElementBinder.GetObjectIdFromTrace(ctx.Database);

                if (labelId.IsValid && !labelId.IsErased)
                {
                    AeccPipeLabel aeccLabel = (AeccPipeLabel)labelId.GetObject(acDb.OpenMode.ForWrite);
                    if (aeccLabel != null)
                    {
                        // If the pipe object ID has changed, erase the old label and create a new one
                        if (pipe.InternalObjectId != aeccLabel.FeatureId)
                        {
                            aeccLabel.Erase();
                            labelId = AeccPipeLabel.Create(
                                pipe.InternalObjectId, 
                                ratio, 
                                labelStyle.InternalObjectId);
                        }
                        else
                        {
                            // Update the ratio
                            aeccLabel.Ratio = ratio;

                            // Update label style
                            aeccLabel.StyleId = labelStyle.InternalObjectId;
                        }
                    }
                }
                else
                {
                    // Create new label
                    labelId = AeccPipeLabel.Create(
                        pipe.InternalObjectId, 
                        ratio, 
                        labelStyle.InternalObjectId);
                }

                var createdLabel = labelId.GetObject(acDb.OpenMode.ForRead) as AeccPipeLabel;
                if (createdLabel != null)
                {
                    return new PipePlanLabel(createdLabel, true);
                }
                return null;
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"PipePlanLabel(Ratio = {Ratio:F2})";

        /// <summary>
        /// Sets the ratio that determines the relative position of a Pipe Plan Label on its Pipe.
        /// </summary>
        /// <param name="ratio"></param>
        /// <returns></returns>
        public PipePlanLabel SetRatio(double ratio)
        {
            SetValue(ratio);
            return this;
        }
        #endregion
    }
}
