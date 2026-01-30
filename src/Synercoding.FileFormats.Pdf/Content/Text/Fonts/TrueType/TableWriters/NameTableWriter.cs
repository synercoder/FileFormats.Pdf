using Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.Tables;

namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.TableWriters;

/// <summary>
/// Writes the 'name' table for a TrueType font.
/// </summary>
internal static class NameTableWriter
{
    /// <summary>
    /// Write a name table to a byte array.
    /// </summary>
    public static byte[] Write(NameTable name)
    {
        // For now, we'll skip writing the name table in subsets
        // A full implementation would rebuild the name table from the name records
        throw new NotImplementedException("NameTable writing is not yet implemented for font subsetting");
    }
}