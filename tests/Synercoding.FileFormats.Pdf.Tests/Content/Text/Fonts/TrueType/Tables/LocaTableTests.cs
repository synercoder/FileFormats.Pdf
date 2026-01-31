using Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.Tables;

namespace Synercoding.FileFormats.Pdf.Tests.Content.Text.Fonts.TrueType.Tables;

public class LocaTableTests
{
    [Fact]
    public void Test_Parse_ShortFormat_ParsesCorrectly()
    {
        // Short format: 16-bit offsets multiplied by 2
        // 3 glyphs + 1 end offset = 4 offsets
        var data = new byte[] {
            0x00, 0x00,  // Glyph 0: offset 0 * 2 = 0
            0x00, 0x10,  // Glyph 1: offset 16 * 2 = 32
            0x00, 0x20,  // Glyph 2: offset 32 * 2 = 64
            0x00, 0x30   // End: offset 48 * 2 = 96
        };

        var table = LocaTable.Parse(data, indexToLocFormat: 0, numGlyphs: 3);

        var (offset0, length0) = table.GetGlyphLocation(0);
        Assert.Equal(0u, offset0);
        Assert.Equal(32u, length0);

        var (offset1, length1) = table.GetGlyphLocation(1);
        Assert.Equal(32u, offset1);
        Assert.Equal(32u, length1);

        var (offset2, length2) = table.GetGlyphLocation(2);
        Assert.Equal(64u, offset2);
        Assert.Equal(32u, length2);
    }

    [Fact]
    public void Test_Parse_LongFormat_ParsesCorrectly()
    {
        // Long format: 32-bit offsets
        // 2 glyphs + 1 end offset = 3 offsets
        var data = new byte[] {
            0x00, 0x00, 0x00, 0x00,  // Glyph 0: offset 0
            0x00, 0x00, 0x01, 0x00,  // Glyph 1: offset 256
            0x00, 0x00, 0x02, 0x00   // End: offset 512
        };

        var table = LocaTable.Parse(data, indexToLocFormat: 1, numGlyphs: 2);

        var (offset0, length0) = table.GetGlyphLocation(0);
        Assert.Equal(0u, offset0);
        Assert.Equal(256u, length0);

        var (offset1, length1) = table.GetGlyphLocation(1);
        Assert.Equal(256u, offset1);
        Assert.Equal(256u, length1);
    }

    [Fact]
    public void Test_GetGlyphLocation_EmptyGlyph_ReturnsZeroLength()
    {
        // Glyph with same start and end offset (empty glyph)
        var data = new byte[] {
            0x00, 0x00,  // Glyph 0: offset 0
            0x00, 0x10,  // Glyph 1: offset 32
            0x00, 0x10,  // Glyph 2: offset 32 (same as glyph 1 = empty)
            0x00, 0x20   // End: offset 64
        };

        var table = LocaTable.Parse(data, indexToLocFormat: 0, numGlyphs: 3);

        var (offset1, length1) = table.GetGlyphLocation(1);
        Assert.Equal(32u, offset1);
        Assert.Equal(0u, length1); // Empty glyph

        Assert.False(table.HasGlyphData(1));
    }

    [Fact]
    public void Test_GetGlyphLocation_InvalidGlyphId_ReturnsZero()
    {
        var data = new byte[] { 0x00, 0x00, 0x00, 0x10 };
        var table = LocaTable.Parse(data, indexToLocFormat: 0, numGlyphs: 1);

        var (offset, length) = table.GetGlyphLocation(999);
        Assert.Equal(0u, offset);
        Assert.Equal(0u, length);

    }

    [Fact]
    public void Test_HasGlyphData_ValidGlyph_ReturnsTrue()
    {
        var data = new byte[] {
            0x00, 0x00,  // Glyph 0: offset 0
            0x00, 0x10   // End: offset 32
        };

        var table = LocaTable.Parse(data, indexToLocFormat: 0, numGlyphs: 1);
        Assert.True(table.HasGlyphData(0));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(-1)]
    public void Test_Parse_InvalidFormat_ThrowsException(short invalidFormat)
    {
        var data = new byte[] { 0x00, 0x00 };

        var exception = Assert.Throws<InvalidOperationException>(() =>
            LocaTable.Parse(data, indexToLocFormat: invalidFormat, numGlyphs: 1));

        Assert.Contains($"Invalid indexToLocFormat: {invalidFormat}", exception.Message);
    }
}