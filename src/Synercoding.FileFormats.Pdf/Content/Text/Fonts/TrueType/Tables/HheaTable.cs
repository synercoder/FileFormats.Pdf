using Synercoding.FileFormats.Pdf.IO;

namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.Tables;

/// <summary>
/// Represents the 'hhea' (Horizontal Header) table in a TrueType font
/// </summary>
internal sealed class HheaTable
{
    /// <summary>
    /// Parse a hhea table from bytes
    /// </summary>
    public static HheaTable Parse(ReadOnlySpan<byte> data)
    {
        if (data.Length < 36)
            throw new InvalidOperationException("Hhea table too short");

        var table = new HheaTable();

        // Version (4 bytes) - skip
        var offset = 4;

        // Ascender (2 bytes)
        table.Ascender = ByteUtils.ReadInt16BigEndian(data, ref offset);

        // Descender (2 bytes)
        table.Descender = ByteUtils.ReadInt16BigEndian(data, ref offset);

        // LineGap (2 bytes)
        table.LineGap = ByteUtils.ReadInt16BigEndian(data, ref offset);

        // AdvanceWidthMax (2 bytes)
        table.AdvanceWidthMax = ByteUtils.ReadUInt16BigEndian(data, ref offset);

        // MinLeftSideBearing (2 bytes)
        table.MinLeftSideBearing = ByteUtils.ReadInt16BigEndian(data, ref offset);

        // MinRightSideBearing (2 bytes)
        table.MinRightSideBearing = ByteUtils.ReadInt16BigEndian(data, ref offset);

        // XMaxExtent (2 bytes)
        table.XMaxExtent = ByteUtils.ReadInt16BigEndian(data, ref offset);

        // CaretSlopeRise (2 bytes) - skip
        // CaretSlopeRun (2 bytes) - skip  
        // CaretOffset (2 bytes) - skip
        // Reserved (8 bytes) - skip
        // MetricDataFormat (2 bytes) - skip
        offset = 34;

        // NumberOfHMetrics (2 bytes)
        table.NumberOfHMetrics = ByteUtils.ReadUInt16BigEndian(data, ref offset);

        return table;
    }

    /// <summary>
    /// Gets the ascender (maximum extent above baseline)
    /// </summary>
    public short Ascender { get; private set; }

    /// <summary>
    /// Gets the descender (maximum extent below baseline)
    /// </summary>
    public short Descender { get; private set; }

    /// <summary>
    /// Gets the line gap
    /// </summary>
    public short LineGap { get; private set; }

    /// <summary>
    /// Gets the maximum advance width
    /// </summary>
    public ushort AdvanceWidthMax { get; private set; }

    /// <summary>
    /// Gets the minimum left side bearing
    /// </summary>
    public short MinLeftSideBearing { get; private set; }

    /// <summary>
    /// Gets the minimum right side bearing
    /// </summary>
    public short MinRightSideBearing { get; private set; }

    /// <summary>
    /// Gets the maximum x extent
    /// </summary>
    public short XMaxExtent { get; private set; }

    /// <summary>
    /// Gets the number of horizontal metrics
    /// </summary>
    public ushort NumberOfHMetrics { get; private set; }

}
