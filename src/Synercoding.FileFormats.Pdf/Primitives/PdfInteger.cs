using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Primitives;

[DebuggerDisplay("{ToString(),nq}")]
public readonly struct PdfInteger : IPdfPrimitive, IEquatable<PdfInteger>
{
    public PdfInteger(long value)
    {
        Value = value;
    }

    public long Value { get; }

    public bool Equals(PdfInteger other)
        => Value == other.Value;

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is PdfInteger pdfInteger && Equals(pdfInteger);

    public override int GetHashCode()
        => Value.GetHashCode();

    [DebuggerStepThrough]
    public override string ToString()
        => $"[Pdf Integer] {Value}";

    public static implicit operator long(PdfInteger i) => i.Value;

    public static implicit operator int(PdfInteger i) => (int)i.Value;

    public static explicit operator PdfInteger(long l) => new PdfInteger(l);

    public static explicit operator PdfInteger(int i) => new PdfInteger(i);
}