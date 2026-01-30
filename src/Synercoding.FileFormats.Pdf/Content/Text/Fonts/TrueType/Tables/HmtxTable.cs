using Synercoding.FileFormats.Pdf.IO;

namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.Tables;

/// <summary>
/// Represents the 'hmtx' (Horizontal Metrics) table in a TrueType font
/// </summary>
internal sealed class HmtxTable
{
    private ushort[]? _advanceWidths;
    private short[]? _leftSideBearings;

    /// <summary>
    /// Parse a hmtx table from bytes
    /// </summary>
    public static HmtxTable Parse(ReadOnlySpan<byte> data, ushort numberOfHMetrics, ushort numGlyphs)
    {
        var table = new HmtxTable
        {
            // Allocate arrays
            _advanceWidths = new ushort[numGlyphs],
            _leftSideBearings = new short[numGlyphs]
        };

        var offset = 0;

        // Read the metrics for numberOfHMetrics glyphs
        for (int i = 0; i < numberOfHMetrics; i++)
        {
            if (offset + 4 > data.Length)
                break;

            table._advanceWidths[i] = ByteUtils.ReadUInt16BigEndian(data, ref offset);
            table._leftSideBearings[i] = ByteUtils.ReadInt16BigEndian(data, ref offset);
        }

        // For glyphs beyond numberOfHMetrics, use the last advance width
        if (numberOfHMetrics > 0)
        {
            var lastAdvanceWidth = table._advanceWidths[numberOfHMetrics - 1];
            for (int i = numberOfHMetrics; i < numGlyphs; i++)
            {
                table._advanceWidths[i] = lastAdvanceWidth;

                // Read left side bearing if available
                if (offset + 2 <= data.Length)
                {
                    table._leftSideBearings[i] = ByteUtils.ReadInt16BigEndian(data, ref offset);
                }
            }
        }

        return table;
    }

    /// <summary>
    /// Get the advance width for a glyph
    /// </summary>
    public ushort GetAdvanceWidth(int glyphId)
    {
        if (_advanceWidths == null || glyphId < 0 || glyphId >= _advanceWidths.Length)
            return 0;

        return _advanceWidths[glyphId];
    }

    /// <summary>
    /// Get the left side bearing for a glyph
    /// </summary>
    public short GetLeftSideBearing(int glyphId)
    {
        if (_leftSideBearings == null || glyphId < 0 || glyphId >= _leftSideBearings.Length)
            return 0;

        return _leftSideBearings[glyphId];
    }

    /// <summary>
    /// Get all advance widths
    /// </summary>
    public ushort[] GetAdvanceWidths()
    {
        return _advanceWidths ?? Array.Empty<ushort>();
    }

    /// <summary>
    /// Get all left side bearings
    /// </summary>
    public short[] GetLeftSideBearings()
    {
        return _leftSideBearings ?? Array.Empty<short>();
    }
}
