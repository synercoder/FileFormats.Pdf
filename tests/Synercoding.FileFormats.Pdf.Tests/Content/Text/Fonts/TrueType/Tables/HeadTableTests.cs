using Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.Tables;

namespace Synercoding.FileFormats.Pdf.Tests.Content.Text.Fonts.TrueType.Tables;

public class HeadTableTests
{
    [Fact]
    public void Test_Parse_WithValidData_ParsesCorrectly()
    {
        // Create a valid head table (54 bytes minimum)
        var data = new byte[] {
            // Version (4 bytes) - 0x00010000
            0x00, 0x01, 0x00, 0x00,
            // FontRevision (4 bytes) - skip
            0x00, 0x01, 0x00, 0x00,
            // ChecksumAdjustment (4 bytes) - skip
            0x12, 0x34, 0x56, 0x78,
            // MagicNumber (4 bytes) - skip
            0x5F, 0x0F, 0x3C, 0xF5,
            // Flags (2 bytes)
            0x00, 0x1F,
            // UnitsPerEm (2 bytes)
            0x07, 0xD0, // 2000
            // Created (8 bytes) - skip
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            // Modified (8 bytes) - skip
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            // xMin (2 bytes)
            0xFE, 0x0C, // -500
            // yMin (2 bytes)
            0xFE, 0x0C, // -500
            // xMax (2 bytes)
            0x07, 0xD0, // 2000
            // yMax (2 bytes)
            0x07, 0xD0, // 2000
            // MacStyle (2 bytes)
            0x00, 0x00,
            // LowestRecPPEM (2 bytes) - skip
            0x00, 0x08,
            // FontDirectionHint (2 bytes) - skip
            0x00, 0x02,
            // IndexToLocFormat (2 bytes)
            0x00, 0x01, // Long format
            // GlyphDataFormat (2 bytes) - required to reach 54 bytes
            0x00, 0x00
        };

        var head = HeadTable.Parse(data);

        Assert.Equal(0x1F, head.Flags);
        Assert.Equal(2000, head.UnitsPerEm);
        Assert.Equal(-500, head.XMin);
        Assert.Equal(-500, head.YMin);
        Assert.Equal(2000, head.XMax);
        Assert.Equal(2000, head.YMax);
        Assert.Equal(0, head.MacStyle);
        Assert.Equal(1, head.IndexToLocFormat);
    }

    [Fact]
    public void Test_Parse_WithTooShortData_ThrowsInvalidOperationException()
    {
        var shortData = new byte[50]; // Less than 54 bytes required

        Assert.Throws<InvalidOperationException>(() => HeadTable.Parse(shortData));
    }

    [Theory]
    [InlineData(1000)]
    [InlineData(2048)]
    [InlineData(1024)]
    public void Test_Parse_WithCommonUnitsPerEm_ParsesCorrectly(ushort unitsPerEm)
    {
        var data = _createValidHeadTableData();

        // Set UnitsPerEm at offset 18-19
        data[18] = (byte)( unitsPerEm >> 8 );
        data[19] = (byte)( unitsPerEm & 0xFF );

        var head = HeadTable.Parse(data);

        Assert.Equal(unitsPerEm, head.UnitsPerEm);
    }

    [Theory]
    [InlineData(0)] // Short format
    [InlineData(1)] // Long format
    public void Test_Parse_WithBothIndexToLocFormats_ParsesCorrectly(short format)
    {
        var data = _createValidHeadTableData();

        // Set IndexToLocFormat at offset 50-51
        data[50] = (byte)( format >> 8 );
        data[51] = (byte)( format & 0xFF );

        var head = HeadTable.Parse(data);

        Assert.Equal(format, head.IndexToLocFormat);
    }

    [Fact]
    public void Test_Parse_WithNegativeBoundingBox_HandlesSignedValues()
    {
        var data = _createValidHeadTableData();

        // Set negative bounding box values
        // xMin = -1000 (0xFC18) - offset 36
        data[36] = 0xFC;
        data[37] = 0x18;
        // yMin = -200 (0xFF38) - offset 38
        data[38] = 0xFF;
        data[39] = 0x38;

        var head = HeadTable.Parse(data);

        Assert.Equal(-1000, head.XMin);
        Assert.Equal(-200, head.YMin);
    }

    private static byte[] _createValidHeadTableData()
    {
        return new byte[] {
            // Offset 0-3: Version (4 bytes)
            0x00, 0x01, 0x00, 0x00,
            // Offset 4-7: FontRevision (4 bytes)
            0x00, 0x01, 0x00, 0x00,
            // Offset 8-11: ChecksumAdjustment (4 bytes)
            0x12, 0x34, 0x56, 0x78,
            // Offset 12-15: MagicNumber (4 bytes)
            0x5F, 0x0F, 0x3C, 0xF5,
            // Offset 16-17: Flags (2 bytes)
            0x00, 0x1F,
            // Offset 18-19: UnitsPerEm (2 bytes)
            0x03, 0xE8, // 1000
            // Offset 20-27: Created (8 bytes)
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            // Offset 28-35: Modified (8 bytes)
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            // Offset 36-37: xMin (2 bytes)
            0x00, 0x00, // 0
            // Offset 38-39: yMin (2 bytes)
            0x00, 0x00, // 0
            // Offset 40-41: xMax (2 bytes)
            0x03, 0xE8, // 1000
            // Offset 42-43: yMax (2 bytes)
            0x03, 0xE8, // 1000
            // Offset 44-45: MacStyle (2 bytes)
            0x00, 0x00,
            // Offset 46-47: LowestRecPPEM (2 bytes)
            0x00, 0x08,
            // Offset 48-49: FontDirectionHint (2 bytes)
            0x00, 0x02,
            // Offset 50-51: IndexToLocFormat (2 bytes)
            0x00, 0x00, // Short format
            // Offset 52-53: GlyphDataFormat (2 bytes) - required to reach 54 bytes
            0x00, 0x00
        };
    }
}
