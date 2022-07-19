using Camber.External.ExternalObjects;
using Dynamo.Graph.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using AcLayout = Autodesk.AutoCAD.DatabaseServices.Layout;

namespace Camber.External
{
    public sealed class ExternalLayout : ExternalObjectBase
    {
        #region fields
        private const string NameExistsMsg = "An External Layout with the same name already exists.";
        private const string PageSetupNotExistsMsg = "A page setup with that name does not exist.";
        #endregion

        #region properties
        internal AcLayout AcLayout => AcObject as AcLayout;

        /// <summary>
        /// Gets the name of an External Layout.
        /// </summary>
        public string Name => AcLayout.LayoutName;

        /// <summary>
        /// Gets the tab order of an External Layout.
        /// </summary>
        public int TabOrder => AcLayout.TabOrder;

        /// <summary>P
        /// Gets the External Block associated with an External Layout.
        /// </summary>
        public ExternalBlock Block
        {
            get
            {
                using (var tr = AcDatabase.TransactionManager.StartTransaction())
                {
                    try
                    {
                        var btr = (acDb.BlockTableRecord)tr.GetObject(AcLayout.BlockTableRecordId, acDb.OpenMode.ForRead);
                        return new ExternalBlock(btr);
                    }
                    catch (Exception e)
                    {
                        throw new InvalidOperationException(e.Message);
                    }
                }
            }
        }
        #endregion

        #region constructors
        internal ExternalLayout(AcLayout acLayout) : base(acLayout)
        {
        }

        /// <summary>
        /// Creates a new External Layout by name.
        /// </summary>
        /// <param name="externalDocument"></param>
        /// <param name="name">The name for the External Layout.</param>
        /// <param name="pageSetupName">Named page setup as defined in the Page Setup Manager.</param>
        /// <returns></returns>
        public static ExternalLayout ByName(
            ExternalDocument externalDocument, 
            string name,
            string pageSetupName)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new InvalidOperationException("Layout name is null or empty.");
            }

            try
            {
                var db = externalDocument.AcDatabase;
                acDb.HostApplicationServices.WorkingDatabase = db;
                var layoutMgr = acDb.LayoutManager.Current;

                if (layoutMgr.LayoutExists(name))
                {
                    throw new InvalidOperationException(NameExistsMsg);
                }

                using (var tr = db.TransactionManager.StartTransaction())
                {
                    var plSets = (acDb.DBDictionary) tr.GetObject(db.PlotSettingsDictionaryId, acDb.OpenMode.ForRead);

                    if (!plSets.Contains(pageSetupName))
                    {
                        throw new InvalidOperationException(PageSetupNotExistsMsg);
                    }

                    var layId = layoutMgr.CreateLayout(name);
                    var acLayout = (AcLayout) tr.GetObject(layId, acDb.OpenMode.ForWrite);

                    var plSet = (acDb.PlotSettings) plSets.GetAt(pageSetupName).GetObject(acDb.OpenMode.ForRead);

                    acLayout.CopyFrom(plSet);
                    tr.Commit();
                    acDb.HostApplicationServices.WorkingDatabase = acDynNodes.Document.Current.AcDocument.Database;
                    return new ExternalLayout(acLayout);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"{nameof(ExternalLayout)}({nameof(Name)} = {Name})";

        /// <summary>
        /// Sets the name of an External Layout.
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        public ExternalLayout SetName(string newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                throw new InvalidOperationException("Name is null or empty.");
            }

            acDb.HostApplicationServices.WorkingDatabase = AcDatabase;
            var layoutMgr = acDb.LayoutManager.Current;

            if (layoutMgr.LayoutExists(newName))
            {
                throw new InvalidOperationException(NameExistsMsg);
            }

            if (Name.ToUpper() == "MODEL")
            {
                throw new InvalidOperationException("The Model Space layout cannot be renamed.");
            }

            using (var tr = AcDatabase.TransactionManager.StartTransaction())
            {
                var layout = (AcLayout) tr.GetObject(InternalObjectId, acDb.OpenMode.ForWrite);
                layout.LayoutName = newName;
                tr.Commit();
                acDb.HostApplicationServices.WorkingDatabase = acDynNodes.Document.Current.AcDocument.Database;
            }
            return this;
        }
        #endregion
    }
}
