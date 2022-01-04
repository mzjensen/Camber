#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using AeccProfile = Autodesk.Civil.DatabaseServices.Profile;
using DynamoServices;
#endregion

namespace Camber.Civil.CivilObjects
{
    [RegisterForTrace]
    public sealed class Profile
    {
        #region properties
        #endregion

        #region constructors
        private Profile() { }
        #endregion

        #region methods
        /// <summary>
        /// Gets a Dynamo-wrapped Profile by Object ID.
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        internal static civDynNodes.Profile GetFromObjectId(acDb.ObjectId oid)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            try
            {
                using (acDynApp.DocumentContext ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccProfile = ctx.Transaction.GetObject(oid, acDb.OpenMode.ForWrite) as AeccProfile;
                    var parentAlign = Alignment.GetByObjectId(aeccProfile.AlignmentId);
                    return parentAlign.ProfileByName(aeccProfile.Name);
                }
            }
            catch { throw; }
        }
        #endregion
    }
}
