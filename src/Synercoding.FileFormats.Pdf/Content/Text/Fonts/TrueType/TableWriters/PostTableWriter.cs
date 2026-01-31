using Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.Tables;
using Synercoding.FileFormats.Pdf.IO;

namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.TableWriters;

/// <summary>
/// Writes the 'post' table for a TrueType font.
/// </summary>
internal static class PostTableWriter
{
    /// <summary>
    /// Write a post table to a byte array.
    /// </summary>
    public static byte[] Write(PostTable post)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);

        // Version (3.0 = no glyph names)
        writer.WriteBigEndian(0x00030000u);

        // ItalicAngle (as Fixed 16.16)
        var italicAngleFixed = (int)( post.ItalicAngle * 65536.0 );
        writer.WriteBigEndian(italicAngleFixed);

        // UnderlinePosition
        writer.WriteBigEndian(post.UnderlinePosition);

        // UnderlineThickness
        writer.WriteBigEndian(post.UnderlineThickness);

        // IsFixedPitch
        writer.WriteBigEndian(post.IsFixedPitch ? 1u : 0u);

        // MinMemType42
        writer.WriteBigEndian(0u);

        // MaxMemType42
        writer.WriteBigEndian(0u);

        // MinMemType1
        writer.WriteBigEndian(0u);

        // MaxMemType1
        writer.WriteBigEndian(0u);

        // Version 3.0 has no additional data (no glyph names)

        return stream.ToArray();
    }
}