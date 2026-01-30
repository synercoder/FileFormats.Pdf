namespace Synercoding.FileFormats.Pdf.Primitives;

/// <summary>
/// Represents a read-only PDF array primitive.
/// </summary>
public interface IPdfArray : IPdfPrimitive, IReadOnlyCollection<IPdfPrimitive>
{
    /// <summary>
    /// Gets the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The element at the specified index.</returns>
    IPdfPrimitive this[int index] { get; }
}
