using Dynamo.Graph.Nodes;
using DynamoServices;
using System;
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using AeccAlignment = Autodesk.Civil.DatabaseServices.Alignment;
using civDynNodes = Autodesk.Civil.DynamoNodes;

namespace Camber.Civil.CivilObjects
{
    [RegisterForTrace]
    public static class Alignment
    {
        /// <summary>
        /// Gets a Dynamo-wrapped Alignment by Object ID.
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        internal static civDynNodes.Alignment GetByObjectId(acDb.ObjectId oid)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            try
            {
                using (acDynApp.DocumentContext ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccAlign = ctx.Transaction.GetObject(oid, acDb.OpenMode.ForWrite) as AeccAlignment;
                    return civDynNodes.Selection.AlignmentByName(aeccAlign.Name, document);
                }
            }
            catch { throw; }
        }

        #region query methods
        /// <summary>
        /// Gets the Profile Views that are based on an Alignment.
        /// </summary>
        /// <param name="alignment"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static IList<ProfileView> ProfileViews(this civDynNodes.Alignment alignment)
        {
            List<ProfileView> profileViews = new List<ProfileView>();
            acDynNodes.Document document = acDynNodes.Document.Current;

            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    AeccAlignment aeccAlign = (AeccAlignment)ctx.Transaction.GetObject(alignment.InternalObjectId, acDb.OpenMode.ForRead);

                    foreach (acDb.ObjectId oid in aeccAlign.GetProfileViewIds())
                    {
                        profileViews.Add(ProfileView.GetByObjectId(oid));
                    }
                    return profileViews;
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            } 
        }

        /// <summary>
        /// Gets the Sample Line Groups that are based on an Alignment.
        /// </summary>
        /// <param name="alignment"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static IList<SampleLineGroup> SampleLineGroups(this civDynNodes.Alignment alignment)
        {
            List<SampleLineGroup> slGroups = new List<SampleLineGroup>();
            acDynNodes.Document document = acDynNodes.Document.Current;

            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    AeccAlignment aeccAlign = (AeccAlignment)ctx.Transaction.GetObject(alignment.InternalObjectId, acDb.OpenMode.ForRead);

                    foreach (acDb.ObjectId oid in aeccAlign.GetSampleLineGroupIds())
                    {
                        slGroups.Add(SampleLineGroup.GetByObjectId(oid));
                    }
                    return slGroups;
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        /// <summary>
        /// Gets if an Alignment is a Connected Alignment.
        /// </summary>
        /// <param name="alignment"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static bool IsConnectedAlignment(this civDynNodes.Alignment alignment)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;

            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    AeccAlignment aeccAlign = (AeccAlignment)ctx.Transaction.GetObject(alignment.InternalObjectId, acDb.OpenMode.ForRead);
                    return aeccAlign.IsConnectedAlignment;
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        /// <summary>
        /// Gets if an Alignment is an Offset Alignment.
        /// </summary>
        /// <param name="alignment"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static bool IsOffsetAlignment(this civDynNodes.Alignment alignment)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;

            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    AeccAlignment aeccAlign = (AeccAlignment)ctx.Transaction.GetObject(alignment.InternalObjectId, acDb.OpenMode.ForRead);
                    return aeccAlign.IsOffsetAlignment;
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }
        #endregion

        #region action methods
        /// <summary>
        /// Imports an Alignment Label Set for an Alignment.
        /// </summary>
        /// <param name="alignment"></param>
        /// <param name="labelSetStyleName">The name of the label set style to import.</param>
        /// <returns></returns>
        public static civDynNodes.Alignment ImportLabelSet(
            this civDynNodes.Alignment alignment,
            string labelSetStyleName)
        {
            if (string.IsNullOrEmpty(labelSetStyleName))
            {
                throw new InvalidOperationException("Label set style name is null or empty.");
            }

            try
            {
                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    var aeccAlign =
                        (AeccAlignment) ctx.Transaction.GetObject(alignment.InternalObjectId, acDb.OpenMode.ForWrite);
                    aeccAlign.ImportLabelSet(labelSetStyleName);
                }

                return alignment;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
        #endregion
    }
}
