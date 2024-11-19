using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Parsing;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.Parsing;

public class TokeniserTests
{
    [Theory]
    [InlineData("obj", TokenType.Obj)]
    [InlineData("endobj", TokenType.EndObj)]
    [InlineData("stream", TokenType.Stream)]
    [InlineData("endstream", TokenType.EndStream)]
    [InlineData("null", TokenType.Null)]
    [InlineData("[", TokenType.BeginArray)]
    [InlineData("true", TokenType.Boolean)]
    [InlineData("false", TokenType.Boolean)]
    [InlineData("123", TokenType.Number)]
    [InlineData("/Test", TokenType.Name)]
    [InlineData("(Hello world!)", TokenType.StringLiteral)]
    [InlineData("<48656C6C6F20776F726C6421>", TokenType.StringHex)]
    [InlineData("<<", TokenType.BeginDictionary)]
    [InlineData("1.2", TokenType.Number)]
    [InlineData(">>", TokenType.EndDictionary)]
    [InlineData("]", TokenType.EndArray)]
    [InlineData("% this is a comment", TokenType.Comment)]
    public void Test_For_Correct_TokenType(string asciiData, TokenType expected)
    {
        var provider = new PdfByteArrayProvider(Encoding.ASCII.GetBytes(asciiData));
        var tokeniser = new Tokenizer(provider);

        if (!tokeniser.TryGetNextToken(out Token? token, skipComments: false))
            Assert.Fail("Could not get the next token.");

        Assert.Equal(expected, token.TokenType);
    }

    [Theory]
    [InlineData("1.2", 1.2)]
    [InlineData(".2", 0.2)]
    [InlineData("+1.2", 1.2)]
    [InlineData("+.2", 0.2)]
    [InlineData("-1.2", -1.2)]
    [InlineData("-.2", -0.2)]
    [InlineData("123456789.987645321", 123456789.987645321)]
    [InlineData("+123456789.987645321", 123456789.987645321)]
    [InlineData("-123456789.987645321", -123456789.987645321)]
    [InlineData(".23456789", 0.23456789)]
    [InlineData("+1.23456789", 1.23456789)]
    public void Test_For_Real_Numbers(string asciiData, double expected)
    {
        var provider = new PdfByteArrayProvider(Encoding.ASCII.GetBytes(asciiData));
        var tokeniser = new Tokenizer(provider);

        if (!tokeniser.TryGetNextToken(out Token? token))
            Assert.Fail("Could not get the next token.");

        if (token is TokenNumber number)
            Assert.Equal(expected, number.Value, tolerance: 0.00000000001);
        else
            Assert.Fail($"Did not retrieve a real token, instead got a {token.GetType().Name}.");
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void Test_For_Boolean_Tokens(string asciiData, bool expected)
    {
        var provider = new PdfByteArrayProvider(Encoding.ASCII.GetBytes(asciiData));
        var tokeniser = new Tokenizer(provider);

        if (!tokeniser.TryGetNextToken(out Token? token))
            Assert.Fail("Could not get the next token.");

        if (token is TokenBoolean boolean)
            Assert.Equal(expected, boolean.Value);
        else
            Assert.Fail($"Did not retrieve a boolean token, instead got a {token.GetType().Name}.");
    }
}
