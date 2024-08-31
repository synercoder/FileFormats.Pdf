using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Exceptions;

public class ParseException : PdfException
{
    internal ParseException(string? message)
        : base(message)
    { }

    [DoesNotReturn]
    internal static void ThrowUnexpectedByte(byte expected, byte found)
        => throw new ParseException($"Unexpected byte, expected {expected:X2}, but got {found:X2}.");

    [DoesNotReturn]
    internal static void ThrowUnexpectedByte(byte found, string expected)
        => throw new ParseException($"Unexpected byte. {expected} But found {found:X2}.");

    [DoesNotReturn]
    internal static void ThrowUnexpectedByte(byte expectedStart, byte expectedEnd, byte found)
        => throw new ParseException($"Unexpected byte, expected byte in range {expectedStart:X2} - {expectedEnd:X2}, but got {found:X2}.");

    [DoesNotReturn]
    internal static void ThrowUnexpectedEOF()
        => throw new ParseException("Unexpected end of pdf data.");
}
