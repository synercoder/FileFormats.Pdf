using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Encryption;

public interface IDecryptor
{
    PdfString Decrypt(PdfString rawValue, PdfObjectId id);
    IPdfStreamObject Decrypt(IPdfStreamObject stream, PdfObjectId id);
}
