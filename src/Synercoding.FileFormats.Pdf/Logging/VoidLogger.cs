namespace Synercoding.FileFormats.Pdf.Logging;

/// <summary>
/// A logger implementation that discards all log messages.
/// </summary>
public class VoidLogger : IPdfLogger
{
    /// <summary>
    /// Logs a message by discarding it (no operation).
    /// </summary>
    /// <param name="level">The log level for the message.</param>
    /// <param name="category">The category or source of the log message.</param>
    /// <param name="exception">An optional exception associated with the log message.</param>
    /// <param name="message">The log message format string.</param>
    /// <param name="args">The arguments to format into the message.</param>
    public void Log(PdfLogLevel level, string category, Exception? exception, string message, object?[] args)
    { }
}
