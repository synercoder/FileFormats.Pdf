namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts;

/// <summary>
/// Represents the measurement result for a text string
/// </summary>
public sealed class TextMeasurement
{
    /// <summary>
    /// Initializes a new instance of <see cref="TextMeasurement"/>
    /// </summary>
    /// <param name="boundingBox">The bounding box of the text</param>
    /// <param name="ascent">The actual ascent of the measured text</param>
    /// <param name="descent">The actual descent of the measured text</param>
    /// <param name="xHeight">The x-height of the measured text</param>
    public TextMeasurement(Rectangle boundingBox, double ascent, double descent, double xHeight)
    {
        BoundingBox = boundingBox;
        Ascent = ascent;
        Descent = descent;
        XHeight = xHeight;
    }

    /// <summary>
    /// Gets the bounding box of the text
    /// </summary>
    public Rectangle BoundingBox { get; }

    /// <summary>
    /// Gets the actual ascent of the measured text (character-dependent)
    /// </summary>
    public double Ascent { get; }

    /// <summary>
    /// Gets the actual descent of the measured text (character-dependent)
    /// </summary>
    public double Descent { get; }

    /// <summary>
    /// Gets the x-height of the text (useful for visual alignment)
    /// </summary>
    public double XHeight { get; }
}