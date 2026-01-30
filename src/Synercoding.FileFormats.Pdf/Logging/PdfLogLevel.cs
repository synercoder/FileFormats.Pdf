namespace Synercoding.FileFormats.Pdf.Logging;

/// <summary>
/// Specifies the severity level of a log message.
/// </summary>
public enum PdfLogLevel
{
    /// <summary>
    /// Logs that contain the most detailed messages.
    /// </summary>
    Trace,

    /// <summary>
    /// Logs that are used for interactive investigation during development.
    /// </summary>
    Debug,

    /// <summary>
    /// Logs that track the general flow of the application.
    /// </summary>
    Information,

    /// <summary>
    /// Logs that highlight an abnormal or unexpected event but do not stop application execution.
    /// </summary>
    Warning,

    /// <summary>
    /// Logs that highlight when the current flow of execution is stopped due to a failure.
    /// </summary>
    Error,

    /// <summary>
    /// Logs that describe an unrecoverable application or system crash.
    /// </summary>
    Critical
}
