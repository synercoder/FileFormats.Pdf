using Synercoding.FileFormats.Pdf.Content.Colors.ColorSpaces;

namespace Synercoding.FileFormats.Pdf.Content.Colors;

/// <summary>
/// Class representing a RGB color
/// </summary>
public sealed class RgbColor : Color, IEquatable<RgbColor>
{
    private const string COLOR_COMPONENT_OUT_OF_RANGE = "Color component value must be between 0.0 (minimum intensity) and 1.0 (maximum intensity).";

    /// <summary>
    /// Constructor for a <see cref="RgbColor"/>
    /// </summary>
    public RgbColor()
    { }

    /// <summary>
    /// Constructor for a <see cref="RgbColor"/>
    /// </summary>
    /// <param name="red">The red component of the color</param>
    /// <param name="green">The green component of the color</param>
    /// <param name="blue">The blue component of the color</param>
    public RgbColor(double red, double green, double blue)
    {
        Red = red;
        Green = green;
        Blue = blue;
    }

    /// <summary>
    /// Construct a <see cref="RgbColor"/> from bytes values
    /// </summary>
    /// <param name="red">The red component of the color</param>
    /// <param name="green">The green component of the color</param>
    /// <param name="blue">The blue component of the color</param>
    public static RgbColor FromBytes(byte red, byte green, byte blue)
        => new RgbColor(red / 255d, green / 255d, blue / 255d);

    /// <summary>
    /// The red component of this color
    /// </summary>
    /// <remarks>Value must be between 0.0 (minimum intensity) and 1.0 (maximum intensity).</remarks>
    public double Red
    {
        get;
        init
        {
            if (value is < 0 or > 1)
                throw new ArgumentOutOfRangeException(nameof(Red), COLOR_COMPONENT_OUT_OF_RANGE);

            field = value;
        }
    }

    /// <summary>
    /// The green component of this color
    /// </summary>
    /// <remarks>Value must be between 0.0 (minimum intensity) and 1.0 (maximum intensity).</remarks>
    public double Green
    {
        get;
        init
        {
            if (value is < 0 or > 1)
                throw new ArgumentOutOfRangeException(nameof(Green), COLOR_COMPONENT_OUT_OF_RANGE);

            field = value;
        }
    }

    /// <summary>
    /// The blue component of this color
    /// </summary>
    /// <remarks>Value must be between 0.0 (minimum intensity) and 1.0 (maximum intensity).</remarks>
    public double Blue
    {
        get;
        init
        {
            if (value is < 0 or > 1)
                throw new ArgumentOutOfRangeException(nameof(Blue), COLOR_COMPONENT_OUT_OF_RANGE);

            field = value;
        }
    }

    /// <inheritdoc />
    public override double[] Components => new double[] { Red, Green, Blue };

    /// <inheritdoc />
    public override ColorSpace Colorspace
        => DeviceRGB.Instance;

    /// <inheritdoc />
    public bool Equals(RgbColor? other)
    {
        return other is not null
            && Red == other.Red
            && Green == other.Green
            && Blue == other.Blue;
    }

    /// <inheritdoc />
    public override bool Equals(Color? other)
        => Equals(other as RgbColor);

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => Equals(obj as RgbColor);

    /// <inheritdoc />
    public override int GetHashCode()
        => HashCode.Combine(Red, Green, Blue);
}
