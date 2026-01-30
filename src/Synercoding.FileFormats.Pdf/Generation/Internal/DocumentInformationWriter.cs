using Synercoding.FileFormats.Pdf.DocumentObjects;
using Synercoding.FileFormats.Pdf.Parsing;
using Synercoding.FileFormats.Pdf.Primitives;
using System.Globalization;

namespace Synercoding.FileFormats.Pdf.Generation.Internal;

/// <summary>
/// Writes the document information dictionary
/// </summary>
internal class DocumentInformationWriter
{
    private readonly ObjectWriter _objectWriter;

    public DocumentInformationWriter(ObjectWriter objectWriter)
    {
        _objectWriter = objectWriter ?? throw new ArgumentNullException(nameof(objectWriter));
    }

    /// <summary>
    /// Writes the info dictionary if it contains any data, returns reference or null
    /// </summary>
    public PdfReference? WriteIfNeeded(DocumentInformation info)
    {
        var hasData = !string.IsNullOrEmpty(info.Title) ||
                     !string.IsNullOrEmpty(info.Author) ||
                     !string.IsNullOrEmpty(info.Subject) ||
                     !string.IsNullOrEmpty(info.Keywords) ||
                     !string.IsNullOrEmpty(info.Creator) ||
                     !string.IsNullOrEmpty(info.Producer) ||
                     info.CreationDate.HasValue ||
                     info.ModDate.HasValue ||
                     info.ExtraInfo.Count > 0;

        if (!hasData)
            return null;

        var infoId = _objectWriter.TableBuiler.ReserveId();
        var infoDict = new PdfDictionary();

        if (!string.IsNullOrEmpty(info.Title))
            infoDict[PdfNames.Title] = new PdfString(PDFDocEncoding.Encode(info.Title), false);
        if (!string.IsNullOrEmpty(info.Author))
            infoDict[PdfNames.Author] = new PdfString(PDFDocEncoding.Encode(info.Author), false);
        if (!string.IsNullOrEmpty(info.Subject))
            infoDict[PdfNames.Subject] = new PdfString(PDFDocEncoding.Encode(info.Subject), false);
        if (!string.IsNullOrEmpty(info.Keywords))
            infoDict[PdfNames.Keywords] = new PdfString(PDFDocEncoding.Encode(info.Keywords), false);
        if (!string.IsNullOrEmpty(info.Creator))
            infoDict[PdfNames.Creator] = new PdfString(PDFDocEncoding.Encode(info.Creator), false);
        if (!string.IsNullOrEmpty(info.Producer))
            infoDict[PdfNames.Producer] = new PdfString(PDFDocEncoding.Encode(info.Producer), false);

        if (info.CreationDate.HasValue)
            infoDict[PdfNames.CreationDate] = _formatDateTimeOffset(info.CreationDate.Value);
        if (info.ModDate.HasValue)
            infoDict[PdfNames.ModDate] = _formatDateTimeOffset(info.ModDate.Value);

        foreach (var extra in info.ExtraInfo)
            infoDict[PdfName.Get(extra.Key)] = new PdfString(PDFDocEncoding.Encode(extra.Value), false);

        _objectWriter.Write(new PdfObject<PdfDictionary>
        {
            Id = infoId,
            Value = infoDict
        });

        return infoId.GetReference();
    }

    private static PdfString _formatDateTimeOffset(DateTimeOffset dateTime)
    {
        var offset = dateTime.Offset;

        var formattedDate = dateTime.ToString("'D:'yyyyMMddHHmmss", CultureInfo.InvariantCulture);

        return new PdfString(PDFDocEncoding.Encode($"{formattedDate}{_formatOffset(offset)}"), false);
    }

    private static string _formatOffset(TimeSpan offset)
    {
        if (offset == TimeSpan.Zero)
            return "Z";

        var offsetSign = offset >= TimeSpan.Zero ? "+" : "-";
        var absOffset = Math.Abs(offset.Hours);
        var offsetMinutes = Math.Abs(offset.Minutes);
        return $"{offsetSign}{absOffset:D2}'{offsetMinutes:D2}";
    }
}
