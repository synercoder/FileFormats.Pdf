namespace Synercoding.FileFormats.Pdf.Exceptions;

/// <summary>
/// Exception thrown when PDF parsing operations fail.
/// </summary>
public class ParseException : PdfException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParseException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    internal ParseException(string? message)
        : base(message)
    { }
}
