using Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.Tables;

namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.TableWriters;

/// <summary>
/// Writes the 'glyf' (Glyph Data) table for a TrueType font.
/// </summary>
internal static class GlyfTableWriter
{
    /// <summary>
    /// Write a glyf table to a byte array.
    /// </summary>
    public static byte[] Write(GlyfTable glyf)
    {
        // For now, just return the raw data
        // In a subsetting scenario, we would filter and reorder glyphs
        return glyf.GetRawData();
    }

    /// <summary>
    /// Write a glyf table from individual glyph data.
    /// </summary>
    public static byte[] Write(List<byte[]?> glyphDataList)
    {
        using var stream = new MemoryStream();

        foreach (var glyphData in glyphDataList)
        {
            if (glyphData != null && glyphData.Length > 0)
            {
                stream.Write(glyphData, 0, glyphData.Length);

                // Pad to 4-byte boundary if necessary
                var padding = ( 4 - ( glyphData.Length % 4 ) ) % 4;
                if (padding > 0)
                {
                    stream.Write(new byte[padding], 0, padding);
                }
            }
            // Empty glyphs don't take any space in the glyf table
        }

        return stream.ToArray();
    }
}