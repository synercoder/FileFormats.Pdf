using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Logging;
using Synercoding.FileFormats.Pdf.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Parsing;

public class Tokenizer
{
    private readonly IPdfLogger? _logger;

    public Tokenizer(IPdfBytesProvider pdfBytesProvider)
        : this(pdfBytesProvider, null)
    { }

    public Tokenizer(IPdfBytesProvider pdfBytesProvider, IPdfLogger? logger)
    {
        PdfBytesProvider = pdfBytesProvider ?? throw new ArgumentNullException(nameof(pdfBytesProvider));
        _logger = logger;
    }

    internal IPdfBytesProvider PdfBytesProvider { get; }

    public long Position
    {
        get => PdfBytesProvider.Position;
        set => PdfBytesProvider.Seek(value, SeekOrigin.Begin);
    }

    public bool TryPeekNextTokenType([NotNullWhen(true)] out TokenType? tokenType, bool skipComments = true)
    {
        var position = Position;

        try
        {
            _logger.LogTrace<Tokenizer>("Peeking next token at {Position}", position);

            tokenType = null;

            PdfBytesProvider.SkipWhiteSpace();

            if (PdfBytesProvider.TryPeek(2, out var peekedBytes))
            {
                tokenType = (peekedBytes[0], peekedBytes[1]) switch
                {
                    var (b, _) when b == ByteUtils.PERCENT_SIGN => TokenType.Comment,
                    var (b, _) when b == ByteUtils.SOLIDUS => TokenType.Name,
                    var (b, _) when b == 0x2B || b == 0x2D || b == 0x2E || ( b >= 0x30 && b <= 0x39 ) => TokenType.Number,
                    var (b1, b2) when b1 == ByteUtils.LESS_THAN_SIGN && b2 == ByteUtils.LESS_THAN_SIGN => TokenType.BeginDictionary,
                    var (b1, b2) when b1 == ByteUtils.GREATER_THAN_SIGN && b2 == ByteUtils.GREATER_THAN_SIGN => TokenType.EndDictionary,
                    var (b, _) when b == ByteUtils.LEFT_SQUARE_BRACKET => TokenType.BeginArray,
                    var (b, _) when b == ByteUtils.RIGHT_SQUARE_BRACKET => TokenType.EndArray,
                    var (b, _) when b == ByteUtils.PARENTHESIS_OPEN => TokenType.StringLiteral,
                    var (b, _) when b == ByteUtils.LESS_THAN_SIGN => TokenType.StringHex,
                    var (b1, b2) when b1 == 0x52 && ByteUtils.IsDelimiterorWhiteSpace(b2) => TokenType.Reference,
                    _ when PdfBytesProvider.IsTrueNext(read: false) => TokenType.Boolean,
                    _ when PdfBytesProvider.IsFalseNext(read: false) => TokenType.Boolean,
                    _ when PdfBytesProvider.IsObjNext(read: false) => TokenType.Obj,
                    _ when PdfBytesProvider.IsEndObjNext(read: false) => TokenType.EndObj,
                    _ when PdfBytesProvider.IsStreamNext(read: false) => TokenType.Stream,
                    _ when PdfBytesProvider.IsEndStreamNext(read: false) => TokenType.EndStream,
                    _ when PdfBytesProvider.IsNullNext(read: false) => TokenType.Null,
                    _ when PdfBytesProvider.IsTrailerNext(read: false) => TokenType.Trailer,
                    _ => TokenType.Other
                };
            }
            else if (PdfBytesProvider.TryPeek(out var peek))
            {
                tokenType = peek switch
                {
                    var b when b == ByteUtils.PERCENT_SIGN => TokenType.Comment,
                    var b when b == ByteUtils.SOLIDUS => TokenType.Name,
                    var b when b == 0x2B || b == 0x2D || b == 0x2E || ( b >= 0x30 && b <= 0x39 ) => TokenType.Number,
                    var b when b == ByteUtils.LEFT_SQUARE_BRACKET => TokenType.BeginArray,
                    var b when b == ByteUtils.RIGHT_SQUARE_BRACKET => TokenType.EndArray,
                    var b when b == 0x52 => TokenType.Reference,
                    _ => TokenType.Other
                };
            }

            _logger.LogTrace<Tokenizer>("Peeked at {TokenType}", tokenType);

            if (skipComments && tokenType == TokenType.Comment)
            {
                _logger.LogTrace<Tokenizer>("Token was a comment we can skip, peeking again.");
                return TryPeekNextTokenType(out tokenType, skipComments: true);
            }

            return tokenType is not null;
        }
        finally
        {
            _logger.LogTrace<Tokenizer>("Return to position {Position}", position);
            Position = position;
        }
    }

    public bool TryGetNextToken([NotNullWhen(true)] out Token? token, bool skipComments = true)
    {
        token = null;

        PdfBytesProvider.SkipWhiteSpace();

        if (PdfBytesProvider.TryPeek(2, out var peekedBytes))
        {
            token = (peekedBytes[0], peekedBytes[1]) switch
            {
                var (b, _) when b == ByteUtils.PERCENT_SIGN => _readComment(),
                var (b, _) when b == ByteUtils.SOLIDUS => _readName(),
                var (b, _) when b == 0x2B || b == 0x2D || b == 0x2E || ( b >= 0x30 && b <= 0x39 ) => _readNumber(),
                var (b1, b2) when b1 == ByteUtils.LESS_THAN_SIGN && b2 == ByteUtils.LESS_THAN_SIGN => _readBeginDictionary(),
                var (b1, b2) when b1 == ByteUtils.GREATER_THAN_SIGN && b2 == ByteUtils.GREATER_THAN_SIGN => _readEndDictionary(),
                var (b, _) when b == ByteUtils.LEFT_SQUARE_BRACKET => _readBeginArray(),
                var (b, _) when b == ByteUtils.RIGHT_SQUARE_BRACKET => _readEndArray(),
                var (b, _) when b == ByteUtils.PARENTHESIS_OPEN => _readStringLiteral(),
                var (b, _) when b == ByteUtils.LESS_THAN_SIGN => _readStringHex(),
                var (b1, b2) when b1 == 0x52 && ByteUtils.IsDelimiterorWhiteSpace(b2) => _readReference(),
                _ when PdfBytesProvider.IsNullNext(read: false) => _readNull(),
                _ when PdfBytesProvider.IsTrueNext(read: false) => _readBoolean(true),
                _ when PdfBytesProvider.IsFalseNext(read: false) => _readBoolean(false),
                _ when PdfBytesProvider.IsObjNext(read: false) => _read(TokenType.Obj),
                _ when PdfBytesProvider.IsEndObjNext(read: false) => _read(TokenType.EndObj),
                _ when PdfBytesProvider.IsStreamNext(read: false) => _read(TokenType.Stream),
                _ when PdfBytesProvider.IsEndStreamNext(read: false) => _read(TokenType.EndStream),
                _ when PdfBytesProvider.IsNullNext(read: false) => _read(TokenType.Null),
                _ when PdfBytesProvider.IsTrailerNext(read: false) => _read(TokenType.Trailer),
                _ => _readOtherToken(),
            };
        }
        else if (PdfBytesProvider.TryPeek(out var peek))
        {
            token = peek switch
            {
                var b when b == ByteUtils.PERCENT_SIGN => _readComment(),
                var b when b == ByteUtils.SOLIDUS => _readName(),
                var b when b == 0x2B || b == 0x2D || b == 0x2E || ( b >= 0x30 && b <= 0x39 ) => _readNumber(),
                var b when b == ByteUtils.LEFT_SQUARE_BRACKET => _readBeginArray(),
                var b when b == ByteUtils.RIGHT_SQUARE_BRACKET => _readEndArray(),
                var b when b == ByteUtils.PARENTHESIS_OPEN => _readStringLiteral(),
                var b when b == ByteUtils.LESS_THAN_SIGN => _readStringHex(),
                var b when b == 0x52 => _readReference(),
                _ => _readOtherToken(),
            };
        }

        if (skipComments && token is TokenBytes tokenBytes && tokenBytes.TokenType == TokenType.Comment)
        {
            _logger.LogTrace<Tokenizer>("Found comment token: {Comment}", Encoding.ASCII.GetString(tokenBytes.Bytes));
            return TryGetNextToken(out token, skipComments: true);
        }

        _logger.LogTrace<Tokenizer>("Found token {Token}", token);
        return token is not null;
    }

    public bool TryPeek([MaybeNullWhen(false)] out Token token)
    {
        var position = PdfBytesProvider.Position;

        try
        {
            return TryGetNextToken(out token);
        }
        finally
        {
            PdfBytesProvider.Seek(position, SeekOrigin.Begin);
        }
    }

    private Token _readBeginDictionary()
    {
        PdfBytesProvider
            .SkipOrThrow(ByteUtils.LESS_THAN_SIGN)
            .SkipOrThrow(ByteUtils.LESS_THAN_SIGN);

        return new Token(TokenType.BeginDictionary);
    }

    private Token _readEndDictionary()
    {
        PdfBytesProvider
            .SkipOrThrow(ByteUtils.GREATER_THAN_SIGN)
            .SkipOrThrow(ByteUtils.GREATER_THAN_SIGN);

        return new Token(TokenType.EndDictionary);
    }

    private Token _readBeginArray()
    {
        PdfBytesProvider
            .SkipOrThrow(ByteUtils.LEFT_SQUARE_BRACKET);

        return new Token(TokenType.BeginArray);
    }

    private Token _readEndArray()
    {
        PdfBytesProvider
            .SkipOrThrow(ByteUtils.RIGHT_SQUARE_BRACKET);

        return new Token(TokenType.EndArray);
    }

    private TokenName _readName()
    {
        PdfBytesProvider
            .SkipOrThrow(ByteUtils.SOLIDUS);

        var bytes = new List<byte>();
        while (PdfBytesProvider.TryPeek(out byte b) && !ByteUtils.IsDelimiterorWhiteSpace(b))
        {
            bytes.Add(PdfBytesProvider.ReadByte());
        }

        var name = new PdfName(bytes.ToArray());

        return new TokenName(name);
    }

    private Token _readReference()
    {
        PdfBytesProvider
            .SkipOrThrow(0x52); // capital R

        return new Token(TokenType.Reference);
    }

    private TokenNumber _readNumber()
    {
        var negative = false;

        if (!PdfBytesProvider.TryPeek(out byte possibleSign))
            throw ParseException.UnexpectedEOF();

        if (possibleSign == 0x2D || possibleSign == 0x2B) // minus and plus signs
        {
            // Eat the sign
            PdfBytesProvider.Skip();
            negative = possibleSign == 0x2D;
        }

        long number = 0;
        double fraction = 0;
        int fractions = 0;
        while (true)
        {
            if (!PdfBytesProvider.TryPeek(out byte peek) || ByteUtils.IsDelimiter(peek) || ByteUtils.IsWhiteSpace(peek))
            {
                if (fractions > 0)
                {
                    return negative
                        ? new TokenNumber(( number + fraction ) * -1, true)
                        : new TokenNumber(number + fraction, true);
                }
                else
                {
                    return negative
                        ? new TokenNumber(number * -1, false)
                        : new TokenNumber(number, false);
                }
            }

            if (peek == 0x2E && fractions > 0)
                throw new ParseException($"Found multiple fractional points in the same number.");

            var b = PdfBytesProvider.ReadByte();
            if (b == 0x2E)
            {
                fractions = 1;
                continue;
            }

            if (b < 0x30 || b > 0x39)
                throw new ParseException(0x30, 0x39, b);

            if (fractions > 0)
            {
                fraction += 1d / Math.Pow(10, fractions) * ( b - 0x30 );
                fractions++;
            }
            else
            {
                number *= 10;
                number += ( b - 0x30 );
            }
        }
    }

    private Token _readStringLiteral()
    {
        if (PdfBytesProvider.ReadByte() is byte b && b != ByteUtils.PARENTHESIS_OPEN)
            throw new ParseException(ByteUtils.PARENTHESIS_OPEN, b);

        int parenthesisLevel = 0;
        var bytes = new List<byte>();

        while (PdfBytesProvider.TryRead(out byte b1))
        {
            if (b1 == ByteUtils.REVERSE_SOLIDUS)
            {
                if (!PdfBytesProvider.TryPeek(out byte b2))
                    throw ParseException.UnexpectedEOF();

                switch (b2)
                {
                    case (byte)'n':
                        bytes.Add(ByteUtils.LINE_FEED);
                        PdfBytesProvider.Skip();
                        break;
                    case (byte)'r':
                        bytes.Add(ByteUtils.CARRIAGE_RETURN);
                        PdfBytesProvider.Skip();
                        break;
                    case (byte)'t':
                        bytes.Add(ByteUtils.HORIZONTAL_TAB);
                        PdfBytesProvider.Skip();
                        break;
                    case (byte)'b':
                        bytes.Add(0x09); // backspace charactr
                        PdfBytesProvider.Skip();
                        break;
                    case (byte)'f':
                        bytes.Add(ByteUtils.FORM_FEED);
                        PdfBytesProvider.Skip();
                        break;
                    case (byte)'(':
                    case (byte)')':
                    case (byte)'\\':
                        bytes.Add(PdfBytesProvider.ReadByte());
                        break;
                    case byte next when ByteUtils.IsOctal(next):
                        bytes.Add(PdfBytesProvider.ReadOctalAsByte());
                        break;
                    case (byte)'\n':
                        PdfBytesProvider.Skip();
                        break;
                    case (byte)'\r'
                        when PdfBytesProvider.TryPeek(2, out var crlf)
                        && crlf[1] == ByteUtils.LINE_FEED:
                        PdfBytesProvider.Skip(2);
                        break;
                }
            }
            else if (b1 == ByteUtils.PARENTHESIS_OPEN)
            {
                parenthesisLevel++;
                bytes.Add(b1);
            }
            else if (b1 == ByteUtils.PARENTHESIS_CLOSED)
            {
                if (parenthesisLevel == 0)
                    break;

                parenthesisLevel--;
                bytes.Add(b1);
            }
            else
            {
                bytes.Add(b1);
            }
        }

        if (parenthesisLevel != 0)
            throw new ParseException("Unbalanced parenthesis in string literal.");

        return new TokenBytes(TokenType.StringLiteral, bytes.ToArray());
    }

    private TokenBytes _readStringHex()
    {
        if (PdfBytesProvider.ReadByte() is byte b && b != ByteUtils.LESS_THAN_SIGN)
            throw new ParseException(ByteUtils.LESS_THAN_SIGN, b);

        var bytes = new List<byte>();
        while (PdfBytesProvider.ReadByte() is byte h && h != ByteUtils.GREATER_THAN_SIGN)
        {
            if (ByteUtils.IsHex(h))
                bytes.Add(h);
            else
                throw new ParseException(h, "Expected hex byte value.");
        }

        return new TokenBytes(TokenType.StringHex, bytes.ToArray());
    }

    private TokenBytes _readComment()
    {
        if (PdfBytesProvider.ReadByte() is byte b && b != ByteUtils.PERCENT_SIGN)
            throw new ParseException(ByteUtils.PERCENT_SIGN, b);

        var bytes = new List<byte>();
        while (PdfBytesProvider.TryRead(out byte c))
        {
            if (c == ByteUtils.CARRIAGE_RETURN && PdfBytesProvider.TryPeek(out byte peek) && peek == ByteUtils.LINE_FEED)
                break;
            if (c == ByteUtils.LINE_FEED)
                break;

            bytes.Add(c);
        }

        return new TokenBytes(TokenType.Comment, bytes.ToArray());
    }

    private TokenBytes _readOtherToken()
    {
        var bytes = new List<byte>();
        while (PdfBytesProvider.TryPeek(out byte b) && !ByteUtils.IsDelimiterorWhiteSpace(b))
        {
            bytes.Add(PdfBytesProvider.ReadByte());
        }

        return new TokenBytes(TokenType.Other, bytes.ToArray());
    }

    private Token _readNull()
    {
        PdfBytesProvider.Skip(4);
        return new Token(TokenType.Null);
    }

    private TokenBoolean _readBoolean(bool value)
    {
        PdfBytesProvider.Skip(value ? 4 : 5);

        return new TokenBoolean(value);
    }

    private Token _read(TokenType tokenType)
    {
        if (tokenType == TokenType.Null && PdfBytesProvider.IsNullNext(read: true))
            return new Token(tokenType);
        if (tokenType == TokenType.Obj && PdfBytesProvider.IsObjNext(read: true))
            return new Token(tokenType);
        if (tokenType == TokenType.EndObj && PdfBytesProvider.IsEndObjNext(read: true))
            return new Token(tokenType);
        if (tokenType == TokenType.Stream && PdfBytesProvider.IsStreamNext(read: true))
            return new Token(tokenType);
        if (tokenType == TokenType.EndStream && PdfBytesProvider.IsEndStreamNext(read: true))
            return new Token(tokenType);
        if (tokenType == TokenType.Trailer && PdfBytesProvider.IsTrailerNext(read: true))
            return new Token(tokenType);

        throw new ParseException($"Expected {tokenType} was not found.");
    }
}
