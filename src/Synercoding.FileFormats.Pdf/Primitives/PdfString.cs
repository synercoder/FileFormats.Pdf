using System.Diagnostics;

namespace Synercoding.FileFormats.Pdf.Primitives;

[DebuggerDisplay("{ToString(),nq}")]
public class PdfString : IPdfPrimitive, IEquatable<PdfString>
{
    public PdfString(string value, PdfStringEncoding encoding, bool isHex)
    {
        IsHex = isHex;
        Value = value;
        Encoding = encoding;
    }

    public string Value { get; }

    public PdfStringEncoding Encoding { get; }

    public bool IsHex { get; }

    public bool Equals(PdfString? other)
    {
        if (other is null)
            return false;

        return IsHex == other.IsHex
            && Encoding == other.Encoding
            && Value == other.Value;
    }

    public override bool Equals(object? obj)
        => Equals(obj as PdfString);

    public override int GetHashCode()
        => HashCode.Combine(IsHex, Encoding, Value);

    public override string ToString()
        => $"[Pdf {( IsHex ? "hex" : "literal" )} {Encoding} string] \"{Value}\"";

    public static bool operator ==(PdfString left, PdfString right)
        => left.Equals(right);

    public static bool operator !=(PdfString left, PdfString right)
        => !( left == right );
}
