#region references
using System;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civApp = Autodesk.Civil.ApplicationServices;
using AeccGradingCriteria = Autodesk.Civil.DatabaseServices.Styles.GradingCriteria;
using AeccGradingCriteriaSet = Autodesk.Civil.DatabaseServices.Styles.GradingCriteriaSet;
using DynamoServices;
using System.Runtime.CompilerServices;
using System.Reflection;
using Camber.Civil.Styles;
#endregion

namespace Camber.Civil.GradingCriteria
{
    [RegisterForTrace]
    public sealed class GradingCriteria : Style
    {
        #region properties
        internal AeccGradingCriteria AeccGradingCriteria => AcObject as AeccGradingCriteria;

        /// <summary>
        /// Gets the Grading Criteria Set that a Grading Criteria belongs to.
        /// </summary>
        public GradingCriteriaSet CriteriaSet
        {
            get
            {
                acDynNodes.Document document = acDynNodes.Document.Current;

                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    civApp.CivilDocument cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                    foreach (acDb.ObjectId setId in cdoc.Styles.GradingCriteriaSets)
                    {
                        var set = setId.GetObject(acDb.OpenMode.ForWrite) as AeccGradingCriteriaSet;
                        if (set[Name] != null)
                        {
                            return GradingCriteriaSet.GetByObjectId(setId);
                        }
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the cut slope value for a Grading Criteria.
        /// </summary>
        public double CutSlope => AeccGradingCriteria.CutSlope.Value;

        /// <summary>
        /// Gets how the cut slope should be represented in a Grading Criteria.
        /// </summary>
        public string CutSlopeFormatType => AeccGradingCriteria.CutSlopeFormatType.Value.ToString();

        /// <summary>
        /// Gets the horizontal distance that grading projection lines will be extended to from a Grading footprint.
        /// </summary>
        public double Distance => AeccGradingCriteria.Distance.Value;

        /// <summary>
        /// Gets the type of projection for the Distance target of a Grading Criteria.
        /// </summary>
        public string DistanceProjectionType => AeccGradingCriteria.DistanceProjectionType.ToString();

        /// <summary>
        /// Gets the elevation at which grading projection lines will be extended to from a Grading footprint.
        /// </summary>
        public double Elevation => AeccGradingCriteria.Elevation.Value;

        /// <summary>
        /// Gets the type of projection for the Elevation target of a Grading Criteria.
        /// </summary>
        public string ElevationProjectionType => AeccGradingCriteria.ElevationProjectionType.ToString();

        /// <summary>
        /// Gets the fill slope value of a Grading Criteria.
        /// </summary>
        public double FillSlope => AeccGradingCriteria.FillSlope.Value;

        /// <summary>
        /// Gets how the fill slope should be represented in a Grading Criteria.
        /// </summary>
        public string FillSlopeFormatType => AeccGradingCriteria.FillSlopeFormatType.Value.ToString();

        /// <summary>
        /// Gets how interior corner projections are cleaned up when a Grading footprint corner has different elevations.
        /// </summary>
        public string InteriorCornerOverlap => AeccGradingCriteria.InteriorCornerOverlap.Value.ToString();

        /// <summary>
        /// Gets the fixed distance from a Grading footprint that a projection will extend.
        /// </summary>
        public double ProjectionDistance => AeccGradingCriteria.ProjectionDistance.Value;

        /// <summary>
        /// Gets the relative elevation of a projection in a Grading Criteria.
        /// </summary>
        public double ProjectionRelativeElevation => AeccGradingCriteria.ProjectionRelativeElevation.Value;

        /// <summary>
        /// Gets the relative elevation that projection lines will be extended to from a Grading footprint.
        /// </summary>
        public double RelativeElevation => AeccGradingCriteria.RelativeElevation.Value;

        /// <summary>
        /// Gets the type of projection for the Relative Elevation property of a Grading Criteria.
        /// </summary>
        public string RelativeElevationProjectionType => AeccGradingCriteria.RelativeElevationProjectionType.ToString();

        /// <summary>
        /// Gets whether to search first for cut or fill slopes in the case where both would work.
        /// </summary>
        public string SearchOrder => AeccGradingCriteria.SearchOrder.Value.ToString();

        /// <summary>
        /// Gets the slope value of a Grading Criteria.
        /// </summary>
        public double Slope => AeccGradingCriteria.Slope.Value;

        /// <summary>
        /// Gets how the slope should be represented in a Grading Criteria. 
        /// </summary>
        public string SlopeFormatType => AeccGradingCriteria.SlopeFormatType.Value.ToString();

        /// <summary>
        /// Gets the type of projection for the Surface target of a Grading Criteria.
        /// </summary>
        public string SurfaceProjectionType => AeccGradingCriteria.SurfaceProjectionType.ToString();

        /// <summary>
        /// Gets the target method of a Grading Criteria.
        /// </summary>
        public string Target => AeccGradingCriteria.Target.ToString();
        #endregion

        #region constructors
        internal GradingCriteria(AeccGradingCriteria aeccGradingCriteria, bool isDynamoOwned = false) : base(aeccGradingCriteria, isDynamoOwned) { }

        internal static GradingCriteria GetByObjectId(acDb.ObjectId criteriaId)
            => StyleSupport.Get<GradingCriteria, AeccGradingCriteria>
            (criteriaId, (criteria) => new GradingCriteria(criteria));
        #endregion

        #region methods
        public override string ToString() => $"GradingCriteria(Name = {Name}, Target = {Target})";

        protected GradingCriteria SetValue(object value, [CallerMemberName] string methodName = null)
        {
            if (methodName.StartsWith("Set"))
            {
                methodName = methodName.Substring(3);
            }
            return SetValue(methodName, value);
        }

        protected GradingCriteria SetValue(string propertyName, object value)
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccParentStyle = ctx.Transaction.GetObject(CriteriaSet.AeccGradingCriteriaSet.ObjectId, acDb.OpenMode.ForWrite);
                    PropertyInfo propInfo = AeccGradingCriteria.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                    propInfo?.SetValue(AeccGradingCriteria, value);
                    return this;
                }
            }
            catch (Exception e) { throw e.InnerException; }
        }
        protected GradingCriteria SetNestedProperty(object value, [CallerMemberName] string methodName = null)
        {
            if (methodName.StartsWith("Set"))
            {
                methodName = methodName.Substring(3);
            }
            return SetNestedProperty(methodName, value);
        }

        protected GradingCriteria SetNestedProperty(string propertyName, object value)
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccParentStyle = ctx.Transaction.GetObject(CriteriaSet.AeccGradingCriteriaSet.ObjectId, acDb.OpenMode.ForWrite);
                    Utils.ReflectionUtils.SetNestedProperty(AeccGradingCriteria, propertyName + ".Value", value);
                    return this;
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// Sets the cut slope value for a Grading Criteria.
        /// </summary>
        /// <param name="slope"></param>
        /// <returns></returns>
        public GradingCriteria SetCutSlope(double slope) => SetNestedProperty(slope);

        /// <summary>
        /// Sets how the cut slope should be represented in a Grading Criteria.
        /// </summary>
        /// <param name="formatType"></param>
        /// <returns></returns>
        public GradingCriteria SetCutSlopeFormatType(string formatType) => SetNestedProperty(Enum.Parse(typeof(Autodesk.Civil.GradingSlopeFormatType), formatType));

        /// <summary>
        /// Sets the horizontal distance that grading projection lines will be extended to from a Grading footprint.
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public GradingCriteria SetDistance(double distance) => SetNestedProperty(distance);

        /// <summary>
        /// Sets the type of projection for the Distance target of a Grading Criteria.
        /// </summary>
        /// <param name="projectionType"></param>
        /// <returns></returns>
        public GradingCriteria SetDistanceProjectionType(string projectionType) => SetValue(Enum.Parse(typeof(Autodesk.Civil.GradingDistanceProjectionType), projectionType));

        /// <summary>
        /// Sets the elevation at which grading projection lines will be extended to from a Grading footprint.
        /// </summary>
        /// <param name="elevation"></param>
        /// <returns></returns>
        public GradingCriteria SetElevation(double elevation) => SetNestedProperty(elevation);

        /// <summary>
        /// Sets the type of projection for the Elevation target of a Grading Criteria.
        /// </summary>
        /// <param name="projectionType"></param>
        /// <returns></returns>
        public GradingCriteria SetElevationProjectionType(string projectionType) => SetValue(Enum.Parse(typeof(Autodesk.Civil.GradingElevationProjectionType), projectionType));

        /// <summary>
        /// Sets the fill slope value for a Grading Criteria.
        /// </summary>
        /// <param name="slope"></param>
        /// <returns></returns>
        public GradingCriteria SetFillSlope(double slope) => SetNestedProperty(slope);

        /// <summary>
        /// Sets how the fill slope should be represented in a Grading Criteria.
        /// </summary>
        /// <param name="formatType"></param>
        /// <returns></returns>
        public GradingCriteria SetFillSlopeFormatType(string formatType) => SetNestedProperty(Enum.Parse(typeof(Autodesk.Civil.GradingSlopeFormatType), formatType));

        /// <summary>
        /// Sets how interior corner projections are cleaned up when a Grading footprint corner has different elevations.
        /// </summary>
        /// <param name="overlapSolution"></param>
        /// <returns></returns>
        public GradingCriteria SetInteriorCornerOverlap(string overlapSolution) =>
            SetNestedProperty(Enum.Parse(typeof(Autodesk.Civil.GradingInteriorCornerOverlapSolutionType), overlapSolution));

        /// <summary>
        /// Sets the fixed distance from a Grading footprint that a projection will extend.
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public GradingCriteria SetProjectionDistance(double distance) => SetNestedProperty(distance);

        /// <summary>
        /// Sets the relative elevation of a projection in a Grading Criteria.
        /// </summary>
        /// <param name="elevation"></param>
        /// <returns></returns>
        public GradingCriteria SetProjectionRelativeElevation(double elevation) => SetNestedProperty(elevation);

        /// <summary>
        /// Sets the relative elevation that projection lines will be extended to from a Grading footprint.
        /// </summary>
        /// <param name="elevation"></param>
        /// <returns></returns>
        public GradingCriteria SetRelativeElevation(double elevation) => SetNestedProperty(elevation);

        /// <summary>
        /// Sets the type of projection for the Relative Elevation property of a Grading Criteria.
        /// </summary>
        /// <param name="projectionType"></param>
        /// <returns></returns>
        public GradingCriteria SetRelativeElevationProjectionType(string projectionType) =>
            SetValue(Enum.Parse(typeof(Autodesk.Civil.GradingRelativeElevationProjectionType), projectionType));

        /// <summary>
        /// Sets whether to search first for cut or fill slopes in the case where both would work.
        /// </summary>
        /// <param name="searchOrder"></param>
        /// <returns></returns>
        public GradingCriteria SetSearchOrder(string searchOrder) => SetNestedProperty(Enum.Parse(typeof(Autodesk.Civil.GradingSearchOrderType), searchOrder));

        /// <summary>
        /// Sets the slope value of a Grading Criteria.
        /// </summary>
        /// <param name="slope"></param>
        /// <returns></returns>
        public GradingCriteria SetSlope(double slope) => SetNestedProperty(slope);

        /// <summary>
        /// Sets how the slope should be represented in a Grading Criteria. 
        /// </summary>
        /// <param name="formatType"></param>
        /// <returns></returns>
        public GradingCriteria SetSlopeFormatType(string formatType) => SetNestedProperty(Enum.Parse(typeof(Autodesk.Civil.SlopeFormatType), formatType));

        /// <summary>
        /// Sets the type of projection for the Surface target of a Grading Criteria.
        /// </summary>
        /// <param name="projectionType"></param>
        /// <returns></returns>
        public GradingCriteria SetSurfaceProjectionType(string projectionType) => SetValue(Enum.Parse(typeof(Autodesk.Civil.GradingSurfaceProjectionType), projectionType));

        /// <summary>
        /// Sets the target method of a Grading Criteria.
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public GradingCriteria SetTarget(string targetType) => SetValue(Enum.Parse(typeof(Autodesk.Civil.GradingTargetType), targetType));
        #endregion
    }
}
