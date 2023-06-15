using Synercoding.FileFormats.Pdf.Internals;
using Synercoding.FileFormats.Pdf.LowLevel.Colors.ColorSpaces;
using Synercoding.FileFormats.Pdf.LowLevel.Text;
using Synercoding.FileFormats.Pdf.LowLevel.XRef;

namespace Synercoding.FileFormats.Pdf.LowLevel.Internal;

internal sealed class PageResources : IDisposable
{
    private const string PREFIX_IMAGE = "Im";
    private const string PREFIX_SEPARATION = "Sep";

    private readonly TableBuilder _tableBuilder;
    private readonly Map<PdfName, Image> _images;
    private readonly Dictionary<Separation, (PdfName Name, PdfReference Id)> _separations;
    private readonly Dictionary<Type1StandardFont, PdfReference> _standardFonts;

    private int _separationCounter = 0;
    private int _imageCounter = 0;

    internal PageResources(TableBuilder tableBuilder)
    {
        _tableBuilder = tableBuilder;
        _images = new Map<PdfName, Image>();
        _separations = new Dictionary<Separation, (PdfName Name, PdfReference Id)>();
        _standardFonts = new Dictionary<Type1StandardFont, PdfReference>();
    }

    public IReadOnlyDictionary<PdfName, Image> Images
        => _images.Forward;

    internal IReadOnlyDictionary<Separation, (PdfName Name, PdfReference Id)> SeparationReferences
        => _separations;

    internal IReadOnlyCollection<(Type1StandardFont Font, PdfReference Reference)> FontReferences
        => _standardFonts
                .Select(kv => (kv.Key, kv.Value))
                .ToArray();

    public void Dispose()
    {
        foreach (var kv in _images)
            kv.Value.Dispose();

        _images.Clear();
    }

    public PdfName AddJpgUnsafe(System.IO.Stream jpgStream, int originalWidth, int originalHeight, ColorSpace colorSpace)
    {
        var id = _tableBuilder.ReserveId();

        var pdfImage = new Image(id, jpgStream, originalWidth, originalHeight, colorSpace);

        return AddImage(pdfImage);
    }

    public PdfName AddJpgUnsafe(System.IO.Stream jpgStream, int originalWidth, int originalHeight, PdfName colorSpace, double[] decodeArray)
    {
        var id = _tableBuilder.ReserveId();

        var pdfImage = new Image(id, jpgStream, originalWidth, originalHeight, colorSpace, decodeArray);

        return AddImage(pdfImage);
    }

    public PdfName AddImage(SixLabors.ImageSharp.Image image)
    {
        var id = _tableBuilder.ReserveId();

        var pdfImage = new Image(id, image);

        return AddImage(pdfImage);
    }

    public PdfName AddImage(Image image)
    {
        if (_images.Reverse.Contains(image))
            return _images.Reverse[image];

        var key = PREFIX_IMAGE + System.Threading.Interlocked.Increment(ref _imageCounter).ToString().PadLeft(6, '0');

        var pdfName = PdfName.Get(key);

        _images.Add(pdfName, image);

        return pdfName;
    }

    internal PdfName AddStandardFont(Type1StandardFont font)
    {
        if (!_standardFonts.ContainsKey(font))
            _standardFonts[font] = _tableBuilder.ReserveId();

        return font.LookupName;
    }

    internal PdfName AddSeparation(Separation separation)
    {
        if (_separations.TryGetValue(separation, out var tuple))
            return tuple.Name;

        var key = PREFIX_SEPARATION + System.Threading.Interlocked.Increment(ref _separationCounter).ToString().PadLeft(6, '0');
        var name = PdfName.Get(key);
        _separations[separation] = (name, _tableBuilder.ReserveId());

        return name;
    }
}
