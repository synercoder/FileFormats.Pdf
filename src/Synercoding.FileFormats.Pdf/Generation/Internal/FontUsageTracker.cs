namespace Synercoding.FileFormats.Pdf.Generation.Internal;

/// <summary>
/// Tracks font usage for subsetting
/// </summary>
internal sealed class FontUsageTracker
{
    private readonly HashSet<char> _usedCharacters = new();
    private readonly Dictionary<char, int> _charToGlyph = new();

    /// <summary>
    /// Add text to track its character usage
    /// </summary>
    /// <param name="text">Text to track</param>
    public void AddText(string text)
    {
        foreach (var ch in text)
            _usedCharacters.Add(ch);
    }

    /// <summary>
    /// Record a character to glyph mapping
    /// </summary>
    /// <param name="character">The character</param>
    /// <param name="glyphId">The glyph ID</param>
    public void AddCharacterMapping(char character, int glyphId)
    {
        _charToGlyph[character] = glyphId;
    }

    /// <summary>
    /// Gets the set of used characters
    /// </summary>
    public IReadOnlySet<char> UsedCharacters => _usedCharacters;

    /// <summary>
    /// Gets the character to glyph mappings
    /// </summary>
    public IReadOnlyDictionary<char, int> CharacterToGlyphMapping => _charToGlyph;

    /// <summary>
    /// Gets whether any characters have been tracked
    /// </summary>
    public bool HasUsage => _usedCharacters.Count > 0;

    /// <summary>
    /// Gets the used characters as a set
    /// </summary>
    public IReadOnlySet<char> GetUsedCharacters() => _usedCharacters;
}
