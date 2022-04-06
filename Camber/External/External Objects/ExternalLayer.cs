#region references
using Autodesk.DesignScript.Runtime;
using DSCore;
using System;
using System.Collections.Generic;
using acColor = Autodesk.AutoCAD.Colors;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AcLayer = Autodesk.AutoCAD.DatabaseServices.LayerTableRecord;
using AcLayerTable = Autodesk.AutoCAD.DatabaseServices.LayerTable;
#endregion

namespace Camber.External.ExternalObjects
{
    public class ExternalLayer : ExternalObjectBase
    {
        #region properties
        internal AcLayer AcLayer => AcObject as AcLayer;

        /// <summary>
        /// Gets the description of an External Layer.
        /// </summary>
        public string Description => GetString();

        /// <summary>
        /// Gets if an External Layer has any property overrides associated with any viewport in the drawing.
        /// </summary>
        public bool HasOverrides => GetBool();

        /// <summary>
        /// Gets if an External Layer is frozen.
        /// </summary>
        public bool IsFrozen => GetBool();

        /// <summary>
        /// Gets if an External Layer is hidden.
        /// </summary>
        public bool IsHidden => GetBool();

        /// <summary>
        /// Gets if an External Layer is locked.
        /// </summary>
        public bool IsLocked => GetBool();

        /// <summary>
        /// Gets if an External Layer is off.
        /// </summary>
        public bool IsOff => GetBool();

        /// <summary>
        /// Gets if an External Layer is plottable.
        /// </summary>
        public bool IsPlottable => GetBool();

        /// <summary>
        /// Gets if an External Layer is reconciled.
        /// </summary>
        public bool IsReconciled => GetBool();

        /// <summary>
        /// Gets if an External Layer is used.
        /// </summary>
        public bool IsUsed => GetBool();

        /// <summary>
        /// Gets the lineweight of an External Layer.
        /// </summary>
        public object Lineweight
        {
            get
            {
                using (var tr = AcDatabase.TransactionManager.StartOpenCloseTransaction())
                {
                    var lt = (AcLayerTable)tr.GetObject(AcDatabase.LayerTableId, acDb.OpenMode.ForRead);
                    var ltr = (AcLayer)tr.GetObject(lt[this.Name], acDb.OpenMode.ForRead);

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
        }

        /// <summary>
        /// Gets the name of an External Layer.
        /// </summary>
        public string Name => GetString();

        /// <summary>
        /// Gets the Plot Style of an External Layer.
        /// </summary>
        public string PlotStyle => GetString("PlotStyleName");

        /// <summary>
        /// Gets the transparency of an External Layer.
        /// </summary>
        public double Transparency
        {
            get
            {
                double retVal = 0.0;
                using (var tr = AcDatabase.TransactionManager.StartTransaction())
                {
                    AcLayerTable layerTable = tr.GetObject(
                        AcDatabase.LayerTableId,
                        acDb.OpenMode.ForRead) as AcLayerTable;
                    if (layerTable != null && layerTable.Has(this.Name))
                    {
                        layerTable.UpgradeOpen();
                        AcLayer layerTableRecord = tr.GetObject(
                            layerTable[this.Name], 
                            acDb.OpenMode.ForWrite) as AcLayer;
                        acColor.Transparency acTransparency = layerTableRecord.Transparency;
                        if (acTransparency.IsByAlpha)
                        {
                            acTransparency = layerTableRecord.Transparency;
                            retVal = (double)(((int)byte.MaxValue - (int)acTransparency.Alpha) * 100 / (int)byte.MaxValue);
                        }
                        layerTable.DowngradeOpen();
                    }
                }
                return retVal;
            }
        }

        /// <summary>
        /// Gets if an External Layer is frozen by default in new viewports.
        /// </summary>
        public bool ViewportVisibilityDefault => GetBool();
        #endregion

        #region constructors
        internal ExternalLayer(AcLayer acLayer) : base(acLayer) { }

        /// <summary>
        /// Creates a new External Layer.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="externalDocument"></param>
        /// <param name="color"></param>
        /// <param name="linetype"></param>
        /// <param name="lineweight"></param>
        /// <returns></returns>
        public static ExternalLayer Create(
            string name,
            ExternalDocument externalDocument,
            [DefaultArgument("Color.ByARGB(255, 255, 255, 255)")]
            Color color,
            [DefaultArgument("\"Continuous\"")]
            string linetype,
            [DefaultArgument("\"ByLineWeightDefault\"")]
            string lineweight
        )
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (color == null)
            {
                throw new ArgumentNullException(nameof(color));
            }

            if (string.IsNullOrWhiteSpace(linetype))
            {
                throw new ArgumentNullException(nameof(linetype));
            }

            if (string.IsNullOrWhiteSpace(lineweight))
            {
                throw new ArgumentNullException(nameof(lineweight));
            }
            
            // Check lineweight    
            acDb.LineWeight lwEnum;
            if (!Enum.TryParse(lineweight, out lwEnum))
            {
                throw new InvalidOperationException("Invalid lineweight.");
            }

            // Get linetype id
            acDb.ObjectId linetypeId = GetLinetypeTableRecord(linetype, externalDocument).Id;

            using (var tr = externalDocument.AcDatabase.TransactionManager.StartTransaction())
            {
                if (tr.GetObject(
                        externalDocument.AcDatabase.LayerTableId, 
                        acDb.OpenMode.ForRead) is AcLayerTable layerTable)
                {
                    // Check if layer already exists
                    if (layerTable.Has(name))
                    {
                        throw new InvalidOperationException("A layer with the same name already exists.");
                    }

                    layerTable.UpgradeOpen();
                    // Create new layer table record and set properties
                    AcLayer layerRecord = new AcLayer();
                    layerRecord.Name = name;
                    layerRecord.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(color.Red, color.Green, color.Blue);
                    layerRecord.LineWeight = lwEnum;
                    layerRecord.LinetypeObjectId = linetypeId;
                    layerTable.Add(layerRecord);
                    layerTable.DowngradeOpen();
                    tr.AddNewlyCreatedDBObject(layerRecord, true);
                    
                    return new ExternalLayer(layerRecord);
                }
            }
            throw new InvalidOperationException("Could not create layer.");
        }
        #endregion

        #region methods
        public override string ToString() => $"ExternalLayer(Name = {Name})";

        /// <summary>
        /// Gets a linetype table record from a database by name.
        /// </summary>
        /// <param name="linetype"></param>
        /// <param name="externalDocument"></param>
        /// <returns></returns>
        private static acDb.LinetypeTableRecord GetLinetypeTableRecord(
            string linetype,
            ExternalDocument externalDocument)
        {
            if (string.IsNullOrWhiteSpace(linetype))
                throw new ArgumentNullException(nameof(linetype));
            
            if (externalDocument != null)
            {
                using (var tr = externalDocument.AcDatabase.TransactionManager.StartTransaction())
                {
                    if (tr.GetObject(
                            externalDocument.AcDatabase.LinetypeTableId, 
                            acDb.OpenMode.ForRead) is acDb.LinetypeTable linetypeTable)
                    {
                        if (!linetypeTable.Has(linetype))
                        {
                            throw new InvalidOperationException("Linetype does not exist.");
                        }

                        if (tr.GetObject(linetypeTable[linetype], acDb.OpenMode.ForRead)
                            is acDb.LinetypeTableRecord linetypeTableRecord)
                        {
                            return linetypeTableRecord;
                        }
                    }
                }
            }
            throw new InvalidOperationException("Could not get linetype table record.");
        }

        /// <summary>
        /// Gets info about the color assigned to an External Layer.
        /// </summary>
        /// <returns></returns>
        [MultiReturn(new[]
        {
            "Index",
            "Color name",
            "Book name"
        })]
        public Dictionary<string, object> GetColorInfo()
        {
            using (var tr = AcDatabase.TransactionManager.StartTransaction())
            {
                try
                {
                    var lt = (AcLayerTable)tr.GetObject(AcDatabase.LayerTableId, acDb.OpenMode.ForRead);
                    var ltr = (AcLayer)tr.GetObject(lt[this.Name], acDb.OpenMode.ForRead);
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
                catch (Exception ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
            }
        }

        /// <summary>
        /// Sets the color of an External Layer by ACI number.
        /// </summary>
        /// <param name="colorIndex"></param>
        /// <returns></returns>
        public ExternalLayer SetColor(int colorIndex) =>
            SetValue(acColor.Color.FromColorIndex(acColor.ColorMethod.ByAci, (short)colorIndex));

        /// <summary>
        /// Sets the color of an External Layer by color and book name.
        /// </summary>
        /// <param name="colorName"></param>
        /// <param name="bookName"></param>
        /// <returns></returns>
        public ExternalLayer SetColor(string colorName, string bookName)
        {
            if (string.IsNullOrWhiteSpace(colorName))
            {
                throw new InvalidOperationException("Invalid color name.");
            }

            if (string.IsNullOrWhiteSpace(bookName))
            {
                throw new InvalidOperationException("Invalid book name.");
            }

            var color = acColor.Color.FromNames(colorName, bookName);
            return SetValue(color);
        }

        /// <summary>
        /// Removes all overrides associated with an External Layer, for all viewports. 
        /// </summary>
        /// <returns></returns>
        public ExternalLayer RemoveAllOverrides()
        {
            try
            {
                AcLayer.UpgradeOpen();
                AcLayer.RemoveAllOverrides();
                AcLayer.DowngradeOpen();
                return this;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Sets the description of an External Layer.
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public ExternalLayer SetDescription(string description) => SetValue(description);

        /// <summary>
        /// Sets if an External Layer is frozen.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public ExternalLayer SetIsFrozen(bool @bool) => SetValue(@bool);

        /// <summary>
        /// Sets if an External Layer is hidden.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public ExternalLayer SetIsHidden(bool @bool) => SetValue(@bool);

        /// <summary>
        /// Sets if an External Layer is locked.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public ExternalLayer SetIsLocked(bool @bool) => SetValue(@bool);

        /// <summary>
        /// Sets if an External Layer is off.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public ExternalLayer SetIsOff(bool @bool) => SetValue(@bool);

        /// <summary>
        /// Sets if an External Layer is plottable.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public ExternalLayer SetIsPlottable(bool @bool) => SetValue(@bool);

        /// <summary>
        /// Sets if an External Layer is reconciled.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public ExternalLayer SetIsReconciled(bool @bool) => SetValue(@bool);

        /// <summary>
        /// Sets the lineweight of an External Layer.
        /// </summary>
        /// <param name="lineweight"></param>
        /// <returns></returns>
        public ExternalLayer SetLineweight(string lineweight)
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

            return SetValue(lwEnum, "LineWeight");
        }

        /// <summary>
        /// Sets the transparency of an External Layer.
        /// </summary>
        /// <param name="transparency"></param>
        /// <returns></returns>
        public ExternalLayer SetTransparency(double transparency)
        {
            if (transparency < 0.0 || transparency > 100.0)
            {
                throw new InvalidOperationException("Transparency must be between [0,100].");
            }

            using (var tr = AcDatabase.TransactionManager.StartTransaction())
            {
                AcLayerTable layerTable = tr.GetObject(
                    AcDatabase.LayerTableId,
                    acDb.OpenMode.ForRead) as AcLayerTable;
                if (layerTable != null && layerTable.Has(this.Name))
                {
                    layerTable.UpgradeOpen();
                    (tr.GetObject(layerTable[this.Name], acDb.OpenMode.ForWrite) as AcLayer).Transparency 
                        = new acColor.Transparency((byte)((double)byte.MaxValue * (100.0 - transparency) / 100.0));
                    layerTable.DowngradeOpen();
                }
            }
            return this;
        }

        /// <summary>
        /// Sets the Plot Style of an External Layer.
        /// </summary>
        /// <param name="plotStyleName"></param>
        /// <returns></returns>
        public ExternalLayer SetPlotStyle(string plotStyleName) => SetValue((object)plotStyleName, "PlotStyleName");

        /// <summary>
        /// Sets if an External Layer is frozen by default in new viewports.
        /// </summary>
        /// <param name="bool"></param>
        /// <returns></returns>
        public ExternalLayer SetViewportVisibilityDefault(bool @bool) => SetValue(@bool);
        #endregion
    }
}
