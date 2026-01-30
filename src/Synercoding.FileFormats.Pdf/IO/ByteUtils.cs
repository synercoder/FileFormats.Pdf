namespace Synercoding.FileFormats.Pdf.IO;

/// <summary>
/// Utility methods and constants for working with bytes in PDF parsing.
/// </summary>
public static class ByteUtils
{
    /// <summary>Opening parenthesis '(' character.</summary>
    public const byte PARENTHESIS_OPEN = (byte)'(';
    /// <summary>Closing parenthesis ')' character.</summary>
    public const byte PARENTHESIS_CLOSED = (byte)')';
    /// <summary>Less than '&lt;' character.</summary>
    public const byte LESS_THAN_SIGN = (byte)'<';
    /// <summary>Greater than '&gt;' character.</summary>
    public const byte GREATER_THAN_SIGN = (byte)'>';
    /// <summary>Left square bracket '[' character.</summary>
    public const byte LEFT_SQUARE_BRACKET = (byte)'[';
    /// <summary>Right square bracket ']' character.</summary>
    public const byte RIGHT_SQUARE_BRACKET = (byte)']';
    /// <summary>Left curly bracket '{' character.</summary>
    public const byte LEFT_CURLY_BRACKET = (byte)'{';
    /// <summary>Right curly bracket '}' character.</summary>
    public const byte RIGHT_CURLY_BRACKET = (byte)'}';
    /// <summary>Forward slash '/' character.</summary>
    public const byte SOLIDUS = (byte)'/';
    /// <summary>Backslash '\\' character.</summary>
    public const byte REVERSE_SOLIDUS = (byte)'\\';
    /// <summary>Percent '%' character.</summary>
    public const byte PERCENT_SIGN = (byte)'%';

    /// <summary>Null character (0x00).</summary>
    public const byte NULL = 0x00;
    /// <summary>Horizontal tab character (0x09).</summary>
    public const byte HORIZONTAL_TAB = 0x09;
    /// <summary>Line feed character (0x0A).</summary>
    public const byte LINE_FEED = 0x0A;
    /// <summary>Form feed character (0x0C).</summary>
    public const byte FORM_FEED = 0x0C;
    /// <summary>Carriage return character (0x0D).</summary>
    public const byte CARRIAGE_RETURN = 0x0D;
    /// <summary>Space character (0x20).</summary>
    public const byte SPACE = 0x20;

    /// <summary>
    /// Converts a stream to a byte array.
    /// </summary>
    /// <param name="stream">The stream to convert.</param>
    /// <returns>A byte array containing the stream's data.</returns>
    public static byte[] ToByteArray(Stream stream)
    {
        if (stream is MemoryStream memoryStream)
            return memoryStream.ToArray();

        if (stream.Length < int.MaxValue)
            _getBytesViaMemoryStreamCopy(stream);

        var originalPosition = stream.Position;

        stream.Position = 0;

        var bytes = new byte[stream.Length];

        long position = 0;

        const int BUFFER_SIZE = 4096;
        var buffer = new byte[BUFFER_SIZE];
        while (true)
        {
            var read = stream.Read(buffer);
            Array.Copy(buffer, 0, bytes, position, read);
            position += read;

            if (read < BUFFER_SIZE)
                break;
        }

        stream.Position = originalPosition;

        return bytes;
    }

    private static byte[] _getBytesViaMemoryStreamCopy(Stream stream)
    {
        using var tempStream = new MemoryStream();

        var position = stream.Position;

        stream.Position = 0;
        stream.CopyTo(tempStream);
        stream.Position = position;

        return tempStream.ToArray();
    }

    /// <summary>
    /// Determines whether the specified byte is a PDF delimiter character.
    /// </summary>
    /// <param name="b">The byte to check.</param>
    /// <param name="insidePostScriptCalculator">Whether the check is inside a PostScript calculator context.</param>
    /// <returns><c>true</c> if the byte is a delimiter; otherwise, <c>false</c>.</returns>
    public static bool IsDelimiter(byte b, bool insidePostScriptCalculator = false)
    {
        return (b, insidePostScriptCalculator) switch
        {
            (LEFT_CURLY_BRACKET, true) => true,
            (RIGHT_CURLY_BRACKET, true) => true,
            (PARENTHESIS_OPEN, _) => true,
            (PARENTHESIS_CLOSED, _) => true,
            (LESS_THAN_SIGN, _) => true,
            (GREATER_THAN_SIGN, _) => true,
            (LEFT_SQUARE_BRACKET, _) => true,
            (RIGHT_SQUARE_BRACKET, _) => true,
            (SOLIDUS, _) => true,
            (PERCENT_SIGN, _) => true,
            _ => false
        };
    }

    /// <summary>
    /// Determines whether the specified byte is a PDF delimiter character or whitespace.
    /// </summary>
    /// <param name="b">The byte to check.</param>
    /// <param name="insidePostScriptCalculator">Whether the check is inside a PostScript calculator context.</param>
    /// <returns><c>true</c> if the byte is a delimiter or whitespace; otherwise, <c>false</c>.</returns>
    public static bool IsDelimiterOrWhiteSpace(byte b, bool insidePostScriptCalculator = false)
        => IsDelimiter(b, insidePostScriptCalculator) || IsWhiteSpace(b);

    /// <summary>
    /// Determines whether the specified byte is a PDF whitespace character.
    /// </summary>
    /// <param name="b">The byte to check.</param>
    /// <returns><c>true</c> if the byte is whitespace; otherwise, <c>false</c>.</returns>
    public static bool IsWhiteSpace(byte b)
    {
        return b switch
        {
            NULL => true,
            HORIZONTAL_TAB => true,
            LINE_FEED => true,
            FORM_FEED => true,
            CARRIAGE_RETURN => true,
            SPACE => true,
            _ => false
        };
    }

    /// <summary>
    /// Determines whether two consecutive bytes form a CRLF (carriage return + line feed) sequence.
    /// </summary>
    /// <param name="b1">The first byte.</param>
    /// <param name="b2">The second byte.</param>
    /// <returns><c>true</c> if the bytes form a CRLF sequence; otherwise, <c>false</c>.</returns>
    public static bool IsCRLF(byte b1, byte b2)
        => b1 == CARRIAGE_RETURN && b2 == LINE_FEED;

    /// <summary>
    /// Determines whether the specified byte represents an octal digit (0-7).
    /// </summary>
    /// <param name="b">The byte to check.</param>
    /// <returns><c>true</c> if the byte is an octal digit; otherwise, <c>false</c>.</returns>
    public static bool IsOctal(byte b)
        => b >= '0' && b <= '7';

    /// <summary>
    /// Determines whether the specified byte represents a hexadecimal digit (0-9, a-f, A-F).
    /// </summary>
    /// <param name="b">The byte to check.</param>
    /// <returns><c>true</c> if the byte is a hexadecimal digit; otherwise, <c>false</c>.</returns>
    public static bool IsHex(byte b)
        => ( b >= '0' && b <= '9' )
        || ( b >= 'a' && b <= 'f' )
        || ( b >= 'A' && b <= 'F' );

    /// <summary>
    /// Determines whether the specified byte represents an alphabetic character (a-z, A-Z).
    /// </summary>
    /// <param name="b">The byte to check.</param>
    /// <returns><c>true</c> if the byte is an alphabetic character; otherwise, <c>false</c>.</returns>
    public static bool IsChar(byte b)
        => ( b >= 'a' && b <= 'z' )
        || ( b >= 'A' && b <= 'Z' );

    /// <summary>
    /// Read a big-endian unsigned 16-bit integer from the specified offset
    /// </summary>
    public static ushort ReadUInt16BigEndian(ReadOnlySpan<byte> data, int offset)
    {
        return (ushort)( ( data[offset] << 8 ) | data[offset + 1] );
    }

    /// <summary>
    /// Read a big-endian unsigned 16-bit integer from the specified offset and advance the offset
    /// </summary>
    public static ushort ReadUInt16BigEndian(ReadOnlySpan<byte> data, ref int offset)
    {
        var value = ReadUInt16BigEndian(data, offset);
        offset += 2;
        return value;
    }

    /// <summary>
    /// Read a big-endian signed 16-bit integer from the specified offset
    /// </summary>
    public static short ReadInt16BigEndian(ReadOnlySpan<byte> data, int offset)
    {
        return (short)( ( data[offset] << 8 ) | data[offset + 1] );
    }

    /// <summary>
    /// Read a big-endian signed 16-bit integer from the specified offset and advance the offset
    /// </summary>
    public static short ReadInt16BigEndian(ReadOnlySpan<byte> data, ref int offset)
    {
        var value = ReadInt16BigEndian(data, offset);
        offset += 2;
        return value;
    }

    /// <summary>
    /// Read a big-endian unsigned 32-bit integer from the specified offset
    /// </summary>
    public static uint ReadUInt32BigEndian(ReadOnlySpan<byte> data, int offset)
    {
        return (uint)( ( data[offset] << 24 ) | ( data[offset + 1] << 16 ) | ( data[offset + 2] << 8 ) | data[offset + 3] );
    }

    /// <summary>
    /// Read a big-endian unsigned 32-bit integer from the specified offset and advance the offset
    /// </summary>
    public static uint ReadUInt32BigEndian(ReadOnlySpan<byte> data, ref int offset)
    {
        var value = ReadUInt32BigEndian(data, offset);
        offset += 4;
        return value;
    }

    /// <summary>
    /// Read a big-endian signed 32-bit integer from the specified offset
    /// </summary>
    public static int ReadInt32BigEndian(ReadOnlySpan<byte> data, int offset)
    {
        return ( data[offset] << 24 ) | ( data[offset + 1] << 16 ) | ( data[offset + 2] << 8 ) | data[offset + 3];
    }

    /// <summary>
    /// Read a big-endian signed 32-bit integer from the specified offset and advance the offset
    /// </summary>
    public static int ReadInt32BigEndian(ReadOnlySpan<byte> data, ref int offset)
    {
        var value = ReadInt32BigEndian(data, offset);
        offset += 4;
        return value;
    }
}
