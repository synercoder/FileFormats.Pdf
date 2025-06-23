using Synercoding.FileFormats.Pdf.IO;

namespace Synercoding.FileFormats.Pdf.Parsing.Internal.XRef;

internal class IncrementalUpdateHandler
{
    private readonly IXRefParser[] _xrefParsers;

    public IncrementalUpdateHandler(params IXRefParser[] xrefParsers)
    {
        _xrefParsers = xrefParsers ?? throw new ArgumentNullException(nameof(xrefParsers));
    }

    public Trailer GetTrailer(IPdfBytesProvider pdfBytesProvider,
        long pdfStart,
        long xrefPosition,
        ReaderSettings readerSettings)
    {
        foreach (var parser in _xrefParsers)
            if (parser.CanParse(pdfBytesProvider, pdfStart + xrefPosition))
                return parser.GetTrailer(pdfBytesProvider, pdfStart, xrefPosition, readerSettings);

        throw new InvalidOperationException($"No suitable XRef parser found for position {xrefPosition}");
    }

    public XRefTable ProcessIncrementalUpdates(
        IPdfBytesProvider pdfBytesProvider,
        long pdfStart,
        long initialXRefPosition,
        ObjectReader reader)
    {
        var (trailer, table) = _parseXRefSection(pdfBytesProvider, pdfStart, initialXRefPosition, reader);
        var lastTrailer = trailer;

        // Process incremental updates by following /Prev chain
        while (trailer.Prev.HasValue)
        {
            var (oldTrailer, previousTable) = _parseXRefSection(pdfBytesProvider, pdfStart, trailer.Prev.Value, reader);
            table = previousTable.Merge(table);
            trailer = oldTrailer;
        }

        return table;
    }

    private (Trailer Trailer, XRefTable XRefTable) _parseXRefSection(
        IPdfBytesProvider pdfBytesProvider,
        long pdfStart,
        long xrefPosition,
        ObjectReader reader)
    {
        foreach (var parser in _xrefParsers)
            if (parser.CanParse(pdfBytesProvider, pdfStart + xrefPosition))
                return parser.Parse(pdfBytesProvider, pdfStart, xrefPosition, reader);

        throw new InvalidOperationException($"No suitable XRef parser found for position {xrefPosition}");
    }
}
