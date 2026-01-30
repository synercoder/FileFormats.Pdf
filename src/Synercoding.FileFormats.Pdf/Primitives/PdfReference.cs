using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Primitives;

/// <summary>
/// Represents a reference to a PDF object, consisting of an object ID.
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
public readonly struct PdfReference : IPdfPrimitive, IEquatable<PdfReference>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PdfReference"/> struct.
    /// </summary>
    /// <param name="id">The object ID being referenced.</param>
    public PdfReference(PdfObjectId id)
    {
        Id = id;
    }

    /// <summary>
    /// Gets the object ID being referenced.
    /// </summary>
    public PdfObjectId Id { get; init; }

    /// <inheritdoc />
    public bool Equals(PdfReference other)
        => Id.Equals(other.Id);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is PdfReference pdfRef && Equals(pdfRef);

    /// <inheritdoc />
    public override int GetHashCode()
        => HashCode.Combine(Id.ObjectNumber, Id.Generation);

    /// <inheritdoc />
    [DebuggerStepThrough]
    public override string ToString()
        => $"{Id} R";

    /// <summary>
    /// Determines whether two <see cref="PdfReference"/> instances are equal.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public static bool operator ==(PdfReference left, PdfReference right)
        => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="PdfReference"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns>True if the instances are not equal; otherwise, false.</returns>
    public static bool operator !=(PdfReference left, PdfReference right)
        => !( left == right );
}
