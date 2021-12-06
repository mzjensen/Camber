#region references
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using AcBlockReference = Autodesk.AutoCAD.DatabaseServices.BlockReference;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
#endregion

namespace Camber.AutoCAD.External
{
    public sealed class ExternalBlockReference : ExternalObject
    {
        #region properties
        internal AcBlockReference AcEntity => AcObject as AcBlockReference;
        
        /// <summary>
        /// Gets the External Block referenced by an External Block Reference.
        /// </summary>
        public ExternalBlock Block
        {
            get
            {
                acDb.Transaction t = AcDatabase.TransactionManager.StartTransaction();
                using (t)
                {
                    acDb.BlockTableRecord btr = (acDb.BlockTableRecord)t.GetObject(AcEntity.BlockTableRecord, acDb.OpenMode.ForRead);
                    return new ExternalBlock(btr);
                }
            }
        }

        /// <summary>
        /// Gets the coordinate system of an External Block Reference.
        /// </summary>
        public CoordinateSystem CoordinateSystem => acDynNodes.AutoCADUtility.MatrixToCoordinateSystem(AcEntity.BlockTransform);
        #endregion

        #region constructors
        [SupressImportIntoVM]
        internal static ExternalBlockReference GetByObjectId(acDb.ObjectId oid)
            => Get<ExternalBlockReference, AcBlockReference>
            (oid, (bref) => new ExternalBlockReference(bref));

        internal ExternalBlockReference(AcBlockReference acBlockReference) : base(acBlockReference) { }
        #endregion

        #region methods
        public override string ToString() => $"ExternalBlockReference(Source Block = {Block.Name})";

        /// <summary>
        /// Sets the coordinate system of an External Block Reference. This can be used to set the insertion point, rotation, and scale factors.
        /// </summary>
        /// <param name="cs"></param>
        /// <returns></returns>
        public ExternalBlockReference SetCoordinateSystem(CoordinateSystem cs) => 
            (ExternalBlockReference)SetValue(
            acDynNodes.AutoCADUtility.CooridnateSystemToMatrix(cs),
            "BlockTransform");
        #endregion
    }
}
