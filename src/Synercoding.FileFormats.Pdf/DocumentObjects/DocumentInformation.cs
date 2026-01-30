using Synercoding.FileFormats.Pdf.Generation.Internal;

namespace Synercoding.FileFormats.Pdf.DocumentObjects;

/// <summary>
/// This class contains information about the document
/// </summary>
public class DocumentInformation
{
    private readonly PdfWriter _pdfWriter;

    internal DocumentInformation(PdfWriter pdfWriter)
    {
        _pdfWriter = pdfWriter ?? throw new ArgumentNullException(nameof(pdfWriter));
        ExtraInfo = new ThrowsWhenWrittenDictionary(_pdfWriter);
    }

    /// <summary>
    /// The document's title
    /// </summary>
    public string? Title
    {
        get;
        set
        {
            _pdfWriter.ThrowsWhenEndingWritten();
            field = value;
        }
    }

    /// <summary>
    /// The name of the person who created the document
    /// </summary>
    public string? Author
    {
        get;
        set
        {
            _pdfWriter.ThrowsWhenEndingWritten();
            field = value;
        }
    }

    /// <summary>
    /// The subject of the document
    /// </summary>
    public string? Subject
    {
        get;
        set
        {
            _pdfWriter.ThrowsWhenEndingWritten();
            field = value;
        }
    }

    /// <summary>
    /// Keywords associated with the document
    /// </summary>
    public string? Keywords
    {
        get;
        set
        {
            _pdfWriter.ThrowsWhenEndingWritten();
            field = value;
        }
    }

    /// <summary>
    /// If the document was converted to PDF from another format, the name of the conforming product that created the original document from which it was converted. Otherwise the name of the application that created the document.
    /// </summary>
    public string? Creator
    {
        get;
        set
        {
            _pdfWriter.ThrowsWhenEndingWritten();
            field = value;
        }
    }

    /// <summary>
    /// If the document was converted to PDF from another format, the name of the conforming product that converted it to PDF.
    /// </summary>
    public string? Producer
    {
        get;
        set
        {
            _pdfWriter.ThrowsWhenEndingWritten();
            field = value;
        }
    }

    /// <summary>
    /// The date and time the document was created, in human-readable form.
    /// </summary>
    public DateTimeOffset? CreationDate
    {
        get;
        set
        {
            _pdfWriter.ThrowsWhenEndingWritten();
            field = value;
        }
    } = DateTimeOffset.Now;

    /// <summary>
    /// The date and time the document was most recently modified, in human-readable form.
    /// </summary>
    public DateTimeOffset? ModDate
    {
        get;
        set
        {
            _pdfWriter.ThrowsWhenEndingWritten();
            field = value;
        }
    }

    /// <summary>
    /// Extra information that will be added to the PDF meta data
    /// </summary>
    public IDictionary<string, string> ExtraInfo { get; }
}
