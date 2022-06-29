using Dynamo.Graph.Nodes;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using AcMText = Autodesk.AutoCAD.DatabaseServices.MText;

namespace Camber.AutoCAD.Objects
{
    public static class MText
    {
        #region query methods
        /// <summary>
        /// Gets the name of the text style assigned to an MText.
        /// </summary>
        /// <param name="mText"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static string TextStyle(this acDynNodes.MText mText) => GetString(mText, "TextStyleName");
        #endregion

        #region action methods
        /// <summary>
        /// Sets the text style for an MText.
        /// </summary>
        /// <param name="mText"></param>
        /// <param name="textStyleName">The name of the text style to assign.</param>
        /// <returns></returns>
        public static acDynNodes.MText SetTextStyle(this acDynNodes.MText mText, string textStyleName)
        {
            if (string.IsNullOrEmpty(textStyleName))
            {
                throw new InvalidOperationException("Text style name is null or empty.");
            }
            
            try
            {
                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    var textStyleTbl = (acDb.TextStyleTable) ctx.Transaction.GetObject(
                        ctx.Database.TextStyleTableId,
                        acDb.OpenMode.ForRead);
                    if (!textStyleTbl.Has(textStyleName))
                    {
                        throw new InvalidOperationException("Text style does not exist.");
                    }

                    var styleId = textStyleTbl[textStyleName];
                    var acMText = (AcMText) ctx.Transaction.GetObject(mText.InternalObjectId, acDb.OpenMode.ForWrite);
                    acMText.TextStyleId = styleId;
                    return mText;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
            
        }
        #endregion

        #region helper methods
        internal static string GetString(
            acDynNodes.MText mText,
            [CallerMemberName] string propertyName = null)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new Autodesk.AutoCAD.DynamoApp.Services.DocumentContext(document.AcDocument))
            {
                try
                {
                    var acMText = (AcMText)ctx.Transaction.GetObject(
                        mText.InternalObjectId,
                        acDb.OpenMode.ForRead);
                    PropertyInfo propInfo = acMText.GetType().GetProperty(
                        propertyName,
                        BindingFlags.Public | BindingFlags.Instance);
                    if (propInfo != null)
                    {
                        var value = propInfo.GetValue(acMText).ToString();
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
            acDynNodes.MText mText,
            [CallerMemberName] string propertyName = null)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new Autodesk.AutoCAD.DynamoApp.Services.DocumentContext(document.AcDocument))
            {
                try
                {
                    var acMText = (AcMText)ctx.Transaction.GetObject(
                        mText.InternalObjectId,
                        acDb.OpenMode.ForRead);
                    PropertyInfo propInfo = acMText.GetType().GetProperty(
                        propertyName,
                        BindingFlags.Public | BindingFlags.Instance);
                    if (propInfo != null)
                    {
                        return (bool)propInfo.GetValue(acMText);
                    }
                }
                catch { }
                return false;
            }
        }

        internal static double GetDouble(
            acDynNodes.MText mText,
            [CallerMemberName] string propertyName = null)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new Autodesk.AutoCAD.DynamoApp.Services.DocumentContext(document.AcDocument))
            {
                try
                {
                    var acMText = (AcMText)ctx.Transaction.GetObject(
                        mText.InternalObjectId,
                        acDb.OpenMode.ForRead);
                    PropertyInfo propInfo = acMText.GetType().GetProperty(
                        propertyName,
                        BindingFlags.Public | BindingFlags.Instance);
                    if (propInfo != null)
                    {
                        return (double)propInfo.GetValue(acMText);
                    }
                }
                catch { }
                return double.NaN;
            }
        }

        internal static acDynNodes.MText SetValue(
            acDynNodes.MText mText,
            object value, [CallerMemberName]
            string methodName = null)
        {
            if (methodName.StartsWith("Set"))
            {
                methodName = methodName.Substring(3);
            }
            return SetValue(mText, methodName, value);
        }

        internal static acDynNodes.MText SetValue(
            acDynNodes.MText mText,
            string propertyName,
            object value)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new Autodesk.AutoCAD.DynamoApp.Services.DocumentContext(document.AcDocument))
            {
                try
                {
                    var acMText = (AcMText)ctx.Transaction.GetObject(
                        mText.InternalObjectId,
                        acDb.OpenMode.ForWrite);
                    PropertyInfo propInfo = acMText.GetType().GetProperty(
                        propertyName,
                        BindingFlags.Public | BindingFlags.Instance);
                    propInfo?.SetValue(acMText, value);
                    return mText;
                }
                catch { throw; }
            }
        }
        #endregion
    }
}
