using Microsoft.Extensions.Logging;
using Synercoding.FileFormats.Pdf.Logging;

namespace Synercoding.FileFormats.Pdf.ConsoleTester;

public class MicrosoftLogger : IPdfLogger
{
    private readonly ILoggerFactory _loggerFactory;
    public MicrosoftLogger(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    public void Log(PdfLogLevel level, string category, Exception? exception, string message, object?[] args)
    {
        var logger = _loggerFactory.CreateLogger(category);

        var microsoftLevel = level switch
        {
            PdfLogLevel.Trace => LogLevel.Trace,
            PdfLogLevel.Debug => LogLevel.Debug,
            PdfLogLevel.Information => LogLevel.Information,
            PdfLogLevel.Warning => LogLevel.Warning,
            PdfLogLevel.Error => LogLevel.Error,
            PdfLogLevel.Critical => LogLevel.Critical,

            _ => LogLevel.Information, // Default to information
        };

        logger.Log(microsoftLevel, 0, exception, message, args);
    }
}
