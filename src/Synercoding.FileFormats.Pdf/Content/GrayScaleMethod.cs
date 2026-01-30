namespace Synercoding.FileFormats.Pdf.Content;

/// <summary>
/// What method is used to generate a 1 component grayscale pixel byte array
/// </summary>
public enum GrayScaleMethod
{
    /// <summary>
    /// Use the red channel
    /// </summary>
    RedChannel,
    /// <summary>
    /// Use the green channel
    /// </summary>
    GreenChannel,
    /// <summary>
    /// Use the blue channel
    /// </summary>
    BlueChannel,
    /// <summary>
    /// Use the alpha channel
    /// </summary>
    AlphaChannel,
    /// <summary>
    /// Use the average of the Red, Green and Blue channels.
    /// </summary>
    AverageOfRGBChannels,
    /// <summary>
    /// The constants defined by ITU-R BT.601 are 0.299 red + 0.587 green + 0.114 blue.
    /// </summary>
    BT601,
    /// <summary>
    /// The constants defined by ITU-R BT.709 are 0.2126 red + 0.7152 green + 0.0722 blue.
    /// </summary>
    BT709,
}
