#region references
using Autodesk.DesignScript.Geometry;
using Camber.Utilities.GeometryConversions;
using Dynamo.Graph.Nodes;
using DynamoServices;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Camber.Properties;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using acGeom = Autodesk.AutoCAD.Geometry;
using Autodesk.DesignScript.Runtime;
#endregion

namespace Camber.AutoCAD.Objects
{
    public static class Text
    {
        #region query methods
        /// <summary>
        /// Gets the alignment point of a Text object.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static Point AlignmentPoint(this acDynNodes.Text text)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                acDb.DBText acDbText = (acDb.DBText)ctx.Transaction.GetObject(
                    text.InternalObjectId,
                    acDb.OpenMode.ForRead);
                return GeometryConversions.AcPointToDynPoint(acDbText.AlignmentPoint);
            }
        }

        /// <summary>
        /// Gets whether a Text object is in its default alignment.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static bool IsDefaultAlignment(this acDynNodes.Text text) => GetBool(text);

        /// <summary>
        /// Gets the normal vector of the plane containing a Text object.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static Vector Normal(this acDynNodes.Text text)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                acDb.DBText acDbText = (acDb.DBText)ctx.Transaction.GetObject(
                    text.InternalObjectId,
                    acDb.OpenMode.ForRead);
                return GeometryConversions.AcVectorToDynamoVector(acDbText.Normal);
            }
        }

        /// <summary>
        /// Gets the thickness of a Text object.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static double Thickness(this acDynNodes.Text text) => GetDouble(text);
        #endregion

        #region action methods

        /// <summary>
        /// Sets the alignment point of a Text object.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static acDynNodes.Text SetAlignmentPoint(this acDynNodes.Text text, Point point)
            => SetValue(
                text,
                (acGeom.Point3d)GeometryConversions.DynPointToAcPoint(point, true));

        /// <summary>
        /// Sets the normal vector of the plane containing a Text object.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static acDynNodes.Text SetNormal(this acDynNodes.Text text, Vector vector)
            => SetValue(
                text,
                (acGeom.Vector3d)GeometryConversions.DynamoVectorToAcVector(
                    vector, 
                    true));

        /// <summary>
        /// Sets the thickness of a Text object.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="thickness"></param>
        /// <returns></returns>
        public static acDynNodes.Text SetThickness(this acDynNodes.Text text, double thickness) 
            => SetValue(text, thickness);
        #endregion

        #region helper methods
        internal static string GetString(
            acDynNodes.Text text, 
            [CallerMemberName] string propertyName = null)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                try
                {
                    var acDbText = (acDb.DBText)ctx.Transaction.GetObject(
                        text.InternalObjectId, 
                        acDb.OpenMode.ForRead);
                    PropertyInfo propInfo = acDbText.GetType().GetProperty(
                        propertyName, 
                        BindingFlags.Public | BindingFlags.Instance);
                    if (propInfo != null)
                    {
                        var value = propInfo.GetValue(acDbText).ToString();
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
                return "Not applicable.";
            }
        }

        internal static bool GetBool(
            acDynNodes.Text text, 
            [CallerMemberName] string propertyName = null)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                try
                {
                    var acDbText = (acDb.DBText)ctx.Transaction.GetObject(
                        text.InternalObjectId, 
                        acDb.OpenMode.ForRead);
                    PropertyInfo propInfo = acDbText.GetType().GetProperty(
                        propertyName, 
                        BindingFlags.Public | BindingFlags.Instance);
                    if (propInfo != null)
                    {
                        return (bool)propInfo.GetValue(acDbText);
                    }
                }
                catch { }
                return false;
            }
        }

        internal static double GetDouble(
            acDynNodes.Text text,
            [CallerMemberName] string propertyName = null)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                try
                {
                    var acDbText = (acDb.DBText)ctx.Transaction.GetObject(
                        text.InternalObjectId,
                        acDb.OpenMode.ForRead);
                    PropertyInfo propInfo = acDbText.GetType().GetProperty(
                        propertyName,
                        BindingFlags.Public | BindingFlags.Instance);
                    if (propInfo != null)
                    {
                        return (double)propInfo.GetValue(acDbText);
                    }
                }
                catch { }
                return double.NaN;
            }
        }

        internal static acDynNodes.Text SetValue(
            acDynNodes.Text text, 
            object value, [CallerMemberName] 
            string methodName = null)
        {
            if (methodName.StartsWith("Set"))
            {
                methodName = methodName.Substring(3);
            }
            return SetValue(text, methodName, value);
        }

        internal static acDynNodes.Text SetValue(
            acDynNodes.Text text, 
            string propertyName, 
            object value)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                try
                {
                    var acDbText = (acDb.DBText)ctx.Transaction.GetObject(
                        text.InternalObjectId, 
                        acDb.OpenMode.ForWrite);
                    PropertyInfo propInfo = acDbText.GetType().GetProperty(
                        propertyName, 
                        BindingFlags.Public | BindingFlags.Instance);
                    propInfo?.SetValue(acDbText, value);
                    return text;
                }
                catch { throw; }
            }
        }
        #endregion

        #region deprecated
        /// <summary>
        /// Gets the contents of a Text object.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.AutoCAD.Objects.Text.Contents",
            "Autodesk.AutoCAD.DynamoNodes.Text.Contents")]
        [NodeCategory("Query")]
        public static string Contents(this acDynNodes.Text text)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Text.Contents"));
            return GetString(text, "TextString");
        }

        /// <summary>
        /// Gets the height of a Text object.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.AutoCAD.Objects.Text.Height",
            "Autodesk.AutoCAD.DynamoNodes.Text.TextHeight")]
        [NodeCategory("Query")]
        public static double Height(this acDynNodes.Text text)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Text.TextHeight"));
            return GetDouble(text);
        }

        /// <summary>
        /// Gets the horizontal alignment mode of a Text object.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static string HorizontalMode(this acDynNodes.Text text)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MESSAGE, "Text.Justification"));
            return GetString(text);
        }

        /// <summary>
        /// Gets whether a Text object is backward (i.e. mirrored in the X direction).
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.AutoCAD.Objects.Text.IsBackward",
            "Autodesk.AutoCAD.DynamoNodes.Text.IsBackward")]
        [NodeCategory("Query")]
        public static bool IsBackward(this acDynNodes.Text text)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Text.IsBackward"));
            return GetBool(text, "IsMirroredInX");
        }

        /// <summary>
        /// Gets whether a Text object is upside down (i.e. mirrored in the Y direction).
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.AutoCAD.Objects.Text.IsUpsideDown",
            "Autodesk.AutoCAD.DynamoNodes.Text.IsUpsideDown")]
        [NodeCategory("Query")]
        public static bool IsUpsideDown(this acDynNodes.Text text)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Text.IsUpsideDown"));
            return GetBool(text, "IsMirroredInY");
        }

        /// <summary>
        /// Gets the oblique angle (in degrees) of a Text object in the range of [0,360).
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.AutoCAD.Objects.Text.Obliquing",
            "Autodesk.AutoCAD.DynamoNodes.Text.ObliqueAngle")]
        [NodeCategory("Query")]
        public static double Obliquing(this acDynNodes.Text text)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Text.ObliqueAngle"));
            return Utilities.MathUtilities.RadiansToDegrees(GetDouble(text, "Oblique"));
        }

        /// <summary>
        /// Gets the insertion point of a Text object in WCS coordinates.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.AutoCAD.Objects.Text.Position",
            "Autodesk.AutoCAD.DynamoNodes.Object.Location")]
        [NodeCategory("Query")]
        public static Point Position(this acDynNodes.Text text)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Object.Location"));

            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                acDb.DBText acDbText = (acDb.DBText)ctx.Transaction.GetObject(
                    text.InternalObjectId,
                    acDb.OpenMode.ForRead);
                return GeometryConversions.AcPointToDynPoint(acDbText.Position);
            }
        }

        /// <summary>
        /// Gets the rotation angle (in degrees) of a Text object in the range of [0,360).
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.AutoCAD.Objects.Text.Rotation",
            "Autodesk.AutoCAD.DynamoNodes.Object.Rotation")]
        [NodeCategory("Query")]
        public static double Rotation(this acDynNodes.Text text)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Object.Rotation"));
            return Utilities.MathUtilities.RadiansToDegrees(GetDouble(text));
        }

        /// <summary>
        /// Sets the contents of a Text object.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="contents"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.AutoCAD.Objects.Text.SetContents",
            "Autodesk.AutoCAD.DynamoNodes.Text.SetContents")]
        public static acDynNodes.Text SetContents(this acDynNodes.Text text, string contents)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Text.SetContents"));
            return SetValue(text, (object)contents, "TextString");
        }

        /// <summary>
        /// Sets the height of a Text object.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.AutoCAD.Objects.Text.SetHeight",
            "Autodesk.AutoCAD.DynamoNodes.Text.SetTextHeight")]
        public static acDynNodes.Text SetHeight(this acDynNodes.Text text, double height)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Text.SetTextHeight"));
            return SetValue(text, height);
        }

        /// <summary>
        /// Sets whether a Text object is backward (i.e. mirrored in the X direction).
        /// </summary>
        /// <param name="text"></param>
        /// <param name="bool"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.AutoCAD.Objects.Text.SetIsBackward",
            "Autodesk.AutoCAD.DynamoNodes.Text.SetBackward")]
        public static acDynNodes.Text SetIsBackward(this acDynNodes.Text text, bool @bool)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Text.SetBackward"));
            return SetValue(text, @bool, "IsMirroredInX");
        }

        /// <summary>
        /// Sets whether a Text object is upside down (i.e. mirrored in the Y direction).
        /// </summary>
        /// <param name="text"></param>
        /// <param name="bool"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.AutoCAD.Objects.Text.SetIsUpsideDown",
            "Autodesk.AutoCAD.DynamoNodes.Text.SetUpsideDown")]
        public static acDynNodes.Text SetIsUpsideDown(this acDynNodes.Text text, bool @bool)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Text.SetUpsideDown"));
            return SetValue(text, @bool, "IsMirroredInY");
        }

        /// <summary>
        /// Sets the oblique angle (in degrees) of a Text object in the range of [0,360).
        /// </summary>
        /// <param name="text"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.AutoCAD.Objects.Text.SetObliquing",
            "Autodesk.AutoCAD.DynamoNodes.Text.SetObliqueAngle")]
        public static acDynNodes.Text SetObliquing(this acDynNodes.Text text, double angle)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Text.SetObliqueAngle"));
            return SetValue(text, Utilities.MathUtilities.DegreesToRadians(angle), "Oblique");
        }

        /// <summary>
        /// Sets the insertion point of a Text object in WCS coordinates.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.AutoCAD.Objects.Text.SetPosition",
            "Autodesk.AutoCAD.DynamoNodes.Object.SetLocation")]
        public static acDynNodes.Text SetPosition(this acDynNodes.Text text, Point point)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Object.SetLocation"));
            return SetValue(text, (acGeom.Point3d)GeometryConversions.DynPointToAcPoint(point, true));
        }

        /// <summary>
        /// Sets the rotation angle (in degrees) of a Text object in the range of [0,360).
        /// </summary>
        /// <param name="text"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.AutoCAD.Objects.Text.SetRotation",
            "Autodesk.AutoCAD.DynamoNodes.Object.SetRotation")]
        public static acDynNodes.Text SetRotation(this acDynNodes.Text text, double angle)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Object.SetRotation"));
            return SetValue(text, Utilities.MathUtilities.DegreesToRadians(angle));
        }

        /// <summary>
        /// Sets the text style for a Text object.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="textStyleName">The name of the text style to assign.</param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.AutoCAD.Objects.Text.SetTextStyle",
            "Autodesk.AutoCAD.DynamoNodes.Text.SetTextStyle")]
        public static acDynNodes.Text SetTextStyle(this acDynNodes.Text text, string textStyleName)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Text.SetTextStyle"));

            if (string.IsNullOrEmpty(textStyleName))
            {
                throw new InvalidOperationException("Text style name is null or empty.");
            }

            try
            {
                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    var textStyleTbl = (acDb.TextStyleTable)ctx.Transaction.GetObject(
                        ctx.Database.TextStyleTableId,
                        acDb.OpenMode.ForRead);
                    if (!textStyleTbl.Has(textStyleName))
                    {
                        throw new InvalidOperationException("Text style does not exist.");
                    }

                    var styleId = textStyleTbl[textStyleName];
                    var acDBText = (acDb.DBText)ctx.Transaction.GetObject(text.InternalObjectId, acDb.OpenMode.ForWrite);
                    acDBText.TextStyleId = styleId;
                    return text;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Sets the width factor of a Text object.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="widthFactor"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.AutoCAD.Objects.Text.SetWidthFactor",
            "Autodesk.AutoCAD.DynamoNodes.Text.SetWidthFactor")]
        public static acDynNodes.Text SetWidthFactor(this acDynNodes.Text text, double widthFactor)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Text.SetWidthFactor"));

            if (widthFactor <= 0)
            {
                throw new InvalidOperationException(
                    "The width factor must be greater than zero.");
            }
            return SetValue(text, widthFactor);
        }

        /// <summary>
        /// Gets the name of the Text Style assigned to a Text object.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.AutoCAD.Objects.Text.TextStyle",
            "Autodesk.AutoCAD.DynamoNodes.Text.TextStyle")]
        [NodeCategory("Query")]
        public static string TextStyle(this acDynNodes.Text text)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Text.TextStyle"));
            return GetString(text, "TextStyleName");
        }

        /// <summary>
        /// Gets the vertical alignment mode of a Text object.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static double VerticalMode(this acDynNodes.Text text)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MESSAGE, "Text.Justification"));
            return GetDouble(text);
        }

        /// <summary>
        /// Gets the width factor of a Text object.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [NodeMigrationMapping(
            "Camber.AutoCAD.Objects.Text.WidthFactor",
            "Autodesk.AutoCAD.DynamoNodes.Text.WidthFactor")]
        [NodeCategory("Query")]
        public static double WidthFactor(this acDynNodes.Text text)
        {
            LogWarningMessageEvents.OnLogInfoMessage(string.Format(Resources.NODE_DEPRECATED_MIGRATION_MESSAGE, "Text.WidthFactor"));
            return GetDouble(text);
        }
        #endregion
    }
}
