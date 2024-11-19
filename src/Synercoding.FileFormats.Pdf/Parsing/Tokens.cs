using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Parsing;

public record Token(TokenType TokenType);
public record TokenBoolean(bool Value) : Token(TokenType.Boolean);
public record TokenBytes(TokenType TokenType, byte[] Bytes) : Token(TokenType);
public record TokenNumber(double Value, bool Fractional) : Token(TokenType.Number);
public record TokenName(PdfName Name) : Token(TokenType.Name);
