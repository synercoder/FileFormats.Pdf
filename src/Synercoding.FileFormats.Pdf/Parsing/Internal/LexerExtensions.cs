using Synercoding.FileFormats.Pdf.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Parsing.Internal;
internal static class LexerExtensions
{
    public static TToken ReadOrThrow<TToken>(this Lexer lexer, bool skipComments = true)
        where TToken : Token
    {
        if (!lexer.TryGetNextToken(out var token, skipComments))
            throw new UnexpectedEndOfFileException();

        if (token is not TToken ttoken)
            throw new UnexpectedTokenException(typeof(TToken), token.GetType());

        return ttoken;
    }

    public static Token ReadOrThrow(this Lexer tokenizer, TokenKind tokenKind, bool skipComments = true)
    {
        if (!tokenizer.TryGetNextToken(out var token, skipComments))
            throw new UnexpectedEndOfFileException();

        if (token.TokenKind != tokenKind)
            throw new UnexpectedTokenException(tokenKind, token.TokenKind);

        return token;
    }

    public static bool TryGetNextAs<TToken>(this Lexer tokenizer, [NotNullWhen(true)] out TToken? token, bool skipComments = true)
        where TToken : Token
    {
        token = null;
        if (!tokenizer.TryGetNextToken(out var nextToken, skipComments))
            return false;

        if (nextToken is not TToken ttoken)
            return false;

        token = ttoken;
        return true;
    }
}
