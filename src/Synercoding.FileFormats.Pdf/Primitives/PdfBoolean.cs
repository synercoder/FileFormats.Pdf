using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Primitives;

[DebuggerDisplay("{ToString(),nq}")]
public readonly struct PdfBoolean : IPdfPrimitive, IEquatable<PdfBoolean>
{
    public static PdfBoolean True { get; } = new PdfBoolean(true);
    public static PdfBoolean False { get; } = new PdfBoolean(false);

    public PdfBoolean(bool value)
    {
        Value = value;
    }

    public bool Value { get; init; }

    public bool Equals(PdfBoolean other)
        => Value == other.Value;

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is PdfBoolean pdfBool && Equals(pdfBool);

    public override int GetHashCode()
        => Value.GetHashCode();

    [DebuggerStepThrough]
    public override string ToString()
        => $"[Pdf Boolean] {Value}";

    public static implicit operator bool(PdfBoolean d) => d.Value;

    public static explicit operator PdfBoolean(bool b) => new PdfBoolean(b);

    public static bool operator ==(PdfBoolean left, PdfBoolean right)
        => left.Equals(right);

    public static bool operator !=(PdfBoolean left, PdfBoolean right)
        => !( left == right );
}
