using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Parsing;

public record Token(TokenType TokenType);
public record TokenBoolean(bool Value) : Token(TokenType.Boolean);
public record TokenBytes(TokenType TokenType, byte[] Bytes) : Token(TokenType);
public record TokenInteger(long Number) : Token(TokenType.Integer);
public record TokenReal(double Number) : Token(TokenType.Real);
public record TokenName(PdfName Name) : Token(TokenType.Name);
