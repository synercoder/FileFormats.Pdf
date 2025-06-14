using Synercoding.FileFormats.Pdf.Parsing;

namespace Synercoding.FileFormats.Pdf.Exceptions;

public class UnexpectedTokenException : ParseException
{
    internal UnexpectedTokenException(TokenKind expected, TokenKind actual)
        : base($"Unexpected token, expected: {expected}, actual: {actual}.")
    { }

    internal UnexpectedTokenException(Type expected, Type actual)
        : base($"Unexpected token, expected: {expected}, actual: {actual}.")
    { }

    internal UnexpectedTokenException(string message)
        : base(message)
    { }
}
