using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Logging;
using Synercoding.FileFormats.Pdf.Parsing.Internal;
using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.FileFormats.Pdf.Primitives.Internal;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Parsing;

public class Parser
{
    private readonly IPdfLogger? _logger;

    public Parser(Lexer lexer)
        : this(lexer, null)
    { }

    internal Parser(Lexer lexer, IPdfLogger? logger)
    {
        Lexer = lexer ?? throw new ArgumentNullException(nameof(lexer));
        _logger = logger;
    }

    internal Lexer Lexer { get; }

    public PdfObject ReadObject()
    {
        var id = ReadInteger();
        var generation = ReadInteger();

        Lexer.ReadOrThrow(TokenKind.Obj);

        var pdfObject = ReadNext();

        if (pdfObject is null)
        {
            _logger.LogError<Parser>("Could not read object ");
            throw new UnexpectedEndOfFileException();
        }

        Lexer.ReadOrThrow(TokenKind.EndObj);

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

        Lexer.ReadOrThrow(TokenKind.Obj);

        var pdfObject = ReadNext() ?? throw new UnexpectedEndOfFileException();

        if (pdfObject is not T correctType)
            throw new ParseException($"Pdf object is not of the correct type, expected: {typeof(T).Name}, retrieved: {pdfObject?.GetType()?.Name}.");

        Lexer.ReadOrThrow(TokenKind.EndObj);

        return new PdfObject<T>()
        {
            Id = new PdfObjectId((int)id, (int)generation),
            Value = correctType
        };
    }

    public long ReadInteger()
    {
        var token = Lexer.ReadOrThrow<TokenNumber>();
        if (token.DoubleValue.HasValue)
            throw new ParseException("Expected an integer, but found a real.");

        return token.LongValue!.Value;
    }

    public bool ReadBoolean()
        => Lexer.ReadOrThrow<TokenBoolean>().Value;

    public IPdfDictionary ReadDictionaryOrStream()
    {
        var dictionary = ReadDictionary();

        if (!Lexer.TryPeekNextTokenKind(out var tokenKind) || tokenKind != TokenKind.Stream)
            return dictionary;

        _ = Lexer.ReadOrThrow(TokenKind.Stream);

        if (!dictionary.TryGetValue(PdfNames.Length, out var lengthPrimitive) || lengthPrimitive is not PdfNumber lengthNumber)
            throw new ParseException("Pdf dictionary for stream does not contain a Length property that is an integer.");

        Lexer.PdfBytesProvider.TryReadEndOfLineMarker();

        var streamData = new byte[(int)lengthNumber.Value];
        for (int i = 0; i < lengthNumber.Value; i++)
        {
            streamData[i] = Lexer.PdfBytesProvider.ReadByte();
        }

        Lexer.PdfBytesProvider.TryReadEndOfLineMarker();
        Lexer.PdfBytesProvider.TryReadEndOfLineMarker();

        Lexer.ReadOrThrow(TokenKind.EndStream);

        return new ReadOnlyPdfStream(dictionary, streamData);
    }

    public IPdfStream ReadStreamObject()
    {
        var dictionary = ReadDictionaryOrStream();

        if (dictionary is IPdfStream streamObject)
            return streamObject;

        throw new ParseException("When parsing for a stream object, a stream token was expected but not encountered.");
    }

    public IPdfDictionary ReadDictionary()
    {
        _ = Lexer.ReadOrThrow(TokenKind.BeginDictionary);

        var dictionary = new PdfDictionary();
        while (Lexer.TryPeekNextTokenKind(out TokenKind? nextTokenKind) && nextTokenKind != TokenKind.EndDictionary)
        {
            var nameToken = Lexer.ReadOrThrow<TokenName>();

            var value = ReadNext();

            if (value is not null)
                dictionary.Add(nameToken.Name, value);
        }

        Lexer.ReadOrThrow(TokenKind.EndDictionary);

        return new ReadOnlyPdfDictionary(dictionary);
    }

    public IPdfPrimitive ReadNext(TokenKind? nextTokenKind = null)
    {
        if (nextTokenKind is null && !Lexer.TryPeekNextTokenKind(out nextTokenKind))
        {
            throw new UnexpectedEndOfFileException();
        }

        return nextTokenKind switch
        {
            TokenKind.BeginDictionary => ReadDictionaryOrStream(),
            TokenKind.BeginArray => ReadArray(),
            TokenKind.Boolean => new PdfBoolean(ReadBoolean()),
            TokenKind.Number => _readReferenceOrNumber(),
            TokenKind.StringHex => ReadStringHex(),
            TokenKind.StringLiteral => ReadStringLiteral(),
            TokenKind.Name => Lexer.ReadOrThrow<TokenName>().Name,
            TokenKind.Null => Lexer.ReadOrThrow<TokenNull>().Instance,
            _ => throw new UnexpectedTokenException($"Unexpected token type, expected a value but found: {nextTokenKind}")
        };
    }

    private IPdfPrimitive _readReferenceOrNumber()
    {
        var firstNumber = Lexer.ReadOrThrow<TokenNumber>();
        if (firstNumber.DoubleValue.HasValue)
            return new PdfNumber(firstNumber.DoubleValue.Value);

        var firstInteger = firstNumber.LongValue!.Value;

        var position = Lexer.Position;

        if (!Lexer.TryGetNextAs<TokenNumber>(out var secondIntToken) || secondIntToken.DoubleValue.HasValue)
        {
            Lexer.Position = position;
            return firstNumber.DoubleValue.HasValue
                ? new PdfNumber(firstNumber.DoubleValue.Value)
                : new PdfNumber(firstNumber.LongValue.Value);
        }

        if (!Lexer.TryGetNextToken(out var thirdToken) || thirdToken.TokenKind != TokenKind.Reference)
        {
            Lexer.Position = position;
            return firstNumber.DoubleValue.HasValue
                ? new PdfNumber(firstNumber.DoubleValue.Value)
                : new PdfNumber(firstNumber.LongValue.Value);
        }

        return new PdfReference()
        {
            Id = new PdfObjectId()
            {
                ObjectNumber = (int)firstInteger,
                Generation = (int)secondIntToken.LongValue!.Value
            }
        };
    }


    public IPdfArray ReadArray()
    {
        _ = Lexer.ReadOrThrow(TokenKind.BeginArray);

        var array = new PdfArray();
        while (Lexer.TryPeekNextTokenKind(out TokenKind? nextTokenKind) && nextTokenKind != TokenKind.EndArray)
        {
            var value = ReadNext(nextTokenKind);

            array.Add(value);
        }

        Lexer.ReadOrThrow(TokenKind.EndArray);

        return new ReadOnlyPdfArray(array);
    }

    public PdfReference ReadReference()
    {
        long id = ReadInteger();
        long generation = ReadInteger();
        _ = Lexer.ReadOrThrow(TokenKind.Reference, false);

        return new PdfReference()
        {
            Id = new PdfObjectId()
            {
                ObjectNumber = (int)id,
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
        var stringToken = Lexer.ReadOrThrow<TokenBytes>();

        var expectedType = isHex
            ? TokenKind.StringHex
            : TokenKind.StringLiteral;

        if (stringToken.TokenKind != expectedType)
        {
            throw new UnexpectedTokenException(expectedType, stringToken.TokenKind);
        }

        var bytes = stringToken.Bytes;

        if (isHex)
            bytes = Convert.FromHexString(Encoding.ASCII.GetString(bytes));

        return new PdfString(bytes, isHex);
    }
}
