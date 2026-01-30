namespace Synercoding.FileFormats.Pdf.Content;

/// <summary>
/// Enum representing the ways the area to be filled can be determined
/// </summary>
public enum FillRule : byte
{
    /// <summary>
    /// Represents the non-zero winding rule
    /// </summary>
    NonZeroWindingNumber = 0,
    /// <summary>
    /// Represents the even odd rule
    /// </summary>
    EvenOdd = 1
}
