#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccPipeNetworkBandStyle = Autodesk.Civil.DatabaseServices.Styles.PipeNetworkBandStyle;
#endregion

namespace Camber.Civil.Styles.Bands
{
    public sealed class PipeNetworkBandStyle : BandStyle
    {
        #region properties
        internal AeccPipeNetworkBandStyle AeccPipeNetworkBandStyle => AcObject as AeccPipeNetworkBandStyle;
        #endregion

        #region constructors
        internal PipeNetworkBandStyle(AeccPipeNetworkBandStyle aeccPipeNetworkBandStyle, bool isDynamoOwned = false) 
            : base(aeccPipeNetworkBandStyle, isDynamoOwned) { }

        internal static PipeNetworkBandStyle GetByObjectId(acDb.ObjectId pipeNetworkBandStyleId)
            => StyleSupport.Get<PipeNetworkBandStyle, AeccPipeNetworkBandStyle>
            (pipeNetworkBandStyleId, (pipeNetworkBandStyle) => new PipeNetworkBandStyle(pipeNetworkBandStyle));
        #endregion

        #region methods
        public override string ToString() => $"PipeNetworkBandStyle(Name = {Name})";
        #endregion
    }
}
