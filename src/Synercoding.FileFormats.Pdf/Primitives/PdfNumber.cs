using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Primitives;

/// <summary>
/// Represents a PDF numeric primitive value, which can be either an integer or real number.
/// </summary>
[DebuggerDisplay("Pdf({ToString(),nq})")]
public readonly struct PdfNumber : IPdfPrimitive, IEquatable<PdfNumber>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PdfNumber"/> struct with a double value.
    /// </summary>
    /// <param name="value">The numeric value.</param>
    public PdfNumber(double value)
    {
        _doubleValue = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PdfNumber"/> struct with an integer value.
    /// </summary>
    /// <param name="value">The numeric value.</param>
    public PdfNumber(int value)
    {
        _longValue = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PdfNumber"/> struct with a long value.
    /// </summary>
    /// <param name="value">The numeric value.</param>
    public PdfNumber(long value)
    {
        _longValue = value;
    }

    private readonly double? _doubleValue;
    private readonly long? _longValue;

    /// <summary>
    /// Gets the numeric value as a double.
    /// </summary>
    public double Value
        => _doubleValue ?? _longValue ?? default;

    internal long LongValue
        => _longValue ?? (long)( _doubleValue ?? default );

    internal bool IsFractional
        => _doubleValue.HasValue;

    /// <inheritdoc />
    public bool Equals(PdfNumber other)
        => ( _doubleValue.HasValue && other._doubleValue.HasValue && _doubleValue == other._doubleValue )
        || ( _longValue.HasValue && other._longValue.HasValue && _longValue == other._longValue )
        || ( _doubleValue.HasValue && other._longValue.HasValue && _doubleValue == other._longValue )
        || ( _longValue.HasValue && other._doubleValue.HasValue && _longValue == other._doubleValue )
        || ( !_longValue.HasValue && !_doubleValue.HasValue );

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is PdfNumber pdfReal && Equals(pdfReal);

    /// <inheritdoc />
    public override int GetHashCode()
        => HashCode.Combine(_doubleValue, _longValue);

    /// <inheritdoc />
    [DebuggerStepThrough]
    public override string ToString()
        => _doubleValue.HasValue
        ? $"{Value}"
        : _longValue.HasValue
        ? $"{LongValue}"
        : "default struct";

    /// <summary>
    /// Implicitly converts a <see cref="PdfNumber"/> to a <see cref="double"/>.
    /// </summary>
    /// <param name="r">The PDF number to convert.</param>
    public static implicit operator double(PdfNumber r) => r.Value;

    /// <summary>
    /// Implicitly converts a <see cref="PdfNumber"/> to a <see cref="float"/>.
    /// </summary>
    /// <param name="r">The PDF number to convert.</param>
    public static implicit operator float(PdfNumber r) => (float)r.Value;

    /// <summary>
    /// Implicitly converts a <see cref="PdfNumber"/> to an <see cref="int"/>.
    /// </summary>
    /// <param name="r">The PDF number to convert.</param>
    public static implicit operator int(PdfNumber r) => (int)r.LongValue;

    /// <summary>
    /// Implicitly converts a <see cref="PdfNumber"/> to a <see cref="long"/>.
    /// </summary>
    /// <param name="r">The PDF number to convert.</param>
    public static implicit operator long(PdfNumber r) => r.LongValue;

    /// <summary>
    /// Explicitly converts a <see cref="double"/> to a <see cref="PdfNumber"/>.
    /// </summary>
    /// <param name="d">The double value to convert.</param>
    public static explicit operator PdfNumber(double d) => new PdfNumber(d);

    /// <summary>
    /// Explicitly converts a <see cref="float"/> to a <see cref="PdfNumber"/>.
    /// </summary>
    /// <param name="f">The float value to convert.</param>
    public static explicit operator PdfNumber(float f) => new PdfNumber(f);

    /// <summary>
    /// Explicitly converts an <see cref="int"/> to a <see cref="PdfNumber"/>.
    /// </summary>
    /// <param name="i">The integer value to convert.</param>
    public static explicit operator PdfNumber(int i) => new PdfNumber(i);

    /// <summary>
    /// Explicitly converts a <see cref="long"/> to a <see cref="PdfNumber"/>.
    /// </summary>
    /// <param name="i">The long value to convert.</param>
    public static explicit operator PdfNumber(long i) => new PdfNumber(i);

    /// <summary>
    /// Determines whether two <see cref="PdfNumber"/> instances are equal.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public static bool operator ==(PdfNumber left, PdfNumber right)
        => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="PdfNumber"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns>True if the instances are not equal; otherwise, false.</returns>
    public static bool operator !=(PdfNumber left, PdfNumber right)
        => !( left == right );
}
