namespace Synercoding.FileFormats.Pdf.LowLevel.Colors;

/// <summary>
/// Class with predefined colors
/// </summary>
public static class PredefinedColors
{
    /// <summary>
    /// Cyan
    /// </summary>
    public static Color Cyan { get; } = new CmykColor(1, 0, 0, 0);

    /// <summary>
    /// Magenta
    /// </summary>
    public static Color Magenta { get; } = new CmykColor(0, 1, 0, 0);

    /// <summary>
    /// Yellow
    /// </summary>
    public static Color Yellow { get; } = new CmykColor(0, 0, 1, 0);

    /// <summary>
    /// Black
    /// </summary>
    public static Color Black { get; } = new GrayColor(0);

    /// <summary>
    /// Dark gray
    /// </summary>
    public static Color DarkGray { get; } = new GrayColor(0.25);

    /// <summary>
    /// Gray
    /// </summary>
    public static Color Gray { get; } = new GrayColor(0.5);

    /// <summary>
    /// Light gray
    /// </summary>
    public static Color LightGray { get; } = new GrayColor(0.75);

    /// <summary>
    /// White
    /// </summary>
    public static Color White { get; } = new GrayColor(1);

    /// <summary>
    /// Red
    /// </summary>
    public static Color Red { get; } = new RgbColor(1, 0, 0);

    /// <summary>
    /// Green
    /// </summary>
    public static Color Green { get; } = new RgbColor(0, 1, 0);

    /// <summary>
    /// Blue
    /// </summary>
    public static Color Blue { get; } = new RgbColor(0, 0, 1);
}
