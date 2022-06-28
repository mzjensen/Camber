#region references
using System;
using System.Collections.Generic;
using System.Linq;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acRx = Autodesk.AutoCAD.Runtime;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using civDb = Autodesk.Civil.DatabaseServices;
using AeccFolder = Autodesk.Civil.DatabaseServices.Folder;
using Camber.Civil.CivilObjects;
using Camber.Civil.Intersections;
using Autodesk.DesignScript.Runtime;
#endregion

namespace Camber.Civil
{
    public class Folder : acDynNodes.ObjectBase
    {
        #region fields
        internal FolderCategory _category;
        #endregion

        #region properties
        internal AeccFolder AeccFolder => AcObject as AeccFolder;

        /// <summary>
        /// Gets the category of a Folder.
        /// </summary>
        public string Category => _category.ToString();

        /// <summary>
        /// Gets the name of a Folder.
        /// </summary>
        public string Name
        {
            get
            {
                try
                {
                    return AeccFolder.Name;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the parent Folder of a Folder.
        /// Returns null if the Folder has no parent.
        /// </summary>
        public Folder ParentFolder
        {
            get
            {
                acDynNodes.Document document = acDynNodes.Document.Current;

                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    acDb.ObjectId parentId = AeccFolder.GetParent();

                    if (parentId == acDb.ObjectId.Null)
                    {
                        return null;
                    }

                    AeccFolder parent = (AeccFolder)ctx.Transaction.GetObject(
                        AeccFolder.GetParent(),
                        acDb.OpenMode.ForWrite);

                    try
                    {
                        // The API name property only works for subfolders and alignment root folders
                        string name = parent.Name;
                        if (parent.Name.ToUpper().Contains("ALIGNMENT"))
                        {
                            return null;
                        }
                        return new Folder(parent, _category, false);
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the Civil Objects contained within a Folder.
        /// Returns an empty list for root Folders.
        /// </summary>
        private IList<civDynNodes.CivilObject> CivilObjects
        {
            get
            {
                var civilObjs = new List<civDynNodes.CivilObject>();
                acDynNodes.Document document = acDynNodes.Document.Current;

                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    acDb.BlockTable bt = (acDb.BlockTable)ctx.Transaction.GetObject(
                        document.AcDocument.Database.BlockTableId,
                        acDb.OpenMode.ForRead);
                    acDb.BlockTableRecord btr = (acDb.BlockTableRecord)ctx.Transaction.GetObject(
                        bt[acDb.BlockTableRecord.ModelSpace],
                        acDb.OpenMode.ForRead);

                    foreach (acDb.ObjectId oid in btr)
                    {
                        if (!oid.ObjectClass.DxfName.StartsWith("AECC"))
                        {
                            continue;
                        }

                        civDb.Entity entity = (civDb.Entity)ctx.Transaction.GetObject(
                            oid, 
                            acDb.OpenMode.ForRead);

                        if (entity.FolderId != AeccFolder.ObjectId)
                        {
                            continue;
                        }

                        switch (entity)
                        {
                            case civDb.Alignment alignment:
                                civilObjs.Add(civDynNodes.Selection.AlignmentByName(
                                    entity.Name, 
                                    document));
                                break;
                            case civDb.Network pipeNetwork:
                                civilObjs.Add(PipeNetworks.PipeNetwork.GetByObjectId(oid));
                                break;
                            case civDb.PressurePipeNetwork pressurePipeNetwork:
                                civilObjs.Add(PressureNetworks.PressureNetwork.GetByObjectId(oid));
                                break;
                            case civDb.Corridor corridor:
                                civilObjs.Add(civDynNodes.Selection.CorridorByName(entity.Name, document));
                                break;
                            case civDb.ViewFrameGroup viewFrameGroup:
                                civilObjs.Add(ViewFrameGroup.GetByObjectId(oid));
                                break;
                            case civDb.Surface surface:
                                civilObjs.Add(civDynNodes.Selection.SurfaceByName(entity.Name, document));
                                break;
                            case civDb.Intersection intx:
                                civilObjs.Add(Intersection.GetByObjectId(oid));
                                break;
                        }
                    }
                    return civilObjs;
                }
            }
        }

        /// <summary>
        /// Gets the subfolders contained within a Folder.
        /// Returns an empty list if the Folder is a root Folder.
        /// </summary>
        private IList<Folder> Subfolders
        {
            get
            {
                List<Folder> subFolders = new List<Folder>();
                acDynNodes.Document document = acDynNodes.Document.Current;

                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    foreach(acDb.ObjectId oid in AeccFolder.GetSubFolders())
                    {
                        AeccFolder aeccFolder = (AeccFolder)ctx.Transaction.GetObject(
                            oid,
                            acDb.OpenMode.ForWrite);
                        subFolders.Add(new Folder(aeccFolder, _category, false));
                    }
                }
                return subFolders;
            }
        }
        #endregion

        #region constructors
        internal Folder(
            AeccFolder aeccFolder,
            FolderCategory category,
            bool isDynamoOwned = false)
            : base(aeccFolder, isDynamoOwned)
        {
            _category = category;
        }

        /// <summary>
        /// Creates a new Folder by name and category.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public static Folder ByNameCategory(string name, string category)
        {
            if (!Enum.IsDefined(typeof(FolderCategory), category))
            {
                throw new InvalidOperationException("Invalid category.");
            }

            acDynNodes.Document document = acDynNodes.Document.Current;
            FolderCategory folderCategory = (FolderCategory)Enum.Parse(typeof(FolderCategory), category);

            Folder rootFolder = GetRootFolderByCategory(document, folderCategory);
            try
            {
                acDb.ObjectId oid = rootFolder.AeccFolder.CreateFolder(name);
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    AeccFolder aeccFolder = (AeccFolder)ctx.Transaction.GetObject(
                        oid,
                        acDb.OpenMode.ForWrite);
                    return new Folder(aeccFolder, folderCategory, false);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
        #endregion

        #region public methods
        public override string ToString() => $"Folder(Name = {Name}, Category = {Category})";

        /// <summary>
        /// Gets a list of Folders. Each input can be used as a filter to further refine the list.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="category"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IList<Folder> GetFolders(
            acDynNodes.Document document,
            string category = "",
            string name = "")
        {
            if (!string.IsNullOrEmpty(category) && !Enum.IsDefined(typeof(FolderCategory), category))
            {
                throw new ArgumentException("Invalid Folder category.");
            }

            IList<Folder> rootFolders = GetRootFolders(document);
            List<Folder> subFolders = new List<Folder>();

            foreach (Folder rootFolder in rootFolders)
            {
                foreach (Folder subFolder in rootFolder.Subfolders)
                {
                    subFolders.Add(subFolder);
                }
            }

            IQueryable<Folder> q = subFolders.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                q = q.Where(p => p.Category == category);
            }
            if (!string.IsNullOrEmpty(name))
            {
                q = q.Where(p => p.Name.Contains(name));
            }
            return q.ToList();
        }

        /// <summary>
        /// Adds a new subfolder within a Folder.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Folder AddSubfolder(string name)
        {
            try
            {
                AeccFolder.CreateFolder(name);
                return this;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Gets the contents of a Folder.
        /// </summary>
        /// <returns></returns>
        [MultiReturn(new[] { "CivilObjects", "Subfolders" })]
        public Dictionary<string, object> GetContents()
        {
            return new Dictionary<string, object>
                {
                    { "CivilObjects", this.CivilObjects },
                    { "Subfolders", this.Subfolders }
                };
        }

        /// <summary>
        /// Adds a Civil Object to a Folder.
        /// </summary>
        /// <param name="civilObject"></param>
        /// <returns></returns>
        public Folder AddCivilObject(civDynNodes.CivilObject civilObject)
        {
            try
            {
                AeccFolder.AddEntity(civilObject.InternalObjectId);
                return this;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Deletes an empty Folder.
        /// </summary>
        public void Delete()
        {
            try
            {
                acDynNodes.Document document = acDynNodes.Document.Current;
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    AeccFolder.UpgradeOpen();
                    AeccFolder.DeleteFolder();
                }  
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Renames a Folder.
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        public Folder Rename(string newName)
        {
            try
            {
                AeccFolder.RenameFolder(newName);
                return this;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
        #endregion

        #region private methods
        /// <summary>
        /// Gets all root Folders.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        internal static IList<Folder> GetRootFolders(acDynNodes.Document document)
        {
            List<Folder> folders = new List<Folder>();
            var folderTypes = Enum.GetValues(typeof(FolderCategory));

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                foreach (FolderCategory folderType in folderTypes)
                {
                    if (folderType.ToString().ToUpper().Contains("ALIGNMENT"))
                    {
                        civDb.AlignmentType aeccAlignType = civDb.AlignmentType.Utility;

                        if (folderType != FolderCategory.MiscellaneousAlignment)
                        {
                            aeccAlignType = (civDb.AlignmentType)Enum.Parse(
                                typeof(civDb.AlignmentType),
                                folderType.ToString().Replace("Alignment", ""));
                        }

                        acDb.ObjectId rootAlignFolderId = civDb.FolderUtil.GetAlignmentRootFolder(
                            aeccAlignType,
                            document.AcDocument.Database);
                        AeccFolder rootAlignFolder = (AeccFolder)ctx.Transaction.GetObject(
                            rootAlignFolderId,
                            acDb.OpenMode.ForWrite);
                        folders.Add(new Folder(rootAlignFolder, folderType, false));
                    }
                    else
                    {
                        acDb.ObjectId rootFolderId = civDb.FolderUtil.GetNonAlignmentRootFolder(
                            GetRXClassByCategory(folderType),
                            document.AcDocument.Database);
                        AeccFolder rootFolder = (AeccFolder)ctx.Transaction.GetObject(
                            rootFolderId,
                            acDb.OpenMode.ForWrite);
                        folders.Add(new Folder(rootFolder, folderType, false));
                    }
                }
            }
            return folders;
        }

        /// <summary>
        /// Gets a root Folder by category.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        internal static Folder GetRootFolderByCategory(
            acDynNodes.Document document,
            FolderCategory category)
        {
            IList<Folder> rootFolders = GetRootFolders(document);
            return rootFolders.FirstOrDefault(folder => folder._category == category);
        }

        /// <summary>
        /// Determines the root folder type that an Entity would go in.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal static FolderCategory DetermineRootFolderType(civDb.Entity entity)
        {
            // Initialize enum with a value, but it will always be changed by the method return.
            FolderCategory folderType = FolderCategory.CenterlineAlignment;

            switch (entity)
            {
                case civDb.Alignment alignment:
                    string alignType = alignment.AlignmentType.ToString();
                    switch (alignType)
                    {
                        case "Centerline":
                            folderType = FolderCategory.CenterlineAlignment;
                            break;
                        case "Offset":
                            folderType = FolderCategory.OffsetAlignment;
                            break;
                        case "CurbReturn":
                            folderType = FolderCategory.CurbReturnAlignment;
                            break;
                        case "Utility":
                            folderType = FolderCategory.MiscellaneousAlignment;
                            break;
                        case "Rail":
                            folderType = FolderCategory.RailAlignment;
                            break;
                    }
                    break;
                case civDb.Network pipeNetwork:
                    folderType = FolderCategory.PipeNetwork;
                    break;
                case civDb.PressurePipeNetwork pressurePipeNetwork:
                    folderType = FolderCategory.PressureNetwork;
                    break;
                case civDb.Corridor corridor:
                    folderType = FolderCategory.Corridor;
                    break;
                case civDb.ViewFrameGroup viewFrameGroup:
                    folderType = FolderCategory.ViewFrameGroup;
                    break;
                case civDb.Surface surface:
                    folderType = FolderCategory.Surface;
                    break;
                case civDb.Intersection intx:
                    folderType = FolderCategory.Intersection;
                    break;
            }
            return folderType;
        }

        /// <summary>
        /// Gets the associated RXClass of a Folder category.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        internal static acRx.RXClass GetRXClassByCategory(FolderCategory category)
        {
            acRx.RXClass rxClass = null;
            string categoryString = category.ToString();

            if (categoryString.ToUpper().Contains("ALIGNMENT"))
            {
                rxClass = acRx.RXObject.GetClass(typeof(civDb.Alignment));
            }
            else if (categoryString.ToUpper().Contains("PIPENETWORK"))
            {
                rxClass = acRx.RXObject.GetClass(typeof(civDb.Network));
            }
            else if (categoryString.ToUpper().Contains("PRESSURENETWORK"))
            {
                rxClass = acRx.RXObject.GetClass(typeof(civDb.PressurePipeNetwork));
            }
            else
            {
                Type myType = Type.GetType(
                    "Autodesk.Civil.DatabaseServices." + categoryString + ",AeccDbMgd");
                rxClass = acRx.RXObject.GetClass(myType);
            }
            return rxClass;
        }
        #endregion

        #region enums
        [IsVisibleInDynamoLibrary(false)]
        public enum FolderCategory
        {
            CenterlineAlignment,
            Corridor,
            CurbReturnAlignment,
            OffsetAlignment,
            Intersection,
            PipeNetwork,
            PressureNetwork,
            RailAlignment,
            Surface,
            // The actual type in the API AlignmentType enum is "Utility",
            // but in the UI it is called "Miscellaneous"
            MiscellaneousAlignment,
            ViewFrameGroup
        }
        #endregion
    }
}
