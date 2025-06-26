namespace Synercoding.FileFormats.Pdf.Exceptions;

public class ParseException : PdfException
{
    internal ParseException(string? message)
        : base(message)
    { }
}
