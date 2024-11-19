using Synercoding.FileFormats.Pdf.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Parsing.Internal;

internal static class TokenizerExtensions
{
    public static TToken ReadOrThrow<TToken>(this Tokenizer tokenizer, bool skipComments = true)
        where TToken : Token
    {
        if (!tokenizer.TryGetNextToken(out var token, skipComments))
            throw ParseException.UnexpectedEOF();

        if (token is not TToken ttoken)
            throw new ParseException(typeof(TToken), token.GetType());

        return ttoken;
    }

    public static Token ReadOrThrow(this Tokenizer tokenizer, TokenType tokenType, bool skipComments = true)
    {
        if (!tokenizer.TryGetNextToken(out var token, skipComments))
            throw ParseException.UnexpectedEOF();

        if (token.TokenType != tokenType)
            throw new ParseException(tokenType, token.TokenType);

        return token;
    }

    public static bool TryGetNextAs<TToken>(this Tokenizer tokenizer, [NotNullWhen(true)] out TToken? token, bool skipComments = true)
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
