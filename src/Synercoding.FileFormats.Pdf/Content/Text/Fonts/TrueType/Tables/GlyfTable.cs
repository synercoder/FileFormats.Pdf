using Synercoding.FileFormats.Pdf.IO;

namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.Tables;

/// <summary>
/// Represents the 'glyf' (Glyph Data) table in a TrueType font
/// </summary>
internal sealed class GlyfTable
{
    private readonly byte[] _glyfData;
    private readonly LocaTable _loca;

    private GlyfTable(byte[] glyfData, LocaTable loca)
    {
        _glyfData = glyfData;
        _loca = loca;
    }

    /// <summary>
    /// Parse a glyf table from bytes
    /// </summary>
    public static GlyfTable Parse(ReadOnlySpan<byte> data, LocaTable loca)
    {
        // Store the raw glyf data and loca table for on-demand parsing
        var glyfData = data.ToArray();
        return new GlyfTable(glyfData, loca);
    }

    /// <summary>
    /// Get the bounding box for a glyph
    /// </summary>
    public GlyphBoundingBox? GetGlyphBoundingBox(ushort glyphId)
    {
        var (offset, length) = _loca.GetGlyphLocation(glyphId);

        if (length == 0)
            return null; // Empty glyph

        // Glyph header is at least 10 bytes
        if (offset + 10 > _glyfData.Length)
            return null;

        // Read glyph header
        var numberOfContours = ByteUtils.ReadInt16BigEndian(_glyfData, (int)offset);

        // Bounding box is the same for simple and composite glyphs
        var xMin = ByteUtils.ReadInt16BigEndian(_glyfData, (int)offset + 2);
        var yMin = ByteUtils.ReadInt16BigEndian(_glyfData, (int)offset + 4);
        var xMax = ByteUtils.ReadInt16BigEndian(_glyfData, (int)offset + 6);
        var yMax = ByteUtils.ReadInt16BigEndian(_glyfData, (int)offset + 8);

        return new GlyphBoundingBox(xMin, yMin, xMax, yMax);
    }

    /// <summary>
    /// Get bounding boxes for multiple glyphs at once (more efficient)
    /// </summary>
    public Dictionary<ushort, GlyphBoundingBox> GetGlyphBoundingBoxes(IEnumerable<ushort> glyphIds)
    {
        var result = new Dictionary<ushort, GlyphBoundingBox>();

        foreach (var glyphId in glyphIds)
        {
            var bbox = GetGlyphBoundingBox(glyphId);
            if (bbox != null)
            {
                result[glyphId] = bbox;
            }
        }

        return result;
    }

    /// <summary>
    /// Get the raw glyph data for a specific glyph
    /// </summary>
    public byte[]? GetGlyphData(ushort glyphId)
    {
        var (offset, length) = _loca.GetGlyphLocation(glyphId);

        if (length == 0)
            return null; // Empty glyph

        if (offset + length > _glyfData.Length)
            return null;

        var glyphData = new byte[length];
        Array.Copy(_glyfData, offset, glyphData, 0, length);
        return glyphData;
    }

    /// <summary>
    /// Get the raw glyf table data
    /// </summary>
    public byte[] GetRawData()
    {
        return (byte[])_glyfData.Clone();
    }
}

/// <summary>
/// Represents a glyph's bounding box
/// </summary>
internal sealed class GlyphBoundingBox
{
    public GlyphBoundingBox(short xMin, short yMin, short xMax, short yMax)
    {
        XMin = xMin;
        YMin = yMin;
        XMax = xMax;
        YMax = yMax;
    }

    /// <summary>
    /// Minimum x coordinate for the glyph
    /// </summary>
    public short XMin { get; }

    /// <summary>
    /// Minimum y coordinate for the glyph
    /// </summary>
    public short YMin { get; }

    /// <summary>
    /// Maximum x coordinate for the glyph
    /// </summary>
    public short XMax { get; }

    /// <summary>
    /// Maximum y coordinate for the glyph
    /// </summary>
    public short YMax { get; }

    /// <summary>
    /// Width of the bounding box
    /// </summary>
    public int Width => XMax - XMin;

    /// <summary>
    /// Height of the bounding box
    /// </summary>
    public int Height => YMax - YMin;
}