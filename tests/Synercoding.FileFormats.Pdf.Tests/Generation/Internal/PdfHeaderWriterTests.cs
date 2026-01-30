using Synercoding.FileFormats.Pdf.Generation;
using Synercoding.FileFormats.Pdf.Generation.Internal;
using Synercoding.FileFormats.Pdf.IO;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.Generation.Internal;

public class PdfHeaderWriterTests
{
    [Theory]
    [InlineData(1, 7)]
    [InlineData(1, 0)]
    [InlineData(1, 4)]
    [InlineData(2, 0)]
    public void Test_WriteTo_WritesCorrectHeader(byte major, byte minor)
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        var pdfStream = new PdfStream(memoryStream);
        var pdfVersion = new PdfVersion(major, minor);

        // Act
        var position = PdfHeaderWriter.WriteTo(pdfStream, pdfVersion);

        // Assert
        Assert.Equal(0, position); // Should return start position

        memoryStream.Position = 0;
        var bytes = memoryStream.ToArray();
        var content = Encoding.ASCII.GetString(bytes);

        // Check PDF header
        Assert.StartsWith($"%PDF-{major}.{minor}", content);

        // Check for newline after version
        var versionEndIndex = $"%PDF-{major}.{minor}".Length;
        Assert.Equal('\r', (char)bytes[versionEndIndex]);
        Assert.Equal('\n', (char)bytes[versionEndIndex + 1]);

        // Check for binary comment marker
        Assert.Equal(ByteUtils.PERCENT_SIGN, bytes[versionEndIndex + 2]);

        // Check for 4 high bytes (>= 128)
        Assert.Equal(0x81, bytes[versionEndIndex + 3]);
        Assert.Equal(0x82, bytes[versionEndIndex + 4]);
        Assert.Equal(0x83, bytes[versionEndIndex + 5]);
        Assert.Equal(0x84, bytes[versionEndIndex + 6]);

        // Check for final newline
        Assert.Equal('\r', (char)bytes[versionEndIndex + 7]);
        Assert.Equal('\n', (char)bytes[versionEndIndex + 8]);
    }

    [Fact]
    public void Test_WriteTo_ReturnsStreamPosition()
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        var pdfStream = new PdfStream(memoryStream);
        var pdfVersion = new PdfVersion(1, 7);

        // Write some data first to offset position
        pdfStream.Write("SOME HEADER DATA");
        var expectedPosition = pdfStream.Position;

        // Act
        var returnedPosition = PdfHeaderWriter.WriteTo(pdfStream, pdfVersion);

        // Assert
        Assert.Equal(expectedPosition, returnedPosition);
    }

    [Fact]
    public void Test_WriteTo_ThrowsOnNullStream()
    {
        // Arrange
        PdfStream? pdfStream = null;
        var pdfVersion = new PdfVersion(1, 7);

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => PdfHeaderWriter.WriteTo(pdfStream!, pdfVersion));
        Assert.Equal("stream", exception.ParamName);
    }

    [Fact]
    public void Test_WriteTo_ThrowsOnNullVersion()
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        var pdfStream = new PdfStream(memoryStream);
        PdfVersion? pdfVersion = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => PdfHeaderWriter.WriteTo(pdfStream, pdfVersion!));
        Assert.Equal("pdfVersion", exception.ParamName);
    }

    [Fact]
    public void Test_WriteTo_CorrectByteLength()
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        var pdfStream = new PdfStream(memoryStream);
        var pdfVersion = new PdfVersion(1, 7);

        // Act
        PdfHeaderWriter.WriteTo(pdfStream, pdfVersion);

        // Assert
        var bytes = memoryStream.ToArray();
        // Expected format: %PDF-1.7\r\n%[4 binary bytes]\r\n
        // That's: 8 chars for "%PDF-1.7" + 2 for "\r\n" + 1 for "%" + 4 binary bytes + 2 for "\r\n" = 17 bytes
        Assert.Equal(17, bytes.Length);
    }

    [Fact]
    public void Test_WriteTo_BinaryBytesAreHighValues()
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        var pdfStream = new PdfStream(memoryStream);
        var pdfVersion = new PdfVersion(1, 7);

        // Act
        PdfHeaderWriter.WriteTo(pdfStream, pdfVersion);

        // Assert
        var bytes = memoryStream.ToArray();
        // Binary bytes start after "%PDF-1.7\r\n%"
        var binaryStartIndex = 11;

        // All 4 binary bytes should be >= 128 (high bit set)
        for (int i = 0; i < 4; i++)
        {
            Assert.True(bytes[binaryStartIndex + i] >= 128,
                $"Binary byte at index {i} should be >= 128, but was {bytes[binaryStartIndex + i]}");
        }
    }

    [Fact]
    public void Test_WriteTo_MultipleCallsAppendCorrectly()
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        var pdfStream = new PdfStream(memoryStream);
        var pdfVersion1 = new PdfVersion(1, 4);
        var pdfVersion2 = new PdfVersion(2, 0);

        // Act
        var pos1 = PdfHeaderWriter.WriteTo(pdfStream, pdfVersion1);
        var pos2 = PdfHeaderWriter.WriteTo(pdfStream, pdfVersion2);

        // Assert
        Assert.Equal(0, pos1);
        Assert.Equal(17, pos2); // Second header starts after first (17 bytes)

        var bytes = memoryStream.ToArray();
        Assert.Equal(34, bytes.Length); // Two headers of 17 bytes each

        // Check both headers are present
        var content = Encoding.ASCII.GetString(bytes);
        Assert.Contains("%PDF-1.4", content);
        Assert.Contains("%PDF-2.0", content);
    }

    [Fact]
    public void Test_WriteTo_PreservesStreamPosition()
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        var pdfStream = new PdfStream(memoryStream);
        var pdfVersion = new PdfVersion(1, 7);

        // Act
        PdfHeaderWriter.WriteTo(pdfStream, pdfVersion);
        var finalPosition = pdfStream.Position;

        // Assert
        Assert.Equal(17, finalPosition); // Stream position should be at end of written data
    }

    [Fact]
    public void Test_WriteTo_WritesLineEndingsCorrectly()
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        var pdfStream = new PdfStream(memoryStream);
        var pdfVersion = new PdfVersion(1, 5);

        // Act
        PdfHeaderWriter.WriteTo(pdfStream, pdfVersion);

        // Assert
        var bytes = memoryStream.ToArray();

        // First line ending after version "%PDF-1.5"
        var firstCR = 8;  // Index after "%PDF-1.5"
        var firstLF = 9;
        Assert.Equal(0x0D, bytes[firstCR]); // \r
        Assert.Equal(0x0A, bytes[firstLF]); // \n

        // Second line ending after binary comment (after % + 4 binary bytes)
        var secondCR = 15; // After the 4 binary bytes (index 10 = %, 11-14 = binary, 15 = \r)
        var secondLF = 16;
        Assert.Equal(0x0D, bytes[secondCR]); // \r
        Assert.Equal(0x0A, bytes[secondLF]); // \n
    }
}
