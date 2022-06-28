#region references
using System;
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using AeccAlignment = Autodesk.Civil.DatabaseServices.Alignment;
using DynamoServices;
using Dynamo.Graph.Nodes;
#endregion

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

        /// <summary>
        /// Gets the Profile Views that are based on an Alignment.
        /// </summary>
        /// <param name="alignment"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static IList<ProfileView> ProfileViews(civDynNodes.Alignment alignment)
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
        public static IList<SampleLineGroup> SampleLineGroups(civDynNodes.Alignment alignment)
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
        public static bool IsConnectedAlignment(civDynNodes.Alignment alignment)
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
        public static bool IsOffsetAlignment(civDynNodes.Alignment alignment)
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
    }
}
