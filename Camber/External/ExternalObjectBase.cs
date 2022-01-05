#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using Autodesk.DesignScript.Runtime;
#endregion

namespace Camber.External
{
    [IsVisibleInDynamoLibrary(false)]
    public class ExternalObjectBase
    {
        #region properties
        protected acDb.DBObject AcObject { get; set; }
        protected acDb.ObjectId AcObjectId { get; set; }
        protected acDb.Database AcDatabase { get; set; }
        public acDb.DBObject InternalDBObject { get { return AcObject; } }
        public acDb.ObjectId InternalObjectId { get { return AcObjectId; } }
        #endregion

        #region constructors
        protected ExternalObjectBase(acDb.DBObject obj)
        {
            AcObject = obj;
            AcObjectId = obj.ObjectId;
            AcDatabase = obj.Database;
        }
        #endregion

        #region methods
        #endregion
    }
}
