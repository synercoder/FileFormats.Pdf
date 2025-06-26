namespace Synercoding.FileFormats.Pdf.Exceptions;

public class EncryptionException : ParseException
{
    internal EncryptionException()
        : base("Could not decrypt the PDF.")
    { }
}
