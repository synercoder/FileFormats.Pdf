namespace Synercoding.FileFormats.Pdf.Parsing;

public enum TokenType
{
    Boolean,
    Integer,
    Real,
    StringLiteral,
    StringHex,
    BeginArray,
    EndArray,
    Name,
    Comment,
    BeginDictionary,
    EndDictionary,
    Reference,
    Null,
    Other
}
