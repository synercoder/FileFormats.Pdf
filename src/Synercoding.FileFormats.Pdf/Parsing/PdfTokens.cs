using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Parsing;

public record Token(TokenKind TokenKind);
public record TokenBoolean(bool Value) : Token(TokenKind.Boolean);
public record TokenBytes(TokenKind TokenType, byte[] Bytes) : Token(TokenType);
public record TokenNumber(double? DoubleValue, long? LongValue) : Token(TokenKind.Number);
public record TokenName(PdfName Name) : Token(TokenKind.Name);
