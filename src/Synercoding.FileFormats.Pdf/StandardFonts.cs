using Synercoding.FileFormats.Pdf.LowLevel;
using Synercoding.FileFormats.Pdf.LowLevel.Text;

namespace Synercoding.FileFormats.Pdf;

/// <summary>
/// Collection of the 14 standard fonts
/// </summary>
public static class StandardFonts
{
    /// <summary>
    /// A type 1 standard font for Times Roman
    /// </summary>
    public static Type1StandardFont TimesRoman { get; }
        = new Type1StandardFont(PdfName.Get("Times-Roman"), PdfName.Get("StdFont-Times-Roman"));

    /// <summary>
    /// A type 1 standard font for Times-Bold
    /// </summary>
    public static Type1StandardFont TimesRomanBold { get; }
        = new Type1StandardFont(PdfName.Get("Times-Bold"), PdfName.Get("StdFont-Times-Roman-Bold"));

    /// <summary>
    /// A type 1 standard font for Times-Italic
    /// </summary>
    public static Type1StandardFont TimesRomanItalic { get; }
        = new Type1StandardFont(PdfName.Get("Times-Italic"), PdfName.Get("StdFont-Times-Roman-Italic"));

    /// <summary>
    /// A type 1 standard font for Times-BoldItalic
    /// </summary>
    public static Type1StandardFont TimesRomanBoldItalic { get; }
        = new Type1StandardFont(PdfName.Get("Times-BoldItalic"), PdfName.Get("StdFont-Times-Roman-Bold-Italic"));

    /// <summary>
    /// A type 1 standard font for Helvetica
    /// </summary>
    public static Type1StandardFont Helvetica { get; }
        = new Type1StandardFont(PdfName.Get("Helvetica"), PdfName.Get("StdFont-Helvetica"));

    /// <summary>
    /// A type 1 standard font for Helvetica-Bold
    /// </summary>
    public static Type1StandardFont HelveticaBold { get; }
        = new Type1StandardFont(PdfName.Get("Helvetica-Bold"), PdfName.Get("StdFont-Helvetica-Bold"));

    /// <summary>
    /// A type 1 standard font for Helvetica-Oblique
    /// </summary>
    public static Type1StandardFont HelveticaOblique { get; }
        = new Type1StandardFont(PdfName.Get("Helvetica-Oblique"), PdfName.Get("StdFont-Helvetica-Oblique"));

    /// <summary>
    /// A type 1 standard font for Helvetica-BoldOblique
    /// </summary>
    public static Type1StandardFont HelveticaBoldOblique { get; }
        = new Type1StandardFont(PdfName.Get("Helvetica-BoldOblique"), PdfName.Get("StdFont-Helvetica-BoldOblique"));

    /// <summary>
    /// A type 1 standard font for Courier
    /// </summary>
    public static Type1StandardFont Courier { get; }
        = new Type1StandardFont(PdfName.Get("Courier"), PdfName.Get("StdFont-Courier"));

    /// <summary>
    /// A type 1 standard font for Courier-Bold
    /// </summary>
    public static Type1StandardFont CourierBold { get; }
        = new Type1StandardFont(PdfName.Get("Courier-Bold"), PdfName.Get("StdFont-Courier-Bold"));

    /// <summary>
    /// A type 1 standard font for Courier-Oblique
    /// </summary>
    public static Type1StandardFont CourierOblique { get; }
        = new Type1StandardFont(PdfName.Get("Courier-Oblique"), PdfName.Get("StdFont-Courier-Oblique"));

    /// <summary>
    /// A type 1 standard font for Courier-BoldOblique
    /// </summary>
    public static Type1StandardFont CourierBoldOblique { get; }
        = new Type1StandardFont(PdfName.Get("Courier-BoldOblique"), PdfName.Get("StdFont-Courier-BoldOblique"));

    /// <summary>
    /// A type 1 standard font for Symbol
    /// </summary>
    public static Type1StandardFont Symbol { get; }
        = new Type1StandardFont(PdfName.Get("Symbol"), PdfName.Get("StdFont-Symbol"));

    /// <summary>
    /// A type 1 standard font for ZapfDingbats
    /// </summary>
    public static Type1StandardFont ZapfDingbats { get; }
        = new Type1StandardFont(PdfName.Get("ZapfDingbats"), PdfName.Get("StdFont-ZapfDingbats"));
}
