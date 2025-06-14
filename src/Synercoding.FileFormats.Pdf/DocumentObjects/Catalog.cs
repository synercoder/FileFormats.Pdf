using Synercoding.FileFormats.Pdf.Parsing;
using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.FileFormats.Pdf.Primitives.Extensions;

namespace Synercoding.FileFormats.Pdf.DocumentObjects;

internal class Catalog
{
    private readonly IPdfDictionary _pdfDictionary;
    private readonly ObjectReader _objectReader;

    public Catalog(IPdfDictionary pdfDictionary, ObjectReader objectReader)
    {
        _pdfDictionary = pdfDictionary;
        _objectReader = objectReader;
        if (!_pdfDictionary.TryGetValue<PdfName>(PdfNames.Type, _objectReader, out var type))
        {
            throw new ArgumentException($"The provided dictionary does not contain the required key {PdfNames.Type}",
                nameof(pdfDictionary));
        }

        if (type != PdfNames.Catalog)
            throw new ArgumentException("The provided dictionary type is not /Catalog.", nameof(pdfDictionary));

        Type = type;

        if (!_pdfDictionary.TryGetValue<PdfReference>(PdfNames.Pages, out var pagesReference))
        {
            throw new ArgumentException($"The provided dictionary does not contain the required key {PdfNames.Pages}",
                nameof(pdfDictionary));
        }

        Pages = PageTreeNode.GetRoot(pagesReference, objectReader);
    }

    public PdfName Type { get; }

    public PageTreeNode Pages { get; }
}
