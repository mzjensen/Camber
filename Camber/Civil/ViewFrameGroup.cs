#region references
using System;
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civDb = Autodesk.Civil.DatabaseServices;
using civApp = Autodesk.Civil.ApplicationServices;
using AeccViewFrame = Autodesk.Civil.DatabaseServices.ViewFrame;
using AeccViewFrameGroup = Autodesk.Civil.DatabaseServices.ViewFrameGroup;
using Autodesk.DesignScript.Runtime;
using DynamoServices;
using System.Linq;
using Dynamo.Graph.Nodes;
#endregion

namespace Camber.Civil
{
    [RegisterForTrace]
    public sealed class ViewFrameGroup : CivilObject
    {
        #region properties
        internal AeccViewFrameGroup AeccViewFrameGroup => AcObject as AeccViewFrameGroup;
        private const string NameIsNullOrEmptyMessage = "Name is null or empty.";
        private const string DocumentIsNullMessage = "Document is null.";

        /// <summary>
        /// Gets the default relative anchor position of left side Match Line Labels for a View Frame Group.
        /// </summary>
        public string DefaultLeftMatchLineLabelAnchorPosition => GetString();

        /// <summary>
        /// Gets the default relative anchor position of right side Match Line Labels for a View Frame Group.
        /// </summary>
        public string DefaultRightMatchLineLabelAnchorPosition => GetString();

        /// <summary>
        /// Gets the default relative anchor position of View Frame Labels for a View Frame Group.
        /// </summary>
        public string DefaultViewFrameLabelAnchorPosition => GetString();

        /// <summary>
        /// Gets the View Frames in a View Frame Group.
        /// </summary>
        public IList<ViewFrame> ViewFrames
        {
            get
            {
                var viewFrames = new List<ViewFrame>();
                var viewFrameIds = AeccViewFrameGroup.GetViewFrameIds();
                foreach (acDb.ObjectId oid in viewFrameIds)
                {
                    viewFrames.Add(ViewFrame.GetByObjectId(oid));
                }
                return viewFrames;
            }
        }
        #endregion

        #region constructors
        internal ViewFrameGroup(AeccViewFrameGroup aeccViewFrameGroup, bool isDynamoOwned = false) : base(aeccViewFrameGroup, isDynamoOwned) { }

        [SupressImportIntoVM]
        internal static ViewFrameGroup GetByObjectId(acDb.ObjectId viewFrameGroupId)
            => CivilObjectSupport.Get<ViewFrameGroup, AeccViewFrameGroup>
            (viewFrameGroupId, (viewFrameGroup) => new ViewFrameGroup(viewFrameGroup));

        /// <summary>
        /// Gets a View Frame Group by name in the document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="name"></param>
        /// <param name="allowReference">Include data references?</param>
        /// <returns></returns>
        [NodeCategory("Actions")]
        public static ViewFrameGroup GetViewFrameGroupByName(acDynNodes.Document document, string name, bool allowReference = false)
        {
            if (document is null) { throw new ArgumentNullException(DocumentIsNullMessage); }
            if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException(NameIsNullOrEmptyMessage); }

            return GetViewFrameGroups(document, allowReference)
                .FirstOrDefault(item => item.Name.Equals
                (name, StringComparison.OrdinalIgnoreCase));
        }
        #endregion

        #region methods
        public override string ToString() => $"ViewFrameGroup(Name = {Name})";

        /// <summary>
        /// Gets the View Frame Groups in the document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="allowReference">Include data references?</param>
        /// <returns></returns>
        public static IList<ViewFrameGroup> GetViewFrameGroups(acDynNodes.Document document, bool allowReference = false)
        {
            if (document is null)
            {
                throw new ArgumentNullException(DocumentIsNullMessage);
            }

            IList<ViewFrameGroup> viewFrameGroups = new List<ViewFrameGroup>();

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                civApp.CivilDocument cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);

                acDb.Transaction tr = ctx.Transaction;

                using (acDb.ObjectIdCollection groupIds = cdoc.GetViewFrameGroupIds())
                {
                    foreach (acDb.ObjectId oid in groupIds)
                    {
                        if (!oid.IsValid || oid.IsErased || oid.IsEffectivelyErased)
                        {
                            continue;
                        }

                        if (tr.GetObject(oid, acDb.OpenMode.ForRead, false, true) is AeccViewFrameGroup viewFrameGroup)
                        {
                            if (allowReference || (!viewFrameGroup.IsReferenceObject && !viewFrameGroup.IsReferenceSubObject))
                            {
                                viewFrameGroups.Add(new ViewFrameGroup(viewFrameGroup));
                            }
                        }
                    }
                }
            }
            return viewFrameGroups;
        }
        #endregion
    }
}
