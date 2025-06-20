using Synercoding.FileFormats.Pdf.Parsing;
using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.FileFormats.Pdf.Primitives.Extensions;

namespace Synercoding.FileFormats.Pdf.DocumentObjects.Internal;
internal class ReadOnlyDocumentInformation : IReadOnlyDocumentInformation
{
    private readonly IPdfDictionary _pdfDictionary;
    private readonly ObjectReader _objectReader;

    public ReadOnlyDocumentInformation(IPdfDictionary pdfDictionary, ObjectReader objectReader)
    {
        _pdfDictionary = pdfDictionary;
        _objectReader = objectReader;

        Title = _pdfDictionary.TryGetValue<PdfString>(PdfNames.Title, objectReader, out var titleString)
            ? titleString.Value
            : null;

        Author = _pdfDictionary.TryGetValue<PdfString>(PdfNames.Author, objectReader, out var authorString)
            ? authorString.Value
            : null;

        Subject = _pdfDictionary.TryGetValue<PdfString>(PdfNames.Subject, objectReader, out var subjectString)
            ? subjectString.Value
            : null;

        Keywords = _pdfDictionary.TryGetValue<PdfString>(PdfNames.Keywords, objectReader, out var keywordsString)
            ? keywordsString.Value
            : null;

        Creator = _pdfDictionary.TryGetValue<PdfString>(PdfNames.Creator, objectReader, out var creatorString)
            ? creatorString.Value
            : null;

        Producer = _pdfDictionary.TryGetValue<PdfString>(PdfNames.Producer, objectReader, out var producerString)
            ? producerString.Value
            : null;

        CreationDate = _pdfDictionary.TryGetValue<PdfString>(PdfNames.CreationDate, objectReader, out var creationDateString)
            && creationDateString.TryParseAsDateTimeOffset(out var creationDate)
            ? creationDate
            : null;

        ModDate = _pdfDictionary.TryGetValue<PdfString>(PdfNames.ModDate, objectReader, out var modDateString)
            && modDateString.TryParseAsDateTimeOffset(out var modDate)
            ? modDate
            : null;

        var extraInfo = new Dictionary<string, string>();

        foreach (var (key, value) in pdfDictionary)
        {
            if (key == PdfNames.Title) continue;
            if (key == PdfNames.Author) continue;
            if (key == PdfNames.Subject) continue;
            if (key == PdfNames.Keywords) continue;
            if (key == PdfNames.Creator) continue;
            if (key == PdfNames.Producer) continue;
            if (key == PdfNames.CreationDate) continue;
            if (key == PdfNames.ModDate) continue;

            if (value is not PdfString valueString) continue;

            extraInfo.Add(key.Display, valueString.Value);
        }

        ExtraInfo = extraInfo;
    }

    public string? Title { get; }

    public string? Author { get; }

    public string? Subject { get; }

    public string? Keywords { get; }

    public string? Creator { get; }

    public string? Producer { get; }

    public DateTimeOffset? CreationDate { get; }

    public DateTimeOffset? ModDate { get; }

    public IReadOnlyDictionary<string, string>? ExtraInfo { get; }
}
