using Synercoding.FileFormats.Pdf.DocumentObjects;

namespace Synercoding.FileFormats.Pdf.Generation.Extensions;

public static class PdfWriterExtensions
{
    /// <summary>
    /// Set meta information for this document.
    /// </summary>
    /// <param name="writer">The pdf writer to change the document info from.</param>
    /// <param name="infoAction">Action used to set meta data.</param>
    /// <returns>Returns <paramref name="writer"/> to chain calls.</returns>
    public static PdfWriter SetDocumentInfo(this PdfWriter writer, Action<DocumentInformation> infoAction)
    {
        infoAction(writer.DocumentInformation);

        return writer;
    }
}
