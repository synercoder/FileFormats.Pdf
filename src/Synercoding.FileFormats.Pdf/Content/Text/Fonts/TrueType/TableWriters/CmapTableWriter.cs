using Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.Tables;
using Synercoding.FileFormats.Pdf.IO;

namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.TableWriters;

/// <summary>
/// Writes the 'cmap' (Character to Glyph Mapping) table for a TrueType font.
/// </summary>
internal static class CmapTableWriter
{
    /// <summary>
    /// Write a cmap table to a byte array.
    /// </summary>
    public static byte[] Write(CmapTable cmap)
    {
        // Extract the character to glyph mapping
        var charToGlyph = cmap.GetCharacterMappings();
        return Write(charToGlyph);
    }

    /// <summary>
    /// Write a cmap table from a character to glyph mapping dictionary.
    /// </summary>
    public static byte[] Write(Dictionary<int, ushort> charToGlyph)
    {
        // Determine whether to use format 4 (BMP only) or format 12 (full Unicode)
        var maxChar = charToGlyph.Keys.DefaultIfEmpty(0).Max();
        var useFormat12 = maxChar > 0xFFFF;

        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);

        // Write cmap header
        writer.WriteBigEndian((ushort)0); // Version
        writer.WriteBigEndian((ushort)1); // Number of encoding tables

        // Write encoding record
        writer.WriteBigEndian((ushort)3); // Platform ID (Microsoft)
        writer.WriteBigEndian((ushort)( useFormat12 ? 10 : 1 )); // Encoding ID (Unicode full or BMP)
        writer.WriteBigEndian((uint)12); // Offset to subtable (after header + 1 record = 4 + 8 = 12)

        // Write the subtable
        if (useFormat12)
            _writeFormat12(writer, charToGlyph);
        else
            _writeFormat4(writer, charToGlyph);

        return stream.ToArray();
    }

    private static void _writeFormat4(BinaryWriter writer, Dictionary<int, ushort> charToGlyph)
    {
        // Format 4 is for BMP (Basic Multilingual Plane) characters only
        var segments = _createSegments(charToGlyph);

        var segCount = (ushort)segments.Count;
        var searchRange = (ushort)( 2 * _highestPowerOf2LessThanOrEqual(segCount) );
        var entrySelector = (ushort)Math.Log2(searchRange / 2);
        var rangeShift = (ushort)( ( 2 * segCount ) - searchRange );

        // Calculate subtable length
        // Format 4 structure: 14 bytes header + segCount*2 endCodes + 2 bytes padding + segCount*2 startCodes + segCount*2 idDeltas + segCount*2 idRangeOffsets
        var subtableLength = (ushort)( 14 + 2 + ( 8 * segCount ) );

        // Write format 4 header
        var startPosition = writer.BaseStream.Position;
        writer.WriteBigEndian((ushort)4); // Format
        writer.WriteBigEndian(subtableLength); // Length
        writer.WriteBigEndian((ushort)0); // Language
        writer.WriteBigEndian((ushort)( segCount * 2 )); // segCountX2
        writer.WriteBigEndian(searchRange);
        writer.WriteBigEndian(entrySelector);
        writer.WriteBigEndian(rangeShift);

        // Write endCode array
        foreach (var seg in segments)
            writer.WriteBigEndian((ushort)seg.EndCode);

        // Reserved padding
        writer.WriteBigEndian((ushort)0);

        // Write startCode array
        foreach (var seg in segments)
            writer.WriteBigEndian((ushort)seg.StartCode);

        // Write idDelta array
        foreach (var seg in segments)
            writer.WriteBigEndian(seg.IdDelta);

        // Write idRangeOffset array (all zeros for simple mapping)
        foreach (var seg in segments)
            writer.WriteBigEndian((ushort)0);

        // No glyphIdArray needed when using idDelta
    }

    private static void _writeFormat12(BinaryWriter writer, Dictionary<int, ushort> charToGlyph)
    {
        // Format 12 supports full Unicode range
        var groups = _createGroups(charToGlyph);

        // Calculate subtable length
        var subtableLength = 16 + ( groups.Count * 12 );

        // Write format 12 header
        writer.WriteBigEndian((ushort)12); // Format
        writer.WriteBigEndian((ushort)0); // Reserved
        writer.WriteBigEndian((uint)subtableLength); // Length
        writer.WriteBigEndian((uint)0); // Language
        writer.WriteBigEndian((uint)groups.Count); // Number of groups

        // Write groups
        foreach (var group in groups)
        {
            writer.WriteBigEndian(group.StartCharCode);
            writer.WriteBigEndian(group.EndCharCode);
            writer.WriteBigEndian(group.StartGlyphID);
        }
    }

    private static List<Segment> _createSegments(Dictionary<int, ushort> charToGlyph)
    {
        var segments = new List<Segment>();
        var sortedChars = charToGlyph.Keys.Where(c => c <= 0xFFFF).OrderBy(c => c).ToList();

        if (sortedChars.Count == 0)
        {
            // Add required end segment
            segments.Add(new Segment { StartCode = 0xFFFF, EndCode = 0xFFFF, IdDelta = 1 });
            return segments;
        }

        var currentStart = sortedChars[0];
        var currentEnd = sortedChars[0];
        var startGlyph = charToGlyph[currentStart];

        for (int i = 1; i < sortedChars.Count; i++)
        {
            var ch = sortedChars[i];
            var glyph = charToGlyph[ch];

            // Check if this character continues the sequence
            if (ch == currentEnd + 1 && glyph == charToGlyph[currentEnd] + 1)
            {
                currentEnd = ch;
            }
            else
            {
                // End current segment
                var idDelta = (short)( startGlyph - currentStart );
                segments.Add(new Segment
                {
                    StartCode = currentStart,
                    EndCode = currentEnd,
                    IdDelta = idDelta
                });

                // Start new segment
                currentStart = ch;
                currentEnd = ch;
                startGlyph = glyph;
            }
        }

        // Add final segment
        var finalIdDelta = (short)( startGlyph - currentStart );
        segments.Add(new Segment
        {
            StartCode = currentStart,
            EndCode = currentEnd,
            IdDelta = finalIdDelta
        });

        // Add required end segment
        segments.Add(new Segment { StartCode = 0xFFFF, EndCode = 0xFFFF, IdDelta = 1 });

        return segments;
    }

    private static List<Group> _createGroups(Dictionary<int, ushort> charToGlyph)
    {
        var groups = new List<Group>();
        var sortedChars = charToGlyph.Keys.OrderBy(c => c).ToList();

        if (sortedChars.Count == 0)
            return groups;

        var currentStart = sortedChars[0];
        var currentEnd = sortedChars[0];
        var startGlyph = charToGlyph[currentStart];

        for (int i = 1; i < sortedChars.Count; i++)
        {
            var ch = sortedChars[i];
            var glyph = charToGlyph[ch];

            // Check if this character continues the sequence
            if (ch == currentEnd + 1 && glyph == charToGlyph[currentEnd] + 1)
            {
                currentEnd = ch;
            }
            else
            {
                // End current group
                groups.Add(new Group
                {
                    StartCharCode = (uint)currentStart,
                    EndCharCode = (uint)currentEnd,
                    StartGlyphID = (uint)startGlyph
                });

                // Start new group
                currentStart = ch;
                currentEnd = ch;
                startGlyph = glyph;
            }
        }

        // Add final group
        groups.Add(new Group
        {
            StartCharCode = (uint)currentStart,
            EndCharCode = (uint)currentEnd,
            StartGlyphID = (uint)startGlyph
        });

        return groups;
    }

    private static int _highestPowerOf2LessThanOrEqual(int n)
    {
        int power = 1;
        while (power * 2 <= n)
            power *= 2;
        return power;
    }

    private class Segment
    {
        public int StartCode { get; init; }
        public int EndCode { get; init; }
        public short IdDelta { get; init; }
    }

    private class Group
    {
        public uint StartCharCode { get; init; }
        public uint EndCharCode { get; init; }
        public uint StartGlyphID { get; init; }
    }
}
