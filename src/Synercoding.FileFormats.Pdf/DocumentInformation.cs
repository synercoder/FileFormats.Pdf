using Synercoding.FileFormats.Pdf.LowLevel;

namespace Synercoding.FileFormats.Pdf;

/// <summary>
/// This class contains information about the document
/// </summary>
public class DocumentInformation
{
    internal DocumentInformation(PdfReference id)
    {
        Reference = id;
    }

    /// <summary>
    /// The document's title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// The name of the person who created the document
    /// </summary>
    public string? Author { get; set; }

    /// <summary>
    /// The subject of the document
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// Keywords associated with the document
    /// </summary>
    public string? Keywords { get; set; }

    /// <summary>
    /// If the document was converted to PDF from another format, the name of the conforming product that created the original document from which it was converted. Otherwise the name of the application that created the document.
    /// </summary>
    public string? Creator { get; set; }

    /// <summary>
    /// If the document was converted to PDF from another format, the name of the conforming product that converted it to PDF.
    /// </summary>
    public string? Producer { get; set; }

    /// <summary>
    /// The date and time the document was created, in human-readable form.
    /// </summary>
    public DateTime? CreationDate { get; set; } = DateTime.Now;

    /// <summary>
    /// The date and time the document was most recently modified, in human-readable form.
    /// </summary>
    public DateTime? ModDate { get; set; }

    /// <summary>
    /// A pdf reference object that can be used to reference to this object
    /// </summary>
    public PdfReference Reference { get; }

    /// <summary>
    /// Extra information that will be added to the PDF meta data
    /// </summary>
    public IDictionary<string, string> ExtraInfo { get; } = new Dictionary<string, string>();
}
