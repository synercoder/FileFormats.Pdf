namespace Synercoding.FileFormats.Pdf.Logging;

/// <summary>
/// Factory for creating PDF logger instances.
/// </summary>
internal static class LoggerFactory
{
    /// <summary>
    /// Creates a new logger instance.
    /// </summary>
    /// <returns>A new logger instance.</returns>
    public static IPdfLogger CreateNewLogger()
        => new VoidLogger();
}

