using Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.Tables;
using Synercoding.FileFormats.Pdf.IO;

namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.TableWriters;

/// <summary>
/// Writes the 'loca' (Index to Location) table for a TrueType font.
/// </summary>
internal static class LocaTableWriter
{
    /// <summary>
    /// Write a loca table to a byte array.
    /// </summary>
    public static byte[] Write(LocaTable loca, short indexToLocFormat)
    {
        var offsets = loca.GetOffsets();
        return Write(offsets, indexToLocFormat);
    }

    /// <summary>
    /// Write a loca table from explicit offset data.
    /// </summary>
    public static byte[] Write(uint[] offsets, short indexToLocFormat)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);

        if (indexToLocFormat == 0)
        {
            // Short format: store as 16-bit values divided by 2
            foreach (var offset in offsets)
            {
                writer.WriteBigEndian((ushort)( offset / 2 ));
            }
        }
        else if (indexToLocFormat == 1)
        {
            // Long format: store as 32-bit values
            foreach (var offset in offsets)
            {
                writer.WriteBigEndian(offset);
            }
        }
        else
        {
            throw new ArgumentException($"Invalid indexToLocFormat: {indexToLocFormat}", nameof(indexToLocFormat));
        }

        return stream.ToArray();
    }
}