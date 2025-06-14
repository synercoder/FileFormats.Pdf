namespace Synercoding.FileFormats.Pdf.Logging;

internal static class LoggerFactory
{
    public static IPdfLogger CreateNewLogger()
        => new VoidLogger();
}
        
