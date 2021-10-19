#region references
using System;
using System.Runtime.CompilerServices;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using acDynApp = Autodesk.AutoCAD.DynamoApp.Services;
using acDynNodes = Autodesk.AutoCAD.DynamoNodes;
using AeccLabelStyleComponent = Autodesk.Civil.DatabaseServices.Styles.LabelStyleComponent;
using AeccLabelStyleComponentType = Autodesk.Civil.DatabaseServices.Styles.LabelStyleComponentType;
#endregion


namespace Camber.Civil.Styles.Labels
{
    public class LabelStyleComponent : acDynNodes.ObjectBase
    {
        #region properties
        internal AeccLabelStyleComponent AeccLabelStyleComponent => AcObject as AeccLabelStyleComponent;
        internal AeccLabelStyleComponentType Type { get; set; }
        protected const string NotApplicableMessage = "Property is not applicable for this component.";

        // General settings. Common to most all LabeStyleComponent types.
        public string Name => GetString(ComponentSettings.General);
        public string AnchorComponent => GetString(ComponentSettings.General);
        public string ReferenceTextObjectType => GetString(ComponentSettings.General);
        public string EndAnchorComponent => GetString(ComponentSettings.General);
        public string StartAnchorComponent => GetString(ComponentSettings.General);
        public string DisplayMode => GetString(ComponentSettings.General, "UsedIn");
        public bool UseEndPointAnchor => GetBool(ComponentSettings.General);
        public bool IsVisible => GetBool(ComponentSettings.General, "Visibility");
        public bool SpanOutsideSegments => GetBool(ComponentSettings.General);

        // Block settings. Unique to LabelStyleBlockComponent type.
        public string BlockAttachment => GetString(ComponentSettings.Block, "Attachment");
        public string BlockLinetype => GetString(ComponentSettings.Block, "Linetype");
        public double BlockHeight => GetDouble(ComponentSettings.Block);
        public double BlockRotationAngle => GetDouble(ComponentSettings.Block, "RotationAngle");
        public double BlockXOffset => GetDouble(ComponentSettings.Block, "XOffset");
        public double BlockYOffset => GetDouble(ComponentSettings.Block, "YOffset");
        public acDynNodes.Block Block => acDynNodes.AutoCADUtility.GetBlockByName(GetString(ComponentSettings.Block, "BlockName"), acDynNodes.Document.Current.AcDocument);

        // Direction Arrow settings. Unique to LabelStyleDirectionArrowComponent type.
        public string ArrowLinetype => GetString(ComponentSettings.DirectionArrow, "Linetype");
        public string ArrowheadStyle => GetString(ComponentSettings.DirectionArrow);
        public bool ArrowFixedLength => GetBool(ComponentSettings.DirectionArrow, "FixedLength");
        public double ArrowLengthOrMinimumLength => GetDouble(ComponentSettings.DirectionArrow, "LengthOrMinimumLength");
        public double ArrowheadSize => GetDouble(ComponentSettings.DirectionArrow);
        public double ArrowRotationAngle => GetDouble(ComponentSettings.DirectionArrow, "RotationAngle");
        public double ArrowXOffset => GetDouble(ComponentSettings.DirectionArrow, "XOffset");
        public double ArrowYOffset => GetDouble(ComponentSettings.DirectionArrow, "YOffset");

        // Line settings. Unique to LabelStyleLineComponent type.
        public string LineLengthType => GetString(ComponentSettings.Line, "LengthType");
        public double LineAngle => GetDouble(ComponentSettings.Line, "Angle");
        public double LineEndPointXOffset => GetDouble(ComponentSettings.Line, "EndPointXOffset");
        public double LineEndPointYOffset => GetDouble(ComponentSettings.Line, "EndPointYOffset");
        public double LineFixedLength => GetDouble(ComponentSettings.Line, "FixedLength");
        public double LineStartPointXOffset => GetDouble(ComponentSettings.Line, "StartPointXOffset");
        public double LineStartPointYOffset => GetDouble(ComponentSettings.Line, "StartPointYOffset");

        // Border settings. Common to Text, Reference Text, and Text For Each.
        public string BorderType => GetString(ComponentSettings.Border);
        public string BorderLinetype => GetString(ComponentSettings.Border, "Linetype");
        public double BorderGap => GetDouble(ComponentSettings.Border, "Gap");
        public bool BorderIsVisible => GetBool(ComponentSettings.Border, "Visible");
        public bool BorderBackgroundMask => GetBool(ComponentSettings.Border, "BackgroundMask");
        
        // Text settings. Common for Text, Reference Text, and Text For Each.
        public string TextAttachment => GetString(ComponentSettings.Text, "Attachment");
        public string TextContents => GetString(ComponentSettings.Text, "Contents");
        public double TextAngle => GetDouble(ComponentSettings.Text, "Angle");
        public double TextHeight => GetDouble(ComponentSettings.Text, "Height");
        public double TextMaxWidth => GetDouble(ComponentSettings.Text, "MaxWidth");
        public double TextXOffset => GetDouble(ComponentSettings.Text, "XOffset");
        public double TextYOffset => GetDouble(ComponentSettings.Text, "YOffset");

        // Tick settings. Unique to LabelStyleTickComponent type.
        public string TickLinetype => GetString(ComponentSettings.Tick, "Linetype");
        public bool TickAlignWithObject => GetBool(ComponentSettings.Tick, "AlignWithObject");
        public double TickBlockHeight => GetDouble(ComponentSettings.Tick, "BlockHeight");
        public double TickRotationAngle => GetDouble(ComponentSettings.Tick, "RotationAngle");
        public acDynNodes.Block TickBlock => acDynNodes.AutoCADUtility.GetBlockByName(GetString(ComponentSettings.Tick, "BlockName"), acDynNodes.Document.Current.AcDocument);
        #endregion

        #region constructors
        internal static LabelStyleComponent GetByObjectId(acDb.ObjectId componentId)
        {
            acDynNodes.Document document = acDynNodes.Document.Current;
            using (acDynApp.DocumentContext ctx = new acDynApp.DocumentContext(document.AcDocument))
            {
                var aeccObject = (AeccLabelStyleComponent)ctx.Transaction.GetObject(componentId, acDb.OpenMode.ForWrite);
                string className = aeccObject.GetType().Name;
                className = className.Replace("LabelStyle", "");
                className = className.Replace("Component", "");
                return new LabelStyleComponent(aeccObject, (AeccLabelStyleComponentType)Enum.Parse(typeof(AeccLabelStyleComponentType), className));
            }
        }

        internal LabelStyleComponent(
            AeccLabelStyleComponent aeccLabelStyleComponent, 
            AeccLabelStyleComponentType type, 
            bool isDynamoOwned = false) 
            : base(aeccLabelStyleComponent, isDynamoOwned)
        {
            Type = type;
        }
        #endregion

        #region methods
        public override string ToString() => $"LabelStyleComponent(Name = {Name}, Type = {Type})";

        private static object GetPropertyValue(object src, string propName)
        {
            if (src == null) throw new ArgumentException("Value cannot be null.", "src");
            if (propName == null) throw new ArgumentException("Value cannot be null.", "propName");

            if (propName.Contains("."))//complex type nested
            {
                var temp = propName.Split(new char[] { '.' }, 2);
                return GetPropertyValue(GetPropertyValue(src, temp[0]), temp[1]);
            }
            else
            {
                var prop = src.GetType().GetProperty(propName);
                return prop != null ? prop.GetValue(src, null) : null;
            }
        }

        protected double GetDouble(ComponentSettings settingsGroup, [CallerMemberName] string propertyName = null)
        {
            try
            {
                return (double)GetPropertyValue(AeccLabelStyleComponent, settingsGroup.ToString() + "." + propertyName + ".Value");

            }
            catch { }
            //return double.NaN;
            throw new Exception(NotApplicableMessage);
        }

        protected string GetString(ComponentSettings settingsGroup, [CallerMemberName] string propertyName = null)
        {
            try
            {
                if (propertyName == "Name")
                {
                    return (string)GetPropertyValue(AeccLabelStyleComponent, "General.Name");
                }

                return (string)GetPropertyValue(AeccLabelStyleComponent, settingsGroup.ToString() + "." + propertyName + ".Value");

            }
            catch { }
            //return "Not applicable";
            throw new Exception(NotApplicableMessage);
        }

        protected bool GetBool(ComponentSettings settingsGroup, [CallerMemberName] string propertyName = null)
        {
            try
            {
                return (bool)GetPropertyValue(AeccLabelStyleComponent, settingsGroup.ToString() + "." + propertyName + ".Value");
            }
            catch { }
            //return false;
            throw new Exception(NotApplicableMessage);
        }
        #endregion
    }
}
