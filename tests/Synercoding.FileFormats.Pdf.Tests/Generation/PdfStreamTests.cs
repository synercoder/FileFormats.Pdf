using Synercoding.FileFormats.Pdf.Generation;
using Synercoding.FileFormats.Pdf.IO.Filters;
using Synercoding.FileFormats.Pdf.Parsing;
using Synercoding.FileFormats.Pdf.Primitives;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.Generation;

public class PdfStreamTests
{
    [Fact]
    public void Test_ToStreamObject_NoFilters_ReturnsStreamObjectWithCorrectLength()
    {
        // Arrange
        var data = Encoding.ASCII.GetBytes("Hello PDF World!");
        using var memoryStream = new MemoryStream();
        var pdfStream = new PdfStream(memoryStream);
        pdfStream.Write(data, 0, data.Length);

        // Act
        var streamObject = pdfStream.ToStreamObject();

        // Assert
        Assert.NotNull(streamObject);
        Assert.Equal(data.Length, streamObject.Length);
        Assert.Equal(data, streamObject.RawData);
        Assert.True(streamObject.ContainsKey(PdfNames.Length));
        Assert.False(streamObject.ContainsKey(PdfNames.Filter));
    }

    [Fact]
    public void Test_ToStreamObject_WithSingleFilter_ReturnsStreamObjectWithFilterAndEncodedData()
    {
        // Arrange
        var originalData = Encoding.ASCII.GetBytes("Test data for ASCII hex encoding");
        using var memoryStream = new MemoryStream();
        var pdfStream = new PdfStream(memoryStream);
        pdfStream.Write(originalData, 0, originalData.Length);

        var filter = new ASCIIHexDecode();

        // Act
        var streamObject = pdfStream.ToStreamObject(filter);

        // Assert
        Assert.NotNull(streamObject);
        Assert.True(streamObject.ContainsKey(PdfNames.Filter));
        Assert.True(streamObject.ContainsKey(PdfNames.Length));

        var filterValue = streamObject[PdfNames.Filter];
        Assert.IsType<PdfName>(filterValue);
        Assert.Equal(PdfNames.ASCIIHexDecode, filterValue);

        // Verify the data was encoded
        var encodedData = filter.Encode(originalData, null);
        Assert.Equal(encodedData.Length, streamObject.Length);
        Assert.Equal(encodedData, streamObject.RawData);
    }

    [Fact]
    public void Test_ToStreamObject_WithMultipleFilters_ReturnsStreamObjectWithFilterArrayAndEncodedData()
    {
        // Arrange
        var originalData = Encoding.ASCII.GetBytes("Test data for multiple filter encoding");
        using var memoryStream = new MemoryStream();
        var pdfStream = new PdfStream(memoryStream);
        pdfStream.Write(originalData, 0, originalData.Length);

        var asciiHexFilter = new ASCIIHexDecode();
        var flateFilter = new FlateDecode();

        // Act
        var streamObject = pdfStream.ToStreamObject(asciiHexFilter, flateFilter);

        // Assert
        Assert.NotNull(streamObject);
        Assert.True(streamObject.ContainsKey(PdfNames.Filter));
        Assert.True(streamObject.ContainsKey(PdfNames.Length));

        var filterValue = streamObject[PdfNames.Filter];
        Assert.IsType<PdfArray>(filterValue);
        var filterArray = (PdfArray)filterValue;
        Assert.Equal(2, filterArray.Count);
        Assert.Equal(PdfNames.ASCIIHexDecode, filterArray[0]);
        Assert.Equal(PdfNames.FlateDecode, filterArray[1]);

        // Verify the data was encoded with both filters in order
        var firstEncoded = asciiHexFilter.Encode(originalData, null);
        var secondEncoded = flateFilter.Encode(firstEncoded, null);
        Assert.Equal(secondEncoded.Length, streamObject.Length);
        Assert.Equal(secondEncoded, streamObject.RawData);
    }

    [Fact]
    public void Test_ToStreamObject_WithPassThroughFilter_ThrowsArgumentException()
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        var pdfStream = new PdfStream(memoryStream);
        pdfStream.Write("Some data");

        var passThroughFilter = new PassThroughFilterStub();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => pdfStream.ToStreamObject(passThroughFilter));
        Assert.Contains("Provided stream filter is passthrough", exception.Message);
        Assert.Equal("filters", exception.ParamName);
    }

    [Fact]
    public void Test_ToStreamObject_EmptyStream_ReturnsStreamObjectWithZeroLength()
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        var pdfStream = new PdfStream(memoryStream);

        // Act
        var streamObject = pdfStream.ToStreamObject();

        // Assert
        Assert.NotNull(streamObject);
        Assert.Equal(0, streamObject.Length);
        Assert.Empty(streamObject.RawData);
        Assert.True(streamObject.ContainsKey(PdfNames.Length));
        Assert.False(streamObject.ContainsKey(PdfNames.Filter));
    }

    [Fact]
    public void Test_ToStreamObject_LargeData_HandlesCorrectly()
    {
        // Arrange
        var largeData = new byte[100000]; // 100KB
        new Random(42).NextBytes(largeData);

        using var memoryStream = new MemoryStream();
        var pdfStream = new PdfStream(memoryStream);
        pdfStream.Write(largeData, 0, largeData.Length);

        // Act
        var streamObject = pdfStream.ToStreamObject();

        // Assert
        Assert.NotNull(streamObject);
        Assert.Equal(largeData.Length, streamObject.Length);
        Assert.Equal(largeData, streamObject.RawData);
    }

    [Fact]
    public void Test_ToStreamObject_NonMemoryStream_HandlesCorrectly()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            var testData = Encoding.ASCII.GetBytes("Test data for file stream");

            using (var fileStream = new FileStream(tempFile, FileMode.Create, FileAccess.ReadWrite))
            {
                var pdfStream = new PdfStream(fileStream);
                pdfStream.Write(testData, 0, testData.Length);

                // Act
                var streamObject = pdfStream.ToStreamObject();

                // Assert
                Assert.NotNull(streamObject);
                Assert.Equal(testData.Length, streamObject.Length);
                Assert.Equal(testData, streamObject.RawData);

                // Verify stream position is restored
                Assert.Equal(testData.Length, fileStream.Position);
            }
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void Test_ToStreamObject_WithFilters_PreservesOriginalStreamPosition()
    {
        // Arrange
        var data1 = Encoding.ASCII.GetBytes("First part");
        var data2 = Encoding.ASCII.GetBytes("Second part");

        using var memoryStream = new MemoryStream();
        var pdfStream = new PdfStream(memoryStream);

        pdfStream.Write(data1, 0, data1.Length);
        var positionAfterFirstWrite = memoryStream.Position;

        // Act
        var streamObject = pdfStream.ToStreamObject(new ASCIIHexDecode());

        // Write more data after creating stream object
        pdfStream.Write(data2, 0, data2.Length);

        // Assert
        Assert.Equal(positionAfterFirstWrite, data1.Length);
        Assert.Equal(data1.Length + data2.Length, memoryStream.Position);

        // Stream object should only contain first part
        var encodedData1 = new ASCIIHexDecode().Encode(data1, null);
        Assert.Equal(encodedData1, streamObject.RawData);
    }

    [Fact]
    public void Test_ToStreamObject_MultipleCallsWithDifferentFilters_ReturnsCorrectData()
    {
        // Arrange
        var testData = Encoding.ASCII.GetBytes("Reusable test data");
        using var memoryStream = new MemoryStream();
        var pdfStream = new PdfStream(memoryStream);
        pdfStream.Write(testData, 0, testData.Length);

        // Act
        var streamObject1 = pdfStream.ToStreamObject();
        var streamObject2 = pdfStream.ToStreamObject(new ASCIIHexDecode());
        var streamObject3 = pdfStream.ToStreamObject(new FlateDecode());

        // Assert
        Assert.Equal(testData, streamObject1.RawData);
        Assert.Equal(new ASCIIHexDecode().Encode(testData, null), streamObject2.RawData);
        Assert.Equal(new FlateDecode().Encode(testData, null), streamObject3.RawData);

        // All should have correct filter metadata
        Assert.False(streamObject1.ContainsKey(PdfNames.Filter));
        Assert.Equal(PdfNames.ASCIIHexDecode, streamObject2[PdfNames.Filter]);
        Assert.Equal(PdfNames.FlateDecode, streamObject3[PdfNames.Filter]);
    }

    [Fact]
    public void Test_ToStreamObject_WithThreeFilters_ReturnsStreamObjectWithFilterArrayInCorrectOrder()
    {
        // Arrange
        var originalData = Encoding.ASCII.GetBytes("Test data for triple filter encoding");
        using var memoryStream = new MemoryStream();
        var pdfStream = new PdfStream(memoryStream);
        pdfStream.Write(originalData, 0, originalData.Length);

        var runLengthFilter = new RunLengthDecode();
        var asciiHexFilter = new ASCIIHexDecode();
        var flateFilter = new FlateDecode();

        // Act
        var streamObject = pdfStream.ToStreamObject(runLengthFilter, asciiHexFilter, flateFilter);

        // Assert
        Assert.NotNull(streamObject);
        Assert.True(streamObject.ContainsKey(PdfNames.Filter));

        var filterValue = streamObject[PdfNames.Filter];
        Assert.IsType<PdfArray>(filterValue);
        var filterArray = (PdfArray)filterValue;
        Assert.Equal(3, filterArray.Count);
        Assert.Equal(PdfNames.RunLengthDecode, filterArray[0]);
        Assert.Equal(PdfNames.ASCIIHexDecode, filterArray[1]);
        Assert.Equal(PdfNames.FlateDecode, filterArray[2]);

        // Verify the data was encoded with all filters in order
        var firstEncoded = runLengthFilter.Encode(originalData, null);
        var secondEncoded = asciiHexFilter.Encode(firstEncoded, null);
        var thirdEncoded = flateFilter.Encode(secondEncoded, null);
        Assert.Equal(thirdEncoded.Length, streamObject.Length);
        Assert.Equal(thirdEncoded, streamObject.RawData);
    }

    // Regression tests for issue #87 — the literal-string escape table and the
    // hex-string fallback used for CID-encoded show-text operands.

    [Fact]
    public void WriteStringHex_EmptyArray_WritesEmptyAngleBrackets()
    {
        using var memoryStream = new MemoryStream();
        var pdfStream = new PdfStream(memoryStream);

        pdfStream.WriteStringHex(Array.Empty<byte>());

        Assert.Equal(new byte[] { 0x3C, 0x3E }, memoryStream.ToArray());
    }

    [Fact]
    public void WriteStringHex_GlyphId0x000D_PreservesCarriageReturn()
    {
        // Issue #87: Source Sans Pro capital 'J' has glyph id 13 (0x000D).
        // Previously this was written as a literal string, where the parser
        // would silently normalise 0x0D to 0x0A and the consumer would look
        // up CID 10 instead of 13.
        using var memoryStream = new MemoryStream();
        var pdfStream = new PdfStream(memoryStream);

        pdfStream.WriteStringHex(new byte[] { 0x00, 0x0D });

        Assert.Equal("<000D>", Encoding.ASCII.GetString(memoryStream.ToArray()));
    }

    [Fact]
    public void WriteStringHex_GlyphIdWithCarriageReturnInHighByte_IsPreserved()
    {
        using var memoryStream = new MemoryStream();
        var pdfStream = new PdfStream(memoryStream);

        pdfStream.WriteStringHex(new byte[] { 0x0D, 0x42 });

        Assert.Equal("<0D42>", Encoding.ASCII.GetString(memoryStream.ToArray()));
    }

    [Fact]
    public void WriteStringHex_ConsecutiveCidsForming0D0A_PreservesAlignment()
    {
        // A literal string would collapse the 0D 0A pair into a single 0x0A,
        // shifting alignment for every subsequent 2-byte CID. A hex string
        // round-trips every byte.
        using var memoryStream = new MemoryStream();
        var pdfStream = new PdfStream(memoryStream);

        pdfStream.WriteStringHex(new byte[] { 0x01, 0x0D, 0x0A, 0x02 });

        Assert.Equal("<010D0A02>", Encoding.ASCII.GetString(memoryStream.ToArray()));
    }

    [Fact]
    public void WriteStringHex_BytesThatWouldBeEscapedInLiteral_AreWrittenRaw()
    {
        using var memoryStream = new MemoryStream();
        var pdfStream = new PdfStream(memoryStream);

        pdfStream.WriteStringHex(new byte[] { 0x28, 0x29, 0x5C });

        Assert.Equal("<28295C>", Encoding.ASCII.GetString(memoryStream.ToArray()));
    }

    [Fact]
    public void WriteStringHex_AllBytes_RoundTripExactly()
    {
        var input = new byte[256];
        for (int i = 0; i < 256; i++)
            input[i] = (byte)i;

        using var memoryStream = new MemoryStream();
        var pdfStream = new PdfStream(memoryStream);

        pdfStream.WriteStringHex(input);

        var output = Encoding.ASCII.GetString(memoryStream.ToArray());
        Assert.StartsWith("<", output);
        Assert.EndsWith(">", output);
        Assert.Equal(( input.Length * 2 ) + 2, output.Length);

        // Parse the hex back and verify every byte round-trips.
        var hex = output[1..^1];
        var roundTripped = new byte[input.Length];
        for (int i = 0; i < input.Length; i++)
            roundTripped[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        Assert.Equal(input, roundTripped);
    }

    [Fact]
    public void WriteStringLiteral_String_EscapesCarriageReturn()
    {
        using var memoryStream = new MemoryStream();
        var pdfStream = new PdfStream(memoryStream);

        pdfStream.WriteStringLiteral("a\rb");

        Assert.Equal("(a\\rb)", Encoding.ASCII.GetString(memoryStream.ToArray()));
    }

    [Fact]
    public void WriteStringLiteral_String_EscapesLineFeed()
    {
        using var memoryStream = new MemoryStream();
        var pdfStream = new PdfStream(memoryStream);

        pdfStream.WriteStringLiteral("a\nb");

        Assert.Equal("(a\\nb)", Encoding.ASCII.GetString(memoryStream.ToArray()));
    }

    [Fact]
    public void WriteStringLiteral_String_EscapesTab()
    {
        using var memoryStream = new MemoryStream();
        var pdfStream = new PdfStream(memoryStream);

        pdfStream.WriteStringLiteral("a\tb");

        Assert.Equal("(a\\tb)", Encoding.ASCII.GetString(memoryStream.ToArray()));
    }

    [Fact]
    public void WriteStringLiteral_String_StillEscapesParenthesesAndBackslash()
    {
        using var memoryStream = new MemoryStream();
        var pdfStream = new PdfStream(memoryStream);

        pdfStream.WriteStringLiteral("(a)\\b");

        Assert.Equal("(\\(a\\)\\\\b)", Encoding.ASCII.GetString(memoryStream.ToArray()));
    }

    [Fact]
    public void WriteStringLiteral_Bytes_EscapesControlCharacters()
    {
        using var memoryStream = new MemoryStream();
        var pdfStream = new PdfStream(memoryStream);

        pdfStream.WriteStringLiteral(new byte[] { 0x0D, 0x0A, 0x09, 0x08, 0x0C });

        Assert.Equal("(\\r\\n\\t\\b\\f)", Encoding.ASCII.GetString(memoryStream.ToArray()));
    }

    private class PassThroughFilterStub : IStreamFilter
    {
        public PdfName Name => PdfName.Get("PassThrough");
        public bool PassThrough => true;
        public byte[] Encode(byte[] input, IPdfDictionary? parameters) => input;
    }
}
