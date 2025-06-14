namespace Synercoding.FileFormats.Pdf.Primitives;
public class PdfObject<T>
    where T : IPdfPrimitive
{
    public required PdfObjectId Id { get; init; }
    public required T Value { get; init; }
}
