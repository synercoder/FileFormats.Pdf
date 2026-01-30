using Synercoding.FileFormats.Pdf.IO;

namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType;

/// <summary>
/// Writes TrueType font data from table components.
/// </summary>
internal sealed class TtfWriter
{
    private const uint SFNT_VERSION_1_0 = 0x00010000;
    private const int TABLE_DIRECTORY_ENTRY_SIZE = 16;
    private const int OFFSET_TABLE_SIZE = 12;

    /// <summary>
    /// Write a TrueType font from the given tables.
    /// </summary>
    public byte[] WriteTtf(TtfWriterInput input)
    {
        var tables = _collectTables(input);
        var numTables = (ushort)tables.Count;

        // Calculate search parameters for table directory
        var searchRange = _calculateSearchRange(numTables);
        var entrySelector = _calculateEntrySelector(searchRange);
        var rangeShift = (ushort)( ( numTables * 16 ) - searchRange );

        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);

        // Write offset table (font header)
        writer.WriteBigEndian(SFNT_VERSION_1_0);
        writer.WriteBigEndian(numTables);
        writer.WriteBigEndian(searchRange);
        writer.WriteBigEndian(entrySelector);
        writer.WriteBigEndian(rangeShift);

        // Calculate table offsets
        var currentOffset = (uint)( OFFSET_TABLE_SIZE + ( numTables * TABLE_DIRECTORY_ENTRY_SIZE ) );
        var tableEntries = new List<TableEntry>();

        foreach (var (tag, data) in tables.OrderBy(t => t.tag, StringComparer.Ordinal))
        {
            var tableData = data;
            var paddedLength = _padToLongword(tableData.Length);

            tableEntries.Add(new TableEntry
            {
                Tag = tag,
                Checksum = _calculateChecksum(tableData),
                Offset = currentOffset,
                Length = (uint)tableData.Length,
                Data = tableData,
                PaddedLength = paddedLength
            });

            currentOffset += (uint)paddedLength;
        }

        // Write table directory
        foreach (var entry in tableEntries)
        {
            writer.WriteFourCC(entry.Tag);
            writer.WriteBigEndian(entry.Checksum);
            writer.WriteBigEndian(entry.Offset);
            writer.WriteBigEndian(entry.Length);
        }

        // Write table data
        foreach (var entry in tableEntries)
        {
            writer.Write(entry.Data);

            // Add padding if necessary
            var padding = entry.PaddedLength - entry.Data.Length;
            if (padding > 0)
            {
                writer.Write(new byte[padding]);
            }
        }

        var fontBytes = stream.ToArray();

        // Update head table checksum adjustment
        _updateChecksumAdjustment(fontBytes, tableEntries);

        return fontBytes;
    }

    private List<(string tag, byte[] data)> _collectTables(TtfWriterInput input)
    {
        var tables = new List<(string tag, byte[] data)>();

        if (input.Head != null)
            tables.Add(("head", TableWriters.HeadTableWriter.Write(input.Head)));

        if (input.Hhea != null)
            tables.Add(("hhea", TableWriters.HheaTableWriter.Write(input.Hhea)));

        if (input.Maxp != null)
            tables.Add(("maxp", TableWriters.MaxpTableWriter.Write(input.Maxp)));

        if (input.Cmap != null)
            tables.Add(("cmap", TableWriters.CmapTableWriter.Write(input.Cmap)));

        if (input.Hmtx != null)
            tables.Add(("hmtx", TableWriters.HmtxTableWriter.Write(input.Hmtx)));

        if (input.Loca != null)
            tables.Add(("loca", TableWriters.LocaTableWriter.Write(input.Loca, input.Head?.IndexToLocFormat ?? 0)));

        if (input.Glyf != null)
            tables.Add(("glyf", TableWriters.GlyfTableWriter.Write(input.Glyf)));

        if (input.Name != null)
            tables.Add(("name", TableWriters.NameTableWriter.Write(input.Name)));

        if (input.Post != null)
            tables.Add(("post", TableWriters.PostTableWriter.Write(input.Post)));

        if (input.OS2 != null)
            tables.Add(("OS/2", TableWriters.OS2TableWriter.Write(input.OS2)));

        return tables;
    }

    private static ushort _calculateSearchRange(ushort numTables)
    {
        var maxPower = 1;
        while (maxPower * 2 <= numTables)
            maxPower *= 2;
        return (ushort)( maxPower * 16 );
    }

    private static ushort _calculateEntrySelector(ushort searchRange)
    {
        var log2 = 0;
        var value = searchRange / 16;
        while (value > 1)
        {
            value /= 2;
            log2++;
        }
        return (ushort)log2;
    }

    private static int _padToLongword(int length)
    {
        return ( length + 3 ) & ~3;
    }

    private static uint _calculateChecksum(byte[] data)
    {
        uint sum = 0;
        var length = _padToLongword(data.Length);

        for (int i = 0; i < length; i += 4)
        {
            uint value = 0;
            for (int j = 0; j < 4; j++)
            {
                var byteIndex = i + j;
                var byteValue = byteIndex < data.Length ? data[byteIndex] : (byte)0;
                value = ( value << 8 ) | byteValue;
            }
            sum += value;
        }

        return sum;
    }

    private static void _updateChecksumAdjustment(byte[] fontBytes, List<TableEntry> tableEntries)
    {
        var headEntry = tableEntries.FirstOrDefault(e => e.Tag == "head");
        if (headEntry == null)
            return;

        // Calculate checksum for entire font
        uint sum = _calculateChecksum(fontBytes);
        uint checksumAdjustment = 0xB1B0AFBA - sum;

        // Update checksum adjustment field in head table (offset 8 from start of head table)
        var headTableOffset = (int)headEntry.Offset;
        var checksumAdjustmentOffset = headTableOffset + 8;

        fontBytes[checksumAdjustmentOffset] = (byte)( checksumAdjustment >> 24 );
        fontBytes[checksumAdjustmentOffset + 1] = (byte)( checksumAdjustment >> 16 );
        fontBytes[checksumAdjustmentOffset + 2] = (byte)( checksumAdjustment >> 8 );
        fontBytes[checksumAdjustmentOffset + 3] = (byte)checksumAdjustment;
    }

    private class TableEntry
    {
        public required string Tag { get; init; }
        public required uint Checksum { get; init; }
        public required uint Offset { get; init; }
        public required uint Length { get; init; }
        public required byte[] Data { get; init; }
        public required int PaddedLength { get; init; }
    }
}
