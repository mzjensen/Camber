#region references
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AcBlock = Autodesk.AutoCAD.DatabaseServices.BlockTableRecord;
#endregion

namespace Camber.AutoCAD.External
{
    public class ExternalBlock : ExternalObjectBase
    {
        #region properties
        protected AcBlock AcBlock => AcObject as AcBlock;

        /// <summary>
        /// Gets the name of an External Block.
        /// </summary>
        public string Name => AcBlock.Name;

        /// <summary>
        /// Gets the description of an External Block.
        /// </summary>
        public string Description => AcBlock.Comments;
        #endregion

        #region constructors
        internal ExternalBlock(AcBlock acBlock) : base(acBlock) { }
        #endregion

        #region methods
        public override string ToString() => $"ExternalBlock(Name = {Name})";

        /// <summary>
        /// Sets the name of an External Block.
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        public ExternalBlock SetName(string newName)
        {
            if (string.IsNullOrEmpty(newName)) { throw new ArgumentException("Name is null or empty."); }
            return SetValue(newName);
        }

        /// <summary>
        /// Sets the description of an External Block.
        /// </summary>
        /// <param name="newDescription"></param>
        /// <returns></returns>
        public ExternalBlock SetDescription(string newDescription)
        {
            if (string.IsNullOrEmpty(newDescription)) { throw new ArgumentException("Description is null or empty."); }
            return SetValue((object)newDescription, "Comments");
        }

        #region helper methods
        protected ExternalBlock SetValue(object value, [CallerMemberName] string methodName = null)
        {
            if (methodName.StartsWith("Set"))
            {
                methodName = methodName.Substring(3);
            }
            return SetValue(methodName, value);
        }

        protected ExternalBlock SetValue(string propertyName, object value)
        {
            acDb.Transaction t = AcDatabase.TransactionManager.StartTransaction();

            using (t)
            {
                try
                {
                    acDb.BlockTable bt = (acDb.BlockTable)t.GetObject(AcDatabase.BlockTableId, acDb.OpenMode.ForRead);
                    AcBlock btr = (AcBlock)t.GetObject(bt[Name], acDb.OpenMode.ForWrite);
                    PropertyInfo propInfo = btr.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                    propInfo?.SetValue(btr, value);
                    return this;
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException(e.Message);
                }
            }
        }
        #endregion
        #endregion
    }
}
