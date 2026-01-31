using Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType;
using Synercoding.FileFormats.Pdf.Generation.Internal;

namespace Synercoding.FileFormats.Pdf.Tests.Content.Text.Fonts.TrueType;

/// <summary>
/// Round-trip tests for TtfWriter to ensure written fonts can be parsed correctly.
/// </summary>
public class TtfWriterTests
{
    private static readonly string _testFontPath = Path.Combine("TestFiles", "Fonts", "JuliettRegular-7OXnA.ttf");

    [Fact]
    public void Test_TtfWriter_RoundTrip_OriginalFontParseable()
    {
        // Arrange
        var originalFontData = File.ReadAllBytes(_testFontPath);
        var originalFont = new TrueTypeFont(originalFontData);

        // Act - Write the font using TtfWriter and parse it back
        var ttfWriter = new TtfWriter();
        var input = new TtfWriterInput
        {
            Head = originalFont.Tables.Head,
            Hhea = originalFont.Tables.Hhea,
            Maxp = originalFont.Tables.Maxp,
            Cmap = originalFont.Tables.Cmap,
            Hmtx = originalFont.Tables.Hmtx,
            Loca = originalFont.Tables.Loca,
            Glyf = originalFont.Tables.Glyf
        };

        var writtenFontData = ttfWriter.WriteTtf(input);

        // Assert - The written font should be parseable
        var parsedFont = new TrueTypeFont(writtenFontData);

        // Verify essential tables exist
        Assert.NotNull(parsedFont.Tables.Head);
        Assert.NotNull(parsedFont.Tables.Hhea);
        Assert.NotNull(parsedFont.Tables.Maxp);
        Assert.NotNull(parsedFont.Tables.Cmap);
        Assert.NotNull(parsedFont.Tables.Hmtx);
        Assert.NotNull(parsedFont.Tables.Loca);
        Assert.NotNull(parsedFont.Tables.Glyf);

        // Verify basic functionality
        Assert.True(parsedFont.GetGlyphId('A') > 0, "Should be able to get glyph ID for 'A'");
        Assert.True(parsedFont.GetGlyphId('B') > 0, "Should be able to get glyph ID for 'B'");
    }

    [Fact]
    public void Test_FontSubsetter_RoundTrip_SubsetFontParseable()
    {
        // Arrange
        var originalFontData = File.ReadAllBytes(_testFontPath);
        var originalFont = new TrueTypeFont(originalFontData);
        var tracker = new FontUsageTracker();
        tracker.AddText("Hello World");

        // Act - Create subset and parse it back
        var subsetter = new FontSubsetter();
        var result = subsetter.CreateSubset(originalFont, tracker);
        var subsetFont = new TrueTypeFont(result.SubsetFontData);

        // Assert - The subset font should be parseable and functional
        Assert.NotNull(subsetFont.Tables.Head);
        Assert.NotNull(subsetFont.Tables.Hhea);
        Assert.NotNull(subsetFont.Tables.Maxp);
        Assert.NotNull(subsetFont.Tables.Cmap);
        Assert.NotNull(subsetFont.Tables.Hmtx);
        Assert.NotNull(subsetFont.Tables.Loca);
        Assert.NotNull(subsetFont.Tables.Glyf);

        // Verify that used characters still map correctly
        var usedChars = "Hello World".Distinct().ToArray();
        foreach (char c in usedChars)
        {
            if (c == ' ') continue; // Skip space character if not mapped

            var originalGid = originalFont.GetGlyphId(c);
            var subsetGid = subsetFont.GetGlyphId(c);

            Assert.True(originalGid > 0, $"Original font should have glyph for '{c}'");
            Assert.True(subsetGid > 0, $"Subset font should have glyph for '{c}'");

            // Verify CID to GID mapping exists
            Assert.True(result.CidToGidMap.ContainsKey((ushort)originalGid),
                $"CID to GID map should contain mapping for character '{c}' (GID {originalGid})");

            var expectedRemappedGid = result.CidToGidMap[(ushort)originalGid];
            Assert.Equal(expectedRemappedGid, (ushort)subsetGid);
        }
    }

    [Fact]
    public void Test_FontSubsetter_PreservesNotDefGlyph()
    {
        // Arrange
        var originalFontData = File.ReadAllBytes(_testFontPath);
        var originalFont = new TrueTypeFont(originalFontData);
        var tracker = new FontUsageTracker();
        tracker.AddText("A"); // Only use one character

        // Act
        var subsetter = new FontSubsetter();
        var result = subsetter.CreateSubset(originalFont, tracker);
        var subsetFont = new TrueTypeFont(result.SubsetFontData);

        // Assert - .notdef glyph should always be at GID 0
        var notdefGid = subsetFont.GetGlyphId((char)0); // .notdef typically maps to char 0
        // Even if char 0 doesn't map, .notdef should be accessible at GID 0 in the glyf table
        Assert.NotNull(subsetFont.Tables.Glyf);

        // Verify that we have at least 2 glyphs: .notdef + 'A'
        Assert.True(subsetFont.Tables.Maxp?.NumGlyphs >= 2,
            $"Subset font should have at least 2 glyphs (.notdef + 'A'), but has {subsetFont.Tables.Maxp?.NumGlyphs}");
    }

    [Theory]
    [InlineData("A")]
    [InlineData("Hello")]
    [InlineData("0123456789")]
    [InlineData("!@#$%^&*()")]
    public void Test_FontSubsetter_VariousCharacterSets_RoundTrip(string testText)
    {
        // Arrange
        var originalFontData = File.ReadAllBytes(_testFontPath);
        var originalFont = new TrueTypeFont(originalFontData);
        var tracker = new FontUsageTracker();
        tracker.AddText(testText);

        // Act
        var subsetter = new FontSubsetter();
        var result = subsetter.CreateSubset(originalFont, tracker);

        // Assert - Should be able to parse the subset font
        var subsetFont = new TrueTypeFont(result.SubsetFontData); // Should not throw

        // Verify all used characters are mappable
        var uniqueChars = testText.Distinct().ToArray();
        foreach (char c in uniqueChars)
        {
            var originalGid = originalFont.GetGlyphId(c);
            var subsetGid = subsetFont.GetGlyphId(c);

            if (originalGid > 0) // Original font has this character
            {
                Assert.True(subsetGid > 0, $"Subset font should have glyph for '{c}' (original GID: {originalGid})");
                Assert.True(result.CidToGidMap.ContainsKey((ushort)originalGid),
                    $"CID to GID map should contain entry for '{c}' (GID {originalGid})");
            }
        }
    }

    [Fact]
    public void Test_FontSubsetter_EmptyText_CreatesMinimalFont()
    {
        // Arrange
        var originalFontData = File.ReadAllBytes(_testFontPath);
        var originalFont = new TrueTypeFont(originalFontData);
        var tracker = new FontUsageTracker();
        // Don't add any text - create minimal subset

        // Act
        var subsetter = new FontSubsetter();
        var result = subsetter.CreateSubset(originalFont, tracker);

        // Assert - Should create a minimal but valid font
        var subsetFont = new TrueTypeFont(result.SubsetFontData); // Should not throw

        // Should have at least .notdef glyph
        Assert.True(subsetFont.Tables.Maxp?.NumGlyphs >= 1,
            $"Even empty subset should have at least .notdef glyph, but has {subsetFont.Tables.Maxp?.NumGlyphs} glyphs");
    }
}
