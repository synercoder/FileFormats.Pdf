namespace Synercoding.FileFormats.Pdf.Logging;

/// <summary>
/// A logger implementation that forwards log messages to multiple underlying loggers.
/// </summary>
public class MultiLogger : IPdfLogger
{
    private readonly IEnumerable<IPdfLogger> _loggers;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultiLogger"/> class.
    /// </summary>
    /// <param name="loggers">The collection of loggers to forward messages to.</param>
    /// <exception cref="ArgumentNullException">Thrown when loggers is null.</exception>
    public MultiLogger(params IEnumerable<IPdfLogger> loggers)
    {
        _loggers = loggers ?? throw new ArgumentNullException(nameof(loggers));
    }

    /// <summary>
    /// Forwards a log message to all underlying loggers. Exceptions from individual loggers are silently ignored.
    /// </summary>
    /// <param name="level">The log level.</param>
    /// <param name="category">The log category.</param>
    /// <param name="exception">The exception associated with the log message, if any.</param>
    /// <param name="message">The log message format string.</param>
    /// <param name="args">The arguments to format into the message.</param>
    public void Log(PdfLogLevel level, string category, Exception? exception, string message, object?[] args)
    {
        foreach (var logger in _loggers)
        {
            try
            {
                logger.Log(level, category, exception, message, args);
            }
            catch { }
        }
    }
}
