#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccRuleSet = Autodesk.Civil.DatabaseServices.Styles.NetworkRuleSetStyle;
using Camber.Civil.Styles;
#endregion

namespace Camber.Civil.PipeNetworks
{
    public class RuleSet : Style
    {
        #region properties
        internal AeccRuleSet AeccRuleSet => AcObject as AeccRuleSet;
        #endregion

        #region constructors
        internal RuleSet(AeccRuleSet aeccRuleSet, bool isDynamoOwned = false) : base(aeccRuleSet, isDynamoOwned) { }
        
        internal static RuleSet GetByObjectId(acDb.ObjectId ruleSetId)
            => StyleSupport.Get<RuleSet, AeccRuleSet>
            (ruleSetId, (ruleSet) => new RuleSet(ruleSet));
        #endregion

        #region methods
        public override string ToString() => $"RuleSet(Name = {Name})";

        /// <summary>
        /// Remove all rules that belong to the Rule Set.
        /// </summary>
        /// <returns></returns>
        public RuleSet RemoveAllRules()
        {
            bool openedForWrite = AeccRuleSet.IsWriteEnabled;
            if (!openedForWrite) AeccRuleSet.UpgradeOpen();
            AeccRuleSet.RemoveAllRules();
            if (!openedForWrite) AeccStyle.DowngradeOpen();
            return this;
        }
        #endregion
    }
}
