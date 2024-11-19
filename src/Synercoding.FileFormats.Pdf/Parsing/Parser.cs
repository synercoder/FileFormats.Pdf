using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Logging;
using Synercoding.FileFormats.Pdf.Parsing.Internal;
using Synercoding.FileFormats.Pdf.Primitives;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Parsing;

public class Parser
{
    private readonly IPdfLogger? _logger;

    public Parser(Tokenizer tokenizer)
        : this(tokenizer, null)
    { }

    internal Parser(Tokenizer tokenizer, IPdfLogger? logger)
    {
        Tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
        _logger = logger;
    }

    public Tokenizer Tokenizer { get; }

    public PdfObject ReadObject()
    {
        var id = ReadInteger();
        var generation = ReadInteger();

        Tokenizer.ReadOrThrow(TokenType.Obj);

        var pdfObject = ReadNext();

        if (pdfObject is null)
        {
            _logger.LogError<Parser>("Could not read object ");
            throw ParseException.UnexpectedEOF();
        }

        Tokenizer.ReadOrThrow(TokenType.EndObj);

        return new PdfObject()
        {
            Id = new PdfObjectId((int)id, (int)generation),
            Value = pdfObject
        };
    }

    public PdfObject<T> ReadObject<T>()
        where T : IPdfPrimitive
    {
        var id = ReadInteger();
        var generation = ReadInteger();

        Tokenizer.ReadOrThrow(TokenType.Obj);

        var pdfObject = ReadNext() ?? throw ParseException.UnexpectedEOF();

        if (pdfObject is not T correctType)
            throw new ParseException($"Pdf object is not of the correct type, expected: {typeof(T).Name}, retrieved: {pdfObject?.GetType()?.Name}.");

        Tokenizer.ReadOrThrow(TokenType.EndObj);

        return new PdfObject<T>()
        {
            Id = new PdfObjectId((int)id, (int)generation),
            Value = correctType
        };
    }

    public long ReadInteger()
    {
        var token = Tokenizer.ReadOrThrow<TokenNumber>();
        if (token.Fractional)
            throw new ParseException("Expected an integer, but found a real.");

        return (long)token.Value;
    }

    public bool ReadBoolean()
        => Tokenizer.ReadOrThrow<TokenBoolean>().Value;

    public IPdfDictionary ReadDictionaryOrStream()
    {
        var dictionary = ReadDictionary();

        if (!Tokenizer.TryPeekNextTokenType(out var tokenType) || tokenType != TokenType.Stream)
            return dictionary;

        _ = Tokenizer.ReadOrThrow(TokenType.Stream);

        if (!dictionary.TryGetValue(PdfNames.Length, out var lengthPrimitive) || lengthPrimitive is not PdfInteger lengthInteger)
            throw new ParseException("Pdf dictionary for stream does not contain a Length property that is an integer.");

        Tokenizer.PdfBytesProvider.ReadEndOfLineMarker();

        var streamData = new byte[lengthInteger.Value];
        for (int i = 0; i < lengthInteger.Value; i++)
        {
            streamData[i] = Tokenizer.PdfBytesProvider.ReadByte();
        }

        Tokenizer.PdfBytesProvider.ReadEndOfLineMarker();

        Tokenizer.PdfBytesProvider.TryReadEndOfLineMarker();

        Tokenizer.ReadOrThrow(TokenType.EndStream);

        return new PdfStreamObject(dictionary, streamData);
    }

    public IPdfStreamObject ReadStreamObject()
    {
        var dictionary = ReadDictionaryOrStream();

        if (dictionary is IPdfStreamObject streamObject)
            return streamObject;

        throw new ParseException("When parsing for a stream object, a stream token was expected but not encountered.");
    }

    public IPdfDictionary ReadDictionary()
    {
        _ = Tokenizer.ReadOrThrow(TokenType.BeginDictionary);

        var dictionary = new PdfDictionary();
        while (Tokenizer.TryPeekNextTokenType(out TokenType? nextTokenType) && nextTokenType != TokenType.EndDictionary)
        {
            var nameToken = Tokenizer.ReadOrThrow<TokenName>();

            var value = ReadNext();

            if (value is not null)
                dictionary.Add(nameToken.Name, value);
        }

        Tokenizer.ReadOrThrow(TokenType.EndDictionary);

        return dictionary;
    }

    public IPdfPrimitive? ReadNext(TokenType? nextTokenType = null)
    {
        if (nextTokenType is null && !Tokenizer.TryPeekNextTokenType(out nextTokenType))
        {
            throw ParseException.UnexpectedEOF();
        }

        return nextTokenType switch
        {
            TokenType.BeginDictionary => ReadDictionaryOrStream(),
            TokenType.BeginArray => ReadArray(),
            TokenType.Boolean => new PdfBoolean(ReadBoolean()),
            TokenType.Number => _readReferenceOrNumber(),
            TokenType.StringHex => ReadStringHex(),
            TokenType.StringLiteral => ReadStringLiteral(),
            TokenType.Name => Tokenizer.ReadOrThrow<TokenName>().Name,
            TokenType.Null => null,
            _ => throw new ParseException($"Unexpected token type, expected a value but found: {nextTokenType}")
        };
    }

    private IPdfPrimitive _readReferenceOrNumber()
    {
        var firstNumber = Tokenizer.ReadOrThrow<TokenNumber>();
        if (firstNumber.Fractional)
            return new PdfReal(firstNumber.Value);

        var firstInteger = (int)firstNumber.Value;

        var position = Tokenizer.Position;

        if (!Tokenizer.TryGetNextAs<TokenNumber>(out var secondIntToken))
        {
            Tokenizer.Position = position;
            return firstNumber.Fractional
                ? new PdfReal(firstNumber.Value)
                : new PdfInteger(firstInteger);
        }

        if (!Tokenizer.TryGetNextToken(out var thirdToken) || thirdToken.TokenType != TokenType.Reference)
        {
            Tokenizer.Position = position;
            return firstNumber.Fractional
                ? new PdfReal(firstNumber.Value)
                : new PdfInteger(firstInteger);
        }

        return new PdfReference()
        {
            Id = new PdfObjectId()
            {
                Id = firstInteger,
                Generation = (int)secondIntToken.Value
            }
        };
    }


    public PdfArray ReadArray()
    {
        _ = Tokenizer.ReadOrThrow(TokenType.BeginArray);

        var array = new PdfArray();
        while (Tokenizer.TryPeekNextTokenType(out TokenType? nextTokenType) && nextTokenType != TokenType.EndArray)
        {
            var value = ReadNext(nextTokenType);

            if (value is not null)
                array.Add(value);
        }

        Tokenizer.ReadOrThrow(TokenType.EndArray);

        return array;
    }

    public PdfReference ReadReference()
    {
        long id = ReadInteger();
        long generation = ReadInteger();
        _ = Tokenizer.ReadOrThrow(TokenType.Reference, false);

        return new PdfReference()
        {
            Id = new PdfObjectId()
            {
                Id = (int)id,
                Generation = (int)generation
            }
        };
    }

    public PdfString ReadStringHex()
        => _readString(true);

    public PdfString ReadStringLiteral()
        => _readString(false);

    private PdfString _readString(bool isHex)
    {
        var stringToken = Tokenizer.ReadOrThrow<TokenBytes>();

        var expectedType = isHex
            ? TokenType.StringHex
            : TokenType.StringLiteral;

        if (stringToken.TokenType != expectedType)
        {
            throw new ParseException(expectedType, stringToken.TokenType);
        }

        var (value, encoding) = stringToken.Bytes switch
        {
        [0xFE, 0xFF, ..] => (Encoding.BigEndianUnicode.GetString(stringToken.Bytes, 2, stringToken.Bytes.Length - 2), PdfStringEncoding.Utf16BE),
        [0xFF, 0xFE, ..] => (Encoding.Unicode.GetString(stringToken.Bytes, 2, stringToken.Bytes.Length - 2), PdfStringEncoding.Utf16LE),
        [0xEF, 0xBB, 0xBF, ..] => (Encoding.Unicode.GetString(stringToken.Bytes, 3, stringToken.Bytes.Length - 3), PdfStringEncoding.Utf8),
            _ => (PDFDocEncoding.Decode(stringToken.Bytes), PdfStringEncoding.PdfDocEncoding)
        };

        return new PdfString(value, encoding, isHex);
    }
}
