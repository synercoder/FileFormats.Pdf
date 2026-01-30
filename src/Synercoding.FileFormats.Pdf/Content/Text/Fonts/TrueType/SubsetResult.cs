namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType;

/// <summary>
/// Result of font subsetting.
/// </summary>
internal sealed class SubsetResult
{
    /// <summary>
    /// The subset font data.
    /// </summary>
    public required byte[] SubsetFontData { get; init; }

    /// <summary>
    /// Mapping from CID to GID in the subset font.
    /// </summary>
    public required Dictionary<ushort, ushort> CidToGidMap { get; init; }
}
