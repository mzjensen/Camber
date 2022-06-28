#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using AeccPartSize = Autodesk.Civil.DatabaseServices.Styles.PartSize;
using DynamoServices;
using Camber.Civil.Styles;
#endregion

namespace Camber.Civil.PipeNetworks
{
    [RegisterForTrace]
    public class PartSize : acDynNodes.ObjectBase
    {
        #region properties
        internal AeccPartSize AeccPartSize => AcObject as AeccPartSize;

        /// <summary>
        /// Gets the Part Size description.
        /// </summary>
        public string Description => AeccPartSize.Description;

        /// <summary>
        /// Gets the Part Size name.
        /// </summary>
        public string Name => AeccPartSize.Name;
        #endregion

        #region constructors
        internal PartSize(AeccPartSize aeccPartSize, bool isDynamoOwned = false) : base(aeccPartSize, isDynamoOwned) { }

        internal static PartSize GetByObjectId(acDb.ObjectId partSizeId)
            => StyleSupport.Get<PartSize, AeccPartSize>
            (partSizeId, (partSize) => new PartSize(partSize));
        #endregion

        #region methods
        public override string ToString() => $"PartSize(Name = {Name})";


        /// <summary>
        /// Sets the name of the Part Size.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public PartSize SetName(string name)
        {
            try
            {
                bool openedForWrite = AeccPartSize.IsWriteEnabled;
                if (!openedForWrite) AeccPartSize.UpgradeOpen();
                AeccPartSize.Name = name;
                if (!openedForWrite) AeccPartSize.DowngradeOpen();
                return this;
            }
            catch { throw; }
        }


        /// <summary>
        /// Sets the description of the Part Size.
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public PartSize SetDescription(string description)
        {
            try
            {
                bool openedForWrite = AeccPartSize.IsWriteEnabled;
                if (!openedForWrite) AeccPartSize.UpgradeOpen();
                AeccPartSize.Description = description;
                if (!openedForWrite) AeccPartSize.DowngradeOpen();
                return this;
            }
            catch { throw; }
        }
        #endregion
    }
}
