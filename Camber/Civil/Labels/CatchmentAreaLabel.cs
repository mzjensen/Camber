#region
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civApp = Autodesk.Civil.ApplicationServices;
using AeccCatchmentAreaLabel = Autodesk.Civil.DatabaseServices.CatchmentLabel;
using DynamoServices;
using Camber.Civil.Styles.Labels.Catchment;
#endregion

namespace Camber.Civil.Labels
{
    [RegisterForTrace]
    public sealed class CatchmentAreaLabel : Label
    {
        #region properties
        internal AeccCatchmentAreaLabel AeccCatchmentAreaLabel => AcObject as AeccCatchmentAreaLabel;

        /// <summary>
        /// Gets the Catchment that the Catchment Area Label is associated with.
        /// </summary>
        public Catchment Catchment { get; set; }
        #endregion

        #region constructors
        internal CatchmentAreaLabel(AeccCatchmentAreaLabel AeccCatchmentAreaLabel, Catchment catchment, bool isDynamoOwned = false) : base(AeccCatchmentAreaLabel, isDynamoOwned)
        {
            Catchment = catchment;
        }

        /// <summary>
        /// Creates a Catchment Area Label by Catchment.
        /// </summary>
        /// <param name="catchment"></param>
        /// <param name="labelStyle"></param>
        /// <returns></returns>
        public static CatchmentAreaLabel ByCatchment(Catchment catchment, CatchmentAreaLabelStyle labelStyle)
        {

            acDynNodes.Document document = acDynNodes.Document.Current;

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                civApp.CivilDocument cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                acDb.ObjectId labelId = acDynApp.ElementBinder.GetObjectIdFromTrace(ctx.Database);

                if (labelId.IsValid && !labelId.IsErased)
                {
                    AeccCatchmentAreaLabel aeccLabel = (AeccCatchmentAreaLabel)labelId.GetObject(acDb.OpenMode.ForWrite);
                    if (aeccLabel != null)
                    {
                        // Update label style
                        aeccLabel.StyleId = labelStyle.InternalObjectId;
                    }
                }
                else
                {
                    // Create new label
                    labelId = AeccCatchmentAreaLabel.Create(catchment.InternalObjectId, labelStyle.InternalObjectId);
                }

                var createdLabel = labelId.GetObject(acDb.OpenMode.ForRead) as AeccCatchmentAreaLabel;
                if (createdLabel != null)
                {
                    return new CatchmentAreaLabel(createdLabel, catchment, true);
                }
                return null;
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"CatchmentAreaLabel(Catchment = {Catchment.Name})";
        #endregion
    }
}
