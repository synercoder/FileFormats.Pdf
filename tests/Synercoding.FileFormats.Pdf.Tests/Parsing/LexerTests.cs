using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Parsing;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.Parsing;

public class LexerTests
{
    [Fact]
    public void Constructor_WithValidProvider_SetsProviderProperty()
    {
        var bytes = Encoding.ASCII.GetBytes("test");
        var provider = new PdfByteArrayProvider(bytes);
        
        var lexer = new Lexer(provider);
        
        Assert.Same(provider, lexer.PdfBytesProvider);
    }

    [Fact]
    public void Constructor_WithNullProvider_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Lexer(null!));
    }

    [Fact]
    public void Position_GetAndSet_WorksCorrectly()
    {
        var bytes = Encoding.ASCII.GetBytes("test string");
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        lexer.Position = 5;
        Assert.Equal(5, lexer.Position);
        
        lexer.Position = 0;
        Assert.Equal(0, lexer.Position);
    }

    [Theory]
    [InlineData("% This is a comment", TokenKind.Comment)]
    [InlineData("/Name", TokenKind.Name)]
    [InlineData("123", TokenKind.Number)]
    [InlineData("123.45", TokenKind.Number)]
    [InlineData("-42", TokenKind.Number)]
    [InlineData("+3.14", TokenKind.Number)]
    [InlineData("<<", TokenKind.BeginDictionary)]
    [InlineData(">>", TokenKind.EndDictionary)]
    [InlineData("[", TokenKind.BeginArray)]
    [InlineData("]", TokenKind.EndArray)]
    [InlineData("(string literal)", TokenKind.StringLiteral)]
    [InlineData("<48656C6C6F>", TokenKind.StringHex)]
    [InlineData("R ", TokenKind.Reference)]
    public void TryPeekNextTokenType_IdentifiesTokenTypesCorrectly(string input, TokenKind expectedTokenKind)
    {
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        var result = lexer.TryPeekNextTokenKind(out var tokenKind, skipComments: false);
        
        Assert.True(result);
        Assert.Equal(expectedTokenKind, tokenKind);
        Assert.Equal(0, lexer.Position); // Position should not change after peek
    }

    [Theory]
    [InlineData("null", TokenKind.Null)]
    [InlineData("obj", TokenKind.Obj)]
    [InlineData("endobj", TokenKind.EndObj)]
    [InlineData("stream", TokenKind.Stream)]
    [InlineData("endstream", TokenKind.EndStream)]
    [InlineData("trailer", TokenKind.Trailer)]
    [InlineData("true", TokenKind.Boolean)]
    [InlineData("false", TokenKind.Boolean)]
    public void TryPeekNextTokenType_IdentifiesKeywordsCorrectly(string input, TokenKind expectedTokenKind)
    {
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        var result = lexer.TryPeekNextTokenKind(out var tokenKind);
        
        Assert.True(result);
        Assert.Equal(expectedTokenKind, tokenKind);
    }

    [Fact]
    public void TryPeekNextTokenType_WithComment_SkipsComment()
    {
        var bytes = Encoding.ASCII.GetBytes("% comment\n/Name");
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        var result = lexer.TryPeekNextTokenKind(out var tokenKind, skipComments: true);
        
        Assert.True(result);
        Assert.Equal(TokenKind.Name, tokenKind);
    }

    [Fact]
    public void TryPeekNextTokenType_WithComment_DoesNotSkipComment()
    {
        var bytes = Encoding.ASCII.GetBytes("% comment\n/Name");
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        var result = lexer.TryPeekNextTokenKind(out var tokenKind, skipComments: false);
        
        Assert.True(result);
        Assert.Equal(TokenKind.Comment, tokenKind);
    }

    [Fact]
    public void TryPeekNextTokenType_EmptyInput_ReturnsFalse()
    {
        var bytes = new byte[0];
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        var result = lexer.TryPeekNextTokenKind(out var tokenKind);
        
        Assert.False(result);
        Assert.Null(tokenKind);
    }

    [Fact]
    public void TryGetNextToken_Name_ReturnsCorrectToken()
    {
        var bytes = Encoding.ASCII.GetBytes("/TestName");
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        var result = lexer.TryGetNextToken(out var token);
        
        Assert.True(result);
        Assert.IsType<TokenName>(token);
        var nameToken = (TokenName)token;
        Assert.Equal("TestName", nameToken.Name.Display);
    }

    [Theory]
    [InlineData("123", 123.0)]
    [InlineData("-456", -456.0)]
    [InlineData("+789", 789.0)]
    [InlineData("123.45", 123.45)]
    [InlineData("-67.89", -67.89)]
    [InlineData("+0.123", 0.123)]
    [InlineData("0", 0.0)]
    [InlineData("0.0", 0.0)]
    public void TryGetNextToken_Number_ReturnsCorrectToken(string input, double expectedValue)
    {
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        var result = lexer.TryGetNextToken(out var token);
        
        Assert.True(result);
        Assert.IsType<TokenNumber>(token);
        var numberToken = (TokenNumber)token;

        if (numberToken.DoubleValue.HasValue)
        {
            Assert.Null(numberToken.LongValue);

            Assert.Equal(expectedValue, numberToken.DoubleValue.Value, precision: 6);
        }
        if (numberToken.LongValue.HasValue)
        {
            Assert.Null(numberToken.DoubleValue);
            Assert.Equal((long)expectedValue, numberToken.LongValue.Value);
        }
    }

    [Fact]
    public void TryGetNextToken_Boolean_True_ReturnsCorrectToken()
    {
        var bytes = Encoding.ASCII.GetBytes("true");
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        var result = lexer.TryGetNextToken(out var token);
        
        Assert.True(result);
        Assert.IsType<TokenBoolean>(token);
        var boolToken = (TokenBoolean)token;
        Assert.True(boolToken.Value);
    }

    [Fact]
    public void TryGetNextToken_Boolean_False_ReturnsCorrectToken()
    {
        var bytes = Encoding.ASCII.GetBytes("false");
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        var result = lexer.TryGetNextToken(out var token);
        
        Assert.True(result);
        Assert.IsType<TokenBoolean>(token);
        var boolToken = (TokenBoolean)token;
        Assert.False(boolToken.Value);
    }

    [Fact]
    public void TryGetNextToken_StringLiteral_ReturnsCorrectToken()
    {
        var bytes = Encoding.ASCII.GetBytes("(Hello World)");
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        var result = lexer.TryGetNextToken(out var token);
        
        Assert.True(result);
        Assert.IsType<TokenBytes>(token);
        var bytesToken = (TokenBytes)token;
        Assert.Equal(TokenKind.StringLiteral, bytesToken.TokenKind);
        Assert.Equal("Hello World", Encoding.ASCII.GetString(bytesToken.Bytes));
    }

    [Fact]
    public void TryGetNextToken_StringLiteral_WithEscapedCharacters_ReturnsCorrectToken()
    {
        var bytes = Encoding.ASCII.GetBytes(@"(Line1\nLine2\tTabbed\(Parenthesis\))");
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        var result = lexer.TryGetNextToken(out var token);
        
        Assert.True(result);
        Assert.IsType<TokenBytes>(token);
        var bytesToken = (TokenBytes)token;
        Assert.Equal(TokenKind.StringLiteral, bytesToken.TokenKind);
        var resultString = Encoding.ASCII.GetString(bytesToken.Bytes);
        Assert.Contains("Line1\nLine2", resultString);
        Assert.Contains("\t", resultString);
        Assert.Contains("(Parenthesis)", resultString);
    }

    [Fact]
    public void TryGetNextToken_StringLiteral_WithNestedParentheses_ReturnsCorrectToken()
    {
        var bytes = Encoding.ASCII.GetBytes("(Outer (nested) text)");
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        var result = lexer.TryGetNextToken(out var token);
        
        Assert.True(result);
        Assert.IsType<TokenBytes>(token);
        var bytesToken = (TokenBytes)token;
        Assert.Equal(TokenKind.StringLiteral, bytesToken.TokenKind);
        Assert.Equal("Outer (nested) text", Encoding.ASCII.GetString(bytesToken.Bytes));
    }

    [Fact]
    public void TryGetNextToken_StringHex_ReturnsCorrectToken()
    {
        var bytes = Encoding.ASCII.GetBytes("<48656C6C6F>");
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        var result = lexer.TryGetNextToken(out var token);
        
        Assert.True(result);
        Assert.IsType<TokenBytes>(token);
        var bytesToken = (TokenBytes)token;
        Assert.Equal(TokenKind.StringHex, bytesToken.TokenKind);
        Assert.Equal("48656C6C6F", Encoding.ASCII.GetString(bytesToken.Bytes));
    }

    [Fact]
    public void TryGetNextToken_Comment_ReturnsCorrectToken()
    {
        var bytes = Encoding.ASCII.GetBytes("% This is a comment\n");
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        var result = lexer.TryGetNextToken(out var token, skipComments: false);
        
        Assert.True(result);
        Assert.IsType<TokenBytes>(token);
        var bytesToken = (TokenBytes)token;
        Assert.Equal(TokenKind.Comment, bytesToken.TokenKind);
        Assert.Equal(" This is a comment", Encoding.ASCII.GetString(bytesToken.Bytes));
    }

    [Theory]
    [InlineData("<<", TokenKind.BeginDictionary)]
    [InlineData(">>", TokenKind.EndDictionary)]
    [InlineData("[", TokenKind.BeginArray)]
    [InlineData("]", TokenKind.EndArray)]
    [InlineData("R", TokenKind.Reference)]
    [InlineData("null", TokenKind.Null)]
    [InlineData("obj", TokenKind.Obj)]
    [InlineData("endobj", TokenKind.EndObj)]
    [InlineData("stream", TokenKind.Stream)]
    [InlineData("endstream", TokenKind.EndStream)]
    [InlineData("trailer", TokenKind.Trailer)]
    public void TryGetNextToken_SimpleTokens_ReturnsCorrectTokenKind(string input, TokenKind expectedTokenKind)
    {
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        var result = lexer.TryGetNextToken(out var token);
        
        Assert.True(result);
        Assert.NotNull(token);
        Assert.Equal(expectedTokenKind, token.TokenKind);
    }

    [Fact]
    public void TryGetNextToken_WithWhitespace_SkipsWhitespace()
    {
        var bytes = Encoding.ASCII.GetBytes("   \t\n\r  /Name");
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        var result = lexer.TryGetNextToken(out var token);
        
        Assert.True(result);
        Assert.IsType<TokenName>(token);
        var nameToken = (TokenName)token;
        Assert.Equal("Name", nameToken.Name.Display);
    }

    [Fact]
    public void TryGetNextToken_SkipsComments_WhenEnabled()
    {
        var bytes = Encoding.ASCII.GetBytes("% comment\n/Name");
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);

        var result = lexer.TryGetNextToken(out var token, skipComments: true);

        Assert.True(result);
        Assert.IsType<TokenName>(token);
        var nameToken = (TokenName)token;
        Assert.Equal("Name", nameToken.Name.Display);
    }

    [Fact]
    public void TryGetNextToken_Returns_WhenOnlyWhiteSpaceAfterComment()
    {
        var bytes = Encoding.ASCII.GetBytes("% comment\n");
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);

        var result = lexer.TryGetNextToken(out var token, skipComments: true);

        Assert.False(result);
    }

    [Fact]
    public void TryGetNextToken_EmptyInput_ReturnsFalse()
    {
        var bytes = new byte[0];
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        var result = lexer.TryGetNextToken(out var token);
        
        Assert.False(result);
        Assert.Null(token);
    }

    [Fact]
    public void TryPeek_ReturnsTokenWithoutAdvancingPosition()
    {
        var bytes = Encoding.ASCII.GetBytes("/Name 123");
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        var peekResult = lexer.TryPeek(out var peekedToken);
        var initialPosition = lexer.Position;
        
        var getResult = lexer.TryGetNextToken(out var actualToken);
        
        Assert.True(peekResult);
        Assert.True(getResult);
        Assert.Equal(0, initialPosition);
        Assert.IsType<TokenName>(peekedToken);
        Assert.IsType<TokenName>(actualToken);
        
        var peekedName = ((TokenName)peekedToken).Name.Display;
        var actualName = ((TokenName)actualToken).Name.Display;
        Assert.Equal(peekedName, actualName);
    }

    [Fact]
    public void TryGetNextToken_MultipleTokens_AdvancesPosition()
    {
        var bytes = Encoding.ASCII.GetBytes("/Name1 /Name2 123");
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        // First token
        var result1 = lexer.TryGetNextToken(out var token1);
        Assert.True(result1);
        Assert.IsType<TokenName>(token1);
        Assert.Equal("Name1", ((TokenName)token1).Name.Display);
        
        // Second token
        var result2 = lexer.TryGetNextToken(out var token2);
        Assert.True(result2);
        Assert.IsType<TokenName>(token2);
        Assert.Equal("Name2", ((TokenName)token2).Name.Display);
        
        // Third token
        var result3 = lexer.TryGetNextToken(out var token3);
        Assert.True(result3);
        Assert.IsType<TokenNumber>(token3);
        Assert.Equal(123L, ((TokenNumber)token3).LongValue);
    }

    [Fact]
    public void TryGetNextToken_StringLiteral_UnbalancedParentheses_ThrowsParseException()
    {
        var bytes = Encoding.ASCII.GetBytes("(unbalanced");
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        Assert.Throws<ParseException>(() => lexer.TryGetNextToken(out var token));
    }

    [Fact]
    public void TryGetNextToken_StringHex_InvalidHexCharacter_ThrowsParseException()
    {
        var bytes = Encoding.ASCII.GetBytes("<4G>");
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        Assert.Throws<UnexpectedByteException>(() => lexer.TryGetNextToken(out var token));
    }

    [Fact]
    public void TryGetNextToken_Number_MultipleFractionalPoints_ThrowsParseException()
    {
        var bytes = Encoding.ASCII.GetBytes("123.45.67");
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        Assert.Throws<ParseException>(() => lexer.TryGetNextToken(out var token));
    }

    [Fact]
    public void TryGetNextToken_Number_InvalidDigit_ThrowsParseException()
    {
        var bytes = Encoding.ASCII.GetBytes("12Z34");
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        Assert.Throws<UnexpectedByteException>(() => lexer.TryGetNextToken(out var token));
    }

    [Theory]
    [InlineData("   ")]  // Only whitespace
    [InlineData("\t\n\r")] // Only whitespace characters
    public void TryGetNextToken_OnlyWhitespace_ReturnsFalse(string input)
    {
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        var result = lexer.TryGetNextToken(out var token);
        
        Assert.False(result);
        Assert.Null(token);
    }

    [Fact]
    public void Position_Seeking_WorksCorrectly()
    {
        var bytes = Encoding.ASCII.GetBytes("/Name1 /Name2");
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        // Read first token
        lexer.TryGetNextToken(out var token1);
        var positionAfterFirst = lexer.Position;
        
        // Read second token
        lexer.TryGetNextToken(out var token2);
        
        // Seek back to after first token
        lexer.Position = positionAfterFirst;
        
        // Read second token again
        lexer.TryGetNextToken(out var token2Again);
        
        Assert.IsType<TokenName>(token2);
        Assert.IsType<TokenName>(token2Again);
        Assert.Equal(((TokenName)token2).Name.Display, ((TokenName)token2Again).Name.Display);
    }

    [Fact]
    public void TryGetNextToken_StringLiteral_WithOctalEscapeSequences_ReturnsCorrectToken()
    {
        var bytes = Encoding.ASCII.GetBytes(@"(\101\102\103)");  // \101 = A, \102 = B, \103 = C
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        var result = lexer.TryGetNextToken(out var token);
        
        Assert.True(result);
        Assert.IsType<TokenBytes>(token);
        var bytesToken = (TokenBytes)token;
        Assert.Equal(TokenKind.StringLiteral, bytesToken.TokenKind);
        Assert.Equal("ABC", Encoding.ASCII.GetString(bytesToken.Bytes));
    }

    [Fact]
    public void TryGetNextToken_StringLiteral_WithLineBreakEscaping_ReturnsCorrectToken()
    {
        var bytes = Encoding.ASCII.GetBytes("(Line1\\\nLine2)");
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        var result = lexer.TryGetNextToken(out var token);
        
        Assert.True(result);
        Assert.IsType<TokenBytes>(token);
        var bytesToken = (TokenBytes)token;
        Assert.Equal(TokenKind.StringLiteral, bytesToken.TokenKind);
        Assert.Equal("Line1Line2", Encoding.ASCII.GetString(bytesToken.Bytes));
    }

    [Fact]
    public void TryGetNextToken_StringLiteral_WithCRLFEscaping_ReturnsCorrectToken()
    {
        var bytes = Encoding.ASCII.GetBytes("(Line1\\\r\nLine2)");
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        var result = lexer.TryGetNextToken(out var token);
        
        Assert.True(result);
        Assert.IsType<TokenBytes>(token);
        var bytesToken = (TokenBytes)token;
        Assert.Equal(TokenKind.StringLiteral, bytesToken.TokenKind);
        Assert.Equal("Line1Line2", Encoding.ASCII.GetString(bytesToken.Bytes));
    }

    [Fact]
    public void TryGetNextToken_OtherToken_ReturnsCorrectToken()
    {
        var bytes = Encoding.ASCII.GetBytes("customKeyword");
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        var result = lexer.TryGetNextToken(out var token);
        
        Assert.True(result);
        Assert.IsType<TokenBytes>(token);
        var bytesToken = (TokenBytes)token;
        Assert.Equal(TokenKind.Other, bytesToken.TokenKind);
        Assert.Equal("customKeyword", Encoding.ASCII.GetString(bytesToken.Bytes));
    }
}
