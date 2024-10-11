#region references

using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using AeccPart = Autodesk.Civil.DatabaseServices.Part;
using AeccPipe = Autodesk.Civil.DatabaseServices.Pipe;
using AeccStructure = Autodesk.Civil.DatabaseServices.Structure;
using DynamoServices;
using Camber.Civil.CivilObjects;
using Autodesk.DesignScript.Runtime;
using Camber.Properties;

#endregion

namespace Camber.Civil.PipeNetworks.Parts
{
    [RegisterForTrace]
    public abstract class Part : CivilObject
    {
        #region properties
        internal AeccPart AeccPart => AcObject as AeccPart;

        /// <summary>
        /// Gets whether to use overridden rules for the Part.
        /// </summary>
        public bool OverrideRuleSet => GetBool();

        /// <summary>
        /// Gets the Part Family that the Part belongs to.
        /// </summary>
        public PartFamily PartFamily => PartFamily.GetByObjectId(AeccPart.PartFamilyId);

        /// <summary>
        /// Gets the Part's Rule Set.
        /// </summary>
        public RuleSet RuleSet => RuleSet.GetByObjectId(AeccPart.RuleSetStyleId);
        #endregion

        #region constructors
        internal Part(
            AeccPart aeccPart, 
            bool isDynamoOwned = false) 
            : base(aeccPart, isDynamoOwned) { }
        #endregion

        #region methods
        public override string ToString() => $"Part(Name = {Name}, Domain = {Domain})";

        /// <summary>
        /// Adds the Part to the specified Section View.
        /// </summary>
        /// <param name="sectionView"></param>
        /// <returns></returns>
        public Part AddToSectionView(SectionView sectionView)
        {
            bool openedForWrite = AeccPart.IsWriteEnabled;
            if (!openedForWrite) AeccPart.UpgradeOpen();
            AeccPart.AddToSectionView(sectionView.InternalObjectId);
            if (!openedForWrite) AeccPart.DowngradeOpen();
            return this;
        }

        /// <summary>
        /// Removes the Part from all Profile Views in which it is drawn. 
        /// </summary>
        /// <returns></returns>
        public Part RemoveFromAllProfileViews()
        {
            bool openedForWrite = AeccPart.IsWriteEnabled;
            if (!openedForWrite) AeccPart.UpgradeOpen();
            AeccPart.RemoveFromAllProfileViews();
            if (!openedForWrite) AeccPart.DowngradeOpen();
            return this;
        }

        /// <summary>
        /// Removes the Part from all Section Views in which it is drawn. 
        /// </summary>
        /// <returns></returns>
        public Part RemoveFromAllSectionViews()
        {
            bool openedForWrite = AeccPart.IsWriteEnabled;
            if (!openedForWrite) AeccPart.UpgradeOpen();
            AeccPart.RemoveFromAllSectionViews();
            if (!openedForWrite) AeccPart.DowngradeOpen();
            return this;
        }

        /// <summary>
        /// Removes the Part from a specified Section View in which it is drawn.
        /// </summary>
        /// <param name="sectionView"></param>
        /// <returns></returns>
        public Part RemoveFromSectionView(SectionView sectionView)
        {
            bool openedForWrite = AeccPart.IsWriteEnabled;
            if (!openedForWrite) AeccPart.UpgradeOpen();
            AeccPart.RemoveFromSectionView(sectionView.InternalObjectId);
            if (!openedForWrite) AeccPart.DowngradeOpen();
            return this;
        }

        /// <summary>
        /// Sets whether to use overridden rules for the Part.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public Part SetOverrideRuleSet(bool @bool)
        {
            SetValue(@bool);
            return this;
        }

        /// <summary>
        /// Sets the Part's Rule Set.
        /// </summary>
        /// <param name="ruleSet"></param>
        /// <returns></returns>
        public Part SetRuleSet(RuleSet ruleSet)
        {
            SetValue(ruleSet.InternalObjectId, "RuleSetStyleId");
            return this;
        }
        #endregion

        #region obsolete
        /// <summary>
        /// Adds the Part to the specified Profile View.
        /// </summary>
        /// <param name="profileView"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Part.AddToProfileView",
            "Autodesk.Civil.DynamoNodes.Part.AddToProfileView")]
        public Part AddToProfileView(ProfileView profileView)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_OBSOLETE_MIGRATION_MESSAGE, "Part.AddToProfileView"));

            bool openedForWrite = AeccPart.IsWriteEnabled;
            if (!openedForWrite) AeccPart.UpgradeOpen();
            AeccPart.AddToProfileView(profileView.InternalObjectId);
            if (!openedForWrite) AeccPart.DowngradeOpen();
            return this;
        }

        /// <summary>
        /// Applies the rules that are assigned to a Pipe Network Part.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Part.ApplyRules",
            "Autodesk.Civil.DynamoNodes.Part.ApplyRules")]
        public Part ApplyRules()
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_OBSOLETE_MIGRATION_MESSAGE, "Part.ApplyRules"));

            bool openedForWrite = AeccPart.IsWriteEnabled;
            if (!openedForWrite) AeccPart.UpgradeOpen();
            AeccPart.ApplyRules();
            if (!openedForWrite) AeccPart.DowngradeOpen();
            return this;
        }

        /// <summary>
        /// Gets the domain of the Part (Pipe or Structure).
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Part.Domain",
            "Autodesk.Civil.DynamoNodes.Part.Domain")]
        public string Domain
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_OBSOLETE_MIGRATION_MESSAGE, "Part.Domain"));
                return GetString();
            }
        }

        /// <summary>
        /// Gets the material defined for the Part.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Part.Material",
            "Autodesk.Civil.DynamoNodes.Part.Material")]
        public string Material
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_OBSOLETE_MIGRATION_MESSAGE, "Part.Material"));
                return GetString();
            }
        }

        /// <summary>
        /// Gets the Part's description.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Part.PartDescription",
            "Autodesk.Civil.DynamoNodes.Part.PartDescription")]
        public string PartDescription
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_OBSOLETE_MIGRATION_MESSAGE, "Part.PartDescription"));
                return GetString();
            }
        }

        /// <summary>
        /// Gets the sub type of the Part.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Part.PartSubType",
            "Autodesk.Civil.DynamoNodes.Part.PartSubType")]
        public string PartSubType
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_OBSOLETE_MIGRATION_MESSAGE, "Part.PartSubType"));
                return GetString();
            }
        }

        /// <summary>
        /// Gets the Pipe Network that the Part belongs to.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Part.PipeNetwork",
            "Autodesk.Civil.DynamoNodes.Part.PipeNetwork")]
        public PipeNetwork PipeNetwork
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_OBSOLETE_MIGRATION_MESSAGE, "Part.PipeNetwork"));
                return PipeNetwork.GetByObjectId(AeccPart.NetworkId);
            }
        }

        /// <summary>
        /// Gets the Profile Views that the Part is displayed in.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Part.ProfileViewsDisplayedIn",
            "Autodesk.Civil.DynamoNodes.Part.ProfileViewsDisplayedIn")]
        public IList<ProfileView> ProfileViewsDisplayedIn
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_OBSOLETE_MIGRATION_MESSAGE, "Part.ProfileViewsDisplayedIn"));

                var views = new List<ProfileView>();
                acDb.ObjectIdCollection viewIds = AeccPart.GetProfileViewsDisplayingMe();
                foreach (acDb.ObjectId oid in viewIds)
                {
                    views.Add(ProfileView.GetByObjectId(oid));
                }
                return views;
            }
        }

        /// <summary>
        /// Gets the Part's reference Alignment.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Part.ReferenceAlignment",
            "Autodesk.Civil.DynamoNodes.Part.ReferenceAlignment")]
        public civDynNodes.Alignment ReferenceAlignment
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_OBSOLETE_MIGRATION_MESSAGE, "Part.ReferenceAlignment"));

                try
                {
                    return civDynNodes.Selection.AlignmentByName(AeccPart.RefAlignmentName, acDynNodes.Document.Current);
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the Part's reference Surface.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Part.ReferenceSurface",
            "Autodesk.Civil.DynamoNodes.Part.ReferenceSurface")]
        public civDynNodes.Surface ReferenceSurface
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_OBSOLETE_MIGRATION_MESSAGE, "Part.ReferenceSurface"));

                try
                {
                    return civDynNodes.Selection.SurfaceByName(AeccPart.RefSurfaceName, acDynNodes.Document.Current);
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Removes the Part from a specified Profile View in which it is drawn.
        /// </summary>
        /// <param name="profileView"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Part.RemoveFromProfileView",
            "Autodesk.Civil.DynamoNodes.Part.RemoveFromProfileView")]
        public Part RemoveFromProfileView(ProfileView profileView)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_OBSOLETE_MIGRATION_MESSAGE, "Part.RemoveFromProfileView"));

            bool openedForWrite = AeccPart.IsWriteEnabled;
            if (!openedForWrite) AeccPart.UpgradeOpen();
            AeccPart.RemoveFromProfileView(profileView.InternalObjectId);
            if (!openedForWrite) AeccPart.DowngradeOpen();
            return this;
        }

        /// <summary>
        /// Gets the Sections Views that the Part is displayed in.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Part.SectionsViewsDisplayedIn",
            "Autodesk.Civil.DynamoNodes.Part.SectionViewsDisplayedIn")]
        public IList<SectionView> SectionsViewsDisplayedIn
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_OBSOLETE_MIGRATION_MESSAGE, "Part.SectionViewsDisplayedIn"));

                var views = new List<SectionView>();
                acDb.ObjectIdCollection viewIds = AeccPart.GetSectionViewsDisplayingMe();
                foreach (acDb.ObjectId oid in viewIds)
                {
                    views.Add(SectionView.GetByObjectId(oid));
                }
                return views;
            }
        }

        /// <summary>
        /// Sets the Part's reference Alignment.
        /// </summary>
        /// <param name="alignment"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Part.SetReferenceAlignment",
            "Autodesk.Civil.DynamoNodes.Part.SetReferenceAlignment")]
        public Part SetReferenceAlignment(civDynNodes.Alignment alignment)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_OBSOLETE_MIGRATION_MESSAGE, "Part.SetReferenceAlignment"));
            SetValue(alignment.InternalObjectId, "RefAlignmentId");
            return this;
        }

        /// <summary>
        /// Sets the Part's reference Surface.
        /// </summary>
        /// <param name="surface"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.Civil.PipeNetworks.Parts.Part.SetReferenceSurface",
            "Autodesk.Civil.DynamoNodes.Part.SetReferenceSurface")]
        public Part SetReferenceSurface(civDynNodes.Surface surface)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_OBSOLETE_MIGRATION_MESSAGE, "Part.SetReferenceSurface"));
            SetValue(surface.InternalObjectId, "RefSurfaceId");
            return this;
        }

        /// <summary>
        /// Swap the Part with a new Part Family and Part Size.
        /// </summary>
        /// <param name="newPartFamily"></param>
        /// <param name="newPartSize"></param>
        /// <returns></returns>
        public Part Swap(PartFamily newPartFamily, PartSize newPartSize)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_OBSOLETE_MIGRATION_MESSAGE, "Pipe.Swap or Structure.Swap"));

            bool openedForWrite = AeccPart.IsWriteEnabled;
            if (!openedForWrite) AeccPart.UpgradeOpen();
            AeccPart.SwapPartFamilyAndSize(newPartFamily.InternalObjectId, newPartSize.InternalObjectId);
            if (!openedForWrite) AeccPart.DowngradeOpen();
            return this;
        }

        /// <summary>
        /// Gets the wall thickness of the Part, measured from the inside edge to the outside edge.
        /// </summary>
        [IsVisibleInDynamoLibrary(false)]
        public double WallThickness
        {
            get
            {
                LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_OBSOLETE_MESSAGE, "Pipe.WallThickness or Structure.WallThickness"));
                return GetDouble();
            }
        }

        #endregion
    }
}
