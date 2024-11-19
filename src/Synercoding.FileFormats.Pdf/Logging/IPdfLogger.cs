namespace Synercoding.FileFormats.Pdf.Logging;

public interface IPdfLogger
{
    void Log(PdfLogLevel level, string category, Exception? exception, string message, object?[] args);
}
