using Synercoding.FileFormats.Pdf.Logging;
using Synercoding.FileFormats.Pdf.Parsing.Filters;

namespace Synercoding.FileFormats.Pdf;

public class ReaderSettings
{
    /// <summary>
    /// When using a file stream and file size is under this value, copy to memory instead.
    /// </summary>
    public long MaxMemoryCopy { get; init; } = 50_000_000;

    public Filters SupportedStreamFilters { get; init; } = Filters.GetDefault();

    public IPdfLogger Logger { get; init; } = DefaultLoggerFactory();

    public static Func<IPdfLogger> DefaultLoggerFactory { get; set; } = _defaultLoggerFactory;

    private static IPdfLogger _defaultLoggerFactory()
        => new VoidLogger();
}
