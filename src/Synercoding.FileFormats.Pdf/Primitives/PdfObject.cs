namespace Synercoding.FileFormats.Pdf.Primitives;

public class PdfObject
{
    public required PdfObjectId Id { get; init; }
    public required IPdfPrimitive Value { get; init; }
}
