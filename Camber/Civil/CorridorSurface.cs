#region references
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Camber.Utilities.GeometryConversions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acGeom = Autodesk.AutoCAD.Geometry;
using AeccCorridor = Autodesk.Civil.DatabaseServices.Corridor;
using AeccSurface = Autodesk.Civil.DatabaseServices.Surface;
using AeccCorridorSurface = Autodesk.Civil.DatabaseServices.CorridorSurface;
using civDb = Autodesk.Civil.DatabaseServices;
using civDynNodes = Autodesk.Civil.DynamoNodes;
#endregion

namespace Camber.Civil
{
    public sealed class CorridorSurface
    {
        #region properties
        internal AeccCorridorSurface AeccCorridorSurface { get; }

        internal AeccCorridor AeccCorridor { get; }

        internal AeccSurface AeccSurface { get; }

        /// <summary>
        /// Gets if a Corridor Surface is built.
        /// This is the status of the checkbox for the surface in the Corridor's surface settings window.
        /// </summary>
        public bool Build => AeccCorridorSurface.IsBuild;

        /// <summary>
        /// Gets the Corridor that a Corridor Surface is associated with.
        /// </summary>
        public civDynNodes.Corridor Corridor => CivilObjects.Corridor.GetFromObjectId(AeccCorridorSurface.CorridorId);


        /// <summary>
        /// Gets the description of a Corridor Surface.
        /// </summary>
        public string Description => AeccCorridorSurface.Description;

        /// <summary>
        /// Gets the Feature Line codes used to construct a Corridor Surface.
        /// </summary>
        public List<string> FeatureLineCodes => AeccCorridorSurface.FeatureLineCodes().ToList();

        /// <summary>
        /// Gets the name of a Corridor Surface.
        /// </summary>
        public string Name => AeccCorridorSurface.Name;

        /// <summary>
        /// Gets the overhang correction type assigned to a Corridor Surface.
        /// </summary>
        public string OverhangCorrection => AeccCorridorSurface.OverhangCorrection.ToString();

        /// <summary>
        /// Gets the TIN Surface that a Corridor Surface is associated with.
        /// </summary>
        public civDynNodes.Surface Surface => (civDynNodes.Surface)Corridor.SurfaceByName(Name);
        #endregion

        #region constructors
        internal CorridorSurface(AeccCorridorSurface aeccCorridorSurface)
        {
            AeccCorridorSurface = aeccCorridorSurface;
            using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
            {
                AeccCorridor corr = (AeccCorridor)ctx.Transaction.GetObject(
                    aeccCorridorSurface.CorridorId,
                    acDb.OpenMode.ForRead);
                AeccSurface surf = (AeccSurface)ctx.Transaction.GetObject(
                    aeccCorridorSurface.SurfaceId,
                    acDb.OpenMode.ForRead);
                
                AeccCorridor = corr;
                AeccSurface = surf;
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"CorridorSurface(Name = {Name})";

        /// <summary>
        /// Gets the Link code data used to construct a Corridor Surface.
        /// </summary>
        /// <returns></returns>
        [MultiReturn(new[] { "Code Name", "Is Breakline" })]
        public Dictionary<string, object> GetLinkCodeData()
        {
            List<string> names = AeccCorridorSurface.LinkCodes().ToList();
            List<bool> isBreak = new List<bool>();

            foreach (string name in names)
            {
                isBreak.Add(AeccCorridorSurface.IsLinkCodeAsBreakLine(name));
            }

            return new Dictionary<string, object>
            {
                { "Code Name", names },
                { "Is Breakline", isBreak }
            };
        }

        /// <summary>
        /// Adds a Feature Line code to a Corridor Surface.
        /// </summary>
        /// <param name="codeName"></param>
        /// <returns></returns>
        public CorridorSurface AddFeatureLineCode(string codeName)
        {
            try
            {
                AeccCorridor.UpgradeOpen();
                AeccCorridorSurface.AddFeatureLineCode(codeName);
                RebuildAndRegen();
                AeccCorridor.DowngradeOpen();
                return this;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Adds a Link code to a Corridor Surface.
        /// </summary>
        /// <param name="codeName"></param>
        /// <param name="addAsBreakline"></param>
        /// <returns></returns>
        public CorridorSurface AddLinkCode(string codeName, bool addAsBreakline)
        {
            try
            {
                AeccCorridor.UpgradeOpen();
                AeccCorridorSurface.AddLinkCode(codeName, addAsBreakline);
                RebuildAndRegen();
                AeccCorridor.DowngradeOpen();
                return this;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Removes a Feature Line code from a Corridor Surface.
        /// </summary>
        /// <param name="codeName"></param>
        /// <returns></returns>
        public CorridorSurface RemoveFeatureLineCode(string codeName)
        {
            try
            {
                AeccCorridor.UpgradeOpen();
                AeccCorridorSurface.RemoveFeatureLineCode(codeName);
                RebuildAndRegen();
                AeccCorridor.DowngradeOpen();
                return this;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Removes a Link code from a Corridor Surface.
        /// </summary>
        /// <param name="codeName"></param>
        /// <returns></returns>
        public CorridorSurface RemoveLinkCode(string codeName)
        {
            try
            {
                AeccCorridor.UpgradeOpen();
                AeccCorridorSurface.RemoveLinkCode(codeName);
                RebuildAndRegen();
                AeccCorridor.DowngradeOpen();
                return this;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Sets if a Link code should be treated as a breakline or not.
        /// </summary>
        /// <param name="codeName"></param>
        /// <param name="isBreakline"></param>
        /// <returns></returns>
        public CorridorSurface SetLinkCodeAsBreakline(string codeName, bool isBreakline)
        {
            try
            {
                AeccCorridor.UpgradeOpen();
                AeccCorridorSurface.AddLinkCode(codeName, isBreakline);
                RebuildAndRegen();
                AeccCorridor.DowngradeOpen();
                return this;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Adds an empty outside boundary to a Corridor Surface.
        /// </summary>
        /// <param name="boundaryName"></param>
        /// <returns></returns>
        public CorridorSurface AddBoundary(string boundaryName)
        {
            try
            {
                AeccCorridor.UpgradeOpen();
                AeccCorridorSurface.Boundaries.Add(boundaryName);
                AeccCorridor.DowngradeOpen();
                return this;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Adds an outside boundary defined by a Polyline to a Corridor Surface.
        /// </summary>
        /// <param name="boundaryName"></param>
        /// <param name="polyline"></param>
        /// <returns></returns>
        public CorridorSurface AddBoundaryByPolyline(string boundaryName, acDynNodes.Polyline polyline)
        {
            try
            {
                AeccCorridor.UpgradeOpen();
                AeccCorridorSurface.Boundaries.Add(boundaryName, polyline.InternalObjectId);
                RebuildAndRegen();
                AeccCorridor.DowngradeOpen();
                return this;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Adds an outside boundary defined by Points to a Corridor Surface.
        /// </summary>
        /// <param name="boundaryName"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        public CorridorSurface AddBoundaryByPoints(string boundaryName, List<Point> points)
        {
            try
            {
                acGeom.Point3dCollection pointColl = GeometryConversions.DynPointsToAcPointCollection(
                    points, 
                    true) as acGeom.Point3dCollection;

                AeccCorridor.UpgradeOpen();
                AeccCorridorSurface.Boundaries.Add(boundaryName, pointColl);
                RebuildAndRegen();
                AeccCorridor.DowngradeOpen();
                return this;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Adds an outside boundary defined by a Feature Line code to a Corridor Surface.
        /// </summary>
        /// <param name="boundaryName"></param>
        /// <param name="codeName"></param>
        /// <returns></returns>
        public CorridorSurface AddBoundaryByCode(string boundaryName, string codeName)
        {
            try
            {
                AeccCorridor.UpgradeOpen();
                AeccCorridorSurface.Boundaries.Add(boundaryName, codeName);
                RebuildAndRegen();
                AeccCorridor.DowngradeOpen();
                return this;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Adds the Corridor extents as an outer boundary to a Corridor Surface.
        /// </summary>
        /// <param name="boundaryName"></param>
        /// <returns></returns>
        public CorridorSurface AddExtentsBoundary(string boundaryName)
        {
            try
            {
                AeccCorridor.UpgradeOpen();
                AeccCorridorSurface.Boundaries.AddCorridorExtentsBoundary(boundaryName);
                RebuildAndRegen();
                AeccCorridor.DowngradeOpen();
                return this;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Removes a boundary from a Corridor Surface by name.
        /// </summary>
        /// <param name="boundaryName"></param>
        /// <returns></returns>
        public CorridorSurface RemoveBoundary(string boundaryName)
        {
            try
            {
                AeccCorridor.UpgradeOpen();
                AeccCorridorSurface.Boundaries.Remove(boundaryName);
                RebuildAndRegen();
                AeccCorridor.DowngradeOpen();
                return this;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

        }

        /// <summary>
        /// Sets if a Corridor Surface is built.
        /// This is the same as checking or un-checking the surface in the Corridor's surface settings window.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public CorridorSurface SetBuild(bool @bool)
        {
            SetValue(@bool, "IsBuild");
            RebuildAndRegen();
            return this;
        }
        
        /// <summary>
        /// Sets the description of a Corridor Surface.
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public CorridorSurface SetDescription(string description) => SetValue((object)description);

        /// <summary>
        /// Sets the name of a Corridor Surface.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CorridorSurface SetName(string name) => SetValue((object)name);

        /// <summary>
        /// Sets the overhang correction type assigned to a Corridor Surface.
        /// </summary>
        /// <param name="overhangCorrectionType"></param>
        /// <returns></returns>
        public CorridorSurface SetOverhangCorrection(string overhangCorrectionType)
        {
            civDb.OverhangCorrectionType ovType;
            if (!Enum.TryParse(overhangCorrectionType, out ovType))
            {
                throw new InvalidOperationException("Invalid overhang correction type.");
            }
            SetValue(ovType);
            RebuildAndRegen();
            return this;
        }

        private void RebuildAndRegen(
            bool rebuildCorridor = true,
            bool rebuildSurface = true,
            bool regen = true)
        {
            if (rebuildCorridor) { AeccCorridor.Rebuild(); }
            if (rebuildSurface) { AeccSurface.Rebuild(); }
            if (regen) { acDynNodes.Document.Current.AcDocument.Editor.Regen(); }
        }

        protected CorridorSurface SetValue(object value, [CallerMemberName] string methodName = null)
        {
            if (methodName.StartsWith("Set"))
            {
                methodName = methodName.Substring(3);
            }
            return SetValue(methodName, value);
        }

        protected CorridorSurface SetValue(string propertyName, object value)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                try
                {
                    civDb.Corridor aeccCorr = (civDb.Corridor)ctx.Transaction.GetObject(
                        AeccCorridorSurface.CorridorId, 
                        acDb.OpenMode.ForWrite);
                    PropertyInfo propInfo = AeccCorridorSurface.GetType()
                        .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                    propInfo?.SetValue(AeccCorridorSurface, value);
                    return this;
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException(e.Message);
                }
            }
        }
        #endregion
    }
}
