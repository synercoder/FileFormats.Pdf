using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Parsing;

public class Tokenizer
{
    private readonly IPdfBytesProvider _pdfBytesProvider;

    public Tokenizer(IPdfBytesProvider pdfBytesProvider)
    {
        _pdfBytesProvider = pdfBytesProvider ?? throw new ArgumentNullException(nameof(pdfBytesProvider));
    }

    public long Position
    {
        get => _pdfBytesProvider.Position;
        set => _pdfBytesProvider.Seek(value, SeekOrigin.Begin);
    }

    public bool TryGetNextToken([NotNullWhen(true)] out Token? token, bool skipComments = true)
    {
        token = null;

        _skipWhiteSpace();

        if (_pdfBytesProvider.TryPeek(2, out var peekedBytes))
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
                _ when _pdfBytesProvider.IsNullNext(read: false) => _readNull(),
                _ when _pdfBytesProvider.IsTrueNext(read: false) => _readBoolean(true),
                _ when _pdfBytesProvider.IsFalseNext(read: false) => _readBoolean(false),
                _ when _pdfBytesProvider.IsObjNext(read: false) => _read(TokenType.Obj),
                _ when _pdfBytesProvider.IsEndObjNext(read: false) => _read(TokenType.EndObj),
                _ when _pdfBytesProvider.IsStreamNext(read: false) => _read(TokenType.Stream),
                _ when _pdfBytesProvider.IsEndStreamNext(read: false) => _read(TokenType.EndStream),
                _ when _pdfBytesProvider.IsNullNext(read: false) => _read(TokenType.Null),
                _ => _readOtherToken(),
            };
        }
        else if (_pdfBytesProvider.TryPeek(out var peek))
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

        if (skipComments && token?.TokenType == TokenType.Comment)
            return TryGetNextToken(out token, skipComments: true);

        return token is not null;
    }

    public bool TryPeek([MaybeNullWhen(false)] out Token token)
    {
        var position = _pdfBytesProvider.Position;

        try
        {
            return TryGetNextToken(out token);
        }
        finally
        {
            _pdfBytesProvider.Seek(position, SeekOrigin.Begin);
        }
    }

    private void _skipWhiteSpace()
    {
        while (_pdfBytesProvider.TryPeek(out byte b) && ByteUtils.IsWhiteSpace(b))
            _pdfBytesProvider.Skip();
    }

    private Token _readBeginDictionary()
    {
        _pdfBytesProvider
            .SkipOrThrow(ByteUtils.LESS_THAN_SIGN)
            .SkipOrThrow(ByteUtils.LESS_THAN_SIGN);

        return new Token(TokenType.BeginDictionary);
    }

    private Token _readEndDictionary()
    {
        _pdfBytesProvider
            .SkipOrThrow(ByteUtils.GREATER_THAN_SIGN)
            .SkipOrThrow(ByteUtils.GREATER_THAN_SIGN);

        return new Token(TokenType.EndDictionary);
    }

    private Token _readBeginArray()
    {
        _pdfBytesProvider
            .SkipOrThrow(ByteUtils.LEFT_SQUARE_BRACKET);

        return new Token(TokenType.BeginArray);
    }

    private Token _readEndArray()
    {
        _pdfBytesProvider
            .SkipOrThrow(ByteUtils.RIGHT_SQUARE_BRACKET);

        return new Token(TokenType.EndArray);
    }

    private TokenName _readName()
    {
        _pdfBytesProvider
            .SkipOrThrow(ByteUtils.SOLIDUS);

        var bytes = new List<byte>();
        while (_pdfBytesProvider.TryPeek(out byte b) && !ByteUtils.IsDelimiterorWhiteSpace(b))
        {
            bytes.Add(_pdfBytesProvider.ReadByte());
        }

        var name = new PdfName(bytes.ToArray());

        return new TokenName(name);
    }

    private Token _readReference()
    {
        _pdfBytesProvider
            .SkipOrThrow(0x52); // capital R

        return new Token(TokenType.Reference);
    }

    private Token _readNumber()
    {
        var negative = false;

        if (!_pdfBytesProvider.TryPeek(out byte possibleSign))
            ParseException.ThrowUnexpectedEOF();

        if (possibleSign == 0x2D || possibleSign == 0x2B) // minus and plus signs
        {
            // Eat the sign
            _pdfBytesProvider.Skip();
            negative = possibleSign == 0x2D;
        }

        long number = 0;
        double fraction = 0;
        int fractions = 0;
        while (true)
        {
            if (!_pdfBytesProvider.TryPeek(out byte peek) || ByteUtils.IsDelimiter(peek) || ByteUtils.IsWhiteSpace(peek))
            {
                if (fractions > 0)
                {
                    return negative
                        ? new TokenReal(( number + fraction ) * -1)
                        : new TokenReal(number + fraction);
                }
                else
                {
                    return negative
                        ? new TokenInteger(number * -1)
                        : new TokenInteger(number);
                }
            }

            if (peek == 0x2E && fractions > 0)
                throw new ParseException($"Found multiple fractional points in the same number.");

            var b = _pdfBytesProvider.ReadByte();
            if (b == 0x2E)
            {
                fractions = 1;
                continue;
            }

            if (b < 0x30 || b > 0x39)
                ParseException.ThrowUnexpectedByte(0x30, 0x39, b);

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
        if (_pdfBytesProvider.ReadByte() is byte b && b != ByteUtils.PARENTHESIS_OPEN)
            ParseException.ThrowUnexpectedByte(ByteUtils.PARENTHESIS_OPEN, b);

        int parenthesisLevel = 0;
        var bytes = new List<byte>();

        while (_pdfBytesProvider.TryRead(out byte b1))
        {
            if (b1 == ByteUtils.REVERSE_SOLIDUS)
            {
                if (!_pdfBytesProvider.TryPeek(out byte b2))
                    ParseException.ThrowUnexpectedEOF();

                switch (b2)
                {
                    case (byte)'n':
                        bytes.Add(ByteUtils.LINE_FEED);
                        _pdfBytesProvider.Skip();
                        break;
                    case (byte)'r':
                        bytes.Add(ByteUtils.CARRIAGE_RETURN);
                        _pdfBytesProvider.Skip();
                        break;
                    case (byte)'t':
                        bytes.Add(ByteUtils.HORIZONTAL_TAB);
                        _pdfBytesProvider.Skip();
                        break;
                    case (byte)'b':
                        bytes.Add(0x09); // backspace charactr
                        _pdfBytesProvider.Skip();
                        break;
                    case (byte)'f':
                        bytes.Add(ByteUtils.FORM_FEED);
                        _pdfBytesProvider.Skip();
                        break;
                    case (byte)'(':
                    case (byte)')':
                    case (byte)'\\':
                        bytes.Add(_pdfBytesProvider.ReadByte());
                        break;
                    case byte next when ByteUtils.IsOctal(next):
                        bytes.Add(_pdfBytesProvider.ReadOctalAsByte());
                        break;
                    case (byte)'\n':
                        _pdfBytesProvider.Skip();
                        break;
                    case (byte)'\r'
                        when _pdfBytesProvider.TryPeek(2, out var crlf)
                        && crlf[1] == ByteUtils.LINE_FEED:
                        _pdfBytesProvider.Skip(2);
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
        if (_pdfBytesProvider.ReadByte() is byte b && b != ByteUtils.LESS_THAN_SIGN)
            ParseException.ThrowUnexpectedByte(ByteUtils.LESS_THAN_SIGN, b);

        var bytes = new List<byte>();
        while (_pdfBytesProvider.ReadByte() is byte h && h != ByteUtils.GREATER_THAN_SIGN)
        {
            if (ByteUtils.IsHex(h))
                bytes.Add(h);
            else
                ParseException.ThrowUnexpectedByte(h, "Expected hex byte value.");
        }

        return new TokenBytes(TokenType.StringHex, bytes.ToArray());
    }

    private TokenBytes _readComment()
    {
        if (_pdfBytesProvider.ReadByte() is byte b && b != ByteUtils.PERCENT_SIGN)
            ParseException.ThrowUnexpectedByte(ByteUtils.PERCENT_SIGN, b);

        var bytes = new List<byte>();
        while (_pdfBytesProvider.TryRead(out byte c))
        {
            if (c == ByteUtils.CARRIAGE_RETURN && _pdfBytesProvider.TryPeek(out byte peek) && peek == ByteUtils.LINE_FEED)
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
        while (_pdfBytesProvider.TryPeek(out byte b) && !ByteUtils.IsDelimiterorWhiteSpace(b))
        {
            bytes.Add(_pdfBytesProvider.ReadByte());
        }

        return new TokenBytes(TokenType.Other, bytes.ToArray());
    }

    private Token _readNull()
    {
        _pdfBytesProvider.Skip(4);
        return new Token(TokenType.Null);
    }

    private TokenBoolean _readBoolean(bool value)
    {
        _pdfBytesProvider.Skip(value ? 4 : 5);

        return new TokenBoolean(value);
    }

    private Token _read(TokenType tokenType)
    {
        if (tokenType == TokenType.Null && _pdfBytesProvider.IsNullNext(read: true))
            return new Token(tokenType);
        if (tokenType == TokenType.Obj && _pdfBytesProvider.IsObjNext(read: true))
            return new Token(tokenType);
        if (tokenType == TokenType.EndObj && _pdfBytesProvider.IsEndObjNext(read: true))
            return new Token(tokenType);
        if (tokenType == TokenType.Stream && _pdfBytesProvider.IsStreamNext(read: true))
            return new Token(tokenType);
        if (tokenType == TokenType.EndStream && _pdfBytesProvider.IsEndStreamNext(read: true))
            return new Token(tokenType);

        throw new ParseException($"Expected {tokenType} was not found.");
    }
}
