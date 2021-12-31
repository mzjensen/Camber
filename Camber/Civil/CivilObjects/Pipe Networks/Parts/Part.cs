#region references
using System;
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using AeccPart = Autodesk.Civil.DatabaseServices.Part;
using AeccPipe = Autodesk.Civil.DatabaseServices.Pipe;
using AeccStructure = Autodesk.Civil.DatabaseServices.Structure;
using DynamoServices;
using Dynamo.Graph.Nodes;
using Camber.Civil.CivilObjects;
#endregion

namespace Camber.Civil.PipeNetworks.Parts
{
    [RegisterForTrace]
    public abstract class Part : CivilObject
    {
        #region properties
        internal AeccPart AeccPart => AcObject as AeccPart;

        /// <summary>
        /// Gets the domain of the Part (Pipe or Structure).
        /// </summary>
        public string Domain => GetString();

        /// <summary>
        /// Gets the material defined for the Part.
        /// </summary>
        public string Material => GetString();

        /// <summary>
        /// Gets the Pipe Network that the Part belongs to.
        /// </summary>
        public PipeNetwork PipeNetwork => PipeNetwork.GetByObjectId(AeccPart.NetworkId);

        /// <summary>
        /// Gets whether to use overridden rules for the Part.
        /// </summary>
        public bool OverrideRuleSet => GetBool();

        /// <summary>
        /// Gets the Part's description.
        /// </summary>
        public string PartDescription => GetString();

        /// <summary>
        /// Gets the Part Family that the Part belongs to.
        /// </summary>
        public PartFamily PartFamily => PartFamily.GetByObjectId(AeccPart.PartFamilyId);

        /// <summary>
        /// Gets the sub type of the Part.
        /// </summary>
        public string PartSubType => GetString();

        /// <summary>
        /// Gets the Profile Views that the Part is displayed in.
        /// </summary>
        public IList<ProfileView> ProfileViewsDisplayedIn
        {
            get
            {
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
        public civDynNodes.Alignment ReferenceAlignment
        {
            get
            {
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
        public civDynNodes.Surface ReferenceSurface
        {
            get
            {
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
        /// Gets the Part's Rule Set.
        /// </summary>
        public RuleSet RuleSet => RuleSet.GetByObjectId(AeccPart.RuleSetStyleId);

        /// <summary>
        /// Gets the Sections Views that the Part is displayed in.
        /// </summary>
        public IList<SectionView> SectionsViewsDisplayedIn
        {
            get
            {
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
        /// Gets the wall thickness of the Part, measured from the inside edge to the outside edge.
        /// </summary>
        public double WallThickness => GetDouble();
        #endregion

        #region constructors
        internal Part(AeccPart aeccPart, bool isDynamoOwned = false) : base(aeccPart, isDynamoOwned) { }

        /// <summary>
        /// Converts a Civil Object to its appropriate Part (Pipe or Structure).
        /// </summary>
        /// <param name="civilObject"></param>
        /// <returns></returns>
        [NodeCategory("Actions")]
        public static Part GetFromCivilObject(civDynNodes.CivilObject civilObject)
        {
            var document = acDynNodes.Document.Current;
            acDb.ObjectId oid = civilObject.InternalObjectId;
            using (acDynApp.DocumentContext ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                var aeccObject = ctx.Transaction.GetObject(oid, acDb.OpenMode.ForRead);
                if (aeccObject is AeccPipe)
                {
                    return Pipe.GetByObjectId(oid);
                }
                else if (aeccObject is AeccStructure)
                {
                    return Structure.GetByObjectId(oid);
                }
                else
                {
                    throw new ArgumentException("Object is not a Part.");
                }
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"Part(Name = {Name}, Domain = {Domain})";

        

        /// <summary>
        /// Adds the Part to the specified Profile View.
        /// </summary>
        /// <param name="profileView"></param>
        /// <returns></returns>
        public Part AddToProfileView(ProfileView profileView)
        {
            bool openedForWrite = AeccPart.IsWriteEnabled;
            if (!openedForWrite) AeccPart.UpgradeOpen();
            AeccPart.AddToProfileView(profileView.InternalObjectId);
            if (!openedForWrite) AeccPart.DowngradeOpen();
            return this;
        }

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
        /// Applies the rules that are assigned to a Pipe Network Part.
        /// </summary>
        public Part ApplyRules()
        {
            bool openedForWrite = AeccPart.IsWriteEnabled;
            if (!openedForWrite) AeccPart.UpgradeOpen();
            AeccPart.ApplyRules();
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
        /// Removes the Part from a specified Profile View in which it is drawn.
        /// </summary>
        /// <param name="profileView"></param>
        /// <returns></returns>
        public Part RemoveFromProfileView(ProfileView profileView)
        {
            bool openedForWrite = AeccPart.IsWriteEnabled;
            if (!openedForWrite) AeccPart.UpgradeOpen();
            AeccPart.RemoveFromProfileView(profileView.InternalObjectId);
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
        /// Swap the Part with a new Part Family and Part Size.
        /// </summary>
        /// <param name="newPartFamily"></param>
        /// <param name="newPartSize"></param>
        /// <returns></returns>
        public Part Swap(PartFamily newPartFamily, PartSize newPartSize)
        {
            bool openedForWrite = AeccPart.IsWriteEnabled;
            if (!openedForWrite) AeccPart.UpgradeOpen();
            AeccPart.SwapPartFamilyAndSize(newPartFamily.InternalObjectId, newPartSize.InternalObjectId);
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
        /// Sets the Part's reference Alignment.
        /// </summary>
        /// <param name="alignment"></param>
        /// <returns></returns>
        public Part SetReferenceAlignment(civDynNodes.Alignment alignment)
        {
            SetValue(alignment.InternalObjectId, "RefAlignmentId");
            return this;
        }

        /// <summary>
        /// Sets the Part's reference Surface.
        /// </summary>
        /// <param name="surface"></param>
        /// <returns></returns>
        public Part SetReferenceSurface(civDynNodes.Surface surface)
        {
            SetValue(surface.InternalObjectId, "RefSurfaceId");
            return this;
        }

        /// <summary>
        /// Sets the Part's Rule Set.
        /// <param name="ruleSet"></param>
        /// <returns></returns>
        public Part SetRuleSet(RuleSet ruleSet)
        {
            SetValue(ruleSet.InternalObjectId, "RuleSetStyleId");
            return this;
        }
        #endregion
    }
}
