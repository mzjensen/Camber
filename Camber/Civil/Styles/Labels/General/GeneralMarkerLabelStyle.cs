#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels.General
{
    public sealed class GeneralMarkerLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal GeneralMarkerLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static GeneralMarkerLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<GeneralMarkerLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new GeneralMarkerLabelStyle(labelStyle));

        /// <summary>
        /// Creates a General Marker Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GeneralMarkerLabelStyle ByName(string name)
        {
            return (GeneralMarkerLabelStyle)CreateByNameType(
                name, 
                GeneralLabelStyles.GeneralMarkerLabelStyles.ToString(),  
                typeof(GeneralMarkerLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"GeneralMarkerLabelStyle(Name = {Name})";
        #endregion
    }
}
