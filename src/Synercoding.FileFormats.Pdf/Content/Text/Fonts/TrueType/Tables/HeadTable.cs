using Synercoding.FileFormats.Pdf.IO;

namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.Tables;

/// <summary>
/// Represents the 'head' table in a TrueType font
/// </summary>
internal sealed class HeadTable
{
    /// <summary>
    /// Parse a head table from bytes
    /// </summary>
    public static HeadTable Parse(ReadOnlySpan<byte> data)
    {
        if (data.Length < 54)
            throw new InvalidOperationException("Head table too short");

        var table = new HeadTable();

        // Version (4 bytes) - skip
        // FontRevision (4 bytes) - skip
        // ChecksumAdjustment (4 bytes) - skip
        // MagicNumber (4 bytes) - skip
        var offset = 16;

        // Flags (2 bytes)
        table.Flags = ByteUtils.ReadUInt16BigEndian(data, ref offset);

        // UnitsPerEm (2 bytes)
        table.UnitsPerEm = ByteUtils.ReadUInt16BigEndian(data, ref offset);

        // Created (8 bytes) - skip
        // Modified (8 bytes) - skip
        offset += 16;

        // Bounding box
        table.XMin = ByteUtils.ReadInt16BigEndian(data, ref offset);
        table.YMin = ByteUtils.ReadInt16BigEndian(data, ref offset);
        table.XMax = ByteUtils.ReadInt16BigEndian(data, ref offset);
        table.YMax = ByteUtils.ReadInt16BigEndian(data, ref offset);

        // MacStyle (2 bytes)
        table.MacStyle = ByteUtils.ReadUInt16BigEndian(data, ref offset);

        // LowestRecPPEM (2 bytes) - skip
        offset += 2;

        // FontDirectionHint (2 bytes) - skip
        offset += 2;

        // IndexToLocFormat (2 bytes)
        table.IndexToLocFormat = ByteUtils.ReadInt16BigEndian(data, ref offset);

        return table;
    }

    /// <summary>
    /// Gets the flags
    /// </summary>
    public ushort Flags { get; private set; }

    /// <summary>
    /// Gets the units per em
    /// </summary>
    public ushort UnitsPerEm { get; private set; }

    /// <summary>
    /// Gets the minimum x coordinate
    /// </summary>
    public short XMin { get; private set; }

    /// <summary>
    /// Gets the minimum y coordinate
    /// </summary>
    public short YMin { get; private set; }

    /// <summary>
    /// Gets the maximum x coordinate
    /// </summary>
    public short XMax { get; private set; }

    /// <summary>
    /// Gets the maximum y coordinate
    /// </summary>
    public short YMax { get; private set; }

    /// <summary>
    /// Gets the Mac style flags
    /// </summary>
    public ushort MacStyle { get; private set; }

    /// <summary>
    /// Gets the index to location format (0 for short, 1 for long)
    /// </summary>
    public short IndexToLocFormat { get; private set; }

}
