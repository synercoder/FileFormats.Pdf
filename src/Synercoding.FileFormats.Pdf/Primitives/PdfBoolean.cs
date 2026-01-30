using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Primitives;

/// <summary>
/// Represents a PDF boolean primitive value.
/// </summary>
[DebuggerDisplay("Pdf({ToString(),nq})")]
public readonly struct PdfBoolean : IPdfPrimitive, IEquatable<PdfBoolean>
{
    /// <summary>
    /// Gets a <see cref="PdfBoolean"/> instance representing true.
    /// </summary>
    public static PdfBoolean True { get; } = new PdfBoolean(true);

    /// <summary>
    /// Gets a <see cref="PdfBoolean"/> instance representing false.
    /// </summary>
    public static PdfBoolean False { get; } = new PdfBoolean(false);

    /// <summary>
    /// Initializes a new instance of the <see cref="PdfBoolean"/> struct.
    /// </summary>
    /// <param name="value">The boolean value.</param>
    public PdfBoolean(bool value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the boolean value.
    /// </summary>
    public bool Value { get; init; }

    /// <inheritdoc />
    public bool Equals(PdfBoolean other)
        => Value == other.Value;

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is PdfBoolean pdfBool && Equals(pdfBool);

    /// <inheritdoc />
    public override int GetHashCode()
        => Value.GetHashCode();

    /// <inheritdoc />
    [DebuggerStepThrough]
    public override string ToString()
        => $"{Value}";

    /// <summary>
    /// Implicitly converts a <see cref="PdfBoolean"/> to a <see cref="bool"/>.
    /// </summary>
    /// <param name="d">The PDF boolean to convert.</param>
    public static implicit operator bool(PdfBoolean d) => d.Value;

    /// <summary>
    /// Explicitly converts a <see cref="bool"/> to a <see cref="PdfBoolean"/>.
    /// </summary>
    /// <param name="b">The boolean value to convert.</param>
    public static explicit operator PdfBoolean(bool b) => new PdfBoolean(b);

    /// <summary>
    /// Determines whether two <see cref="PdfBoolean"/> instances are equal.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public static bool operator ==(PdfBoolean left, PdfBoolean right)
        => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="PdfBoolean"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns>True if the instances are not equal; otherwise, false.</returns>
    public static bool operator !=(PdfBoolean left, PdfBoolean right)
        => !( left == right );
}
