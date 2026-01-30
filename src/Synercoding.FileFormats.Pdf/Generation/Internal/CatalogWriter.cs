using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Generation.Internal;

/// <summary>
/// Creates and writes the PDF catalog (root) object
/// </summary>
internal class CatalogWriter
{
    private readonly ObjectWriter _objectWriter;

    public CatalogWriter(ObjectWriter objectWriter)
    {
        _objectWriter = objectWriter ?? throw new ArgumentNullException(nameof(objectWriter));
    }

    /// <summary>
    /// Creates and writes the catalog object, returns its reference
    /// </summary>
    public PdfReference WriteCatalog(PdfReference pagesRef, WriterSettings settings, PageMode? pageMode, PageLayout? pageLayout)
    {
        var catalogId = _objectWriter.TableBuiler.ReserveId();

        var catalog = new PdfDictionary
        {
            [PdfNames.Type] = PdfNames.Catalog,
            [PdfNames.Pages] = pagesRef
        };

        // Add optional entries based on nullable values
        if (pageMode.HasValue)
        {
            catalog[PdfNames.PageMode] = pageMode.Value switch
            {
                PageMode.UseNone => PdfNames.UseNone,
                PageMode.UseOutlines => PdfNames.UseOutlines,
                PageMode.UseThumbs => PdfNames.UseThumbs,
                PageMode.FullScreen => PdfNames.FullScreen,
                PageMode.UseOC => PdfNames.UseOC,
                PageMode.UseAttachments => PdfNames.UseAttachments,
                _ => throw new InvalidOperationException("Provided page mode option not supported.")
            };
        }

        if (pageLayout.HasValue)
        {
            catalog[PdfNames.PageLayout] = pageLayout.Value switch
            {
                PageLayout.SinglePage => PdfNames.SinglePage,
                PageLayout.OneColumn => PdfNames.OneColumn,
                PageLayout.TwoColumnLeft => PdfNames.TwoColumnLeft,
                PageLayout.TwoColumnRight => PdfNames.TwoColumnRight,
                PageLayout.TwoPageLeft => PdfNames.TwoPageLeft,
                PageLayout.TwoPageRight => PdfNames.TwoPageRight,
                _ => throw new InvalidOperationException("Provided page layout option not supported.")
            };
        }

        _objectWriter.Write(new PdfObject<PdfDictionary>
        {
            Id = catalogId,
            Value = catalog
        });

        return catalogId.GetReference();
    }
}
