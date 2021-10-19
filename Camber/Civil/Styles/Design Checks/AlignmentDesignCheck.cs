#region references
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using acApp = Autodesk.AutoCAD.ApplicationServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using AeccAlignmentDesignCheck = Autodesk.Civil.AlignmentDesignCheck;
using Autodesk.DesignScript.Runtime;
#endregion

namespace Camber.Civil.Styles.DesignChecks
{
    public sealed class AlignmentDesignCheck
    {
        #region properties
        private AeccAlignmentDesignCheck AeccAlignmentDesignCheck { get; set; }

        /// <summary>
        /// Gets the description of an Alignment Design Check.
        /// </summary>
        public string Description => AeccAlignmentDesignCheck.Description;

        /// <summary>
        /// Gets the type of an Alignment Design Check.
        /// </summary>
        public string DesignCheckType => AeccAlignmentDesignCheck.DesignCheckType.ToString();

        /// <summary>
        /// Gets the expression of an Alignment Design Check.
        /// </summary>
        public string Expression => AeccAlignmentDesignCheck.Expression;

        /// <summary>
        /// Gets the name of an Alignment Design Check.
        /// </summary>
        public string Name => AeccAlignmentDesignCheck.Name;
        #endregion

        #region constructors
        internal AlignmentDesignCheck(AeccAlignmentDesignCheck aeccAlignmentDesignCheckSet)
        {
            AeccAlignmentDesignCheck = aeccAlignmentDesignCheckSet;
        }

        /// <summary>
        /// Creates an Alignment Design Check of a given type of name, expression, and description.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="expression"></param>
        /// <param name="checkType"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static AlignmentDesignCheck ByNameExpression(string name, string expression, string checkType, string description = "")
        {
            if (!Enum.IsDefined(typeof(Autodesk.Civil.AlignmentDesignCheckType), checkType)) { throw new ArgumentException("Invalid design check type."); }
            var collectionName = checkType + "DesignChecks";
            try
            {
                using (var ctx = new acDynApp.DocumentContext((acApp.Document)null))
                {
                    var root = new Autodesk.Civil.AlignmentDesignCheckRoot(ctx.Database);
                    var aeccCheckCollection = (Autodesk.Civil.AlignmentDesignCheckCollection)root.GetType().GetProperty(collectionName).GetValue(root, null);
                    MethodInfo addMethod = aeccCheckCollection.GetType().GetMethod("Add");
                    var newCheck = (AeccAlignmentDesignCheck)addMethod.Invoke(aeccCheckCollection, new string[] { name, description, expression });
                    return new AlignmentDesignCheck(newCheck);
                }
            }
            catch (Exception e) { throw e.InnerException; }         
        }
        #endregion

        #region methods
        public override string ToString() => $"AlignmentDesignCheck(Name = {Name}, Type = {DesignCheckType})";

        /// <summary>
        /// Gets all of the Alignment Design Checks in the drawing.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [MultiReturn(new[] { 
            "Curve", 
            "Line", 
            "Spiral", 
            "Superelevation", 
            "Tangent Intersection" })]
        public static Dictionary<string, object> GetAlignmentDesignChecks(acDynNodes.Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            var curveChecks = new List<AlignmentDesignCheck>();
            var lineChecks = new List<AlignmentDesignCheck>();
            var spiralChecks = new List<AlignmentDesignCheck>();
            var superChecks = new List<AlignmentDesignCheck>();
            var tanIntChecks = new List<AlignmentDesignCheck>();

            using (var ctx = new acDynApp.DocumentContext((acApp.Document)null))
            {
                var root = new Autodesk.Civil.AlignmentDesignCheckRoot(ctx.Database);

                foreach (var check in root.CurveDesignChecks) { curveChecks.Add(new AlignmentDesignCheck(check)); }
                foreach (var check in root.LineDesignChecks) { lineChecks.Add(new AlignmentDesignCheck(check)); }
                foreach (var check in root.SpiralDesignChecks) { spiralChecks.Add(new AlignmentDesignCheck(check)); }
                foreach (var check in root.SuperelevationDesignChecks) { superChecks.Add(new AlignmentDesignCheck(check)); }
                foreach (var check in root.TangentIntersectionDesignChecks) { tanIntChecks.Add(new AlignmentDesignCheck(check)); }
            }
            return new Dictionary<string, object>
                {
                    { "Curve", curveChecks },
                    { "Line", lineChecks },
                    { "Spiral", spiralChecks },
                    { "Superelevation", superChecks },
                    { "Tangent Intersection", tanIntChecks }
                };
        }

        protected AlignmentDesignCheck SetValue(object value, [CallerMemberName] string methodName = null)
        {
            if (methodName.StartsWith("Set"))
            {
                methodName = methodName.Substring(3);
            }
            return SetValue(methodName, value);
        }

        protected AlignmentDesignCheck SetValue(string propertyName, object value)
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    PropertyInfo propInfo = AeccAlignmentDesignCheck.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                    propInfo?.SetValue(AeccAlignmentDesignCheck, value);
                    return this;
                }
            }
            catch (Exception e) { throw e.InnerException; }
        }

        /// <summary>
        /// Sets the description of an Alignment Design Check.
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public AlignmentDesignCheck SetDescription(string description) => SetValue(description);

        /// <summary>
        /// Sets the expression of an Alignment Design Check.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public AlignmentDesignCheck SetExpression(string expression) => SetValue(expression);
        #endregion
    }
}