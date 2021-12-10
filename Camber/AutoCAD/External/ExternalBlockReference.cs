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
        /// <param name="destinationBlock">The block where the block reference will be created.</param>
        /// <returns></returns>
        public static ExternalBlockReference ByCoordinateSystem(
            ExternalBlock sourceBlock, 
            CoordinateSystem cs, 
            string layer, 
            ExternalBlock destinationBlock)
        {
            if (string.IsNullOrEmpty(layer)) { throw new ArgumentNullException("layer"); }
            
            ExternalBlockReference retBlk = null;

            acDb.Database destDb = destinationBlock.AcBlock.Database;
            ExternalDocument destDoc = new ExternalDocument(destDb, destDb.Filename);

            // Ensure that the block exists in the destination
            ExternalBlock localBlock = ExternalBlock.Import(sourceBlock, destDoc, false);

            using (var tr = destDb.TransactionManager.StartTransaction())
            {
                try
                {
                    // Create block reference and add to destination block table record
                    AcBlockReference bref = new AcBlockReference(new acGeom.Point3d(0, 0, 0), localBlock.InternalObjectId);
                    acDb.BlockTableRecord btr = (acDb.BlockTableRecord)tr.GetObject(destinationBlock.InternalObjectId, acDb.OpenMode.ForWrite);
                    btr.AppendEntity(bref);
                    tr.AddNewlyCreatedDBObject(bref, true);
                    // Set properties
                    bref.BlockTransform = acDynNodes.AutoCADUtility.CooridnateSystemToMatrix(cs);
                    // Ensure destination has input layer
                    ExternalDocument.EnsureLayer(destDoc, layer);
                    bref.Layer = layer;
                    retBlk = new ExternalBlockReference(bref);
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException(e.Message);
                }
                tr.Commit();
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
