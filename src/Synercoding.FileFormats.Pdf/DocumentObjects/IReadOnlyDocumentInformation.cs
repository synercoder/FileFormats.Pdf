namespace Synercoding.FileFormats.Pdf.DocumentObjects;

public interface IReadOnlyDocumentInformation
{
    /// <summary>
    /// The document's title
    /// </summary>
    string? Title { get; }

    /// <summary>
    /// The name of the person who created the document
    /// </summary>
    string? Author { get; }

    /// <summary>
    /// The subject of the document
    /// </summary>
    string? Subject { get; }

    /// <summary>
    /// Keywords associated with the document
    /// </summary>
    string? Keywords { get; }

    /// <summary>
    /// If the document was converted to PDF from another format, the name of the conforming product that created the original document from which it was converted. Otherwise the name of the application that created the document.
    /// </summary>
    string? Creator { get; }

    /// <summary>
    /// If the document was converted to PDF from another format, the name of the conforming product that converted it to PDF.
    /// </summary>
    string? Producer { get; }

    /// <summary>
    /// The date and time the document was created, in human-readable form.
    /// </summary>
    DateTimeOffset? CreationDate { get; }

    /// <summary>
    /// The date and time the document was most recently modified, in human-readable form.
    /// </summary>
    DateTimeOffset? ModDate { get; }

    /// <summary>
    /// Extra information that will be added to the PDF meta data
    /// </summary>
    IReadOnlyDictionary<string, string>? ExtraInfo { get; }
}
