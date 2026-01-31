namespace Synercoding.FileFormats.Pdf.Generation.Internal;

/// <summary>
/// Writes the cross-reference table for a PDF document
/// </summary>
internal class CrossReferenceTableWriter
{
    private readonly TableBuilder _tableBuilder;

    public CrossReferenceTableWriter(TableBuilder tableBuilder)
    {
        _tableBuilder = tableBuilder ?? throw new ArgumentNullException(nameof(tableBuilder));
    }

    /// <summary>
    /// Writes the cross-reference table to the stream and returns the byte position where it started
    /// </summary>
    public long WriteTo(PdfStream stream)
    {
        var xrefPosition = stream.Position;

        // Write "xref" keyword
        stream.Write("xref").NewLine();

        // Get all subsections from TableBuilder
        var subsections = _tableBuilder.GetSubsections();

        foreach (var subsection in subsections)
        {
            // Write subsection header: "firstId count"
            stream.Write(subsection.FirstObjectNumber)
                  .Space()
                  .Write(subsection.Count)
                  .NewLine();

            // Write each entry in format: "0000000123 00000 n "
            foreach (var entry in subsection.Entries)
            {
                stream.Write(entry.ByteOffset.ToString("D10"))
                      .Space()
                      .Write(entry.Generation.ToString("D5"))
                      .Space()
                      .Write(entry.InUse ? 'n' : 'f')
                      .NewLine();
            }
        }

        return xrefPosition;
    }
}
