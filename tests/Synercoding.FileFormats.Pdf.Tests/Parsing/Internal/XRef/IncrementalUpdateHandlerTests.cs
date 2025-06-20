using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Parsing;
using Synercoding.FileFormats.Pdf.Parsing.Internal;
using Synercoding.FileFormats.Pdf.Parsing.Internal.XRef;
using Synercoding.FileFormats.Pdf.Primitives;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.Parsing.Internal.XRef;

public class IncrementalUpdateHandlerTests
{
    [Fact]
    public void Test_ProcessIncrementalUpdates_SingleXRef_ReturnsDirectly()
    {
        var mockParser = new MockXRefParser(canParse: true);
        var handler = new IncrementalUpdateHandler(mockParser);

        var content = "xref\n0 1\n0000000000 65535 f \ntrailer\n<</Size 1 /Root 1 0 R>>";
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        using var stream = new MemoryStream();
        using var reader = new ObjectReader(stream);

        var (trailer, table) = handler.ProcessIncrementalUpdates(provider, 0, 0, reader);

        Assert.Equal(1, mockParser.ParseCallCount);
        Assert.NotNull(trailer);
        Assert.NotNull(table);
    }

    [Fact]
    public void Test_ProcessIncrementalUpdates_MultipleXRefs_FollowsPrevChain()
    {
        var mockParser = new MockXRefParser(canParse: true);
        mockParser.SetupPrevChain(new long?[] { 100, null }); // Chain: current -> 100 -> null
        
        var handler = new IncrementalUpdateHandler(mockParser);

        var content = "dummy content";
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        using var stream = new MemoryStream();
        using var reader = new ObjectReader(stream);

        var (trailer, table) = handler.ProcessIncrementalUpdates(provider, 0, 0, reader);

        Assert.Equal(2, mockParser.ParseCallCount); // Should parse twice due to /Prev chain
        Assert.NotNull(trailer);
        Assert.NotNull(table);
    }

    [Fact]
    public void Test_ProcessIncrementalUpdates_NoSuitableParser_ThrowsException()
    {
        var mockParser = new MockXRefParser(canParse: false);
        var handler = new IncrementalUpdateHandler(mockParser);

        var content = "invalid content";
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        using var stream = new MemoryStream();
        using var reader = new ObjectReader(stream);

        Assert.Throws<InvalidOperationException>(() => 
            handler.ProcessIncrementalUpdates(provider, 0, 0, reader));
    }

    [Fact]
    public void Test_ProcessIncrementalUpdates_MultipleParsers_UsesFirstApplicable()
    {
        var firstParser = new MockXRefParser(canParse: false);
        var secondParser = new MockXRefParser(canParse: true);
        var handler = new IncrementalUpdateHandler(firstParser, secondParser);

        var content = "content";
        var contentBytes = Encoding.ASCII.GetBytes(content);
        var provider = new PdfByteArrayProvider(contentBytes);

        using var stream = new MemoryStream();
        using var reader = new ObjectReader(stream);

        handler.ProcessIncrementalUpdates(provider, 0, 0, reader);

        Assert.Equal(0, firstParser.ParseCallCount);
        Assert.Equal(1, secondParser.ParseCallCount);
    }

    private class MockXRefParser : IXRefParser
    {
        private readonly bool _canParse;
        private readonly Queue<long?> _prevValues = new();

        public int ParseCallCount { get; private set; }

        public MockXRefParser(bool canParse)
        {
            _canParse = canParse;
        }

        public void SetupPrevChain(long?[] prevValues)
        {
            _prevValues.Clear();
            foreach (var prev in prevValues)
                _prevValues.Enqueue(prev);
        }

        public bool CanParse(IPdfBytesProvider pdfBytesProvider, long xrefPosition)
        {
            return _canParse;
        }

        public (Trailer Trailer, XRefTable XRefTable) Parse(IPdfBytesProvider pdfBytesProvider, long pdfStart, long xrefPosition, ObjectReader reader)
        {
            ParseCallCount++;

            // Create mock trailer with appropriate /Prev value
            var trailerDict = new PdfDictionary
            {
                [PdfNames.Size] = new PdfNumber(1),
                [PdfNames.Root] = new PdfReference { Id = new PdfObjectId(1, 0) }
            };

            if (_prevValues.TryDequeue(out var prevValue) && prevValue.HasValue)
                trailerDict[PdfNames.Prev] = new PdfNumber(prevValue.Value);

            var readerSettings = new ReaderSettings();
            var trailer = new Trailer(trailerDict, readerSettings);

            // Create mock xref table
            var items = new List<XRefItem>
            {
                new FreeXRefItem(new PdfObjectId(0, 65535, true))
            };
            var table = new XRefTable(items);

            return (trailer, table);
        }
    }
}
