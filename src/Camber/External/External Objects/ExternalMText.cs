using System;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Camber.Utilities;
using Camber.Utilities.GeometryConversions;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acGeom = Autodesk.AutoCAD.Geometry;
using AcMText = Autodesk.AutoCAD.DatabaseServices.MText;

namespace Camber.External.ExternalObjects
{
    public sealed class ExternalMText : ExternalObject
    {
        #region properties
        internal AcMText AcMText => AcObject as AcMText;

        /// <summary>
        /// Gets the text contents of an External MText.
        /// </summary>
        public string Contents => AcMText.Contents;

        /// <summary>
        /// Gets the number of columns in an External MText.
        /// </summary>
        public int? Columns
        {
            get
            {
                try
                {
                    int count = AcMText.ColumnCount;
                    return count;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the defined height of an External MText.
        /// </summary>
        public double Height => AcMText.Height;

        /// <summary>
        /// Gets the defined width of an External MText.
        /// </summary>
        public double Width => AcMText.Width;

        /// <summary>
        /// Gets the text height of an External MText.
        /// </summary>
        public double TextHeight => AcMText.TextHeight;

        /// <summary>
        /// Gets the insertion point of an External MText.
        /// </summary>
        public Point InsertionPoint => GeometryConversions.AcPointToDynPoint(AcMText.Location);

        /// <summary>
        /// Gets the text style of an External MText.
        /// </summary>
        public string TextStyle => AcMText.TextStyleName;

        /// <summary>
        /// Gets the attachment settings of an External MText.
        /// </summary>
        public string Attachment => AcMText.Attachment.ToString();
        #endregion

        #region constructors
        [SupressImportIntoVM]
        internal static ExternalMText GetByObjectId(acDb.ObjectId oid)
            => Get<ExternalMText, AcMText>
            (oid, mtext => new ExternalMText(mtext));

        internal ExternalMText(AcMText acMtext) : base(acMtext) { }

        /// <summary>
        /// Creates an External MText.
        /// </summary>
        /// <param name="point">The insertion point of the MText.</param>
        /// <param name="contents">The text contents.</param>
        /// <param name="layer"></param>
        /// <param name="textHeight"></param>
        /// <param name="rotation"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        public static ExternalMText ByPoint(
            Point point,
            string contents,
            string layer,
            ExternalBlock block,
            double textHeight = 1.0,
            double rotation = 0.0)
        {
            if (string.IsNullOrEmpty(layer))
            {
                throw new InvalidOperationException("Layer name is null or empty.");
            }

            if (textHeight <= 0)
            {
                throw new InvalidOperationException("Text height must be greater than zero.");
            }
            
            var externalDoc = block.ExternalDocument;
            
            try
            {
                using (var tr = externalDoc.AcDatabase.TransactionManager.StartTransaction())
                {
                    var acMtext = new AcMText();
                    acMtext.SetDatabaseDefaults(externalDoc.AcDatabase);
                    ExternalDocument.EnsureLayer(externalDoc, layer);
                    acMtext.TextHeight = textHeight;
                    acMtext.Rotation = MathUtilities.DegreesToRadians(rotation);
                    acMtext.Location = (acGeom.Point3d)GeometryConversions.DynPointToAcPoint(point);
                    acMtext.Layer = layer;
                    acMtext.Contents = contents;

                    var btr = (acDb.BlockTableRecord)tr.GetObject(block.InternalObjectId, acDb.OpenMode.ForWrite);
                    btr.AppendEntity(acMtext);
                    tr.AddNewlyCreatedDBObject(acMtext, true);
                    tr.Commit();
                    return new ExternalMText(acMtext);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"ExternalMText(Contents = {Contents})";

        /// <summary>
        /// Sets the contents of External MText.
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        public ExternalMText SetContents(string contents) => (ExternalMText) SetValue(contents);

        /// <summary>
        /// Sets the text height of External MText.
        /// </summary>
        /// <param name="textHeight"></param>
        /// <returns></returns>
        public ExternalMText SetTextHeight(double textHeight) => (ExternalMText)SetValue(textHeight);

        /// <summary>
        /// Sets the defined height of External MText.
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        public ExternalMText SetHeight(double height) => (ExternalMText) SetValue(height);

        /// <summary>
        /// Sets the defined width of External MText.
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        public ExternalMText SetWidth(double width) => (ExternalMText) SetValue(width);

        /// <summary>
        /// Sets the text style for External MText.
        /// </summary>
        /// <param name="textStyleName">The name of the text style to assign.</param>
        /// <returns></returns>
        public ExternalMText SetTextStyle(string textStyleName)
        {
            if (string.IsNullOrEmpty(textStyleName))
            {
                throw new InvalidOperationException("Text style name is null or empty.");
            }

            try
            {
                using (var tr = AcDatabase.TransactionManager.StartTransaction())
                {
                    var textStyleTbl = (acDb.TextStyleTable)tr.GetObject(
                        AcDatabase.TextStyleTableId,
                        acDb.OpenMode.ForRead);
                    if (!textStyleTbl.Has(textStyleName))
                    {
                        throw new InvalidOperationException("Text style does not exist.");
                    }

                    var styleId = textStyleTbl[textStyleName];
                    var acMText = (AcMText)tr.GetObject(InternalObjectId, acDb.OpenMode.ForWrite);
                    acMText.TextStyleId = styleId;
                    tr.Commit();
                }
                return this;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

        }

        /// <summary>
        /// Sets the attachment settings for an External MText. It is recommended to use the out-of-the-box dropdown nodes for MText attachment modes.
        /// </summary>
        /// <param name="horizontalAttachment"></param>
        /// <param name="verticalAttachment"></param>
        /// <returns></returns>
        public ExternalMText SetAttachment(string verticalAttachment, string horizontalAttachment)
        {
            var joinedAttachment = verticalAttachment + horizontalAttachment;
            try
            {
                using (var tr = AcDatabase.TransactionManager.StartTransaction())
                {
                    var acMText = (AcMText)tr.GetObject(InternalObjectId, acDb.OpenMode.ForWrite);
                    var parsedAttachPnt = (acDb.AttachmentPoint)Enum.Parse(typeof(acDb.AttachmentPoint), joinedAttachment, true);
                    acMText.Attachment = parsedAttachPnt;
                    tr.Commit();
                }
                return this;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
        #endregion
    }
}
