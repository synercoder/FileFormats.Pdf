namespace Synercoding.FileFormats.Pdf.Logging;

public class MultiLogger : IPdfLogger
{
    private readonly IEnumerable<IPdfLogger> _loggers;

    public MultiLogger(params IEnumerable<IPdfLogger> loggers)
    {
        _loggers = loggers ?? throw new ArgumentNullException(nameof(loggers));
    }

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
