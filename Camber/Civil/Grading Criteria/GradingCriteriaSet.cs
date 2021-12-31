#region references
using System;
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acApp = Autodesk.AutoCAD.ApplicationServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civApp = Autodesk.Civil.ApplicationServices;
using AeccGradingCriteriaSet = Autodesk.Civil.DatabaseServices.Styles.GradingCriteriaSet;
using DynamoServices;
using Camber.Civil.Styles;
#endregion

namespace Camber.Civil.GradingCriteria
{
    [RegisterForTrace]
    public sealed class GradingCriteriaSet : Style
    {
        #region properties
        internal AeccGradingCriteriaSet AeccGradingCriteriaSet => AcObject as AeccGradingCriteriaSet;

        /// <summary>
        /// Gets the Grading Criterias in a Grading Criteria Set.
        /// </summary>
        public List<GradingCriteria> GradingCriteria
        {
            get
            {
                List<GradingCriteria> criterias = new List<GradingCriteria>();
                foreach (acDb.ObjectId criteriaId in AeccGradingCriteriaSet.GradingCriteriaIds())
                {
                    criterias.Add(Civil.GradingCriteria.GradingCriteria.GetByObjectId(criteriaId));
                }
                return criterias;
            }
        }
        #endregion

        #region constructors
        internal GradingCriteriaSet(AeccGradingCriteriaSet aeccGradingCriteriaSet, bool isDynamoOwned = false) : base(aeccGradingCriteriaSet, isDynamoOwned) { }

        internal static GradingCriteriaSet GetByObjectId(acDb.ObjectId setId)
            => StyleSupport.Get<GradingCriteriaSet, AeccGradingCriteriaSet>
            (setId, (set) => new GradingCriteriaSet(set));

        /// <summary>
        /// Creates a Grading Criteria Set by name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="checkSetType"></param>
        /// <returns></returns>
        public static GradingCriteriaSet ByName(string name)
        {
            if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException("Name is null or empty."); }

            return (GradingCriteriaSet)CreateByNameType(name, "GradingCriteriaSets", "Camber.Civil.Styles.GradingCriteria.GradingCriteriaSet");
        }
        #endregion

        #region methods
        public override string ToString() => $"GradingCriteriaSet(Name = {Name})";

        /// <summary>
        /// Gets all of the Grading Criteria Sets in the document.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static IList<GradingCriteriaSet> GetGradingCriteriaSets(acDynNodes.Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            var res = new List<GradingCriteriaSet>();
            using (var ctx = new acDynApp.DocumentContext((acApp.Document)null))
            {
                var cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                foreach (acDb.ObjectId oid in cdoc.Styles.GradingCriteriaSets)
                {
                    var obj = (AeccGradingCriteriaSet)oid.GetObject(acDb.OpenMode.ForWrite);
                    if (obj != null)
                    {
                        res.Add(new GradingCriteriaSet(obj, false));
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Adds a new Grading Criteria to a Grading Criteria Set.
        /// </summary>
        /// <param name="criteriaName"></param>
        /// <returns></returns>
        public GradingCriteriaSet AddCriteria(string criteriaName)
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccSet = ctx.Transaction.GetObject(InternalObjectId, acDb.OpenMode.ForWrite);
                    AeccGradingCriteriaSet.AddCriteria(criteriaName);
                    return this;
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// Removes a Grading Criteria by name from a Grading Criteria Set.
        /// </summary>
        /// <param name="criteriaName"></param>
        /// <returns></returns>
        public GradingCriteriaSet RemoveCriteria(string criteriaName)
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccSet = ctx.Transaction.GetObject(InternalObjectId, acDb.OpenMode.ForWrite);
                    AeccGradingCriteriaSet.RemoveCriteria(criteriaName);
                    return this;
                }
            }
            catch { throw; }
        }
        #endregion
    }
}
