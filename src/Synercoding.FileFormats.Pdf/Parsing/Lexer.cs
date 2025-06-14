using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Logging;
using Synercoding.FileFormats.Pdf.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Parsing;

public class Lexer
{
    private readonly IPdfLogger? _logger;

    public Lexer(IPdfBytesProvider pdfBytesProvider)
        : this(pdfBytesProvider, null)
    { }

    public Lexer(IPdfBytesProvider pdfBytesProvider, IPdfLogger? logger)
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

    public bool TryPeekNextTokenKind([NotNullWhen(true)] out TokenKind? tokenKind, bool skipComments = true)
    {
        var position = Position;

        try
        {
            _logger.LogTrace<Lexer>("Peeking next token at {Position}", position);

            tokenKind = null;

            PdfBytesProvider.SkipWhiteSpace();

            if (PdfBytesProvider.TryPeek(2, out var peekedBytes))
            {
                tokenKind = (peekedBytes[0], peekedBytes[1]) switch
                {
                    var (b, _) when b == ByteUtils.PERCENT_SIGN => TokenKind.Comment,
                    var (b, _) when b == ByteUtils.SOLIDUS => TokenKind.Name,
                    var (b, _) when b == 0x2B || b == 0x2D || b == 0x2E || ( b >= 0x30 && b <= 0x39 ) => TokenKind.Number,
                    var (b1, b2) when b1 == ByteUtils.LESS_THAN_SIGN && b2 == ByteUtils.LESS_THAN_SIGN => TokenKind.BeginDictionary,
                    var (b1, b2) when b1 == ByteUtils.GREATER_THAN_SIGN && b2 == ByteUtils.GREATER_THAN_SIGN => TokenKind.EndDictionary,
                    var (b, _) when b == ByteUtils.LEFT_SQUARE_BRACKET => TokenKind.BeginArray,
                    var (b, _) when b == ByteUtils.RIGHT_SQUARE_BRACKET => TokenKind.EndArray,
                    var (b, _) when b == ByteUtils.PARENTHESIS_OPEN => TokenKind.StringLiteral,
                    var (b, _) when b == ByteUtils.LESS_THAN_SIGN => TokenKind.StringHex,
                    var (b1, b2) when b1 == 0x52 && ByteUtils.IsDelimiterOrWhiteSpace(b2) => TokenKind.Reference,
                    _ => TokenKind.Other
                };
            }
            else if (PdfBytesProvider.TryPeek(out var peek))
            {
                tokenKind = peek switch
                {
                    var b when b == ByteUtils.PERCENT_SIGN => TokenKind.Comment,
                    var b when b == ByteUtils.SOLIDUS => TokenKind.Name,
                    var b when b == 0x2B || b == 0x2D || b == 0x2E || ( b >= 0x30 && b <= 0x39 ) => TokenKind.Number,
                    var b when b == ByteUtils.LEFT_SQUARE_BRACKET => TokenKind.BeginArray,
                    var b when b == ByteUtils.RIGHT_SQUARE_BRACKET => TokenKind.EndArray,
                    var b when b == 0x52 => TokenKind.Reference,
                    _ => TokenKind.Other
                };
            }

            if (tokenKind == TokenKind.Other)
            {
                _logger.LogDebug<Lexer>("Found token is Other, reading next token to determine exact type.");
                if (TryGetNextToken(out var nextToken, skipComments))
                {
                    tokenKind = nextToken.TokenKind;
                }
                else
                {
                    _logger.LogWarning<Lexer>("TryPeek worked, so bytes were available, but no token was found while reading.");
                }
            }

            _logger.LogTrace<Lexer>("Peeked at {TokenType}", tokenKind);

            if (skipComments && tokenKind == TokenKind.Comment)
            {
                _logger.LogTrace<Lexer>("Token was a comment we can skip, peeking again.");
                _ = TryGetNextToken(out _, false);
                return TryPeekNextTokenKind(out tokenKind, skipComments: true);
            }

            return tokenKind is not null;
        }
        finally
        {
            _logger.LogTrace<Lexer>("Return to position {Position}", position);
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
                var (b1, b2) when b1 == 0x52 && ByteUtils.IsDelimiterOrWhiteSpace(b2) => _readReference(),
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

        if (token is TokenBytes tokenBytes && tokenBytes.TokenKind == TokenKind.Other)
        {
            token = tokenBytes.Bytes switch
            {
                var b when Equals(b, "null") => new TokenNull(),
                var b when Equals(b, "obj") => new Token(TokenKind.Obj),
                var b when Equals(b, "endobj") => new Token(TokenKind.EndObj),
                var b when Equals(b, "stream") => new Token(TokenKind.Stream),
                var b when Equals(b, "endstream") => new Token(TokenKind.EndStream),
                var b when Equals(b, "trailer") => new Token(TokenKind.Trailer),
                var b when Equals(b, "true") => new TokenBoolean(true),
                var b when Equals(b, "false") => new TokenBoolean(false),
                _ => tokenBytes
            };
        }

        if (skipComments && token is TokenBytes possibleComment && possibleComment.TokenKind == TokenKind.Comment)
        {
            _logger.LogTrace<Lexer>("Found comment token: {Comment}", Encoding.UTF8.GetString(possibleComment.Bytes));
            return TryGetNextToken(out token, skipComments: true);
        }

        _logger.LogTrace<Lexer>("Found token {Token}", token);
        return token is not null;

        static bool Equals(byte[] bytes, string input)
        {
            if (bytes.Length != input.Length)
                return false;

            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] != input[i])
                    return false;
            }
            return true;
        }
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

        return new Token(TokenKind.BeginDictionary);
    }

    private Token _readEndDictionary()
    {
        PdfBytesProvider
            .SkipOrThrow(ByteUtils.GREATER_THAN_SIGN)
            .SkipOrThrow(ByteUtils.GREATER_THAN_SIGN);

        return new Token(TokenKind.EndDictionary);
    }

    private Token _readBeginArray()
    {
        PdfBytesProvider.SkipOrThrow(ByteUtils.LEFT_SQUARE_BRACKET);

        return new Token(TokenKind.BeginArray);
    }

    private Token _readEndArray()
    {
        PdfBytesProvider.SkipOrThrow(ByteUtils.RIGHT_SQUARE_BRACKET);

        return new Token(TokenKind.EndArray);
    }

    private TokenName _readName()
    {
        PdfBytesProvider.SkipOrThrow(ByteUtils.SOLIDUS);

        var bytes = new List<byte>();
        while (PdfBytesProvider.TryPeek(out byte b) && !ByteUtils.IsDelimiterOrWhiteSpace(b))
        {
            bytes.Add(PdfBytesProvider.ReadByte());
        }

        var name = new PdfName(bytes.ToArray());

        return new TokenName(name);
    }

    private Token _readReference()
    {
        PdfBytesProvider.SkipOrThrow('R'); // capital R

        return new Token(TokenKind.Reference);
    }

    private TokenNumber _readNumber()
    {
        var negative = false;

        if (!PdfBytesProvider.TryPeek(out byte possibleSign))
            throw new UnexpectedEndOfFileException();

        if (possibleSign == '-' || possibleSign == '+')
        {
            // Eat the sign
            PdfBytesProvider.Skip();
            negative = possibleSign == '-';
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
                        ? new TokenNumber(( number + fraction ) * -1, null)
                        : new TokenNumber(number + fraction, null);
                }
                else
                {
                    return negative
                        ? new TokenNumber(null, number * -1)
                        : new TokenNumber(null, number);
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
                throw new UnexpectedByteException(0x30, 0x39, b);

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
            throw new UnexpectedByteException(ByteUtils.PARENTHESIS_OPEN, b);

        int parenthesisLevel = 1;
        var bytes = new List<byte>();

        while (PdfBytesProvider.TryRead(out byte b1))
        {
            if (b1 == ByteUtils.REVERSE_SOLIDUS)
            {
                if (!PdfBytesProvider.TryPeek(out byte b2))
                    throw new UnexpectedEndOfFileException();

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
                        bytes.Add(0x09); // backspace character
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
                parenthesisLevel--;

                if (parenthesisLevel == 0)
                    break;

                bytes.Add(b1);
            }
            else
            {
                bytes.Add(b1);
            }
        }

        if (parenthesisLevel != 0)
            throw new ParseException("Unbalanced parenthesis in string literal.");

        return new TokenBytes(TokenKind.StringLiteral, bytes.ToArray());
    }

    private TokenBytes _readStringHex()
    {
        if (PdfBytesProvider.ReadByte() is byte b && b != ByteUtils.LESS_THAN_SIGN)
            throw new UnexpectedByteException(ByteUtils.LESS_THAN_SIGN, b);

        var bytes = new List<byte>();
        while (PdfBytesProvider.ReadByte() is byte h && h != ByteUtils.GREATER_THAN_SIGN)
        {
            if (ByteUtils.IsHex(h))
                bytes.Add(h);
            else
                throw new UnexpectedByteException(h, "Expected hex byte value.");
        }

        return new TokenBytes(TokenKind.StringHex, bytes.ToArray());
    }

    private TokenBytes _readComment()
    {
        if (PdfBytesProvider.ReadByte() is byte b && b != ByteUtils.PERCENT_SIGN)
            throw new UnexpectedByteException(ByteUtils.PERCENT_SIGN, b);

        var bytes = new List<byte>();
        while (PdfBytesProvider.TryRead(out byte c))
        {
            if (c == ByteUtils.CARRIAGE_RETURN && PdfBytesProvider.TryPeek(out byte peek) && peek == ByteUtils.LINE_FEED)
                break;
            if (c == ByteUtils.LINE_FEED)
                break;

            bytes.Add(c);
        }

        return new TokenBytes(TokenKind.Comment, bytes.ToArray());
    }

    private TokenBytes _readOtherToken()
    {
        var bytes = new List<byte>();
        while (PdfBytesProvider.TryPeek(out byte b) && !ByteUtils.IsDelimiterOrWhiteSpace(b))
        {
            bytes.Add(PdfBytesProvider.ReadByte());
        }

        return new TokenBytes(TokenKind.Other, bytes.ToArray());
    }
}
