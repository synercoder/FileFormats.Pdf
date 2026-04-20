using SixLabors.ImageSharp.PixelFormats;

namespace Synercoding.FileFormats.Pdf.Content;

/// <summary>
/// What method is used to generate a 1 component grayscale pixel byte array
/// </summary>
public static class GrayScaleMethods
{
    /// <summary>
    /// Use the red channel
    /// </summary>
    public static byte RedChannel(ref Rgba32 pixel) => pixel.R;
    /// <summary>
    /// Use the red channel
    /// </summary>
    public static byte RedChannel(ref Rgb24 pixel) => pixel.R;
    /// <summary>
    /// Use the green channel
    /// </summary>
    public static byte GreenChannel(ref Rgba32 pixel) => pixel.G;
    /// <summary>
    /// Use the green channel
    /// </summary>
    public static byte GreenChannel(ref Rgb24 pixel) => pixel.G;
    /// <summary>
    /// Use the blue channel
    /// </summary>
    public static byte BlueChannel(ref Rgba32 pixel) => pixel.B;
    /// <summary>
    /// Use the blue channel
    /// </summary>
    public static byte BlueChannel(ref Rgb24 pixel) => pixel.B;
    /// <summary>
    /// Use the alpha channel
    /// </summary>
    public static byte AlphaChannel(ref Rgba32 pixel) => pixel.A;
    /// <summary>
    /// Use the average of the Red, Green and Blue channels.
    /// </summary>
    public static byte AverageOfRGBChannels(ref Rgba32 pixel) => (byte)( ( pixel.R + pixel.G + pixel.B ) / 3 );
    /// <summary>
    /// Use the average of the Red, Green and Blue channels.
    /// </summary>
    public static byte AverageOfRGBChannels(ref Rgb24 pixel) => (byte)( ( pixel.R + pixel.G + pixel.B ) / 3 );
    /// <summary>
    /// The constants defined by ITU-R BT.601 are 0.299 red + 0.587 green + 0.114 blue.
    /// </summary>
    public static byte BT601(ref Rgba32 pixel) => (byte)( ( pixel.R * 0.299 ) + ( pixel.G * 0.587 ) + ( pixel.B * 0.114 ) );
    /// <summary>
    /// The constants defined by ITU-R BT.601 are 0.299 red + 0.587 green + 0.114 blue.
    /// </summary>
    public static byte BT601(ref Rgb24 pixel) => (byte)( ( pixel.R * 0.299 ) + ( pixel.G * 0.587 ) + ( pixel.B * 0.114 ) );
    /// <summary>
    /// The constants defined by ITU-R BT.709 are 0.2126 red + 0.7152 green + 0.0722 blue.
    /// </summary>
    public static byte BT709(ref Rgba32 pixel) => (byte)( ( pixel.R * 0.2126 ) + ( pixel.G * 0.7152 ) + ( pixel.B * 0.0722 ) );
    /// <summary>
    /// The constants defined by ITU-R BT.709 are 0.2126 red + 0.7152 green + 0.0722 blue.
    /// </summary>
    public static byte BT709(ref Rgb24 pixel) => (byte)( ( pixel.R * 0.2126 ) + ( pixel.G * 0.7152 ) + ( pixel.B * 0.0722 ) );

    /// <summary>
    /// Create a threshold method from an initial method. The threshold method will return the lowValue if the initial method returns a value less than the threshold, and the highValue otherwise.
    /// </summary>
    /// <param name="initialMethod">The initial method to use.</param>
    /// <param name="threshold">The threshold value.</param>
    /// <param name="lowValue">The value to return if the initial method returns a value less than the threshold.</param>
    /// <param name="highValue">The value to return if the initial method returns a value greater than or equal to the threshold.</param>
    /// <returns>The threshold method.</returns>
    public static GrayScaleMethod32 Threshold(GrayScaleMethod32 initialMethod, byte threshold, byte lowValue = 0x00, byte highValue = 0xFF)
    {
        return (ref Rgba32 pixel) =>
        {
            var value = initialMethod(ref pixel);
            return value < threshold ? lowValue : highValue;
        };
    }

    /// <summary>
    /// Create a threshold method from an initial method. The threshold method will return the lowValue if the initial method returns a value less than the threshold, and the highValue otherwise.
    /// </summary>
    /// <param name="initialMethod">The initial method to use.</param>
    /// <param name="threshold">The threshold value.</param>
    /// <param name="lowValue">The value to return if the initial method returns a value less than the threshold.</param>
    /// <param name="highValue">The value to return if the initial method returns a value greater than or equal to the threshold.</param>
    /// <returns>The threshold method.</returns>
    public static GrayScaleMethod24 Threshold(GrayScaleMethod24 initialMethod, byte threshold, byte lowValue = 0x00, byte highValue = 0xFF)
    {
        return (ref Rgb24 pixel) =>
        {
            var value = initialMethod(ref pixel);
            return value < threshold ? lowValue : highValue;
        };
    }
}

public delegate byte GrayScaleMethod32(ref Rgba32 pixel);
public delegate byte GrayScaleMethod24(ref Rgb24 pixel);
