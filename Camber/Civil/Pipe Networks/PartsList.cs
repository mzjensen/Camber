#region references
using System;
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acApp = Autodesk.AutoCAD.ApplicationServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civApp = Autodesk.Civil.ApplicationServices;
using AeccPartsList = Autodesk.Civil.DatabaseServices.Styles.PartsList;
using DynamoServices;
using Camber.Civil.Styles;
#endregion

namespace Camber.Civil.PipeNetworks
{
    [RegisterForTrace]
    public sealed class PartsList : Style
    {
        #region properties
        internal AeccPartsList AeccPipeNetworkPartsList => AcObject as AeccPartsList;

        /// <summary>
        /// Gets all Pipe Part Families in the Parts List.
        /// </summary>
        public IList<PartFamily> PipeFamilies
        {
            get
            {
                var families = new List<PartFamily>();
                foreach (acDb.ObjectId id in AeccPipeNetworkPartsList.GetPartFamilyIdsByDomain(Autodesk.Civil.DatabaseServices.DomainType.Pipe))
                {
                    families.Add(PartFamily.GetByObjectId(id));
                }
                return families;
            }
        }


        /// <summary>
        /// Gets all Structure Part Families in the Pipe Network Parts List.
        /// </summary>
        public IList<PartFamily> StructureFamilies
        {
            get
            {
                var families = new List<PartFamily>();
                foreach (acDb.ObjectId id in AeccPipeNetworkPartsList.GetPartFamilyIdsByDomain(Autodesk.Civil.DatabaseServices.DomainType.Structure))
                {
                    families.Add(PartFamily.GetByObjectId(id));
                }
                return families;
            }
        }
        #endregion

        #region constructors
        internal PartsList(AeccPartsList aeccPipeNetworkPartsList, bool isDynamoOwned = false) : base(aeccPipeNetworkPartsList, isDynamoOwned) { }

        internal static PartsList GetByObjectId(acDb.ObjectId partsListId)
            => StyleSupport.Get<PartsList, AeccPartsList>
            (partsListId, (partsList) => new PartsList(partsList));


        /// <summary>
        /// Creates a Parts List by name. Currently does not support element binding.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PartsList ByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name is null or empty.");
            }

            var document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                var cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                if (cdoc.Styles.PartsListSet.Contains(name))
                {
                    throw new Exception("The document already has a Parts List with the same name.");
                }
                var id = cdoc.Styles.PartsListSet.Add(name);
                return GetByObjectId(id);
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"PartsList(Name = {Name})";

        /// <summary>
        /// Gets all of the Parts Lists in the document.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static IList<PartsList> GetPartsLists(acDynNodes.Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            var res = new List<PartsList>();
            using (var ctx = new acDynApp.DocumentContext((acApp.Document)null))
            {
                var cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                foreach (acDb.ObjectId oid in cdoc.Styles.PartsListSet)
                {
                    var obj = (AeccPartsList)oid.GetObject(acDb.OpenMode.ForWrite);
                    if (obj != null)
                    {
                        res.Add(new PartsList(obj, false));
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Remove a Part Family from the Parts List referenced by unique description.
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public PartsList RemovePartFamily(string description)
        {
            try
            {
                bool openedForWrite = AeccPipeNetworkPartsList.IsWriteEnabled;
                if (!openedForWrite) AeccPipeNetworkPartsList.UpgradeOpen();
                AeccPipeNetworkPartsList.RemovePartFamily(description);
                if (!openedForWrite) AeccPipeNetworkPartsList.DowngradeOpen();
                return this;
            }
            catch { throw; }
        }
        #endregion
    }
}
