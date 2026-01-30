using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Primitives;

/// <summary>
/// Represents a PDF object identifier consisting of an object number and generation number.
/// </summary>
[DebuggerDisplay("Pdf({ToString(),nq})")]
public readonly struct PdfObjectId : IEquatable<PdfObjectId>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PdfObjectId"/> struct with generation 0.
    /// </summary>
    /// <param name="objectNumber">The object number.</param>
    public PdfObjectId(int objectNumber)
        : this(objectNumber, 0)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PdfObjectId"/> struct.
    /// </summary>
    /// <param name="objectNumber">The object number.</param>
    /// <param name="generation">The generation number.</param>
    public PdfObjectId(int objectNumber, int generation)
        : this(objectNumber, generation, false)
    { }

    internal PdfObjectId(int objectNumber, int generation, bool allowObjectZero)
    {
        if (allowObjectZero && objectNumber < 0)
            throw new ArgumentOutOfRangeException(nameof(objectNumber), "Object number must be a non-negative integer.");
        else if (!allowObjectZero && objectNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(objectNumber), "Object number must be a positive integer.");

        if (generation < 0)
            throw new ArgumentOutOfRangeException(nameof(generation), "Generation number must be a non-negative integer.");

        ObjectNumber = objectNumber;
        Generation = generation;
    }

    /// <summary>
    /// Gets the object number.
    /// </summary>
    public int ObjectNumber { get; init; }

    /// <summary>
    /// Gets the generation number.
    /// </summary>
    public int Generation { get; init; }

    /// <summary>
    /// Creates a <see cref="PdfReference"/> from this object ID.
    /// </summary>
    /// <returns>A new PDF reference pointing to this object.</returns>
    public PdfReference GetReference()
        => new PdfReference(this);

    /// <inheritdoc />
    public bool Equals(PdfObjectId other)
        => ObjectNumber == other.ObjectNumber && Generation == other.Generation;

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is PdfObjectId pdfObjId && Equals(pdfObjId);

    /// <inheritdoc />
    public override int GetHashCode()
        => HashCode.Combine(ObjectNumber, Generation);

    /// <inheritdoc />
    [DebuggerStepThrough]
    public override string ToString()
        => $"{ObjectNumber} {Generation}";

    /// <summary>
    /// Determines whether two <see cref="PdfObjectId"/> instances are equal.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public static bool operator ==(PdfObjectId left, PdfObjectId right)
        => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="PdfObjectId"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns>True if the instances are not equal; otherwise, false.</returns>
    public static bool operator !=(PdfObjectId left, PdfObjectId right)
        => !( left == right );
}
