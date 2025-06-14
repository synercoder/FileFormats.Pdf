using Synercoding.FileFormats.Pdf.Parsing;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.Parsing;

public class PDFDocEncodingTests
{
    [Theory]
    [InlineData('A', true)]
    [InlineData('z', true)]
    [InlineData('0', true)]
    [InlineData(' ', true)]
    [InlineData('\t', true)]
    [InlineData('\n', true)]
    [InlineData('\r', true)]
    [InlineData('€', true)] // Euro sign
    [InlineData('©', true)] // Copyright
    [InlineData('®', true)] // Registered trademark
    [InlineData('™', true)] // Trademark
    [InlineData('•', true)] // Bullet
    [InlineData('—', true)] // Em dash
    [InlineData('–', true)] // En dash
    [InlineData('\u201C', true)] // Left double quote
    [InlineData('\u201D', true)] // Right double quote
    [InlineData('\u2018', true)] // Left single quote
    [InlineData('\u2019', true)] // Right single quote
    [InlineData('ñ', true)] // Latin small letter n with tilde
    [InlineData('Ñ', true)] // Latin capital letter n with tilde
    [InlineData('ü', true)] // Latin small letter u with diaeresis
    [InlineData('Ü', true)] // Latin capital letter u with diaeresis
    [InlineData('ø', true)] // Latin small letter o with stroke
    [InlineData('Ø', true)] // Latin capital letter o with stroke
    [InlineData('œ', true)] // Latin small ligature oe
    [InlineData('Œ', true)] // Latin capital ligature oe
    [InlineData('\uFB01', true)] // Latin small ligature fi
    [InlineData('\uFB02', true)] // Latin small ligature fl
    [InlineData('\u0131', true)] // Latin small letter dotless i
    [InlineData('Ł', true)] // Latin capital letter l with stroke
    [InlineData('ł', true)] // Latin small letter l with stroke
    [InlineData('Š', true)] // Latin capital letter s with caron
    [InlineData('š', true)] // Latin small letter s with caron
    [InlineData('Ž', true)] // Latin capital letter z with caron
    [InlineData('ž', true)] // Latin small letter z with caron
    [InlineData('Ÿ', true)] // Latin capital letter y with diaeresis
    [InlineData('\u1234', false)] // Unsupported character
    [InlineData('\uFFFD', false)] // Replacement character
    [InlineData('中', false)] // Chinese character
    [InlineData('א', false)] // Hebrew character
    public void Test_IsSupportedCharacter(char character, bool expected)
    {
        var result = PDFDocEncoding.IsSupportedCharacter(character);
        
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0x20, true)] // Space
    [InlineData(0x41, true)] // 'A'
    [InlineData(0x7A, true)] // 'z'
    [InlineData(0x09, true)] // Tab
    [InlineData(0x0A, true)] // Line feed
    [InlineData(0x0D, true)] // Carriage return
    [InlineData(0x80, true)] // Bullet
    [InlineData(0x81, true)] // Dagger
    [InlineData(0xA0, true)] // Euro sign
    [InlineData(0xFF, true)] // Latin small letter y with diaeresis
    [InlineData(0x00, false)] // NULL (not in PDFDocEncoding)
    [InlineData(0x01, false)] // Not supported
    [InlineData(0x08, false)] // Not supported
    [InlineData(0x0B, false)] // Not supported
    [InlineData(0x0E, false)] // Not supported
    [InlineData(0x7F, false)] // DEL (not in PDFDocEncoding)
    [InlineData(0x9F, false)] // Not supported
    [InlineData(0xAD, false)] // Not supported (soft hyphen gap)
    public void Test_IsSupportedCodePoint(byte codePoint, bool expected)
    {
        var result = PDFDocEncoding.IsSupportedCodePoint(codePoint);
        
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Hello", new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F })]
    [InlineData("World", new byte[] { 0x57, 0x6F, 0x72, 0x6C, 0x64 })]
    [InlineData("123", new byte[] { 0x31, 0x32, 0x33 })]
    [InlineData("", new byte[] { })]
    [InlineData(" ", new byte[] { 0x20 })]
    [InlineData("\t\n\r", new byte[] { 0x09, 0x0A, 0x0D })]
    [InlineData("€", new byte[] { 0xA0 })] // Euro sign
    [InlineData("©®™", new byte[] { 0xA9, 0xAE, 0x92 })] // Copyright, registered, trademark
    [InlineData("•—–", new byte[] { 0x80, 0x84, 0x85 })] // Bullet, em dash, en dash
    [InlineData("\u201C\u201D", new byte[] { 0x8D, 0x8E })] // Left and right double quotes
    [InlineData("\u2018\u2019", new byte[] { 0x8F, 0x90 })] // Left and right single quotes
    [InlineData("ñÑ", new byte[] { 0xF1, 0xD1 })] // Latin n with tilde
    [InlineData("üÜ", new byte[] { 0xFC, 0xDC })] // Latin u with diaeresis
    [InlineData("øØ", new byte[] { 0xF8, 0xD8 })] // Latin o with stroke
    [InlineData("œŒ", new byte[] { 0x9C, 0x96 })] // Latin oe ligature
    [InlineData("\uFB01\uFB02", new byte[] { 0x93, 0x94 })] // Latin fi and fl ligatures
    [InlineData("\u0131Łł", new byte[] { 0x9A, 0x95, 0x9B })] // Dotless i, L with stroke
    [InlineData("ŠšŽž", new byte[] { 0x97, 0x9D, 0x99, 0x9E })] // S and Z with caron
    [InlineData("Ÿ", new byte[] { 0x98 })] // Y with diaeresis
    public void Test_Encode_ValidStrings(string input, byte[] expected)
    {
        var result = PDFDocEncoding.Encode(input);
        
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F }, "Hello")]
    [InlineData(new byte[] { 0x57, 0x6F, 0x72, 0x6C, 0x64 }, "World")]
    [InlineData(new byte[] { 0x31, 0x32, 0x33 }, "123")]
    [InlineData(new byte[] { }, "")]
    [InlineData(new byte[] { 0x20 }, " ")]
    [InlineData(new byte[] { 0x09, 0x0A, 0x0D }, "\t\n\r")]
    [InlineData(new byte[] { 0xA0 }, "€")] // Euro sign
    [InlineData(new byte[] { 0xA9, 0xAE, 0x92 }, "©®™")] // Copyright, registered, trademark
    [InlineData(new byte[] { 0x80, 0x84, 0x85 }, "•—–")] // Bullet, em dash, en dash
    [InlineData(new byte[] { 0x8D, 0x8E }, "\u201C\u201D")] // Left and right double quotes
    [InlineData(new byte[] { 0x8F, 0x90 }, "\u2018\u2019")] // Left and right single quotes
    [InlineData(new byte[] { 0xF1, 0xD1 }, "ñÑ")] // Latin n with tilde
    [InlineData(new byte[] { 0xFC, 0xDC }, "üÜ")] // Latin u with diaeresis
    [InlineData(new byte[] { 0xF8, 0xD8 }, "øØ")] // Latin o with stroke
    [InlineData(new byte[] { 0x9C, 0x96 }, "œŒ")] // Latin oe ligature
    [InlineData(new byte[] { 0x93, 0x94 }, "\uFB01\uFB02")] // Latin fi and fl ligatures
    [InlineData(new byte[] { 0x9A, 0x95, 0x9B }, "\u0131Łł")] // Dotless i, L with stroke
    [InlineData(new byte[] { 0x97, 0x9D, 0x99, 0x9E }, "ŠšŽž")] // S and Z with caron
    [InlineData(new byte[] { 0x98 }, "Ÿ")] // Y with diaeresis
    public void Test_Decode_ValidBytes(byte[] input, string expected)
    {
        var result = PDFDocEncoding.Decode(input);
        
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Hello World")]
    [InlineData("The quick brown fox jumps over the lazy dog")]
    [InlineData("1234567890")]
    [InlineData("!@#$%^&*()")]
    [InlineData("€©®™•—–\u201C\u201D\u2018\u2019")]
    [InlineData("àáâãäåæçèéêëìíîï")]
    [InlineData("ñòóôõöøùúûüýÿ")]
    [InlineData("ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏ")]
    [InlineData("ÑÒÓÔÕÖØÙÚÛÜÝŸ")]
    [InlineData("œŒ\uFB01\uFB02\u0131ŁłŠšŽž")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t\n\r")]
    public void Test_Encode_Then_Decode_RoundTrip(string original)
    {
        var encoded = PDFDocEncoding.Encode(original);
        var decoded = PDFDocEncoding.Decode(encoded);
        
        Assert.Equal(original, decoded);
    }

    [Theory]
    [InlineData("中文")] // Chinese characters
    [InlineData("עברית")] // Hebrew
    [InlineData("العربية")] // Arabic
    [InlineData("русский")] // Cyrillic
    [InlineData("Test\u1234")] // Contains unsupported character
    public void Test_Encode_UnsupportedCharacters_ThrowsArgumentOutOfRangeException(string input)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PDFDocEncoding.Encode(input));
    }

    [Theory]
    [InlineData(new byte[] { 0x00 })] // NULL (not in PDFDocEncoding)
    [InlineData(new byte[] { 0x01 })] // Not supported
    [InlineData(new byte[] { 0x08 })] // Not supported
    [InlineData(new byte[] { 0x0B })] // Not supported
    [InlineData(new byte[] { 0x0E })] // Not supported
    [InlineData(new byte[] { 0x7F })] // DEL (not in PDFDocEncoding)
    [InlineData(new byte[] { 0x9F })] // Not supported
    [InlineData(new byte[] { 0xAD })] // Not supported (soft hyphen gap)
    [InlineData(new byte[] { 0x48, 0x00, 0x6C })] // Contains unsupported byte in middle
    public void Test_Decode_UnsupportedCodePoints_ThrowsDecoderFallbackException(byte[] input)
    {
        Assert.Throws<DecoderFallbackException>(() => PDFDocEncoding.Decode(input));
    }

    [Fact]
    public void Test_Encode_LargeString()
    {
        var largeString = new string('A', 10000);
        var expectedBytes = new byte[10000];
        Array.Fill(expectedBytes, (byte)0x41);
        
        var result = PDFDocEncoding.Encode(largeString);
        
        Assert.Equal(expectedBytes, result);
    }

    [Fact]
    public void Test_Decode_LargeByteArray()
    {
        var largeBytes = new byte[10000];
        Array.Fill(largeBytes, (byte)0x41);
        var expectedString = new string('A', 10000);
        
        var result = PDFDocEncoding.Decode(largeBytes);
        
        Assert.Equal(expectedString, result);
    }

    [Theory]
    [InlineData(0x18, '\u02D8')] // Breve
    [InlineData(0x19, '\u02C7')] // Caron
    [InlineData(0x1A, '\u02C6')] // Circumflex accent
    [InlineData(0x1B, '\u02D9')] // Dot above
    [InlineData(0x1C, '\u02DD')] // Double acute accent
    [InlineData(0x1D, '\u02DB')] // Ogonek
    [InlineData(0x1E, '\u02DA')] // Ring above
    [InlineData(0x1F, '\u02DC')] // Small tilde
    public void Test_Decode_SpecialDiacriticalMarks(byte input, char expected)
    {
        var result = PDFDocEncoding.Decode(new byte[] { input });
        
        Assert.Equal(expected.ToString(), result);
    }

    [Theory]
    [InlineData('\u02D8', 0x18)] // Breve
    [InlineData('\u02C7', 0x19)] // Caron
    [InlineData('\u02C6', 0x1A)] // Circumflex accent
    [InlineData('\u02D9', 0x1B)] // Dot above
    [InlineData('\u02DD', 0x1C)] // Double acute accent
    [InlineData('\u02DB', 0x1D)] // Ogonek
    [InlineData('\u02DA', 0x1E)] // Ring above
    [InlineData('\u02DC', 0x1F)] // Small tilde
    public void Test_Encode_SpecialDiacriticalMarks(char input, byte expected)
    {
        var result = PDFDocEncoding.Encode(input.ToString());
        
        Assert.Equal(new byte[] { expected }, result);
    }

    [Theory]
    [InlineData(0x82, '\u2021')] // Double dagger
    [InlineData(0x83, '\u2026')] // Horizontal ellipsis
    [InlineData(0x87, '\u2044')] // Fraction slash
    [InlineData(0x88, '\u2039')] // Single left-pointing angle quotation mark
    [InlineData(0x89, '\u203A')] // Single right-pointing angle quotation mark
    [InlineData(0x8A, '\u2212')] // Minus sign
    [InlineData(0x8B, '\u2030')] // Per mille sign
    [InlineData(0x8C, '\u201E')] // Double low-9 quotation mark
    [InlineData(0x91, '\u201A')] // Single low-9 quotation mark
    public void Test_Decode_SpecialPunctuation(byte input, char expected)
    {
        var result = PDFDocEncoding.Decode(new byte[] { input });
        
        Assert.Equal(expected.ToString(), result);
    }

    [Theory]
    [InlineData('\u2021', 0x82)] // Double dagger
    [InlineData('\u2026', 0x83)] // Horizontal ellipsis
    [InlineData('\u2044', 0x87)] // Fraction slash
    [InlineData('\u2039', 0x88)] // Single left-pointing angle quotation mark
    [InlineData('\u203A', 0x89)] // Single right-pointing angle quotation mark
    [InlineData('\u2212', 0x8A)] // Minus sign
    [InlineData('\u2030', 0x8B)] // Per mille sign
    [InlineData('\u201E', 0x8C)] // Double low-9 quotation mark
    [InlineData('\u201A', 0x91)] // Single low-9 quotation mark
    public void Test_Encode_SpecialPunctuation(char input, byte expected)
    {
        var result = PDFDocEncoding.Encode(input.ToString());
        
        Assert.Equal(new byte[] { expected }, result);
    }

    [Theory]
    [InlineData("Hello World! 123")] // Basic ASCII
    [InlineData("\t\n\r")] // Control characters
    [InlineData("€©®™•—–\u201C\u201D\u2018\u2019‡…⁄‹›−‰„‚")] // Extended punctuation and symbols
    [InlineData("\u02D8\u02C7\u02C6\u02D9\u02DD\u02DB\u02DA\u02DC")] // Diacritical marks
    [InlineData("àáâãäåæçèéêëìíîïñòóôõöøùúûüýÿ")] // Latin characters with diacritics
    [InlineData("ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÑÒÓÔÕÖØÙÚÛÜÝŸ")] // Latin characters with diacritics
    [InlineData("œŒ\uFB01\uFB02\u0131ŁłŠšŽžÐðÞþß")] // Special characters
    [InlineData("¡¢£¤¥¦§¨©ª«¬®¯°±²³´µ¶·¸¹º»¼½¾¿×÷")] // Mathematical and currency symbols
    public void Test_AllSupportedCharacters_CanBeEncodedAndDecoded(string testString)
    {
        var encoded = PDFDocEncoding.Encode(testString);
        var decoded = PDFDocEncoding.Decode(encoded);
        Assert.Equal(testString, decoded);
    }

    [Fact]
    public void Test_Encode_PreservesStringLength_ForBasicASCII()
    {
        var input = "Hello World 123";
        var result = PDFDocEncoding.Encode(input);
        
        Assert.Equal(input.Length, result.Length);
    }

    [Fact]
    public void Test_Decode_PreservesArrayLength_ForBasicASCII()
    {
        var input = new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x57, 0x6F, 0x72, 0x6C, 0x64 };
        var result = PDFDocEncoding.Decode(input);
        
        Assert.Equal(input.Length, result.Length);
    }
}
