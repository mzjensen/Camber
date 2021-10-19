#region references
using System;
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acGeom = Autodesk.AutoCAD.Geometry;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using AeccPressurePart = Autodesk.Civil.DatabaseServices.PressurePart;
using AeccPressurePipe = Autodesk.Civil.DatabaseServices.PressurePipe;
using AeccPressureAppurtenance = Autodesk.Civil.DatabaseServices.PressureAppurtenance;
using AeccPressureFitting = Autodesk.Civil.DatabaseServices.PressureFitting;
using Autodesk.DesignScript.Geometry;
using DynamoServices;
using Dynamo.Graph.Nodes;
using Camber.Utils;
#endregion

namespace Camber.Civil.PressureNetworks.Parts
{
    [RegisterForTrace]
    public abstract class PressurePart : CivilObjectExtensions
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
                    for (int i = 0; i < AeccPressurePart.ConnectionCount - 1; i++)
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

        /// <summary>
        /// Gets the Pressure Part's domain.
        /// </summary>
        public string Domain => GetString("PartDomain");

        /// <summary>
        /// Gets the Pressure Part's description.
        /// </summary>
        public string PartDescription => GetString();

        /// <summary>
        /// Gets the Pressure Part's type.
        /// </summary>
        public string PartType => GetString();

        /// <summary>
        /// Gets the Pressure Network that the Pressure Part belongs to.
        /// </summary>
        public PressureNetwork PressureNetwork => PressureNetwork.GetByObjectId(AeccPressurePart.NetworkId);

        /// <summary>
        /// Gets the Profile Views that the Pressure Part is displayed in.
        /// </summary>
        public IList<ProfileView> ProfileViewsDisplayedIn
        {
            get
            {
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
        public civDynNodes.Alignment ReferenceAlignment
        {
            get
            {
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
        public civDynNodes.Surface ReferenceSurface
        {
            get
            {
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
        #endregion

        #region constructors
        internal PressurePart(AeccPressurePart aeccPressurePart, bool isDynamoOwned = false) : base(aeccPressurePart, isDynamoOwned) { }

        /// <summary>
        /// Converts a Civil Object to its appropriate Pressure Part (Pipe, Fitting, or Appurtenance).
        /// </summary>
        /// <param name="civilObject"></param>
        /// <returns></returns>
        [NodeCategory("Actions")]
        public static PressurePart GetFromCivilObject(civDynNodes.CivilObject civilObject)
        {
            var document = acDynNodes.Document.Current;
            acDb.ObjectId oid = civilObject.InternalObjectId;
            using (acDynApp.DocumentContext ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                var aeccObject = ctx.Transaction.GetObject(oid, acDb.OpenMode.ForRead);
                if (aeccObject is AeccPressurePipe)
                {
                    return PressurePipe.GetByObjectId(oid);
                }
                else if (aeccObject is AeccPressureAppurtenance)
                {
                    return PressureAppurtenance.GetByObjectId(oid);
                }
                else if (aeccObject is AeccPressureFitting)
                {
                    return PressureFitting.GetByObjectId(oid);
                }
                else
                {
                    throw new ArgumentException("Object is not a Pressure Part.");
                }
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"PressurePart(Name = {Name})";

        /// <summary>
        /// Adds the Pressure Part to the specified Profile View.
        /// </summary>
        /// <param name="profileView"></param>
        /// <returns></returns>
        public PressurePart AddToProfileView(ProfileView profileView)
        {
            bool openedForWrite = AeccPressurePart.IsWriteEnabled;
            if (!openedForWrite) AeccPressurePart.UpgradeOpen();
            AeccPressurePart.AddToProfileView(profileView.InternalObjectId);
            if (!openedForWrite) AeccPressurePart.DowngradeOpen();
            return this;
        }


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


        /// <summary>
        /// Removes the Pressure Part from a specified Profile View in which it is drawn.
        /// </summary>
        /// <param name="profileView"></param>
        /// <returns></returns>
        public PressurePart RemoveFromProfileView(ProfileView profileView)
        {
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
        public PressurePart SetPosition(Point point)
        {
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
        public PressurePart SetReferenceAlignment(civDynNodes.Alignment alignment)
        {
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
        public PressurePart SetReferenceSurface(civDynNodes.Surface surface)
        {
            bool openedForWrite = AeccPressurePart.IsWriteEnabled;
            if (!openedForWrite) AeccPressurePart.UpgradeOpen();
            AeccPressurePart.ReferenceSurfaceId = surface.InternalObjectId;
            if (!openedForWrite) AeccPressurePart.DowngradeOpen();
            return this;
        }
        #endregion
    }
}
