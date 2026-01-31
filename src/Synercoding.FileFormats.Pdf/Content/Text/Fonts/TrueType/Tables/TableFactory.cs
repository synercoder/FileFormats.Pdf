using Synercoding.FileFormats.Pdf.IO;

namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.Tables;

/// <summary>
/// Factory methods for creating TrueType font tables.
/// </summary>
internal static class TableFactory
{
    /// <summary>
    /// Create a CmapTable from a character to glyph mapping.
    /// </summary>
    public static CmapTable CreateCmapTable(Dictionary<int, ushort> charToGlyph)
    {
        // Build cmap data using CmapTableWriter
        var cmapData = TableWriters.CmapTableWriter.Write(charToGlyph);

        // Parse it back to create a CmapTable
        return CmapTable.Parse(cmapData);
    }

    /// <summary>
    /// Create an HmtxTable from metrics arrays.
    /// </summary>
    public static HmtxTable CreateHmtxTable(ushort[] advanceWidths, short[] leftSideBearings, ushort numberOfHMetrics)
    {
        // Build hmtx data using HmtxTableWriter
        var hmtxData = TableWriters.HmtxTableWriter.Write(advanceWidths, leftSideBearings, numberOfHMetrics);

        // Parse it back to create an HmtxTable
        return HmtxTable.Parse(hmtxData, numberOfHMetrics, (ushort)advanceWidths.Length);
    }

    /// <summary>
    /// Create a LocaTable from offset array.
    /// </summary>
    public static LocaTable CreateLocaTable(uint[] offsets, short indexToLocFormat)
    {
        // Build loca data using LocaTableWriter
        var locaData = TableWriters.LocaTableWriter.Write(offsets, indexToLocFormat);

        // Parse it back to create a LocaTable
        return LocaTable.Parse(locaData, indexToLocFormat, (ushort)( offsets.Length - 1 ));
    }

    /// <summary>
    /// Create a GlyfTable from glyph data.
    /// </summary>
    public static GlyfTable CreateGlyfTable(byte[] glyfData, LocaTable loca)
    {
        // Parse the glyf data to create a GlyfTable
        return GlyfTable.Parse(glyfData, loca);
    }

    /// <summary>
    /// Create a copy of HeadTable with updated values.
    /// </summary>
    public static HeadTable CreateHeadTable(HeadTable original, ushort numGlyphs)
    {
        // For now, we'll just return the original since we can't modify it
        // In a full implementation, we'd create a new HeadTable with updated values
        return original;
    }

    /// <summary>
    /// Create a copy of HheaTable with updated numberOfHMetrics.
    /// </summary>
    public static HheaTable CreateHheaTable(HheaTable original, ushort numberOfHMetrics)
    {
        // Create a temporary hhea table with updated numberOfHMetrics
        using var stream = new System.IO.MemoryStream();
        using var writer = new System.IO.BinaryWriter(stream);

        // Write the complete hhea table with updated numberOfHMetrics
        writer.WriteBigEndian(0x00010000u); // Version 1.0
        writer.WriteBigEndian(original.Ascender);
        writer.WriteBigEndian(original.Descender);
        writer.WriteBigEndian(original.LineGap);
        writer.WriteBigEndian(original.AdvanceWidthMax);
        writer.WriteBigEndian(original.MinLeftSideBearing);
        writer.WriteBigEndian(original.MinRightSideBearing);
        writer.WriteBigEndian(original.XMaxExtent);
        writer.WriteBigEndian((short)1); // CaretSlopeRise
        writer.WriteBigEndian((short)0); // CaretSlopeRun
        writer.WriteBigEndian((short)0); // CaretOffset
        writer.WriteBigEndian((short)0); // Reserved
        writer.WriteBigEndian((short)0); // Reserved
        writer.WriteBigEndian((short)0); // Reserved
        writer.WriteBigEndian((short)0); // Reserved
        writer.WriteBigEndian((short)0); // MetricDataFormat
        writer.WriteBigEndian(numberOfHMetrics); // Updated numberOfHMetrics

        var modifiedData = stream.ToArray();
        return HheaTable.Parse(modifiedData);
    }

    /// <summary>
    /// Create a copy of MaxpTable with updated numGlyphs.
    /// </summary>
    public static MaxpTable CreateMaxpTable(MaxpTable original, ushort numGlyphs)
    {
        // Create a temporary MaxpTable with updated numGlyphs
        // We'll create the modified maxp data and parse it back
        using var stream = new System.IO.MemoryStream();
        using var writer = new System.IO.BinaryWriter(stream);

        // Write the maxp table with updated values
        writer.WriteBigEndian(original.Version);
        writer.WriteBigEndian(numGlyphs); // Updated glyph count

        if (original.Version == 0x00010000)
        {
            // Keep original values for these fields (shouldn't matter for subset)
            writer.WriteBigEndian(original.MaxPoints);
            writer.WriteBigEndian(original.MaxContours);
            writer.WriteBigEndian(original.MaxCompositePoints);
            writer.WriteBigEndian(original.MaxCompositeContours);

            // Write standard values for the rest
            writer.WriteBigEndian((ushort)2); // MaxZones
            writer.WriteBigEndian((ushort)0); // MaxTwilightPoints
            writer.WriteBigEndian((ushort)0); // MaxStorage
            writer.WriteBigEndian((ushort)0); // MaxFunctionDefs
            writer.WriteBigEndian((ushort)0); // MaxInstructionDefs
            writer.WriteBigEndian((ushort)0); // MaxStackElements
            writer.WriteBigEndian((ushort)0); // MaxSizeOfInstructions
            writer.WriteBigEndian((ushort)0); // MaxComponentElements
            writer.WriteBigEndian((ushort)0); // MaxComponentDepth
        }

        var modifiedData = stream.ToArray();
        return MaxpTable.Parse(modifiedData);
    }
}