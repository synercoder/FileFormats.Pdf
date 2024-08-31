using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Primitives;

public readonly struct PdfObjectId : IEquatable<PdfObjectId>
{
    public int Id { get; init; }
    public int Generation { get; init; }

    public bool Equals(PdfObjectId other)
        => Id == other.Id && Generation == other.Generation;

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is PdfObjectId pdfObjId && Equals(pdfObjId);

    public override int GetHashCode()
        => HashCode.Combine(Id, Generation);

    [DebuggerStepThrough]
    public override string ToString()
        => $"[Pdf Id] {Id} {Generation}";
}
