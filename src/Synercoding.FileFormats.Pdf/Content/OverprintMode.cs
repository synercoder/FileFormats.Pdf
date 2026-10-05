namespace Synercoding.FileFormats.Pdf.Content;

/// <summary>
/// Specifies how overprinting is applied when painting in a DeviceCMYK color space.
/// </summary>
public enum OverprintMode
{
    /// <summary>
    /// Each source color component value replaces the value previously painted for the corresponding device colorant,
    /// regardless of what the new value is.
    /// </summary>
    Standard = 0,
    /// <summary>
    /// A tint value of 0.0 for a source color component leaves the corresponding component of the previously painted color unchanged.
    /// </summary>
    NonZero = 1
}
