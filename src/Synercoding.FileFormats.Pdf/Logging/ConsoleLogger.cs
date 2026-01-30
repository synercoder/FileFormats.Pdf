namespace Synercoding.FileFormats.Pdf.Logging;

/// <summary>
/// A logger implementation that writes log messages to the console.
/// </summary>
public class ConsoleLogger : IPdfLogger
{
    private readonly Func<PdfLogLevel, string, bool> _isEnabled;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleLogger"/> class that logs all messages.
    /// </summary>
    public ConsoleLogger()
        : this((_, _) => true)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleLogger"/> class with a custom enablement function.
    /// </summary>
    /// <param name="isEnabled">A function that determines whether logging is enabled for a given level and category.</param>
    public ConsoleLogger(Func<PdfLogLevel, string, bool> isEnabled)
    {
        _isEnabled = isEnabled;
    }

    /// <summary>
    /// Logs a message to the console if enabled for the specified level and category.
    /// </summary>
    /// <param name="level">The log level for the message.</param>
    /// <param name="category">The category or source of the log message.</param>
    /// <param name="exception">An optional exception associated with the log message.</param>
    /// <param name="message">The log message format string.</param>
    /// <param name="args">The arguments to format into the message.</param>
    public void Log(PdfLogLevel level, string category, Exception? exception, string message, object?[] args)
    {
        if (!_isEnabled(level, category))
            return;

        var writer = level == PdfLogLevel.Error || level == PdfLogLevel.Critical
            ? Console.Error
            : Console.Out;

        writer.WriteLine(SimpleLogFormatter.Format(level, category, exception, message, args));
    }
}
