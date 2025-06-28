namespace Synercoding.FileFormats.Pdf.Exceptions;

public class EncryptionException : ParseException
{
    internal EncryptionException(string message)
        : base(message)
    { }

    internal EncryptionException()
        : this("Could not decrypt the PDF.")
    { }
}
