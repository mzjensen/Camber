#region references
using Autodesk.AutoCAD.Colors;
using Autodesk.DesignScript.Runtime;
using Dynamo.Graph.Nodes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
#endregion

namespace Camber.AutoCAD.Objects
{
    public static class Layer
    {
        #region query methods
        /// <summary>
        /// Gets the lineweight of a Layer.
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        [NodeCategory("Query")]
        public static object Lineweight(acDynNodes.Layer layer)
        {
            using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
            {
                var lt = (acDb.LayerTable)ctx.Transaction.GetObject(
                    ctx.Database.LayerTableId, 
                    acDb.OpenMode.ForRead);
                var ltr = (acDb.LayerTableRecord)ctx.Transaction.GetObject(
                    lt[layer.Name], 
                    acDb.OpenMode.ForRead);
                
                var lw = ltr.LineWeight;
                if ((int)lw < 0)
                {
                    if ((int)lw == -3)
                    {
                        return "Default";
                    }

                    return lw.ToString();
                }
                return (double)lw / 100;
            }
        }
        #endregion

        #region action methods
        /// <summary>
        /// Gets info about the color assigned to a Layer.
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        [MultiReturn(new[] {
            "Index",
            "Color name",
            "Book name" })]
        public static Dictionary<string, object> GetColorInfo(acDynNodes.Layer layer)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                try
                {
                    var lt = (acDb.LayerTable)ctx.Transaction.GetObject(ctx.Database.LayerTableId, acDb.OpenMode.ForRead);
                    var ltr = (acDb.LayerTableRecord)ctx.Transaction.GetObject(lt[layer.Name], acDb.OpenMode.ForRead);
                    short index = ltr.Color.ColorIndex;
                    string colorName = ltr.Color.ColorName;
                    string bookName = ltr.Color.BookName;

                    return new Dictionary<string, object>
                    {
                        { "Index", index },
                        { "Color name", colorName },
                        { "Book name", bookName }
                    };
                }
                catch { throw; }
            }
        }

        /// <summary>
        /// Sets the color of a Layer by ACI number.
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="colorIndex"></param>
        /// <returns></returns>
        public static acDynNodes.Layer SetColor(acDynNodes.Layer layer, int colorIndex)
        {
            var color = Color.FromColorIndex(ColorMethod.ByAci, (short)colorIndex);
            SetValue(layer, color);
            return layer;
        }

        /// <summary>
        /// Sets the color of a Layer by color and book name.
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="colorName"></param>
        /// <param name="bookName"></param>
        /// <returns></returns>
        public static acDynNodes.Layer SetColor(acDynNodes.Layer layer, string colorName, string bookName)
        {
            if (string.IsNullOrWhiteSpace(colorName))
            {
                throw new InvalidOperationException("Invalid color name.");
            }

            if (string.IsNullOrWhiteSpace(bookName))
            {
                throw new InvalidOperationException("Invalid book name.");
            }

            var color = Color.FromNames(colorName, bookName);
            SetValue(layer, color);
            return layer;
        }

        /// <summary>
        /// Sets the lineweight of a Layer.
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="lineweight"></param>
        /// <returns></returns>
        public static acDynNodes.Layer SetLineweight(acDynNodes.Layer layer, string lineweight)
        {
            if (string.IsNullOrWhiteSpace(lineweight))
            {
                throw new ArgumentNullException(nameof(lineweight));
            }
            acDb.LineWeight lwEnum;
            if (!Enum.TryParse(lineweight, out lwEnum))
            {
                throw new InvalidOperationException("Invalid lineweight.");
            }

            SetValue(layer, lwEnum, "LineWeight");
            return layer;
        }
        #endregion

        #region helper methods
        internal static acDynNodes.Layer SetValue(acDynNodes.Layer layer, object value, [CallerMemberName] string methodName = null)
        {
            if (methodName.StartsWith("Set"))
            {
                methodName = methodName.Substring(3);
            }
            return SetValue(layer, methodName, value);
        }

        internal static acDynNodes.Layer SetValue(acDynNodes.Layer layer, string propertyName, object value)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                try
                {
                    var lt = (acDb.LayerTable)ctx.Transaction.GetObject(ctx.Database.LayerTableId, acDb.OpenMode.ForWrite);
                    var ltr = (acDb.LayerTableRecord)ctx.Transaction.GetObject(lt[layer.Name], acDb.OpenMode.ForWrite);
                    PropertyInfo propInfo = ltr.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                    propInfo?.SetValue(ltr, value);
                    return layer;
                }
                catch { throw; }
            }
        }
        #endregion
    }
}
