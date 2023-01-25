using System;

namespace Synercoding.FileFormats.Pdf.LowLevel.Colors.ColorSpaces;

/// <summary>
/// Class representing DeviceGray colorspace
/// </summary>
public sealed class DeviceGray : ColorSpace, IEquatable<DeviceGray>
{
    private DeviceGray() { }

    /// <summary>
    /// Instance of <see cref="DeviceGray"/> class
    /// </summary>
    public static DeviceGray Instance { get; } = new DeviceGray();

    /// <inheritdoc />
    public override int Components
        => 1;

    /// <inheritdoc />
    public override PdfName Name
        => PdfName.Get("DeviceGray");

    /// <inheritdoc />
    public bool Equals(DeviceGray? other)
        => other is not null;

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => Equals(obj as DeviceGray);

    /// <inheritdoc />
    public override bool Equals(ColorSpace? other)
        => Equals(other as DeviceGray);

    /// <inheritdoc />
    public override int GetHashCode()
        => HashCode.Combine(Instance, Components);
}
