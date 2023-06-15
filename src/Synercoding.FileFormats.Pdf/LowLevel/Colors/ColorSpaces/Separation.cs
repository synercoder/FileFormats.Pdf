namespace Synercoding.FileFormats.Pdf.LowLevel.Colors.ColorSpaces;

/// <summary>
/// Class representing a separation colorspace
/// </summary>
public sealed class Separation : ColorSpace, IEquatable<Separation>
{
    /// <summary>
    /// Constructor for <see cref="Separation"/>.
    /// </summary>
    /// <param name="name">The name of this separation</param>
    /// <param name="baseColor">The color this separation will be based on.</param>
    public Separation(PdfName name, Color baseColor)
    {
        Name = name;
        BasedOnColor = baseColor;
    }

    /// <summary>
    /// The color this separation is based upon.
    /// </summary>
    public Color BasedOnColor { get; }

    /// <inheritdoc />
    public override int Components => 1;

    /// <inheritdoc />
    public override PdfName Name { get; }

    /// <inheritdoc />
    public bool Equals(Separation? other)
        => other is not null
        && other.BasedOnColor.Equals(BasedOnColor)
        && other.Name.Equals(Name);

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => Equals(obj as Separation);

    /// <inheritdoc />
    public override bool Equals(ColorSpace? other)
        => Equals(other as Separation);

    /// <inheritdoc />
    public override int GetHashCode()
        => HashCode.Combine(BasedOnColor, Name);
}
