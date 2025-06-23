using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Parsing.Internal.XRef;

namespace Synercoding.FileFormats.Pdf.Parsing.Internal;

internal class PdfFooter
{
    private readonly StartXRefFinder _startXRefFinder;
    private readonly IncrementalUpdateHandler _incrementalUpdateHandler;

    public PdfFooter(
        StartXRefFinder startXRefFinder,
        IncrementalUpdateHandler incrementalUpdateHandler)
    {
        _startXRefFinder = startXRefFinder ?? throw new ArgumentNullException(nameof(startXRefFinder));
        _incrementalUpdateHandler = incrementalUpdateHandler ?? throw new ArgumentNullException(nameof(incrementalUpdateHandler));
    }

    public static PdfFooter CreateDefault()
    {
        var startXRefFinder = new StartXRefFinder();
        var xrefParsers = new IXRefParser[]
        {
            new ClassicXRefParser(),
            new XRefStreamParser()
        };
        var incrementalUpdateHandler = new IncrementalUpdateHandler(xrefParsers);

        return new PdfFooter(startXRefFinder, incrementalUpdateHandler);
    }

    public XRefTable Parse(IPdfBytesProvider pdfBytesProvider, long pdfStart, ObjectReader reader)
    {
        var startXRef = _startXRefFinder.FindStartXRef(pdfBytesProvider, pdfStart);
        return _incrementalUpdateHandler.ProcessIncrementalUpdates(pdfBytesProvider, pdfStart, startXRef, reader);
    }

    public Trailer GetTrailer(IPdfBytesProvider pdfBytesProvider, long pdfStart, ReaderSettings readerSettings)
    {
        var startXRef = _startXRefFinder.FindStartXRef(pdfBytesProvider, pdfStart);
        return _incrementalUpdateHandler.GetTrailer(pdfBytesProvider, pdfStart, startXRef, readerSettings);
    }
}

