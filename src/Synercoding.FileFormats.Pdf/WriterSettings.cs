using Synercoding.FileFormats.Pdf.Logging;
using Synercoding.FileFormats.Pdf.Parsing.Filters;

namespace Synercoding.FileFormats.Pdf;

public sealed class WriterSettings
{
    /// <summary>
    /// The filters that are supported by the PdfReader
    /// </summary>
    public Filters SupportedStreamFilters { get; init; } = Filters.GetDefault();

    /// <summary>
    /// The logger used while reading PDF files.
    /// </summary>
    public IPdfLogger Logger { get; init; } = LoggerFactory.CreateNewLogger();
}
