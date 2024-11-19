namespace Synercoding.FileFormats.Pdf.Logging;

public class ConsoleLogger : IPdfLogger
{
    private readonly Func<PdfLogLevel, string, bool> _isEnabled;

    public ConsoleLogger()
        : this((_, _) => true)
    { }

    public ConsoleLogger(Func<PdfLogLevel, string, bool> isEnabled)
    {
        _isEnabled = isEnabled;
    }

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
