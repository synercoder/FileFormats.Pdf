using Synercoding.FileFormats.Pdf.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.IO;

internal static class IPdfBytesProviderExtensions
{
    public static IPdfBytesProvider ReadEndOfLineMarker(this IPdfBytesProvider bytesProvider)
    {
        var nextByte = bytesProvider.ReadByte();
        if (nextByte == ByteUtils.CARRIAGE_RETURN)
            nextByte = bytesProvider.ReadByte();

        if (nextByte != ByteUtils.LINE_FEED)
            throw new UnexpectedByteException(ByteUtils.LINE_FEED, nextByte);

        return bytesProvider;
    }

    public static bool TryReadEndOfLineMarker(this IPdfBytesProvider bytesProvider)
    {
        var position = bytesProvider.Position;

        var nextByte = bytesProvider.ReadByte();
        if (nextByte == ByteUtils.CARRIAGE_RETURN)
            nextByte = bytesProvider.ReadByte();

        if(nextByte != ByteUtils.LINE_FEED)
            bytesProvider.Seek(position, SeekOrigin.Begin);

        return nextByte == ByteUtils.LINE_FEED;
    }

    public static IPdfBytesProvider SkipWhiteSpace(this IPdfBytesProvider pdfBytesProvider)
    {
        while (pdfBytesProvider.TryPeek(out byte b) && ByteUtils.IsWhiteSpace(b))
            pdfBytesProvider.Skip();

        return pdfBytesProvider;
    }

    public static IPdfBytesProvider Skip(this IPdfBytesProvider pdfBytesProvider, int amount = 1)
    {
        if (amount < 1)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be atleast 1.");

        for(int i = 0; i < amount; i++)
        {
            if (!pdfBytesProvider.TryRead(out _))
                throw new UnexpectedEndOfFileException();
        }

        return pdfBytesProvider;
    }

    public static IPdfBytesProvider SkipOrThrow(this IPdfBytesProvider pdfBytesProvider, byte expected)
    {
        if (!pdfBytesProvider.TryRead(out byte actual))
            throw new UnexpectedEndOfFileException();

        if (actual != expected)
            throw new UnexpectedByteException(expected, actual);

        return pdfBytesProvider;
    }

    public static IPdfBytesProvider SkipOrThrow(this IPdfBytesProvider pdfBytesProvider, char expected)
    {
        if (expected > 0xFF)
            throw new ArgumentOutOfRangeException(nameof(expected), "Only chars in byte range (0x00-0xFF) can be used.");
        return pdfBytesProvider.SkipOrThrow((byte)expected);
    }

    public static bool TryRead(this IPdfBytesProvider pdfBytesProvider, int length, [NotNullWhen(true)] out byte[] bytes)
    {
        if (pdfBytesProvider.Position + length > pdfBytesProvider.Length)
        {
            bytes = Array.Empty<byte>();
            return false;
        }

        bytes = new byte[length];
        return pdfBytesProvider.TryRead(bytes, 0, length);
    }

    public static bool TryPeek(this IPdfBytesProvider pdfBytesProvider, out byte b)
    {
        var position = pdfBytesProvider.Position;

        try
        {
            return pdfBytesProvider.TryRead(out b);
        }
        finally
        {
            pdfBytesProvider.Seek(position, SeekOrigin.Begin);
        }
    }

    public static bool TryPeek(this IPdfBytesProvider pdfBytesProvider, int length, [NotNullWhen(true)] out byte[] bytes)
    {
        var position = pdfBytesProvider.Position;

        try
        {
            return pdfBytesProvider.TryRead(length, out bytes);
        }
        finally
        {
            pdfBytesProvider.Seek(position, SeekOrigin.Begin);
        }
    }

    public static byte ReadOctalAsByte(this IPdfBytesProvider pdfBytesProvider)
    {
        // Eat the backslash when it is at the first position
        if (pdfBytesProvider.TryPeek(out byte b) && b == 0x5C)
            _ = pdfBytesProvider.ReadByte();

        var b1 = pdfBytesProvider.ReadByte();

        if (!ByteUtils.IsOctal(b1))
            throw new UnexpectedByteException(0x30, 0x37, b1);

        if (pdfBytesProvider.TryPeek(2, out var bytes) && ByteUtils.IsOctal(bytes[1]) && ByteUtils.IsOctal(bytes[0]))
        {
            pdfBytesProvider.Skip(2);
            int value = ( ( b1 - '0' ) << 6 )
                | ( ( bytes[0] - '0' ) << 3 )
                | ( ( bytes[1] - '0' ) << 0 );

            if (value > 0xFF)
            {
                // PDF 2.0: 7.3.4.2: The number ddd may consist of one, two, or three octal digits; high-order overflow shall be ignored.
                return (byte)( value & 0xFF );
            }

            return (byte)value;
        }
        else if (pdfBytesProvider.TryPeek(out byte next) && ByteUtils.IsOctal(next))
        {
            pdfBytesProvider.Skip(1);
            return (byte)( ( ( b1 - '0' ) << 3 ) | ( next - '0' ) );
        }
        else
        {
            return (byte)( b1 - '0' );
        }
    }

    public static byte ReadByte(this IPdfBytesProvider pdfBytesProvider)
    {
        if (!pdfBytesProvider.TryRead(out byte b))
            throw new UnexpectedEndOfFileException();
        return b;
    }
}
