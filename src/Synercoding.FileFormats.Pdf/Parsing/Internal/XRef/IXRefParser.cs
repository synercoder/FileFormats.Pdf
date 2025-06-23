using Synercoding.FileFormats.Pdf.IO;

namespace Synercoding.FileFormats.Pdf.Parsing.Internal.XRef;

internal interface IXRefParser
{
    bool CanParse(IPdfBytesProvider pdfBytesProvider, long xrefPosition);
    Trailer GetTrailer(IPdfBytesProvider pdfBytesProvider, long pdfStart, long xrefPosition, ReaderSettings readerSettings);
    (Trailer Trailer, XRefTable XRefTable) Parse(IPdfBytesProvider pdfBytesProvider, long pdfStart, long xrefPosition, ObjectReader reader);
}
