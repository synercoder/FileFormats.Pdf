using Synercoding.FileFormats.Pdf.Parsing.Filters;
using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Parsing;

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
        var result = _filter.Decode(Array.Empty<byte>(), null, null!);
        
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
        var decoded = _filter.Decode(encoded, null, null!);
        
        Assert.Equal(input, decoded);
    }

    [Fact]
    public void Encode_TwoIdenticalBytes_ReturnsCorrectEncoding()
    {
        var input = new byte[] { 65, 65 }; // 'AA'
        
        var encoded = _filter.Encode(input, null);
        var decoded = _filter.Decode(encoded, null, null!);
        
        Assert.Equal(input, decoded);
    }

    [Fact]
    public void Encode_SimpleString_RoundTripWorks()
    {
        var input = System.Text.Encoding.ASCII.GetBytes("ABCABC");
        
        var encoded = _filter.Encode(input, null);
        var decoded = _filter.Decode(encoded, null, null!);
        
        Assert.Equal(input, decoded);
    }

    [Fact]
    public void Encode_RepeatingPattern_RoundTripWorks()
    {
        var input = System.Text.Encoding.ASCII.GetBytes("AAAAAABBBBBBCCCCCC");
        
        var encoded = _filter.Encode(input, null);
        var decoded = _filter.Decode(encoded, null, null!);
        
        Assert.Equal(input, decoded);
    }

    [Fact]
    public void Encode_LongRepeatingPattern_RoundTripWorks()
    {
        var input = System.Text.Encoding.ASCII.GetBytes("ABCABCABCABCABCABC");
        
        var encoded = _filter.Encode(input, null);
        var decoded = _filter.Decode(encoded, null, null!);
        
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
        var decoded = _filter.Decode(encoded, null, null!);
        
        Assert.Equal(input, decoded);
    }

    [Fact]
    public void Encode_BinaryData_RoundTripWorks()
    {
        var input = new byte[] { 0, 1, 2, 3, 4, 5, 255, 254, 253, 0, 1, 2 };
        
        var encoded = _filter.Encode(input, null);
        var decoded = _filter.Decode(encoded, null, null!);
        
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
        var decoded = _filter.Decode(encoded, null, null!);
        
        Assert.Equal(inputArray, decoded);
        
        // Verify compression occurred (encoded should be smaller than input for repetitive data)
        Assert.True(encoded.Length < inputArray.Length, 
            $"Expected compression but encoded length ({encoded.Length}) >= input length ({inputArray.Length})");
    }

    [Fact]
    public void Decode_InvalidData_ThrowsException()
    {
        var invalidData = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
        
        Assert.Throws<InvalidDataException>(() => _filter.Decode(invalidData, null, null!));
    }

    [Fact]
    public void Encode_NullParameters_WorksCorrectly()
    {
        var input = System.Text.Encoding.ASCII.GetBytes("Test");
        
        var encoded = _filter.Encode(input, null);
        var decoded = _filter.Decode(encoded, null, null!);
        
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
        var decoded = _filter.Decode(encoded, null, null!);
        
        Assert.Equal(input, decoded);
    }

    [Fact]
    public void Encode_SmallRandomData_RoundTripWorks()
    {
        var random = new Random(42); // Fixed seed for reproducible tests
        var input = new byte[50]; // Much smaller test first
        random.NextBytes(input);
        
        var encoded = _filter.Encode(input, null);
        var decoded = _filter.Decode(encoded, null, null!);
        
        Assert.Equal(input, decoded);
    }

    [Fact]
    public void Encode_DataWithNullBytes_RoundTripWorks()
    {
        var input = new byte[] { 0, 1, 0, 2, 0, 3, 0, 0, 0, 4 };
        
        var encoded = _filter.Encode(input, null);
        var decoded = _filter.Decode(encoded, null, null!);
        
        Assert.Equal(input, decoded);
    }

    [Fact]
    public void Encode_MaxByteValues_RoundTripWorks()
    {
        var input = new byte[] { 255, 255, 254, 254, 253, 253 };
        
        var encoded = _filter.Encode(input, null);
        var decoded = _filter.Decode(encoded, null, null!);
        
        Assert.Equal(input, decoded);
    }

    [Fact]
    public void Decode_EarlyChangeDefault_UsesDefaultValue1()
    {
        var input = System.Text.Encoding.ASCII.GetBytes("ABCABCABC");
        
        var encoded = _filter.Encode(input, null);
        var decoded = _filter.Decode(encoded, null, null!);
        
        Assert.Equal(input, decoded);
    }

    [Fact]
    public void Decode_EarlyChange1_RoundTripWorks()
    {
        var input = System.Text.Encoding.ASCII.GetBytes("ABCABCABCABC");
        
        var parameters = new PdfDictionary();
        parameters.Add(PdfNames.EarlyChange, new PdfNumber(1));
        
        var encoded = _filter.Encode(input, parameters);
        var objectReader = _createMockObjectReader();
        var decoded = _filter.Decode(encoded, parameters, objectReader);
        
        Assert.Equal(input, decoded);
    }

    [Fact]
    public void Decode_EarlyChange0_RoundTripWorks()
    {
        var input = System.Text.Encoding.ASCII.GetBytes("ABCABCABCABC");
        
        var parameters = new PdfDictionary();
        parameters.Add(PdfNames.EarlyChange, new PdfNumber(0));
        
        var encoded = _filter.Encode(input, parameters);
        var objectReader = _createMockObjectReader();
        var decoded = _filter.Decode(encoded, parameters, objectReader);
        
        Assert.Equal(input, decoded);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void Decode_EarlyChangeParameter_RoundTripWorks(int earlyChangeValue)
    {
        var input = System.Text.Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog. The quick brown fox jumps over the lazy dog.");
        
        var parameters = new PdfDictionary();
        parameters.Add(PdfNames.EarlyChange, new PdfNumber(earlyChangeValue));
        
        var encoded = _filter.Encode(input, parameters);
        var objectReader = _createMockObjectReader();
        var decoded = _filter.Decode(encoded, parameters, objectReader);
        
        Assert.Equal(input, decoded);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void Decode_EarlyChangeWithLargeData_RoundTripWorks(int earlyChangeValue)
    {
        var pattern = System.Text.Encoding.ASCII.GetBytes("Pattern123");
        var input = new List<byte>();
        
        // Create large repetitive data to trigger code length increases
        for (int i = 0; i < 100; i++)
        {
            input.AddRange(pattern);
            input.AddRange(System.Text.Encoding.ASCII.GetBytes(i.ToString()));
        }
        
        var inputArray = input.ToArray();
        var parameters = new PdfDictionary();
        parameters.Add(PdfNames.EarlyChange, new PdfNumber(earlyChangeValue));
        
        var encoded = _filter.Encode(inputArray, parameters);
        var objectReader = _createMockObjectReader();
        var decoded = _filter.Decode(encoded, parameters, objectReader);
        
        Assert.Equal(inputArray, decoded);
    }

    [Fact]
    public void Decode_EarlyChangeFractionalNumber_UsesDefaultValue1()
    {
        var input = System.Text.Encoding.ASCII.GetBytes("TestData");
        
        var parameters = new PdfDictionary();
        parameters.Add(PdfNames.EarlyChange, new PdfNumber(1.5)); // Fractional number should be ignored
        
        var encoded = _filter.Encode(input, parameters);
        var objectReader = _createMockObjectReader();
        var decoded = _filter.Decode(encoded, parameters, objectReader);
        
        Assert.Equal(input, decoded);
    }

    [Fact]
    public void Decode_EarlyChangeNonIntegerParameter_UsesDefaultValue1()
    {
        var input = System.Text.Encoding.ASCII.GetBytes("TestData");
        
        var parameters = new PdfDictionary();
        parameters.Add(PdfNames.EarlyChange, new PdfString(System.Text.Encoding.ASCII.GetBytes("NotANumber"), false)); // Wrong type should be ignored
        
        var encoded = _filter.Encode(input, null); // Encode without parameters
        var objectReader = _createMockObjectReader();
        var decoded = _filter.Decode(encoded, parameters, objectReader);
        
        Assert.Equal(input, decoded);
    }

    [Theory]
    [InlineData(-1)] // Invalid negative value
    [InlineData(2)]  // Invalid value > 1
    [InlineData(100)] // Invalid large value
    public void Decode_EarlyChangeInvalidValues_StillProcesses(int earlyChangeValue)
    {
        var input = System.Text.Encoding.ASCII.GetBytes("TestData");
        
        var parameters = new PdfDictionary();
        parameters.Add(PdfNames.EarlyChange, new PdfNumber(earlyChangeValue));
        
        var encoded = _filter.Encode(input, parameters);
        var objectReader = _createMockObjectReader();
        var decoded = _filter.Decode(encoded, parameters, objectReader);
        
        Assert.Equal(input, decoded);
    }

    [Fact]
    public void Decode_EarlyChange0VsEarlyChange1_ProducesSameResult()
    {
        var input = System.Text.Encoding.ASCII.GetBytes("ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZ");
        
        var parameters0 = new PdfDictionary();
        parameters0.Add(PdfNames.EarlyChange, new PdfNumber(0));
        
        var parameters1 = new PdfDictionary();
        parameters1.Add(PdfNames.EarlyChange, new PdfNumber(1));
        
        var encoded0 = _filter.Encode(input, parameters0);
        var encoded1 = _filter.Encode(input, parameters1);
        
        var objectReader = _createMockObjectReader();
        var decoded0 = _filter.Decode(encoded0, parameters0, objectReader);
        var decoded1 = _filter.Decode(encoded1, parameters1, objectReader);
        
        // Both should decode to the same original input
        Assert.Equal(input, decoded0);
        Assert.Equal(input, decoded1);
        Assert.Equal(decoded0, decoded1);
    }

    private static ObjectReader _createMockObjectReader()
    {
        // Create a minimal PDF content for ObjectReader
        var pdfContent = 
            "%PDF-1.7" + '\n' +
            "1 0 obj" + '\n' +
            "123" + '\n' +
            "endobj" + '\n' +
            "xref" + '\n' +
            "0 2" + '\n' +
            "0000000000 65535 f " + '\n' +
            "0000000009 00000 n " + '\n' +
            "trailer" + '\n' +
            "<< /Size 2 >>" + '\n' +
            "startxref" + '\n' +
            "28" + '\n' +
            "%%EOF";
        
        var bytes = System.Text.Encoding.ASCII.GetBytes(pdfContent);
        var provider = new PdfByteArrayProvider(bytes);
        return new ObjectReader(provider, new ReaderSettings());
    }
}
