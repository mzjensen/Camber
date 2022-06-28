#region references

using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civDb = Autodesk.Civil.DatabaseServices;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using AeccViewFrame = Autodesk.Civil.DatabaseServices.ViewFrame;
using AeccViewFrameGroup = Autodesk.Civil.DatabaseServices.ViewFrameGroup;
using AeccAlignment = Autodesk.Civil.DatabaseServices.Alignment;
using Autodesk.DesignScript.Runtime;
using DynamoServices;
#endregion

namespace Camber.Civil.CivilObjects
{
    [RegisterForTrace]
    public sealed class ViewFrame : CivilObject
    {
        #region properties
        internal AeccViewFrame AeccViewFrame => AcObject as AeccViewFrame;


        /// <summary>
        /// Gets the parent Alignment of a View Frame.
        /// </summary>
        public civDynNodes.Alignment Alignment
        {
            get
            {
                acDb.ObjectId alignmentId = AeccViewFrame.AlignmentId;
                AeccAlignment aeccAlignment = (AeccAlignment)alignmentId.GetObject(acDb.OpenMode.ForRead);
                string name = aeccAlignment.Name;
                return civDynNodes.Selection.AlignmentByName(name, acDynNodes.Document.Current);
            }
        }

        /// <summary>
        /// Gets the raw end station of a View Frame.
        /// </summary>
        public double EndStation => GetDouble();

        /// <summary>
        /// Gets the View Frame Group that a View Frame belongs to.
        /// </summary>
        public ViewFrameGroup ViewFrameGroup => ViewFrameGroup.GetByObjectId(AeccViewFrame.GroupId);

        /// <summary>
        /// Gets if the label is visible for a View Frame.
        /// </summary>
        public bool IsLabelVisible => GetBool();

        /// <summary>
        /// Gets the relative anchor position of the label for a View Frame.
        /// </summary>
        public string LabelAnchorPosition => GetString();

        /// <summary>
        /// Gets the Sheet Set file name for a View Frame.
        /// </summary>
        public string SheetSet => GetString();

        /// <summary>
        /// Gets the raw start station of a View Frame.
        /// </summary>
        public double StartStation => GetDouble();
        #endregion

        #region constructors
        internal ViewFrame(AeccViewFrame aeccViewFrame, bool isDynamoOwned = false) : base(aeccViewFrame, isDynamoOwned) { }

        [SupressImportIntoVM]
        internal static ViewFrame GetByObjectId(acDb.ObjectId viewFrameId)
            => CivilObjectSupport.Get<ViewFrame, AeccViewFrame>
            (viewFrameId, (viewFrame) => new ViewFrame(viewFrame));
        #endregion

        #region methods
        public override string ToString() => $"ViewFrame(Name = {Name})";
        #endregion
    }
}
