using Synercoding.FileFormats.Pdf.Generation.Internal;

namespace Synercoding.FileFormats.Pdf.Tests.Generation.Internal;

public class FontUsageTrackerTests
{
    [Fact]
    public void Test_Constructor_InitializesEmptyTracker()
    {
        var tracker = new FontUsageTracker();

        Assert.Empty(tracker.UsedCharacters);
        Assert.Empty(tracker.CharacterToGlyphMapping);
        Assert.False(tracker.HasUsage);
    }

    [Fact]
    public void Test_AddText_WithSimpleText_TracksCharacters()
    {
        var tracker = new FontUsageTracker();

        tracker.AddText("Hello");

        Assert.Equal(4, tracker.UsedCharacters.Count); // H, e, l, o (l appears twice but set only stores unique)
        Assert.Contains('H', tracker.UsedCharacters);
        Assert.Contains('e', tracker.UsedCharacters);
        Assert.Contains('l', tracker.UsedCharacters);
        Assert.Contains('o', tracker.UsedCharacters);
        Assert.True(tracker.HasUsage);
    }

    [Fact]
    public void Test_AddText_WithEmptyText_DoesNotAddCharacters()
    {
        var tracker = new FontUsageTracker();

        tracker.AddText("");

        Assert.Empty(tracker.UsedCharacters);
        Assert.False(tracker.HasUsage);
    }

    [Fact]
    public void Test_AddText_WithMultipleCalls_AccumulatesCharacters()
    {
        var tracker = new FontUsageTracker();

        tracker.AddText("Hello");
        tracker.AddText("World");

        Assert.Equal(7, tracker.UsedCharacters.Count); // H,e,l,o,W,r,d (l appears in both but counted once)
        Assert.Contains('H', tracker.UsedCharacters);
        Assert.Contains('W', tracker.UsedCharacters);
        Assert.Contains('r', tracker.UsedCharacters);
        Assert.Contains('d', tracker.UsedCharacters);
    }

    [Fact]
    public void Test_AddText_WithDuplicateCharacters_StoresUnique()
    {
        var tracker = new FontUsageTracker();

        tracker.AddText("aaa");

        Assert.Single(tracker.UsedCharacters);
        Assert.Contains('a', tracker.UsedCharacters);
    }

    [Fact]
    public void Test_AddText_WithUnicodeCharacters_HandlesCorrectly()
    {
        var tracker = new FontUsageTracker();

        tracker.AddText("Café");

        Assert.Equal(4, tracker.UsedCharacters.Count);
        Assert.Contains('C', tracker.UsedCharacters);
        Assert.Contains('a', tracker.UsedCharacters);
        Assert.Contains('f', tracker.UsedCharacters);
        Assert.Contains('é', tracker.UsedCharacters);
    }

    [Fact]
    public void Test_AddCharacterMapping_StoresMapping()
    {
        var tracker = new FontUsageTracker();

        tracker.AddCharacterMapping('A', 100);
        tracker.AddCharacterMapping('B', 200);

        Assert.Equal(2, tracker.CharacterToGlyphMapping.Count);
        Assert.Equal(100, tracker.CharacterToGlyphMapping['A']);
        Assert.Equal(200, tracker.CharacterToGlyphMapping['B']);
    }

    [Fact]
    public void Test_AddCharacterMapping_WithDuplicateCharacter_UpdatesMapping()
    {
        var tracker = new FontUsageTracker();

        tracker.AddCharacterMapping('A', 100);
        tracker.AddCharacterMapping('A', 200);

        Assert.Single(tracker.CharacterToGlyphMapping);
        Assert.Equal(200, tracker.CharacterToGlyphMapping['A']);
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    [InlineData("Hello World")]
    [InlineData("The quick brown fox jumps over the lazy dog")]
    public void Test_AddText_WithVariousLengths_TracksCorrectCount(string text)
    {
        var tracker = new FontUsageTracker();

        tracker.AddText(text);

        var expectedCount = text.Distinct().Count();
        Assert.Equal(expectedCount, tracker.UsedCharacters.Count);
        Assert.Equal(expectedCount > 0, tracker.HasUsage);
    }

    [Fact]
    public void Test_UsedCharacters_IsReadOnly()
    {
        var tracker = new FontUsageTracker();
        tracker.AddText("Test");

        var usedChars = tracker.UsedCharacters;

        // Should not be able to modify the returned collection
        Assert.IsAssignableFrom<System.Collections.Generic.IReadOnlySet<char>>(usedChars);
    }

    [Fact]
    public void Test_CharacterToGlyphMapping_IsReadOnly()
    {
        var tracker = new FontUsageTracker();
        tracker.AddCharacterMapping('A', 100);

        var mapping = tracker.CharacterToGlyphMapping;

        // Should not be able to modify the returned collection
        Assert.IsAssignableFrom<System.Collections.Generic.IReadOnlyDictionary<char, int>>(mapping);
    }

    [Fact]
    public void Test_AddText_WithSpecialCharacters_HandlesCorrectly()
    {
        var tracker = new FontUsageTracker();

        tracker.AddText("Hello, World! @#$%^&*()");

        // Should include all unique characters including punctuation and symbols
        Assert.Contains(',', tracker.UsedCharacters);
        Assert.Contains(' ', tracker.UsedCharacters);
        Assert.Contains('!', tracker.UsedCharacters);
        Assert.Contains('@', tracker.UsedCharacters);
        Assert.Contains('(', tracker.UsedCharacters);
        Assert.Contains(')', tracker.UsedCharacters);
    }
}
