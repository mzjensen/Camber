#region references
using System;
using System.Linq;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using civDb = Autodesk.Civil.DatabaseServices;
using AeccPartFamily = Autodesk.Civil.DatabaseServices.Styles.PartFamily;
using DynamoServices;
using Camber.Civil.Styles;
#endregion

namespace Camber.Civil.PipeNetworks
{
    [RegisterForTrace]
    public class PartFamily : acDynNodes.ObjectBase
    {
        #region properties
        internal AeccPartFamily AeccPartFamily => AcObject as AeccPartFamily;

        /// <summary>
        /// Gets the bounding shape type of the Part Family. Returns 'Undefined' for Pipe Part Families.
        /// </summary>
        public string BoundingShape => AeccPartFamily.BoundingShape.ToString();

        /// <summary>
        /// Gets the Part Family description.
        /// </summary>
        public string Description => AeccPartFamily.Description;

        /// <summary>
        /// Gets the domain of the Part Family.
        /// </summary>
        public string Domain => AeccPartFamily.Domain.ToString();

        /// <summary>
        /// Gets the GUID of the Part Family.
        /// </summary>
        public string GUID => AeccPartFamily.GUID;

        /// <summary>
        /// Gets the name of the Part Family.
        /// </summary>
        public string Name => AeccPartFamily.Name;

        /// <summary>
        /// Gets the part type of the Part Family.
        /// </summary>
        public string PartType => AeccPartFamily.PartType.ToString();

        /// <summary>
        /// Gets the swept shape of the Part Family. Returns 'Undefined' for Structure Part Families.
        /// </summary>
        public string SweptShape => AeccPartFamily.SweptShape.ToString();
        #endregion

        #region constructors
        internal PartFamily(AeccPartFamily aeccPartFamily, bool isDynamoOwned = false) : base(aeccPartFamily, isDynamoOwned) { }

        internal static PartFamily GetByObjectId(acDb.ObjectId partFamilyId)
            => StyleSupport.Get<PartFamily, AeccPartFamily>
            (partFamilyId, (partFamily) => new PartFamily(partFamily));

        /// <summary>
        /// Adds a Pipe Part Family to a Pipe Network Parts List referenced by unique description within the current active catalog. 
        /// </summary>
        /// <param name="pipeNetworkPartsList"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static PartFamily PipeFamilyByDescription(PartsList pipeNetworkPartsList, string description, string name)
        {
            if (string.IsNullOrEmpty(description))
            {
                throw new Exception("Description is null or empty.");
            }

            var document = acDynNodes.Document.Current;
            bool hasPartFamilyWithSameDescription = false;
            var res = CommonConstruct<PartFamily, AeccPartFamily>(document,
                (ctx) =>
                {
                    if (pipeNetworkPartsList.PipeFamilies.Any(x => x.Description == description))
                    {
                        hasPartFamilyWithSameDescription = true;
                        return null;
                    }
                    bool openedForWrite = pipeNetworkPartsList.AeccPipeNetworkPartsList.IsWriteEnabled;
                    if (!openedForWrite) pipeNetworkPartsList.AeccPipeNetworkPartsList.UpgradeOpen();
                    pipeNetworkPartsList.AeccPipeNetworkPartsList.AddPartFamilyByDescription(civDb.DomainType.Pipe, description);
                    if (!openedForWrite) pipeNetworkPartsList.AeccPipeNetworkPartsList.DowngradeOpen();
                    var id = pipeNetworkPartsList.AeccPipeNetworkPartsList[description];
                    var newList = (AeccPartFamily)ctx.Transaction.GetObject(id, acDb.OpenMode.ForWrite);
                    newList.Name = name;
                    return newList;
                },
                (ctx, partFamily, existing) =>
                {
                    if (existing)
                    {
                        partFamily.Name = name;

                        if (partFamily.Description != description && !pipeNetworkPartsList.PipeFamilies.Any(x => x.Description == description))
                        {
                            bool openedForWrite = pipeNetworkPartsList.AeccPipeNetworkPartsList.IsWriteEnabled;
                            if (!openedForWrite) pipeNetworkPartsList.AeccPipeNetworkPartsList.UpgradeOpen();
                            pipeNetworkPartsList.RemovePartFamily(description);
                            pipeNetworkPartsList.AeccPipeNetworkPartsList.AddPartFamilyByDescription(civDb.DomainType.Pipe, description);
                            if (!openedForWrite) pipeNetworkPartsList.AeccPipeNetworkPartsList.DowngradeOpen();
                        }
                        else if (partFamily.Description != description && pipeNetworkPartsList.PipeFamilies.Any(x => x.Description == description))
                        {
                            hasPartFamilyWithSameDescription = true;
                            return false;
                        }
                    }
                    return true;
                });

            if (hasPartFamilyWithSameDescription)
            {
                throw new Exception("The Pipe Network Parts List already contains a Part Family with the same description.");
            }
            return res;
        }
        #endregion

        #region methods
        public override string ToString() => $"PartFamily(Name = {Name}, Domain = {Domain})";

        /// <summary>
        /// Removes a Part Size from the Part Family.
        /// </summary>
        /// <param name="index">The index of the Part Size.</param>
        /// <returns></returns>
        public PartFamily RemovePartSize(int index)
        {
            try
            {
                bool openedForWrite = AeccPartFamily.IsWriteEnabled;
                if (!openedForWrite) AeccPartFamily.UpgradeOpen();
                AeccPartFamily.RemovePartSize(index);
                if (!openedForWrite) AeccPartFamily.DowngradeOpen();
                return this;
            }
            catch { throw; }
        }

        /// <summary>
        /// Sets the name of the Part Family.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public PartFamily SetName(string name)
        {
            try
            {
                bool openedForWrite = AeccPartFamily.IsWriteEnabled;
                if (!openedForWrite) AeccPartFamily.UpgradeOpen();
                AeccPartFamily.Name = name;
                if (!openedForWrite) AeccPartFamily.DowngradeOpen();
                return this;
            }
            catch { throw; }
        }
        #endregion
    }
}
