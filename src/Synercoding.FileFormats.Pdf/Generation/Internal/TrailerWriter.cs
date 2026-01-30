using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Generation.Internal;

/// <summary>
/// Writes the PDF trailer dictionary and final PDF structure
/// </summary>
internal class TrailerWriter
{
    private readonly TableBuilder _tableBuilder;
    private readonly WriterSettings _settings;

    public TrailerWriter(TableBuilder tableBuilder, WriterSettings settings)
    {
        _tableBuilder = tableBuilder ?? throw new ArgumentNullException(nameof(tableBuilder));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    /// <summary>
    /// Writes the trailer dictionary, startxref, and %%EOF
    /// </summary>
    public void WriteTo(PdfStream stream, PdfReference catalogRef, PdfReference? infoRef, long xrefPosition)
    {
        var trailerDict = new PdfDictionary
        {
            [PdfNames.Size] = new PdfNumber(_tableBuilder.Count + 1), // +1 for 0 object
            [PdfNames.Root] = catalogRef
        };

        if (infoRef.HasValue)
            trailerDict[PdfNames.Info] = infoRef.Value;

        // Write trailer
        stream.Write("trailer").NewLine();
        var serializer = new DirectObjectSerializer(stream);
        serializer.WriteDirect(trailerDict);
        stream.NewLine();

        // Write startxref
        stream.Write("startxref").NewLine();
        stream.Write(xrefPosition).NewLine();

        // Write EOF marker
        stream.Write("%%EOF");
    }
}