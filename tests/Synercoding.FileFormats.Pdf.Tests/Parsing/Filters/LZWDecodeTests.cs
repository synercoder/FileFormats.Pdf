using Synercoding.FileFormats.Pdf.Parsing.Filters;
using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Tests.Parsing.Filters;

public class LZWDecodeTests
{
    private readonly LZWDecode _filter = new();

    [Fact]
    public void Name_ShouldReturnLZWDecode()
    {
        Assert.Equal(PdfNames.LZWDecode, _filter.Name);
    }

    [Fact]
    public void Decode_EmptyInput_ReturnsEmptyArray()
    {
        var result = _filter.Decode(Array.Empty<byte>(), null);
        
        Assert.Empty(result);
    }

    [Fact]
    public void Encode_EmptyInput_ReturnsEmptyArray()
    {
        var result = _filter.Encode(Array.Empty<byte>(), null);
        
        Assert.Empty(result);
    }

    [Fact]
    public void Encode_SingleByte_ReturnsCorrectEncoding()
    {
        var input = new byte[] { 65 }; // 'A'
        
        var encoded = _filter.Encode(input, null);
        var decoded = _filter.Decode(encoded, null);
        
        Assert.Equal(input, decoded);
    }

    [Fact]
    public void Encode_TwoIdenticalBytes_ReturnsCorrectEncoding()
    {
        var input = new byte[] { 65, 65 }; // 'AA'
        
        var encoded = _filter.Encode(input, null);
        var decoded = _filter.Decode(encoded, null);
        
        Assert.Equal(input, decoded);
    }

    [Fact]
    public void Encode_SimpleString_RoundTripWorks()
    {
        var input = System.Text.Encoding.ASCII.GetBytes("ABCABC");
        
        var encoded = _filter.Encode(input, null);
        var decoded = _filter.Decode(encoded, null);
        
        Assert.Equal(input, decoded);
    }

    [Fact]
    public void Encode_RepeatingPattern_RoundTripWorks()
    {
        var input = System.Text.Encoding.ASCII.GetBytes("AAAAAABBBBBBCCCCCC");
        
        var encoded = _filter.Encode(input, null);
        var decoded = _filter.Decode(encoded, null);
        
        Assert.Equal(input, decoded);
    }

    [Fact]
    public void Encode_LongRepeatingPattern_RoundTripWorks()
    {
        var input = System.Text.Encoding.ASCII.GetBytes("ABCABCABCABCABCABC");
        
        var encoded = _filter.Encode(input, null);
        var decoded = _filter.Decode(encoded, null);
        
        Assert.Equal(input, decoded);
    }

    [Fact]
    public void Encode_AllAsciiCharacters_RoundTripWorks()
    {
        var input = new byte[128];
        for (int i = 0; i < 128; i++)
        {
            input[i] = (byte)i;
        }
        
        var encoded = _filter.Encode(input, null);
        var decoded = _filter.Decode(encoded, null);
        
        Assert.Equal(input, decoded);
    }

    [Fact]
    public void Encode_BinaryData_RoundTripWorks()
    {
        var input = new byte[] { 0, 1, 2, 3, 4, 5, 255, 254, 253, 0, 1, 2 };
        
        var encoded = _filter.Encode(input, null);
        var decoded = _filter.Decode(encoded, null);
        
        Assert.Equal(input, decoded);
    }

    [Fact]
    public void Encode_LargeDataWithPatterns_RoundTripWorks()
    {
        var pattern = System.Text.Encoding.ASCII.GetBytes("Hello World! ");
        var input = new List<byte>();
        
        // Repeat pattern 100 times to create larger data with repetition
        for (int i = 0; i < 100; i++)
        {
            input.AddRange(pattern);
        }
        
        var inputArray = input.ToArray();
        var encoded = _filter.Encode(inputArray, null);
        var decoded = _filter.Decode(encoded, null);
        
        Assert.Equal(inputArray, decoded);
        
        // Verify compression occurred (encoded should be smaller than input for repetitive data)
        Assert.True(encoded.Length < inputArray.Length, 
            $"Expected compression but encoded length ({encoded.Length}) >= input length ({inputArray.Length})");
    }

    [Fact]
    public void Decode_InvalidData_ThrowsException()
    {
        var invalidData = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
        
        Assert.Throws<InvalidDataException>(() => _filter.Decode(invalidData, null));
    }

    [Fact]
    public void Encode_NullParameters_WorksCorrectly()
    {
        var input = System.Text.Encoding.ASCII.GetBytes("Test");
        
        var encoded = _filter.Encode(input, null);
        var decoded = _filter.Decode(encoded, null);
        
        Assert.Equal(input, decoded);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("AB")]
    [InlineData("ABC")]
    [InlineData("ABCD")]
    [InlineData("ABCDE")]
    [InlineData("The quick brown fox jumps over the lazy dog")]
    [InlineData("AAAAAAAAAAAAAAAAAAAA")]
    [InlineData("ABCABCABCABCABCABC")]
    [InlineData("123456789012345678901234567890")]
    public void Encode_VariousStrings_RoundTripWorks(string testString)
    {
        var input = System.Text.Encoding.ASCII.GetBytes(testString);
        
        var encoded = _filter.Encode(input, null);
        var decoded = _filter.Decode(encoded, null);
        
        Assert.Equal(input, decoded);
    }

    [Fact]
    public void Encode_SmallRandomData_RoundTripWorks()
    {
        var random = new Random(42); // Fixed seed for reproducible tests
        var input = new byte[50]; // Much smaller test first
        random.NextBytes(input);
        
        var encoded = _filter.Encode(input, null);
        var decoded = _filter.Decode(encoded, null);
        
        Assert.Equal(input, decoded);
    }

    [Fact]
    public void Encode_DataWithNullBytes_RoundTripWorks()
    {
        var input = new byte[] { 0, 1, 0, 2, 0, 3, 0, 0, 0, 4 };
        
        var encoded = _filter.Encode(input, null);
        var decoded = _filter.Decode(encoded, null);
        
        Assert.Equal(input, decoded);
    }

    [Fact]
    public void Encode_MaxByteValues_RoundTripWorks()
    {
        var input = new byte[] { 255, 255, 254, 254, 253, 253 };
        
        var encoded = _filter.Encode(input, null);
        var decoded = _filter.Decode(encoded, null);
        
        Assert.Equal(input, decoded);
    }
}