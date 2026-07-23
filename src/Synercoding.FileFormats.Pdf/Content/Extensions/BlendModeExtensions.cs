using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Content.Extensions;

internal static class BlendModeExtensions
{
    public static PdfName ToPdfName(this BlendMode blendMode)
        => blendMode switch
        {
            BlendMode.Normal => PdfNames.Normal,
            BlendMode.Multiply => PdfNames.Multiply,
            BlendMode.Screen => PdfNames.Screen,
            BlendMode.Overlay => PdfNames.Overlay,
            BlendMode.Darken => PdfNames.Darken,
            BlendMode.Lighten => PdfNames.Lighten,
            BlendMode.ColorDodge => PdfNames.ColorDodge,
            BlendMode.ColorBurn => PdfNames.ColorBurn,
            BlendMode.HardLight => PdfNames.HardLight,
            BlendMode.SoftLight => PdfNames.SoftLight,
            BlendMode.Difference => PdfNames.Difference,
            BlendMode.Exclusion => PdfNames.Exclusion,
            BlendMode.Hue => PdfNames.Hue,
            BlendMode.Saturation => PdfNames.Saturation,
            BlendMode.Color => PdfNames.Color,
            BlendMode.Luminosity => PdfNames.Luminosity,
            _ => throw new ArgumentOutOfRangeException(nameof(blendMode), blendMode, null)
        };
}
