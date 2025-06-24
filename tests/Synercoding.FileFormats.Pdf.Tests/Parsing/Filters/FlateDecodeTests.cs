using Synercoding.FileFormats.Pdf.Parsing.Filters;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.Parsing.Filters;

public class FlateDecodeTests
{
    [Theory]
    [InlineData("Hello World!", "Hello World!")]
    [InlineData("", "")]
    [InlineData("A", "A")]
    [InlineData("This is a longer test string with multiple words and punctuation!", "This is a longer test string with multiple words and punctuation!")]
    [InlineData("AAAAAAAAAAAAAAAAAAAAAAA", "AAAAAAAAAAAAAAAAAAAAAAA")] // Highly compressible
    public void Test_Encode_Then_Decode_RoundTrip(string input, string expected)
    {
        var filter = new FlateDecode();
        var inputBytes = Encoding.UTF8.GetBytes(input);

        var encoded = filter.Encode(inputBytes, null);
        var decoded = filter.Decode(encoded, null, null!);

        Assert.Equal(expected, Encoding.UTF8.GetString(decoded));
    }

    [Fact]
    public void Test_Decode_EmptyInput()
    {
        var filter = new FlateDecode();
        var input = new byte[] { 0x78, 0x9C, 0x03, 0x00, 0x00, 0x00, 0x00, 0x01 }; // Empty zlib stream

        var result = filter.Decode(input, null, null!);

        Assert.Empty(result);
    }

    [Fact]
    public void Test_Encode_EmptyInput()
    {
        var filter = new FlateDecode();
        var input = Array.Empty<byte>();

        var result = filter.Encode(input, null);

        Assert.NotEmpty(result); // Should contain zlib header
        Assert.True(result.Length >= 2); // At least header bytes
    }

    [Theory]
    [InlineData(new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F })] // "Hello"
    [InlineData(new byte[] { 0x00, 0x01, 0x02, 0x03, 0xFF })] // Binary data
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF })] // All 1s
    public void Test_Binary_Data_RoundTrip(byte[] input)
    {
        var filter = new FlateDecode();

        var encoded = filter.Encode(input, null);
        var decoded = filter.Decode(encoded, null, null!);

        Assert.Equal(input, decoded);
    }

    [Fact]
    public void Test_Large_Data_RoundTrip()
    {
        var filter = new FlateDecode();
        var input = new byte[10000];
        new Random(42).NextBytes(input); // Deterministic random data

        var encoded = filter.Encode(input, null);
        var decoded = filter.Decode(encoded, null, null!);

        Assert.Equal(input, decoded);
    }

    [Fact]
    public void Test_Compression_Efficiency()
    {
        var filter = new FlateDecode();
        var input = Encoding.UTF8.GetBytes(new string('A', 1000)); // Highly compressible

        var encoded = filter.Encode(input, null);

        Assert.True(encoded.Length < input.Length, "Compressed data should be smaller than input for highly repetitive data");
    }

    [Fact]
    public void Test_Filter_Name()
    {
        var filter = new FlateDecode();
        
        Assert.Equal("FlateDecode", filter.Name.Display);
    }
}
