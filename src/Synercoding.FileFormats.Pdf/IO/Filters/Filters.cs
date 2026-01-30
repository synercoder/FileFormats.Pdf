using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.IO.Filters;

/// <summary>
/// Registry for PDF stream filters, allowing registration and retrieval of filters by name.
/// </summary>
internal static class Filters
{
    private static readonly IDictionary<PdfName, IStreamFilter> _registeredFilters = new Dictionary<PdfName, IStreamFilter>()
    {
        [PdfNames.ASCIIHexDecode] = new ASCIIHexDecode(),
        [PdfNames.ASCII85Decode] = new ASCII85Decode(),
        [PdfNames.FlateDecode] = new FlateDecode(),
        [PdfNames.LZWDecode] = new LZWDecode(),
        [PdfNames.RunLengthDecode] = new RunLengthDecode(),
        [PdfNames.CCITTFaxDecode] = new CCITTFaxDecode(),
        [PdfNames.JBIG2Decode] = new JBIG2Decode(),
        [PdfNames.DCTDecode] = new DCTDecode(),
        [PdfNames.JPXDecode] = new JPXDecode(),
    };

    /// <summary>
    /// Gets a registered filter by its name.
    /// </summary>
    /// <param name="name">The name of the filter to retrieve.</param>
    /// <returns>The filter associated with the specified name.</returns>
    /// <exception cref="PdfException">Thrown when no filter is registered with the specified name.</exception>
    public static IStreamFilter Get(PdfName name)
    {
        if (!_registeredFilters.TryGetValue(name, out var filter))
            throw new PdfException($"Filter with name /{name.Display} is not supported.");

        return filter;
    }
}
