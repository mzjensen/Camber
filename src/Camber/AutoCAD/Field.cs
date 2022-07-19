using Autodesk.AutoCAD.DynamoNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using AcField = Autodesk.AutoCAD.DatabaseServices.Field;

namespace Camber.AutoCAD
{
    public sealed class Field : ObjectBase
    {
        #region properties
        internal AcField AcField => AcObject as AcField;

        /// <summary>
        /// Gets the expression of a Field.
        /// </summary>
        public string Expression => AcField.GetFieldCode(acDb.FieldCodeFlags.AddMarkers | acDb.FieldCodeFlags.FieldCode);

        /// <summary>
        /// Gets the data type of a Field.
        /// </summary>
        public string DataType => AcField.DataType.ToString();

        /// <summary>
        /// Gets the evaluation option explicitly set for a Field.
        /// </summary>
        public string EvaluationOption => AcField.EvaluationOption.ToString();

        /// <summary>
        /// Gets the output format set in a Field.
        /// </summary>
        public string Format => AcField.Format;

        /// <summary>
        /// Gets the Objects that are referenced by a Field.
        /// </summary>
        public List<acDynNodes.Object> ReferencedObjects
        {
            get
            {
                var objs = new List<acDynNodes.Object>();
                var code = AcField.GetFieldCode();

                using (var ctx = new acDynApp.DocumentContext(acDynNodes.Document.Current.AcDocument))
                {
                    while(code != "")
                    {
                        code = ExtractObjectId(code, false, out acDb.ObjectId oid);

                        if (code == "")
                        {
                            continue;
                        }

                        // Skip duplicates
                        if (objs.Any(x => x.InternalObjectId == oid))
                        {
                            continue;
                        }

                        var acObj = ctx.Transaction.GetObject(oid, acDb.OpenMode.ForRead);
                        var dynObj = SelectionByQuery.GetObjectByObjectHandle(acObj.Handle.ToString());
                        objs.Add(Objects.Object.ConvertToCamberObject(dynObj));
                    }
                }
                return objs;
            }
        }

        /// <summary>
        /// Gets the state of a Field.
        /// </summary>
        public string State => AcField.State.ToString();

        /// <summary>
        /// Gets the evaluation result of a Field in its original data type.
        /// </summary>
        public object Value => AcField.Value;

        /// <summary>
        /// Gets the value of a Field as a formatted string.
        /// </summary>
        public string ValueFormatted => AcField.GetStringValue();
        #endregion

        #region constructors
        internal Field(AcField acField, bool isDynamoOwned = false) : base(acField, isDynamoOwned)
        {
        }
        #endregion

        #region public methods
        public override string ToString() => $"{nameof(Field)}({nameof(DataType)} = {DataType}, Value = {ValueFormatted})";

        /// <summary>
        /// Gets the list of Fields embedded within a Field, with the option to recursively get all embedded Fields.
        /// </summary>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public List<Field> GetEmbeddedFields(bool recursive = false)
        {
            var childFields = new List<Field>();

            if (!AcField.GetChildren().Any())
            {
                return childFields;
            }
            if (recursive)
            {
                GetChildFieldsRecursive(AcField, childFields);
            }
            else
            {
                foreach (var child in AcField.GetChildren())
                {
                    childFields.Add(new Field(child));
                }
            }

            return childFields;
        }

        /// <summary>
        /// Sets the expression of a Field. It is up to the user to identify the correct field expression syntax to use.
        /// </summary>
        /// <param name="fieldExpression"></param>
        /// <returns></returns>
        public Field SetExpression(string fieldExpression)
        {
            var doc = acDynNodes.Document.Current.AcDocument;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(doc))
                {
                    AcField.UpgradeOpen();
                    AcField.SetFieldCode(fieldExpression);
                    AcField.Evaluate();
                    AcField.DowngradeOpen();
                    return this;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Sets the output format for a Field. It is up to the user to identify the correct format string syntax to use.
        /// </summary>
        /// <param name="formatString"></param>
        /// <returns></returns>
        public Field SetFormat(string formatString)
        {
            var doc = acDynNodes.Document.Current.AcDocument;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(doc))
                {
                    AcField.UpgradeOpen();
                    AcField.Format = formatString;
                    AcField.Evaluate();
                    AcField.DowngradeOpen();
                    return this;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }
        #endregion

        #region private methods
        /// <summary>
        /// Recursively gets child fields from a parent field.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="fields"></param>
        private static void GetChildFieldsRecursive(AcField parent, List<Field> fields)
        {
            if (!parent.GetChildren().Any())
            {
                return;
            }
            foreach (var child in parent.GetChildren())
            {
                fields.Add(new Field(child));
                GetChildFieldsRecursive(child, fields);
            }
        }

        /// <summary>
        /// Locates and extracts an Object ID from a Field's code if it exists.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        private static string ExtractObjectId(string code, bool searchForFieldPointers, out acDb.ObjectId oid)
        {
            var prefix = "%<\\_ObjId ";
            const string fldPtrPrefix = "%<\\_FldPtr ";
            const string suffix = ">%";

            if (code.Contains(fldPtrPrefix) && searchForFieldPointers)
            {
                prefix = fldPtrPrefix;
            }
            
            var prefixIndex = code.IndexOf(prefix, StringComparison.Ordinal);

            if (prefixIndex > 0)
            {
                var oidIndex = prefixIndex + prefix.Length;
                var remainder = code.Substring(oidIndex);
                var suffixIndex = remainder.IndexOf(suffix, StringComparison.Ordinal);
                var oidString = remainder.Remove(suffixIndex);
                oid = new acDb.ObjectId(new IntPtr(Convert.ToInt64(oidString)));
                return remainder.Substring(suffixIndex + suffix.Length);
            }
            oid = acDb.ObjectId.Null;
            return "";
        }
        #endregion
    }
}
