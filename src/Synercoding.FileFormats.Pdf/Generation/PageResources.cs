using SixLabors.ImageSharp.PixelFormats;
using Synercoding.FileFormats.Pdf.Collections;
using Synercoding.FileFormats.Pdf.Content;
using Synercoding.FileFormats.Pdf.Content.Colors.ColorSpaces;
using Synercoding.FileFormats.Pdf.Content.Text.Fonts;
using Synercoding.FileFormats.Pdf.Generation.Internal;
using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Generation;

internal sealed class PageResources : IDisposable
{
    private const string PREFIX_IMAGE = "Im";
    private const string PREFIX_FONT = "F";
    private const string PREFIX_SEPARATION = "Sep";
    private const string PREFIX_EXTGSTATE = "ExGs";

    private readonly TableBuilder _tableBuilder;
    private readonly CachedResources _cachedResources;
    private readonly Map<PdfName, PdfImage> _images;
    private readonly Dictionary<Separation, (PdfName Name, PdfReference Id)> _separations;
    private readonly Dictionary<Font, (PdfName Name, PdfReference Id, FontUsageTracker Tracker)> _fonts;
    private readonly Dictionary<ExtendedGraphicsState, (PdfName Name, PdfReference Id)> _extendedGraphicsStates;

    private int _stateCounter = 0;
    private int _separationCounter = 0;
    private int _imageCounter = 0;
    private int _fontCounter = 0;

    internal PageResources(TableBuilder tableBuilder, CachedResources cachedResources)
    {
        _tableBuilder = tableBuilder;
        _cachedResources = cachedResources;
        Id = tableBuilder.ReserveId();
        _images = new Map<PdfName, PdfImage>();
        _separations = new Dictionary<Separation, (PdfName Name, PdfReference Id)>();
        _fonts = new Dictionary<Font, (PdfName, PdfReference, FontUsageTracker)>();
        _extendedGraphicsStates = new Dictionary<ExtendedGraphicsState, (PdfName Name, PdfReference Id)>();
    }

    public PdfObjectId Id { get; }

    public IReadOnlyDictionary<PdfName, PdfImage> Images
        => _images.Forward;

    public IReadOnlyDictionary<ExtendedGraphicsState, (PdfName Name, PdfReference Id)> ExtendedGraphicsStates
        => _extendedGraphicsStates;

    internal IReadOnlyDictionary<Separation, (PdfName Name, PdfReference Id)> SeparationReferences
        => _separations;

    internal IReadOnlyDictionary<Font, (PdfName Name, PdfReference Id, FontUsageTracker Tracker)> FontReferences
        => _fonts;

    public void Dispose()
    {
        foreach (var kv in _images)
            kv.Value.Dispose();

        _images.Clear();
    }

    public PdfName AddJpgUnsafe(Stream jpgStream, int originalWidth, int originalHeight, ColorSpace colorSpace)
    {
        var id = _tableBuilder.ReserveId();

        var pdfImage = new PdfImage(id, jpgStream, originalWidth, originalHeight, colorSpace, null, null, (PdfNames.DCTDecode, null));

        return Add(pdfImage);
    }

    public PdfName Add(SixLabors.ImageSharp.Image<Rgba32> image, Separation separation, GrayScaleMethod grayScaleMethod = GrayScaleMethod.AverageOfRGBChannels)
    {
        var pdfImage = PdfImage.GetSeparation(_tableBuilder, image, separation, grayScaleMethod);

        return Add(pdfImage);
    }

    public PdfName Add(SixLabors.ImageSharp.Image<Rgba32> image)
    {
        var pdfImage = PdfImage.Get(_tableBuilder, image);

        return Add(pdfImage);
    }

    public PdfName Add(PdfImage image)
    {
        if (_images.Reverse.Contains(image))
            return _images.Reverse[image];

        var key = PREFIX_IMAGE + Interlocked.Increment(ref _imageCounter).ToString().PadLeft(6, '0');

        var pdfName = PdfName.Get(key);

        _images.Add(pdfName, image);

        return pdfName;
    }

    internal (PdfName, FontUsageTracker) Add(Font font)
    {
        if (_fonts.TryGetValue(font, out var triple))
            return (triple.Name, triple.Tracker);

        var key = PREFIX_FONT + Interlocked.Increment(ref _fontCounter).ToString().PadLeft(6, '0');

        var (reference, tracker) = _cachedResources.GetOrAdd(font);

        var pdfName = PdfName.Get(key);

        _fonts.Add(font, (pdfName, reference, tracker));

        return (pdfName, tracker);
    }

    internal PdfName Add(Separation separation)
    {
        if (_separations.TryGetValue(separation, out var tuple))
            return tuple.Name;

        var key = PREFIX_SEPARATION + Interlocked.Increment(ref _separationCounter).ToString().PadLeft(6, '0');
        var name = PdfName.Get(key);

        var sepRef = _cachedResources.GetOrAdd(separation);

        _separations[separation] = (name, sepRef);

        return name;
    }

    internal PdfName Add(ExtendedGraphicsState extendedGraphicsState)
    {
        if (_extendedGraphicsStates.TryGetValue(extendedGraphicsState, out var tuple))
            return tuple.Name;

        var key = PREFIX_EXTGSTATE + Interlocked.Increment(ref _stateCounter).ToString().PadLeft(6, '0');
        var name = PdfName.Get(key);
        _extendedGraphicsStates[extendedGraphicsState] = (name, new PdfReference(_tableBuilder.ReserveId()));

        return name;
    }
}
