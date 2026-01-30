using Synercoding.FileFormats.Pdf.IO;

namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.Tables;

/// <summary>
/// Represents the 'post' table in a TrueType font
/// </summary>
internal sealed class PostTable
{
    /// <summary>
    /// Parse a post table from bytes
    /// </summary>
    public static PostTable Parse(ReadOnlySpan<byte> data)
    {
        if (data.Length < 32)
            throw new InvalidOperationException("Post table too short");

        var table = new PostTable
        {
            // Version (4 bytes)
            Version = ByteUtils.ReadUInt32BigEndian(data, 0)
        };

        // ItalicAngle (4 bytes) - fixed point 16.16
        var italicAngleRaw = ByteUtils.ReadInt32BigEndian(data, 4);
        table.ItalicAngle = italicAngleRaw / 65536.0;

        // UnderlinePosition (2 bytes)
        table.UnderlinePosition = ByteUtils.ReadInt16BigEndian(data, 8);

        // UnderlineThickness (2 bytes)
        table.UnderlineThickness = ByteUtils.ReadInt16BigEndian(data, 10);

        // IsFixedPitch (4 bytes)
        table.IsFixedPitch = ByteUtils.ReadUInt32BigEndian(data, 12) != 0;

        // MinMemType42 (4 bytes) - skip
        // MaxMemType42 (4 bytes) - skip
        // MinMemType1 (4 bytes) - skip
        // MaxMemType1 (4 bytes) - skip

        // Additional data depends on version, but we don't need it for basic functionality

        return table;
    }

    /// <summary>
    /// Gets the version
    /// </summary>
    public uint Version { get; private set; }

    /// <summary>
    /// Gets the italic angle
    /// </summary>
    public double ItalicAngle { get; private set; }

    /// <summary>
    /// Gets the underline position
    /// </summary>
    public short UnderlinePosition { get; private set; }

    /// <summary>
    /// Gets the underline thickness
    /// </summary>
    public short UnderlineThickness { get; private set; }

    /// <summary>
    /// Gets whether this is a fixed pitch font
    /// </summary>
    public bool IsFixedPitch { get; private set; }
}
