using System.Text;

namespace Synercoding.FileFormats.Pdf.IO;

/// <summary>
/// Extension methods for BinaryWriter to support big-endian writing.
/// </summary>
internal static class BinaryWriterExtensions
{
    /// <summary>
    /// Write a 16-bit unsigned integer in big-endian format.
    /// </summary>
    public static void WriteBigEndian(this BinaryWriter writer, ushort value)
    {
        writer.Write((byte)( value >> 8 ));
        writer.Write((byte)value);
    }

    /// <summary>
    /// Write a 32-bit unsigned integer in big-endian format.
    /// </summary>
    public static void WriteBigEndian(this BinaryWriter writer, uint value)
    {
        writer.Write((byte)( value >> 24 ));
        writer.Write((byte)( value >> 16 ));
        writer.Write((byte)( value >> 8 ));
        writer.Write((byte)value);
    }

    /// <summary>
    /// Write a 16-bit signed integer in big-endian format.
    /// </summary>
    public static void WriteBigEndian(this BinaryWriter writer, short value)
    {
        writer.Write((byte)( value >> 8 ));
        writer.Write((byte)value);
    }

    /// <summary>
    /// Write a 32-bit signed integer in big-endian format.
    /// </summary>
    public static void WriteBigEndian(this BinaryWriter writer, int value)
    {
        writer.Write((byte)( value >> 24 ));
        writer.Write((byte)( value >> 16 ));
        writer.Write((byte)( value >> 8 ));
        writer.Write((byte)value);
    }

    /// <summary>
    /// Write a 64-bit signed integer in big-endian format.
    /// </summary>
    public static void WriteBigEndian(this BinaryWriter writer, long value)
    {
        writer.Write((byte)( value >> 56 ));
        writer.Write((byte)( value >> 48 ));
        writer.Write((byte)( value >> 40 ));
        writer.Write((byte)( value >> 32 ));
        writer.Write((byte)( value >> 24 ));
        writer.Write((byte)( value >> 16 ));
        writer.Write((byte)( value >> 8 ));
        writer.Write((byte)value);
    }

    /// <summary>
    /// Write a four-character code (FourCC) tag.
    /// </summary>
    public static void WriteFourCC(this BinaryWriter writer, string tag)
    {
        if (tag.Length != 4 && tag != "OS/2")
            throw new ArgumentException($"Tag must be exactly 4 characters, got '{tag}'", nameof(tag));

        if (tag == "OS/2")
            // Special handling for OS/2 table tag
            writer.Write(Encoding.ASCII.GetBytes("OS/2"));
        else
            writer.Write(Encoding.ASCII.GetBytes(tag));
    }
}
