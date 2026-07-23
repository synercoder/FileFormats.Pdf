namespace Synercoding.FileFormats.Pdf.Content;

/// <summary>
/// Specifies the blend mode to be used in the transparent imaging model.
/// </summary>
public enum BlendMode
{
    /// <summary>
    /// Selects the source color, ignoring the backdrop.
    /// </summary>
    Normal,
    /// <summary>
    /// Multiplies the backdrop and source color values.
    /// </summary>
    /// <remarks>
    /// The result color is always at least as dark as either of the two constituent
    /// colors. Multiplying any color with black produces black; multiplying with
    /// white leaves the original color unchanged. Painting successive overlapping
    /// objects with a color other than black or white produces progressively darker
    /// colors.
    /// </remarks>
    Multiply,
    /// <summary>
    /// Multiplies the complements of the backdrop and source color values,
    /// then complements the result
    /// </summary>
    /// <remarks>
    /// The result color is always at least as light as either of the two constituent
    /// colors. Screening any color with white produces white; screening with black
    /// leaves the original color unchanged. The effect is similar to projecting
    /// multiple photographic slides simultaneously onto a single screen.
    /// </remarks>
    Screen,
    /// <summary>
    /// Multiplies or screens the colors, depending on the backdrop color value.
    /// Source colors overlay the backdrop while preserving its highlights and
    /// shadows. The backdrop color is not replaced but is mixed with the source
    /// color to reflect the lightness or darkness of the backdrop.
    /// </summary>
    Overlay,
    /// <summary>
    /// Selects the darker of the backdrop and source colors.
    /// </summary>
    /// <remarks>
    /// The backdrop is replaced with the source where the source is darker;
    /// otherwise, it is left unchanged.
    /// </remarks>
    Darken,
    /// <summary>
    /// Selects the lighter of the backdrop and source colors.
    /// </summary>
    /// <remarks>
    /// The backdrop is replaced with the source where the source is lighter;
    /// otherwise, it is left unchanged.
    /// </remarks>
    Lighten,
    /// <summary>
    /// Brightens the backdrop color to reflect the source color.
    /// Painting with black produces no changes.
    /// </summary>
    ColorDodge,
    /// <summary>
    /// Darkens the backdrop color to reflect the source color.
    /// Painting with white produces no change.
    /// </summary>
    ColorBurn,
    /// <summary>
    /// Multiplies or screens the colors, depending on the source color value.
    /// The effect is similar to shining a harsh spotlight on the backdrop.
    /// </summary>
    HardLight,
    /// <summary>
    /// Darkens or lightens the colors, depending on the source color value.
    /// The effect is similar to shining a diffused spotlight on the backdrop.
    /// </summary>
    SoftLight,
    /// <summary>
    /// Subtracts the darker of the two constituent colors from the lighter color.
    /// </summary>
    /// <remarks>
    /// Painting with white inverts the backdrop color;
    /// painting with black produces no change.
    /// </remarks>
    Difference,
    /// <summary>
    /// Produces an effect similar to that of the <see cref="Difference"/> mode but lower in contrast.
    /// Painting with white inverts the backdrop color; painting with black produces no change.
    /// </summary>
    Exclusion,
    /// <summary>
    /// Creates a color with the hue of the source color and the saturation and luminosity of the backdrop color.
    /// </summary>
    Hue,
    /// <summary>
    /// Creates a color with the saturation of the source color and the hue and luminosity of the backdrop color. Painting with this mode in an area of the backdrop that is a pure gray (no saturation) produces no change.
    /// </summary>
    Saturation,
    /// <summary>
    /// Creates a color with the hue and saturation of the source color and the luminosity of the backdrop color. This preserves the gray levels of the backdrop and is useful for coloring monochrome images or tinting color images.
    /// </summary>
    Color,
    /// <summary>
    /// Creates a color with the luminosity of the source color and the hue and saturation of the backdrop color. This produces an inverse effect to that of the <see cref="Color"/> mode.
    /// </summary>
    Luminosity
}
