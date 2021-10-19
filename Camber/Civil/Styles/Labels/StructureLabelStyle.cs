#region references
using acDb = Autodesk.AutoCAD.DatabaseServices;
using AeccLabelStyle = Autodesk.Civil.DatabaseServices.Styles.LabelStyle;
#endregion

namespace Camber.Civil.Styles.Labels
{
    public sealed class StructureLabelStyle : LabelStyle
    {
        #region properties
        internal AeccLabelStyle AeccLabelStyle => AcObject as AeccLabelStyle;

        #endregion

        #region constructors
        internal StructureLabelStyle(AeccLabelStyle aeccLabelStyle, bool isDynamoOwned = false) : base(aeccLabelStyle, isDynamoOwned) { }

        internal static StructureLabelStyle GetByObjectId(acDb.ObjectId labelStyleId)
            => StyleSupport.Get<StructureLabelStyle, AeccLabelStyle>
            (labelStyleId, (labelStyle) => new StructureLabelStyle(labelStyle));

        /// <summary>
        /// Creates a Structure Label Style by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static StructureLabelStyle ByName(string name)
        {
            return (StructureLabelStyle)CreateByNameType(
                name, 
                "StructureLabeStyles.LabelStyles", 
                typeof(StructureLabelStyle).ToString());
        }
        #endregion

        #region methods
        public override string ToString() => $"StructureLabelStyle(Name = {Name})";
        #endregion
    }
}
