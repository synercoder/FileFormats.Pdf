namespace Synercoding.FileFormats.Pdf.Logging;

internal static class IPdfLoggerExtensions
{
    public static void LogTrace(this IPdfLogger? logger, string category, string message, params object?[] args)
        => logger?.Log(PdfLogLevel.Trace, category, null, message, args);
    public static void LogTrace(this IPdfLogger? logger, string category, Exception exception, string message, params object?[] args)
        => logger?.Log(PdfLogLevel.Trace, category, exception, message, args);
    public static void LogTrace<TCategory>(this IPdfLogger? logger, string message, params object?[] args)
        => logger?.Log(PdfLogLevel.Trace, typeof(TCategory).FullName ?? typeof(TCategory).Name, null, message, args);
    public static void LogTrace<TCategory>(this IPdfLogger? logger, Exception exception, string message, params object?[] args)
        => logger?.Log(PdfLogLevel.Trace, typeof(TCategory).FullName ?? typeof(TCategory).Name, exception, message, args);

    public static void LogDebug(this IPdfLogger? logger, string category, string message, params object?[] args)
        => logger?.Log(PdfLogLevel.Debug, category, null, message, args);
    public static void LogDebug(this IPdfLogger? logger, string category, Exception exception, string message, params object?[] args)
        => logger?.Log(PdfLogLevel.Debug, category, exception, message, args);
    public static void LogDebug<TCategory>(this IPdfLogger? logger, string message, params object?[] args)
        => logger?.Log(PdfLogLevel.Debug, typeof(TCategory).FullName ?? typeof(TCategory).Name, null, message, args);
    public static void LogDebug<TCategory>(this IPdfLogger? logger, Exception exception, string message, params object?[] args)
        => logger?.Log(PdfLogLevel.Debug, typeof(TCategory).FullName ?? typeof(TCategory).Name, exception, message, args);

    public static void LogInformation(this IPdfLogger? logger, string category, string message, params object?[] args)
        => logger?.Log(PdfLogLevel.Information, category, null, message, args);
    public static void LogInformation(this IPdfLogger? logger, string category, Exception exception, string message, params object?[] args)
        => logger?.Log(PdfLogLevel.Information, category, exception, message, args);
    public static void LogInformation<TCategory>(this IPdfLogger? logger, string message, params object?[] args)
        => logger?.Log(PdfLogLevel.Information, typeof(TCategory).FullName ?? typeof(TCategory).Name, null, message, args);
    public static void LogInformation<TCategory>(this IPdfLogger? logger, Exception exception, string message, params object?[] args)
        => logger?.Log(PdfLogLevel.Information, typeof(TCategory).FullName ?? typeof(TCategory).Name, exception, message, args);

    public static void LogWarning(this IPdfLogger? logger, string category, string message, params object?[] args)
        => logger?.Log(PdfLogLevel.Warning, category, null, message, args);
    public static void LogWarning(this IPdfLogger? logger, string category, Exception exception, string message, params object?[] args)
        => logger?.Log(PdfLogLevel.Warning, category, exception, message, args);
    public static void LogWarning<TCategory>(this IPdfLogger? logger, string message, params object?[] args)
        => logger?.Log(PdfLogLevel.Warning, typeof(TCategory).FullName ?? typeof(TCategory).Name, null, message, args);
    public static void LogWarning<TCategory>(this IPdfLogger? logger, Exception exception, string message, params object?[] args)
        => logger?.Log(PdfLogLevel.Warning, typeof(TCategory).FullName ?? typeof(TCategory).Name, exception, message, args);

    public static void LogError(this IPdfLogger? logger, string category, string message, params object?[] args)
        => logger?.Log(PdfLogLevel.Error, category, null, message, args);
    public static void LogError(this IPdfLogger? logger, string category, Exception exception, string message, params object?[] args)
        => logger?.Log(PdfLogLevel.Error, category, exception, message, args);
    public static void LogError<TCategory>(this IPdfLogger? logger, string message, params object?[] args)
        => logger?.Log(PdfLogLevel.Error, typeof(TCategory).FullName ?? typeof(TCategory).Name, null, message, args);
    public static void LogError<TCategory>(this IPdfLogger? logger, Exception exception, string message, params object?[] args)
        => logger?.Log(PdfLogLevel.Error, typeof(TCategory).FullName ?? typeof(TCategory).Name, exception, message, args);

    public static void LogCritical(this IPdfLogger? logger, string category, string message, params object?[] args)
        => logger?.Log(PdfLogLevel.Critical, category, null, message, args);
    public static void LogCritical(this IPdfLogger? logger, string category, Exception exception, string message, params object?[] args)
        => logger?.Log(PdfLogLevel.Critical, category, exception, message, args);
    public static void LogCritical<TCategory>(this IPdfLogger? logger, string message, params object?[] args)
        => logger?.Log(PdfLogLevel.Critical, typeof(TCategory).FullName ?? typeof(TCategory).Name, null, message, args);
    public static void LogCritical<TCategory>(this IPdfLogger? logger, Exception exception, string message, params object?[] args)
        => logger?.Log(PdfLogLevel.Critical, typeof(TCategory).FullName ?? typeof(TCategory).Name, exception, message, args);
}
