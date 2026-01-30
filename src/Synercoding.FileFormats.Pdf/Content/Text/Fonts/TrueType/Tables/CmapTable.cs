using Synercoding.FileFormats.Pdf.IO;

namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.Tables;

/// <summary>
/// Represents the 'cmap' (Character to Glyph Mapping) table in a TrueType font
/// </summary>
internal sealed class CmapTable
{
    private readonly Dictionary<int, ushort> _charToGlyph = new();

    /// <summary>
    /// Parse a cmap table from bytes
    /// </summary>
    public static CmapTable Parse(ReadOnlySpan<byte> data)
    {
        if (data.Length < 4)
            throw new InvalidOperationException("Cmap table too short");

        var table = new CmapTable();

        // Version (2 bytes) - should be 0
        var version = ByteUtils.ReadUInt16BigEndian(data, 0);
        if (version != 0)
            throw new InvalidOperationException($"Unsupported cmap version: {version}");

        // Number of encoding tables (2 bytes)
        var numTables = ByteUtils.ReadUInt16BigEndian(data, 2);

        // Find a suitable encoding table (prefer Unicode)
        int selectedOffset = -1;
        int selectedFormat = -1;

        for (int i = 0; i < numTables; i++)
        {
            var recordOffset = 4 + ( i * 8 );
            if (recordOffset + 8 > data.Length)
                break;

            var platformId = ByteUtils.ReadUInt16BigEndian(data, recordOffset);
            var encodingId = ByteUtils.ReadUInt16BigEndian(data, recordOffset + 2);
            var offset = ByteUtils.ReadUInt32BigEndian(data, recordOffset + 4);

            // Prefer Microsoft Unicode (3, 1) or Unicode (0, x)
            if (( platformId == 3 && encodingId == 1 ) || platformId == 0)
            {
                selectedOffset = (int)offset;
                break;
            }
            // Also accept Microsoft Symbol (3, 0) as fallback
            else if (platformId == 3 && encodingId == 0 && selectedOffset == -1)
            {
                selectedOffset = (int)offset;
            }
        }

        if (selectedOffset == -1)
            throw new InvalidOperationException("No suitable cmap encoding found");

        // Parse the selected subtable
        if (selectedOffset + 2 > data.Length)
            throw new InvalidOperationException("Invalid cmap subtable offset");

        var format = ByteUtils.ReadUInt16BigEndian(data, selectedOffset);
        selectedFormat = format;

        switch (format)
        {
            case 0:
                _parseFormat0(table, data.Slice(selectedOffset));
                break;
            case 4:
                _parseFormat4(table, data.Slice(selectedOffset));
                break;
            case 6:
                _parseFormat6(table, data.Slice(selectedOffset));
                break;
            case 12:
                _parseFormat12(table, data.Slice(selectedOffset));
                break;
            default:
                throw new InvalidOperationException($"Unsupported cmap format: {format}");
        }

        return table;
    }

    /// <summary>
    /// Get the glyph ID for a character
    /// </summary>
    public int GetGlyphId(char character)
    {
        return GetGlyphId((int)character);
    }

    /// <summary>
    /// Get the glyph ID for a character code
    /// </summary>
    public int GetGlyphId(int charCode)
    {
        return _charToGlyph.TryGetValue(charCode, out var glyphId) ? glyphId : 0;
    }

    private static void _parseFormat0(CmapTable table, ReadOnlySpan<byte> data)
    {
        // Format 0: Byte encoding table
        if (data.Length < 262)
            throw new InvalidOperationException("Format 0 cmap too short");

        // Skip format (2) and length (2) and language (2)
        var offset = 6;

        for (int i = 0; i < 256; i++)
        {
            var glyphId = data[offset + i];
            if (glyphId != 0)
            {
                table._charToGlyph[i] = (ushort)glyphId;
            }
        }
    }

    private static void _parseFormat4(CmapTable table, ReadOnlySpan<byte> data)
    {
        // Format 4: Segment mapping to delta values
        if (data.Length < 14)
            throw new InvalidOperationException("Format 4 cmap too short");

        // Format (2 bytes) - already read
        // Length (2 bytes)
        var length = ByteUtils.ReadUInt16BigEndian(data, 2);
        if (data.Length < length)
            throw new InvalidOperationException("Format 4 cmap length mismatch");

        // Language (2 bytes) - skip
        // SegCountX2 (2 bytes)
        var segCountX2 = ByteUtils.ReadUInt16BigEndian(data, 6);
        var segCount = segCountX2 / 2;

        // SearchRange (2 bytes) - skip
        // EntrySelector (2 bytes) - skip
        // RangeShift (2 bytes) - skip
        var offset = 14;

        // Read segment arrays
        var endCodes = new ushort[segCount];
        for (int i = 0; i < segCount; i++)
        {
            endCodes[i] = ByteUtils.ReadUInt16BigEndian(data, ref offset);
        }

        // Reserved pad (2 bytes)
        offset += 2;

        var startCodes = new ushort[segCount];
        for (int i = 0; i < segCount; i++)
        {
            startCodes[i] = ByteUtils.ReadUInt16BigEndian(data, ref offset);
        }

        var idDeltas = new short[segCount];
        for (int i = 0; i < segCount; i++)
        {
            idDeltas[i] = ByteUtils.ReadInt16BigEndian(data, ref offset);
        }

        var idRangeOffsetStart = offset;
        var idRangeOffsets = new ushort[segCount];
        for (int i = 0; i < segCount; i++)
        {
            idRangeOffsets[i] = ByteUtils.ReadUInt16BigEndian(data, ref offset);
        }

        // Process segments
        for (int i = 0; i < segCount; i++)
        {
            var endCode = endCodes[i];
            var startCode = startCodes[i];
            var idDelta = idDeltas[i];
            var idRangeOffset = idRangeOffsets[i];

            for (int c = startCode; c <= endCode && c != 0xFFFF; c++)
            {
                int glyphId;

                if (idRangeOffset == 0)
                {
                    glyphId = ( c + idDelta ) & 0xFFFF;
                }
                else
                {
                    var glyphIdOffset = idRangeOffsetStart + ( i * 2 ) + idRangeOffset + ( ( c - startCode ) * 2 );
                    if (glyphIdOffset + 2 <= data.Length)
                    {
                        glyphId = ByteUtils.ReadUInt16BigEndian(data, glyphIdOffset);
                        if (glyphId != 0)
                        {
                            glyphId = ( glyphId + idDelta ) & 0xFFFF;
                        }
                    }
                    else
                    {
                        glyphId = 0;
                    }
                }

                if (glyphId != 0)
                {
                    table._charToGlyph[c] = (ushort)glyphId;
                }
            }
        }
    }

    private static void _parseFormat6(CmapTable table, ReadOnlySpan<byte> data)
    {
        // Format 6: Trimmed table mapping
        if (data.Length < 10)
            throw new InvalidOperationException("Format 6 cmap too short");

        // Format (2 bytes) - already read
        // Length (2 bytes)
        var length = ByteUtils.ReadUInt16BigEndian(data, 2);
        if (data.Length < length)
            throw new InvalidOperationException("Format 6 cmap length mismatch");

        // Language (2 bytes) - skip
        // FirstCode (2 bytes)
        var firstCode = ByteUtils.ReadUInt16BigEndian(data, 6);

        // EntryCount (2 bytes)
        var entryCount = ByteUtils.ReadUInt16BigEndian(data, 8);

        var offset = 10;
        for (int i = 0; i < entryCount; i++)
        {
            if (offset + 2 > data.Length)
                break;

            var glyphId = ByteUtils.ReadUInt16BigEndian(data, offset);
            if (glyphId != 0)
            {
                table._charToGlyph[firstCode + i] = glyphId;
            }
            offset += 2;
        }
    }

    private static void _parseFormat12(CmapTable table, ReadOnlySpan<byte> data)
    {
        // Format 12: Segmented coverage
        if (data.Length < 16)
            throw new InvalidOperationException("Format 12 cmap too short");

        // Format (2 bytes) - already read
        // Reserved (2 bytes) - skip
        // Length (4 bytes)
        var length = ByteUtils.ReadUInt32BigEndian(data, 4);
        if (data.Length < length)
            throw new InvalidOperationException("Format 12 cmap length mismatch");

        // Language (4 bytes) - skip
        // NumGroups (4 bytes)
        var numGroups = ByteUtils.ReadUInt32BigEndian(data, 12);

        var offset = 16;
        for (uint i = 0; i < numGroups; i++)
        {
            if (offset + 12 > data.Length)
                break;

            var startCharCode = ByteUtils.ReadUInt32BigEndian(data, offset);
            var endCharCode = ByteUtils.ReadUInt32BigEndian(data, offset + 4);
            var startGlyphId = ByteUtils.ReadUInt32BigEndian(data, offset + 8);

            for (uint c = startCharCode; c <= endCharCode; c++)
            {
                table._charToGlyph[(int)c] = (ushort)( startGlyphId + ( c - startCharCode ) );
            }

            offset += 12;
        }
    }

    /// <summary>
    /// Get the character to glyph mappings.
    /// </summary>
    public Dictionary<int, ushort> GetCharacterMappings()
    {
        return new Dictionary<int, ushort>(_charToGlyph);
    }

    /// <summary>
    /// Try to get the glyph ID for a character.
    /// </summary>
    public bool TryGetGlyphId(int character, out ushort glyphId)
    {
        return _charToGlyph.TryGetValue(character, out glyphId);
    }
}
