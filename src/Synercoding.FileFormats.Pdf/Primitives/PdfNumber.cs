using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Primitives;

[DebuggerDisplay("[PdfNumber] {ToString(),nq}")]
public readonly struct PdfNumber : IPdfPrimitive, IEquatable<PdfNumber>
{
    public PdfNumber(double value)
    {
        _doubleValue = value;
    }

    public PdfNumber(int value)
    {
        _longValue = value;
    }

    public PdfNumber(long value)
    {
        _longValue = value;
    }

    private readonly double? _doubleValue;
    private readonly long? _longValue;

    public double Value
        => _doubleValue ?? _longValue ?? default;

    internal long LongValue
        => _longValue ?? (long)(_doubleValue ?? default);

    public bool Equals(PdfNumber other)
        => ( _doubleValue.HasValue && other._doubleValue.HasValue && _doubleValue == other._doubleValue )
        || ( _longValue.HasValue && other._longValue.HasValue && _longValue == other._longValue )
        || ( _doubleValue.HasValue && other._longValue.HasValue && _doubleValue == other._longValue )
        || ( _longValue.HasValue && other._doubleValue.HasValue && _longValue == other._doubleValue )
        || (!_longValue.HasValue && !_doubleValue.HasValue);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is PdfNumber pdfReal && Equals(pdfReal);

    public override int GetHashCode()
        => HashCode.Combine(_doubleValue, _longValue);

    [DebuggerStepThrough]
    public override string ToString()
        => _doubleValue.HasValue
        ? $"{Value}"
        : _longValue.HasValue
        ? $"{LongValue}"
        : "default struct";

    public static implicit operator double(PdfNumber r) => r.Value;

    public static implicit operator float(PdfNumber r) => (float)r.Value;

    public static implicit operator int(PdfNumber r) => (int)r.LongValue;

    public static implicit operator long(PdfNumber r) => (long)r.LongValue;

    public static explicit operator PdfNumber(double d) => new PdfNumber(d);

    public static explicit operator PdfNumber(float f) => new PdfNumber(f);

    public static explicit operator PdfNumber(int i) => new PdfNumber(i);

    public static explicit operator PdfNumber(long i) => new PdfNumber(i);

    public static bool operator ==(PdfNumber left, PdfNumber right)
        => left.Equals(right);

    public static bool operator !=(PdfNumber left, PdfNumber right)
        => !( left == right );
}
