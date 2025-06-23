using Synercoding.FileFormats.Pdf.Encryption;
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
    private readonly IDecryptor? _decryptor;
    private readonly IPdfLogger? _logger;

    public Parser(Lexer lexer)
        : this(lexer, null, null)
    { }

    internal Parser(Lexer lexer, IDecryptor? decryptor, IPdfLogger? logger)
    {
        Lexer = lexer ?? throw new ArgumentNullException(nameof(lexer));
        _decryptor = decryptor;
        _logger = logger;
    }

    internal Lexer Lexer { get; }

    public PdfObject<T> ReadObject<T>()
        where T : IPdfPrimitive
    {
        var id = ReadInteger();
        var generation = ReadInteger();

        Lexer.ReadOrThrow(TokenKind.Obj);

        var objId = new PdfObjectId((int)id, (int)generation);

        var pdfObject = ReadNext(objId) ?? throw new UnexpectedEndOfFileException();

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

    public IPdfDictionary ReadDictionaryOrStream(PdfObjectId? forObjectId)
    {
        var dictionary = ReadDictionary(forObjectId);

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

        if (!forObjectId.HasValue || _decryptor is null)
            return new ReadOnlyPdfStreamObject(dictionary, streamData);

        return _decryptor.Decrypt(new ReadOnlyPdfStreamObject(dictionary, streamData), forObjectId.Value);
    }

    public IPdfStreamObject ReadStreamObject(PdfObjectId? forObjectId)
    {
        var dictionary = ReadDictionaryOrStream(forObjectId);

        if (dictionary is IPdfStreamObject streamObject)
            return streamObject;

        throw new ParseException("When parsing for a stream object, a stream token was expected but not encountered.");
    }

    public IPdfDictionary ReadDictionary(PdfObjectId? forObjectId)
    {
        _ = Lexer.ReadOrThrow(TokenKind.BeginDictionary);

        var dictionary = new PdfDictionary();
        while (Lexer.TryPeekNextTokenKind(out TokenKind? nextTokenKind) && nextTokenKind != TokenKind.EndDictionary)
        {
            var nameToken = Lexer.ReadOrThrow<TokenName>();

            var value = ReadNext(forObjectId);

            if (value is not null)
                dictionary.Add(nameToken.Name, value);
        }

        Lexer.ReadOrThrow(TokenKind.EndDictionary);

        return new ReadOnlyPdfDictionary(dictionary);
    }

    public IPdfPrimitive ReadNext(PdfObjectId? forObjectId, TokenKind? nextTokenKind = null)
    {
        if (nextTokenKind is null && !Lexer.TryPeekNextTokenKind(out nextTokenKind))
        {
            throw new UnexpectedEndOfFileException();
        }

        return nextTokenKind switch
        {
            TokenKind.BeginDictionary => ReadDictionaryOrStream(forObjectId),
            TokenKind.BeginArray => ReadArray(forObjectId),
            TokenKind.Boolean => new PdfBoolean(ReadBoolean()),
            TokenKind.Number => _readReferenceOrNumber(),
            TokenKind.StringHex => ReadStringHex(forObjectId),
            TokenKind.StringLiteral => ReadStringLiteral(forObjectId),
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


    public IPdfArray ReadArray(PdfObjectId? forObjectId = null)
    {
        _ = Lexer.ReadOrThrow(TokenKind.BeginArray);

        var array = new PdfArray();
        while (Lexer.TryPeekNextTokenKind(out TokenKind? nextTokenKind) && nextTokenKind != TokenKind.EndArray)
        {
            var value = ReadNext(forObjectId, nextTokenKind);

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

    public PdfString ReadStringHex(PdfObjectId? forObjectId = null)
        => _readString(forObjectId, true);

    public PdfString ReadStringLiteral(PdfObjectId? forObjectId = null)
        => _readString(forObjectId, false);

    private PdfString _readString(PdfObjectId? forObjectId, bool isHex)
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

        if (!forObjectId.HasValue || _decryptor is null)
            return new PdfString(bytes, isHex);

        return _decryptor.Decrypt(new PdfString(bytes, isHex), forObjectId.Value);
    }
}
