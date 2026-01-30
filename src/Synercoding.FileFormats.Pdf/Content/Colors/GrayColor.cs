using Synercoding.FileFormats.Pdf.Content.Colors.ColorSpaces;

namespace Synercoding.FileFormats.Pdf.Content.Colors;

/// <summary>
/// Class representing a gray color
/// </summary>
public sealed class GrayColor : Color, IEquatable<GrayColor>
{
    /// <summary>
    /// Construct a new <see cref="GrayColor"/> object
    /// </summary>
    /// <param name="gray"></param>
    public GrayColor(double gray)
    {
        Gray = gray;
    }

    /// <summary>
    /// The single component that represents the gray color. 
    /// </summary>
    /// <remarks>
    /// 0.0 = Black
    /// 1.0 = white
    /// </remarks>
    public double Gray
    {
        get;
        init
        {
            if (value is < 0 or > 1)
                throw new ArgumentOutOfRangeException(nameof(Gray), "Gray value must be between 0.0 (black) and 1.0 (white).");

            field = value;
        }
    }

    /// <inheritdoc />
    public override ColorSpace Colorspace
        => DeviceGray.Instance;

    /// <inheritdoc />
    public override double[] Components => new double[] { Gray };

    /// <inheritdoc />
    public bool Equals(GrayColor? other)
    {
        return other is not null
            && Gray == other.Gray;
    }

    /// <inheritdoc />
    public override bool Equals(Color? other)
        => Equals(other as GrayColor);

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => Equals(obj as GrayColor);

    /// <inheritdoc />
    public override int GetHashCode()
        => HashCode.Combine(Gray);
}
