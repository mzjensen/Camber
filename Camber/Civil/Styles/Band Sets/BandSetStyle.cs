#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccBandSetStyle = Autodesk.Civil.DatabaseServices.Styles.BandSetStyle;
using Autodesk.DesignScript.Runtime;
#endregion

namespace Camber.Civil.Styles.BandSets
{
    [IsVisibleInDynamoLibrary(false)]
    public class BandSetStyle : Style
    {
        #region properties
        internal AeccBandSetStyle AeccBandSetStyle => AcObject as AeccBandSetStyle;

        /// <summary>
        /// Gets whether the data band major/minor interval distances for Profile/Section Views
        /// matches the Profile/Section View Style's major/minor grid spacing intervals.
        /// </summary>
        public bool MatchIncrementToGridIntervals => GetBool();
        #endregion

        #region constructors
        internal BandSetStyle(AeccBandSetStyle aeccBandSetStyle, bool isDynamoOwned = false) : base(aeccBandSetStyle, isDynamoOwned) { }
        
        internal static BandSetStyle GetByObjectId(acDb.ObjectId bandSetStyleId)
            => StyleSupport.Get<BandSetStyle, AeccBandSetStyle>
            (bandSetStyleId, (bandSetStyle) => new BandSetStyle(bandSetStyle));
        #endregion

        #region methods
        public override string ToString() => $"BandSetStyle(Name = {Name})";

        /// <summary>
        /// Sets whether the data band major/minor interval distances for Profile/Section Views
        /// matches the Profile/Section View Style's major/minor grid spacing intervals.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public BandSetStyle SetMatchIncrementToGridIntervals(bool @bool)
        {
            SetValue(@bool);
            return this;
        }
        #endregion
    }
}
