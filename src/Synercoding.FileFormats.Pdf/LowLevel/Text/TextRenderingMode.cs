namespace Synercoding.FileFormats.Pdf.LowLevel.Text;

/// <summary>
/// Text rendering mode
/// </summary>
public enum TextRenderingMode
{
    /// <summary>
    /// Fill text.
    /// </summary>
    Fill = 0,
    /// <summary>
    /// Stroke text.
    /// </summary>
    Stroke = 1,
    /// <summary>
    /// Fill, then stroke text.
    /// </summary>
    FillThenStroke = 2,
    /// <summary>
    /// Neither fill nor stroke text (invisible).
    /// </summary>
    Invisible = 3,
    /// <summary>
    /// Fill text and add to path for clipping.
    /// </summary>
    FillAddClippingPath = 4,
    /// <summary>
    /// Stroke text and add to path for clipping.
    /// </summary>
    StrokeAddClippingPath = 5,
    /// <summary>
    /// Fill, then stroke text and add path for clipping.
    /// </summary>
    FillThenStrokeAddClippingPath = 6,
    /// <summary>
    /// Add text to path for clipping.
    /// </summary>
    AddClippingPath = 7
}
