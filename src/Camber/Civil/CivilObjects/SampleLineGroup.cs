using System;
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using civDb = Autodesk.Civil.DatabaseServices;
using civApp = Autodesk.Civil.ApplicationServices;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using AeccSampleLineGroup = Autodesk.Civil.DatabaseServices.SampleLineGroup;
using AeccAlignment = Autodesk.Civil.DatabaseServices.Alignment;
using Autodesk.DesignScript.Runtime;
using Camber.Properties;
using DynamoServices;

namespace Camber.Civil.CivilObjects
{
    [RegisterForTrace]
    public sealed class SampleLineGroup : CivilObject
    {
        #region properties
        internal AeccSampleLineGroup AeccSampleLineGroup => AcObject as AeccSampleLineGroup;
        #endregion

        #region constructors
        internal SampleLineGroup(AeccSampleLineGroup aeccSampleLineGroup, bool isDynamoOwned = false) : base(aeccSampleLineGroup, isDynamoOwned) { }

        [SupressImportIntoVM]
        internal static SampleLineGroup GetByObjectId(acDb.ObjectId sampleLineGroupId)
            => CivilObjectSupport.Get<SampleLineGroup, AeccSampleLineGroup>
            (sampleLineGroupId, (sampleLineGroup) => new SampleLineGroup(sampleLineGroup));
        #endregion

        #region methods
        public override string ToString() => $"SampleLineGroup(Name = {Name})";

        /// <summary>
        /// Gets the Sample Lines near a given station within a specified tolerance.
        /// </summary>
        /// <param name="station"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public IList<SampleLine> GetSampleLinesByStation(double station, double tolerance = 0.001)
        {
            List<SampleLine> sampleLines = new List<SampleLine>();
            acDb.ObjectIdCollection aeccSampleLines = AeccSampleLineGroup.GetSampleLineIds(station, tolerance);
            if (aeccSampleLines.Count > 0)
            {
                foreach (acDb.ObjectId sampleLineId in aeccSampleLines)
                {
                    sampleLines.Add(SampleLine.GetByObjectId(sampleLineId));
                }
                return sampleLines;
            }
            else
            {
                throw new Exception("No Sample Lines found near the given station with the specified tolerance.");
            }
        }

        /// <summary>
        /// Adds labels to all Sample Lines in a Sample Line Group using the default style.
        /// </summary>
        /// <returns></returns>
        public SampleLineGroup AddLabels()
        {
            acDb.ObjectId labelGroupId = civDb.SampleLineLabelGroup.Create(this.InternalObjectId);
            if (labelGroupId == acDb.ObjectId.Null)
            {
                throw new InvalidOperationException("Could not create Sample Line Label Group.");
            }

            return this;
        }
        #endregion

        #region obsolete
        /// <summary>
        /// Gets the parent Alignment of a Sample Line Group.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.SampleLineGroup.Alignment",
            "Autodesk.Civil.DynamoNodes.SampleLineGroup.Alignment")]
        public civDynNodes.Alignment Alignment
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_OBSOLETE_MIGRATION_MESSAGE, "SampleLineGroup.Alignment"));

                acDb.ObjectId alignmentId = AeccSampleLineGroup.ParentAlignmentId;
                AeccAlignment aeccAlignment = (AeccAlignment)alignmentId.GetObject(acDb.OpenMode.ForRead);
                string name = aeccAlignment.Name;
                return civDynNodes.Selection.AlignmentByName(name, acDynNodes.Document.Current);
            }
        }

        /// <summary>
        /// Returns a Sample Line Group by name. A new Sample Line Group will be created if it does not already exist.
        /// </summary>
        /// <param name="alignment"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static SampleLineGroup ByAlignment(civDynNodes.Alignment alignment, string groupName)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_OBSOLETE_MESSAGE, "SampleLineGroup.ByName"));

            if (string.IsNullOrEmpty(groupName))
            {
                throw new ArgumentNullException("Sample Line Group name is null");
            }

            acDynNodes.Document document = acDynNodes.Document.Current;

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                civApp.CivilDocument cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                acDb.ObjectId groupId = acDynApp.ElementBinder.GetObjectIdFromTrace(ctx.Database);

                if (groupId.IsValid && !groupId.IsErased)
                {
                    AeccSampleLineGroup group = (AeccSampleLineGroup)groupId.GetObject(acDb.OpenMode.ForWrite);
                    if (group != null)
                    {
                        if (group.ParentAlignmentId == alignment.InternalObjectId)
                        {
                            // Update properties
                            group.Name = groupName;
                        }
                        else
                        {
                            // If the parent alignment ID has changed, erase the old group and create a new one
                            group.Erase();
                            groupId = AeccSampleLineGroup.Create(groupName, alignment.InternalObjectId);
                        }
                    }
                }
                else
                {
                    // Create new group
                    groupId = AeccSampleLineGroup.Create(groupName, alignment.InternalObjectId);
                }

                var aeccGroup = groupId.GetObject(acDb.OpenMode.ForRead) as AeccSampleLineGroup;
                if (aeccGroup != null)
                {
                    return new SampleLineGroup(aeccGroup, true);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets all the Sample Lines in a Sample Line Group.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.CivilObjects.SampleLineGroup.SampleLines",
            "Autodesk.Civil.DynamoNodes.SampleLineGroup.SampleLines")]
        public IList<SampleLine> SampleLines
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_OBSOLETE_MIGRATION_MESSAGE, "SampleLineGroup.SampleLines"));

                List<SampleLine> sampleLines = new List<SampleLine>();
                foreach (acDb.ObjectId sampleLineId in AeccSampleLineGroup.GetSampleLineIds())
                {
                    sampleLines.Add(SampleLine.GetByObjectId(sampleLineId));
                }
                return sampleLines;
            }
        }
        #endregion
    }
}
