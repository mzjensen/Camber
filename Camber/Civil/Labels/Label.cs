#region references
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acGeom = Autodesk.AutoCAD.Geometry;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using civDynNodes = Autodesk.Civil.DynamoNodes;
using AeccLabel = Autodesk.Civil.DatabaseServices.Label;
using Autodesk.DesignScript.Runtime;
using Autodesk.DesignScript.Geometry;
using DynamoServices;
using Camber.Utils;
using Camber.Civil.Styles.Labels;
using System.Text;
using Camber.Civil.Styles.Objects;
#endregion

namespace Camber.Civil.Labels
{
    [RegisterForTrace]
    public class Label : CivilObject
    {
        #region notes
        // Label location and dragged offset is a bit confusing. See this page for an explanation.
        // https://civilizeddevelopment.typepad.com/civilized-development/labels/
        #endregion

        #region properties
        internal AeccLabel AeccLabel => AcObject as AeccLabel;

        /// <summary>
        /// Gets whether anchor markers are supported by a Label.
        /// </summary>
        public bool AllowsAnchorMarker => GetBool();

        /// <summary>
        /// Gets whether dimension anchors are supported by a Label.
        /// </summary>
        public bool AllowsDimensionAnchors => GetBool();

        /// <summary>
        /// Gets whether a Label can be dragged.
        /// </summary>
        public bool AllowsDragging => GetBool();

        /// <summary>
        /// Gets whether a Label can be flipped.
        /// </summary>
        public bool AllowsFlipping => GetBool();

        /// <summary>
        /// Gets whether leader attachment options are supported by a Label.
        /// </summary>
        public bool AllowsLeaderAttachment => GetBool();

        /// <summary>
        /// Gets whether a Label can be pinned.
        /// </summary>
        public bool AllowsPinning => GetBool();

        /// <summary>
        /// Gets whether a Label can be reversed.
        /// </summary>
        public bool AllowsReversing => GetBool();

        /// <summary>
        /// Gets a Label's auto stagger type.
        /// </summary>
        public string AutoStagger => GetString();

        /// <summary>
        /// Gets a Label's mask type.
        /// </summary>
        public string Mask => GetString();

        /// <summary>
        /// Gets a Label's rotation type, indicating whether the rotation is due to being dragged, settings in the Label composer, or both.
        /// </summary>
        public string RotationType => GetString();

        /// <summary>
        /// Gets a Label's anchor point.
        /// </summary>
        public Point AnchorPoint => GeometryConversions.AcPointToDynPoint(AeccLabel.AnchorInfo.Location);

        /// <summary>
        /// Gets a Label's anchor marker rotation angle.
        /// </summary>
        public double AnchorMarkerRotationAngle => GetDouble();

        /// <summary>
        /// Gets a Label's anchor Marker Style.
        /// </summary>
        public MarkerStyle AnchorMarkerStyle => (MarkerStyle)MarkerStyle.GetByObjectId(AeccLabel.AnchorMarkerStyleId);

        /// <summary>
        /// Gets whether a Label can be rotated.
        /// </summary>
        public bool CanRotate => GetBool();

        /// <summary>
        /// Gets a Label's dimension anchor option.
        /// </summary>
        public string DimensionAnchorOption => GetString();

        /// <summary>
        /// Gets a Label's dimension anchor value.
        /// </summary>
        public double DimensionAnchorValue => GetDouble();

        /// <summary>
        /// Gets whether a Label has been dragged.
        /// </summary>
        public bool IsDragged => GetBool("Dragged");

        /// <summary>
        /// Gets the dragged offset of a Label.
        /// </summary>
        public Vector DraggedOffset => Vector.ByCoordinates(AeccLabel.DraggedOffset.X, AeccLabel.DraggedOffset.Y, AeccLabel.DraggedOffset.Z);

        /// <summary>
        /// Gets the flipped state of a Label.
        /// </summary>
        public bool IsFlipped => GetBool("Flipped");

        /// <summary>
        /// Gets the location of a Label.
        /// </summary>
        public Point Location => GeometryConversions.AcPointToDynPoint(AeccLabel.LabelLocation);

        /// <summary>
        /// Gets a Label's leader attachment option.
        /// </summary>
        public string LeaderAttachment => GetString();

        /// <summary>
        /// Gets a Label's leader tail visibility option.
        /// </summary>
        public string LeaderTailVisibility => GetString();

        /// <summary>
        /// Gets a Label's leader visibility option.
        /// </summary>
        public string LeaderVisibility => GetString();

        /// <summary>
        /// Gets the pinned state of a Label.
        /// </summary>
        public bool IsPinned => GetBool("Pinned");

        /// <summary>
        /// Gets the reversed state of a Label.
        /// </summary>
        public bool IsReversed => GetBool("Reversed");

        /// <summary>
        /// Gets the rotation angle of a Label.
        /// </summary>
        public double Rotation => MathUtils.RadiansToDegrees(GetDouble("RotationAngle"));

        /// <summary>
        /// Gets all of the Text, Reference Text, and Text For Each Label Style Components for a Label.
        /// </summary>
        public List<LabelStyleComponent> TextComponents => GetTextComponents();

        /// <summary>
        /// Gets the text contents of a Label.
        /// </summary>
        public string TextContents => GetDisplayedLabelText();
        #endregion

        #region constructors
        internal Label(AeccLabel aeccLabel, bool isDynamoOwned = false) : base(aeccLabel, isDynamoOwned) { }

        [SupressImportIntoVM]
        internal static Label GetByObjectId(acDb.ObjectId labelId)
            => CivilObjectSupport.Get<Label, AeccLabel>
            (labelId, (label) => new Label(label));

        /// <summary>
        /// Returns a Label from Dynamo Object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static Label ByObject(acDynNodes.Object obj)
        {
            var id = obj.InternalObjectId;
            return GetByObjectId(id);
        }
        #endregion

        #region methods
        public override string ToString() => $"Label";

        /// <summary>
        /// Clears all text component overrides in a Label.
        /// </summary>
        /// <returns></returns>
        public Label ClearAllTextComponentOverrides()
        {
            try
            {
                bool openedForWrite = AeccEntity.IsWriteEnabled;
                if (!openedForWrite) AeccEntity.UpgradeOpen();
                AeccLabel.ClearAllTextComponentOverrides();
                if (!openedForWrite) AeccEntity.DowngradeOpen();
            }
            catch { }
            return this;
        }
        
        /// <summary>
        /// Resets a Label to its default.
        /// </summary>
        /// <returns></returns>
        public Label Reset()
        {
            try
            {
                bool openedForWrite = AeccEntity.IsWriteEnabled;
                if (!openedForWrite) AeccEntity.UpgradeOpen();
                AeccLabel.Reset();
                if (!openedForWrite) AeccEntity.DowngradeOpen();
            }
            catch { }
            return this;
        }

        /// <summary>
        /// Resets the location of a Label to its default.
        /// </summary>
        /// <returns></returns>
        public Label ResetLocation()
        {
            try
            {
                bool openedForWrite = AeccEntity.IsWriteEnabled;
                if (!openedForWrite) AeccEntity.UpgradeOpen();
                AeccLabel.ResetLocation();
                if (!openedForWrite) AeccEntity.DowngradeOpen();
            }
            catch { }
            return this;
        }

        /// <summary>
        /// Resets the properties of a Label to their defaults.
        /// </summary>
        /// <returns></returns>
        public Label ResetProperties()
        {
            try
            {
                bool openedForWrite = AeccEntity.IsWriteEnabled;
                if (!openedForWrite) AeccEntity.UpgradeOpen();
                AeccLabel.ResetProperties();
                if (!openedForWrite) AeccEntity.DowngradeOpen();
            }
            catch { }
            return this;
        }

        /// <summary>
        /// Sets the dragged offset of a Label.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Label SetDraggedOffset(Vector offset)
        {
            acGeom.Vector3d acVector = new acGeom.Vector3d(offset.X, offset.Y, offset.Z);
            acGeom.Point3d newLocation = AeccLabel.AnchorInfo.Location + acVector;
            SetValue(newLocation, "LabelLocation");
            return this;
        }

        /// <summary>
        /// Sets the location of a Label.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Label SetLocation(Point point)
        {
            acGeom.Point3d acPoint = new acGeom.Point3d(point.X, point.Y, point.Z);
            SetValue(acPoint, "LabelLocation");
            return this;
        }

        /// <summary>
        /// Sets the rotation angle of a Label.
        /// </summary>
        /// <param name="angle">Angle (degrees) in the range of [0,360).</param>
        /// <returns></returns>
        public Label SetRotation(double angle)
        {
            SetValue(MathUtils.DegreesToRadians(angle), "RotationAngle");
            return this;
        }

        /// <summary>
        /// Attempts to get a list of the Text, TextForEach, and ReferenceText Components for a Label.
        /// </summary>
        /// <returns></returns>
        private List<LabelStyleComponent> GetTextComponents()
        {
            List<LabelStyleComponent> components = new List<LabelStyleComponent>();
            acDb.ObjectIdCollection compIds = AeccLabel.GetTextComponentIds();
            foreach (acDb.ObjectId compId in compIds)
            {
                components.Add(LabelStyleComponent.GetByObjectId(compId));
            }
            return components;
        }

        /// <summary>
        /// Gets the actual label text that is displayed on-screen, not what is given in the label style.
        /// </summary>
        /// <param name="labelId"></param>
        /// <returns></returns>
        private string GetDisplayedLabelText()
        {
            // Credit to Norman Yuan at https://drive-cad-with-code.blogspot.com/2016/11/extract-autocad-civil3ds-label-text.html

            acDb.ObjectId labelId = AeccLabel.Id;

            //if (labelId.ObjectClass.DxfName.ToUpper() != "AECC_GENERAL_SEGMENT_LABEL")
            //{
            //    throw new ArgumentException("Label is not of type \"AECC_GENERAL_SEGMENT_LABEL\".");
            //}

            StringBuilder labelText = new StringBuilder();

            acDynNodes.Document document = acDynNodes.Document.Current;

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                var aeccLabel = labelId.GetObject(acDb.OpenMode.ForRead) as AeccLabel;
                if (aeccLabel != null)
                {
                    bool changed = !aeccLabel.Dragged && aeccLabel.AllowsDragging;
                    try
                    {
                        if (changed)
                        {
                            aeccLabel.UpgradeOpen();
                            double delta = aeccLabel.StartPoint.DistanceTo(aeccLabel.EndPoint);
                            aeccLabel.LabelLocation =
                                new acGeom.Point3d(aeccLabel.LabelLocation.X +
                                    delta, aeccLabel.LabelLocation.Y +
                                    delta, aeccLabel.LabelLocation.Z);
                        }

                        var dbObjs = FullExplode(aeccLabel);
                        foreach (var obj in dbObjs)
                        {
                            if (obj.GetType() == typeof(acDb.DBText))
                            {
                                labelText.Append(" " + (obj as acDb.DBText).TextString);
                            }
                            obj.Dispose();
                        }
                    }
                    finally
                    {
                        if (changed) aeccLabel.ResetLocation();
                    }
                }
            }
            return labelText.ToString().Trim();
        }

        /// <summary>
        /// Recursively explode an object. Used for exploding labels down to primitive components.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private static List<acDb.DBObject> FullExplode(acDb.Entity entity)
        {
            // Final result
            List<acDb.DBObject> fullList = new List<acDb.DBObject>();

            // First explode of the entity
            acDb.DBObjectCollection explodedObjects = new acDb.DBObjectCollection();
            entity.Explode(explodedObjects);
            foreach (acDb.Entity explodedObj in explodedObjects)
            {
                // If the exploded entity is a Block Reference or MText, then explode again
                if (explodedObj.GetType() == typeof(acDb.BlockReference) ||
                    explodedObj.GetType() == typeof(acDb.MText))
                {
                    fullList.AddRange(FullExplode(explodedObj));
                }
                else
                {
                    fullList.Add(explodedObj);
                }   
            }
            return fullList;
        }

        /// <summary>
        /// Gets the overridden text in a specified text component.
        /// </summary>
        /// <param name="labelStyleComponent">Text, Text For Each, or Reference Text component</param>
        /// <returns></returns>
        public string GetTextComponentOverride(LabelStyleComponent labelStyleComponent)
        {
            return AeccLabel.GetTextComponentOverride(labelStyleComponent.InternalObjectId);
        }

        /// <summary>
        /// Gets whether the specified text component is overridden.
        /// </summary>
        /// <param name="labelStyleComponent"></param>
        /// <returns></returns>
        public bool IsTextComponentOverridden(LabelStyleComponent labelStyleComponent)
        {
            // The spelling is wrong in the API method
            return AeccLabel.IsTextComponentOverriden(labelStyleComponent.InternalObjectId);
        }

        /// <summary>
        /// Gets the overidden justification of the specified text component.
        /// </summary>
        /// <param name="labelStyleComponent"></param>
        /// <returns></returns>
        public string GetTextComponentJustificationOverride(LabelStyleComponent labelStyleComponent)
        {
            return AeccLabel.GetTextComponentJustificationOverride(labelStyleComponent.InternalObjectId).ToString();
        }

        /// <summary>
        /// Sets the target Civil Object of a reference text component.
        /// </summary>
        /// <param name="labelStyleComponent"></param>
        /// <param name="civilObject"></param>
        /// <returns></returns>
        public Label SetReferenceTextTarget(LabelStyleComponent labelStyleComponent, civDynNodes.CivilObject civilObject)
        {
            try
            {
                bool openedForWrite = AeccEntity.IsWriteEnabled;
                if (!openedForWrite) AeccEntity.UpgradeOpen();
                AeccLabel.SetReferenceTextTarget(labelStyleComponent.InternalObjectId, civilObject.InternalObjectId);
                if (!openedForWrite) AeccEntity.DowngradeOpen();
            }
            catch { }
            return this;
        }

        /// <summary>
        /// Overrides the text of a specified text component.
        /// </summary>
        /// <param name="labelStyleComponent"></param>
        /// <param name="textOverride"></param>
        /// <returns></returns>
        public Label SetTextComponentOverride(LabelStyleComponent labelStyleComponent, string textOverride)
        {
            try
            {
                bool openedForWrite = AeccEntity.IsWriteEnabled;
                if (!openedForWrite) AeccEntity.UpgradeOpen();
                AeccLabel.SetTextComponentOverride(labelStyleComponent.InternalObjectId, textOverride);
                if (!openedForWrite) AeccEntity.DowngradeOpen();
            }
            catch { }
            return this;
        }
    }
    #endregion
}