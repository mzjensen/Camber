#region references
using System.Collections.Generic;
using Newtonsoft.Json;
using Dynamo.Graph.Nodes;
using Camber.Civil.Styles.Labels;
using Camber.Civil.Styles.Labels.Parcel;
#endregion

namespace Camber.UI
{
    [NodeName("Parcel Area Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Parcel")]
    [NodeDescription("Select Parcel Area Label Style.")]
    [IsDesignScriptCompatible]
    public class ParcelAreaLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "parcelAreaLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.ParcelLabelStyles.ToString() + "." + ParcelLabelStyles.AreaLabelStyles.ToString();
        private static string LabelStyleType = typeof(ParcelAreaLabelStyle).ToString();

        public ParcelAreaLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public ParcelAreaLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }

    [NodeName("Parcel Line Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Parcel")]
    [NodeDescription("Select Parcel Line Label Style.")]
    [IsDesignScriptCompatible]
    public class ParcelLineLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "parcelLineLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.ParcelLabelStyles.ToString() + "." + ParcelLabelStyles.LineLabelStyles.ToString();
        private static string LabelStyleType = typeof(ParcelLineLabelStyle).ToString();

        public ParcelLineLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public ParcelLineLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }

    [NodeName("Parcel Curve Label Styles")]
    [NodeCategory("Camber.Civil 3D.Styles.Label Styles.Parcel")]
    [NodeDescription("Select Parcel Curve Label Style.")]
    [IsDesignScriptCompatible]
    public class ParcelCurveLabelStylesDropDown : LabelStyleDropDownBase
    {
        private const string OutputName = "parcelCurveLabelStyle";
        private static string LabelStyleCollection = LabelStyleCollections.ParcelLabelStyles.ToString() + "." + ParcelLabelStyles.CurveLabelStyles.ToString();
        private static string LabelStyleType = typeof(ParcelCurveLabelStyle).ToString();

        public ParcelCurveLabelStylesDropDown() : base(OutputName, LabelStyleCollection, LabelStyleType) { }

        [JsonConstructor]
        public ParcelCurveLabelStylesDropDown(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts)
            : base(OutputName, LabelStyleCollection, LabelStyleType, inPorts, outPorts) { }
    }
}
