using Synercoding.FileFormats.Pdf.Content.Colors.ColorSpaces;
using Synercoding.FileFormats.Pdf.Content.Text.Fonts;
using Synercoding.FileFormats.Pdf.Generation.Internal;
using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf;

internal class CachedResources
{
    private readonly TableBuilder _tableBuilder;

    public CachedResources(TableBuilder tableBuilder)
    {
        _tableBuilder = tableBuilder;
    }

    public PdfReference GetOrAdd(Separation separation)
    {
        if (Separations.TryGetValue(separation, out var id))
            return id;

        id = new PdfReference(_tableBuilder.ReserveId());
        Separations.Add(separation, id);
        return id;
    }

    public (PdfReference Reference, FontUsageTracker FontUsageTracker) GetOrAdd(Font font)
    {
        if (Fonts.TryGetValue(font, out var tuple))
            return tuple;

        var id = new PdfReference(_tableBuilder.ReserveId());
        var tracker = new FontUsageTracker();
        Fonts.Add(font, (id, tracker));
        return (id, tracker);
    }

    public IDictionary<Separation, PdfReference> Separations { get; } = new Dictionary<Separation, PdfReference>();
    public IDictionary<Font, (PdfReference Reference, FontUsageTracker FontUsageTracker)> Fonts { get; }
        = new Dictionary<Font, (PdfReference, FontUsageTracker)>();
}
