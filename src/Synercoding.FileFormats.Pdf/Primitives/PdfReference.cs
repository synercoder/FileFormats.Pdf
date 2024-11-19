using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Primitives;

[DebuggerDisplay("{ToString(),nq}")]
public readonly struct PdfReference : IPdfPrimitive, IEquatable<PdfReference>
{
    public PdfObjectId Id { get; init; }

    public bool Equals(PdfReference other)
        => Id.Equals(other.Id);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is PdfReference pdfRef && Equals(pdfRef);

    public override int GetHashCode()
        => HashCode.Combine(Id.Id, Id.Generation);

    [DebuggerStepThrough]
    public override string ToString()
        => $"[Pdf Reference] {Id.Id} {Id.Generation} R";

    public static bool operator ==(PdfReference left, PdfReference right)
        => left.Equals(right);

    public static bool operator !=(PdfReference left, PdfReference right)
        => !( left == right );
}
