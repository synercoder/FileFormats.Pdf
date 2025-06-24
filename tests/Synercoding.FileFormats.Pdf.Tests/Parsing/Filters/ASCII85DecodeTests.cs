using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.Parsing.Filters;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.Parsing.Filters;

public class ASCII85DecodeTests
{
    [Theory]
    [InlineData("9jqo^~>", "Man ")] // Classic ASCII85 example
    public void Test_Decode_ValidInput(string input, string expectedText)
    {
        var filter = new ASCII85Decode();
        var inputBytes = Encoding.ASCII.GetBytes(input);
        var expected = Encoding.ASCII.GetBytes(expectedText);

        var result = filter.Decode(inputBytes, null, null!);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("z~>", new byte[] { 0, 0, 0, 0 })] // 'z' represents four null bytes
    [InlineData("zz~>", new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 })] // Multiple 'z' characters
    [InlineData("9jqo^z~>", new byte[] { 77, 97, 110, 32, 0, 0, 0, 0 })] // Mixed with 'z'
    public void Test_Decode_ZShortcut(string input, byte[] expected)
    {
        var filter = new ASCII85Decode();
        var inputBytes = Encoding.ASCII.GetBytes(input);

        var result = filter.Decode(inputBytes, null, null!);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("!!!!!~>", new byte[] { 0, 0, 0, 0 })] // All minimum characters
    public void Test_Decode_EdgeCases(string input, byte[] expected)
    {
        var filter = new ASCII85Decode();
        var inputBytes = Encoding.ASCII.GetBytes(input);
        
        var result = filter.Decode(inputBytes, null, null!);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("  \t\n  9jqo^  \n\r  ~>", "Man ")] // Whitespace handling
    [InlineData("9jqo^ ~>", "Man ")] // Embedded spaces
    [InlineData("\t9jqo^\t~>", "Man ")] // Various whitespace types
    public void Test_Decode_WhitespaceHandling(string input, string expectedText)
    {
        var filter = new ASCII85Decode();
        var inputBytes = Encoding.ASCII.GetBytes(input);
        var expected = Encoding.ASCII.GetBytes(expectedText);

        var result = filter.Decode(inputBytes, null, null!);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("9jqo")] // Incomplete group (4 characters instead of 5)
    [InlineData("9jq")] // Incomplete group (3 characters)
    [InlineData("9j")] // Incomplete group (2 characters)
    [InlineData("9")] // Incomplete group (1 character)
    public void Test_Decode_IncompleteGroups(string input)
    {
        var filter = new ASCII85Decode();
        var inputBytes = Encoding.ASCII.GetBytes(input);

        var result = filter.Decode(inputBytes, null, null!);

        // Should handle incomplete groups gracefully without throwing
        Assert.NotNull(result);
    }

    [Theory]
    [InlineData("v")] // Invalid character (above range 'u')
    [InlineData("9jqo^~x")] // Invalid character 'x' after ~
    public void Test_Decode_InvalidCharacters_ThrowsParseException(string input)
    {
        var filter = new ASCII85Decode();
        var inputBytes = Encoding.ASCII.GetBytes(input);

        Assert.Throws<ParseException>(() => filter.Decode(inputBytes, null, null!));
    }

    [Theory]
    [InlineData("")] // Empty input
    [InlineData("~>")] // Just end marker
    [InlineData("   \t\n   ~>")] // Whitespace and end marker
    public void Test_Decode_EmptyOrEndMarkerOnly(string input)
    {
        var filter = new ASCII85Decode();
        var inputBytes = Encoding.ASCII.GetBytes(input);

        var result = filter.Decode(inputBytes, null, null!);

        Assert.Empty(result);
    }

    [Theory]
    [InlineData("Man ")]
    [InlineData("sure.")]
    [InlineData("Hello World")]
    [InlineData("ASCII85")]
    public void Test_Encode_ValidInput(string inputText)
    {
        var filter = new ASCII85Decode();
        var inputBytes = Encoding.ASCII.GetBytes(inputText);

        var result = filter.Encode(inputBytes, null);
        
        // Should be able to decode back correctly
        var decoded = filter.Decode(result, null, null!);
        Assert.Equal(inputBytes, decoded);
    }

    [Theory]
    [InlineData(new byte[] { 0, 0, 0, 0 })] // Four null bytes should encode to 'z'
    [InlineData(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 })] // Eight null bytes should encode to 'zz'
    public void Test_Encode_ZShortcut(byte[] input)
    {
        var filter = new ASCII85Decode();

        var result = filter.Encode(input, null);
        var resultString = Encoding.ASCII.GetString(result);

        // Should contain 'z' characters for null byte groups
        Assert.Contains('z', resultString);
        Assert.EndsWith("~>", resultString);

        // Verify round-trip
        var decoded = filter.Decode(result, null, null!);
        Assert.Equal(input, decoded);
    }

    [Theory]
    [InlineData(new byte[] { })] // Empty input
    [InlineData(new byte[] { 0 })] // Single byte
    [InlineData(new byte[] { 255 })] // Maximum byte value
    [InlineData(new byte[] { 0, 1, 2, 254, 255 })] // Various values
    public void Test_Encode_EdgeCases(byte[] input)
    {
        var filter = new ASCII85Decode();

        var result = filter.Encode(input, null);
        var resultString = Encoding.ASCII.GetString(result);

        // Should always end with ~>
        Assert.EndsWith("~>", resultString);
        
        // Should only contain valid ASCII85 characters plus end marker
        var validChars = "!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstu";
        foreach (char c in resultString.TrimEnd('~', '>'))
        {
            if (c != 'z') // 'z' is special case
                Assert.Contains(c, validChars);
        }
    }

    [Theory]
    [InlineData("Man ")]
    [InlineData("sure.")]
    [InlineData("Hello World")]
    [InlineData("ASCII85")]
    [InlineData("The quick brown fox jumps over the lazy dog")]
    [InlineData("1234567890!@#$%^&*()_+-=[]{}|;':\",./<>?")]
    public void Test_Encode_Then_Decode_RoundTrip_Text(string originalText)
    {
        var filter = new ASCII85Decode();
        var originalBytes = Encoding.UTF8.GetBytes(originalText);

        var encoded = filter.Encode(originalBytes, null);
        var decoded = filter.Decode(encoded, null, null!);

        Assert.Equal(originalBytes, decoded);
    }

    [Theory]
    [InlineData(new byte[] { 77, 97, 110, 32 })] // "Man "
    [InlineData(new byte[] { 0, 0, 0, 0 })] // Four nulls
    [InlineData(new byte[] { 255, 255, 255, 255 })] // Four max bytes
    [InlineData(new byte[] { 1, 2, 3 })] // Partial group
    [InlineData(new byte[] { 1, 2, 3, 4, 5, 6, 7 })] // Non-multiple of 4
    public void Test_Encode_Then_Decode_RoundTrip_Bytes(byte[] original)
    {
        var filter = new ASCII85Decode();

        var encoded = filter.Encode(original, null);
        var decoded = filter.Decode(encoded, null, null!);

        Assert.Equal(original, decoded);
    }

    [Fact]
    public void Test_Decode_Large_Input()
    {
        var filter = new ASCII85Decode();
        
        // Create a large input with 1000 bytes
        var originalData = new byte[1000];
        for (int i = 0; i < 1000; i++)
            originalData[i] = (byte)(i % 256);

        var encoded = filter.Encode(originalData, null);
        var decoded = filter.Decode(encoded, null, null!);

        Assert.Equal(originalData, decoded);
    }

    [Fact]
    public void Test_Filter_Name()
    {
        var filter = new ASCII85Decode();
        
        Assert.Equal("ASCII85Decode", filter.Name.Display);
    }

    [Theory]
    [InlineData(new byte[] { 0x41, 0x42, 0x43 })] // ABC
    [InlineData(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04 })] // Low values
    [InlineData(new byte[] { 0xFB, 0xFC, 0xFD, 0xFE, 0xFF })] // High values
    [InlineData(new byte[] { 0x10, 0x20, 0x30, 0x40, 0x50 })] // Round values
    public void Test_Encode_ProducesValidASCII85(byte[] input)
    {
        var filter = new ASCII85Decode();

        var result = filter.Encode(input, null);
        var resultString = Encoding.ASCII.GetString(result);

        // Should end with ~>
        Assert.EndsWith("~>", resultString);
        
        // All characters except end marker should be valid ASCII85
        var content = resultString.TrimEnd('~', '>');
        foreach (char c in content)
        {
            if (c == 'z')
                continue; // Special case for null groups
            Assert.True(c >= '!' && c <= 'u', $"Character '{c}' (0x{(int)c:X2}) is not in valid ASCII85 range");
        }
    }

    [Fact]
    public void Test_Decode_Without_EndMarker()
    {
        var filter = new ASCII85Decode();
        var input = "9jqo^"; // "Man " without ~>
        var inputBytes = Encoding.ASCII.GetBytes(input);
        var expected = Encoding.ASCII.GetBytes("Man ");

        var result = filter.Decode(inputBytes, null, null!);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test_Decode_TruncatedInput_DoesNotThrow()
    {
        var filter = new ASCII85Decode();
        var input = "9jqo^BlbD"; // Truncated input
        var inputBytes = Encoding.ASCII.GetBytes(input);

        // Should handle truncated input gracefully without throwing
        var result = filter.Decode(inputBytes, null, null!);
        Assert.NotNull(result);
    }
}
