using Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.Tables;
using Synercoding.FileFormats.Pdf.IO;

namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.TableWriters;

/// <summary>
/// Writes the 'head' table for a TrueType font.
/// </summary>
internal static class HeadTableWriter
{
    private const uint VERSION_1_0 = 0x00010000;
    private const uint MAGIC_NUMBER = 0x5F0F3CF5;

    /// <summary>
    /// Write a head table to a byte array.
    /// </summary>
    public static byte[] Write(HeadTable head)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);

        // Version (1.0)
        writer.WriteBigEndian(VERSION_1_0);

        // FontRevision (set to 1.0)
        writer.WriteBigEndian(VERSION_1_0);

        // ChecksumAdjustment (placeholder, will be updated later)
        writer.WriteBigEndian(0u);

        // MagicNumber
        writer.WriteBigEndian(MAGIC_NUMBER);

        // Flags
        writer.WriteBigEndian(head.Flags);

        // UnitsPerEm
        writer.WriteBigEndian(head.UnitsPerEm);

        // Created (set to 0 - Jan 1, 1904)
        writer.WriteBigEndian(0L);

        // Modified (set to current time as seconds since Jan 1, 1904)
        var secondsSince1904 = _getSecondsSince1904();
        writer.WriteBigEndian(secondsSince1904);

        // Bounding box
        writer.WriteBigEndian(head.XMin);
        writer.WriteBigEndian(head.YMin);
        writer.WriteBigEndian(head.XMax);
        writer.WriteBigEndian(head.YMax);

        // MacStyle
        writer.WriteBigEndian(head.MacStyle);

        // LowestRecPPEM (set to 8)
        writer.WriteBigEndian((ushort)8);

        // FontDirectionHint (2 = strongly left to right)
        writer.WriteBigEndian((short)2);

        // IndexToLocFormat
        writer.WriteBigEndian(head.IndexToLocFormat);

        // GlyphDataFormat (0)
        writer.WriteBigEndian((short)0);

        return stream.ToArray();
    }

    private static long _getSecondsSince1904()
    {
        // TrueType uses seconds since Jan 1, 1904
        var epoch1904 = new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var now = DateTime.UtcNow;
        var timeSpan = now - epoch1904;
        return (long)timeSpan.TotalSeconds;
    }
}