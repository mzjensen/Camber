﻿using Camber.External.ExternalObjects;
using Camber.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using AcBlock = Autodesk.AutoCAD.DatabaseServices.BlockTableRecord;
using acDb = Autodesk.AutoCAD.DatabaseServices;

namespace Camber.External
{
    public class ExternalBlock : ExternalObjectBase
    {
        #region properties
        internal AcBlock AcBlock => AcObject as AcBlock;

        /// <summary>
        /// Gets the name of an External Block.
        /// </summary>
        public string Name => AcBlock.Name;

        /// <summary>
        /// Gets the description of an External Block.
        /// </summary>
        public string Description => AcBlock.Comments;

        /// <summary>
        /// Gets whether External Block References associated with an External Block can be exploded.
        /// </summary>
        public bool Explodable => AcBlock.Explodable;

        /// <summary>
        /// Gets if an External Block has a preview icon.
        /// </summary>
        public bool HasPreviewIcon => AcBlock.HasPreviewIcon;

        /// <summary>
        /// Gets if an External Block is anonymous.
        /// </summary>
        public bool IsAnonymous => AcBlock.IsAnonymous;

        /// <summary>
        /// Gets if an External Block is dynamic.
        /// </summary>
        public bool IsDynamic => AcBlock.IsDynamicBlock;

        /// <summary>
        /// Gets if an External Block represents an external reference.
        /// </summary>
        public bool IsFromExternalReference => AcBlock.IsFromExternalReference;

        /// <summary>
        /// Gets if an External Block represents an overlay reference.
        /// </summary>
        public bool IsFromOverlayReference => AcBlock.IsFromOverlayReference;

        /// <summary>
        /// Gets if an External Block represents a layout.
        /// </summary>
        public bool IsLayout => AcBlock.IsLayout;

        /// <summary>
        /// Gets if an External Block that represents an external reference is currently unloaded.
        /// </summary>
        public bool IsUnloaded => AcBlock.IsUnloaded;

        /// <summary>
        /// Gets the units of an External Block.
        /// </summary>
        public string Units => AcBlock.Units.ToString();

        /// <summary>
        /// Gets the external refrence status of an External Block.
        /// </summary>
        public string XrefStatus => AcBlock.XrefStatus.ToString();

        /// <summary>
        /// Gets all of the External Objects in an External Block.
        /// </summary>
        public IList<ExternalObject> Objects
        {
            get
            {
                var retList = new List<ExternalObject>();
                using (var tr = AcDatabase.TransactionManager.StartTransaction())
                {
                    acDb.BlockTable bt = (acDb.BlockTable)tr.GetObject(AcDatabase.BlockTableId, acDb.OpenMode.ForRead);
                    AcBlock btr = (AcBlock)tr.GetObject(bt[Name], acDb.OpenMode.ForRead);
                    foreach (var oid in btr)
                    {
                        var acObj = oid.GetObject(acDb.OpenMode.ForRead);

                        if (!(acObj is acDb.Entity))
                        {
                            continue;
                        }
                        
                        var assemblyObjects = ReflectionUtilities.GetEnumerableOfType<ExternalObject>(acObj);
                        if (assemblyObjects != null && assemblyObjects.Any())
                        {
                            // If there are multiple objects, the first one should be the furthest down the inheritance hierarchy (i.e. the most derived)
                            IEnumerable<ExternalObject> externalObjects = assemblyObjects.ToList();
                            retList.Add(externalObjects.First());
                        }
                        else
                        {
                            // If we don't get any instances of a derived class, just create an ExternalObject
                            retList.Add(new ExternalObject((acDb.Entity)acObj));
                        }
                    }
                    return retList;
                }
            }
        }

        /// <summary>
        /// Gets the anonymous blocks create from an External Block that is dynamic.
        /// </summary>
        public IList<ExternalBlock> AnonymousBlocks
        {
            get
            {
                var anonBlocks = new List<ExternalBlock>();
                using (var tr = AcDatabase.TransactionManager.StartTransaction())
                {
                    try
                    {
                        var bt = (acDb.BlockTable)tr.GetObject(AcDatabase.BlockTableId, acDb.OpenMode.ForRead);
                        var btr = (AcBlock)tr.GetObject(bt[Name], acDb.OpenMode.ForRead);
                        foreach (acDb.ObjectId oid in btr.GetAnonymousBlockIds())
                        {
                            var acAnonBlock = (AcBlock)tr.GetObject(oid, acDb.OpenMode.ForRead);
                            anonBlocks.Add(new ExternalBlock(acAnonBlock));
                        }
                        return anonBlocks;
                    }
                    catch { throw; }
                }
            }
        }

        /// <summary>
        /// Gets the External Block References of an External Block.
        /// </summary>
        public IList<ExternalBlockReference> BlockReferences
        {
            get
            {
                List<ExternalBlockReference> brefs = new List<ExternalBlockReference>();
                acDb.ObjectIdCollection brefIds = AcBlock.GetBlockReferenceIds(true, false);
                foreach (acDb.ObjectId oid in brefIds)
                {
                    brefs.Add(ExternalBlockReference.GetByObjectId(oid));
                }
                return brefs;
            }
        }

        /// <summary>
        /// Gets the External Document that an External Block belongs to.
        /// </summary>
        public ExternalDocument ExternalDocument => new ExternalDocument(AcDatabase, AcDatabase.Filename);
        #endregion

        #region constructors
        internal ExternalBlock(AcBlock acBlock) : base(acBlock) { }

        /// <summary>
        /// Imports an External Block from one External Document to another.
        /// If successful, the new External Block is returned in the context of the destination document.
        /// </summary>
        /// <param name="sourceBlock">The block that will be imported.</param>
        /// <param name="destinationDocument">The destination document where the block will be imported to.</param>
        /// <param name="overwrite">Overwrite the block definition if it already exists in the destination document?</param>
        /// <returns></returns>
        public static ExternalBlock Import(ExternalBlock sourceBlock, ExternalDocument destinationDocument, bool overwrite)
        {
            if (sourceBlock.IsAnonymous) { throw new InvalidOperationException("Cannot import anonymous blocks."); }
            if (sourceBlock.IsLayout) { throw new InvalidOperationException("Cannot import layout blocks."); }

            ExternalBlock destBlock = destinationDocument.BlockByName(sourceBlock.Name);

            var overwriteSwitch = acDb.DuplicateRecordCloning.Ignore;
            if (overwrite)
            {
                overwriteSwitch = acDb.DuplicateRecordCloning.Replace;
            }

            if (!overwrite && destBlock != null)
            {
                return destBlock;
            }
            
            try
            {
                acDb.Database destDb = destinationDocument.AcDatabase;
                acDb.Database sourceDb = sourceBlock.AcDatabase;
                using (var tr = sourceDb.TransactionManager.StartTransaction())
                {
                    acDb.IdMapping mapping = new acDb.IdMapping();
                    acDb.ObjectIdCollection ids = new acDb.ObjectIdCollection()
                    {
                        sourceBlock.InternalObjectId
                    };
                    sourceDb.WblockCloneObjects(ids, destDb.BlockTableId, mapping, overwriteSwitch, false);
                    ExternalBlock newBlk = destinationDocument.BlockByName(sourceBlock.Name);
                    if (newBlk != null)
                    {
                        return newBlk;
                    }
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }
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

        /// <summary>
        /// Sets whether External Block References associated with an External Block can be exploded.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public ExternalBlock SetExplodable(bool @bool) => SetValue(@bool);

        /// <summary>
        /// Sets if an External Block represents an overlay reference.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public ExternalBlock SetIsFromOverlayReference(bool @bool) => SetValue(@bool);

        /// <summary>
        /// Sets if a Block that represents an external reference is currently unloaded.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public ExternalBlock SetIsUnloaded(bool @bool) => SetValue(@bool);


        /// <summary>
        /// Sets the units of an External Block.
        /// </summary>
        /// <param name="units"></param>
        /// <returns></returns>
        public ExternalBlock SetUnits(string units)
        {
            if (!Enum.IsDefined(typeof(acDb.UnitsValue), units))
            {
                throw new ArgumentException("Invalid units.");
            }
            return SetValue(Enum.Parse(typeof(acDb.UnitsValue), units));
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
