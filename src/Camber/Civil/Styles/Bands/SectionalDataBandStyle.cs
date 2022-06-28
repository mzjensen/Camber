#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccSectionalDataBandStyle = Autodesk.Civil.DatabaseServices.Styles.SectionalDataBandStyle;
#endregion

namespace Camber.Civil.Styles.Bands
{
    public sealed class SectionalDataBandStyle : BandStyle
    {
        #region properties
        internal AeccSectionalDataBandStyle AeccSectionalDataBandStyle => AcObject as AeccSectionalDataBandStyle;
        #endregion

        #region constructors
        internal SectionalDataBandStyle(AeccSectionalDataBandStyle aeccSectionalDataBandStyle, bool isDynamoOwned = false) 
            : base(aeccSectionalDataBandStyle, isDynamoOwned) { }

        internal static SectionalDataBandStyle GetByObjectId(acDb.ObjectId sectionalDataBandStyleId)
            => StyleSupport.Get<SectionalDataBandStyle, AeccSectionalDataBandStyle>
            (sectionalDataBandStyleId, (sectionalDataBandStyle) => new SectionalDataBandStyle(sectionalDataBandStyle));
        #endregion

        #region methods
        public override string ToString() => $"SectionalDataBandStyle(Name = {Name})";
        #endregion
    }
}
