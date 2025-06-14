namespace Synercoding.FileFormats.Pdf.Exceptions;

public class ParseException : PdfException
{
    internal ParseException(string? message)
        : base(message)
    { }
}

public class UnexpectedEndOfFileException : ParseException
{
    internal UnexpectedEndOfFileException()
        :base("Unexpected end of pdf data.")
    { }
}
