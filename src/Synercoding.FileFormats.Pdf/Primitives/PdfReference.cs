using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Primitives;

[DebuggerDisplay("[Pdf Reference] {ToString(),nq}")]
public readonly struct PdfReference : IPdfPrimitive, IEquatable<PdfReference>
{
    public PdfReference(PdfObjectId id)
    {
        Id = id;
    }

    public PdfObjectId Id { get; init; }

    public bool Equals(PdfReference other)
        => Id.Equals(other.Id);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is PdfReference pdfRef && Equals(pdfRef);

    public override int GetHashCode()
        => HashCode.Combine(Id.ObjectNumber, Id.Generation);

    [DebuggerStepThrough]
    public override string ToString()
        => $"{Id} R";

    public static bool operator ==(PdfReference left, PdfReference right)
        => left.Equals(right);

    public static bool operator !=(PdfReference left, PdfReference right)
        => !( left == right );
}
