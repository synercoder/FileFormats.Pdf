using System.Diagnostics;

namespace Synercoding.FileFormats.Pdf.Logging;

public class DebugLogger : IPdfLogger
{
    private readonly Func<PdfLogLevel, string, bool> _isEnabled;

    public DebugLogger()
        : this((_, _) => true)
    { }

    public DebugLogger(Func<PdfLogLevel, string, bool> isEnabled)
    {
        _isEnabled = isEnabled;
    }

    public void Log(PdfLogLevel level, string category, Exception? exception, string message, object?[] args)
    {
        if (!_isEnabled(level, category))
            return;

        Debug.WriteLine(SimpleLogFormatter.Format(level, category, exception, message, args));
    }
}
