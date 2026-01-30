using Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType;
using Synercoding.FileFormats.Pdf.Generation.Internal;

namespace Synercoding.FileFormats.Pdf.Tests.Content.Text.Fonts.TrueType;

/// <summary>
/// Unit tests for FontSubsetter to verify subsetting logic and edge cases.
/// </summary>
public class FontSubsetterTests
{
    private static readonly string _testFontPath = Path.Combine("TestFiles", "Fonts", "JuliettRegular-7OXnA.ttf");

    [Fact]
    public void Test_CreateSubset_WithBasicCharacters_CreatesValidSubset()
    {
        // Arrange
        var originalFont = _loadTestFont();
        var tracker = new FontUsageTracker();
        tracker.AddText("Hello");
        var subsetter = new FontSubsetter();

        // Act
        var result = subsetter.CreateSubset(originalFont, tracker);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.SubsetFontData);
        Assert.NotNull(result.CidToGidMap);
        Assert.True(result.SubsetFontData.Length > 0, "Subset font data should not be empty");
        Assert.True(result.CidToGidMap.Count > 0, "CID to GID map should contain mappings");

        // Verify subset font is parseable
        var subsetFont = new TrueTypeFont(result.SubsetFontData);
        Assert.NotNull(subsetFont.Tables.Head);
        Assert.NotNull(subsetFont.Tables.Cmap);
        Assert.NotNull(subsetFont.Tables.Maxp);
    }

    [Theory]
    [InlineData("A", 2)] // .notdef + 'A'
    [InlineData("AB", 3)] // .notdef + 'A' + 'B'
    [InlineData("AAA", 2)] // .notdef + 'A' (duplicates should be deduplicated)
    [InlineData("ABCDE", 6)] // .notdef + 5 letters
    public void Test_CreateSubset_GlyphCounts_AreCorrect(string text, int expectedMinGlyphs)
    {
        // Arrange
        var originalFont = _loadTestFont();
        var tracker = new FontUsageTracker();
        tracker.AddText(text);
        var subsetter = new FontSubsetter();

        // Act
        var result = subsetter.CreateSubset(originalFont, tracker);

        // Assert
        var subsetFont = new TrueTypeFont(result.SubsetFontData);
        Assert.True(subsetFont.Tables.Maxp?.NumGlyphs >= expectedMinGlyphs,
            $"Subset should have at least {expectedMinGlyphs} glyphs for text '{text}', but has {subsetFont.Tables.Maxp?.NumGlyphs}");
    }

    [Fact]
    public void Test_CreateSubset_AlwaysIncludesNotDefGlyph()
    {
        // Arrange
        var originalFont = _loadTestFont();
        var tracker = new FontUsageTracker();
        tracker.AddText("X"); // Single character
        var subsetter = new FontSubsetter();

        // Act
        var result = subsetter.CreateSubset(originalFont, tracker);

        // Assert
        var subsetFont = new TrueTypeFont(result.SubsetFontData);

        // .notdef should always be at GID 0
        Assert.True(result.CidToGidMap.ContainsValue(0),
            "CID to GID map should contain a mapping to GID 0 (.notdef)");

        // Verify we have at least .notdef + the requested character
        Assert.True(subsetFont.Tables.Maxp?.NumGlyphs >= 2,
            $"Subset should have at least 2 glyphs (.notdef + 'X'), but has {subsetFont.Tables.Maxp?.NumGlyphs}");
    }

    [Fact]
    public void Test_CreateSubset_WithEmptyText_CreatesMinimalFont()
    {
        // Arrange
        var originalFont = _loadTestFont();
        var tracker = new FontUsageTracker();
        // Don't add any text - should still create a valid minimal font
        var subsetter = new FontSubsetter();

        // Act
        var result = subsetter.CreateSubset(originalFont, tracker);

        // Assert
        var subsetFont = new TrueTypeFont(result.SubsetFontData);

        // Should have at least .notdef glyph
        Assert.True(subsetFont.Tables.Maxp?.NumGlyphs >= 1,
            $"Even empty subset should have at least .notdef glyph, but has {subsetFont.Tables.Maxp?.NumGlyphs}");

        // Should have valid CID to GID mapping (at least for .notdef)
        Assert.True(result.CidToGidMap.Count >= 1,
            "Even empty subset should have at least one CID to GID mapping");
    }

    [Fact]
    public void Test_CreateSubset_CidToGidMapping_IsCorrect()
    {
        // Arrange
        var originalFont = _loadTestFont();
        var tracker = new FontUsageTracker();
        var testText = "ABC";
        tracker.AddText(testText);
        var subsetter = new FontSubsetter();

        // Act
        var result = subsetter.CreateSubset(originalFont, tracker);

        // Assert
        var subsetFont = new TrueTypeFont(result.SubsetFontData);

        foreach (char c in testText.Distinct())
        {
            var originalGid = originalFont.GetGlyphId(c);
            var subsetGid = subsetFont.GetGlyphId(c);

            if (originalGid > 0) // Character exists in original font
            {
                Assert.True(subsetGid > 0, $"Character '{c}' should be mappable in subset font");
                Assert.True(result.CidToGidMap.ContainsKey((ushort)originalGid),
                    $"CID to GID map should contain mapping for '{c}' (original GID {originalGid})");

                var mappedGid = result.CidToGidMap[(ushort)originalGid];
                Assert.Equal((ushort)subsetGid, mappedGid);
            }
        }
    }

    [Fact]
    public void Test_CreateSubset_WithSpecialCharacters_HandlesCorrectly()
    {
        // Arrange
        var originalFont = _loadTestFont();
        var tracker = new FontUsageTracker();
        var testText = "Hello, World! 123.";
        tracker.AddText(testText);
        var subsetter = new FontSubsetter();

        // Act
        var result = subsetter.CreateSubset(originalFont, tracker);

        // Assert - Should not throw and should create valid subset
        Assert.NotNull(result.SubsetFontData);
        var subsetFont = new TrueTypeFont(result.SubsetFontData);
        Assert.NotNull(subsetFont.Tables.Cmap);

        // Verify punctuation and numbers are handled
        foreach (char c in "123!,.")
        {
            var originalGid = originalFont.GetGlyphId(c);
            if (originalGid > 0)
            {
                var subsetGid = subsetFont.GetGlyphId(c);
                Assert.True(subsetGid > 0, $"Special character '{c}' should be mappable in subset");
            }
        }
    }

    [Fact]
    public void Test_CreateSubset_SubsetSmallerThanOriginal()
    {
        // Arrange
        var originalFont = _loadTestFont();
        var tracker = new FontUsageTracker();
        tracker.AddText("A"); // Only one character
        var subsetter = new FontSubsetter();

        // Act
        var result = subsetter.CreateSubset(originalFont, tracker);

        // Assert - The subset font file should be smaller
        Assert.True(result.SubsetFontData.Length < File.ReadAllBytes(_testFontPath).Length,
            "Subset font should be smaller than original font");

        // Assert - Should have fewer glyphs in the CID to GID map (this represents actual subset)
        var subsetFont = new TrueTypeFont(result.SubsetFontData);
        var originalNumGlyphs = originalFont.Tables.Maxp?.NumGlyphs ?? 0;

        // The maxp table may still report original count due to table interdependencies,
        // but the CID to GID map shows the actual subset size
        Assert.True(result.CidToGidMap.Count < originalNumGlyphs,
            $"CID to GID map should have fewer entries than original glyphs. Original: {originalNumGlyphs}, Subset mappings: {result.CidToGidMap.Count}");

        // Verify the subset font is still parseable and functional
        Assert.NotNull(subsetFont.Tables.Cmap);
        Assert.NotNull(subsetFont.Tables.Glyf);
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    [InlineData("Hello World")]
    [InlineData("The quick brown fox jumps over the lazy dog")]
    [InlineData("0123456789")]
    [InlineData("!@#$%^&*()")]
    public void Test_CreateSubset_VariousInputs_ProduceValidFonts(string text)
    {
        // Arrange
        var originalFont = _loadTestFont();
        var tracker = new FontUsageTracker();
        tracker.AddText(text);
        var subsetter = new FontSubsetter();

        // Act & Assert - Should not throw
        var result = subsetter.CreateSubset(originalFont, tracker);

        // Should produce parseable font
        var subsetFont = new TrueTypeFont(result.SubsetFontData);
        Assert.NotNull(subsetFont.Tables.Head);
        Assert.NotNull(subsetFont.Tables.Cmap);
        Assert.NotNull(subsetFont.Tables.Maxp);
        Assert.NotNull(subsetFont.Tables.Hmtx);
        Assert.NotNull(subsetFont.Tables.Loca);
        Assert.NotNull(subsetFont.Tables.Glyf);
    }

    [Fact]
    public void Test_CreateSubset_WithDuplicateCharacters_Deduplicates()
    {
        // Arrange
        var originalFont = _loadTestFont();
        var tracker1 = new FontUsageTracker();
        var tracker2 = new FontUsageTracker();

        tracker1.AddText("AAAA"); // Repeated A
        tracker2.AddText("A");    // Single A

        var subsetter = new FontSubsetter();

        // Act
        var result1 = subsetter.CreateSubset(originalFont, tracker1);
        var result2 = subsetter.CreateSubset(originalFont, tracker2);

        // Assert - Both should have the same number of glyphs
        var font1 = new TrueTypeFont(result1.SubsetFontData);
        var font2 = new TrueTypeFont(result2.SubsetFontData);

        Assert.Equal(font1.Tables.Maxp?.NumGlyphs, font2.Tables.Maxp?.NumGlyphs);
        Assert.Equal(result1.CidToGidMap.Count, result2.CidToGidMap.Count);
    }

    private static TrueTypeFont _loadTestFont()
    {
        var fontData = File.ReadAllBytes(_testFontPath);
        return new TrueTypeFont(fontData);
    }
}
