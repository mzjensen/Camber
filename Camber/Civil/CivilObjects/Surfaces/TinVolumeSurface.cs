using Autodesk.DesignScript.Runtime;
using Camber.Civil.Styles.Objects;
using DynamoServices;
using System;
using System.Linq;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using AeccTinVolumeSurface = Autodesk.Civil.DatabaseServices.TinVolumeSurface;
using civDb = Autodesk.Civil.DatabaseServices;
using civDynNodes = Autodesk.Civil.DynamoNodes;

namespace Camber.Civil.CivilObjects.Surfaces
{
    [RegisterForTrace]
    public sealed class TinVolumeSurface : CivilObject
    {
        #region properties
        internal AeccTinVolumeSurface AeccTinVolumeSurface => AcObject as AeccTinVolumeSurface;

        /// <summary>
        /// Gets the adjusted cut volume of a TIN Volume Surface.
        /// </summary>
        public double AdjustedCutVolume => AeccTinVolumeSurface.GetVolumeProperties().AdjustedCutVolume;

        /// <summary>
        /// Gets the adjusted fill volume of a TIN Volume Surface.
        /// </summary>
        public double AdjustedFillVolume => AeccTinVolumeSurface.GetVolumeProperties().AdjustedFillVolume;

        /// <summary>
        /// Gets the adjusted net volume of a TIN Volume Surface.
        /// </summary>
        public double AdjustedNetVolume => AeccTinVolumeSurface.GetVolumeProperties().AdjustedNetVolume;

        /// <summary>
        /// Gets the base surface of a TIN Volume Surface.
        /// </summary>
        public civDynNodes.Surface BaseSurface
        {
            get
            {
                var document = acDynNodes.Document.Current;

                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccSurf = (civDb.Surface) ctx.Transaction.GetObject(
                        AeccTinVolumeSurface.GetVolumeProperties().BaseSurface,
                        acDb.OpenMode.ForRead);
                    return civDynNodes.Selection.SurfaceByName(aeccSurf.Name, document);
                }
            }
        }

        /// <summary>
        /// Gets the comparison surface of a TIN Volume Surface.
        /// </summary>
        public civDynNodes.Surface ComparisonSurface
        {
            get
            {
                var document = acDynNodes.Document.Current;

                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccSurf = (civDb.Surface)ctx.Transaction.GetObject(
                        AeccTinVolumeSurface.GetVolumeProperties().ComparisonSurface,
                        acDb.OpenMode.ForRead);
                    return civDynNodes.Selection.SurfaceByName(aeccSurf.Name, document);
                }
            }
        }

        /// <summary>
        /// Gets the cut factor of a TIN Volume Surface.
        /// </summary>
        public double CutFactor => GetDouble();

        /// <summary>
        /// Gets the fill factor of a TIN Volume Surface.
        /// </summary>
        public double FillFactor => GetDouble();

        /// <summary>
        /// Gets the unadjusted cut volume of a TIN Volume Surface.
        /// </summary>
        public double UnadjustedCutVolume => AeccTinVolumeSurface.GetVolumeProperties().UnadjustedCutVolume;

        /// <summary>
        /// Gets the unadjusted fill volume of a TIN Volume Surface.
        /// </summary>
        public double UnadjustedFillVolume => AeccTinVolumeSurface.GetVolumeProperties().UnadjustedFillVolume;

        /// <summary>
        /// Gets the unadjusted net volume of a TIN Volume Surface.
        /// </summary>
        public double UnadjustedNetVolume => AeccTinVolumeSurface.GetVolumeProperties().UnadjustedNetVolume;
        #endregion

        #region constructors
        [SupressImportIntoVM]
        internal static TinVolumeSurface GetByObjectId(acDb.ObjectId tinSurfId)
            => CivilObjectSupport.Get<TinVolumeSurface, AeccTinVolumeSurface>
                (tinSurfId, (aeccTinVolumeSurf) => new TinVolumeSurface(aeccTinVolumeSurf));

        internal TinVolumeSurface(AeccTinVolumeSurface aeccTinVolumeSurface, bool isDynamoOwned = false) 
            : base(aeccTinVolumeSurface, isDynamoOwned) { }

        /// <summary>
        /// Creates a new TIN Volume Surface by base surface and comparison surface.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="surfaceName"></param>
        /// <param name="baseSurface"></param>
        /// <param name="comparisonSurface"></param>
        /// <param name="style"></param>
        /// <param name="cutFactor"></param>
        /// <param name="fillFactor"></param>
        /// <returns></returns>
        public static TinVolumeSurface ByTwoSurfaces(
            acDynNodes.Document document,
            string surfaceName,
            civDynNodes.Surface baseSurface,
            civDynNodes.Surface comparisonSurface,
            SurfaceStyle style,
            double cutFactor = 1.0,
            double fillFactor = 1.0)
        {
            if (string.IsNullOrEmpty(surfaceName))
            {
                throw new InvalidOperationException("Surface name is null or empty.");
            }

            bool hasSurfaceWithSameName = false;
            var res = CommonConstruct<TinVolumeSurface, AeccTinVolumeSurface>(
                document,
                (ctx) =>
                {
                    if (civDynNodes.Selection.Surfaces(document).Any(s => s.Name == surfaceName))
                    {
                        hasSurfaceWithSameName = true;
                        return null;
                    }

                    try
                    {
                        var surfId = AeccTinVolumeSurface.Create(
                            surfaceName,
                            baseSurface.InternalObjectId,
                            comparisonSurface.InternalObjectId,
                            style.InternalObjectId);
                        return (AeccTinVolumeSurface) ctx.Transaction.GetObject(surfId, acDb.OpenMode.ForRead);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(ex.Message);
                    }
                },
                (ctx, surf, existing) =>
                {
                    if (existing)
                    {
                        if (surf.Name != surfaceName && civDynNodes.Selection.Surfaces(document).All(obj => obj.Name != surfaceName))
                        {
                            surf.Name = surfaceName;
                        }
                        else if (surf.Name != surfaceName && civDynNodes.Selection.Surfaces(document).Any(obj => obj.Name == surfaceName))
                        {
                            hasSurfaceWithSameName = true;
                            return false;
                        }
                        surf.StyleId = style.InternalObjectId;
                        surf.CutFactor = cutFactor;
                        surf.FillFactor = fillFactor;
                    }
                    return true;
                });
            if (hasSurfaceWithSameName)
            {
                throw new InvalidOperationException("A Surface with the same name already exists");
            }
            return res;
        }
        #endregion

        #region methods
        public override string ToString() => $"TinVolumeSurface(Name = {Name})";

        /// <summary>
        /// Sets the cut factor for a TIN Volume Surface.
        /// </summary>
        /// <param name="cutFactor"></param>
        /// <returns></returns>
        public TinVolumeSurface SetCutFactor(double cutFactor)
        {
            try
            {
                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    AeccTinVolumeSurface.UpgradeOpen();
                    AeccTinVolumeSurface.CutFactor = cutFactor;
                    AeccTinVolumeSurface.DowngradeOpen();
                    return this;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Sets the fill factor for a TIN Volume Surface.
        /// </summary>
        /// <param name="fillFactor"></param>
        /// <returns></returns>
        public TinVolumeSurface SetFillFactor(double fillFactor)
        {
            try
            {
                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    AeccTinVolumeSurface.UpgradeOpen();
                    AeccTinVolumeSurface.FillFactor = fillFactor;
                    AeccTinVolumeSurface.DowngradeOpen();
                    return this;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
        #endregion
    }
}
