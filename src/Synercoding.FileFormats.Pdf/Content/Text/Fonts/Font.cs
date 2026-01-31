using Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType;
using Synercoding.FileFormats.Pdf.Generation;
using Synercoding.FileFormats.Pdf.Generation.Internal;
using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts;

/// <summary>
/// Represents a font that can be embedded in a PDF document
/// </summary>
public abstract class Font : IEquatable<Font>
{
    /// <summary>
    /// Load a font from a file path
    /// </summary>
    /// <param name="path">Path to the font file</param>
    /// <returns>A font instance</returns>
    public static Font Load(string path)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));

        var data = File.ReadAllBytes(path);
        return Load(data);
    }

    /// <summary>
    /// Load a font from a stream
    /// </summary>
    /// <param name="stream">Stream containing font data</param>
    /// <returns>A font instance</returns>
    public static Font Load(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return Load(ms.ToArray());
    }

    /// <summary>
    /// Load a font from byte array
    /// </summary>
    /// <param name="data">Font data bytes</param>
    /// <returns>A font instance</returns>
    public static Font Load(byte[] data)
    {
        if (data == null || data.Length == 0)
            throw new ArgumentNullException(nameof(data));

        // For now, we only support TrueType fonts
        // In the future, we can detect font type and return appropriate implementation
        return new TrueTypeFont(data);
    }

    /// <summary>
    /// Gets the units per em for this font
    /// </summary>
    public abstract double UnitsPerEm { get; }

    /// <summary>
    /// Gets the maximum ascent across all glyphs
    /// </summary>
    public abstract double Ascent { get; }

    /// <summary>
    /// Gets the maximum descent across all glyphs
    /// </summary>
    public abstract double Descent { get; }

    /// <summary>
    /// Gets the height of capital letters
    /// </summary>
    public abstract double CapHeight { get; }

    /// <summary>
    /// Gets the height of lowercase letters without ascenders
    /// </summary>
    public abstract double XHeight { get; }

    /// <summary>
    /// Gets the underline position
    /// </summary>
    public abstract double UnderlinePosition { get; }

    /// <summary>
    /// Gets the underline thickness
    /// </summary>
    public abstract double UnderlineThickness { get; }

    /// <summary>
    /// Gets the line gap
    /// </summary>
    public abstract double LineGap { get; }

    /// <summary>
    /// Gets the font name
    /// </summary>
    public abstract string FontName { get; }

    /// <summary>
    /// Get the bounding box and metrics for a text string
    /// </summary>
    /// <param name="text">Text to measure</param>
    /// <param name="fontSize">Font size in points</param>
    /// <returns>Text measurement result</returns>
    public abstract TextMeasurement GetBoundingBox(string text, double fontSize);

    /// <summary>
    /// Encode text according to the font's encoding scheme for writing to PDF content streams
    /// </summary>
    /// <param name="text">Text to encode</param>
    /// <returns>Encoded bytes suitable for PDF content stream</returns>
    public abstract byte[] EncodeText(string text);

    /// <summary>
    /// Write this font to the PDF
    /// </summary>
    internal abstract void WriteTo(ObjectWriter writer, PdfObjectId id, FontUsageTracker tracker, WriterSettings writerSettings);

    /// <summary>
    /// Get the width of a glyph
    /// </summary>
    internal abstract double GetGlyphWidth(int glyphId);

    /// <summary>
    /// Get the glyph ID for a character
    /// </summary>
    internal abstract int GetGlyphId(char character);

    /// <summary>
    /// Get the raw font data
    /// </summary>
    internal abstract byte[] GetFontData();

    /// <inheritdoc/>
    public abstract bool Equals(Font? other);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Font font && Equals(font);

    /// <inheritdoc/>
    public abstract override int GetHashCode();
}
