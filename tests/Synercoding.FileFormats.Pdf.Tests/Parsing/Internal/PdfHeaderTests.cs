using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Parsing.Internal;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.Parsing.Internal;

public class PdfHeaderTests
{
    [Theory]
    [InlineData("%PDF-1.4", 1, 4)]
    [InlineData("%PDF-1.7", 1, 7)]
    [InlineData("%PDF-2.0", 2, 0)]
    [InlineData("%PDF-1.0", 1, 0)]
    [InlineData("%PDF-1.1", 1, 1)]
    [InlineData("%PDF-1.2", 1, 2)]
    [InlineData("%PDF-1.3", 1, 3)]
    [InlineData("%PDF-1.5", 1, 5)]
    [InlineData("%PDF-1.6", 1, 6)]
    public void Test_Parse_For_CorrectVersion(string header, byte expectedMajor, byte expectedMinor)
    {
        var headerBytes = Encoding.ASCII.GetBytes(header);
        var provider = new PdfByteArrayProvider(headerBytes);

        var result = PdfHeader.Parse(provider);

        Assert.Equal(expectedMajor, result.Version.Major);
        Assert.Equal(expectedMinor, result.Version.Minor);
        Assert.Equal(0, result.PdfStart);
    }

    [Theory]
    [InlineData("junk%PDF-1.4", 1, 4, 4)]
    [InlineData("garbage data%PDF-2.0", 2, 0, 12)]
    [InlineData("   %PDF-1.7", 1, 7, 3)]
    [InlineData("binary\x00\x01\x02%PDF-1.5", 1, 5, 9)]
    [InlineData("some text before %PDF-1.3 footer", 1, 3, 17)]
    public void Test_Parse_HeaderWithPrecedingData_ReturnsCorrectVersionAndOffset(string input, byte expectedMajor, byte expectedMinor, long expectedOffset)
    {
        var inputBytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(inputBytes);

        var result = PdfHeader.Parse(provider);

        Assert.Equal(expectedMajor, result.Version.Major);
        Assert.Equal(expectedMinor, result.Version.Minor);
        Assert.Equal(expectedOffset, result.PdfStart);
    }

    [Theory]
    [InlineData("%PDF-1")]
    [InlineData("%PDF-1.")]
    [InlineData("%PDF-")]
    public void Test_Parse_IncompleteHeader_ThrowsUnexpectedEndOfFileException(string incompleteHeader)
    {
        var headerBytes = Encoding.ASCII.GetBytes(incompleteHeader);
        var provider = new PdfByteArrayProvider(headerBytes);

        Assert.Throws<UnexpectedEndOfFileException>(() => PdfHeader.Parse(provider));
    }

    [Theory]
    [InlineData("%PD")]
    [InlineData("%")]
    [InlineData("")]
    public void Test_Parse_NoValidHeader_ThrowsParseException(string incompleteHeader)
    {
        var headerBytes = Encoding.ASCII.GetBytes(incompleteHeader);
        var provider = new PdfByteArrayProvider(headerBytes);

        Assert.Throws<ParseException>(() => PdfHeader.Parse(provider));
    }

    [Theory]
    [InlineData("%PDF-1X4")]
    [InlineData("%PDF-1-4")]
    [InlineData("%PDF-1+4")]
    [InlineData("%PDF-1 4")]
    public void Test_Parse_InvalidDotSeparator_ThrowsUnexpectedByteException(string invalidHeader)
    {
        var headerBytes = Encoding.ASCII.GetBytes(invalidHeader);
        var provider = new PdfByteArrayProvider(headerBytes);

        Assert.Throws<UnexpectedByteException>(() => PdfHeader.Parse(provider));
    }

    [Theory]
    [InlineData("nopdf")]
    [InlineData("PDF-1.4")]
    [InlineData("some text without header")]
    [InlineData("text%PD-1.4")]
    [InlineData("text%PDF1.4")]
    [InlineData("text%PdF-1.4")]
    public void Test_Parse_NoValidHeaderInContent_ThrowsParseException(string noHeaderData)
    {
        var inputBytes = Encoding.ASCII.GetBytes(noHeaderData);
        var provider = new PdfByteArrayProvider(inputBytes);

        Assert.Throws<ParseException>(() => PdfHeader.Parse(provider));
    }

    [Theory]
    [InlineData("%PDF-\x39.4", 9, 4)] // ASCII '9' = 0x39
    [InlineData("%PDF-\x30.0", 0, 0)] // ASCII '0' = 0x30
    [InlineData("%PDF-\x35.7", 5, 7)] // ASCII '5' = 0x35
    public void Test_Parse_NumericVersionBytes_ParsedCorrectly(string header, byte expectedMajor, byte expectedMinor)
    {
        var headerBytes = Encoding.ASCII.GetBytes(header);
        var provider = new PdfByteArrayProvider(headerBytes);

        var result = PdfHeader.Parse(provider);

        Assert.Equal(expectedMajor, result.Version.Major);
        Assert.Equal(expectedMinor, result.Version.Minor);
    }

    [Fact]
    public void Test_Parse_MultiplePercentSigns_FindsCorrectHeader()
    {
        var input = "% comment\n% another comment\n%PDF-1.4";
        var inputBytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(inputBytes);

        var result = PdfHeader.Parse(provider);

        Assert.Equal((byte)1, result.Version.Major);
        Assert.Equal((byte)4, result.Version.Minor);
        Assert.Equal(28, result.PdfStart); // Position of the final %
    }

    [Fact]
    public void Test_Parse_HeaderAtEndOfFile_ParsesCorrectly()
    {
        var input = "lots of data before the header %PDF-1.7";
        var inputBytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(inputBytes);

        var result = PdfHeader.Parse(provider);

        Assert.Equal((byte)1, result.Version.Major);
        Assert.Equal((byte)7, result.Version.Minor);
        Assert.Equal(31, result.PdfStart);
    }

    [Theory]
    [InlineData("%PDF-1.4\nrest of pdf")]
    [InlineData("%PDF-2.0\r\nmore content")]
    [InlineData("%PDF-1.7 followed by space")]
    [InlineData("%PDF-1.5\tand tab")]
    public void Test_Parse_HeaderWithFollowingContent_ParsesCorrectly(string input)
    {
        var inputBytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(inputBytes);

        var result = PdfHeader.Parse(provider);

        Assert.NotNull(result);
        Assert.NotNull(result.Version);
        Assert.True(result.PdfStart >= 0);
    }

    [Theory]
    [InlineData("%PDF-1.4", 8)] // Position after parsing header
    [InlineData("data%PDF-2.0", 12)] // Position after parsing header with preceding data
    public void Test_Parse_UpdatesProviderPosition(string input, long expectedPosition)
    {
        var inputBytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(inputBytes);

        PdfHeader.Parse(provider);

        Assert.Equal(expectedPosition, provider.Position);
    }

    [Fact]
    public void Test_Parse_EmptyFile_ThrowsParseException()
    {
        var provider = new PdfByteArrayProvider(Array.Empty<byte>());

        Assert.Throws<ParseException>(() => PdfHeader.Parse(provider));
    }

    [Theory]
    [InlineData("%PDF-1.4%PDF-1.7", 1, 4, 0)] // Should find first valid header
    public void Test_Parse_MultipleHeaders_FindsFirst(string input, byte expectedMajor, byte expectedMinor, long expectedOffset)
    {
        var inputBytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(inputBytes);

        var result = PdfHeader.Parse(provider);

        Assert.Equal(expectedMajor, result.Version.Major);
        Assert.Equal(expectedMinor, result.Version.Minor);
        Assert.Equal(expectedOffset, result.PdfStart);
    }

    [Theory]
    [InlineData("%PDF-1.4", typeof(PdfVersion))]
    public void Test_Parse_ReturnsCorrectTypes(string header, Type expectedVersionType)
    {
        var headerBytes = Encoding.ASCII.GetBytes(header);
        var provider = new PdfByteArrayProvider(headerBytes);

        var result = PdfHeader.Parse(provider);

        Assert.IsType<PdfHeader>(result);
        Assert.IsType(expectedVersionType, result.Version);
    }

    [Theory]
    [InlineData("%PDF-1.4binary\x00\x01\x02\x03data", 1, 4)] // Binary data after header
    [InlineData("%PDF-2.0\xFF\xFE\xFD\xFC", 2, 0)] // High byte values after header
    public void Test_Parse_WithBinaryDataAfterHeader_ParsesCorrectly(string input, byte expectedMajor, byte expectedMinor)
    {
        var inputBytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(inputBytes);

        var result = PdfHeader.Parse(provider);

        Assert.Equal(expectedMajor, result.Version.Major);
        Assert.Equal(expectedMinor, result.Version.Minor);
    }

    [Fact]
    public void Test_Parse_LargeFileWithHeaderAtStart_PerformsWell()
    {
        // Create a large file with header at start
        var headerBytes = Encoding.ASCII.GetBytes("%PDF-1.4");
        var largeData = new byte[10000];
        var combined = headerBytes.Concat(largeData).ToArray();
        var provider = new PdfByteArrayProvider(combined);

        var result = PdfHeader.Parse(provider);

        Assert.Equal((byte)1, result.Version.Major);
        Assert.Equal((byte)4, result.Version.Minor);
        Assert.Equal(0, result.PdfStart);
    }

    [Fact]
    public void Test_Parse_LargeFileWithHeaderInMiddle_PerformsWell()
    {
        // Create a large file with header in the middle
        var prefix = new byte[5000];
        var headerBytes = Encoding.ASCII.GetBytes("%PDF-1.7");
        var suffix = new byte[5000];
        var combined = prefix.Concat(headerBytes).Concat(suffix).ToArray();
        var provider = new PdfByteArrayProvider(combined);

        var result = PdfHeader.Parse(provider);

        Assert.Equal((byte)1, result.Version.Major);
        Assert.Equal((byte)7, result.Version.Minor);
        Assert.Equal(5000, result.PdfStart);
    }
}
