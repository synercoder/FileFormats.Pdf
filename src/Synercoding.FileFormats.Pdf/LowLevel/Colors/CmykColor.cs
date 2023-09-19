using Synercoding.FileFormats.Pdf.LowLevel.Colors.ColorSpaces;

namespace Synercoding.FileFormats.Pdf.LowLevel.Colors;

/// <summary>
/// Class representing a CMYK color
/// </summary>
public sealed class CmykColor : Color, IEquatable<CmykColor>
{
    private const string COLOR_COMPONENT_OUT_OF_RANGE = "Color component value must be between 0.0 (zero concentration) and 1.0 (maximum concentration).";

    private readonly double _cyan = 0;
    private readonly double _magenta = 0;
    private readonly double _yellow = 0;
    private readonly double _key = 0;

    /// <summary>
    /// Constructor for a <see cref="CmykColor"/>
    /// </summary>
    public CmykColor()
    { }

    /// <summary>
    /// Constructor for a <see cref="CmykColor"/>
    /// </summary>
    /// <param name="cyan">The cyan component of the color</param>
    /// <param name="magenta">The magenta component of the color</param>
    /// <param name="yellow">The yellow component of the color</param>
    /// <param name="key">The key component of the color</param>
    public CmykColor(double cyan, double magenta, double yellow, double key)
    {
        Cyan = cyan;
        Magenta = magenta;
        Yellow = yellow;
        Key = key;
    }

    /// <summary>
    /// The cyan component of this color
    /// </summary>
    /// <remarks>Value must be between 0.0 (zero concentration) and 1.0 (maximum concentration).</remarks>
    public double Cyan
    {
        get => _cyan;
        init
        {
            if (value is < 0 or > 1)
                throw new ArgumentOutOfRangeException(nameof(Cyan), COLOR_COMPONENT_OUT_OF_RANGE);

            _cyan = value;
        }
    }

    /// <summary>
    /// The magenta component of this color
    /// </summary>
    /// <remarks>Value must be between 0.0 (zero concentration) and 1.0 (maximum concentration).</remarks>
    public double Magenta
    {
        get => _magenta;
        init
        {
            if (value is < 0 or > 1)
                throw new ArgumentOutOfRangeException(nameof(Magenta), COLOR_COMPONENT_OUT_OF_RANGE);

            _magenta = value;
        }
    }

    /// <summary>
    /// The yellow component of this color
    /// </summary>
    /// <remarks>Value must be between 0.0 (zero concentration) and 1.0 (maximum concentration).</remarks>
    public double Yellow
    {
        get => _yellow;
        init
        {
            if (value is < 0 or > 1)
                throw new ArgumentOutOfRangeException(nameof(Yellow), COLOR_COMPONENT_OUT_OF_RANGE);

            _yellow = value;
        }
    }

    /// <summary>
    /// The key component of this color
    /// </summary>
    /// <remarks>Value must be between 0.0 (zero concentration) and 1.0 (maximum concentration).</remarks>
    public double Key
    {
        get => _key;
        init
        {
            if (value is < 0 or > 1)
                throw new ArgumentOutOfRangeException(nameof(Key), COLOR_COMPONENT_OUT_OF_RANGE);

            _key = value;
        }
    }

    /// <inheritdoc />
    public override ColorSpace Colorspace
        => DeviceCMYK.Instance;

    /// <inheritdoc />
    public override double[] Components => new double[] { Cyan, Magenta, Yellow, Key };

    /// <inheritdoc />
    public bool Equals(CmykColor? other)
    {
        return other is not null
            && Cyan == other.Cyan
            && Magenta == other.Magenta
            && Yellow == other.Yellow
            && Key == other.Key;
    }

    /// <inheritdoc />
    public override bool Equals(Color? other)
        => Equals(other as CmykColor);

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => Equals(obj as CmykColor);

    /// <inheritdoc />
    public override int GetHashCode()
        => HashCode.Combine(Cyan, Magenta, Yellow, Key);
}
