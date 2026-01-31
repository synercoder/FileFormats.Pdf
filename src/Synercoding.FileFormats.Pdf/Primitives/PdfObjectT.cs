namespace Synercoding.FileFormats.Pdf.Primitives;

/// <summary>
/// Represents a PDF object with its object ID and typed value.
/// </summary>
/// <typeparam name="T">The type of the PDF primitive value.</typeparam>
public class PdfObject<T>
    where T : IPdfPrimitive
{
    /// <summary>
    /// Gets the object ID of this PDF object.
    /// </summary>
    public required PdfObjectId Id { get; init; }

    /// <summary>
    /// Gets the value of this PDF object.
    /// </summary>
    public required T Value { get; init; }
}
