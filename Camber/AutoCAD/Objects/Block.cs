#region references
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Reflection;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using Dynamo.Graph.Nodes;
#endregion

namespace Camber.AutoCAD.Objects
{
    public static class Block
    {
        #region properties
        const string NotApplicableMsg = "Not applicable";
        #endregion

        #region methods
        /// <summary>
        /// Gets the Block's Attribute Definitions.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static List<AttributeDefinition> AttributeDefinitions(acDynNodes.Block block)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            var attDefs = new List<AttributeDefinition>();

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                var t = ctx.Transaction;
                var bt = (acDb.BlockTable)t.GetObject(ctx.Database.BlockTableId, acDb.OpenMode.ForRead);
                var btr = (acDb.BlockTableRecord)t.GetObject(bt[block.Name], acDb.OpenMode.ForRead);
                if (btr.HasAttributeDefinitions)
                {
                    foreach (acDb.ObjectId oid in btr)
                    {
                        var obj = oid.GetObject(acDb.OpenMode.ForRead);
                        if (obj is acDb.AttributeDefinition)
                        {
                            attDefs.Add(AttributeDefinition.GetByObjectId(oid));
                        }
                    }
                }
            }
            return attDefs;
        }

        /// <summary>
        /// Gets whether Block References associated with a Block can be exploded. 
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static bool Explodable(acDynNodes.Block block) => GetBool(block);

        /// <summary>
        /// Gets if a Block has a preview icon.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static bool HasPreviewIcon(acDynNodes.Block block) => GetBool(block);

        /// <summary>
        /// Gets if a Block is anonymous.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static bool IsAnonymous(acDynNodes.Block block) => GetBool(block);

        /// <summary>
        /// Gets if a Block is dynamic.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static bool IsDynamic(acDynNodes.Block block) => GetBool(block, "IsDynamicBlock");

        /// <summary>
        /// Gets if a Block represents an external reference.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static bool IsFromExternalReference(acDynNodes.Block block) => GetBool(block);

        /// <summary>
        /// Gets if a Block represents an overlay reference.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static bool IsFromOverlayReference(acDynNodes.Block block) => GetBool(block);

        /// <summary>
        /// Gets if a Block represents a layout.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static bool IsLayout(acDynNodes.Block block) => GetBool(block);


        /// <summary>
        /// Gets if a Block that represents an external reference is currently unloaded.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static bool IsUnloaded(acDynNodes.Block block) => GetBool(block);

        /// <summary>
        /// Gets the units of a Block.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static string Units(acDynNodes.Block block) => GetString(block);


        /// <summary>
        /// Gets the external reference status of a Block.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static string XrefStatus(acDynNodes.Block block) => GetString(block);

        /// <summary>
        /// Gets the anonymous blocks created from a dynamic Block.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static List<acDynNodes.Block> AnonymousBlocks(acDynNodes.Block block)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            var anonBlocks = new List<acDynNodes.Block>();
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                try
                {
                    var bt = (acDb.BlockTable)ctx.Transaction.GetObject(ctx.Database.BlockTableId, acDb.OpenMode.ForRead);
                    var btr = (acDb.BlockTableRecord)ctx.Transaction.GetObject(bt[block.Name], acDb.OpenMode.ForRead);
                    foreach (acDb.ObjectId oid in btr.GetAnonymousBlockIds())
                    {
                        var acAnonBlock = (acDb.BlockTableRecord)ctx.Transaction.GetObject(oid, acDb.OpenMode.ForRead);
                        anonBlocks.Add(document.BlockByName(acAnonBlock.Name));
                    }
                    return anonBlocks;
                }
                catch { throw; }
            }
        }

        /// <summary>
        /// Sets the name of a Block.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static acDynNodes.Block SetName(acDynNodes.Block block, string name)
        {
            try
            {
                return SetValue(block, name);
            }
            catch
            {
                throw new InvalidOperationException("Block cannot be renamed.");
            }
        }


        /// <summary>
        /// Sets the description for a Block.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static acDynNodes.Block SetDescription(acDynNodes.Block block, string description) => SetValue(block, (object)description, "Comments");

        /// <summary>
        /// Sets whether Block References associated with a Block can be exploded.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="bool"></param>
        /// <returns></returns>
        public static acDynNodes.Block SetExplodable(acDynNodes.Block block, bool @bool) => SetValue(block, @bool);

        /// <summary>
        /// Sets if a Block represents an overlay reference.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="bool"></param>
        /// <returns></returns>
        public static acDynNodes.Block SetIsFromOverlayReference(acDynNodes.Block block, bool @bool) => SetValue(block, @bool);

        /// <summary>
        /// Sets if a Block that represents an external reference is currently unloaded.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="bool"></param>
        /// <returns></returns>
        public static acDynNodes.Block SetIsUnloaded(acDynNodes.Block block, bool @bool) => SetValue(block, @bool);

        /// <summary>
        /// Sets the units of a Block.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="units"></param>
        /// <returns></returns>
        public static acDynNodes.Block SetUnits(acDynNodes.Block block, string units)
        {
            if (!Enum.IsDefined(typeof(acDb.UnitsValue), units))
            {
                throw new ArgumentException("Invalid units.");
            }
            return SetValue(block, Enum.Parse(typeof(acDb.UnitsValue), units));
        }

        internal static string GetString(acDynNodes.Block block, [CallerMemberName] string propertyName = null)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                try
                {
                    var bt = (acDb.BlockTable)ctx.Transaction.GetObject(ctx.Database.BlockTableId, acDb.OpenMode.ForRead);
                    var btr = (acDb.BlockTableRecord)ctx.Transaction.GetObject(bt[block.Name], acDb.OpenMode.ForRead);
                    PropertyInfo propInfo = btr.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                    if (propInfo != null)
                    {
                        var value = propInfo.GetValue(btr).ToString();
                        if (string.IsNullOrEmpty(value))
                        {
                            return null;
                        }
                        else
                        {
                            return value;
                        }
                    }
                }
                catch { }
                return NotApplicableMsg;
            }
        }

        internal static bool GetBool(acDynNodes.Block block, [CallerMemberName] string propertyName = null)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                try
                {
                    var bt = (acDb.BlockTable)ctx.Transaction.GetObject(ctx.Database.BlockTableId, acDb.OpenMode.ForRead);
                    var btr = (acDb.BlockTableRecord)ctx.Transaction.GetObject(bt[block.Name], acDb.OpenMode.ForRead);
                    PropertyInfo propInfo = btr.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                    if (propInfo != null)
                    {
                        return (bool)propInfo.GetValue(btr);
                    }
                }
                catch { }
                return false;
            }
        }

        internal static acDynNodes.Block SetValue(acDynNodes.Block block, object value, [CallerMemberName] string methodName = null)
        {
            if (methodName.StartsWith("Set"))
            {
                methodName = methodName.Substring(3);
            }
            return SetValue(block, methodName, value);
        }

        internal static acDynNodes.Block SetValue(acDynNodes.Block block, string propertyName, object value)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                try
                {
                    var bt = (acDb.BlockTable)ctx.Transaction.GetObject(ctx.Database.BlockTableId, acDb.OpenMode.ForWrite);
                    var btr = (acDb.BlockTableRecord)ctx.Transaction.GetObject(bt[block.Name], acDb.OpenMode.ForWrite);
                    PropertyInfo propInfo = btr.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                    propInfo?.SetValue(btr, value);
                    return block;
                }
                catch { throw; }
            }
        }
        #endregion
    }
}
