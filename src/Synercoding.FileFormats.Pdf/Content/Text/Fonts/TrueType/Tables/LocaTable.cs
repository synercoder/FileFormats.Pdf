using Synercoding.FileFormats.Pdf.IO;

namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.Tables;

/// <summary>
/// Represents the 'loca' (Index to Location) table in a TrueType font
/// </summary>
internal sealed class LocaTable
{
    private readonly uint[] _offsets;

    private LocaTable(uint[] offsets)
    {
        _offsets = offsets;
    }

    /// <summary>
    /// Parse a loca table from bytes
    /// </summary>
    public static LocaTable Parse(ReadOnlySpan<byte> data, short indexToLocFormat, ushort numGlyphs)
    {
        // indexToLocFormat: 0 for short (16-bit) offsets, 1 for long (32-bit) offsets
        // numGlyphs + 1 offsets are stored (the last offset is for the end of the last glyph)

        var offsets = new uint[numGlyphs + 1];

        if (indexToLocFormat == 0)
        {
            // Short format: offsets are 16-bit values that need to be multiplied by 2
            for (int i = 0; i <= numGlyphs; i++)
            {
                if (( i * 2 ) + 2 > data.Length)
                    break;

                var shortOffset = ByteUtils.ReadUInt16BigEndian(data, i * 2);
                offsets[i] = (uint)( shortOffset * 2 );
            }
        }
        else if (indexToLocFormat == 1)
        {
            // Long format: offsets are 32-bit values
            for (int i = 0; i <= numGlyphs; i++)
            {
                if (( i * 4 ) + 4 > data.Length)
                    break;

                offsets[i] = ByteUtils.ReadUInt32BigEndian(data, i * 4);
            }
        }
        else
        {
            throw new InvalidOperationException($"Invalid indexToLocFormat: {indexToLocFormat}");
        }

        return new LocaTable(offsets);
    }

    /// <summary>
    /// Get the offset and length for a glyph
    /// </summary>
    public (uint offset, uint length) GetGlyphLocation(ushort glyphId)
    {
        if (glyphId >= _offsets.Length - 1)
            return (0, 0);

        var offset = _offsets[glyphId];
        var nextOffset = _offsets[glyphId + 1];

        // If offset equals nextOffset, the glyph has no outline data
        if (offset >= nextOffset)
            return (offset, 0);

        return (offset, nextOffset - offset);
    }

    /// <summary>
    /// Check if a glyph has outline data
    /// </summary>
    public bool HasGlyphData(int glyphId)
    {
        var (_, length) = GetGlyphLocation((ushort)glyphId);
        return length > 0;
    }

    /// <summary>
    /// Get all offsets
    /// </summary>
    public uint[] GetOffsets()
    {
        return (uint[])_offsets.Clone();
    }
}
