namespace Synercoding.FileFormats.Pdf.Exceptions;

public class UnexpectedByteException : ParseException
{
    internal UnexpectedByteException(byte expected, byte found)
        : base($"Unexpected byte, expected {expected:X2}, but got {found:X2}.")
    { }

    internal UnexpectedByteException(byte found, string expected)
        : base($"Unexpected byte. {expected} But found {found:X2}.")
    { }

    internal UnexpectedByteException(byte expectedStart, byte expectedEnd, byte found)
        : base($"Unexpected byte, expected byte in range {expectedStart:X2} - {expectedEnd:X2}, but got {found:X2}.")
    { }

    internal UnexpectedByteException(Type expected, Type actual)
        : base($"Unexpected token, expected: {expected}, actual: {actual}.")
    { }
}
