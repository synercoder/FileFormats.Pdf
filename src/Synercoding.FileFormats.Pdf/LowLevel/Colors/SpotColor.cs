using Synercoding.FileFormats.Pdf.LowLevel.Colors.ColorSpaces;
using System;

namespace Synercoding.FileFormats.Pdf.LowLevel.Colors;

/// <summary>
/// Class representing a spotcolor
/// </summary>
public class SpotColor : Color, IEquatable<SpotColor>
{
    /// <summary>
    /// Constructor for a <see cref="SpotColor"/>
    /// </summary>
    /// <param name="separation">The colorspace of this spotcolor</param>
    /// <param name="tint">The tint of this spotcolor</param>
    /// <exception cref="ArgumentOutOfRangeException">Throws if the <paramref name="tint"/> &lt; 0 or &gt; 1</exception>
    public SpotColor(Separation separation, double tint)
    {
        if (tint is < 0 or > 1)
            throw new ArgumentOutOfRangeException(nameof(tint), $"Parameter {nameof(tint)} must be between 0.0 and 1.0 (inclusive).");
        Separation = separation;
        Tint = tint;
    }

    /// <summary>
    /// The separation color space used
    /// </summary>
    public Separation Separation { get; }

    /// <summary>
    /// The tint of this spotcolor
    /// </summary>
    public double Tint { get; }

    /// <inheritdoc />
    public override ColorSpace Colorspace
        => Separation;

    /// <inheritdoc />
    public override double[] Components => new double[] { Tint };

    /// <inheritdoc />
    public bool Equals(SpotColor? other)
        => other is not null
        && other.Separation.Equals(Separation)
        && other.Tint == Tint;

    /// <inheritdoc />
    public override bool Equals(Color? other)
        => Equals(other as SpotColor);

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => Equals(obj as SpotColor);

    /// <inheritdoc />
    public override int GetHashCode()
        => HashCode.Combine(Separation, Tint);
}
