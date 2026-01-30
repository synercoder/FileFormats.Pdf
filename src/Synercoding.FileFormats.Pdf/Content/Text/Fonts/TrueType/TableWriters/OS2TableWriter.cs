using Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.Tables;

namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.TableWriters;

/// <summary>
/// Writes the 'OS/2' table for a TrueType font.
/// </summary>
internal static class OS2TableWriter
{
    /// <summary>
    /// Write an OS/2 table to a byte array.
    /// </summary>
    public static byte[] Write(OS2Table os2)
    {
        // For now, we'll skip writing the OS/2 table in subsets
        // A full implementation would rebuild the OS/2 table with updated metrics
        throw new NotImplementedException("OS2Table writing is not yet implemented for font subsetting");
    }
}