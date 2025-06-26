namespace Synercoding.FileFormats.Pdf.Exceptions;

public class UnexpectedEndOfFileException : ParseException
{
    internal UnexpectedEndOfFileException()
        :base("Unexpected end of pdf data.")
    { }
}
