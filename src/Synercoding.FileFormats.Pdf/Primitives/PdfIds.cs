namespace Synercoding.FileFormats.Pdf.Primitives;

/// <summary>
/// Represents the document identifier pair found in PDF trailers.
/// </summary>
public class PdfIds
{
    internal PdfIds(byte[] originalId, byte[] lastVersionId)
    {
        OriginalId = originalId;
        LastVersionId = lastVersionId;
    }

    /// <summary>
    /// Gets the original document identifier created when the document was first created.
    /// </summary>
    public byte[] OriginalId { get; }

    /// <summary>
    /// Gets the document identifier for the last version of the document.
    /// </summary>
    public byte[] LastVersionId { get; }
}
