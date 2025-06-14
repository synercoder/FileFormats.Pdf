using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Primitives;

[DebuggerDisplay("[Pdf Id]  {ToString(),nq}")]
public readonly struct PdfObjectId : IEquatable<PdfObjectId>
{
    public PdfObjectId(int objectNumber, int generation)
        : this(objectNumber, generation, false)
    { }
    internal PdfObjectId(int objectNumber, int generation, bool allowObjectZero)
    {
        if (!allowObjectZero && objectNumber == 0)
            throw new ArgumentOutOfRangeException(nameof(objectNumber), "Object number must be a non-negative integer.");
        else if (!allowObjectZero && objectNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(objectNumber), "Object number must be a positive integer.");
        if (generation < 0)
            throw new ArgumentOutOfRangeException(nameof(generation), "Generation number must be a non-negative integer.");

        ObjectNumber = objectNumber;
        Generation = generation;
    }

    public int ObjectNumber { get; init; }
    public int Generation { get; init; }

    public bool Equals(PdfObjectId other)
        => ObjectNumber == other.ObjectNumber && Generation == other.Generation;

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is PdfObjectId pdfObjId && Equals(pdfObjId);

    public override int GetHashCode()
        => HashCode.Combine(ObjectNumber, Generation);

    [DebuggerStepThrough]
    public override string ToString()
        => $"{ObjectNumber} {Generation}";

    public static bool operator ==(PdfObjectId left, PdfObjectId right)
        => left.Equals(right);

    public static bool operator !=(PdfObjectId left, PdfObjectId right)
        => !( left == right );
}
