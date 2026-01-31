using SixLabors.ImageSharp.PixelFormats;
using Synercoding.FileFormats.Pdf.Content;
using Synercoding.FileFormats.Pdf.Content.Colors.ColorSpaces;
using Synercoding.FileFormats.Pdf.DocumentObjects;
using Synercoding.FileFormats.Pdf.Generation;
using Synercoding.FileFormats.Pdf.Generation.Internal;
using Synercoding.FileFormats.Pdf.Primitives;
using System.Reflection;

namespace Synercoding.FileFormats.Pdf;

/// <summary>
/// Provides functionality to write and create PDF documents.
/// </summary>
public class PdfWriter : IDisposable
{
    private readonly CachedResources _cachedResources;
    private readonly PdfStream _pdfStream;
    private readonly WriterSettings _settings;
    private readonly ObjectWriter _objectWriter;
    private readonly IList<PdfDictionary> _pages = new List<PdfDictionary>();

    private bool _trailerWritten = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="PdfWriter"/> class to write to a file.
    /// </summary>
    /// <param name="filePath">The path to the PDF file to create.</param>
    public PdfWriter(string filePath)
        : this(filePath, new WriterSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PdfWriter"/> class to write to a file with settings.
    /// </summary>
    /// <param name="filePath">The path to the PDF file to create.</param>
    /// <param name="settings">The writer settings.</param>
    public PdfWriter(string filePath, WriterSettings settings)
        : this(File.OpenRead(filePath), settings, true)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PdfWriter"/> class to write to a stream.
    /// </summary>
    /// <param name="stream">The stream to write PDF data to.</param>
    public PdfWriter(Stream stream)
        : this(stream, new WriterSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PdfWriter"/> class to write to a stream with settings.
    /// </summary>
    /// <param name="stream">The stream to write PDF data to.</param>
    /// <param name="settings">The writer settings.</param>
    public PdfWriter(Stream stream, WriterSettings settings)
        : this(stream, settings, false)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PdfWriter"/> class to write to a stream with settings and dispose control.
    /// </summary>
    /// <param name="stream">The stream to write PDF data to.</param>
    /// <param name="settings">The writer settings.</param>
    /// <param name="ownsStream">Whether to dispose the stream when the writer is disposed.</param>
    public PdfWriter(Stream stream, WriterSettings settings, bool ownsStream)
        : this(new PdfStream(stream, ownsStream), settings)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PdfWriter"/> class to write to a PDF stream with settings.
    /// </summary>
    /// <param name="pdfStream">The PDF stream to write to.</param>
    /// <param name="settings">The writer settings.</param>
    public PdfWriter(PdfStream pdfStream, WriterSettings settings)
    {
        _pdfStream = pdfStream;
        _settings = settings;

        var pdfStart = PdfHeaderWriter.WriteTo(_pdfStream, new PdfVersion(2, 0));

        _objectWriter = new ObjectWriter(_pdfStream, pdfStart);

        DocumentInformation = new DocumentInformation(this)
        {
            Producer = $"Synercoding.FileFormats.Pdf {typeof(PdfWriter).GetTypeInfo().Assembly.GetName().Version}",
            CreationDate = DateTimeOffset.Now,
        };

        _cachedResources = new CachedResources(_objectWriter.TableBuiler);
    }

    internal void ThrowsWhenEndingWritten()
    {
        if (_trailerWritten)
            throw new InvalidOperationException("Can not modify the PDF further since the trailer has been written to the stream.");
    }

    /// <summary>
    /// Document information, such as the author and title
    /// </summary>
    public DocumentInformation DocumentInformation { get; }

    /// <summary>
    /// Specifies how the document should be displayed when opened. When null, no page mode is written to the catalog.
    /// </summary>
    public PageMode? PageMode { get; set; }

    /// <summary>
    /// Specifies the page layout to be used when the document is opened. When null, no page layout is written to the catalog.
    /// </summary>
    public PageLayout? PageLayout { get; set; }

    /// <summary>
    /// Gets the number of pages in the document.
    /// </summary>
    public int PageCount
        => _pages.Count;

    /// <summary>
    /// Adds a JPEG image from a stream without validation.
    /// </summary>
    /// <param name="stream">The stream containing JPEG data.</param>
    /// <param name="width">The width of the image in pixels.</param>
    /// <param name="height">The height of the image in pixels.</param>
    /// <param name="colorSpace">The color space of the image.</param>
    /// <returns>The added PDF image.</returns>
    public PdfImage AddJpgUnsafe(Stream stream, int width, int height, ColorSpace colorSpace)
    {
        var pdfImage = new PdfImage(_objectWriter.TableBuiler.ReserveId(), stream, width, height, colorSpace, null, null, (PdfNames.DCTDecode, null));

        _objectWriter.Write(pdfImage.ToStreamObject(_cachedResources));

        return pdfImage;
    }

    /// <summary>
    /// Adds an RGBA image to the document.
    /// </summary>
    /// <param name="image">The image to add.</param>
    /// <returns>The added PDF image.</returns>
    public PdfImage AddImage(SixLabors.ImageSharp.Image<Rgba32> image)
    {
        var pdfImage = PdfImage.Get(_objectWriter.TableBuiler, image);

        _objectWriter.Write(pdfImage.ToStreamObject(_cachedResources));

        return pdfImage;
    }

    /// <summary>
    /// Adds an RGB image to the document.
    /// </summary>
    /// <param name="image">The image to add.</param>
    /// <returns>The added PDF image.</returns>
    public PdfImage AddImage(SixLabors.ImageSharp.Image<Rgb24> image)
    {
        var pdfImage = PdfImage.Get(_objectWriter.TableBuiler, image);

        _objectWriter.Write(pdfImage.ToStreamObject(_cachedResources));

        return pdfImage;
    }

    /// <summary>
    /// Adds an RGBA image as a separation color space image.
    /// </summary>
    /// <param name="image">The image to add.</param>
    /// <param name="separation">The separation color space definition.</param>
    /// <param name="grayScaleMethod">The method to convert to grayscale.</param>
    /// <param name="decodeArray">Optional decode array for the image.</param>
    /// <returns>The added PDF image.</returns>
    public PdfImage AddSeparationImage(SixLabors.ImageSharp.Image<Rgba32> image, Separation separation, GrayScaleMethod grayScaleMethod = GrayScaleMethod.AverageOfRGBChannels, double[]? decodeArray = null)
    {
        var pdfImage = PdfImage.GetSeparation(_objectWriter.TableBuiler, image, separation, grayScaleMethod, decodeArray);

        _objectWriter.Write(pdfImage.ToStreamObject(_cachedResources));

        var separationRef = _cachedResources.GetOrAdd(separation);
        _objectWriter.Write(new PdfObject<IPdfArray>()
        {
            Id = separationRef.Id,
            Value = separation.ToPdfArray()
        });

        return pdfImage;
    }

    /// <summary>
    /// Adds an RGB image as a separation color space image.
    /// </summary>
    /// <param name="image">The image to add.</param>
    /// <param name="separation">The separation color space definition.</param>
    /// <param name="grayScaleMethod">The method to convert to grayscale.</param>
    /// <param name="decodeArray">Optional decode array for the image.</param>
    /// <returns>The added PDF image.</returns>
    public PdfImage AddSeparationImage(SixLabors.ImageSharp.Image<Rgb24> image, Separation separation, GrayScaleMethod grayScaleMethod = GrayScaleMethod.AverageOfRGBChannels, double[]? decodeArray = null)
    {
        var pdfImage = PdfImage.GetSeparation(_objectWriter.TableBuiler, image, separation, grayScaleMethod, decodeArray);

        _objectWriter.Write(pdfImage.ToStreamObject(_cachedResources));

        return pdfImage;
    }

    /// <summary>
    /// Add a page to the pdf file
    /// </summary>
    /// <param name="pageAction">Action used to setup the page</param>
    /// <returns>Returns this <see cref="PdfWriter"/> to chain calls</returns>
    public PdfWriter AddPage(Action<WriteablePage> pageAction)
        => AddPage(pageAction, static (action, page) => action(page));

    /// <summary>
    /// Add a page to the pdf file
    /// </summary>
    /// <param name="data">Data passed into the action</param>
    /// <param name="pageAction">Action used to setup the page</param>
    /// <returns>Returns this <see cref="PdfWriter"/> to chain calls</returns>
    public PdfWriter AddPage<T>(T data, Action<T, WriteablePage> pageAction)
    {
        ThrowsWhenEndingWritten();

        using (var page = new WriteablePage(_objectWriter.TableBuiler, _cachedResources, _pages.Count + 1))
        {
            pageAction(data, page);
            _processPage(page);
        }

        return this;
    }

    /// <summary>
    /// Add a page to the pdf file
    /// </summary>
    /// <param name="pageAction">Action used to setup the page</param>
    /// <returns>Returns an awaitable task that resolves into this <see cref="PdfWriter"/> to chain calls</returns>
    public async Task<PdfWriter> AddPageAsync(Func<WriteablePage, Task> pageAction)
        => await AddPageAsync(pageAction, static async (action, page) => await action(page));

    /// <summary>
    /// Add a page to the pdf file
    /// </summary>
    /// <param name="data">Data passed into the action</param>
    /// <param name="pageAction">Action used to setup the page</param>
    /// <returns>Returns an awaitable task that resolves into this <see cref="PdfWriter"/> to chain calls</returns>
    public async Task<PdfWriter> AddPageAsync<T>(T data, Func<T, WriteablePage, Task> pageAction)
    {
        ThrowsWhenEndingWritten();

        using (var page = new WriteablePage(_objectWriter.TableBuiler, _cachedResources, _pages.Count + 1))
        {
            await pageAction(data, page);
            _processPage(page);
        }

        return this;
    }

    private void _processPage(WriteablePage page)
    {
        // Write resources
        var pageResourceWriter = new PageResourcesWriter(_objectWriter, _cachedResources);
        var resourcesDict = pageResourceWriter.Write(page.Resources);
        _objectWriter.Write(new PdfObject<IPdfDictionary>
        {
            Id = page.Resources.Id,
            Value = resourcesDict
        });

        // Write content stream
        var contentStreamId = page.Content.RawContentStream.Id;
        var streamObj = page.Content.RawContentStream.InnerStream.ToStreamObject(_settings.ContentStreamFilters);
        _objectWriter.Write(new PdfObject<IPdfStreamObject>()
        {
            Id = contentStreamId,
            Value = streamObj
        });

        _pages.Add(page.ToDictionary());
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_trailerWritten)
            return;

        _writeTrailer();
        _pdfStream.Dispose();
    }

    private void _writeTrailer()
    {
        _trailerWritten = true;

        _writeAllUsedFonts();

        var pagesRef = _writePageTreeAndGetRootRef();

        var infoWriter = new DocumentInformationWriter(_objectWriter);
        var infoRef = infoWriter.WriteIfNeeded(DocumentInformation);

        var catalogWriter = new CatalogWriter(_objectWriter);
        var catalogRef = catalogWriter.WriteCatalog(pagesRef, _settings, PageMode, PageLayout);

        var xrefWriter = new CrossReferenceTableWriter(_objectWriter.TableBuiler);
        var xrefPosition = xrefWriter.WriteTo(_pdfStream);

        var trailerWriter = new TrailerWriter(_objectWriter.TableBuiler, _settings);
        trailerWriter.WriteTo(_pdfStream, catalogRef, infoRef, xrefPosition);
    }

    private void _writeAllUsedFonts()
    {
        foreach (var (font, (reference, tracker)) in _cachedResources.Fonts)
            font.WriteTo(_objectWriter, reference.Id, tracker, _settings);
    }

    private PdfReference _writePageTreeAndGetRootRef()
    {
        var (nodes, root) = PageTreeGenerator.Create(_objectWriter.TableBuiler, _pages);

        foreach (var node in nodes)
            _objectWriter.Write(node);

        return root;
    }
}
