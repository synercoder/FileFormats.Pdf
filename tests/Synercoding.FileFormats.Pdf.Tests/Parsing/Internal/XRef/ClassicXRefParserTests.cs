using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Parsing;
using Synercoding.FileFormats.Pdf.Parsing.Internal.XRef;
using Synercoding.FileFormats.Pdf.Primitives;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.Parsing.Internal.XRef;

public class ClassicXRefParserTests
{
    private readonly ClassicXRefParser _parser = new();

    [Theory]
    [InlineData("xref", true)]
    [InlineData("XREF", false)]
    [InlineData("Xref", false)]
    [InlineData("xREF", false)]
    public void Test_CanParse_XRefKeyword_ReturnsExpectedResult(string keyword, bool expectedResult)
    {
        var content = keyword + "\n0 1\n0000000000 65535 f\ntrailer\n<</Size 1>>";
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        var result = _parser.CanParse(provider, 0);

        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void Test_CanParse_NotXRefContent_ReturnsFalse()
    {
        var content = "1 0 obj\n<</Type/XRef>>\nstream\nendstream\nendobj";
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        var result = _parser.CanParse(provider, 0);

        Assert.False(result);
    }

    [Fact]
    public void Test_Parse_SimpleXRefTable_ReturnsCorrectData()
    {
        var content = "xref\n0 2\n0000000000 65535 f \n0000000015 00000 n \ntrailer\n<</Size 2 /Root 1 0 R>>";
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        using var stream = new MemoryStream();
        using var reader = new ObjectReader(stream);

        var (trailer, table) = _parser.Parse(provider, 0, 0, reader);

        Assert.Equal(2, trailer.Size);
        Assert.Equal(2, table.Items.Length);
        
        var freeItem = table.Items.First(x => x.Free);
        Assert.IsType<FreeXRefItem>(freeItem);
        Assert.Equal(0, freeItem.Id.ObjectNumber);
        Assert.Equal(65535, freeItem.Id.Generation);

        var usedItem = table.Items.First(x => !x.Free);
        Assert.IsType<ClassicXRefItem>(usedItem);
        Assert.Equal(1, usedItem.Id.ObjectNumber);
        Assert.Equal(0, usedItem.Id.Generation);
        Assert.Equal(15, ((ClassicXRefItem)usedItem).Offset);
    }

    [Fact]
    public void Test_Parse_MultipleXRefSections_CombinesCorrectly()
    {
        var content = @"xref
0 2
0000000000 65535 f 
0000000015 00000 n 
5 3
0000000100 00000 n 
0000000200 00000 n 
0000000300 00000 n 
trailer
<</Size 8 /Root 1 0 R>>";
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        using var stream = new MemoryStream();
        using var reader = new ObjectReader(stream);

        var (trailer, table) = _parser.Parse(provider, 0, 0, reader);

        Assert.Equal(8, trailer.Size);
        Assert.Equal(5, table.Items.Length); // 0,1 + 5,6,7

        // Check first section
        Assert.True(table.TryGet(new PdfObjectId(0, 65535, true), out var freeItem));
        Assert.True(freeItem.Free);

        Assert.True(table.TryGet(new PdfObjectId(1, 0), out var usedItem1));
        Assert.False(usedItem1.Free);
        Assert.Equal(15, ((ClassicXRefItem)usedItem1).Offset);

        // Check second section
        Assert.True(table.TryGet(new PdfObjectId(5, 0), out var usedItem5));
        Assert.Equal(100, ((ClassicXRefItem)usedItem5).Offset);

        Assert.True(table.TryGet(new PdfObjectId(6, 0), out var usedItem6));
        Assert.Equal(200, ((ClassicXRefItem)usedItem6).Offset);

        Assert.True(table.TryGet(new PdfObjectId(7, 0), out var usedItem7));
        Assert.Equal(300, ((ClassicXRefItem)usedItem7).Offset);
    }

    [Fact]
    public void Test_Parse_TrailerWithPrev_ReturnsPrevValue()
    {
        var content = "xref\n0 1\n0000000000 65535 f \ntrailer\n<</Size 1 /Root 1 0 R /Prev 12345>>";
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        using var stream = new MemoryStream();
        using var reader = new ObjectReader(stream);

        var (trailer, table) = _parser.Parse(provider, 0, 0, reader);

        Assert.Equal(12345, trailer.Prev);
    }

    [Fact]
    public void Test_Parse_WithPdfStartOffset_HandlesCorrectly()
    {
        var prefix = "some data before pdf";
        var content = prefix + "xref\n0 1\n0000000000 65535 f \ntrailer\n<</Size 1 /Root 1 0 R>>";
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        using var stream = new MemoryStream();
        using var reader = new ObjectReader(stream);

        var pdfStart = prefix.Length;
        var (trailer, table) = _parser.Parse(provider, pdfStart, 0, reader);

        Assert.Equal(1, trailer.Size);
        Assert.Single(table.Items);
    }

    [Theory]
    [InlineData("0000000015 00000 n")]  // Standard format
    [InlineData("0000000015 00000 n ")] // Trailing space
    [InlineData("0000000015 00000 n\n")] // Trailing newline
    [InlineData("0000000015 00000 n\r\n")] // Windows line ending
    public void Test_Parse_DifferentLineEndings_HandlesCorrectly(string xrefEntry)
    {
        var content = $"xref\n0 1\n0000000000 65535 f \n1 1\n{xrefEntry}trailer\n<</Size 2 /Root 1 0 R>>";
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        using var stream = new MemoryStream();
        using var reader = new ObjectReader(stream);

        var (trailer, table) = _parser.Parse(provider, 0, 0, reader);

        Assert.Equal(2, trailer.Size);
        Assert.Equal(2, table.Items.Length);
        
        var usedItem = table.Items.First(x => !x.Free);
        Assert.Equal(15, ((ClassicXRefItem)usedItem).Offset);
    }

    [Fact]
    public void Test_Parse_FreeAndInUseEntries_ParsesBothCorrectly()
    {
        var content = @"xref
0 4
0000000000 65535 f 
0000000015 00000 n 
0000000000 00001 f 
0000000030 00000 n 
trailer
<</Size 4 /Root 1 0 R>>";
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        using var stream = new MemoryStream();
        using var reader = new ObjectReader(stream);

        var (trailer, table) = _parser.Parse(provider, 0, 0, reader);

        Assert.Equal(4, table.Items.Length);

        // Check free entries
        var freeItems = table.Items.Where(x => x.Free).ToArray();
        Assert.Equal(2, freeItems.Length);

        // Check in-use entries
        var usedItems = table.Items.Where(x => !x.Free).ToArray();
        Assert.Equal(2, usedItems.Length);
        Assert.Contains(usedItems, item => ((ClassicXRefItem)item).Offset == 15);
        Assert.Contains(usedItems, item => ((ClassicXRefItem)item).Offset == 30);
    }

    [Fact]
    public void Test_CanParse_PreservesProviderPosition()
    {
        var content = "xref\n0 1\n0000000000 65535 f \ntrailer\n<</Size 1>>";
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);
        
        provider.Seek(10, SeekOrigin.Begin); // Set initial position
        var initialPosition = provider.Position;

        _parser.CanParse(provider, 0);

        Assert.Equal(initialPosition, provider.Position);
    }
}
