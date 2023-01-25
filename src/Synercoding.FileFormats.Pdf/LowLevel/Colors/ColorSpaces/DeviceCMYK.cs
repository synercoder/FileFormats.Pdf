using System;

namespace Synercoding.FileFormats.Pdf.LowLevel.Colors.ColorSpaces;

/// <summary>
/// Class representing DeviceCMYK colorspace
/// </summary>
public sealed class DeviceCMYK : ColorSpace, IEquatable<DeviceCMYK>
{
    private DeviceCMYK() { }

    /// <summary>
    /// Instance of <see cref="DeviceCMYK"/> class
    /// </summary>
    public static DeviceCMYK Instance { get; } = new DeviceCMYK();

    /// <inheritdoc />
    public override int Components
        => 4;

    /// <inheritdoc />
    public override PdfName Name
        => PdfName.Get("DeviceCMYK");

    /// <inheritdoc />
    public bool Equals(DeviceCMYK? other)
        => other is not null;

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => Equals(obj as DeviceCMYK);

    /// <inheritdoc />
    public override bool Equals(ColorSpace? other)
        => Equals(other as DeviceCMYK);

    /// <inheritdoc />
    public override int GetHashCode()
        => HashCode.Combine(Instance, Components);
}
