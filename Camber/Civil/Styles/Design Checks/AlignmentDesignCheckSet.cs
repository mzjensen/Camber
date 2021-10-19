#region references
using System;
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acApp = Autodesk.AutoCAD.ApplicationServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civApp = Autodesk.Civil.ApplicationServices;
using DynamoServices;
using AeccAlignmentDesignCheckSet = Autodesk.Civil.DatabaseServices.Styles.AlignmentDesignCheckSet;
#endregion

namespace Camber.Civil.Styles.DesignChecks
{
    [RegisterForTrace]
    public sealed class AlignmentDesignCheckSet : Style
    {
        #region properties
        internal AeccAlignmentDesignCheckSet AeccAlignmentDesignCheckSet => AcObject as AeccAlignmentDesignCheckSet;

        /// <summary>
        /// Gets the Alignment Design Checks in an Alignment Design Check Set.
        /// </summary>
        public IList<AlignmentDesignCheck> DesignChecks
        {
            get
            {
                var checks = new List<AlignmentDesignCheck>();
                foreach (var check in AeccAlignmentDesignCheckSet.GetAllDesignChecks())
                {
                    checks.Add(new AlignmentDesignCheck(check));
                }
                return checks;
            }
        }
        #endregion

        #region constructors
        internal AlignmentDesignCheckSet(AeccAlignmentDesignCheckSet aeccAlignmentDesignCheckSet, bool isDynamoOwned = false) 
            : base(aeccAlignmentDesignCheckSet, isDynamoOwned) { }

        internal static AlignmentDesignCheckSet GetByObjectId(acDb.ObjectId alignmentDesignCheckSetId)
            => StyleSupport.Get<AlignmentDesignCheckSet, AeccAlignmentDesignCheckSet>
            (alignmentDesignCheckSetId, (alignmentDesignCheckSet) => new AlignmentDesignCheckSet(alignmentDesignCheckSet));

        /// <summary>
        /// Creates an Alignment Design Check Set by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static AlignmentDesignCheckSet ByName(string name)
        {
            return (AlignmentDesignCheckSet)DesignCheckSetByNameType(name, "AlignmentDesignCheckSets");
        }
        #endregion

        #region methods
        public override string ToString() => $"AlignmentDesignCheckSet(Name = {Name})";

        /// <summary>
        /// Gets all of the Alignment Design Check Sets in the document.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static IList<AlignmentDesignCheckSet> GetAlignmentDesignCheckSets(acDynNodes.Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            var res = new List<AlignmentDesignCheckSet>();
            using (var ctx = new acDynApp.DocumentContext((acApp.Document)null))
            {
                var cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                foreach (acDb.ObjectId oid in cdoc.Styles.AlignmentDesignCheckSets)
                {
                    var obj = (AeccAlignmentDesignCheckSet)oid.GetObject(acDb.OpenMode.ForWrite);
                    if (obj != null)
                    {
                        res.Add(new AlignmentDesignCheckSet(obj, false));
                    }
                }
            }
            return res;            
        }

        /// <summary>
        /// Adds a design check to an Alignment Design Check Set by name and type.
        /// </summary>
        /// <param name="checkType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public AlignmentDesignCheckSet AddDesignCheck(string checkType, string name)
        {
            if (!Enum.IsDefined(typeof(Autodesk.Civil.AlignmentDesignCheckType), checkType))
            {
                throw new ArgumentException("Invalid check type.");
            }
            
            bool openedForWrite = AeccAlignmentDesignCheckSet.IsWriteEnabled;
            if (!openedForWrite) { AeccAlignmentDesignCheckSet.UpgradeOpen(); }
            
            AeccAlignmentDesignCheckSet.AddDesignCheck(
                (Autodesk.Civil.AlignmentDesignCheckType)Enum.Parse(typeof(Autodesk.Civil.AlignmentDesignCheckType), checkType), 
                name);
            
            if (!openedForWrite) { AeccAlignmentDesignCheckSet.DowngradeOpen(); }
            return this;
        }

        /// <summary>
        /// Removes a design check from an Alignment Design Check Set by name and type.
        /// </summary>
        /// <param name="checkType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public AlignmentDesignCheckSet RemoveDesignCheck(string checkType, string name)
        {
            if (!Enum.IsDefined(typeof(Autodesk.Civil.AlignmentDesignCheckType), checkType))
            {
                throw new ArgumentException("Invalid check type.");
            }

            bool openedForWrite = AeccAlignmentDesignCheckSet.IsWriteEnabled;
            if (!openedForWrite) { AeccAlignmentDesignCheckSet.UpgradeOpen(); }

            AeccAlignmentDesignCheckSet.RemoveDesignCheck(
                (Autodesk.Civil.AlignmentDesignCheckType)Enum.Parse(typeof(Autodesk.Civil.AlignmentDesignCheckType), checkType),
                name);

            if (!openedForWrite) { AeccAlignmentDesignCheckSet.DowngradeOpen(); }
            return this;
        }

        /// <summary>
        /// Removes all design checks from an Alignment Design Check Set.
        /// </summary>
        /// <returns></returns>
        public AlignmentDesignCheckSet RemoveAllDesignChecks()
        {
            bool openedForWrite = AeccAlignmentDesignCheckSet.IsWriteEnabled;
            if (!openedForWrite) { AeccAlignmentDesignCheckSet.UpgradeOpen(); }

            AeccAlignmentDesignCheckSet.RemoveAllDesignCheck();

            if (!openedForWrite) { AeccAlignmentDesignCheckSet.DowngradeOpen(); }
            return this;
        }
        #endregion
    }
}
