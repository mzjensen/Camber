#region references
using System;
using System.Linq;
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using AeccCatchmentGroup = Autodesk.Civil.DatabaseServices.CatchmentGroup;
using Autodesk.DesignScript.Runtime;
using DynamoServices;
#endregion

namespace Camber.Civil
{
    [RegisterForTrace]
    public sealed class CatchmentGroup : CivilObjectExtensions
    {
        #region properties
        internal AeccCatchmentGroup AeccCatchmentGroup => AcObject as AeccCatchmentGroup;

        /// <summary>
        /// Gets the Catchments in the Catchment Group.
        /// </summary>
        public IList<Catchment> Catchments
        {
            get
            {
                var catchments = new List<Catchment>();
                foreach (acDb.ObjectId id in AeccCatchmentGroup.GetAllCatchmentIds())
                {
                    catchments.Add(Catchment.GetByObjectId(id));
                }
                return catchments;
            }
        }
        #endregion

        #region constructors
        internal CatchmentGroup(AeccCatchmentGroup aeccCatchmentGroup, bool isDynamoOwned = false) : base(aeccCatchmentGroup, isDynamoOwned) { }

        [SupressImportIntoVM]
        internal static CatchmentGroup GetByObjectId(acDb.ObjectId catchmentGroupId)
            => CivilObjectSupport.Get<CatchmentGroup, AeccCatchmentGroup>
            (catchmentGroupId, (catchmentGroup) => new CatchmentGroup(catchmentGroup));

        /// <summary>
        /// Creates a Catchment Group by name. Currently does not support binding 
        /// </summary>
        /// <param name="document"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static CatchmentGroup ByName(acDynNodes.Document document, string name)
        {
            if (document is null)
            {
                throw new ArgumentException("Document cannot be null.");
            }

            CatchmentGroup catchmentGroup;

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                var id = AeccCatchmentGroup.Create(ctx.Database, name);
                catchmentGroup = GetByObjectId(id);
            }
            return catchmentGroup;
        }

        #region BUG: producing fatal error when unplugging name port
        //private static CatchmentGroup ByName(string name)
        //{
        //    var document = acDynNodes.Document.Current;

        //    if (string.IsNullOrEmpty(name))
        //    {
        //        throw new ArgumentException("Name is null or empty.");
        //    }

        //    bool hasCatchmentGroupWithSameName = false;
        //    var groups = GetCatchmentGroups(document);
        //    var res = CommonConstruct<CatchmentGroup, AeccCatchmentGroup>(
        //        document,
        //        (ctx) =>
        //        {
        //            if (groups.Any(obj => obj.Name == name))
        //            {
        //                hasCatchmentGroupWithSameName = true;
        //                return null;
        //            }
        //            var oid = AeccCatchmentGroup.Create(ctx.Database, name);
        //            return ctx.Transaction.GetObject(oid, acDb.OpenMode.ForWrite) as AeccCatchmentGroup;
        //        },
        //        (ctx, group, existing) => 
        //        {
        //            if (existing)
        //            {
        //                if (group.Name != name && !groups.Any(obj => obj.Name == name))
        //                {
        //                    group.Name = name;
        //                }
        //                else if (group.Name != name && groups.Any(obj => obj.Name == name))
        //                {
        //                    hasCatchmentGroupWithSameName = true;
        //                    return false;
        //                }
        //            }
        //            return true;
        //        });
        //    if (hasCatchmentGroupWithSameName)
        //    {
        //        throw new Exception("The document already contains a Catchment Group with the same name.");
        //    }
        //    return res;
        //}
        #endregion
        #endregion

        #region methods
        public override string ToString() => $"CatchmentGroup(Name = {Name})";

        /// <summary>
        /// Gets all Catchment Groups in the document.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static IList<CatchmentGroup> GetCatchmentGroups(acDynNodes.Document document)
        {
            if (document is null)
            {
                throw new ArgumentException("Document cannot be null.");
            }

            var ids = new List<acDb.ObjectId>();
            var retList = new List<CatchmentGroup>();

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                for (long i = 0; i < ctx.Database.Handseed.Value; i++)
                {
                    if (ctx.Database.TryGetObjectId(new acDb.Handle(i), out acDb.ObjectId id))
                    {
                        if (!id.IsValid || id.IsErased || id.IsEffectivelyErased)
                        {
                            continue;
                        }
                        if (id.ObjectClass.Name.Contains("Catchment"))
                        {
                            ids.Add(id);
                        }
                    }
                }
                foreach (acDb.ObjectId id in ids)
                {
                    if (ctx.Transaction.GetObject(id, acDb.OpenMode.ForRead, false, true) is AeccCatchmentGroup catchmentGroup)
                    {
                        retList.Add(new CatchmentGroup(catchmentGroup));
                    }
                }
            }
            return retList;
        }

        /// <summary>
        /// Gets a Catchment Group by name in the document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static CatchmentGroup GetCatchmentGroupByName(acDynNodes.Document document, string name)
        {
            if (document is null) { throw new ArgumentNullException("Document is null."); }
            if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException("Name is null or empty."); }

            return GetCatchmentGroups(document)
                .FirstOrDefault(item => item.Name.Equals
                (name, StringComparison.OrdinalIgnoreCase));
        }
        #endregion        
    }
}
