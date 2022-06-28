#region references
using System;
using acDb = Autodesk.AutoCAD.DatabaseServices;
using civDb = Autodesk.Civil.DatabaseServices;
using AeccBandStyle = Autodesk.Civil.DatabaseServices.Styles.BandStyle;
using Autodesk.DesignScript.Runtime;
#endregion

namespace Camber.Civil.Styles.Bands
{
    [IsVisibleInDynamoLibrary(false)]
    public class BandStyle : Style
    {
        #region properties
        internal AeccBandStyle AeccBandStyle => AcObject as AeccBandStyle;

        /// <summary>
        /// Gets the height of the band used in the title.
        /// </summary>
        public double BandHeight => GetDouble();

        /// <summary>
        /// Gets the type of data band data.
        /// </summary>
        public string BandType => GetString();

        /// <summary>
        /// Gets the distance between the title box and the left side of the data band.
        /// </summary>
        public double OffsetFromBand => GetDouble();

        /// <summary>
        /// Gets the current title text.
        /// </summary>
        public string TitleText => GetString("Text");

        /// <summary>
        /// Gets the text box position of the band title.
        /// </summary>
        public string TitleTextBoxPosition => GetString("TextBoxPosition");

        /// <summary>
        /// Gets the width of the box that contains the text of the band title.
        /// </summary>
        public double TitleTextBoxWidth => GetDouble("TextBoxWidth");

        /// <summary>
        /// Gets the height of the band title text in plot units.
        /// </summary>
        public double TitleTextHeight => GetDouble("TextHeight");

        /// <summary>
        /// Gets the band title text location.
        /// </summary>
        public string TitleTextLocation => GetString("TextLocation");

        /// <summary>
        /// Gets the name of the AutoCAD text style for the band title.
        /// </summary>
        public string TitleTextStyle => GetString("TextStyle");

        /// <summary>
        /// Gets the band weeding factor.
        /// </summary>
        public double WeedingFactor => GetDouble();
        #endregion

        #region constructors
        internal BandStyle(AeccBandStyle aeccBandStyle, bool isDynamoOwned = false) : base(aeccBandStyle, isDynamoOwned) { }

        internal static BandStyle GetByObjectId(acDb.ObjectId bandStyleId)
            => StyleSupport.Get<BandStyle, AeccBandStyle>
            (bandStyleId, (bandStyle) => new BandStyle(bandStyle));
        #endregion

        #region methods
        public override string ToString() => $"BandStyle(Name = {Name})";

        /// <summary>
        /// Sets the height of the band used in the title.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public BandStyle SetBandHeight(double value)
        {
            SetValue(value);
            return this;
        }

        /// <summary>
        /// Sets the type of data band data. 
        /// </summary>
        /// <param name="bandType"></param>
        /// <returns></returns>
        public BandStyle SetBandType(string bandType)
        {
            SetValue((Autodesk.Civil.BandType)Enum.Parse(typeof(Autodesk.Civil.BandType), bandType));
            return this;
        }

        /// <summary>
        /// Sets the distance between the title box and the left side of the data band.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public BandStyle SetOffsetFromBand(double offset)
        {
            SetValue(offset);
            return this;
        }

        /// <summary>
        /// Sets the current title text.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public BandStyle SetTitleText(string text)
        {
            SetValue((object)text, "Text");
            return this;
        }

        /// <summary>
        /// Sets the text box position of the band title.
        /// </summary>
        /// <param name="leftOfBand">True = left of band, False = right of band</param>
        /// <returns></returns>
        public BandStyle SetTitleTextBoxPosition(bool leftOfBand)
        {
            if (leftOfBand)
            {
                SetValue(civDb.Styles.BandTitleBoxPosition.LeftOfBand, "TextBoxPosition");
            }
            SetValue(civDb.Styles.BandTitleBoxPosition.RightOfBand, "TextBoxPosition");
            return this;
        }

        /// <summary>
        /// Sets the width of the box that contains the text of the band title.
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public BandStyle SetTitleTextBoxWidth(double width)
        {
            SetValue(width, "TextBoxWidth");
            return this;
        }

        /// <summary>
        /// Sets the height of the band title text in plot units.
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        public BandStyle SetTitleTextHeight(double height)
        {
            SetValue(height, "TextHeight");
            return this;
        }

        /// <summary>
        /// Sets the band title text location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public BandStyle SetTitleTextLocation(string location)
        {
            SetValue((civDb.Styles.BandTitleTextLocation)Enum.Parse(typeof(civDb.Styles.BandTitleTextLocation), location), "TextLocation");
            return this;
        }

        /// <summary>
        /// Sets the name of the AutoCAD text style for the band title.
        /// </summary>
        /// <param name="styleName"></param>
        /// <returns></returns>
        public BandStyle SetTitleTextStyle(string styleName)
        {
            SetValue((object)styleName, "TextStyle");
            return this;
        }

        /// <summary>
        /// Sets the band weeding factor.
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        public BandStyle SetWeedingFactor(double factor)
        {
            SetValue(factor);
            return this;
        }
        #endregion
    }
}
