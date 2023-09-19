namespace Synercoding.FileFormats.Pdf.LowLevel.Colors.ColorSpaces;

/// <summary>
/// Class representing a colorspace
/// </summary>
public abstract class ColorSpace : IEquatable<ColorSpace>
{
    /// <summary>
    /// The number of components for this colorspace
    /// </summary>
    public abstract int Components { get; }

    /// <summary>
    /// The name of this colorspace
    /// </summary>
    public abstract PdfName Name { get; }

    /// <inheritdoc />
    public abstract bool Equals(ColorSpace? other);

    /// <inheritdoc />
    public abstract override bool Equals(object? obj);

    /// <inheritdoc />
    public abstract override int GetHashCode();
}
