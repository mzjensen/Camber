#region references
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Reflection;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acApp = Autodesk.AutoCAD.ApplicationServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using AcBlock = Autodesk.AutoCAD.DatabaseServices.BlockTableRecord;
using Dynamo.Graph.Nodes;

#endregion

namespace Camber.AutoCAD.External
{
    public class ExternalBlock
    {
        #region properties
        internal AcBlock AcBlock { get; private set; }

        /// <summary>
        /// Gets the External Document that an External Block belongs to.
        /// </summary>
        public ExternalDocument ExternalDocument { get; private set; }
        
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
        internal ExternalBlock(AcBlock acBlock, ExternalDocument externalDocument)
        {
            AcBlock = acBlock;
            ExternalDocument = externalDocument;
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
            if (string.IsNullOrEmpty(newName)) { throw new ArgumentException("Input name is null or empty."); }

            try
            {
                if (!AcBlock.IsWriteEnabled) { AcBlock.UpgradeOpen(); }
                AcBlock.Name = newName;
                return this;
            }
            catch { throw; }
        }
        #endregion
    }
}
