using Synercoding.FileFormats.Pdf.Logging;
using Synercoding.FileFormats.Pdf.Parsing.Filters;

namespace Synercoding.FileFormats.Pdf;

public sealed class ReaderSettings
{
    /// <summary>
    /// When stream length is under this value, copy to memory for faster processing.
    /// </summary>
    public long MaxMemoryCopy { get; init; } = 50_000_000;

    /// <summary>
    /// When true, on encountering a recoverable parsing error, will throw an exception.
    /// When false, recover instead. Defaults to false.
    /// </summary>
    public bool Strict { get; init; } = false;

    /// <summary>
    /// The filters that are supported by the PdfReader
    /// </summary>
    public Filters SupportedStreamFilters { get; init; } = Filters.GetDefault();

    /// <summary>
    /// The logger used while reading PDF files.
    /// </summary>
    public IPdfLogger Logger { get; init; } = LoggerFactory.CreateNewLogger();
}
