#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccPressurePipe = Autodesk.Civil.DatabaseServices.PressurePipe;
using Autodesk.DesignScript.Runtime;
using Camber.Civil.CivilObjects;
#endregion

namespace Camber.Civil.PressureNetworks.Parts
{
    public sealed class PressurePipe : PressurePart
    {
        #region properties
        internal AeccPressurePipe AeccPressurePipe => AcObject as AeccPressurePipe;
        #endregion

        #region constructors
        internal PressurePipe(AeccPressurePipe aeccPressurePipe, bool isDynamoOwned = false) : base(aeccPressurePipe, isDynamoOwned) { }

        [SupressImportIntoVM]
        internal static PressurePipe GetByObjectId(acDb.ObjectId pipeId)
            => CivilObjectSupport.Get<PressurePipe, AeccPressurePipe>
            (pipeId, (pipe) => new PressurePipe(pipe));
        #endregion

        #region methods
        public override string ToString() => $"PressurePipe(Name = {Name})";
        #endregion
    }
}
