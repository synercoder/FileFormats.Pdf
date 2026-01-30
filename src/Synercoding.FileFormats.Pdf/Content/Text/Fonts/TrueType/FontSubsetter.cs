using Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.Tables;
using Synercoding.FileFormats.Pdf.Generation.Internal;

namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType;

/// <summary>
/// Creates font subsets containing only used glyphs.
/// </summary>
internal sealed class FontSubsetter
{
    /// <summary>
    /// Create a subset of the font containing only the used characters.
    /// </summary>
    public SubsetResult CreateSubset(TrueTypeFont font, FontUsageTracker tracker)
    {
        // Get the used characters from the tracker
        var usedCharacters = tracker.GetUsedCharacters();

        // Create glyph remapping
        var remapper = new GlyphRemapper();
        var remapResult = remapper.RemapGlyphs(font, usedCharacters);

        // Build subset tables
        var subsetInput = _createSubsetTables(font, remapResult);

        // Write the subset font
        var ttfWriter = new TtfWriter();
        var subsetFontData = ttfWriter.WriteTtf(subsetInput);

        // Create CID to GID mapping
        // CID = Character ID (original glyph ID from full font)
        // GID = Glyph ID (new glyph ID in subset font)
        var cidToGidMap = new Dictionary<ushort, ushort>();
        foreach (var kvp in remapResult.OldToNewMapping)
        {
            cidToGidMap[(ushort)kvp.Key] = (ushort)kvp.Value;
        }

        return new SubsetResult
        {
            SubsetFontData = subsetFontData,
            CidToGidMap = cidToGidMap
        };
    }

    private TtfWriterInput _createSubsetTables(TrueTypeFont font, GlyphRemapResult remapResult)
    {
        var tables = font.Tables;

        // Create subset head table
        var head = _createSubsetHeadTable(tables.Head, remapResult);

        // Create subset maxp table
        var maxp = _createSubsetMaxpTable(tables.Maxp, remapResult);

        // Create subset cmap table
        var cmap = _createSubsetCmapTable(remapResult);

        // Create subset hmtx table first to determine numberOfHMetrics
        var (hmtx, numberOfHMetrics) = _createSubsetHmtxTableWithMetrics(tables.Hmtx, remapResult);

        // Create subset hhea table with the correct numberOfHMetrics
        var hhea = _createSubsetHheaTable(tables.Hhea, remapResult, numberOfHMetrics);

        // Create subset glyf and loca tables
        var (glyf, loca) = _createSubsetGlyfAndLocaTables(tables.Glyf, tables.Loca, remapResult, head?.IndexToLocFormat ?? 0);

        return new TtfWriterInput
        {
            Head = head,
            Hhea = hhea,
            Maxp = maxp,
            Cmap = cmap,
            Hmtx = hmtx,
            Loca = loca,
            Glyf = glyf,
            // Skip optional tables for now
            Name = null,
            Post = tables.Post, // Keep original post table
            OS2 = null
        };
    }

    private HeadTable? _createSubsetHeadTable(HeadTable? original, GlyphRemapResult remapResult)
    {
        if (original == null)
            return null;

        // Create a new head table with the same properties
        // Note: We're creating a copy since HeadTable doesn't have public setters
        // In a real implementation, we'd need to add a constructor or factory method
        return original; // For now, return the original
    }

    private HheaTable? _createSubsetHheaTable(HheaTable? original, GlyphRemapResult remapResult, ushort numberOfHMetrics)
    {
        if (original == null)
            return null;

        // Create a new hhea table with updated numberOfHMetrics
        return TableFactory.CreateHheaTable(original, numberOfHMetrics);
    }

    private MaxpTable? _createSubsetMaxpTable(MaxpTable? original, GlyphRemapResult remapResult)
    {
        if (original == null)
            return null;

        // Create a new maxp table with updated glyph count
        return TableFactory.CreateMaxpTable(original, (ushort)remapResult.TotalGlyphs);
    }

    private CmapTable _createSubsetCmapTable(GlyphRemapResult remapResult)
    {
        // Create a new cmap table with only the included characters
        var charToGlyph = new Dictionary<int, ushort>();
        foreach (var kvp in remapResult.CharacterToNewGlyph)
        {
            charToGlyph[kvp.Key] = kvp.Value;
        }

        return TableFactory.CreateCmapTable(charToGlyph);
    }

    private (HmtxTable?, ushort) _createSubsetHmtxTableWithMetrics(HmtxTable? original, GlyphRemapResult remapResult)
    {
        if (original == null)
            return (null, 0);

        var originalWidths = original.GetAdvanceWidths();
        var originalBearings = original.GetLeftSideBearings();

        var newWidths = new ushort[remapResult.TotalGlyphs];
        var newBearings = new short[remapResult.TotalGlyphs];

        foreach (var kvp in remapResult.NewToOldMapping)
        {
            var newGid = kvp.Key;
            var oldGid = kvp.Value;

            if (oldGid < originalWidths.Length)
                newWidths[newGid] = originalWidths[oldGid];

            if (oldGid < originalBearings.Length)
                newBearings[newGid] = originalBearings[oldGid];
        }

        // Calculate numberOfHMetrics
        var numberOfHMetrics = newWidths.Length;
        if (numberOfHMetrics > 1)
        {
            var lastWidth = newWidths[numberOfHMetrics - 1];
            while (numberOfHMetrics > 1 && newWidths[numberOfHMetrics - 2] == lastWidth)
            {
                numberOfHMetrics--;
            }
        }

        var hmtx = TableFactory.CreateHmtxTable(newWidths, newBearings, (ushort)numberOfHMetrics);
        return (hmtx, (ushort)numberOfHMetrics);
    }

    private (GlyfTable? glyf, LocaTable? loca) _createSubsetGlyfAndLocaTables(
        GlyfTable? originalGlyf,
        LocaTable? originalLoca,
        GlyphRemapResult remapResult,
        short indexToLocFormat)
    {
        if (originalGlyf == null || originalLoca == null)
            return (null, null);

        var glyphDataList = new List<byte[]?>();
        var offsets = new List<uint>();
        uint currentOffset = 0;

        // Build the new glyf table data
        for (ushort newGid = 0; newGid < remapResult.TotalGlyphs; newGid++)
        {
            offsets.Add(currentOffset);

            if (remapResult.NewToOldMapping.TryGetValue(newGid, out var oldGid))
            {
                var glyphData = originalGlyf.GetGlyphData(oldGid);

                if (glyphData != null && glyphData.Length > 0)
                {
                    // For composite glyphs, we need to update the component glyph IDs
                    if (_isCompositeGlyph(glyphData))
                    {
                        glyphData = _remapCompositeGlyph(glyphData, remapResult.OldToNewMapping);
                    }

                    glyphDataList.Add(glyphData);

                    // Align to 4-byte boundary
                    var paddedLength = ( glyphData.Length + 3 ) & ~3;
                    currentOffset += (uint)paddedLength;
                }
                else
                {
                    glyphDataList.Add(null);
                }
            }
            else
            {
                glyphDataList.Add(null);
            }
        }

        // Add final offset
        offsets.Add(currentOffset);

        // Create the glyf table data
        var glyfData = TableWriters.GlyfTableWriter.Write(glyphDataList);

        // Create the tables
        var newLoca = TableFactory.CreateLocaTable(offsets.ToArray(), indexToLocFormat);
        var newGlyf = TableFactory.CreateGlyfTable(glyfData, newLoca);

        return (newGlyf, newLoca);
    }

    private bool _isCompositeGlyph(byte[] glyphData)
    {
        if (glyphData.Length < 2)
            return false;

        var numberOfContours = (short)( ( glyphData[0] << 8 ) | glyphData[1] );
        return numberOfContours < 0;
    }

    private byte[] _remapCompositeGlyph(byte[] glyphData, Dictionary<ushort, ushort> oldToNew)
    {
        var result = (byte[])glyphData.Clone();
        var offset = 10; // Skip header
        bool hasMoreComponents = true;

        while (hasMoreComponents && offset + 4 <= result.Length)
        {
            var flags = (ushort)( ( result[offset] << 8 ) | result[offset + 1] );
            var componentGlyphId = (ushort)( ( result[offset + 2] << 8 ) | result[offset + 3] );

            // Remap the component glyph ID
            if (oldToNew.TryGetValue(componentGlyphId, out var newGlyphId))
            {
                result[offset + 2] = (byte)( newGlyphId >> 8 );
                result[offset + 3] = (byte)newGlyphId;
            }

            offset += 4;

            // Skip arguments based on flags
            if (( flags & 0x0001 ) != 0) // ARG_1_AND_2_ARE_WORDS
            {
                offset += 4;
            }
            else
            {
                offset += 2;
            }

            // Skip transformation matrix
            if (( flags & 0x0008 ) != 0) // WE_HAVE_A_SCALE
            {
                offset += 2;
            }
            else if (( flags & 0x0040 ) != 0) // WE_HAVE_AN_X_AND_Y_SCALE
            {
                offset += 4;
            }
            else if (( flags & 0x0080 ) != 0) // WE_HAVE_A_TWO_BY_TWO
            {
                offset += 8;
            }

            hasMoreComponents = ( flags & 0x0020 ) != 0; // MORE_COMPONENTS
        }

        return result;
    }
}
