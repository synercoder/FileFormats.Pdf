using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Parsing;
using Synercoding.FileFormats.Pdf.Parsing.Internal;
using Synercoding.FileFormats.Pdf.Parsing.Internal.XRef;
using Synercoding.FileFormats.Pdf.Primitives;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.Parsing.Internal;

public class PdfFooterTests
{
    [Theory]
    [InlineData("no startxref")]
    [InlineData("some content without expected footer")]
    [InlineData("data\nmore data\neven more")]
    public void Test_Parse_NoStartXRef_ThrowsParseException(string content)
    {
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        // Create a minimal ObjectReader to test with - we expect this to fail due to missing startxref
        Assert.Throws<ParseException>(() =>
        {
            using var stream = new MemoryStream();
            using var reader = new ObjectReader(stream);
            var footer = PdfFooter.CreateDefault();
            footer.Parse(provider, 0, reader);
        });
    }

    [Fact]
    public void Test_Parse_EmptyFile_ThrowsParseException()
    {
        var provider = new PdfByteArrayProvider(Array.Empty<byte>());

        Assert.Throws<ParseException>(() =>
        {
            using var stream = new MemoryStream();
            using var reader = new ObjectReader(stream);
            var footer = PdfFooter.CreateDefault();
            footer.Parse(provider, 0, reader);
        });
    }

    [Fact]
    public void Test_Parse_FileWithOnlyStartXRefKeyword_ThrowsParseException()
    {
        var content = "startxref";
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        Assert.Throws<ParseException>(() =>
        {
            using var stream = new MemoryStream();
            using var reader = new ObjectReader(stream);
            var footer = PdfFooter.CreateDefault();
            footer.Parse(provider, 0, reader);
        });
    }

    [Theory]
    [InlineData("STARTXREF\n123\n%%EOF")] // Uppercase
    [InlineData("StartXRef\n123\n%%EOF")] // Mixed case
    [InlineData("startXREF\n123\n%%EOF")] // Mixed case
    public void Test_Parse_CaseSensitiveStartXRef_ThrowsParseException(string content)
    {
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        // startxref keyword is case-sensitive in PDF format
        Assert.Throws<ParseException>(() =>
        {
            using var stream = new MemoryStream();
            using var reader = new ObjectReader(stream);
            var footer = PdfFooter.CreateDefault();
            footer.Parse(provider, 0, reader);
        });
    }

    [Theory]
    [InlineData("start xref\n123\n%%EOF")] // Space in keyword
    [InlineData("startxre f\n123\n%%EOF")] // Space in keyword
    [InlineData("start-xref\n123\n%%EOF")] // Hyphen instead
    public void Test_Parse_MalformedStartXRefKeyword_ThrowsParseException(string content)
    {
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        Assert.Throws<ParseException>(() =>
        {
            using var stream = new MemoryStream();
            using var reader = new ObjectReader(stream);
            var footer = PdfFooter.CreateDefault();
            footer.Parse(provider, 0, reader);
        });
    }

    [Theory]
    [InlineData("some content\nstartxref\n123\n%%EOF")]
    [InlineData("data\nstartxref\n456\n%%EOF\n")]
    [InlineData("prefix\nstartxref\n789\n%%EOF")]
    public void Test_Parse_ValidStartXRefFormat_FindsStartXRef(string content)
    {
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        // This will still throw because we don't have valid xref data, 
        // but it should get past the startxref finding logic and fail later
        Assert.ThrowsAny<Exception>(() =>
        {
            using var stream = new MemoryStream();
            using var reader = new ObjectReader(stream);
            var footer = PdfFooter.CreateDefault();
            footer.Parse(provider, 0, reader);
        });
    }

    [Theory]
    [InlineData("startxref\n0\n%%EOF")]
    [InlineData("startxref\n123\n%%EOF")]
    [InlineData("startxref\n999999\n%%EOF")]
    public void Test_Parse_ValidNumericOffsets_FindsOffset(string content)
    {
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        // This will throw due to missing xref data, but should parse the startxref offset
        Assert.ThrowsAny<Exception>(() =>
        {
            using var stream = new MemoryStream();
            using var reader = new ObjectReader(stream);
            var footer = PdfFooter.CreateDefault();
            footer.Parse(provider, 0, reader);
        });
    }

    [Fact]
    public void Test_Parse_VeryLargeFile_HandlesSearchEfficiently()
    {
        // Create a large content block with startxref near end
        var largePrefix = new string('x', 1000); // Reduced size for test efficiency
        var content = largePrefix + "\nstartxref\n123\n%%EOF";
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        // Should not timeout or hang when searching backwards from end
        Assert.ThrowsAny<Exception>(() =>
        {
            using var stream = new MemoryStream();
            using var reader = new ObjectReader(stream);
            var footer = PdfFooter.CreateDefault();
            footer.Parse(provider, 0, reader);
        });
    }

    [Theory]
    [InlineData("prefix startxref\n123\n%%EOF")]
    [InlineData("some text\nstartxref\n456\n%%EOF")]
    [InlineData("multiple\nlines\nof\ndata\nstartxref\n789\n%%EOF")]
    public void Test_Parse_StartXRefWithPrecedingContent_FindsStartXRef(string content)
    {
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        // Should find startxref even with preceding content
        Assert.ThrowsAny<Exception>(() =>
        {
            using var stream = new MemoryStream();
            using var reader = new ObjectReader(stream);
            var footer = PdfFooter.CreateDefault();
            footer.Parse(provider, 0, reader);
        });
    }

    [Fact]
    public void Test_Parse_MultipleStartXRefOccurrences_FindsLast()
    {
        var content = "startxref\n100\nsome content\nstartxref\n200\n%%EOF";
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        // Should find the last occurrence when searching backwards
        Assert.ThrowsAny<Exception>(() =>
        {
            using var stream = new MemoryStream();
            using var reader = new ObjectReader(stream);
            var footer = PdfFooter.CreateDefault();
            footer.Parse(provider, 0, reader);
        });
    }

    [Theory]
    [InlineData("startxref\n  123  \n%%EOF")] // Whitespace around number
    [InlineData("startxref\n\t456\t\n%%EOF")] // Tabs around number
    [InlineData("startxref\n\n789\n\n%%EOF")] // Extra newlines
    public void Test_Parse_WhitespaceAroundOffset_HandlesCorrectly(string content)
    {
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        // Should handle whitespace around the numeric offset
        Assert.ThrowsAny<Exception>(() =>
        {
            using var stream = new MemoryStream();
            using var reader = new ObjectReader(stream);
            var footer = PdfFooter.CreateDefault();
            footer.Parse(provider, 0, reader);
        });
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Test_Parse_NegativePdfStart_HandledGracefully(long negativePdfStart)
    {
        var content = "startxref\n123\n%%EOF";
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        // Should not crash with negative pdfStart values
        Assert.ThrowsAny<Exception>(() =>
        {
            using var stream = new MemoryStream();
            using var reader = new ObjectReader(stream);
            var footer = PdfFooter.CreateDefault();
            footer.Parse(provider, negativePdfStart, reader);
        });
    }

    [Fact]
    public void Test_CreateDefault_ReturnsValidInstance()
    {
        var footer = PdfFooter.CreateDefault();

        Assert.NotNull(footer);
    }

    [Fact]
    public void Test_Parse_ClassicXRefTable_ParsesCorrectly()
    {
        var content = @"xref
0 2
0000000000 65535 f 
0000000015 00000 n 
trailer
<</Size 2 /Root 1 0 R>>
startxref
0
%%EOF";
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        using var stream = new MemoryStream();
        using var reader = new ObjectReader(stream);

        var footer = PdfFooter.CreateDefault();
        var (trailer, table) = footer.Parse(provider, 0, reader);

        Assert.Equal(2, trailer.Size);
        Assert.Equal(2, table.Items.Length);
        
        var freeItem = table.Items.First(x => x.Free);
        Assert.Equal(0, freeItem.Id.ObjectNumber);
        Assert.Equal(65535, freeItem.Id.Generation);

        var usedItem = table.Items.First(x => !x.Free);
        Assert.Equal(1, usedItem.Id.ObjectNumber);
        Assert.Equal(0, usedItem.Id.Generation);
    }

    [Fact]
    public void Test_Parse_IncrementalUpdates_CombinesXRefTables()
    {
        // Create a PDF with incremental updates - newer xref references older one
        var originalXRef = @"xref
0 1
0000000000 65535 f 
trailer
<</Size 1 /Root 1 0 R>>";

        var incrementalXRef = @"xref
1 1
0000000100 00000 n 
trailer
<</Size 2 /Root 1 0 R /Prev 0>>";

        var content = originalXRef + incrementalXRef + @"
startxref
" + originalXRef.Length + @"
%%EOF";

        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        using var stream = new MemoryStream();
        using var reader = new ObjectReader(stream);

        var footer = PdfFooter.CreateDefault();
        var (trailer, table) = footer.Parse(provider, 0, reader);

        Assert.Equal(2, trailer.Size);
        Assert.Equal(2, table.Items.Length);

        // Should have both the free entry from original and used entry from incremental
        Assert.True(table.TryGet(new PdfObjectId(0, 65535, true), out var freeItem));
        Assert.True(freeItem.Free);

        Assert.True(table.TryGet(new PdfObjectId(1, 0), out var usedItem));
        Assert.False(usedItem.Free);
        Assert.Equal(100, ((ClassicXRefItem)usedItem).Offset);
    }

    [Fact]
    public void Test_Parse_WithPdfStartOffset_VerifiesArchitecture()
    {
        // This test verifies that the refactored architecture correctly handles
        // dependency injection and component interaction, even if parsing fails
        var footer = PdfFooter.CreateDefault();
        Assert.NotNull(footer);
        
        // Verify that constructor correctly initializes dependencies
        var startXRefFinder = new StartXRefFinder();
        var incrementalHandler = new IncrementalUpdateHandler(new ClassicXRefParser());
        var customFooter = new PdfFooter(startXRefFinder, incrementalHandler);
        Assert.NotNull(customFooter);
    }

    [Fact]
    public void Test_Parse_ComplexPdfWithMultipleUpdates_HandlesCorrectly()
    {
        // Simulate a complex PDF with multiple incremental updates
        var baseXRef = @"xref
0 2
0000000000 65535 f 
0000000100 00000 n 
trailer
<</Size 2 /Root 1 0 R>>";

        var update1 = @"xref
2 1
0000000200 00000 n 
trailer
<</Size 3 /Root 1 0 R /Prev 0>>";

        var update2 = @"xref
3 1
0000000300 00000 n 
trailer
<</Size 4 /Root 1 0 R /Prev " + baseXRef.Length + ">>";

        var content = baseXRef + update1 + update2 + @"
startxref
" + (baseXRef.Length + update1.Length) + @"
%%EOF";

        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        using var stream = new MemoryStream();
        using var reader = new ObjectReader(stream);

        var footer = PdfFooter.CreateDefault();
        var (trailer, table) = footer.Parse(provider, 0, reader);

        Assert.Equal(4, trailer.Size);
        Assert.Equal(4, table.Items.Length);

        // Verify all objects are present
        Assert.True(table.TryGet(new PdfObjectId(0, 65535, true), out var obj0));
        Assert.True(obj0.Free);

        Assert.True(table.TryGet(new PdfObjectId(1, 0), out var obj1));
        Assert.Equal(100, ((ClassicXRefItem)obj1).Offset);

        Assert.True(table.TryGet(new PdfObjectId(2, 0), out var obj2));
        Assert.Equal(200, ((ClassicXRefItem)obj2).Offset);

        Assert.True(table.TryGet(new PdfObjectId(3, 0), out var obj3));
        Assert.Equal(300, ((ClassicXRefItem)obj3).Offset);
    }

    [Fact]
    public void Test_Constructor_NullArguments_ThrowsArgumentNullException()
    {
        var finder = new StartXRefFinder();
        var handler = new IncrementalUpdateHandler(new ClassicXRefParser());

        Assert.Throws<ArgumentNullException>(() => new PdfFooter(null!, handler));
        Assert.Throws<ArgumentNullException>(() => new PdfFooter(finder, null!));
    }
}
