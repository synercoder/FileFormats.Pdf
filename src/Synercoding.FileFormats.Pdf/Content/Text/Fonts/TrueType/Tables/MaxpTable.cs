using Synercoding.FileFormats.Pdf.IO;

namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.Tables;

/// <summary>
/// Represents the 'maxp' (Maximum Profile) table in a TrueType font
/// </summary>
internal sealed class MaxpTable
{
    /// <summary>
    /// Parse a maxp table from bytes
    /// </summary>
    public static MaxpTable Parse(ReadOnlySpan<byte> data)
    {
        if (data.Length < 6)
            throw new InvalidOperationException("Maxp table too short");

        var table = new MaxpTable();

        // Version (4 bytes)
        var version = ByteUtils.ReadUInt32BigEndian(data, 0);
        table.Version = version;

        // NumGlyphs (2 bytes)
        table.NumGlyphs = ByteUtils.ReadUInt16BigEndian(data, 4);

        // Version 1.0 has additional data
        if (version == 0x00010000 && data.Length >= 32)
        {
            // MaxPoints (2 bytes)
            table.MaxPoints = ByteUtils.ReadUInt16BigEndian(data, 6);

            // MaxContours (2 bytes)
            table.MaxContours = ByteUtils.ReadUInt16BigEndian(data, 8);

            // MaxCompositePoints (2 bytes)
            table.MaxCompositePoints = ByteUtils.ReadUInt16BigEndian(data, 10);

            // MaxCompositeContours (2 bytes)
            table.MaxCompositeContours = ByteUtils.ReadUInt16BigEndian(data, 12);

            // MaxZones (2 bytes) - skip
            // MaxTwilightPoints (2 bytes) - skip
            // MaxStorage (2 bytes) - skip
            // MaxFunctionDefs (2 bytes) - skip
            // MaxInstructionDefs (2 bytes) - skip
            // MaxStackElements (2 bytes) - skip
            // MaxSizeOfInstructions (2 bytes) - skip
            // MaxComponentElements (2 bytes) - skip
            // MaxComponentDepth (2 bytes) - skip
        }

        return table;
    }

    /// <summary>
    /// Gets the version
    /// </summary>
    public uint Version { get; private set; }

    /// <summary>
    /// Gets the number of glyphs
    /// </summary>
    public ushort NumGlyphs { get; private set; }

    /// <summary>
    /// Gets the maximum points in a non-composite glyph
    /// </summary>
    public ushort MaxPoints { get; private set; }

    /// <summary>
    /// Gets the maximum contours in a non-composite glyph
    /// </summary>
    public ushort MaxContours { get; private set; }

    /// <summary>
    /// Gets the maximum points in a composite glyph
    /// </summary>
    public ushort MaxCompositePoints { get; private set; }

    /// <summary>
    /// Gets the maximum contours in a composite glyph
    /// </summary>
    public ushort MaxCompositeContours { get; private set; }
}
