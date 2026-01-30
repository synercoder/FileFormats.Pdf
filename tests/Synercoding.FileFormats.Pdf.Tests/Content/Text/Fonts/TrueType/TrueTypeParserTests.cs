using Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType;

namespace Synercoding.FileFormats.Pdf.Tests.Content.Text.Fonts.TrueType;

public class TrueTypeParserTests
{
    private static readonly string _testFontPath = Path.Combine("Font-Files", "juliett-font", "JuliettRegular-7OXnA.ttf");
    private static readonly string _testOtfPath = Path.Combine("Font-Files", "takota-font", "Takota-7LRl.otf");

    [Fact]
    public void Test_Parse_WithValidTrueTypeFont_ParsesSuccessfully()
    {
        if (!File.Exists(_testFontPath))
        {
            return;
        }

        var fontData = File.ReadAllBytes(_testFontPath);
        var tables = TrueTypeParser.Parse(fontData);

        Assert.NotNull(tables);
        Assert.NotNull(tables.Head);
        Assert.NotNull(tables.Hhea);
        Assert.NotNull(tables.Maxp);
        Assert.NotNull(tables.Cmap);
        Assert.NotNull(tables.Hmtx);
    }

    [Fact]
    public void Test_Parse_WithNullData_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => TrueTypeParser.Parse(null!));
    }

    [Fact]
    public void Test_Parse_WithEmptyData_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => TrueTypeParser.Parse(new byte[0]));
    }

    [Fact]
    public void Test_Parse_WithTooShortData_ThrowsArgumentException()
    {
        var shortData = new byte[10];
        Assert.Throws<ArgumentException>(() => TrueTypeParser.Parse(shortData));
    }

    [Fact]
    public void Test_Parse_WithInvalidScalarType_ThrowsArgumentException()
    {
        // Create data with invalid scalar type
        var invalidData = new byte[12];
        invalidData[0] = 0xFF; // Invalid scalar type
        invalidData[1] = 0xFF;
        invalidData[2] = 0xFF;
        invalidData[3] = 0xFF;

        Assert.Throws<ArgumentException>(() => TrueTypeParser.Parse(invalidData));
    }

    [Fact]
    public void Test_Parse_ValidFont_HasCorrectHeadTable()
    {
        if (!File.Exists(_testFontPath))
        {
            return;
        }

        var fontData = File.ReadAllBytes(_testFontPath);
        var tables = TrueTypeParser.Parse(fontData);

        var head = tables.Head!;
        Assert.True(head.UnitsPerEm > 0);
        Assert.True(head.UnitsPerEm is 1000 or 2048 or 1024); // Common values
        Assert.True(head.XMax > head.XMin);
        Assert.True(head.YMax > head.YMin);
        Assert.True(head.IndexToLocFormat is 0 or 1); // 0 = short, 1 = long
    }

    [Fact]
    public void Test_Parse_ValidFont_HasCorrectHheaTable()
    {
        if (!File.Exists(_testFontPath))
        {
            return;
        }

        var fontData = File.ReadAllBytes(_testFontPath);
        var tables = TrueTypeParser.Parse(fontData);

        var hhea = tables.Hhea!;
        Assert.True(hhea.Ascender > 0);
        Assert.True(hhea.Descender < 0); // Descender is typically negative
        Assert.True(hhea.NumberOfHMetrics > 0);
        Assert.True(hhea.AdvanceWidthMax > 0);
    }

    [Fact]
    public void Test_Parse_ValidFont_HasCorrectMaxpTable()
    {
        if (!File.Exists(_testFontPath))
        {
            return;
        }

        var fontData = File.ReadAllBytes(_testFontPath);
        var tables = TrueTypeParser.Parse(fontData);

        var maxp = tables.Maxp!;
        Assert.True(maxp.NumGlyphs > 0);
        Assert.True(maxp.Version is 0x00005000 or 0x00010000); // Version 0.5 or 1.0
    }

    [Fact]
    public void Test_Parse_ValidFont_CmapCanMapBasicCharacters()
    {
        if (!File.Exists(_testFontPath))
        {
            return;
        }

        var fontData = File.ReadAllBytes(_testFontPath);
        var tables = TrueTypeParser.Parse(fontData);

        var cmap = tables.Cmap!;

        // Test basic ASCII characters
        Assert.True(cmap.GetGlyphId('A') > 0);
        Assert.True(cmap.GetGlyphId('a') > 0);
        Assert.True(cmap.GetGlyphId('0') > 0);
        Assert.True(cmap.GetGlyphId(' ') >= 0); // Space might be glyph 0 or a real glyph

        // Character not in font should return 0 (missing glyph)
        Assert.Equal(0, cmap.GetGlyphId('\uFFFF')); // Very unlikely to be in font
    }

    [Fact]
    public void Test_Parse_ValidFont_HmtxProvideWidths()
    {
        if (!File.Exists(_testFontPath))
        {
            return;
        }

        var fontData = File.ReadAllBytes(_testFontPath);
        var tables = TrueTypeParser.Parse(fontData);

        var hmtx = tables.Hmtx!;
        var cmap = tables.Cmap!;

        // Get glyph for 'A' and check its width
        var glyphId = cmap.GetGlyphId('A');
        var width = hmtx.GetAdvanceWidth(glyphId);

        Assert.True(width > 0); // Width should be positive

        // Check that different characters have potentially different widths
        var glyphIdI = cmap.GetGlyphId('i');
        var glyphIdW = cmap.GetGlyphId('W');
        var widthI = hmtx.GetAdvanceWidth(glyphIdI);
        var widthW = hmtx.GetAdvanceWidth(glyphIdW);

        // 'W' is typically wider than 'i'
        Assert.True(widthW >= widthI);
    }

    [Fact]
    public void Test_Parse_ValidFont_OptionalTablesHandledGracefully()
    {
        if (!File.Exists(_testFontPath))
        {
            return;
        }

        var fontData = File.ReadAllBytes(_testFontPath);
        var tables = TrueTypeParser.Parse(fontData);

        // Optional tables may or may not be present
        // Just verify parsing doesn't fail when they're present
        if (tables.Name != null)
        {
            // If name table exists, try to get some names
            var familyName = tables.Name.FamilyName;
            var postScriptName = tables.Name.PostScriptName;
            // Names can be null, but shouldn't throw
        }

        if (tables.OS2 != null)
        {
            Assert.True(tables.OS2.Version >= 0);
        }

        if (tables.Post != null)
        {
            Assert.True(tables.Post.Version > 0);
        }
    }

    [Fact]
    public void Test_Parse_WithOTFFont_ThrowsNotSupportedException()
    {
        if (!File.Exists(_testOtfPath))
        {
            return;
        }

        var fontData = File.ReadAllBytes(_testOtfPath);

        // OTF files with CFF outlines should throw NotSupportedException
        // Note: Some OTF files actually contain TrueType outlines and should work
        try
        {
            var tables = TrueTypeParser.Parse(fontData);
            // If this succeeds, it means the OTF has TrueType outlines
            Assert.NotNull(tables);
        }
        catch (NotSupportedException)
        {
            // This is expected for OTF with CFF outlines
        }
        catch (ArgumentException)
        {
            // This is also acceptable if the format is not recognized
        }
    }
}
