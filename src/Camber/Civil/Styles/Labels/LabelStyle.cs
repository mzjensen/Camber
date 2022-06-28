#region references
using System;
using System.Collections.Generic;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using civApp = Autodesk.Civil.ApplicationServices;
using civDb = Autodesk.Civil.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
using AeccLabelStyleCollection = Autodesk.Civil.DatabaseServices.Styles.LabelStyleCollection;
using Autodesk.DesignScript.Runtime;
using System.Reflection;
using DynamoServices;
#endregion

namespace Camber.Civil.Styles.Labels
{
    [RegisterForTrace]
    public class LabelStyle : Style
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;
        private const string DuplicateMessage = "A Label Style with that name and type already exists.";
        private const string NullNameMessage = "Name is null or empty.";
        private const string NullTypeMessage = "Type name is null or empty.";
        private const string NullCollectionMessage = "Collection name is null or empty.";

        /// <summary>
        /// Gets the number of child Label Styles under a Label Style.
        /// </summary>
        public int ChildrenCount => GetInt();

        /// <summary>
        /// Gets a Label Style's parent Label Style, if applicable.
        /// </summary>
        public LabelStyle ParentLabelStyle
        {
            get
            {
                var document = acDynNodes.Document.Current;
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument.Database))
                {
                    var aeccLabelStyle = ctx.Transaction.GetObject(InternalObjectId, acDb.OpenMode.ForWrite);
                    try
                    {
                        acDb.ObjectId parentId = AeccLabelStyle.ParentLabelStyleId;
                        var aeccParent = (AeccLabelStyle)ctx.Transaction.GetObject(parentId, acDb.OpenMode.ForWrite);
                        // Return new instance of input type
                        return (LabelStyle)Activator.CreateInstance(GetType(), BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { aeccParent, false }, null);
                    }
                    catch (InvalidOperationException)
                    {
                        throw new Exception("The Label Style has no parent.");
                    }            
                }
            }
        }

        #region behavior
        /// <summary>
        /// Gets the value that specifies the position of a label relative to an object.
        /// </summary>
        public string LabelInsertOption => AeccLabelStyle.Properties.Behavior.InsertOption.Value.ToString();

        /// <summary>
        /// Gets the value that specifies whether labels are placed inside or outside of a curve.
        /// </summary>
        public string LabelInsideCurveOption => AeccLabelStyle.Properties.Behavior.InsideCurveOption.Value.ToString();

        /// <summary>
        /// Gets the value that specifies the orientation reference of labels.
        /// </summary>
        public string LabelOrientationReference => AeccLabelStyle.Properties.Behavior.OrientationReference.Value.ToString();
        #endregion
        #region dragged state components
        /// <summary>
        /// Gets the shape of the border, either rounded or rectangular.
        /// </summary>
        public string DraggedStateBorderType => AeccLabelStyle.Properties.DraggedStateComponents.BorderType.Value.ToString();

        /// <summary>
        /// Gets whether a border is visible for dragged labels.
        /// </summary>
        public bool DraggedStateBorderIsVisible => AeccLabelStyle.Properties.DraggedStateComponents.BorderVisibility.Value;

        /// <summary>
        /// Gets the value that specifies how label content is displayed after it is dragged from its default position.
        /// </summary>
        public string DraggedStateDisplayType => AeccLabelStyle.Properties.DraggedStateComponents.DisplayType.ToString();

        /// <summary>
        /// Gets the distance between the leader and the label text.
        /// </summary>
        public double DraggedStateGap => AeccLabelStyle.Properties.DraggedStateComponents.Gap.Value;

        /// <summary>
        /// Gets the location where a leader hook is drawn in relation to label content.
        /// </summary>
        public string DraggedStateLeaderAttachment => AeccLabelStyle.Properties.DraggedStateComponents.LeaderAttachment.Value.ToString();

        /// <summary>
        /// Gets the boolean value that specifies the justification of label text in relation to the leader.
        /// </summary>
        public bool DraggedStateLeaderJustification => AeccLabelStyle.Properties.DraggedStateComponents.LeaderJustification.Value;

        /// <summary>
        /// Gets the linetype for dragged components.
        /// </summary>
        public string DraggedStateLinetype => AeccLabelStyle.Properties.DraggedStateComponents.Linetype.Value.ToString();

        /// <summary>
        /// Gets the maximum plotted width for all text components.
        /// </summary>
        public double DraggedStateMaxTextWidth => AeccLabelStyle.Properties.DraggedStateComponents.MaxTextWidth.Value;

        /// <summary>
        /// Gets the plotted height for all text components.
        /// </summary>
        public double DraggedStateTextHeight => AeccLabelStyle.Properties.DraggedStateComponents.TextHeight.Value;

        /// <summary>
        /// Gets whether to use background mask when dragging the label.
        /// </summary>
        public bool DraggedStateUseBackgroundMask => AeccLabelStyle.Properties.DraggedStateComponents.UseBackgroundMask.Value;
        #endregion
        #region label
        /// <summary>
        /// Gets the display mode for the Label Style.
        /// </summary>
        public string LabelDisplayMode => AeccLabelStyle.Properties.Label.DisplayMode.Value.ToString();

        /// <summary>
        /// Gets the default layer for the Label Style.
        /// </summary>
        public acDynNodes.Layer LabelLayer => acDynNodes.Document.Current.LayerByName(AeccLabelStyle.Properties.Label.Layer.Value.ToString());

        /// <summary>
        /// Gets the default text style for all text components in the Label Style.
        /// </summary>
        public string LabelTextStyle => AeccLabelStyle.Properties.Label.TextStyle.Value;

        /// <summary>
        /// Gets the visibility of the Label Style (independent of layer).
        /// </summary>
        public bool LabelIsVisible => AeccLabelStyle.Properties.Label.Visibility.Value;
        #endregion
        #region leader
        /// <summary>
        /// Gets the size of the leader arrowhead.
        /// </summary>
        public double LeaderArrowheadSize => AeccLabelStyle.Properties.Leader.ArrowheadSize.Value;

        /// <summary>
        /// Gets the style of the arrowhead for the leader.
        /// </summary>
        public string LeaderArrowheadStyle => AeccLabelStyle.Properties.Leader.ArrowheadStyle.Value;

        /// <summary>
        /// Gets the linetype for leaders in the Label Style.
        /// </summary>
        public string LeaderLinetype => AeccLabelStyle.Properties.Leader.Linetype.Value;

        /// <summary>
        /// Gets the value that specifies the shape of leaders in the Label Style.
        /// </summary>
        public string LeaderShape => AeccLabelStyle.Properties.Leader.Shape.Value.ToString();

        /// <summary>
        /// Gets whether leaders are visible when a label is dragged from its original position.
        /// </summary>
        public bool LeaderIsVisibile => AeccLabelStyle.Properties.Leader.Visibility.Value;
        #endregion
        #region plan readability
        /// <summary>
        /// Gets whether the Label should flip anchors with text.
        /// </summary>
        public bool LabelFlipAnchorsWithText => AeccLabelStyle.Properties.PlanReadability.FlipAnchorsWithText.Value;

        /// <summary>
        /// Gets whether all text components in labels can be read easily in plan view.
        /// </summary>
        public bool LabelIsPlanReadable => AeccLabelStyle.Properties.PlanReadability.PlanReadable.Value;

        /// <summary>
        /// Gets the angle at which label text flips 180 degrees to remain plan readable.
        /// </summary>
        public double LabelPlanReadableBias => AeccLabelStyle.Properties.PlanReadability.PlanReadableBias.Value;
        #endregion
        #endregion

        #region constructors
        internal LabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static LabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<LabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new LabelStyle(labelStyle));

        /// <summary>
        /// Creates a label style by name and type.
        /// </summary>
        /// <param name="labelStyleName">The name of the label style</param>
        /// <param name="labelStyleCollection">The name of the label style collection as it appears in the API, e.g. AlignmentLabelStyles.CurveLabelStyles</param>
        /// <param name="labelStyleType">The fully-qualified type name</param>
        /// <returns></returns>
        protected static Style CreateByNameType(string labelStyleName, string labelStyleCollection, string labelStyleType)
        {
            if (string.IsNullOrEmpty(labelStyleName)) { throw new ArgumentNullException(NullNameMessage); }
            if (string.IsNullOrEmpty(labelStyleCollection)) { throw new ArgumentNullException(NullCollectionMessage); }
            if (string.IsNullOrEmpty(labelStyleType)) { throw new ArgumentNullException(NullTypeMessage); }

            Type type = Type.GetType(labelStyleType);
            var document = acDynNodes.Document.Current;

            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                civApp.CivilDocument cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                var aeccCollection = Utilities.ReflectionUtilities.GetNestedProperty(cdoc.Styles.LabelStyles, labelStyleCollection, "Error");

                // Check for duplicate
                MethodInfo containsMethod = aeccCollection.GetType().GetMethod("Contains", new[] { typeof(string) });
                bool collectionContains = (bool)containsMethod.Invoke(aeccCollection, new string[] { labelStyleName });
                if (collectionContains) { throw new Exception(DuplicateMessage); }

                // Add new style to collection
                MethodInfo addMethod = aeccCollection.GetType().GetMethod("Add", new[] { typeof(string) });
                acDb.ObjectId styleId = (acDb.ObjectId)addMethod.Invoke(aeccCollection, new string[] { labelStyleName });

                var aeccLabelStyle = (AeccLabelStyle)styleId.GetObject(acDb.OpenMode.ForRead);
                if (aeccLabelStyle != null)
                {
                    return (Style)Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { aeccLabelStyle, false }, null);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets a Label Style by name and type.
        /// </summary>
        /// <param name="labelStyleName">The name of the label style</param>
        /// <param name="labelStyleRoot">The name of the root location as it appears in the API</param>
        /// <param name="labelStyleCollection">The name of the label style collection as it appears in the API</param>
        /// <param name="labelStyleType">The fully-qualified type name</param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        public static Style GetByNameType(string labelStyleName, string labelStyleCollection, string labelStyleType)
        {
            if (string.IsNullOrEmpty(labelStyleName)) { throw new ArgumentNullException(NullNameMessage); }
            if (string.IsNullOrEmpty(labelStyleCollection)) { throw new ArgumentNullException(NullCollectionMessage); }
            if (string.IsNullOrEmpty(labelStyleType)) { throw new ArgumentNullException(NullTypeMessage); }

            Type type = Type.GetType(labelStyleType);
            var document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                civApp.CivilDocument cdoc = civApp.CivilDocument.GetCivilDocument(ctx.Database);
                
                AeccLabelStyleCollection aeccCollection = 
                    (AeccLabelStyleCollection)Utilities.ReflectionUtilities.GetNestedProperty(
                        cdoc.Styles.LabelStyles, 
                        labelStyleCollection, 
                        "Error");
                
                var styleId = FindLabelStyleByName(aeccCollection, labelStyleName);
                
                //var itemProp = aeccCollection
                //    .GetType()
                //    .GetProperty("Item", new[] { typeof(string) });

                //var getMethod = itemProp.GetGetMethod();
                //acDb.ObjectId styleId = 
                //    (acDb.ObjectId)getMethod.Invoke(
                //        aeccCollection, 
                //        new string[] { labelStyleName });

                var aeccStyle = styleId.GetObject(acDb.OpenMode.ForRead);
                
                if (aeccStyle != null)
                {
                    return (Style)Activator.CreateInstance(
                        type, 
                        BindingFlags.Instance | BindingFlags.NonPublic, 
                        null, 
                        new object[] { aeccStyle, false }, 
                        null);
                }
                
                return null;
            }
        }

        /// <summary>
        /// Adds a new child Label Style to a Label Style with the default settings.
        /// The new child Label Style is returned.
        /// </summary>
        /// <param name="labelStyle"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public static LabelStyle AddChild(LabelStyle labelStyle, string childName)
        {
            if (string.IsNullOrEmpty(childName)) { throw new ArgumentNullException(NullNameMessage); }

            var document = acDynNodes.Document.Current;
            using (var ctx = new acDynApp.DocumentContext(document.AcDocument.Database))
            {
                var aeccLabelStyle = ctx.Transaction.GetObject(labelStyle.InternalObjectId, acDb.OpenMode.ForWrite);
                acDb.ObjectId newId = labelStyle.AeccLabelStyle.AddChild(childName);
                var newlabelStyle = (AeccLabelStyle)ctx.Transaction.GetObject(newId, acDb.OpenMode.ForWrite);
                // Return new instance of input type
                return (LabelStyle)Activator.CreateInstance(labelStyle.GetType(), BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { newlabelStyle, false }, null);
            }
        }
        #endregion

        #region methods
        public override string ToString() => $"LabelStyle(Name = {Name})";

        /// <summary>
        /// Removes all of a Label Style's descendants.
        /// </summary>
        /// <returns></returns>
        public LabelStyle RemoveAllDescendants()
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument.Database))
                {
                    var aeccLabelStyle = ctx.Transaction.GetObject(InternalObjectId, acDb.OpenMode.ForWrite);
                    AeccLabelStyle.RemoveAllDescendants();
                    return this;
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// Removes a child Label Style by name from a Label Style.
        /// </summary>
        /// <param name="childName"></param>
        /// <returns></returns>
        public LabelStyle RemoveChild(string childName)
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument.Database))
                {
                    var aeccLabelStyle = ctx.Transaction.GetObject(InternalObjectId, acDb.OpenMode.ForWrite);
                    AeccLabelStyle.RemoveChild(childName);
                    return this;
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// Removes a component by name from a Label Style.
        /// </summary>
        /// <param name="componentName"></param>
        /// <returns></returns>
        public LabelStyle RemoveComponent(string componentName)
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument.Database))
                {
                    var aeccLabelStyle = ctx.Transaction.GetObject(InternalObjectId, acDb.OpenMode.ForWrite);
                    AeccLabelStyle.RemoveComponent(componentName);
                    return this;
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// Sets the draw order of components in a Label Style. The order of the input list will be used.
        /// </summary>
        /// <param name="orderedComponents"></param>
        /// <returns></returns>
        public LabelStyle SetComponentsDrawOrder(List<LabelStyleComponent> orderedComponents)
        {
            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument.Database))
                {
                    var aeccLabelStyle = ctx.Transaction.GetObject(InternalObjectId, acDb.OpenMode.ForWrite);
                    var ids = new List<acDb.ObjectId>();
                    foreach (var comp in orderedComponents)
                    {
                        ids.Add(comp.InternalObjectId);
                    }
                    acDb.ObjectId[] idArray = ids.ToArray();
                    AeccLabelStyle.SetComponentsDrawOrder(idArray);
                    return this;
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// Gets the components of a certain type from a Label Style.
        /// </summary>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public IList<LabelStyleComponent> GetComponents(string componentType)
        {
            if (string.IsNullOrEmpty(componentType)) { throw new ArgumentNullException("Component type is null or empty."); }
            if(!Enum.IsDefined(typeof(civDb.Styles.LabelStyleComponentType), componentType)) { throw new ArgumentException("Invalid component type."); }
            var aeccComponentType = (civDb.Styles.LabelStyleComponentType)Enum.Parse(typeof(civDb.Styles.LabelStyleComponentType), componentType);

            var comps = new List<LabelStyleComponent>();
            try
            {
                foreach (acDb.ObjectId oid in AeccLabelStyle.GetComponents(aeccComponentType))
                {
                    comps.Add(LabelStyleComponent.GetByObjectId(oid));
                }
                return comps;
            }
            catch { throw; }
        }

        /// <summary>
        /// Adds a valid component to a Label Style.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public LabelStyle AddComponent(string name, string componentType)
        {
            if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException("Name is null or empty."); }
            if (string.IsNullOrEmpty(componentType)) { throw new ArgumentNullException("Component type is null or empty."); }
            if (!Enum.IsDefined(typeof(civDb.Styles.LabelStyleComponentType), componentType)) { throw new ArgumentException("Invalid component type."); }
            var aeccComponentType = (civDb.Styles.LabelStyleComponentType)Enum.Parse(typeof(civDb.Styles.LabelStyleComponentType), componentType);

            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccLabelStyle = ctx.Transaction.GetObject(InternalObjectId, acDb.OpenMode.ForWrite);
                    AeccLabelStyle.AddComponent(name, aeccComponentType);
                    return this;
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// Adds a valid reference text component to a Label Style.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public LabelStyle AddReferenceTextComponent(string name, string componentType)
        {
            if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException("Name is null or empty."); }
            if (string.IsNullOrEmpty(componentType)) { throw new ArgumentNullException("Component type is null or empty."); }
            if (!Enum.IsDefined(typeof(civDb.Styles.ReferenceTextComponentSelectedType), componentType)) { throw new ArgumentException("Invalid component type."); }
            var aeccComponentType = (civDb.Styles.ReferenceTextComponentSelectedType)Enum.Parse(typeof(civDb.Styles.ReferenceTextComponentSelectedType), componentType);

            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccLabelStyle = ctx.Transaction.GetObject(InternalObjectId, acDb.OpenMode.ForWrite);
                    AeccLabelStyle.AddReferenceTextComponent(name, aeccComponentType);
                    return this;
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// Adds a valid text for each component to a Label Style.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public LabelStyle AddTextForEachComponent(string name, string componentType)
        {
            if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException("Name is null or empty."); }
            if (string.IsNullOrEmpty(componentType)) { throw new ArgumentNullException("Component type is null or empty."); }
            if (!Enum.IsDefined(typeof(civDb.Styles.TextForEachComponentSelectedType), componentType)) { throw new ArgumentException("Invalid component type."); }
            var aeccComponentType = (civDb.Styles.TextForEachComponentSelectedType)Enum.Parse(typeof(civDb.Styles.TextForEachComponentSelectedType), componentType);

            var document = acDynNodes.Document.Current;
            try
            {
                using (var ctx = new acDynApp.DocumentContext(document.AcDocument))
                {
                    var aeccLabelStyle = ctx.Transaction.GetObject(InternalObjectId, acDb.OpenMode.ForWrite);
                    AeccLabelStyle.AddTextForEachComponent(name, aeccComponentType);
                    return this;
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// Gets a label style ID by name.
        /// </summary>
        /// <param name="labelStyleCollection"></param>
        /// <param name="styleName"></param>
        /// <returns></returns>
        private static acDb.ObjectId FindLabelStyleByName(AeccLabelStyleCollection labelStyleCollection, string styleName)
        {
            var retval = acDb.ObjectId.Null;
            foreach (acDb.ObjectId id in labelStyleCollection)
            {
                retval = ProcessChildren(id, styleName);
                if (retval != acDb.ObjectId.Null)
                    break;
            }
            return retval;
        }

        /// <summary>
        /// Returns the Object ID of a child label style by name if it exists, otherwise returns the Object ID of the parent. 
        /// </summary>
        /// <param name="parentStyleId"></param>
        /// <param name="styleName"></param>
        /// <returns></returns>
        private static acDb.ObjectId ProcessChildren(acDb.ObjectId parentStyleId, string styleName)
        {
            var retval = acDb.ObjectId.Null;
            AeccLabelStyle labelStyle = (AeccLabelStyle)parentStyleId.Open(acDb.OpenMode.ForRead);
            if (labelStyle.Name == styleName)
                retval = parentStyleId;
            else
                for (int i = 0; i < labelStyle.ChildrenCount; i++)
                {
                    retval = ProcessChildren(labelStyle[i], styleName);
                    if (retval != acDb.ObjectId.Null)
                        break;
                }
            labelStyle.Close();
            return retval;
        }
        #endregion
    }
}
