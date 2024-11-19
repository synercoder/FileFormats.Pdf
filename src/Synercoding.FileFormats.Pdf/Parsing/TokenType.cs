namespace Synercoding.FileFormats.Pdf.Parsing;

public enum TokenType
{
    Boolean,
    Number,
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
    Obj,
    EndObj,
    Stream,
    EndStream,
    Trailer,
    Other
}
