#region references
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using acApp = Autodesk.AutoCAD.ApplicationServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using AeccProfileDesignCheck = Autodesk.Civil.ProfileDesignCheck;
using Autodesk.DesignScript.Runtime;
using System.Collections.Generic;
#endregion

namespace Camber.Civil.Styles.DesignChecks
{
    public sealed class ProfileDesignCheck
    {
        #region properties
        private AeccProfileDesignCheck AeccProfileDesignCheck { get; set; }

        /// <summary>
        /// Gets the description of a Profile Design Check.
        /// </summary>
        public string Description => AeccProfileDesignCheck.Description;

        /// <summary>
        /// Gets the type of a Profile Design Check.
        /// </summary>
        public string DesignCheckType => AeccProfileDesignCheck.DesignCheckType.ToString();

        /// <summary>
        /// Gets the expression of a Profile Design Check.
        /// </summary>
        public string Expression => AeccProfileDesignCheck.Expression;

        /// <summary>
        /// Gets the name of a Profile Design Check.
        /// </summary>
        public string Name => AeccProfileDesignCheck.Name;
        #endregion

        #region constructors
        internal ProfileDesignCheck(AeccProfileDesignCheck aeccProfileDesignCheck)
        {
            AeccProfileDesignCheck = aeccProfileDesignCheck;
        }

        /// <summary>
        /// Creates a Profile Design Check of a given type of name, expression, and description.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="expression"></param>
        /// <param name="checkType"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static ProfileDesignCheck ByNameExpression(string name, string expression, string checkType, string description = "")
        {
            if (!Enum.IsDefined(typeof(Autodesk.Civil.ProfileDesignCheckType), checkType)) { throw new ArgumentException("Invalid design check type."); }
            var collectionName = checkType + "DesignChecks";
            try
            {
                using (var ctx = new acDynApp.DocumentContext((acApp.Document)null))
                {
                    var root = new Autodesk.Civil.ProfileDesignCheckRoot(ctx.Database);
                    var aeccCheckCollection = (Autodesk.Civil.ProfileDesignCheckCollection)root.GetType().GetProperty(collectionName).GetValue(root, null);
                    MethodInfo addMethod = aeccCheckCollection.GetType().GetMethod("Add");
                    var newCheck = (AeccProfileDesignCheck)addMethod.Invoke(aeccCheckCollection, new string[] { name, description, expression });
                    return new ProfileDesignCheck(newCheck);
                }
            }
            catch (Exception e) { throw e.InnerException; }
        }
        #endregion

        #region methods
        public override string ToString() => $"ProfileDesignCheck(Name = {Name}, Type = {DesignCheckType})";

        protected ProfileDesignCheck SetValue(object value, [CallerMemberName] string methodName = null)
        {
            if (methodName.StartsWith("Set"))
            {
                methodName = methodName.Substring(3);
            }
            return SetValue(methodName, value);
        }

        protected ProfileDesignCheck SetValue(string propertyName, object value)
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    PropertyInfo propInfo = AeccProfileDesignCheck.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                    propInfo?.SetValue(AeccProfileDesignCheck, value);
                    return this;
                }
            }
            catch (Exception e) { throw e.InnerException; }
        }

        /// <summary>
        /// Gets all of the Profile Design Checks in the drawing.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [MultiReturn(new[] { "Curve", "Line" })]
        public static Dictionary<string, object> GetProfileDesignChecks(acDynNodes.Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            var curveChecks = new List<ProfileDesignCheck>();
            var lineChecks = new List<ProfileDesignCheck>();

            using (var ctx = new acDynApp.DocumentContext((acApp.Document)null))
            {
                var root = new Autodesk.Civil.ProfileDesignCheckRoot(ctx.Database);

                foreach (var check in root.CurveDesignChecks) { curveChecks.Add(new ProfileDesignCheck(check)); }
                foreach (var check in root.LineDesignChecks) { lineChecks.Add(new ProfileDesignCheck(check)); }
            }
            return new Dictionary<string, object>
                {
                    { "Curve", curveChecks },
                    { "Line", lineChecks }
                };
        }

        /// <summary>
        /// Sets the description of a Profile Design Check.
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public ProfileDesignCheck SetDescription(string description) => SetValue(description);

        /// <summary>
        /// Sets the expression of a Profile Design Check.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public ProfileDesignCheck SetExpression(string expression) => SetValue(expression);
        #endregion
    }
}