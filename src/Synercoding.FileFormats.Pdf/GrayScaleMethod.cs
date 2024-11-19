namespace Synercoding.FileFormats.Pdf;

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
    AverageOfRGBChannels
}
