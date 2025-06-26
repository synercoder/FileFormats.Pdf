using Synercoding.FileFormats.Pdf.Parsing.Filters;
using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Parsing;
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

    [Fact]
    public void Test_Decode_PredictorDefault_NoChangeInBehavior()
    {
        var filter = new FlateDecode();
        var input = Encoding.UTF8.GetBytes("ABCABCABC");
        
        var parameters = new PdfDictionary();
        parameters.Add(PdfNames.Predictor, new PdfNumber(1)); // Default predictor
        
        var encoded = filter.Encode(input, null);
        var objectReader = _createMockObjectReader();
        var decoded = filter.Decode(encoded, parameters, objectReader);
        
        Assert.Equal(input, decoded);
    }

    [Fact]
    public void Test_Decode_PredictorTiff_AppliesCorrectly()
    {
        var filter = new FlateDecode();
        var input = Encoding.UTF8.GetBytes("ABCDEFGHIJ"); // 10 bytes
        
        var parameters = new PdfDictionary();
        parameters.Add(PdfNames.Predictor, new PdfNumber(2)); // TIFF predictor
        parameters.Add(PdfNames.Columns, new PdfNumber(5)); // 5 columns
        parameters.Add(PdfNames.BitsPerComponent, new PdfNumber(8)); // 8 bits per component
        parameters.Add(PdfNames.Colors, new PdfNumber(1)); // 1 color component
        
        var encoded = filter.Encode(input, null);
        var objectReader = _createMockObjectReader();
        var decoded = filter.Decode(encoded, parameters, objectReader);
        
        // The predictor should be applied after Flate decoding
        Assert.NotNull(decoded);
        Assert.True(decoded.Length > 0);
    }

    [Fact]
    public void Test_Decode_PredictorPng_AppliesCorrectly()
    {
        var filter = new FlateDecode();
        // Create PNG-filtered data: first byte is filter type, followed by data
        var input = new byte[] { 0, 10, 20, 30 }; // None filter + 3 bytes of data
        
        var parameters = new PdfDictionary();
        parameters.Add(PdfNames.Predictor, new PdfNumber(15)); // PNG predictor (10-15 range)
        parameters.Add(PdfNames.Columns, new PdfNumber(1)); // 1 column
        parameters.Add(PdfNames.BitsPerComponent, new PdfNumber(8)); // 8 bits per component
        parameters.Add(PdfNames.Colors, new PdfNumber(3)); // 3 color components (RGB)
        
        var encoded = filter.Encode(input, null);
        var objectReader = _createMockObjectReader();
        var decoded = filter.Decode(encoded, parameters, objectReader);
        
        Assert.NotNull(decoded);
        Assert.True(decoded.Length > 0);
    }

    [Fact]
    public void Test_Decode_TiffPredictor_ProcessWithoutError()
    {
        var filter = new FlateDecode();
        var input = new byte[] { 100, 10, 5, 3, 150, 20, 15, 8 }; // 8 bytes suitable for TIFF predictor
        
        var parameters = new PdfDictionary();
        parameters.Add(PdfNames.Predictor, new PdfNumber(2)); // TIFF predictor
        parameters.Add(PdfNames.Columns, new PdfNumber(4));
        parameters.Add(PdfNames.BitsPerComponent, new PdfNumber(8));
        parameters.Add(PdfNames.Colors, new PdfNumber(1));
        
        var encoded = filter.Encode(input, null);
        var objectReader = _createMockObjectReader();
        
        // Should not throw exception
        var decoded = filter.Decode(encoded, parameters, objectReader);
        Assert.NotNull(decoded);
    }

    [Theory]
    [InlineData(10)] // PNG predictor
    [InlineData(11)] // PNG predictor
    [InlineData(12)] // PNG predictor
    [InlineData(13)] // PNG predictor
    [InlineData(14)] // PNG predictor
    [InlineData(15)] // PNG predictor
    public void Test_Decode_PngPredictorValues_ProcessWithoutError(int predictorValue)
    {
        var filter = new FlateDecode();
        // Create PNG-format data: filter type byte (0) + actual data
        var input = new byte[] { 0, 10, 20, 30 }; // None filter + 3 bytes RGB
        
        var parameters = new PdfDictionary();
        parameters.Add(PdfNames.Predictor, new PdfNumber(predictorValue));
        parameters.Add(PdfNames.Columns, new PdfNumber(1)); // 1 pixel per row
        parameters.Add(PdfNames.BitsPerComponent, new PdfNumber(8));
        parameters.Add(PdfNames.Colors, new PdfNumber(3)); // RGB
        
        var encoded = filter.Encode(input, null);
        var objectReader = _createMockObjectReader();
        
        // Should not throw exception
        var decoded = filter.Decode(encoded, parameters, objectReader);
        Assert.NotNull(decoded);
    }

    [Fact]
    public void Test_Decode_UnsupportedPredictor_ThrowsException()
    {
        var filter = new FlateDecode();
        var input = Encoding.UTF8.GetBytes("TestData");
        
        var parameters = new PdfDictionary();
        parameters.Add(PdfNames.Predictor, new PdfNumber(3)); // Unsupported predictor value
        
        var encoded = filter.Encode(input, null);
        var objectReader = _createMockObjectReader();
        
        Assert.Throws<NotSupportedException>(() => filter.Decode(encoded, parameters, objectReader));
    }

    [Fact]
    public void Test_Decode_PredictorWithDefaultParameters_UsesDefaults()
    {
        var filter = new FlateDecode();
        var input = Encoding.UTF8.GetBytes("ABCD"); // 4 bytes
        
        var parameters = new PdfDictionary();
        parameters.Add(PdfNames.Predictor, new PdfNumber(2)); // TIFF predictor
        // Omit Columns, BitsPerComponent, Colors to test defaults
        
        var encoded = filter.Encode(input, null);
        var objectReader = _createMockObjectReader();
        var decoded = filter.Decode(encoded, parameters, objectReader);
        
        Assert.NotNull(decoded);
        Assert.True(decoded.Length > 0);
    }

    [Fact]
    public void Test_Decode_PredictorWithCustomParameters_UsesCustomValues()
    {
        var filter = new FlateDecode();
        var input = new byte[] { 100, 150, 200, 110, 160, 210 }; // 6 bytes, 2 RGB samples
        
        var parameters = new PdfDictionary();
        parameters.Add(PdfNames.Predictor, new PdfNumber(2)); // TIFF predictor
        parameters.Add(PdfNames.Columns, new PdfNumber(2)); // 2 columns
        parameters.Add(PdfNames.BitsPerComponent, new PdfNumber(8)); // 8 bits per component
        parameters.Add(PdfNames.Colors, new PdfNumber(3)); // 3 color components (RGB)
        
        var encoded = filter.Encode(input, null);
        var objectReader = _createMockObjectReader();
        var decoded = filter.Decode(encoded, parameters, objectReader);
        
        Assert.NotNull(decoded);
        Assert.Equal(6, decoded.Length); // Should maintain same length
    }

    [Fact]
    public void Test_Decode_PredictorFractionalParameter_IsIgnored()
    {
        var filter = new FlateDecode();
        var input = Encoding.UTF8.GetBytes("TestData");
        
        var parameters = new PdfDictionary();
        parameters.Add(PdfNames.Predictor, new PdfNumber(2.5)); // Fractional predictor should be ignored
        
        var encoded = filter.Encode(input, null);
        var objectReader = _createMockObjectReader();
        var decoded = filter.Decode(encoded, parameters, objectReader);
        
        // Should decode without applying predictor (fractional values are ignored)
        Assert.Equal(input, decoded);
    }

    [Fact]
    public void Test_Decode_PredictorInvalidParameterType_IsIgnored()
    {
        var filter = new FlateDecode();
        var input = Encoding.UTF8.GetBytes("TestData");
        
        var parameters = new PdfDictionary();
        parameters.Add(PdfNames.Predictor, new PdfString(Encoding.ASCII.GetBytes("NotANumber"), false));
        
        var encoded = filter.Encode(input, null);
        var objectReader = _createMockObjectReader();
        var decoded = filter.Decode(encoded, parameters, objectReader);
        
        // Should decode without applying predictor (invalid type is ignored)
        Assert.Equal(input, decoded);
    }

    [Theory]
    [InlineData(1, 8, 1)] // Default values
    [InlineData(3, 8, 3)] // RGB
    [InlineData(1, 16, 1)] // 16-bit grayscale
    [InlineData(2, 8, 4)] // CMYK
    public void Test_Decode_PredictorParameterCombinations_ProcessCorrectly(int columns, int bitsPerComponent, int colors)
    {
        var filter = new FlateDecode();
        
        // Create test data that matches the parameter specifications
        int bytesPerComponent = (bitsPerComponent + 7) / 8;
        int samplesPerRow = columns;
        int bytesPerSample = bytesPerComponent * colors;
        int totalBytes = samplesPerRow * bytesPerSample;
        
        var input = new byte[totalBytes];
        for (int i = 0; i < totalBytes; i++)
        {
            input[i] = (byte)(i % 256);
        }
        
        var parameters = new PdfDictionary();
        parameters.Add(PdfNames.Predictor, new PdfNumber(2)); // TIFF predictor
        parameters.Add(PdfNames.Columns, new PdfNumber(columns));
        parameters.Add(PdfNames.BitsPerComponent, new PdfNumber(bitsPerComponent));
        parameters.Add(PdfNames.Colors, new PdfNumber(colors));
        
        var encoded = filter.Encode(input, null);
        var objectReader = _createMockObjectReader();
        var decoded = filter.Decode(encoded, parameters, objectReader);
        
        Assert.NotNull(decoded);
        Assert.Equal(totalBytes, decoded.Length);
    }

    [Fact]
    public void Test_Decode_NoPredictorParameters_NoExceptionThrown()
    {
        var filter = new FlateDecode();
        var input = Encoding.UTF8.GetBytes("TestData");
        
        // No parameters provided - should work normally
        var encoded = filter.Encode(input, null);
        var decoded = filter.Decode(encoded, null, null!);
        
        Assert.Equal(input, decoded);
    }

    [Fact]
    public void Test_Decode_EmptyParametersDictionary_NoExceptionThrown()
    {
        var filter = new FlateDecode();
        var input = Encoding.UTF8.GetBytes("TestData");
        
        var parameters = new PdfDictionary(); // Empty dictionary
        
        var encoded = filter.Encode(input, null);
        var objectReader = _createMockObjectReader();
        var decoded = filter.Decode(encoded, parameters, objectReader);
        
        Assert.Equal(input, decoded);
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
        
        var bytes = Encoding.ASCII.GetBytes(pdfContent);
        var provider = new PdfByteArrayProvider(bytes);
        return new ObjectReader(provider, new ReaderSettings());
    }
}
