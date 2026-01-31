namespace Synercoding.FileFormats.Pdf.Exceptions;

/// <summary>
/// Base exception class for PDF-related errors.
/// </summary>
public class PdfException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PdfException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    internal PdfException(string? message)
        : base(message)
    { }
}
