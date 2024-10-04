using Autodesk.AutoCAD.Runtime;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DynamoNodes;
using AcBlockReference = Autodesk.AutoCAD.DatabaseServices.BlockReference;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acGeom = Autodesk.AutoCAD.Geometry;
using Exception = System.Exception;

namespace Camber.External.ExternalObjects
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
        public CoordinateSystem CoordinateSystem => AcEntity.BlockTransform.ToDyn();

        /// <summary>
        /// Gets whether an External Block Reference has dynamic properties.
        /// </summary>
        public bool IsDynamic
        {
            get
            {
                // Apparently this property will always return false if not accessed within a transaction.
                using (var tr = AcDatabase.TransactionManager.StartTransaction())
                {
                    var acBlkRef = (AcBlockReference)tr.GetObject(AcEntity.ObjectId, acDb.OpenMode.ForRead);
                    var isDynamic = acBlkRef.IsDynamicBlock;
                    tr.Commit();
                    return isDynamic;
                }
            }
        }

        /// <summary>
        /// Gets whether an External Block Reference has any attributes specified.
        /// </summary>
        public bool HasAttributes => AcEntity.AttributeCollection.Count > 0;

        /// <summary>
        /// Gets all of the attribute tags assigned to an External Block Reference.
        /// </summary>
        public IList<string> AttributeTags
        {
            get
            {
                var tags = new List<string>();

                using (var tr = AcDatabase.TransactionManager.StartTransaction())
                {
                    var attrs = AcEntity.AttributeCollection;
                    
                    if (attrs == null)
                    {
                        return tags;
                    }

                    foreach (acDb.ObjectId oid in attrs)
                    {
                        var attRef = (acDb.AttributeReference) tr.GetObject(oid, acDb.OpenMode.ForRead);
                        if (attRef != null)
                        {
                            tags.Add(attRef.Tag);
                        }
                    }
                    tr.Commit();
                }
                return tags;
            }
        }

        /// <summary>
        /// Gets all of the dynamic property names in an External Block Reference.
        /// </summary>
        public IList<string> DynamicPropertyNames
        {
            get
            {
                using (var tr = AcDatabase.TransactionManager.StartTransaction())
                {
                    List<string> propNames = new List<string>();
                    var acBlkRef = (AcBlockReference) tr.GetObject(AcEntity.ObjectId, acDb.OpenMode.ForRead);
                    var props = acBlkRef.DynamicBlockReferencePropertyCollection;

                    if (props == null)
                    {
                        return propNames;
                    }

                    foreach (acDb.DynamicBlockReferenceProperty prop in props)
                    {
                        if (prop.Show)
                        {
                            propNames.Add(prop.PropertyName);
                        }
                    }
                    tr.Commit();
                    return propNames;
                }
            }
        }
        #endregion

        #region constructors
        [SupressImportIntoVM]
        internal static ExternalBlockReference GetByObjectId(acDb.ObjectId oid)
            => Get<ExternalBlockReference, AcBlockReference>
            (oid, bref => new ExternalBlockReference(bref));

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
                    bref.BlockTransform = cs.ToAc();
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
            (ExternalBlockReference)SetValue(cs.ToAc(), "BlockTransform");

        /// <summary>
        /// Gets the text string of an External Block Reference's attribute value by tag.
        /// </summary>
        /// <param name="tag">The attribute tag.</param>
        /// <returns></returns>
        public string AttributeByTag(string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                throw new InvalidOperationException("Tag is null or empty");
            }

            using (var tr = AcDatabase.TransactionManager.StartTransaction())
            {
                var attrs = AcEntity.AttributeCollection;
                if (attrs == null)
                {
                    return string.Empty;
                }
                foreach (acDb.ObjectId oid in attrs)
                {
                    var attributeReference = (acDb.AttributeReference) tr.GetObject(oid, acDb.OpenMode.ForRead);
                    if (attributeReference != null && attributeReference.Tag == tag)
                    {
                        return attributeReference.TextString;
                    }
                }
                tr.Commit();
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the specified dynamic property by name from an External Block Reference.
        /// </summary>
        /// <param name="name">The dynamic property name.</param>
        /// <returns></returns>
        [MultiReturn("Description", "ReadOnly", "ValueType", "Value", "AllowValues")]
        public Dictionary<string, object> DynamicPropertyByName(string name)
        {
            var dict = new Dictionary<string, object>();

            using (var tr = AcDatabase.TransactionManager.StartTransaction())
            {
                var acBlkRef = (AcBlockReference)tr.GetObject(AcEntity.ObjectId, acDb.OpenMode.ForRead);
                var dynProps = acBlkRef.DynamicBlockReferencePropertyCollection;

                if (dynProps == null)
                {
                    return dict;
                }

                foreach (acDb.DynamicBlockReferenceProperty prop in dynProps)
                {
                    if (prop == null || !prop.Show || prop.PropertyName.ToUpper() != name.ToUpper())
                    {
                        continue;
                    }

                    dict.Add("Description", prop.Description);
                    dict.Add("ReadOnly", prop.ReadOnly);
                    dict.Add("ValueType", prop.Value.GetType().Name);
                    dict.Add("Value", prop.Value);
                    dict.Add("AllowValues", prop.GetAllowedValues());

                    break;
                }

                if (dict.Count == 0)
                {
                    throw new InvalidOperationException("Dynamic property not found.");
                }

                tr.Commit();
            }
            return dict;
        }

        /// <summary>
        /// Sets the text string of an External Block Reference's attribute value with the specified tag.
        ///  If the attribute is not defined, it will be added.
        /// </summary>
        /// <param name="tag">The attribute tag name.</param>
        /// <param name="value">The new value.</param>
        /// <returns></returns>
        public ExternalBlockReference SetAttributeByTag(string tag, string value)
        {
            using (var tr = AcDatabase.TransactionManager.StartTransaction())
            {
                var attrs = AcEntity.AttributeCollection;
                
                // First attempt to set the attribute value if it exists
                foreach (acDb.ObjectId oid in attrs)
                {
                    var att = (acDb.AttributeReference) tr.GetObject(oid, acDb.OpenMode.ForRead);
                    
                    if (att == null || att.Tag.ToUpper() != tag.ToUpper())
                    {
                        continue;
                    }
                    
                    if (att.IsConstant)
                    {
                        throw new InvalidOperationException($"Attribute with tag '{tag}' is constant and cannot be changed.");
                    }

                    if (att.TextString == value)
                    {
                        return this;
                    }

                    att.UpgradeOpen();
                    att.TextString = value;
                    att.DowngradeOpen();
                    
                    return this;
                }
                
                // If we get here, then the attribute is not defined and will be added
                var rxClass = RXObject.GetClass(typeof(acDb.AttributeDefinition));
                foreach (var oid in Block.AcBlock)
                {
                    if (!oid.ObjectClass.Equals(rxClass))
                    {
                        continue;
                    }

                    var attDef = (acDb.AttributeDefinition) tr.GetObject(oid, acDb.OpenMode.ForRead);
                    
                    if (attDef == null || attDef.Tag.ToUpper() != tag.ToUpper())
                    {
                        continue;
                    }
                    
                    if (attDef.Constant)
                    {
                        throw new InvalidOperationException($"Attribute with tag '{tag}' is constant and cannot be changed.");
                    }
                    
                    using (var addAtt = new acDb.AttributeReference())
                    {
                        addAtt.SetAttributeFromBlock(attDef, AcEntity.BlockTransform);
                        addAtt.TextString = value;
                        
                        AcEntity.UpgradeOpen();
                        attrs.AppendAttribute(addAtt);
                        AcEntity.DowngradeOpen();
                        return this;
                    }
                }
                tr.Commit();
            }
            throw new InvalidOperationException($"Attribute with tag '{tag}' is not defined.");
        }
        #endregion
    }
}
