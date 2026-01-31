using Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.Tables;
using Synercoding.FileFormats.Pdf.IO;

namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.TableWriters;

/// <summary>
/// Writes the 'maxp' (Maximum Profile) table for a TrueType font.
/// </summary>
internal static class MaxpTableWriter
{
    /// <summary>
    /// Write a maxp table to a byte array.
    /// </summary>
    public static byte[] Write(MaxpTable maxp)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);

        // Version
        writer.WriteBigEndian(maxp.Version);

        // NumGlyphs
        writer.WriteBigEndian(maxp.NumGlyphs);

        // If version 1.0, write additional fields
        if (maxp.Version == 0x00010000)
        {
            // MaxPoints
            writer.WriteBigEndian(maxp.MaxPoints);

            // MaxContours
            writer.WriteBigEndian(maxp.MaxContours);

            // MaxCompositePoints
            writer.WriteBigEndian(maxp.MaxCompositePoints);

            // MaxCompositeContours
            writer.WriteBigEndian(maxp.MaxCompositeContours);

            // MaxZones (always 2)
            writer.WriteBigEndian((ushort)2);

            // MaxTwilightPoints (0)
            writer.WriteBigEndian((ushort)0);

            // MaxStorage (0)
            writer.WriteBigEndian((ushort)0);

            // MaxFunctionDefs (0)
            writer.WriteBigEndian((ushort)0);

            // MaxInstructionDefs (0)
            writer.WriteBigEndian((ushort)0);

            // MaxStackElements (0)
            writer.WriteBigEndian((ushort)0);

            // MaxSizeOfInstructions (0)
            writer.WriteBigEndian((ushort)0);

            // MaxComponentElements (0)
            writer.WriteBigEndian((ushort)0);

            // MaxComponentDepth (0)
            writer.WriteBigEndian((ushort)0);
        }

        return stream.ToArray();
    }
}