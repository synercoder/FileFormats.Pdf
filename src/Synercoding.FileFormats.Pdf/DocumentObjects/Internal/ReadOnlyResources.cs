using Synercoding.FileFormats.Pdf.Logging;
using Synercoding.FileFormats.Pdf.Parsing;
using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.FileFormats.Pdf.Primitives.Extensions;

namespace Synercoding.FileFormats.Pdf.DocumentObjects.Internal;

internal class ReadOnlyResources : IReadOnlyResources
{
    private readonly IPdfDictionary _pdfDictionary;
    private readonly ObjectReader _objectReader;
    private readonly IPdfLogger _logger;

    internal ReadOnlyResources(PdfObjectId? id, IPdfDictionary dictionary, ObjectReader objectReader)
    {
        Id = id;
        _pdfDictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        _objectReader = objectReader ?? throw new ArgumentNullException(nameof(objectReader));
        _logger = objectReader.Settings.Logger;
    }

    internal PdfObjectId? Id { get; }

    public IPdfDictionary? ExtGState
        => _pdfDictionary.GetOrDefault<IPdfDictionary>(PdfNames.ExtGState, _objectReader, defaultValue: null);

    public IPdfDictionary? ColorSpace
        => _pdfDictionary.GetOrDefault<IPdfDictionary>(PdfNames.ColorSpace, _objectReader, defaultValue: null);

    public IPdfDictionary? Pattern
        => _pdfDictionary.GetOrDefault<IPdfDictionary>(PdfNames.Pattern, _objectReader, defaultValue: null);

    public IPdfDictionary? Shading
        => _pdfDictionary.GetOrDefault<IPdfDictionary>(PdfNames.Shading, _objectReader, defaultValue: null);

    public IPdfDictionary? XObject
        => _pdfDictionary.GetOrDefault<IPdfDictionary>(PdfNames.XObject, _objectReader, defaultValue: null);

    public IPdfDictionary? Font
        => _pdfDictionary.GetOrDefault<IPdfDictionary>(PdfNames.Font, _objectReader, defaultValue: null);

    public IPdfArray? ProcSet
        => _pdfDictionary.GetOrDefault<IPdfArray>(PdfNames.ProcSet, _objectReader, defaultValue: null);

    public IPdfDictionary? Properties
        => _pdfDictionary.GetOrDefault<IPdfDictionary>(PdfNames.Properties, _objectReader, defaultValue: null);
}
