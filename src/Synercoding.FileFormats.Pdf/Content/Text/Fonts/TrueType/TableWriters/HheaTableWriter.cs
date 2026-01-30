using Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.Tables;
using Synercoding.FileFormats.Pdf.IO;

namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.TableWriters;

/// <summary>
/// Writes the 'hhea' (Horizontal Header) table for a TrueType font.
/// </summary>
internal static class HheaTableWriter
{
    private const uint VERSION_1_0 = 0x00010000;

    /// <summary>
    /// Write a hhea table to a byte array.
    /// </summary>
    public static byte[] Write(HheaTable hhea)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);

        // Version (1.0)
        writer.WriteBigEndian(VERSION_1_0);

        // Ascender
        writer.WriteBigEndian(hhea.Ascender);

        // Descender
        writer.WriteBigEndian(hhea.Descender);

        // LineGap
        writer.WriteBigEndian(hhea.LineGap);

        // AdvanceWidthMax
        writer.WriteBigEndian(hhea.AdvanceWidthMax);

        // MinLeftSideBearing
        writer.WriteBigEndian(hhea.MinLeftSideBearing);

        // MinRightSideBearing
        writer.WriteBigEndian(hhea.MinRightSideBearing);

        // XMaxExtent
        writer.WriteBigEndian(hhea.XMaxExtent);

        // CaretSlopeRise (1 for upright)
        writer.WriteBigEndian((short)1);

        // CaretSlopeRun (0 for upright)
        writer.WriteBigEndian((short)0);

        // CaretOffset
        writer.WriteBigEndian((short)0);

        // Reserved (4 shorts = 8 bytes)
        writer.WriteBigEndian((short)0);
        writer.WriteBigEndian((short)0);
        writer.WriteBigEndian((short)0);
        writer.WriteBigEndian((short)0);

        // MetricDataFormat (0)
        writer.WriteBigEndian((short)0);

        // NumberOfHMetrics
        writer.WriteBigEndian(hhea.NumberOfHMetrics);

        return stream.ToArray();
    }
}