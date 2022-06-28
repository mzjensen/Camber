#region references
using System;
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acApp = Autodesk.AutoCAD.ApplicationServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civApp = Autodesk.Civil.ApplicationServices;
using civDb = Autodesk.Civil.DatabaseServices;
using DynamoServices;
using AeccProfileDesignCheckSet = Autodesk.Civil.DatabaseServices.Styles.ProfileDesignCheckSet;
using Camber.Civil.Styles;
#endregion

namespace Camber.Civil.DesignChecks
{
    [RegisterForTrace]
    public sealed class ProfileDesignCheckSet : Style
    {
        #region properties
        internal AeccProfileDesignCheckSet AeccProfileDesignCheckSet => AcObject as AeccProfileDesignCheckSet;

        /// <summary>
        /// Gets the Profile Design Checks in a Profile Design Check Set.
        /// </summary>
        public IList<ProfileDesignCheck> DesignChecks
        {
            get
            {
                var checks = new List<ProfileDesignCheck>();
                foreach (var check in AeccProfileDesignCheckSet.GetAllDesignChecks())
                {
                    checks.Add(new ProfileDesignCheck(check));
                }
                return checks;
            }
        }
        #endregion

        #region constructors
        internal ProfileDesignCheckSet(AeccProfileDesignCheckSet aeccProfileDesignCheckSet, bool isDynamoOwned = false)
            : base(aeccProfileDesignCheckSet, isDynamoOwned) { }

        internal static ProfileDesignCheckSet GetByObjectId(acDb.ObjectId profileDesignCheckSetId)
            => StyleSupport.Get<ProfileDesignCheckSet, AeccProfileDesignCheckSet>
            (profileDesignCheckSetId, (profileDesignCheckSet) => new ProfileDesignCheckSet(profileDesignCheckSet));

        /// <summary>
        /// Creates a Profile Design Check Set by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ProfileDesignCheckSet ByName(string name)
        {
            return (ProfileDesignCheckSet)DesignCheckSetByNameType(name, "ProfileDesignCheckSets");
        }
        #endregion

        #region methods
        public override string ToString() => $"ProfileDesignCheckSet(Name = {Name})";

        /// <summary>
        /// Gets all of the Profile Design Check Sets in the document.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static IList<ProfileDesignCheckSet> GetProfileDesignCheckSets(acDynNodes.Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            var res = new List<ProfileDesignCheckSet>();
            using (var ctx = new acDynApp.DocumentContext((acApp.Document)null))
            {
                var cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                foreach (acDb.ObjectId oid in cdoc.Styles.ProfileDesignCheckSets)
                {
                    var obj = (AeccProfileDesignCheckSet)oid.GetObject(acDb.OpenMode.ForWrite);
                    if (obj != null)
                    {
                        res.Add(new ProfileDesignCheckSet(obj, false));
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Gets a curve design check type by name from the Profile Design Check Set.
        /// </summary>
        /// <param name="profileDesignCheckSet"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetCurveDesignCheckType(ProfileDesignCheckSet profileDesignCheckSet, string name)
        {
            try
            {
                return profileDesignCheckSet.AeccProfileDesignCheckSet.GetCurveDesignCheckType(name).ToString();
            }
            catch { throw; }

        }

        /// <summary>
        /// Sets the design check curve type by name and type for a Profile Design Check Set.
        /// </summary>
        /// <param name="curveCheckType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ProfileDesignCheckSet SetCurveDesignCheckType(string curveCheckType, string name)
        {
            if (!Enum.IsDefined(typeof(civDb.Styles.ProfileDesignCheckCurveType), curveCheckType))
            {
                throw new ArgumentException("Invalid curve check type.");
            }

            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccSet = ctx.Transaction.GetObject(InternalObjectId, acDb.OpenMode.ForWrite);
                    AeccProfileDesignCheckSet.SetCurveDesignCheckType(
                        name,
                        (civDb.Styles.ProfileDesignCheckCurveType)
                        Enum.Parse(typeof(civDb.Styles.ProfileDesignCheckCurveType), curveCheckType));
                    return this;
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// Adds a design check to a Profile Design Check Set by name and type.
        /// </summary>
        /// <param name="checkType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ProfileDesignCheckSet AddDesignCheck(string checkType, string name)
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccSet = ctx.Transaction.GetObject(InternalObjectId, acDb.OpenMode.ForWrite);
                    AeccProfileDesignCheckSet.AddDesignCheck(
                        (Autodesk.Civil.ProfileDesignCheckType)Enum.Parse(typeof(Autodesk.Civil.ProfileDesignCheckType), checkType),
                        name);
                    return this;
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// Removes a design check from a Profile Design Check Set by name and type.
        /// </summary>
        /// <param name="checkType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ProfileDesignCheckSet RemoveDesignCheck(string checkType, string name)
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccSet = ctx.Transaction.GetObject(InternalObjectId, acDb.OpenMode.ForWrite);
                    AeccProfileDesignCheckSet.RemoveDesignCheck(
                        (Autodesk.Civil.ProfileDesignCheckType)Enum.Parse(typeof(Autodesk.Civil.ProfileDesignCheckType), checkType),
                        name);
                    return this;
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// Removes all design checks from a Profile Design Check Set.
        /// </summary>
        /// <returns></returns>
        public ProfileDesignCheckSet RemoveAllDesignChecks()
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccSet = ctx.Transaction.GetObject(InternalObjectId, acDb.OpenMode.ForWrite);
                    AeccProfileDesignCheckSet.RemoveAllDesignCheck();
                    return this;
                }
            }
            catch { throw; }
        }
        #endregion
    }
}
