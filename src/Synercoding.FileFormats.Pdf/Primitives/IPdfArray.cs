namespace Synercoding.FileFormats.Pdf.Primitives;

public interface IPdfArray : IPdfPrimitive, IReadOnlyCollection<IPdfPrimitive>
{
    IPdfPrimitive this[int index] { get; }
}
