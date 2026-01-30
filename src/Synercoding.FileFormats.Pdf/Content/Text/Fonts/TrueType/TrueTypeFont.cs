using Synercoding.FileFormats.Pdf.Content.Text.Fonts.CID;
using Synercoding.FileFormats.Pdf.Generation;
using Synercoding.FileFormats.Pdf.Generation.Internal;
using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType;

/// <summary>
/// Represents a TrueType font
/// </summary>
internal sealed class TrueTypeFont : Font
{
    private const string MISSING_TABLE_MESSAGE_FORMAT = "Font is missing required '{0}' table";

    private readonly byte[] _fontData;
    private readonly int _fontDataHash;

    /// <summary>
    /// Create a TrueType font from font data
    /// </summary>
    internal TrueTypeFont(byte[] fontData)
    {
        _fontData = fontData ?? throw new ArgumentNullException(nameof(fontData));
        _fontDataHash = _computeHash(fontData);

        try
        {
            Tables = TrueTypeParser.Parse(fontData);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to parse TrueType font", ex);
        }

        // Validate required tables
        if (Tables.Head == null)
            throw new InvalidOperationException(string.Format(MISSING_TABLE_MESSAGE_FORMAT, "head"));
        if (Tables.Hhea == null)
            throw new InvalidOperationException(string.Format(MISSING_TABLE_MESSAGE_FORMAT, "hhea"));
        if (Tables.Maxp == null)
            throw new InvalidOperationException(string.Format(MISSING_TABLE_MESSAGE_FORMAT, "maxp"));
        if (Tables.Cmap == null)
            throw new InvalidOperationException(string.Format(MISSING_TABLE_MESSAGE_FORMAT, "cmap"));
        if (Tables.Hmtx == null)
            throw new InvalidOperationException(string.Format(MISSING_TABLE_MESSAGE_FORMAT, "hmtx"));
        if (Tables.Loca == null)
            throw new InvalidOperationException(string.Format(MISSING_TABLE_MESSAGE_FORMAT, "loca"));
        if (Tables.Glyf == null)
            throw new InvalidOperationException(string.Format(MISSING_TABLE_MESSAGE_FORMAT, "glyf"));
    }

    /// <inheritdoc/>
    public override double UnitsPerEm => Tables.Head!.UnitsPerEm;

    /// <inheritdoc/>
    public override double Ascent
    {
        get
        {
            // Prefer OS/2 metrics if available
            if (Tables.OS2?.TypoAscender != 0)
                return Tables.OS2!.TypoAscender;
            return Tables.Hhea!.Ascender;
        }
    }

    /// <inheritdoc/>
    public override double Descent
    {
        get
        {
            // Prefer OS/2 metrics if available
            if (Tables.OS2?.TypoDescender != 0)
                return Tables.OS2!.TypoDescender;
            return Tables.Hhea!.Descender;
        }
    }

    /// <inheritdoc/>
    public override double CapHeight
    {
        get
        {
            // Use OS/2 sCapHeight if available
            if (Tables.OS2?.SCapHeight != 0)
                return Tables.OS2!.SCapHeight;

            // Fallback to 70% of ascent (common approximation)
            return Ascent * 0.7;
        }
    }

    /// <inheritdoc/>
    public override double XHeight
    {
        get
        {
            // Use OS/2 sxHeight if available
            if (Tables.OS2?.SxHeight != 0)
                return Tables.OS2!.SxHeight;

            // Fallback to 50% of cap height (common approximation)
            return CapHeight * 0.5;
        }
    }

    /// <inheritdoc/>
    public override double UnderlinePosition
    {
        get
        {
            if (Tables.Post?.UnderlinePosition != 0)
                return Tables.Post!.UnderlinePosition;

            // Default to -10% of units per em
            return -UnitsPerEm * 0.1;
        }
    }

    /// <inheritdoc/>
    public override double UnderlineThickness
    {
        get
        {
            if (Tables.Post?.UnderlineThickness != 0)
                return Tables.Post!.UnderlineThickness;

            // Default to 5% of units per em
            return UnitsPerEm * 0.05;
        }
    }

    /// <inheritdoc/>
    public override double LineGap
    {
        get
        {
            // Prefer OS/2 metrics if available
            if (Tables.OS2?.TypoLineGap != 0)
                return Tables.OS2!.TypoLineGap;
            return Tables.Hhea!.LineGap;
        }
    }

    /// <inheritdoc/>
    public override string FontName
    {
        get
        {
            return Tables.Name?.PostScriptName
                ?? Tables.Name?.FullName
                ?? Tables.Name?.FamilyName
                ?? "UnknownFont";
        }
    }

    /// <inheritdoc/>
    public override byte[] EncodeText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return Array.Empty<byte>();

        var encoded = new byte[text.Length * 2];
        for (int i = 0; i < text.Length; i++)
        {
            var glyphId = Tables.Cmap!.GetGlyphId(text[i]);
            encoded[i * 2] = (byte)( ( glyphId >> 8 ) & 0xFF );
            encoded[( i * 2 ) + 1] = (byte)( glyphId & 0xFF );
        }

        return encoded;
    }

    /// <inheritdoc/>
    public override TextMeasurement GetBoundingBox(string text, double fontSize)
    {
        if (string.IsNullOrEmpty(text))
            return new TextMeasurement(
                new Rectangle(0, 0, 0, 0, Unit.Points),
                0, 0, 0);

        double totalWidth = 0;
        double maxAscent = 0;
        double maxDescent = 0;

        var scale = fontSize / UnitsPerEm;

        // Use actual glyph bounding boxes from glyf table
        foreach (char c in text)
        {
            var glyphId = GetGlyphId(c);
            var width = GetGlyphWidth(glyphId);
            totalWidth += width * scale;

            // Get actual glyph bounding box
            var bbox = Tables.Glyf!.GetGlyphBoundingBox((ushort)glyphId);
            if (bbox != null)
            {
                // Update max ascent and descent based on actual glyph metrics
                maxAscent = Math.Max(maxAscent, bbox.YMax * scale);
                maxDescent = Math.Min(maxDescent, bbox.YMin * scale);
            }
            else
            {
                // Empty glyph or missing character - use font-level metrics
                // This ensures space characters and missing glyphs still contribute to line height
                if (maxAscent == 0)
                    maxAscent = Ascent * scale;
                if (maxDescent == 0)
                    maxDescent = Descent * scale;
            }
        }

        // If no specific ascent was determined (e.g., only spaces), use x-height as default
        if (maxAscent == 0)
            maxAscent = XHeight * scale;

        var boundingBox = new Rectangle(
            0, maxDescent,
            totalWidth, maxAscent,
            Unit.Points);

        return new TextMeasurement(
            boundingBox,
            maxAscent,
            Math.Abs(maxDescent),
            XHeight * scale);
    }

    /// <inheritdoc/>
    internal override void WriteTo(ObjectWriter writer, PdfObjectId id, FontUsageTracker tracker, WriterSettings writerSettings)
    {
        CIDFontWriter.WriteType0Font(writer, id, this, tracker, writerSettings);
    }

    /// <inheritdoc/>
    internal override double GetGlyphWidth(int glyphId)
    {
        if (Tables.Hmtx == null)
            return 0;

        return Tables.Hmtx.GetAdvanceWidth(glyphId);
    }

    /// <inheritdoc/>
    internal override int GetGlyphId(char character)
    {
        if (Tables.Cmap == null)
            return 0;

        return Tables.Cmap.GetGlyphId(character);
    }

    /// <inheritdoc/>
    internal override byte[] GetFontData()
    {
        return _fontData;
    }

    /// <inheritdoc/>
    public override bool Equals(Font? other)
    {
        if (other is not TrueTypeFont ttf)
            return false;

        // Compare by hash first for performance
        if (_fontDataHash != ttf._fontDataHash)
            return false;

        // Then compare actual data if hashes match
        return _fontData.SequenceEqual(ttf._fontData);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return _fontDataHash;
    }

    private static int _computeHash(byte[] data)
    {
        // Simple hash based on font data
        unchecked
        {
            int hash = 17;
            // Sample the font data for performance
            int step = Math.Max(1, data.Length / 100);
            for (int i = 0; i < data.Length; i += step)
            {
                hash = ( hash * 31 ) + data[i];
            }
            hash = ( hash * 31 ) + data.Length;
            return hash;
        }
    }

    /// <summary>
    /// Gets the TrueType tables (for internal use)
    /// </summary>
    internal TrueTypeTables Tables { get; }
}
