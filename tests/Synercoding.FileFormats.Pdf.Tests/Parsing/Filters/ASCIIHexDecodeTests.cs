using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.Parsing.Filters;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.Parsing.Filters;
public class ASCIIHexDecodeTests
{
    [Theory]
    [InlineData("A0   B3 0F9A>", new byte[] { 0xA0, 0xB3, 0x0F, 0x9A })]
    [InlineData("A0   B3 0F9AA>", new byte[] { 0xA0, 0xB3, 0x0F, 0x9A, 0xA0 })]
    [InlineData("A0   B3 0F9AA    >", new byte[] { 0xA0, 0xB3, 0x0F, 0x9A, 0xA0 })]
    [InlineData("A0   B3 0F9A", new byte[] { 0xA0, 0xB3, 0x0F, 0x9A })]
    [InlineData("A0   B3 0F9AA", new byte[] { 0xA0, 0xB3, 0x0F, 0x9A, 0xA0 })]
    [InlineData("A0   B3 0F9AA    ", new byte[] { 0xA0, 0xB3, 0x0F, 0x9A, 0xA0 })]
    public void Test_If_Decode_Correct(string input, byte[] expected)
    {
        var filter = new ASCIIHexDecode();

        var inputBytes = Encoding.UTF8.GetBytes(input);

        var decode = filter.Decode(inputBytes, null);

        Assert.Equal(expected, decode);
    }

    [Theory]
    [InlineData(new byte[] { 0xA0, 0xB3, 0x0F, 0x9A }, "A0B30F9A>")]
    [InlineData(new byte[] { 0xA0, 0xB3, 0x0F, 0x9A, 0xA0 }, "A0B30F9AA0>")]
    public void Test_If_Encode_Correct(byte[] input, string expected)
    {
        var filter = new ASCIIHexDecode();

        var decode = filter.Encode(input, null);

        Assert.Equal(expected, Encoding.ASCII.GetString(decode));
    }

    [Theory]
    [InlineData("00>", new byte[] { 0x00 })] // Minimum value
    [InlineData("FF>", new byte[] { 0xFF })] // Maximum value
    [InlineData("0>", new byte[] { 0x00 })] // Single odd hex digit (should be padded with 0)
    [InlineData("F>", new byte[] { 0xF0 })] // Single odd hex digit
    [InlineData("1", new byte[] { 0x10 })] // Single odd hex digit without EOD
    [InlineData("", new byte[] { })] // Empty input
    [InlineData(">", new byte[] { })] // Just EOD marker
    public void Test_Decode_EdgeCases(string input, byte[] expected)
    {
        var filter = new ASCIIHexDecode();
        var inputBytes = Encoding.UTF8.GetBytes(input);

        var result = filter.Decode(inputBytes, null);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("0123456789ABCDEF>", new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF })] // All uppercase hex digits
    [InlineData("0123456789abcdef>", new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xab, 0xcd, 0xef })] // All lowercase hex digits
    [InlineData("0123456789AbCdEf>", new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAb, 0xCd, 0xEf })] // Mixed case
    public void Test_Decode_HexDigitVariations(string input, byte[] expected)
    {
        var filter = new ASCIIHexDecode();
        var inputBytes = Encoding.UTF8.GetBytes(input);

        var result = filter.Decode(inputBytes, null);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("  \t\n\r  41\t\n\r  42  \t\n\r  >", new byte[] { 0x41, 0x42 })] // Various whitespace characters
    [InlineData(" 4 1 4 2 >", new byte[] { 0x41, 0x42 })] // Spaces between digits
    [InlineData("\t4\n1\r4\t2\n>", new byte[] { 0x41, 0x42 })] // Mixed whitespace between digits
    public void Test_Decode_WhitespaceHandling(string input, byte[] expected)
    {
        var filter = new ASCIIHexDecode();
        var inputBytes = Encoding.UTF8.GetBytes(input);

        var result = filter.Decode(inputBytes, null);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("G0>")] // Invalid hex character G
    [InlineData("0G>")] // Invalid hex character in second position
    [InlineData("4Z>")] // Invalid hex character Z
    [InlineData("@0>")] // Invalid character @
    [InlineData("0#>")] // Invalid character #
    public void Test_Decode_InvalidHexCharacters_ThrowsParseException(string input)
    {
        var filter = new ASCIIHexDecode();
        var inputBytes = Encoding.UTF8.GetBytes(input);

        Assert.Throws<ParseException>(() => filter.Decode(inputBytes, null));
    }

    [Theory]
    [InlineData(new byte[] { })] // Empty input
    [InlineData(new byte[] { 0x00 })] // Single byte
    [InlineData(new byte[] { 0xFF })] // Maximum byte value
    [InlineData(new byte[] { 0x00, 0x01, 0x02, 0xFE, 0xFF })] // Various values
    public void Test_Encode_EdgeCases(byte[] input)
    {
        var filter = new ASCIIHexDecode();

        var encoded = filter.Encode(input, null);
        var encodedString = Encoding.ASCII.GetString(encoded);

        // Should always end with >
        Assert.EndsWith(">", encodedString);
        
        // Should have correct length: 2 chars per byte + 1 for EOD
        Assert.Equal((input.Length * 2) + 1, encoded.Length);
    }

    [Theory]
    [InlineData(new byte[] { 0x41, 0x42, 0x43 })] // ABC
    [InlineData(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04 })] // Low values
    [InlineData(new byte[] { 0xFB, 0xFC, 0xFD, 0xFE, 0xFF })] // High values
    [InlineData(new byte[] { 0x10, 0x20, 0x30, 0x40, 0x50 })] // Round values
    [InlineData(new byte[] { 0x0F, 0x1F, 0x2F, 0x3F, 0x4F })] // Values ending in F
    public void Test_Encode_Then_Decode_RoundTrip(byte[] original)
    {
        var filter = new ASCIIHexDecode();

        var encoded = filter.Encode(original, null);
        var decoded = filter.Decode(encoded, null);

        Assert.Equal(original, decoded);
    }

    [Fact]
    public void Test_Decode_Large_Input()
    {
        var filter = new ASCIIHexDecode();
        
        // Create a large input with 1000 bytes worth of hex data
        var originalData = new byte[1000];
        for (int i = 0; i < 1000; i++)
            originalData[i] = (byte)(i % 256);

        var encoded = filter.Encode(originalData, null);
        var decoded = filter.Decode(encoded, null);

        Assert.Equal(originalData, decoded);
    }

    [Fact]
    public void Test_Filter_Name()
    {
        var filter = new ASCIIHexDecode();
        
        Assert.Equal("ASCIIHexDecode", filter.Name.Display);
    }

    [Theory]
    [InlineData("48656C6C6F>", "Hello")] // "Hello" in hex
    [InlineData("576F726C64>", "World")] // "World" in hex  
    [InlineData("48656C6C6F20576F726C64>", "Hello World")] // "Hello World" in hex
    public void Test_Decode_ReadableText(string hexInput, string expectedText)
    {
        var filter = new ASCIIHexDecode();
        var inputBytes = Encoding.UTF8.GetBytes(hexInput);
        var expected = Encoding.UTF8.GetBytes(expectedText);

        var result = filter.Decode(inputBytes, null);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_Encode_ProducesUppercaseHex()
    {
        var filter = new ASCIIHexDecode();
        var input = new byte[] { 0xab, 0xcd, 0xef }; // Contains values that would be lowercase

        var result = filter.Encode(input, null);
        var resultString = Encoding.ASCII.GetString(result);

        // Should produce uppercase hex
        Assert.Equal("ABCDEF>", resultString);
    }

    [Theory]
    [InlineData("4", new byte[] { 0x40 })] // Odd number of characters, should pad with 0
    [InlineData("41", new byte[] { 0x41 })] // Even number of characters
    [InlineData("412", new byte[] { 0x41, 0x20 })] // Odd again, last should be padded
    public void Test_Decode_OddNumberOfHexDigits(string input, byte[] expected)
    {
        var filter = new ASCIIHexDecode();
        var inputBytes = Encoding.UTF8.GetBytes(input);

        var result = filter.Decode(inputBytes, null);

        Assert.Equal(expected, result);
    }
}
