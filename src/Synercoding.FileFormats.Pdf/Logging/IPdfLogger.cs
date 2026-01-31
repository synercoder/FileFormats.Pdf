namespace Synercoding.FileFormats.Pdf.Logging;

/// <summary>
/// Defines methods for logging PDF-related messages.
/// </summary>
public interface IPdfLogger
{
    /// <summary>
    /// Logs a message with the specified level, category, and optional exception.
    /// </summary>
    /// <param name="level">The log level for the message.</param>
    /// <param name="category">The category or source of the log message.</param>
    /// <param name="exception">An optional exception associated with the log message.</param>
    /// <param name="message">The log message format string.</param>
    /// <param name="args">The arguments to format into the message.</param>
    void Log(PdfLogLevel level, string category, Exception? exception, string message, object?[] args);
}
