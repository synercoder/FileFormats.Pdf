using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Primitives;

[DebuggerDisplay("{ToString(),nq}")]
public readonly struct PdfReal : IPdfPrimitive, IEquatable<PdfReal>
{
    public PdfReal(double value)
    {
        Value = value;
    }

    public double Value { get; }

    public bool Equals(PdfReal other)
        => Value == other.Value;

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is PdfReal pdfInteger && Equals(pdfInteger);

    public override int GetHashCode()
        => Value.GetHashCode();

    [DebuggerStepThrough]
    public override string ToString()
        => $"[Real] {Value}";

    public static implicit operator double(PdfReal r) => r.Value;

    public static implicit operator float(PdfReal r) => (float)r.Value;

    public static explicit operator PdfReal(double d) => new PdfReal(d);

    public static explicit operator PdfReal(float f) => new PdfReal(f);
}