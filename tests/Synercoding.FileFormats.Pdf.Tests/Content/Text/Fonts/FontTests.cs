using Synercoding.FileFormats.Pdf.Content.Text.Fonts;
using Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType;

namespace Synercoding.FileFormats.Pdf.Tests.Content.Text.Fonts;

public class FontTests
{
    private static readonly string _testFontPath = Path.Combine("Font-Files", "juliett-font", "JuliettRegular-7OXnA.ttf");
    private static readonly string _testFontTakotaPath = Path.Combine("Font-Files", "takota-font", "Takota-BRa8.ttf");

    [Fact]
    public void Test_Load_WithValidPath_LoadsFont()
    {
        if (!File.Exists(_testFontPath))
        {
            // Skip test if font file not available
            return;
        }

        var font = Font.Load(_testFontPath);

        Assert.NotNull(font);
        Assert.IsType<TrueTypeFont>(font);
    }

    [Fact]
    public void Test_Load_WithValidBytes_LoadsFont()
    {
        if (!File.Exists(_testFontPath))
        {
            return;
        }

        var fontData = File.ReadAllBytes(_testFontPath);
        var font = Font.Load(fontData);

        Assert.NotNull(font);
        Assert.IsType<TrueTypeFont>(font);
    }

    [Fact]
    public void Test_Load_WithValidStream_LoadsFont()
    {
        if (!File.Exists(_testFontPath))
        {
            return;
        }

        using var stream = File.OpenRead(_testFontPath);
        var font = Font.Load(stream);

        Assert.NotNull(font);
        Assert.IsType<TrueTypeFont>(font);
    }

    [Fact]
    public void Test_Load_WithNullPath_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Font.Load((string)null!));
    }

    [Fact]
    public void Test_Load_WithEmptyPath_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Font.Load(""));
    }

    [Fact]
    public void Test_Load_WithNullBytes_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Font.Load((byte[])null!));
    }

    [Fact]
    public void Test_Load_WithEmptyBytes_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Font.Load(new byte[0]));
    }

    [Fact]
    public void Test_Load_WithNullStream_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Font.Load((Stream)null!));
    }

    [Fact]
    public void Test_Font_Properties_HaveValidValues()
    {
        if (!File.Exists(_testFontPath))
        {
            return;
        }

        var font = Font.Load(_testFontPath);

        Assert.True(font.UnitsPerEm > 0);
        Assert.NotEmpty(font.FontName);

        // Font metrics should be reasonable
        Assert.True(font.Ascent > 0);
        Assert.True(font.Descent < 0); // Descent is typically negative
        Assert.True(font.CapHeight > 0);
        Assert.True(font.XHeight > 0);
        Assert.True(font.UnderlineThickness > 0);
    }

    [Fact]
    public void Test_GetBoundingBox_WithEmptyString_ReturnsZeroBox()
    {
        if (!File.Exists(_testFontPath))
        {
            return;
        }

        var font = Font.Load(_testFontPath);
        var measurement = font.GetBoundingBox("", 12);

        Assert.Equal(0, measurement.BoundingBox.Width.Raw);
        Assert.Equal(0, measurement.BoundingBox.Height.Raw);
        Assert.Equal(0, measurement.Ascent);
        Assert.Equal(0, measurement.Descent);
        Assert.Equal(0, measurement.XHeight);
    }

    [Fact]
    public void Test_GetBoundingBox_WithText_ReturnsValidMeasurement()
    {
        if (!File.Exists(_testFontPath))
        {
            return;
        }

        var font = Font.Load(_testFontPath);
        var measurement = font.GetBoundingBox("Hello", 12);

        Assert.True(measurement.BoundingBox.Width.Raw > 0);
        Assert.True(measurement.BoundingBox.Height.Raw > 0);
        Assert.True(measurement.Ascent > 0);
        Assert.True(measurement.XHeight > 0);
    }

    [Theory]
    [InlineData("Hello World", 12)]
    [InlineData("Test", 24)]
    [InlineData("ABC", 10)]
    public void Test_GetBoundingBox_WithDifferentText_ReturnsConsistentResults(string text, double fontSize)
    {
        if (!File.Exists(_testFontPath))
        {
            return;
        }

        var font = Font.Load(_testFontPath);
        var measurement1 = font.GetBoundingBox(text, fontSize);
        var measurement2 = font.GetBoundingBox(text, fontSize);

        Assert.Equal(measurement1.BoundingBox.Width.Raw, measurement2.BoundingBox.Width.Raw);
        Assert.Equal(measurement1.BoundingBox.Height.Raw, measurement2.BoundingBox.Height.Raw);
        Assert.Equal(measurement1.Ascent, measurement2.Ascent);
        Assert.Equal(measurement1.Descent, measurement2.Descent);
        Assert.Equal(measurement1.XHeight, measurement2.XHeight);
    }

    [Fact]
    public void Test_GetBoundingBox_WithLargerFontSize_ReturnsLargerMeasurement()
    {
        if (!File.Exists(_testFontPath))
        {
            return;
        }

        var font = Font.Load(_testFontPath);
        var measurement12 = font.GetBoundingBox("Test", 12);
        var measurement24 = font.GetBoundingBox("Test", 24);

        Assert.True(measurement24.BoundingBox.Width.Raw > measurement12.BoundingBox.Width.Raw);
        Assert.True(measurement24.BoundingBox.Height.Raw > measurement12.BoundingBox.Height.Raw);
        Assert.True(measurement24.Ascent > measurement12.Ascent);
        Assert.True(measurement24.XHeight > measurement12.XHeight);
    }

    [Fact]
    public void Test_Font_Equality_SameFontData_ReturnsTrue()
    {
        if (!File.Exists(_testFontPath))
        {
            return;
        }

        var fontData = File.ReadAllBytes(_testFontPath);
        var font1 = Font.Load(fontData);
        var font2 = Font.Load(fontData);

        Assert.True(font1.Equals(font2));
        Assert.Equal(font1.GetHashCode(), font2.GetHashCode());
    }

    [Fact]
    public void Test_Font_Equality_DifferentFontData_ReturnsFalse()
    {
        if (!File.Exists(_testFontPath) || !File.Exists(_testFontTakotaPath))
        {
            return;
        }

        var font1 = Font.Load(_testFontPath);
        var font2 = Font.Load(_testFontTakotaPath);

        Assert.False(font1.Equals(font2));
    }

    [Theory]
    [InlineData("a")]      // lowercase without ascenders/descenders
    [InlineData("h")]      // lowercase with ascender
    [InlineData("p")]      // lowercase with descender
    [InlineData("A")]      // uppercase
    [InlineData("1")]      // digit
    public void Test_GetBoundingBox_CharacterTypes_HaveAppropriateMetrics(string text)
    {
        if (!File.Exists(_testFontPath))
        {
            return;
        }

        var font = Font.Load(_testFontPath);
        var measurement = font.GetBoundingBox(text, 12);

        // All characters should have some width and positive ascent
        Assert.True(measurement.BoundingBox.Width.Raw > 0);
        Assert.True(measurement.Ascent > 0);
        Assert.True(measurement.XHeight > 0);

        // Specific character checks
        switch (text)
        {
            case "p": // Has descender
                Assert.True(measurement.Descent > 0);
                break;
            case "h": // Has ascender
                Assert.True(measurement.Ascent > measurement.XHeight);
                break;
            case "A": // Capital letter
                Assert.True(measurement.Ascent >= measurement.XHeight);
                break;
        }
    }

    [Fact]
    public void Test_EncodeText_WithEmptyString_ReturnsEmptyArray()
    {
        if (!File.Exists(_testFontPath))
        {
            return;
        }

        var font = Font.Load(_testFontPath);
        var encoded = font.EncodeText("");

        Assert.Empty(encoded);
    }

    [Fact]
    public void Test_EncodeText_WithNullString_ReturnsEmptyArray()
    {
        if (!File.Exists(_testFontPath))
        {
            return;
        }

        var font = Font.Load(_testFontPath);
        var encoded = font.EncodeText(null!);

        Assert.Empty(encoded);
    }

    [Theory]
    [InlineData("A", new byte[] { 0x00, 0x41 })]
    [InlineData("B", new byte[] { 0x00, 0x42 })]
    [InlineData("a", new byte[] { 0x00, 0x61 })]
    [InlineData("z", new byte[] { 0x00, 0x7A })]
    [InlineData("0", new byte[] { 0x00, 0x30 })]
    [InlineData("9", new byte[] { 0x00, 0x39 })]
    [InlineData(" ", new byte[] { 0x00, 0x20 })]
    public void Test_EncodeText_WithSingleCharacter_ReturnsUTF16BE(string text, byte[] expected)
    {
        if (!File.Exists(_testFontPath))
        {
            return;
        }

        var font = Font.Load(_testFontPath);
        var encoded = font.EncodeText(text);

        Assert.Equal(expected, encoded);
    }

    [Theory]
    [InlineData("AB", new byte[] { 0x00, 0x41, 0x00, 0x42 })]
    [InlineData("Hello", new byte[] { 0x00, 0x48, 0x00, 0x65, 0x00, 0x6C, 0x00, 0x6C, 0x00, 0x6F })]
    [InlineData("123", new byte[] { 0x00, 0x31, 0x00, 0x32, 0x00, 0x33 })]
    public void Test_EncodeText_WithMultipleCharacters_ReturnsUTF16BE(string text, byte[] expected)
    {
        if (!File.Exists(_testFontPath))
        {
            return;
        }

        var font = Font.Load(_testFontPath);
        var encoded = font.EncodeText(text);

        Assert.Equal(expected, encoded);
    }

    [Theory]
    [InlineData("é", new byte[] { 0x00, 0xE9 })]    // Latin small letter e with acute
    [InlineData("ñ", new byte[] { 0x00, 0xF1 })]    // Latin small letter n with tilde
    [InlineData("©", new byte[] { 0x00, 0xA9 })]    // Copyright sign
    public void Test_EncodeText_WithExtendedASCII_ReturnsUTF16BE(string text, byte[] expected)
    {
        if (!File.Exists(_testFontPath))
        {
            return;
        }

        var font = Font.Load(_testFontPath);
        var encoded = font.EncodeText(text);

        Assert.Equal(expected, encoded);
    }

    [Theory]
    [InlineData("€", new byte[] { 0x20, 0xAC })]    // Euro sign
    [InlineData("™", new byte[] { 0x21, 0x22 })]    // Trade mark sign
    public void Test_EncodeText_WithUnicodeCharacters_ReturnsUTF16BE(string text, byte[] expected)
    {
        if (!File.Exists(_testFontPath))
        {
            return;
        }

        var font = Font.Load(_testFontPath);
        var encoded = font.EncodeText(text);

        Assert.Equal(expected, encoded);
    }

    [Fact]
    public void Test_EncodeText_WithLongString_ReturnsCorrectLength()
    {
        if (!File.Exists(_testFontPath))
        {
            return;
        }

        var font = Font.Load(_testFontPath);
        var text = "The quick brown fox jumps over the lazy dog";
        var encoded = font.EncodeText(text);

        // Each character should be encoded as 2 bytes in UTF-16BE
        Assert.Equal(text.Length * 2, encoded.Length);
    }

    [Fact]
    public void Test_EncodeText_ConsistentResults_SameInputSameOutput()
    {
        if (!File.Exists(_testFontPath))
        {
            return;
        }

        var font = Font.Load(_testFontPath);
        var text = "Test String 123";
        var encoded1 = font.EncodeText(text);
        var encoded2 = font.EncodeText(text);

        Assert.Equal(encoded1, encoded2);
    }
}
