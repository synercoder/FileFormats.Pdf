using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Parsing.Internal.XRef;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.Parsing.Internal.XRef;

public class StartXRefFinderTests
{
    private readonly StartXRefFinder _finder = new();

    [Theory]
    [InlineData("startxref\n123\n%%EOF", 123)]
    [InlineData("startxref\n456\n%%EOF", 456)]
    [InlineData("startxref\n0\n%%EOF", 0)]
    [InlineData("startxref\n999999\n%%EOF", 999999)]
    public void Test_FindStartXRef_ValidFormat_ReturnsCorrectOffset(string content, long expectedOffset)
    {
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        var result = _finder.FindStartXRef(provider, 0);

        Assert.Equal(expectedOffset, result);
    }

    [Theory]
    [InlineData("prefix content\nstartxref\n789\n%%EOF", 789)]
    [InlineData("lots of data before\nstartxref\n123\n%%EOF", 123)]
    [InlineData("multiple\nlines\nof\ndata\nstartxref\n456\n%%EOF", 456)]
    public void Test_FindStartXRef_WithPrecedingContent_FindsCorrectOffset(string content, long expectedOffset)
    {
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        var result = _finder.FindStartXRef(provider, 0);

        Assert.Equal(expectedOffset, result);
    }

    [Fact]
    public void Test_FindStartXRef_MultipleStartXRef_FindsLast()
    {
        var content = "startxref\n100\nsome content\nstartxref\n200\n%%EOF";
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        var result = _finder.FindStartXRef(provider, 0);

        Assert.Equal(200, result);
    }

    [Theory]
    [InlineData("startxref\n  123  \n%%EOF", 123)] // Whitespace around number
    [InlineData("startxref\n\t456\t\n%%EOF", 456)] // Tabs around number
    [InlineData("startxref\n\n789\n\n%%EOF", 789)] // Extra newlines
    public void Test_FindStartXRef_WhitespaceAroundOffset_ParsesCorrectly(string content, long expectedOffset)
    {
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        var result = _finder.FindStartXRef(provider, 0);

        Assert.Equal(expectedOffset, result);
    }

    [Theory]
    [InlineData("no startxref")]
    [InlineData("some content without expected footer")]
    [InlineData("data\nmore data\neven more")]
    [InlineData("")]
    public void Test_FindStartXRef_NoStartXRef_ThrowsParseException(string content)
    {
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        Assert.Throws<ParseException>(() => _finder.FindStartXRef(provider, 0));
    }

    [Theory]
    [InlineData("STARTXREF\n123\n%%EOF")] // Uppercase
    [InlineData("StartXRef\n123\n%%EOF")] // Mixed case
    [InlineData("startXREF\n123\n%%EOF")] // Mixed case  
    public void Test_FindStartXRef_CaseSensitive_ThrowsParseException(string content)
    {
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        Assert.Throws<ParseException>(() => _finder.FindStartXRef(provider, 0));
    }

    [Theory]
    [InlineData("start xref\n123\n%%EOF")] // Space in keyword
    [InlineData("startxre f\n123\n%%EOF")] // Space in keyword
    [InlineData("start-xref\n123\n%%EOF")] // Hyphen instead
    public void Test_FindStartXRef_MalformedKeyword_ThrowsParseException(string content)
    {
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        Assert.Throws<ParseException>(() => _finder.FindStartXRef(provider, 0));
    }

    [Fact]
    public void Test_FindStartXRef_OnlyKeyword_ThrowsParseException()
    {
        var content = "startxref";
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        Assert.Throws<ParseException>(() => _finder.FindStartXRef(provider, 0));
    }

    [Fact]
    public void Test_FindStartXRef_LargeFile_HandlesEfficiently()
    {
        var largePrefix = new string('x', 1000);
        var content = largePrefix + "\nstartxref\n123\n%%EOF";
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        var result = _finder.FindStartXRef(provider, 0);

        Assert.Equal(123, result);
    }

    [Theory]
    [InlineData(100)] // PdfStart after content
    [InlineData(50)]  // PdfStart in middle
    public void Test_FindStartXRef_WithPdfStart_RespectsStartPosition(long pdfStart)
    {
        var content = "startxref\n100\n%%EOF" + new string('x', (int)pdfStart) + "startxref\n200\n%%EOF";
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        var result = _finder.FindStartXRef(provider, pdfStart);

        Assert.Equal(200, result);
    }

    [Fact]
    public void Test_FindStartXRef_PdfStartBeyondStartXRef_ThrowsParseException()
    {
        var content = "startxref\n123\n%%EOF";
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        // PdfStart beyond the startxref position
        Assert.Throws<ParseException>(() => _finder.FindStartXRef(provider, 100));
    }
}
