#region references
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acGeom = Autodesk.AutoCAD.Geometry;
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

        /// <summary>
        /// Creates a new External Block Reference by coordinate system.
        /// </summary>
        /// <param name="sourceBlock"></param>
        /// <param name="cs"></param>
        /// <param name="layer"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        public static ExternalBlockReference ByCoordinateSystem(ExternalBlock sourceBlock, CoordinateSystem cs, string layer, ExternalBlock block)
        {
            // TODO: need to add some way to handle blocks being created from one database to another.
            acDb.Database sourceDb = sourceBlock.AcBlock.Database;
            acDb.Database destDb = block.AcBlock.Database;
            acDb.Transaction t = destDb.TransactionManager.StartTransaction();

            ExternalBlockReference retBlk = null;

            using (t)
            {
                try
                {
                    // Create block and add to destination block table record
                    AcBlockReference bref = new AcBlockReference(new acGeom.Point3d(0, 0, 0), sourceBlock.InternalObjectId);
                    acDb.BlockTableRecord btr = (acDb.BlockTableRecord)t.GetObject(block.InternalObjectId, acDb.OpenMode.ForWrite);
                    btr.AppendEntity(bref);
                    t.AddNewlyCreatedDBObject(bref, true);
                    // Set properties
                    bref.BlockTransform = acDynNodes.AutoCADUtility.CooridnateSystemToMatrix(cs);
                    bref.Layer = layer;
                    retBlk = new ExternalBlockReference(bref);
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException(e.Message);
                }
                t.Commit();
            }
            return retBlk;
        }
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
