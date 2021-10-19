#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccPressureAppurtenance = Autodesk.Civil.DatabaseServices.PressureAppurtenance;
using Autodesk.DesignScript.Runtime;
#endregion

namespace Camber.Civil.PressureNetworks.Parts
{
    public sealed class PressureAppurtenance : PressurePart
    {
        #region properties
        internal AeccPressureAppurtenance AeccPressureAppurtenance => AcObject as AeccPressureAppurtenance;
        #endregion

        #region constructors
        internal PressureAppurtenance(AeccPressureAppurtenance aeccPressureAppurtenance, bool isDynamoOwned = false) : base(aeccPressureAppurtenance, isDynamoOwned) { }

        [SupressImportIntoVM]
        internal static PressureAppurtenance GetByObjectId(acDb.ObjectId appurtenanceId)
            => CivilObjectSupport.Get<PressureAppurtenance, AeccPressureAppurtenance>
            (appurtenanceId, (appurtenance) => new PressureAppurtenance(appurtenance));
        #endregion

        #region methods
        public override string ToString() => $"PressureAppurtenance(Name = {Name})";
        #endregion
    }
}
