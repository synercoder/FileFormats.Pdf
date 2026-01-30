namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType;

/// <summary>
/// Handles remapping of glyphs for font subsetting.
/// </summary>
internal sealed class GlyphRemapper
{
    /// <summary>
    /// Create a remapping for the specified glyphs.
    /// </summary>
    public GlyphRemapResult RemapGlyphs(TrueTypeFont font, IReadOnlySet<char> usedCharacters)
    {
        var oldToNew = new Dictionary<ushort, ushort>();
        var newToOld = new Dictionary<ushort, ushort>();
        var includedChars = new Dictionary<char, ushort>();

        // Always include .notdef at position 0
        oldToNew[0] = 0;
        newToOld[0] = 0;

        ushort newGlyphId = 1;

        // Get cmap table to map characters to glyphs
        var cmap = font.Tables.Cmap ?? throw new InvalidOperationException("Font must have a cmap table for subsetting");

        // Map each used character to its glyph and assign new IDs
        foreach (var ch in usedCharacters.OrderBy(c => c))
        {
            if (cmap.TryGetGlyphId(ch, out var oldGlyphId))
            {
                // Skip if we've already mapped this glyph
                if (!oldToNew.ContainsKey(oldGlyphId))
                {
                    oldToNew[oldGlyphId] = newGlyphId;
                    newToOld[newGlyphId] = oldGlyphId;
                    newGlyphId++;
                }

                includedChars[ch] = oldToNew[oldGlyphId];
            }
        }

        // Handle composite glyphs (if any referenced glyphs need to be included)
        var additionalGlyphs = _findCompositeGlyphReferences(font, oldToNew.Keys.ToList());
        foreach (var oldGlyphId in additionalGlyphs)
        {
            if (!oldToNew.ContainsKey(oldGlyphId))
            {
                oldToNew[oldGlyphId] = newGlyphId;
                newToOld[newGlyphId] = oldGlyphId;
                newGlyphId++;
            }
        }

        return new GlyphRemapResult
        {
            OldToNewMapping = oldToNew,
            NewToOldMapping = newToOld,
            CharacterToNewGlyph = includedChars,
            TotalGlyphs = newGlyphId
        };
    }

    private List<ushort> _findCompositeGlyphReferences(TrueTypeFont font, List<ushort> glyphIds)
    {
        var additionalGlyphs = new HashSet<ushort>();
        var glyf = font.Tables.Glyf;

        if (glyf == null)
            return additionalGlyphs.ToList();

        foreach (var glyphId in glyphIds)
        {
            var glyphData = glyf.GetGlyphData(glyphId);
            if (glyphData == null || glyphData.Length < 10)
                continue;

            // Read numberOfContours
            var numberOfContours = (short)( ( glyphData[0] << 8 ) | glyphData[1] );

            // If numberOfContours < 0, this is a composite glyph
            if (numberOfContours < 0)
            {
                // Parse composite glyph components
                var offset = 10; // Skip header
                bool hasMoreComponents = true;

                while (hasMoreComponents && offset + 4 <= glyphData.Length)
                {
                    var flags = (ushort)( ( glyphData[offset] << 8 ) | glyphData[offset + 1] );
                    var componentGlyphId = (ushort)( ( glyphData[offset + 2] << 8 ) | glyphData[offset + 3] );

                    additionalGlyphs.Add(componentGlyphId);

                    offset += 4;

                    // Check argument size based on flags
                    if (( flags & 0x0001 ) != 0) // ARG_1_AND_2_ARE_WORDS
                    {
                        offset += 4; // Skip 2 words
                    }
                    else
                    {
                        offset += 2; // Skip 2 bytes
                    }

                    // Check for transformation matrix
                    if (( flags & 0x0008 ) != 0) // WE_HAVE_A_SCALE
                    {
                        offset += 2;
                    }
                    else if (( flags & 0x0040 ) != 0) // WE_HAVE_AN_X_AND_Y_SCALE
                    {
                        offset += 4;
                    }
                    else if (( flags & 0x0080 ) != 0) // WE_HAVE_A_TWO_BY_TWO
                    {
                        offset += 8;
                    }

                    hasMoreComponents = ( flags & 0x0020 ) != 0; // MORE_COMPONENTS
                }
            }
        }

        // Recursively check if any of the newly found glyphs are also composite
        if (additionalGlyphs.Count > 0)
        {
            var newList = additionalGlyphs.ToList();
            var recursive = _findCompositeGlyphReferences(font, newList);
            foreach (var g in recursive)
            {
                additionalGlyphs.Add(g);
            }
        }

        return additionalGlyphs.ToList();
    }
}
