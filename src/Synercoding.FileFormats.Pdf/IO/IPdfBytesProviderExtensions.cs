using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.Parsing;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Synercoding.FileFormats.Pdf.IO;

internal static class IPdfBytesProviderExtensions
{
    public static bool IsTrueNext(this IPdfBytesProvider pdfBytesProvider)
        => pdfBytesProvider.TryPeek(4, out var possibleTrue)
            && possibleTrue[3] == 0x65  // e
            && possibleTrue[0] == 0x74  // t
            && possibleTrue[1] == 0x72  // r
            && possibleTrue[2] == 0x75; // u

    public static bool IsNullNext(this IPdfBytesProvider pdfBytesProvider)
        => pdfBytesProvider.TryPeek(4, out var possibleNull)
            && possibleNull[3] == 0x6C  // l
            && possibleNull[0] == 0x6E  // n
            && possibleNull[1] == 0x75  // u
            && possibleNull[2] == 0x6C; // l

    public static bool IsFalseNext(this IPdfBytesProvider pdfBytesProvider)
        => pdfBytesProvider.TryPeek(5, out var possibleFalse)
            && possibleFalse[4] == 0x65  // e
            && possibleFalse[0] == 0x66  // f
            && possibleFalse[1] == 0x61  // a
            && possibleFalse[2] == 0x6C  // l
            && possibleFalse[3] == 0x73; // s

    public static IPdfBytesProvider Skip(this IPdfBytesProvider pdfBytesProvider, int amount = 1)
    {
        if (amount < 1)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be atleast 1.");

        for(int i = 0; i < amount; i++)
        {
            if (!pdfBytesProvider.TryRead(out _))
                ParseException.ThrowUnexpectedEOF();
        }

        return pdfBytesProvider;
    }

    public static IPdfBytesProvider SkipOrThrow(this IPdfBytesProvider pdfBytesProvider, byte expected)
    {
        if (!pdfBytesProvider.TryRead(out byte actual))
            ParseException.ThrowUnexpectedEOF();

        if (actual != expected)
            ParseException.ThrowUnexpectedByte(expected, actual);

        return pdfBytesProvider;
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
            ParseException.ThrowUnexpectedByte(0x30, 0x37, b1);

        if (pdfBytesProvider.TryPeek(2, out var bytes) && ByteUtils.IsOctal(bytes[1]) && ByteUtils.IsOctal(bytes[0]))
        {
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
            ParseException.ThrowUnexpectedEOF();
        return b;
    }
}
