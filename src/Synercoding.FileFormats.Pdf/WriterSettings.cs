using Synercoding.FileFormats.Pdf.IO.Filters;
using Synercoding.FileFormats.Pdf.Logging;

namespace Synercoding.FileFormats.Pdf;

/// <summary>
/// Contains behavioral settings for a writer.
/// </summary>
public sealed class WriterSettings
{
    /// <summary>
    /// The logger used while reading PDF files.
    /// </summary>
    public IPdfLogger Logger { get; init; } = LoggerFactory.CreateNewLogger();

    /// <summary>
    /// Enable or disable font subsetting. Default is true.
    /// When enabled, only the glyphs actually used in the document will be embedded, reducing file size.
    /// </summary>
    public bool EnableSubsetting { get; init; } = true;

    /// <summary>
    /// The filters to be used to encode contentstreams. Defaults to just FlateDecode.
    /// </summary>
    public IStreamFilter[] ContentStreamFilters { get; init; } = [new FlateDecode()];
}
