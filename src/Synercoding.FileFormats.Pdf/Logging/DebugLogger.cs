using System.Diagnostics;

namespace Synercoding.FileFormats.Pdf.Logging;

/// <summary>
/// A logger implementation that writes log messages to the Debug output stream.
/// </summary>
public class DebugLogger : IPdfLogger
{
    private readonly Func<PdfLogLevel, string, bool> _isEnabled;

    /// <summary>
    /// Initializes a new instance of the <see cref="DebugLogger"/> class with logging enabled for all levels and categories.
    /// </summary>
    public DebugLogger()
        : this((_, _) => true)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DebugLogger"/> class with a custom predicate to determine if logging is enabled.
    /// </summary>
    /// <param name="isEnabled">A function that determines whether logging is enabled for a given log level and category.</param>
    public DebugLogger(Func<PdfLogLevel, string, bool> isEnabled)
    {
        _isEnabled = isEnabled;
    }

    /// <summary>
    /// Writes a log message to the Debug output if logging is enabled for the specified level and category.
    /// </summary>
    /// <param name="level">The log level.</param>
    /// <param name="category">The log category.</param>
    /// <param name="exception">The exception associated with the log message, if any.</param>
    /// <param name="message">The log message format string.</param>
    /// <param name="args">The arguments to format into the message.</param>
    public void Log(PdfLogLevel level, string category, Exception? exception, string message, object?[] args)
    {
        if (!_isEnabled(level, category))
            return;

        Debug.WriteLine(SimpleLogFormatter.Format(level, category, exception, message, args));
    }
}
