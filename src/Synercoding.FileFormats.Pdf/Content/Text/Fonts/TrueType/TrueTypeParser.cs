using System.Text;
using Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.Tables;
using Synercoding.FileFormats.Pdf.IO;

namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType;

/// <summary>
/// Parser for TrueType font files
/// </summary>
internal static class TrueTypeParser
{
    /// <summary>
    /// Parse TrueType font data
    /// </summary>
    public static TrueTypeTables Parse(byte[] fontData)
    {
        if (fontData == null || fontData.Length < 12)
            throw new ArgumentException("Invalid font data");

        var tables = new Dictionary<string, (uint offset, uint length)>();

        // Read offset table
        var scalarType = ByteUtils.ReadUInt32BigEndian(fontData, 0);

        // Check if it's a TrueType font (0x00010000) or OpenType with TrueType outlines ('true')
        if (scalarType != 0x00010000 && scalarType != 0x74727565)
        {
            // Check if it's an OpenType font with CFF outlines ('OTTO')
            if (scalarType == 0x4F54544F)
                throw new NotSupportedException("OpenType fonts with CFF outlines are not yet supported");

            throw new ArgumentException($"Not a valid TrueType font. ScalarType: 0x{scalarType:X8}");
        }

        var numTables = ByteUtils.ReadUInt16BigEndian(fontData, 4);
        // searchRange, entrySelector, rangeShift - skip

        // Read table directory
        var offset = 12;
        for (int i = 0; i < numTables; i++)
        {
            var tag = Encoding.ASCII.GetString(fontData, offset, 4);
            var checksum = ByteUtils.ReadUInt32BigEndian(fontData, offset + 4);
            var tableOffset = ByteUtils.ReadUInt32BigEndian(fontData, offset + 8);
            var tableLength = ByteUtils.ReadUInt32BigEndian(fontData, offset + 12);

            tables[tag] = (tableOffset, tableLength);
            offset += 16;
        }

        // Parse required tables
        var result = new TrueTypeTables();

        // Head table (required)
        if (tables.TryGetValue("head", out var headInfo))
        {
            var headData = new ReadOnlySpan<byte>(fontData, (int)headInfo.offset, (int)headInfo.length);
            result.Head = HeadTable.Parse(headData);
        }
        else
        {
            throw new InvalidOperationException("Missing required 'head' table");
        }

        // Hhea table (required)
        if (tables.TryGetValue("hhea", out var hheaInfo))
        {
            var hheaData = new ReadOnlySpan<byte>(fontData, (int)hheaInfo.offset, (int)hheaInfo.length);
            result.Hhea = HheaTable.Parse(hheaData);
        }
        else
        {
            throw new InvalidOperationException("Missing required 'hhea' table");
        }

        // Maxp table (required)
        if (tables.TryGetValue("maxp", out var maxpInfo))
        {
            var maxpData = new ReadOnlySpan<byte>(fontData, (int)maxpInfo.offset, (int)maxpInfo.length);
            result.Maxp = MaxpTable.Parse(maxpData);
        }
        else
        {
            throw new InvalidOperationException("Missing required 'maxp' table");
        }

        // Cmap table (required)
        if (tables.TryGetValue("cmap", out var cmapInfo))
        {
            var cmapData = new ReadOnlySpan<byte>(fontData, (int)cmapInfo.offset, (int)cmapInfo.length);
            result.Cmap = CmapTable.Parse(cmapData);
        }
        else
        {
            throw new InvalidOperationException("Missing required 'cmap' table");
        }

        // Hmtx table (required)
        if (tables.TryGetValue("hmtx", out var hmtxInfo))
        {
            var hmtxData = new ReadOnlySpan<byte>(fontData, (int)hmtxInfo.offset, (int)hmtxInfo.length);
            result.Hmtx = HmtxTable.Parse(hmtxData, result.Hhea.NumberOfHMetrics, result.Maxp.NumGlyphs);
        }
        else
        {
            throw new InvalidOperationException("Missing required 'hmtx' table");
        }

        // Loca table (required for TrueType)
        if (tables.TryGetValue("loca", out var locaInfo))
        {
            var locaData = new ReadOnlySpan<byte>(fontData, (int)locaInfo.offset, (int)locaInfo.length);
            result.Loca = LocaTable.Parse(locaData, result.Head.IndexToLocFormat, result.Maxp.NumGlyphs);
        }
        else
        {
            throw new InvalidOperationException("Missing required 'loca' table");
        }

        // Glyf table (required for TrueType)
        if (tables.TryGetValue("glyf", out var glyfInfo))
        {
            var glyfData = new ReadOnlySpan<byte>(fontData, (int)glyfInfo.offset, (int)glyfInfo.length);
            result.Glyf = GlyfTable.Parse(glyfData, result.Loca);
        }
        else
        {
            throw new InvalidOperationException("Missing required 'glyf' table");
        }

        // Name table (optional but useful)
        if (tables.TryGetValue("name", out var nameInfo))
        {
            var nameData = new ReadOnlySpan<byte>(fontData, (int)nameInfo.offset, (int)nameInfo.length);
            result.Name = NameTable.Parse(nameData);
        }

        // OS/2 table (optional but useful for metrics)
        if (tables.TryGetValue("OS/2", out var os2Info))
        {
            var os2Data = new ReadOnlySpan<byte>(fontData, (int)os2Info.offset, (int)os2Info.length);
            result.OS2 = OS2Table.Parse(os2Data);
        }

        // Post table (optional)
        if (tables.TryGetValue("post", out var postInfo))
        {
            var postData = new ReadOnlySpan<byte>(fontData, (int)postInfo.offset, (int)postInfo.length);
            result.Post = PostTable.Parse(postData);
        }

        return result;
    }
}
