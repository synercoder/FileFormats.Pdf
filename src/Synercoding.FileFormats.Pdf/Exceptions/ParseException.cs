using Synercoding.FileFormats.Pdf.Parsing;

namespace Synercoding.FileFormats.Pdf.Exceptions;

public class ParseException : PdfException
{
    internal ParseException(byte expected, byte found)
        : this($"Unexpected byte, expected {expected:X2}, but got {found:X2}.")
    { }

    internal ParseException(byte found, string expected)
        : this($"Unexpected byte. {expected} But found {found:X2}.")
    { }

    internal ParseException(byte expectedStart, byte expectedEnd, byte found)
        : this($"Unexpected byte, expected byte in range {expectedStart:X2} - {expectedEnd:X2}, but got {found:X2}.")
    { }

    internal ParseException(TokenType expected, TokenType actual)
        : this($"Unexpected token, expected: {expected}, actual: {actual}.")
    { }

    internal ParseException(Type expected, Type actual)
        : this($"Unexpected token, expected: {expected}, actual: {actual}.")
    { }

    internal ParseException(string? message)
        : base(message)
    { }

    internal static ParseException UnexpectedEOF()
        => new ParseException("Unexpected end of pdf data.");
}
