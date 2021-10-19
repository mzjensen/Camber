#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccPressureFitting = Autodesk.Civil.DatabaseServices.PressureFitting;
using Autodesk.DesignScript.Runtime;
#endregion

namespace Camber.Civil.PressureNetworks.Parts
{
    public sealed class PressureFitting : PressurePart
    {
        #region properties
        internal AeccPressureFitting AeccPressureFitting => AcObject as AeccPressureFitting;
        #endregion

        #region constructors
        internal PressureFitting(AeccPressureFitting aeccPressureFitting, bool isDynamoOwned = false) : base(aeccPressureFitting, isDynamoOwned) { }

        [SupressImportIntoVM]
        internal static PressureFitting GetByObjectId(acDb.ObjectId fittingId)
            => CivilObjectSupport.Get<PressureFitting, AeccPressureFitting>
            (fittingId, (fitting) => new PressureFitting(fitting));
        #endregion

        #region methods
        public override string ToString() => $"PressureFitting(Name = {Name})";
        #endregion
    }
}
