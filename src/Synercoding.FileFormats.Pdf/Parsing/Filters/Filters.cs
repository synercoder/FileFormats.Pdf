using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Parsing.Filters;

public class Filters
{
    private readonly IDictionary<PdfName, IStreamFilter> _registeredFilters = new Dictionary<PdfName, IStreamFilter>();

    public Filters(params IStreamFilter[] filters)
    {
        if (filters is null)
            filters = Array.Empty<IStreamFilter>();

        foreach (var filter in filters)
            _registeredFilters[filter.Name] = filter;
    }

    public IStreamFilter Get(PdfName name)
    {
        if (!_registeredFilters.TryGetValue(name, out var filter))
            throw new PdfException($"Filter with name /{name.Display} is not supported."
                + "You can add new supported filters by registering them by calling "
                + $"{nameof(ReaderSettings)}.{nameof(ReaderSettings.SupportedStreamFilters)}.{nameof(ReaderSettings.SupportedStreamFilters.Register)}(filterToRegister).");

        return filter;
    }

    public Filters Register(IStreamFilter filter)
    {
        _ = filter ?? throw new ArgumentNullException(nameof(filter));
        _registeredFilters[filter.Name] = filter;
        return this;
    }

    public static Filters GetDefault()
        => new Filters([new ASCII85Decode(), new ASCIIHexDecode(), new RunlengthDecode(), new FlateDecode(), new DCTDecode(), new LZWDecode()]);
}
