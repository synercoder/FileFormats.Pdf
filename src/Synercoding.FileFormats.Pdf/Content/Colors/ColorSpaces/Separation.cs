using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Content.Colors.ColorSpaces;

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
        const string RESERVED_NAME_ERROR = "Name {0} is a reserved name, and can not be used for a separation.";
        if (name == PdfNames.Cyan)
            throw new ArgumentException(string.Format(RESERVED_NAME_ERROR, PdfNames.Cyan), nameof(name));
        if (name == PdfNames.Magenta)
            throw new ArgumentException(string.Format(RESERVED_NAME_ERROR, PdfNames.Magenta), nameof(name));
        if (name == PdfNames.Yellow)
            throw new ArgumentException(string.Format(RESERVED_NAME_ERROR, PdfNames.Yellow), nameof(name));
        if (name == PdfNames.Black)
            throw new ArgumentException(string.Format(RESERVED_NAME_ERROR, PdfNames.Black), nameof(name));

        Name = name;
        BasedOnColor = baseColor;
        TintTransform = GetTintTransform(BasedOnColor);
    }

    /// <summary>
    /// The color this separation is based upon.
    /// </summary>
    public Color BasedOnColor { get; }

    /// <inheritdoc />
    public override int Components => 1;

    /// <inheritdoc />
    public override PdfName Name { get; }

    /// <summary>
    /// The tint transform function for this separation.
    /// </summary>
    public IPdfDictionary TintTransform { get; }

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

    internal IPdfArray ToPdfArray()
        => new PdfArray(
            PdfNames.Separation,
            Name,
            BasedOnColor.Colorspace.Name,
            TintTransform
        );

    internal static IPdfDictionary GetTintTransform(Color basedOnColor)
        => new PdfDictionary()
        {
            [PdfName.Get("C0")] = new PdfArray(new double[basedOnColor.Colorspace.Components]),
            [PdfName.Get("C1")] = new PdfArray(basedOnColor.Components),
            [PdfName.Get("Domain")] = new PdfArray([0, 1]),
            [PdfName.Get("FunctionType")] = new PdfNumber(2),
            [PdfName.Get("N")] = new PdfNumber(1.0)
        };
}
