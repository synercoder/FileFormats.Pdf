namespace Synercoding.FileFormats.Pdf.DocumentObjects;

public class PdfIds
{
    internal PdfIds(byte[] originalId, byte[] lastVersionId)
    {
        OriginalId = originalId;
        LastVersionId = lastVersionId;
    }

    public byte[] OriginalId { get; }
    public byte[] LastVersionId { get; }
}
