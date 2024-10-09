#region references

using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acGeom = Autodesk.AutoCAD.Geometry;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using civDb = Autodesk.Civil.DatabaseServices;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using AeccPressurePart = Autodesk.Civil.DatabaseServices.PressurePart;
using AeccPressurePipe = Autodesk.Civil.DatabaseServices.PressurePipe;
using AeccPressureAppurtenance = Autodesk.Civil.DatabaseServices.PressureAppurtenance;
using AeccPressureFitting = Autodesk.Civil.DatabaseServices.PressureFitting;
using Autodesk.DesignScript.Geometry;
using DynamoServices;
using Camber.Civil.CivilObjects;
using Camber.Utilities.GeometryConversions;
using Autodesk.DesignScript.Runtime;
using Camber.Properties;
#endregion

namespace Camber.Civil.PressureNetworks.Parts
{
    [RegisterForTrace]
    public abstract class PressurePart : CivilObject
    {
        #region properties
        internal AeccPressurePart AeccPressurePart => AcObject as AeccPressurePart;

        /// <summary>
        /// Gets the info for the Pressure Part's connections.
        /// </summary>
        public IList<PressurePartConnection> Connections
        {
            get
            {
                var connections = new List<PressurePartConnection>();
                bool openedForWrite = AeccPressurePart.IsWriteEnabled;
                if (!openedForWrite) AeccPressurePart.UpgradeOpen();
                try
                {
                    for (int i = 0; i < AeccPressurePart.ConnectionCount; i++)
                    {
                        connections.Add(new PressurePartConnection(AeccPressurePart.GetConnectionAt(i)));
                    }
                    return connections;
                }
                catch
                {
                    throw;
                }
            }
        }
        #endregion

        #region constructors
        internal PressurePart(
            AeccPressurePart aeccPressurePart, 
            bool isDynamoOwned = false) 
            : base(aeccPressurePart, isDynamoOwned) { }
        #endregion

        #region methods
        public override string ToString() => $"PressurePart(Name = {Name})";

        /// <summary>
        /// Removes the Pressure Part from all Profile Views in which it is drawn. 
        /// </summary>
        /// <returns></returns>
        public PressurePart RemoveFromAllProfileViews()
        {
            bool openedForWrite = AeccPressurePart.IsWriteEnabled;
            if (!openedForWrite) AeccPressurePart.UpgradeOpen();
            AeccPressurePart.RemoveFromAllProfileViews();
            if (!openedForWrite) AeccPressurePart.DowngradeOpen();
            return this;
        }
        #endregion

        #region deprecated
        /// <summary>
        /// Adds the Pressure Part to the specified Profile View.
        /// </summary>
        /// <param name="profileView"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.Civil.PressureNetworks.Parts.PressurePart.AddToProfileView",
            "Autodesk.Civil.DynamoNodes.PressurePart.AddToProfileView")]
        public PressurePart AddToProfileView(ProfileView profileView)
        {
            LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "PressurePart.AddToProfileView"));

            bool openedForWrite = AeccPressurePart.IsWriteEnabled;
            if (!openedForWrite) AeccPressurePart.UpgradeOpen();
            AeccPressurePart.AddToProfileView(profileView.InternalObjectId);
            if (!openedForWrite) AeccPressurePart.DowngradeOpen();
            return this;
        }

        /// <summary>
        /// Gets the Pressure Part's domain.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PressureNetworks.Parts.PressurePart.Domain",
            "Autodesk.Civil.DynamoNodes.PressurePart.Domain")]
        public string Domain
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "PressurePart.Domain"));
                return GetString("PartDomain");
            }
        }

        /// <summary>
        /// Gets the part data dictionary for a Pressure Part.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PressureNetworks.Parts.PressurePart.PartData",
            "Autodesk.Civil.DynamoNodes.PressurePart.PartData")]
        public Dictionary<string, object> PartData
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "PressurePart.PartData"));

                Dictionary<string, object> propDict = new Dictionary<string, object>();
                civDb.PressureNetworkPartData partData = AeccPressurePart.PartData;
                uint[] propIds = partData.GetAllPropertyIds();
                foreach (uint propId in propIds)
                {
                    civDb.PressurePartProperty prop = partData.GetProperty(propId);
                    propDict.Add(prop.DisplayName, prop.Value);
                }
                return propDict;
            }
        }

        /// <summary>
        /// Gets the Pressure Part's description.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PressureNetworks.Parts.PressurePart.PartDescription",
            "Autodesk.Civil.DynamoNodes.PressurePart.PartDescription")]
        public string PartDescription
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "PressurePart.PartDescription"));
                return GetString();
            }
        }

        /// <summary>
        /// Gets the Pressure Part's type.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PressureNetworks.Parts.PressurePart.PartType",
            "Autodesk.Civil.DynamoNodes.PressurePart.PartType")]
        public string PartType
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "PressurePart.PartType"));
                return GetString();
            }
        }

        /// <summary>
        /// Gets the Pressure Network that the Pressure Part belongs to.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PressureNetworks.Parts.PressurePart.PressureNetwork",
            "Autodesk.Civil.DynamoNodes.PressurePart.PressureNetwork")]
        public PressureNetwork PressureNetwork
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "PressurePart.PressureNetwork"));
                return PressureNetwork.GetByObjectId(AeccPressurePart.NetworkId);
            }
        }

        /// <summary>
        /// Gets the Profile Views that the Pressure Part is displayed in.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PressureNetworks.Parts.PressurePart.ProfileViewsDisplayedIn",
            "Autodesk.Civil.DynamoNodes.PressurePart.ProfileViewsDisplayedIn")]
        public IList<ProfileView> ProfileViewsDisplayedIn
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "PressurePart.ProfileViewsDisplayedIn"));

                var views = new List<ProfileView>();
                acDb.ObjectIdCollection viewIds = AeccPressurePart.GetProfileViewsDisplayingMe();
                foreach (acDb.ObjectId oid in viewIds)
                {
                    views.Add(ProfileView.GetByObjectId(oid));
                }
                return views;
            }
        }

        /// <summary>
        /// Gets the Pressure Part's reference Alignment.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PressureNetworks.Parts.PressurePart.ReferenceAlignment",
            "Autodesk.Civil.DynamoNodes.PressurePart.ReferenceAlignment")]
        public civDynNodes.Alignment ReferenceAlignment
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "PressurePart.ReferenceAlignment"));

                try
                {
                    return civDynNodes.Selection.AlignmentByName(AeccPressurePart.ReferenceAlignmentName, acDynNodes.Document.Current);
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the Pressure Part's reference Surface.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PressureNetworks.Parts.PressurePart.ReferenceSurface",
            "Autodesk.Civil.DynamoNodes.PressurePart.ReferenceSurface")]
        public civDynNodes.Surface ReferenceSurface
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "PressurePart.ReferenceSurface"));

                try
                {
                    return civDynNodes.Selection.SurfaceByName(AeccPressurePart.ReferenceSurfaceName, acDynNodes.Document.Current);
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Removes the Pressure Part from a specified Profile View in which it is drawn.
        /// </summary>
        /// <param name="profileView"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.Civil.PressureNetworks.Parts.PressurePart.RemoveFromProfileView",
            "Autodesk.Civil.DynamoNodes.PressurePart.RemoveFromProfileView")]
        public PressurePart RemoveFromProfileView(ProfileView profileView)
        {
            LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "PressurePart.RemoveFromProfileView"));

            bool openedForWrite = AeccPressurePart.IsWriteEnabled;
            if (!openedForWrite) AeccPressurePart.UpgradeOpen();
            AeccPressurePart.RemoveFromProfileView(profileView.InternalObjectId);
            if (!openedForWrite) AeccPressurePart.DowngradeOpen();
            return this;
        }

        /// <summary>
        /// Sets the position of the Pressure Part.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.Civil.PressureNetworks.Parts.PressurePart.SetPosition",
            "Autodesk.AutoCAD.DynamoNodes.Object.SetLocation")]
        public PressurePart SetPosition(Point point)
        {
            LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Object.SetLocation"));

            bool openedForWrite = AeccPressurePart.IsWriteEnabled;
            if (!openedForWrite) AeccPressurePart.UpgradeOpen();
            AeccPressurePart.Position = (acGeom.Point3d)GeometryConversions.DynPointToAcPoint(point, true);
            if (!openedForWrite) AeccPressurePart.DowngradeOpen();
            return this;
        }

        /// <summary>
        /// Sets the reference Alignment for the Pressure Part.
        /// </summary>
        /// <param name="alignment"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.Civil.PressureNetworks.Parts.PressurePart.SetReferenceAlignment",
            "Autodesk.Civil.DynamoNodes.PressurePart.SetReferenceAlignment")]
        public PressurePart SetReferenceAlignment(civDynNodes.Alignment alignment)
        {
            LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "PressurePart.SetReferenceAlignment"));

            bool openedForWrite = AeccPressurePart.IsWriteEnabled;
            if (!openedForWrite) AeccPressurePart.UpgradeOpen();
            AeccPressurePart.ReferenceAlignmentId = alignment.InternalObjectId;
            if (!openedForWrite) AeccPressurePart.DowngradeOpen();
            return this;
        }

        /// <summary>
        /// Sets the reference Surface for the Pressure Part.
        /// </summary>
        /// <param name="surface"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.Civil.PressureNetworks.Parts.PressurePart.SetReferenceSurface",
            "Autodesk.Civil.DynamoNodes.PressurePart.SetReferenceSurface")]
        public PressurePart SetReferenceSurface(civDynNodes.Surface surface)
        {
            LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "PressurePart.SetReferenceSurface"));

            bool openedForWrite = AeccPressurePart.IsWriteEnabled;
            if (!openedForWrite) AeccPressurePart.UpgradeOpen();
            AeccPressurePart.ReferenceSurfaceId = surface.InternalObjectId;
            if (!openedForWrite) AeccPressurePart.DowngradeOpen();
            return this;
        }

        /// <summary>
        /// Gets the Pressure Part represented as a Dynamo solid.
        /// </summary>
        [NodeMigrationMapping(
            "Camber.Civil.PressureNetworks.Parts.PressurePart.Solid",
            "Autodesk.AutoCAD.DynamoNodes.Object.Geometry")]
        public Solid Solid
        {
            get
            {
                LogWarningMessageEvents.OnLogWarningMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Object.Geometry"));
                return GeometryConversions.AcSolidToDynSolid(AeccPressurePart.Get3dBody());
            }
        }
        #endregion
    }
}
