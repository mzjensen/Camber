using Autodesk.Civil.Settings;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Camber.AutoCAD.Objects;
using Dynamo.Graph.Nodes;
using DynamoServices;
using System;
using System.Collections.Generic;
using Camber.Properties;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using AeccAlignment = Autodesk.Civil.DatabaseServices.Alignment;
using civApp = Autodesk.Civil.ApplicationServices;
using civDb = Autodesk.Civil.DatabaseServices;
using civDynNodes = Autodesk.Civil.DynamoNodes;

namespace Camber.Civil.CivilObjects
{
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

        #region create methods
        /// <summary>
        /// Creates a new Alignment from a Polyline or 3D Polyline. Note that the value for "Add Curves Between Tangents" will be taken from the Civil 3D settings.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="polyline">The Polyline or 3D Polyline to create the Alignment from.</param>
        /// <param name="name">The name of the new Alignment.</param>
        /// <param name="layer"></param>
        /// <param name="style"></param>
        /// <param name="labelSet">The name of the label set style to apply.</param>
        /// <param name="site">The name of the Site to add the Alignment to. The Alignment will be siteless by default if this input is not supplied.</param>
        /// <param name="startStation"></param>
        /// <param name="erasePolyline">If true, the Polyline will be erased after the Alignment is created.</param>
        /// <returns></returns>
        [NodeCategory("Create")]
        public static civDynNodes.Alignment ByPolyline(
            acDynNodes.Document document, 
            acDynNodes.Object polyline,
            string name, 
            string layer, 
            Styles.Objects.AlignmentStyle style,
            string labelSet,
            string site = "",
            double startStation = 0.0,
            bool erasePolyline = false)
        {
            if (!(polyline is acDynNodes.Polyline) && !(polyline is acDynNodes.Polyline3D))
            {
                throw new InvalidOperationException("The input Polyline object must be a Polyline or 3D Polyline.");
            }

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                var cdoc = civApp.CivilDocument.GetCivilDocument(document.AcDocument.Database);
                try
                {
                    var plineOptions = new civDb.PolylineOptions
                    {
                        AddCurvesBetweenTangents = cdoc.Settings.GetSettings<SettingsCmdCreateAlignmentEntities>()
                            .CreateFromEntities.AddCurveBetweenTangents.Value,
                        EraseExistingEntities = erasePolyline,
                        PlineId = polyline.InternalObjectId
                    };
                    acDynNodes.AutoCADUtility.EnsureLayer(ctx, layer);
                    var alignId = AeccAlignment.Create(cdoc, plineOptions, name, site, layer, style.Name, labelSet);
                    var aeccAlign = (AeccAlignment) ctx.Transaction.GetObject(alignId, acDb.OpenMode.ForWrite);
                    aeccAlign.ReferencePointStation = startStation;
                    return GetByObjectId(alignId);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
            }
        }
        #endregion

        #region obsolete
        /// <summary>
        /// Gets the Polycurve geometry of an Alignment. Note that spiral entities will be tessellated.
        /// </summary>
        /// <param name="alignment"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.Alignment.Geometry",
            "Autodesk.Civil.DynamoNodes.Alignment.PolyCurve")]
        [NodeCategory("Query")]
        public static PolyCurve Geometry(this civDynNodes.Alignment alignment)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_OBSOLETE_MIGRATION_MESSAGE, "Alignment.PolyCurve"));
            
            try
            {
                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    var aeccAlign = (AeccAlignment)ctx.Transaction.GetObject(alignment.InternalObjectId, acDb.OpenMode.ForRead);
                    var plineId = aeccAlign.GetPolyline();
                    var acPline = (acDb.Polyline)ctx.Transaction.GetObject(plineId, acDb.OpenMode.ForWrite);
                    var dynPline = (acDynNodes.Polyline)acDynNodes.SelectionByQuery.GetObjectByObjectHandle(plineId.Handle.ToString());
                    dynPline.PruneDuplicateVertices(false);
                    acPline.Erase();
                    return (PolyCurve)dynPline.Geometry;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

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
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_OBSOLETE_MESSAGE, "Alignment.ImportLabelSet"));

            if (string.IsNullOrEmpty(labelSetStyleName))
            {
                throw new InvalidOperationException("Label set style name is null or empty.");
            }

            try
            {
                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    var aeccAlign =
                        (AeccAlignment)ctx.Transaction.GetObject(alignment.InternalObjectId, acDb.OpenMode.ForWrite);
                    aeccAlign.ImportLabelSet(labelSetStyleName);
                }

                return alignment;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Gets the Profile Views that are based on an Alignment.
        /// </summary>
        /// <param name="alignment"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.Alignment.ProfileViews",
            "Autodesk.Civil.DynamoNodes.Alignment.ProfileViews")]
        [NodeCategory("Query")]
        public static IList<ProfileView> ProfileViews(this civDynNodes.Alignment alignment)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_OBSOLETE_MIGRATION_MESSAGE, "Alignment.ProfileViews"));

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
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.Alignment.SampleLineGroups",
            "Autodesk.Civil.DynamoNodes.Alignment.SampleLineGroups")]
        [NodeCategory("Query")]
        public static IList<SampleLineGroup> SampleLineGroups(this civDynNodes.Alignment alignment)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_OBSOLETE_MIGRATION_MESSAGE, "Alignment.SampleLineGroups"));

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
        /// Sets the start station for an Alignment.
        /// </summary>
        /// <param name="alignment"></param>
        /// <param name="startStation"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.Alignment.SetStartStation",
            "Autodesk.Civil.DynamoNodes.Alignment.SetReferencePointStation")]
        public static civDynNodes.Alignment SetStartStation(this civDynNodes.Alignment alignment, double startStation)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_OBSOLETE_MIGRATION_MESSAGE, "Alignment.SetReferencePointStation"));

            if (alignment == null)
            {
                return null;
            }

            try
            {
                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    var aeccAlign = (AeccAlignment)ctx.Transaction.GetObject(
                        alignment.InternalObjectId,
                        acDb.OpenMode.ForWrite);
                    aeccAlign.ReferencePointStation = startStation;
                    return alignment;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
        #endregion
    }
}
