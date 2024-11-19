using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.LowLevel;

/// <summary>
/// A struct representing a reference
/// </summary>
public readonly struct PdfReference : IEquatable<PdfReference>
{
    /// <summary>
    /// Constructor for <see cref="PdfReference"/> that uses generation 0
    /// </summary>
    /// <param name="objectId">The id of the reference</param>
    internal PdfReference(int objectId)
        : this(objectId, 0)
    { }

    /// <summary>
    /// Constructor for <see cref="PdfReference"/>
    /// </summary>
    /// <param name="objectId">The id of the reference</param>
    /// <param name="generation">The generation of the reference</param>
    internal PdfReference(int objectId, int generation)
    {
        ObjectId = objectId;
        Generation = generation;
    }

    /// <summary>
    /// The object id of this reference
    /// </summary>
    public int ObjectId { get; }

    /// <summary>
    /// The generation of this reference
    /// </summary>
    public int Generation { get; }

    public bool Equals(PdfReference other)
        => ObjectId == other.ObjectId && Generation == other.Generation;

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is PdfReference pdfRef && Equals(pdfRef);

    public override int GetHashCode()
        => HashCode.Combine(ObjectId, Generation);

    /// <inheritdoc />
    public override string ToString()
        => $"{ObjectId} {Generation}";

    public static bool operator ==(PdfReference left, PdfReference right)
        => left.Equals(right);

    public static bool operator !=(PdfReference left, PdfReference right) => !( left == right );
}
