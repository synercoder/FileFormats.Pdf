namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType;

/// <summary>
/// Result of glyph remapping.
/// </summary>
internal sealed class GlyphRemapResult
{
    /// <summary>
    /// Mapping from old glyph IDs to new glyph IDs.
    /// </summary>
    public required Dictionary<ushort, ushort> OldToNewMapping { get; init; }

    /// <summary>
    /// Mapping from new glyph IDs to old glyph IDs.
    /// </summary>
    public required Dictionary<ushort, ushort> NewToOldMapping { get; init; }

    /// <summary>
    /// Mapping from characters to new glyph IDs.
    /// </summary>
    public required Dictionary<char, ushort> CharacterToNewGlyph { get; init; }

    /// <summary>
    /// Total number of glyphs in the subset.
    /// </summary>
    public required ushort TotalGlyphs { get; init; }
}
