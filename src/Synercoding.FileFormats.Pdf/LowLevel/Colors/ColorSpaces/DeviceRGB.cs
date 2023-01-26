using System;

namespace Synercoding.FileFormats.Pdf.LowLevel.Colors.ColorSpaces;

/// <summary>
/// Class representing DeviceRGB colorspace
/// </summary>
public sealed class DeviceRGB : ColorSpace, IEquatable<DeviceRGB>
{
    private DeviceRGB() { }

    /// <summary>
    /// Instance of <see cref="DeviceRGB"/> class
    /// </summary>
    public static DeviceRGB Instance { get; } = new DeviceRGB();

    /// <inheritdoc />
    public override int Components
        => 3;

    /// <inheritdoc />
    public override PdfName Name
        => PdfName.Get("DeviceRGB");

    /// <inheritdoc />
    public bool Equals(DeviceRGB? other)
        => other is not null;

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => Equals(obj as DeviceRGB);

    /// <inheritdoc />
    public override bool Equals(ColorSpace? other)
        => Equals(other as DeviceRGB);

    /// <inheritdoc />
    public override int GetHashCode()
        => HashCode.Combine(Instance, Components);
}
