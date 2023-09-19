using Synercoding.FileFormats.Pdf.Internals;
using Synercoding.FileFormats.Pdf.LowLevel;
using Synercoding.FileFormats.Pdf.LowLevel.Internal;
using Synercoding.FileFormats.Pdf.LowLevel.XRef;

namespace Synercoding.FileFormats.Pdf;

/// <summary>
/// This class represents a page in a pdf
/// </summary>
public sealed class PdfPage : IDisposable
{
    private readonly TableBuilder _tableBuilder;
    private readonly PageTree _parent;

    private PageRotation? _rotation;

    internal PdfPage(TableBuilder tableBuilder, PageTree parent)
    {
        _tableBuilder = tableBuilder;
        _parent = parent;
        _parent.AddPage(this);

        PageNumber = _parent.PageCount;
        Reference = tableBuilder.ReserveId();
        Resources = new PageResources(_tableBuilder);
        var contentStream = new ContentStream(tableBuilder.ReserveId(), Resources);

        Content = new PageContentContext(contentStream, new GraphicState());
    }

    internal PdfReference Parent
        => _parent.Reference;

    internal PageResources Resources { get; }

    /// <summary>
    /// The number of the page
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// The <see cref="IPageContentContext"/> to add content to the page.
    /// </summary>
    public IPageContentContext Content { get; }

    /// <summary>
    /// A pdf reference object that can be used to reference to this object
    /// </summary>
    public PdfReference Reference { get; }

    /// <summary>
    /// The rotation of how the page is displayed, must be in increments of 90
    /// </summary>
    public PageRotation? Rotation
    {
        get => _rotation;
        set
        {
            const string ARGUMENT_OUT_OF_RANGE_MESSAGE = "The provided value can only be increments of 90.";
            if (value is not null && !Enum.IsDefined(value.Value))
                throw new ArgumentOutOfRangeException(nameof(Rotation), value, ARGUMENT_OUT_OF_RANGE_MESSAGE);

            _rotation = value;
        }
    }

    /// <summary>
    /// The media box of the <see cref="PdfPage"/>
    /// </summary>
    public Rectangle MediaBox { get; set; } = Sizes.A4.AsRectangle();

    /// <summary>
    /// The cropbox of the <see cref="PdfPage"/>, defaults to <see cref="MediaBox"/>
    /// </summary>
    public Rectangle? CropBox { get; set; }

    /// <summary>
    /// The bleed box of the <see cref="PdfPage"/>
    /// </summary>
    public Rectangle? BleedBox { get; set; }

    /// <summary>
    /// The trim box of the <see cref="PdfPage"/>
    /// </summary>
    public Rectangle? TrimBox { get; set; }

    /// <summary>
    /// The art box of the <see cref="PdfPage"/>
    /// </summary>
    public Rectangle? ArtBox { get; set; }

    /// <inheritdoc />
    public void Dispose()
    {
        Resources.Dispose();

        Content.RawContentStream.Dispose();
    }
}
