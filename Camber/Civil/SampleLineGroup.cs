using System;
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using civApp = Autodesk.Civil.ApplicationServices;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using AeccSampleLineGroup = Autodesk.Civil.DatabaseServices.SampleLineGroup;
using AeccAlignment = Autodesk.Civil.DatabaseServices.Alignment;
using Autodesk.DesignScript.Runtime;
using DynamoServices;

namespace Camber.Civil
{
    [RegisterForTrace]
    public sealed class SampleLineGroup : CivilObject
    {
        #region properties
        internal AeccSampleLineGroup aeccSampleLineGroup => AcObject as AeccSampleLineGroup;

        /// <summary>
        /// Gets all the Sample Lines in the Sample Line Group.
        /// </summary>
        public IList<SampleLine> SampleLines
        {
            get
            {
                List<SampleLine> sampleLines = new List<SampleLine>();
                foreach (acDb.ObjectId sampleLineId in aeccSampleLineGroup.GetSampleLineIds())
                {
                    sampleLines.Add(SampleLine.GetByObjectId(sampleLineId));
                }
                return sampleLines;
            }
        }

        /// <summary>
        /// Gets the parent Alignment of the Sample Line Group.
        /// </summary>
        public civDynNodes.Alignment Alignment
        {
            get
            {
                acDb.ObjectId alignmentId = aeccSampleLineGroup.ParentAlignmentId;
                AeccAlignment aeccAlignment = (AeccAlignment)alignmentId.GetObject(acDb.OpenMode.ForRead);
                string name = aeccAlignment.Name;
                return civDynNodes.Selection.AlignmentByName(name, acDynNodes.Document.Current);
            }
        }
        #endregion

        #region constructors
        internal SampleLineGroup(AeccSampleLineGroup aeccSampleLineGroup, bool isDynamoOwned = false) : base(aeccSampleLineGroup, isDynamoOwned) { }

        [SupressImportIntoVM]
        internal static SampleLineGroup GetByObjectId(acDb.ObjectId sampleLineGroupId)
            => CivilObjectSupport.Get<SampleLineGroup, AeccSampleLineGroup>
            (sampleLineGroupId, (sampleLineGroup) => new SampleLineGroup(sampleLineGroup));

        /// <summary>
        /// Returns a Sample Line Group by name. A new Sample Line Group will be created if it does not already exist.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="groupName"></param>
        /// <param name="alignment"></param>
        /// <returns></returns>
        public static SampleLineGroup ByAlignment(civDynNodes.Alignment alignment, string groupName)
        {
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
        #endregion

        #region methods
        public override string ToString() => $"SampleLineGroup(Name = {Name}, Alignment = {Alignment.Name})";

        /// <summary>
        /// Gets the Sample Lines near the given station within the specified tolerance.
        /// </summary>
        /// <param name="station"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public IList<SampleLine> GetSampleLinesByStation(double station, double tolerance = 0.001)
        {
            List<SampleLine> sampleLines = new List<SampleLine>();
            acDb.ObjectIdCollection aeccSampleLines = aeccSampleLineGroup.GetSampleLineIds(station, tolerance);
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
        #endregion
    }
}
