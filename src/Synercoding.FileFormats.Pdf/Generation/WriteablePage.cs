using Synercoding.FileFormats.Pdf.Content;
using Synercoding.FileFormats.Pdf.Content.Internals;
using Synercoding.FileFormats.Pdf.Generation.Internal;
using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.FileFormats.Pdf.Primitives.Extensions;

namespace Synercoding.FileFormats.Pdf.Generation;

/// <summary>
/// Represents a page in a PDF document that can be written to with content and configured with various page properties.
/// </summary>
public class WriteablePage : IDisposable
{
    private readonly TableBuilder _tableBuilder;

    internal WriteablePage(TableBuilder tableBuilder, CachedResources cachedResources, int pageNumber)
    {
        _tableBuilder = tableBuilder;
        PageNumber = pageNumber;
        Resources = new PageResources(_tableBuilder, cachedResources);
        var contentStream = new ContentStream(tableBuilder.ReserveId(), Resources);

        Content = new PageContentContext(contentStream, new GraphicsState());
        _tableBuilder = tableBuilder;
    }

    internal PageResources Resources { get; }

    /// <summary>
    /// The rotation of how the page is displayed, must be in increments of 90
    /// </summary>
    public PageRotation? Rotation
    {
        get;
        set
        {
            const string ARGUMENT_OUT_OF_RANGE_MESSAGE = "The provided value can only be increments of 90.";
            if (value is not null && !Enum.IsDefined(value.Value))
                throw new ArgumentOutOfRangeException(nameof(Rotation), value, ARGUMENT_OUT_OF_RANGE_MESSAGE);

            field = value;
        }
    }

    /// <summary>
    /// The number of this page
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// The media box of the <see cref="WriteablePage"/>
    /// </summary>
    public Rectangle MediaBox { get; set; } = Sizes.A4.AsRectangle();

    /// <summary>
    /// The crop box of the <see cref="WriteablePage"/>, defaults to <see cref="MediaBox"/>
    /// </summary>
    public Rectangle? CropBox { get; set; }

    /// <summary>
    /// The bleed box of the <see cref="WriteablePage"/>
    /// </summary>
    public Rectangle? BleedBox { get; set; }

    /// <summary>
    /// The trim box of the <see cref="WriteablePage"/>
    /// </summary>
    public Rectangle? TrimBox { get; set; }

    /// <summary>
    /// The art box of the <see cref="WriteablePage"/>
    /// </summary>
    public Rectangle? ArtBox { get; set; }

    /// <summary>
    /// The <see cref="IPageContentContext"/> to add content to the page.
    /// </summary>
    public IPageContentContext Content { get; }

    internal PdfDictionary ToDictionary()
    {
        var dictionary = new PdfDictionary()
        {
            [PdfNames.Type] = PdfNames.Page,
            [PdfNames.MediaBox] = MediaBox.ToArray(),
            [PdfNames.Resources] = Resources.Id.GetReference(),
            [PdfNames.Contents] = Content.RawContentStream.Id.GetReference()
        };

        if (CropBox.HasValue)
            dictionary[PdfNames.CropBox] = CropBox.Value.ToArray();
        if (BleedBox.HasValue)
            dictionary[PdfNames.BleedBox] = BleedBox.Value.ToArray();
        if (TrimBox.HasValue)
            dictionary[PdfNames.TrimBox] = TrimBox.Value.ToArray();
        if (ArtBox.HasValue)
            dictionary[PdfNames.ArtBox] = ArtBox.Value.ToArray();
        if (Rotation.HasValue)
            dictionary[PdfNames.Rotate] = new PdfNumber((int)Rotation.Value);

        return dictionary;
    }

    /// <summary>
    /// Releases all resources used by the <see cref="WriteablePage"/>.
    /// </summary>
    public void Dispose()
    {
        Resources.Dispose();

        Content.RawContentStream.Dispose();
    }
}
