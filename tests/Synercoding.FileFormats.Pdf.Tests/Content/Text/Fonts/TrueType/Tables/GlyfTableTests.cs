using Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.Tables;

namespace Synercoding.FileFormats.Pdf.Tests.Content.Text.Fonts.TrueType.Tables;

public class GlyfTableTests
{
    [Fact]
    public void Test_GetGlyphBoundingBox_SimpleGlyph_ParsesCorrectly()
    {
        // Create loca table with one glyph at offset 0, length 20
        var locaData = new byte[] {
            0x00, 0x00,  // Glyph 0: offset 0
            0x00, 0x0A   // End: offset 20 (10 * 2)
        };
        var loca = LocaTable.Parse(locaData, indexToLocFormat: 0, numGlyphs: 1);

        // Create glyf data with a simple glyph
        var glyfData = new byte[] {
            0x00, 0x01,  // numberOfContours = 1 (simple glyph)
            0x00, 0x0A,  // xMin = 10
            0xFF, 0xF6,  // yMin = -10
            0x01, 0xF4,  // xMax = 500
            0x03, 0xE8,  // yMax = 1000
            // Rest of glyph data would follow...
            0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00
        };

        var glyf = GlyfTable.Parse(glyfData, loca);
        var bbox = glyf.GetGlyphBoundingBox(0);

        Assert.NotNull(bbox);
        Assert.Equal(10, bbox!.XMin);
        Assert.Equal(-10, bbox.YMin);
        Assert.Equal(500, bbox.XMax);
        Assert.Equal(1000, bbox.YMax);
        Assert.Equal(490, bbox.Width);
        Assert.Equal(1010, bbox.Height);
    }

    [Fact]
    public void Test_GetGlyphBoundingBox_CompositeGlyph_ParsesCorrectly()
    {
        // Create loca table
        var locaData = new byte[] {
            0x00, 0x00,  // Glyph 0: offset 0
            0x00, 0x0A   // End: offset 20
        };
        var loca = LocaTable.Parse(locaData, indexToLocFormat: 0, numGlyphs: 1);

        // Create glyf data with a composite glyph
        var glyfData = new byte[] {
            0xFF, 0xFF,  // numberOfContours = -1 (composite glyph)
            0x00, 0x14,  // xMin = 20
            0x00, 0x1E,  // yMin = 30
            0x00, 0xC8,  // xMax = 200
            0x01, 0x2C,  // yMax = 300
            // Rest of composite glyph data...
            0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00
        };

        var glyf = GlyfTable.Parse(glyfData, loca);
        var bbox = glyf.GetGlyphBoundingBox(0);

        Assert.NotNull(bbox);
        Assert.Equal(20, bbox!.XMin);
        Assert.Equal(30, bbox.YMin);
        Assert.Equal(200, bbox.XMax);
        Assert.Equal(300, bbox.YMax);
    }

    [Fact]
    public void Test_GetGlyphBoundingBox_EmptyGlyph_ReturnsNull()
    {
        // Create loca table with empty glyph (same start and end offset)
        var locaData = new byte[] {
            0x00, 0x00,  // Glyph 0: offset 0
            0x00, 0x00   // End: offset 0 (empty)
        };
        var loca = LocaTable.Parse(locaData, indexToLocFormat: 0, numGlyphs: 1);

        var glyfData = new byte[10]; // Some data
        var glyf = GlyfTable.Parse(glyfData, loca);

        var bbox = glyf.GetGlyphBoundingBox(0);
        Assert.Null(bbox);
    }

    [Fact]
    public void Test_GetGlyphBoundingBox_InvalidGlyphId_ReturnsNull()
    {
        var locaData = new byte[] { 0x00, 0x00, 0x00, 0x0A };
        var loca = LocaTable.Parse(locaData, indexToLocFormat: 0, numGlyphs: 1);

        var glyfData = new byte[20];
        var glyf = GlyfTable.Parse(glyfData, loca);

        var bbox = glyf.GetGlyphBoundingBox(999);
        Assert.Null(bbox);
    }

    [Fact]
    public void Test_GetGlyphBoundingBoxes_MultipleGlyphs_ReturnsCorrectBoxes()
    {
        // Create loca table with 3 glyphs
        var locaData = new byte[] {
            0x00, 0x00,  // Glyph 0: offset 0
            0x00, 0x0A,  // Glyph 1: offset 20
            0x00, 0x14,  // Glyph 2: offset 40
            0x00, 0x14   // End: offset 40 (glyph 2 is empty)
        };
        var loca = LocaTable.Parse(locaData, indexToLocFormat: 0, numGlyphs: 3);

        // Create glyf data with two glyphs
        var glyfData = new byte[] {
            // Glyph 0
            0x00, 0x01,  // numberOfContours = 1
            0x00, 0x0A,  // xMin = 10
            0x00, 0x14,  // yMin = 20
            0x00, 0x1E,  // xMax = 30
            0x00, 0x28,  // yMax = 40
            0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00,
            // Glyph 1
            0x00, 0x01,  // numberOfContours = 1
            0x00, 0x32,  // xMin = 50
            0x00, 0x3C,  // yMin = 60
            0x00, 0x46,  // xMax = 70
            0x00, 0x50,  // yMax = 80
            0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00
        };

        var glyf = GlyfTable.Parse(glyfData, loca);
        var boxes = glyf.GetGlyphBoundingBoxes(new ushort[] { 0, 1, 2 });

        Assert.Equal(2, boxes.Count); // Only 2 glyphs have data

        Assert.True(boxes.ContainsKey(0));
        Assert.Equal(10, boxes[0].XMin);
        Assert.Equal(20, boxes[0].YMin);

        Assert.True(boxes.ContainsKey(1));
        Assert.Equal(50, boxes[1].XMin);
        Assert.Equal(60, boxes[1].YMin);

        Assert.False(boxes.ContainsKey(2)); // Empty glyph
    }

    [Fact]
    public void Test_GlyphBoundingBox_NegativeCoordinates_HandledCorrectly()
    {
        var locaData = new byte[] {
            0x00, 0x00,  // Glyph 0: offset 0
            0x00, 0x0A   // End: offset 20
        };
        var loca = LocaTable.Parse(locaData, indexToLocFormat: 0, numGlyphs: 1);

        var glyfData = new byte[] {
            0x00, 0x01,  // numberOfContours = 1
            0xFF, 0x9C,  // xMin = -100
            0xFF, 0x38,  // yMin = -200
            0x00, 0x64,  // xMax = 100
            0x00, 0xC8,  // yMax = 200
            0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00
        };

        var glyf = GlyfTable.Parse(glyfData, loca);
        var bbox = glyf.GetGlyphBoundingBox(0);

        Assert.NotNull(bbox);
        Assert.Equal(-100, bbox!.XMin);
        Assert.Equal(-200, bbox.YMin);
        Assert.Equal(100, bbox.XMax);
        Assert.Equal(200, bbox.YMax);
        Assert.Equal(200, bbox.Width);
        Assert.Equal(400, bbox.Height);
    }
}