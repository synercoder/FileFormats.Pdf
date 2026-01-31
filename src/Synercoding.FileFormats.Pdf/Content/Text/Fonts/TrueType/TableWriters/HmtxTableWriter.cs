using Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.Tables;
using Synercoding.FileFormats.Pdf.IO;

namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.TableWriters;

/// <summary>
/// Writes the 'hmtx' (Horizontal Metrics) table for a TrueType font.
/// </summary>
internal static class HmtxTableWriter
{
    /// <summary>
    /// Write a hmtx table to a byte array.
    /// </summary>
    public static byte[] Write(HmtxTable hmtx)
    {
        var advanceWidths = hmtx.GetAdvanceWidths();
        var leftSideBearings = hmtx.GetLeftSideBearings();

        // Calculate numberOfHMetrics (number of entries with unique advance widths)
        var numberOfHMetrics = advanceWidths.Length;
        if (numberOfHMetrics > 1)
        {
            // Find the last unique advance width
            var lastWidth = advanceWidths[numberOfHMetrics - 1];
            while (numberOfHMetrics > 1 && advanceWidths[numberOfHMetrics - 2] == lastWidth)
            {
                numberOfHMetrics--;
            }
        }

        return Write(advanceWidths, leftSideBearings, (ushort)numberOfHMetrics);
    }

    /// <summary>
    /// Write a hmtx table from explicit metrics data.
    /// </summary>
    public static byte[] Write(ushort[] advanceWidths, short[] leftSideBearings, ushort numberOfHMetrics)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);

        // Write the metrics for numberOfHMetrics glyphs
        for (int i = 0; i < numberOfHMetrics && i < advanceWidths.Length; i++)
        {
            writer.WriteBigEndian(advanceWidths[i]);
            writer.WriteBigEndian(leftSideBearings[i]);
        }

        // Write remaining left side bearings (if any)
        for (int i = numberOfHMetrics; i < leftSideBearings.Length; i++)
        {
            writer.WriteBigEndian(leftSideBearings[i]);
        }

        return stream.ToArray();
    }
}