namespace Synercoding.FileFormats.Pdf.Logging;

public class VoidLogger : IPdfLogger
{
    public void Log(PdfLogLevel level, string category, Exception? exception, string message, object?[] args)
    { }
}
