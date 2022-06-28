#region references
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using civApp = Autodesk.Civil.ApplicationServices;
using civDb = Autodesk.Civil.DatabaseServices;
using civStyles = Autodesk.Civil.DatabaseServices.Styles;
using AeccStyle = Autodesk.Civil.DatabaseServices.Styles.StyleBase;
using AeccDisplayStyle = Autodesk.Civil.DatabaseServices.Styles.DisplayStyle;
using Autodesk.DesignScript.Runtime;
using DynamoServices;
using Camber.Civil.Styles.Objects;
using Camber.Civil.Styles.Views;
using Camber.Civil.DesignChecks;
#endregion

namespace Camber.Civil.Styles
{
    [RegisterForTrace]
    public class Style : acDynNodes.ObjectBase
    {
        #region properties
        internal AeccStyle AeccStyle => AcObject as AeccStyle;
        private const string DuplicateMessage = "A Style with that name and type already exists.";
        private const string NullNameMessage = "Name is null or empty.";
        private const string NullTypeMessage = "Type name is null or empty.";
        private const string NullCollectionMessage = "Collection name is null or empty.";
        private const string InvalidTypeMessage = "Invalid style type.";
        
        /// <summary>
        /// Gets the "Created by" string for a Style.
        /// </summary>
        public string CreateBy => GetString();

        /// <summary>
        /// Gets the "Date created" string for a Style.
        /// </summary>
        public string DateCreated => GetString();

        /// <summary>
        /// Gets the "Date modified" string for a Style.
        /// </summary>
        public string DateModified => GetString();

        /// <summary>
        /// Gets the "Modified by" string for a Style.
        /// </summary>
        public string ModifiedBy => GetString();

        /// <summary>
        /// Gets the name of a Style.
        /// </summary>
        public string Name => AeccStyle.Name;

        /// <summary>
        /// Gets whether a Style is used by other objects in the current drawing.
        /// </summary>
        public bool IsUsed => AeccStyle.IsUsed;
        #endregion

        #region constructors
        internal static Style GetByObjectId(acDb.ObjectId styleId)
            => StyleSupport.Get<Style, AeccStyle>
            (styleId, (style) => new Style(style));

        internal Style(AeccStyle aeccStyle, bool isDynamoOwned = false) : base(aeccStyle, isDynamoOwned) { }

        /// <summary>
        /// Gets a style by name and type.
        /// </summary>
        /// <param name="styleName">The name of the style</param>
        /// <param name="styleCollection">The name of the style collection as it appears in the API</param>
        /// <param name="styleType">The fully-qualified type name</param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        public static Style GetByNameType(string styleName, string styleCollection, string styleType)
        {
            if (string.IsNullOrEmpty(styleName)) { throw new ArgumentNullException(NullNameMessage); }
            if (string.IsNullOrEmpty(styleCollection)) { throw new ArgumentNullException(NullCollectionMessage); }
            if (string.IsNullOrEmpty(styleType)) { throw new ArgumentNullException(NullTypeMessage); }

            Type type = Type.GetType(styleType);
            var document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                civApp.CivilDocument cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                var aeccStyleCollection = (civDb.Styles.StyleCollectionBase)cdoc.Styles.GetType().GetProperty(styleCollection).GetValue(cdoc.Styles, null);
                var itemProp = aeccStyleCollection.GetType().GetProperty("Item", new[] { typeof(string) });
                var getMethod = itemProp.GetGetMethod();
                acDb.ObjectId styleId = (acDb.ObjectId)getMethod.Invoke(aeccStyleCollection, new string[] { styleName });

                var aeccStyle = styleId.GetObject(acDb.OpenMode.ForRead);
                if (aeccStyle != null)
                {
                    return (Style)Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { aeccStyle, false }, null);
                }
                return null;
            }
        }

        /// <summary>
        /// Creates a style by name and type.
        /// </summary>
        /// <param name="styleName">The name of the style</param>
        /// <param name="styleCollection">The name of the style collection as it appears in the API</param>
        /// <param name="styleType">The fully-qualified type name</param>
        /// <returns></returns>
        protected static Style CreateByNameType(string styleName, string styleCollection, string styleType)
        {
            if (string.IsNullOrEmpty(styleName)) { throw new ArgumentNullException(NullNameMessage); }
            if (string.IsNullOrEmpty(styleCollection)) { throw new ArgumentNullException(NullCollectionMessage); }
            if (string.IsNullOrEmpty(styleType)) { throw new ArgumentNullException(NullTypeMessage); }

            Type type = Type.GetType(styleType);
            var document = acDynNodes.Document.Current;

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                civApp.CivilDocument cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                var aeccStyleCollection = cdoc.Styles.GetType().GetProperty(styleCollection).GetValue(cdoc.Styles, null);
                
                // Check for duplicate
                MethodInfo containsMethod = aeccStyleCollection.GetType().GetMethod("Contains", new[] { typeof(string) });
                bool collectionContains = (bool)containsMethod.Invoke(aeccStyleCollection, new string[] { styleName });
                if (collectionContains) { throw new Exception(DuplicateMessage); }
                
                // Add new style to collection
                MethodInfo addMethod = aeccStyleCollection.GetType().GetMethod("Add", new[] { typeof(string) });
                acDb.ObjectId styleId = (acDb.ObjectId)addMethod.Invoke(aeccStyleCollection, new string[] { styleName });

                var aeccStyle = (AeccStyle)styleId.GetObject(acDb.OpenMode.ForRead);
                if (aeccStyle != null)
                {
                    return (Style)Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { aeccStyle, false }, null);
                }
                return null;
            }
        }

        /// <summary>
        /// Creates an Object Style by name and type.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="styleType"></param>
        /// <returns></returns>
        public static Style ObjectStyleByNameType(string name, string styleType)
        {
            if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException(NullNameMessage); }
            if (string.IsNullOrEmpty(styleType)) { throw new ArgumentNullException(NullTypeMessage); }
            if (!Enum.IsDefined(typeof(ObjectStyleCollections), styleType)) { throw new ArgumentException(InvalidTypeMessage); }

            // Remove the 's' from the end of the collection name
            string qualifiedName = "Camber.Civil.Styles.Objects." + styleType.Remove(styleType.Length - 1, 1);

            return CreateByNameType(name, styleType, qualifiedName);
        }

        /// <summary>
        /// Creates a View Style by name and type.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="styleType"></param>
        /// <returns></returns>
        public static Style ViewStyleByNameType(string name, string styleType)
        {
            if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException(NullNameMessage); }
            if (string.IsNullOrEmpty(styleType)) { throw new ArgumentNullException(NullTypeMessage); }
            if (!Enum.IsDefined(typeof(ViewStyleCollections), styleType)) { throw new ArgumentException(InvalidTypeMessage); }

            // Remove the 's' from the end of the collection name
            string qualifiedName = "Camber.Civil.Styles.Views." + styleType.Remove(styleType.Length - 1, 1);

            return CreateByNameType(name, styleType, qualifiedName);
        }

        /// <summary>
        /// Creates a Design Check Set by name and type.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="checkSetType"></param>
        /// <returns></returns>
        protected static Style DesignCheckSetByNameType(string name, string checkSetType)
        {
            if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException(NullNameMessage); }
            if (string.IsNullOrEmpty(checkSetType)) { throw new ArgumentNullException(NullTypeMessage); }
            if (!Enum.IsDefined(typeof(DesignCheckSetCollections), checkSetType)) { throw new ArgumentException(InvalidTypeMessage); }

            // Remove the 's' from the end of the collection name
            string qualifiedName = "Camber.Civil.DesignChecks." + checkSetType.Remove(checkSetType.Length - 1, 1);

            return CreateByNameType(name, checkSetType, qualifiedName);
        }
        #region BUG: trace-enabled constructors don't seem to work with style collections.
        ///// <summary>
        ///// Creates a style by name and type.
        ///// </summary>
        ///// <param name="styleName"></param>
        ///// <param name="styleCollection"></param>
        ///// <param name="styleType"></param>
        ///// <returns></returns>
        //protected static Style CreateByNameType(string styleName, StyleCollections styleCollection, Type styleType)
        //{
        //    if (string.IsNullOrEmpty(styleName))
        //    {
        //        throw new ArgumentNullException("Style name is null or empty.");
        //    }

        //    var document = acDynNodes.Document.Current;
        //    string styleCollectionName = styleCollection.ToString();
        //    string[] args = new string[] { styleName };

        //    using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
        //    {
        //        civApp.CivilDocument cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
        //        acDb.ObjectId styleId = acDynApp.ElementBinder.GetObjectIdFromTrace(ctx.Database);
        //        var aeccStyleCollection = cdoc.Styles.GetType().GetProperty(styleCollectionName).GetValue(cdoc.Styles, null);
        //        MethodInfo containsMethod = aeccStyleCollection.GetType().GetMethod("Contains", new[] { typeof(string) });
        //        bool collectionContains = (bool)containsMethod.Invoke(aeccStyleCollection, args);
        //        if (collectionContains)
        //        {
        //            throw new Exception("The document already contains a style with the same name.");
        //        }
        //        else if (styleId.IsValid && !styleId.IsErased)
        //        {
        //            var style = styleId.GetObject(acDb.OpenMode.ForWrite);
        //            if (style != null)
        //            {
        //                // Update the style name
        //                style.GetType().GetProperty("Name").SetValue(style, styleName, null);
        //            }
        //        }
        //        else
        //        {
        //            // Add new style to collection
        //            MethodInfo addMethod = aeccStyleCollection.GetType().GetMethod("Add", new[] { typeof(string) });
        //            styleId = (acDb.ObjectId)addMethod.Invoke(aeccStyleCollection, args);
        //        }

        //        var aeccStyle = (AeccStyle)styleId.GetObject(acDb.OpenMode.ForRead);
        //        if (aeccStyle != null)
        //        {
        //            // Get constructor from the input type.
        //            // The BindingFlags are required to get non-public constructors, otherwise it returns null.
        //            ConstructorInfo constructor = styleType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(acDb.DBObject), typeof(bool) }, null);
        //            var styleInstance = (Style)constructor.Invoke(new object[] { aeccStyle, true });
        //            return styleInstance;
        //        }
        //        return null;
        //    }
        //}
        #endregion
        #endregion

        #region methods
        public override string ToString() => "Style(Name = " + Name + ")";

        /// <summary>
        /// Sets the name of a Style.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Style SetName(string name) => SetValue(name);

        /// <summary>
        /// Sets the "Created by" string for a Style.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Style SetCreateBy(string value) => SetValue(value);

        /// <summary>
        /// Duplicates a Style.
        /// </summary>
        /// <param name="newStyleName"></param>
        /// <returns></returns>
        public Style Duplicate(string newStyleName)
        {
            try
            {
                AeccStyle.CopyAsSibling(newStyleName);
                return this;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Gets the Display Styles by view direction for a Style. 
        /// </summary>
        /// <param name="style"></param>
        /// <param name="viewDirection"></param>
        /// <returns></returns>
        public static IEnumerable<DisplayStyle> GetDisplayStyles(Style style, string viewDirection)
        {
            if (string.IsNullOrEmpty(viewDirection))
            {
                throw new ArgumentException("View direction is null or empty.");
            }

            if (!Enum.IsDefined(typeof(DisplayStyleViewDirections), viewDirection))
            {
                throw new ArgumentException("Invalid view direction.");
            }

            List<DisplayStyle> displayStyles = new List<DisplayStyle>();
            
            // Get all methods that contain "DisplayStyle" in the name
            var methods = style.AeccStyle.GetType().GetMethods().Where(m => (m.Name.Contains("DisplayStyle") && m.Name.Contains(viewDirection) && !m.Name.Contains("Hatch")));

            // Loop through the collection of methods
            foreach (MethodInfo method in methods)
            {
                var methodParams = method.GetParameters();
                if (methodParams.Length > 1)
                    // If method contains more than 1 parameter, skip
                    continue;
                if (methodParams.Length == 0)
                {
                    var aeccDisplayStyle = (AeccDisplayStyle)method.Invoke(style.AeccStyle, new object[] { });
                    if (aeccDisplayStyle == null)
                        continue;
                    DisplayStyle displayStyle = new DisplayStyle(style, aeccDisplayStyle);

                    // First check if name is valid from list of special cases
                    string name = GetDisplayStyleName(style, viewDirection);

                    // Otherwise get name from method name
                    if (string.IsNullOrEmpty(name))
                    {
                        // Remove characters from method name
                        name = method.Name;
                        var charsToRemove = new string[] {
                        "Get",
                        "get_",
                        "DisplayStyle",
                        "Plan",
                        "Profile",
                        "Model",
                        "Section" };

                        foreach (var c in charsToRemove)
                        {
                            name = name.Replace(c, string.Empty);
                        }
                    }

                    displayStyle.Name = name;
                    displayStyle.ViewDirection = viewDirection;
                    displayStyles.Add(displayStyle);
                    continue;
                }
                ParameterInfo param = methodParams[0];
                if (!param.ParameterType.IsEnum)
                    // If not an enumeration, skip
                    continue;
                
                // Check all values in the enumeration
                foreach (var enumValue in Enum.GetValues(param.ParameterType))
                {
                    try
                    {
                        var aeccDisplayStyle = (AeccDisplayStyle)method.Invoke(style.AeccStyle, new object[] { enumValue });
                        if (aeccDisplayStyle == null || enumValue.ToString().Contains("Curvature"))
                            // Something went wrong
                            continue;
                        DisplayStyle displayStyle = new DisplayStyle(style, aeccDisplayStyle);
                        displayStyle.Name = enumValue.ToString();
                        displayStyle.ViewDirection = viewDirection;
                        displayStyles.Add(displayStyle);
                    }
                    catch { }
                }
            }
            
            // Sort Display Styles by name
            displayStyles = displayStyles.OrderBy(x => x.Name).ToList();
            
            // There are some methods within the same class that seem to return the same result,
            // so filter out duplicates by name if they are created somehow
            var uniqueItems = displayStyles.GroupBy(x => x.Name).Select(x => x.First()).ToList();

            // Sometimes Display Styles are created without a name, so drop them
            var result = new List<DisplayStyle>();
            foreach (var displayStyle in uniqueItems)
            {
                if (!string.IsNullOrEmpty(displayStyle.Name))
                {
                    result.Add(displayStyle);
                }
            }

            if (result.Count == 0)
            {
                throw new Exception("This type of Style does not contain any Display Styles in the " + viewDirection +" view direction.");
            }

            return result;
        }

        /// <summary>
        /// Handles the special cases for Display Style names.
        /// </summary>
        /// <param name="style"></param>
        /// <param name="viewDirection"></param>
        /// <returns></returns>
        private static string GetDisplayStyleName(Style style, string viewDirection)
        {
            string name = null;

            if (style is AlignmentStyle && viewDirection == "Section") { name = "LineMarkerInSection"; }
            if (style is FeatureLineStyle && viewDirection == "Section") { name = "CrossingMarker"; }
            if (style is IntersectionStyle) { name = "Marker"; }
            if (style is LinkStyle) { name = "Link"; }
            if (style is MarkerStyle && viewDirection == "Profile") { name = "Marker/CrossingVerticalLine"; }
            if (style is MatchLineStyle && viewDirection == "Plan") { name = "MatchLineMask"; }
            if (style is ParcelStyle && viewDirection == "Section") { name = "ParcelSegmentsMarker"; }
            if (style is PipeStyle && viewDirection == "Model") { name = "PipeSolid"; }
            if (style is ProfileStyle && viewDirection == "Section") { name = "ProfileMarker"; }
            if (style is StructureStyle) { name = "3DSolid"; }
            if (style is ViewFrameStyle) { name = "ViewFrameBorder"; }
            
            return name;
        }

        protected double GetDouble([CallerMemberName] string propertyName = null)
        {
            try
            {
                PropertyInfo propInfo = AeccStyle.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (propInfo != null)
                {
                    return (double)propInfo.GetValue(AeccStyle);
                }
            }
            catch { }
            return double.NaN;
        }

        protected string GetString([CallerMemberName] string propertyName = null)
        {
            try
            {
                PropertyInfo propInfo = AeccStyle.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (propInfo != null)
                {
                    var value = propInfo.GetValue(AeccStyle).ToString();
                    if (string.IsNullOrEmpty(value))
                    {
                        return null;
                    }
                    else
                    {
                        return value;
                    }
                }
            }
            catch { }
            return "Not applicable";
        }

        protected int GetInt([CallerMemberName] string propertyName = null)
        {
            try
            {
                PropertyInfo propInfo = AeccStyle.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (propInfo != null)
                {
                    return (int)propInfo.GetValue(AeccStyle);
                }
            }
            catch { }
            return int.MinValue;
        }

        protected bool GetBool([CallerMemberName] string propertyName = null)
        {
            try
            {
                PropertyInfo propInfo = AeccStyle.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

                if (propInfo != null)
                {
                    return (bool)propInfo.GetValue(AeccStyle);
                }
            }
            catch { }
            return false;
        }

        protected Style SetValue(object value, [CallerMemberName] string methodName = null)
        {
            if (methodName.StartsWith("Set"))
            {
                methodName = methodName.Substring(3);
            }
            return SetValue(methodName, value);
        }

        protected Style SetValue(string propertyName, object value)
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccStyle = ctx.Transaction.GetObject(InternalObjectId, acDb.OpenMode.ForWrite);
                    PropertyInfo propInfo = AeccStyle.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                    propInfo?.SetValue(AeccStyle, value);
                    return this;
                } 
            }
            catch (Exception e) { throw e.InnerException; }
        }
        #endregion
    }
}