using Synercoding.FileFormats.Pdf.IO;

namespace Synercoding.FileFormats.Pdf.Tests.IO;

public class ByteUtilsTests
{
    [Theory]
    [InlineData((byte)'(', true)]
    [InlineData((byte)')', true)]
    [InlineData((byte)'<', true)]
    [InlineData((byte)'>', true)]
    [InlineData((byte)'[', true)]
    [InlineData((byte)']', true)]
    [InlineData((byte)'/', true)]
    [InlineData((byte)'%', true)]
    [InlineData((byte)'{', false)]
    [InlineData((byte)'}', false)]
    [InlineData((byte)'a', false)]
    [InlineData((byte)' ', false)]
    [InlineData((byte)'0', false)]
    public void Test_IsDelimiter_WithoutPostScript(byte input, bool expected)
    {
        var result = ByteUtils.IsDelimiter(input, insidePostScriptCalculator: false);
        
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData((byte)'{', true)]
    [InlineData((byte)'}', true)]
    [InlineData((byte)'(', true)]
    [InlineData((byte)')', true)]
    [InlineData((byte)'<', true)]
    [InlineData((byte)'>', true)]
    [InlineData((byte)'[', true)]
    [InlineData((byte)']', true)]
    [InlineData((byte)'/', true)]
    [InlineData((byte)'%', true)]
    [InlineData((byte)'a', false)]
    [InlineData((byte)' ', false)]
    [InlineData((byte)'0', false)]
    public void Test_IsDelimiter_WithPostScript(byte input, bool expected)
    {
        var result = ByteUtils.IsDelimiter(input, insidePostScriptCalculator: true);
        
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData((byte)0x00, true)] // NULL
    [InlineData((byte)0x09, true)] // HORIZONTAL_TAB
    [InlineData((byte)0x0A, true)] // LINE_FEED
    [InlineData((byte)0x0C, true)] // FORM_FEED
    [InlineData((byte)0x0D, true)] // CARRIAGE_RETURN
    [InlineData((byte)0x20, true)] // SPACE
    [InlineData((byte)'a', false)]
    [InlineData((byte)'0', false)]
    [InlineData((byte)'/', false)]
    [InlineData((byte)0x01, false)]
    [InlineData((byte)0xFF, false)]
    public void Test_IsWhiteSpace(byte input, bool expected)
    {
        var result = ByteUtils.IsWhiteSpace(input);
        
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData((byte)'(', false, true)]
    [InlineData((byte)')', false, true)]
    [InlineData((byte)0x20, false, true)] // SPACE (whitespace)
    [InlineData((byte)0x0A, false, true)] // LINE_FEED (whitespace)
    [InlineData((byte)'{', false, false)] // delimiter only in PostScript
    [InlineData((byte)'{', true, true)] // delimiter in PostScript
    [InlineData((byte)'a', false, false)]
    [InlineData((byte)'0', false, false)]
    public void Test_IsDelimiterOrWhiteSpace(byte input, bool insidePostScript, bool expected)
    {
        var result = ByteUtils.IsDelimiterOrWhiteSpace(input, insidePostScript);
        
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData((byte)0x0D, (byte)0x0A, true)] // CR + LF
    [InlineData((byte)0x0A, (byte)0x0D, false)] // LF + CR
    [InlineData((byte)0x0D, (byte)0x20, false)] // CR + SPACE
    [InlineData((byte)0x20, (byte)0x0A, false)] // SPACE + LF
    [InlineData((byte)'a', (byte)'b', false)]
    [InlineData((byte)0x00, (byte)0x00, false)]
    public void Test_IsCRLF(byte b1, byte b2, bool expected)
    {
        var result = ByteUtils.IsCRLF(b1, b2);
        
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData((byte)'0', true)]
    [InlineData((byte)'1', true)]
    [InlineData((byte)'2', true)]
    [InlineData((byte)'3', true)]
    [InlineData((byte)'4', true)]
    [InlineData((byte)'5', true)]
    [InlineData((byte)'6', true)]
    [InlineData((byte)'7', true)]
    [InlineData((byte)'8', false)]
    [InlineData((byte)'9', false)]
    [InlineData((byte)'a', false)]
    [InlineData((byte)'A', false)]
    [InlineData((byte)'/', false)]
    public void Test_IsOctal(byte input, bool expected)
    {
        var result = ByteUtils.IsOctal(input);
        
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData((byte)'0', true)]
    [InlineData((byte)'1', true)]
    [InlineData((byte)'9', true)]
    [InlineData((byte)'a', true)]
    [InlineData((byte)'f', true)]
    [InlineData((byte)'A', true)]
    [InlineData((byte)'F', true)]
    [InlineData((byte)'g', false)]
    [InlineData((byte)'G', false)]
    [InlineData((byte)'/', false)]
    [InlineData((byte)' ', false)]
    public void Test_IsHex(byte input, bool expected)
    {
        var result = ByteUtils.IsHex(input);
        
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData((byte)'a', true)]
    [InlineData((byte)'b', true)]
    [InlineData((byte)'y', true)]
    [InlineData((byte)'z', true)]
    [InlineData((byte)'A', true)]
    [InlineData((byte)'B', true)]
    [InlineData((byte)'Y', true)]
    [InlineData((byte)'Z', true)]
    [InlineData((byte)'0', false)]
    [InlineData((byte)'9', false)]
    [InlineData((byte)'/', false)]
    [InlineData((byte)' ', false)]
    [InlineData((byte)'`', false)] // Character before 'a'
    [InlineData((byte)'{', false)] // Character after 'z'
    [InlineData((byte)'@', false)] // Character before 'A'
    [InlineData((byte)'[', false)] // Character after 'Z'
    public void Test_IsChar(byte input, bool expected)
    {
        var result = ByteUtils.IsChar(input);
        
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_Constants_HaveCorrectValues()
    {
        Assert.Equal((byte)'(', ByteUtils.PARENTHESIS_OPEN);
        Assert.Equal((byte)')', ByteUtils.PARENTHESIS_CLOSED);
        Assert.Equal((byte)'<', ByteUtils.LESS_THAN_SIGN);
        Assert.Equal((byte)'>', ByteUtils.GREATER_THAN_SIGN);
        Assert.Equal((byte)'[', ByteUtils.LEFT_SQUARE_BRACKET);
        Assert.Equal((byte)']', ByteUtils.RIGHT_SQUARE_BRACKET);
        Assert.Equal((byte)'{', ByteUtils.LEFT_CURLY_BRACKET);
        Assert.Equal((byte)'}', ByteUtils.RIGHT_CURLY_BRACKET);
        Assert.Equal((byte)'/', ByteUtils.SOLIDUS);
        Assert.Equal((byte)'\\', ByteUtils.REVERSE_SOLIDUS);
        Assert.Equal((byte)'%', ByteUtils.PERCENT_SIGN);
        
        Assert.Equal(0x00, ByteUtils.NULL);
        Assert.Equal(0x09, ByteUtils.HORIZONTAL_TAB);
        Assert.Equal(0x0A, ByteUtils.LINE_FEED);
        Assert.Equal(0x0C, ByteUtils.FORM_FEED);
        Assert.Equal(0x0D, ByteUtils.CARRIAGE_RETURN);
        Assert.Equal(0x20, ByteUtils.SPACE);
    }

    [Theory]
    [InlineData(new byte[] { (byte)'(', (byte)')', (byte)'[' }, false, new bool[] { true, true, true })]
    [InlineData(new byte[] { (byte)'{', (byte)'}' }, false, new bool[] { false, false })]
    [InlineData(new byte[] { (byte)'{', (byte)'}' }, true, new bool[] { true, true })]
    [InlineData(new byte[] { (byte)0x20, (byte)0x0A, (byte)'a' }, false, new bool[] { true, true, false })]
    public void Test_IsDelimiterOrWhiteSpace_MultipleBytes(byte[] inputs, bool insidePostScript, bool[] expected)
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            var result = ByteUtils.IsDelimiterOrWhiteSpace(inputs[i], insidePostScript);
            Assert.Equal(expected[i], result);
        }
    }

    [Theory]
    [InlineData(new byte[] { (byte)'0', (byte)'7', (byte)'8' }, new bool[] { true, true, true })]
    [InlineData(new byte[] { (byte)'A', (byte)'F', (byte)'G' }, new bool[] { true, true, false })]
    [InlineData(new byte[] { (byte)'a', (byte)'f', (byte)'g' }, new bool[] { true, true, false })]
    public void Test_IsHex_MultipleBytes(byte[] inputs, bool[] expected)
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            var result = ByteUtils.IsHex(inputs[i]);
            Assert.Equal(expected[i], result);
        }
    }

    [Fact]
    public void Test_AllWhiteSpaceConstants_AreRecognizedAsWhiteSpace()
    {
        Assert.True(ByteUtils.IsWhiteSpace(ByteUtils.NULL));
        Assert.True(ByteUtils.IsWhiteSpace(ByteUtils.HORIZONTAL_TAB));
        Assert.True(ByteUtils.IsWhiteSpace(ByteUtils.LINE_FEED));
        Assert.True(ByteUtils.IsWhiteSpace(ByteUtils.FORM_FEED));
        Assert.True(ByteUtils.IsWhiteSpace(ByteUtils.CARRIAGE_RETURN));
        Assert.True(ByteUtils.IsWhiteSpace(ByteUtils.SPACE));
    }

    [Fact]
    public void Test_AllDelimiterConstants_AreRecognizedAsDelimiters()
    {
        Assert.True(ByteUtils.IsDelimiter(ByteUtils.PARENTHESIS_OPEN));
        Assert.True(ByteUtils.IsDelimiter(ByteUtils.PARENTHESIS_CLOSED));
        Assert.True(ByteUtils.IsDelimiter(ByteUtils.LESS_THAN_SIGN));
        Assert.True(ByteUtils.IsDelimiter(ByteUtils.GREATER_THAN_SIGN));
        Assert.True(ByteUtils.IsDelimiter(ByteUtils.LEFT_SQUARE_BRACKET));
        Assert.True(ByteUtils.IsDelimiter(ByteUtils.RIGHT_SQUARE_BRACKET));
        Assert.True(ByteUtils.IsDelimiter(ByteUtils.SOLIDUS));
        Assert.True(ByteUtils.IsDelimiter(ByteUtils.PERCENT_SIGN));
        
        // These are only delimiters in PostScript context
        Assert.False(ByteUtils.IsDelimiter(ByteUtils.LEFT_CURLY_BRACKET, false));
        Assert.False(ByteUtils.IsDelimiter(ByteUtils.RIGHT_CURLY_BRACKET, false));
        Assert.True(ByteUtils.IsDelimiter(ByteUtils.LEFT_CURLY_BRACKET, true));
        Assert.True(ByteUtils.IsDelimiter(ByteUtils.RIGHT_CURLY_BRACKET, true));
    }
}
