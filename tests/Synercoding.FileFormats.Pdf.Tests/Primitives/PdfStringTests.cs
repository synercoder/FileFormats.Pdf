using Synercoding.FileFormats.Pdf.Primitives;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.Primitives;

public class PdfStringTests
{
    private static PdfString _createPdfString(string value, PdfStringEncoding encoding, bool isHex)
    {
        byte[] bytes = encoding switch
        {
            PdfStringEncoding.PdfDocEncoding => Encoding.Latin1.GetBytes(value),
            PdfStringEncoding.Utf16BE => [0xFE, 0xFF, .. Encoding.BigEndianUnicode.GetBytes(value)],
            PdfStringEncoding.Utf16LE => [0xFF, 0xFE, .. Encoding.Unicode.GetBytes(value)],
            PdfStringEncoding.Utf8 => [0xEF, 0xBB, 0xBF, .. Encoding.UTF8.GetBytes(value)],
            PdfStringEncoding.ByteString => Convert.FromHexString(value.Replace("<", "").Replace(">", "")),
            _ => throw new ArgumentException($"Unsupported encoding: {encoding}")
        };

        return new PdfString(bytes, isHex);
    }
    [Theory]
    [InlineData("test", PdfStringEncoding.PdfDocEncoding, false)]
    [InlineData("hello world", PdfStringEncoding.Utf8, true)]
    [InlineData("", PdfStringEncoding.Utf16BE, false)]
    [InlineData("こんにちは", PdfStringEncoding.Utf16LE, true)]
    public void Test_Constructor_SetsAllProperties(string value, PdfStringEncoding encoding, bool isHex)
    {
        var pdfString = _createPdfString(value, encoding, isHex);

        Assert.Equal(value, pdfString.Value);
        Assert.Equal(encoding, pdfString.Encoding);
        Assert.Equal(isHex, pdfString.IsHex);
    }

    [Fact]
    public void Test_Equals_SameValueEncodingAndHex_ReturnsTrue()
    {
        var string1 = _createPdfString("test", PdfStringEncoding.PdfDocEncoding, false);
        var string2 = _createPdfString("test", PdfStringEncoding.PdfDocEncoding, false);

        Assert.True(string1.Equals(string2));
        Assert.True(string1 == string2);
        Assert.False(string1 != string2);
    }

    [Fact]
    public void Test_Equals_DifferentValue_ReturnsFalse()
    {
        var string1 = _createPdfString("test", PdfStringEncoding.PdfDocEncoding, false);
        var string2 = _createPdfString("different", PdfStringEncoding.PdfDocEncoding, false);

        Assert.False(string1.Equals(string2));
        Assert.False(string1 == string2);
        Assert.True(string1 != string2);
    }

    [Fact]
    public void Test_Equals_DifferentEncoding_ReturnsFalse()
    {
        var string1 = _createPdfString("test", PdfStringEncoding.PdfDocEncoding, false);
        var string2 = _createPdfString("test", PdfStringEncoding.Utf8, false);

        Assert.False(string1.Equals(string2));
        Assert.False(string1 == string2);
        Assert.True(string1 != string2);
    }

    [Fact]
    public void Test_Equals_DifferentIsHex_ReturnsFalse()
    {
        var string1 = _createPdfString("test", PdfStringEncoding.PdfDocEncoding, false);
        var string2 = _createPdfString("test", PdfStringEncoding.PdfDocEncoding, true);

        Assert.False(string1.Equals(string2));
        Assert.False(string1 == string2);
        Assert.True(string1 != string2);
    }

    [Fact]
    public void Test_Equals_AllDifferent_ReturnsFalse()
    {
        var string1 = _createPdfString("test", PdfStringEncoding.PdfDocEncoding, false);
        var string2 = _createPdfString("different", PdfStringEncoding.Utf8, true);

        Assert.False(string1.Equals(string2));
        Assert.False(string1 == string2);
        Assert.True(string1 != string2);
    }

    [Fact]
    public void Test_Equals_Null_ReturnsFalse()
    {
        var pdfString = _createPdfString("test", PdfStringEncoding.PdfDocEncoding, false);
        PdfString? nullString = null;

        Assert.False(pdfString.Equals(nullString));
        Assert.False(pdfString == nullString!);
        Assert.True(pdfString != nullString!);
    }

    [Fact]
    public void Test_Equals_Object_SameString_ReturnsTrue()
    {
        var string1 = _createPdfString("test", PdfStringEncoding.PdfDocEncoding, false);
        object string2 = _createPdfString("test", PdfStringEncoding.PdfDocEncoding, false);

        Assert.True(string1.Equals(string2));
    }

    [Fact]
    public void Test_Equals_Object_DifferentType_ReturnsFalse()
    {
        var pdfString = _createPdfString("test", PdfStringEncoding.PdfDocEncoding, false);
        object obj = "not a pdf string";

        Assert.False(pdfString.Equals(obj));
    }

    [Fact]
    public void Test_Equals_Object_Null_ReturnsFalse()
    {
        var pdfString = _createPdfString("test", PdfStringEncoding.PdfDocEncoding, false);
        object? obj = null;

        Assert.False(pdfString.Equals(obj));
    }

    [Theory]
    [InlineData("test", PdfStringEncoding.PdfDocEncoding, false)]
    [InlineData("hello", PdfStringEncoding.Utf8, true)]
    [InlineData("world", PdfStringEncoding.Utf16BE, false)]
    [InlineData("example", PdfStringEncoding.Utf16LE, true)]
    public void Test_GetHashCode_SameProperties_ReturnsSameHash(string value, PdfStringEncoding encoding, bool isHex)
    {
        var string1 = _createPdfString(value, encoding, isHex);
        var string2 = _createPdfString(value, encoding, isHex);

        Assert.Equal(string1.GetHashCode(), string2.GetHashCode());
    }

    [Fact]
    public void Test_GetHashCode_DifferentValue_ReturnsDifferentHash()
    {
        var string1 = _createPdfString("test", PdfStringEncoding.PdfDocEncoding, false);
        var string2 = _createPdfString("different", PdfStringEncoding.PdfDocEncoding, false);

        Assert.NotEqual(string1.GetHashCode(), string2.GetHashCode());
    }

    [Fact]
    public void Test_EqualityOperators_Reflexive()
    {
        var pdfString = _createPdfString("test", PdfStringEncoding.PdfDocEncoding, false);
        var samePdfString = pdfString;

        Assert.True(pdfString == samePdfString);
        Assert.False(pdfString != samePdfString);
        Assert.True(pdfString.Equals(samePdfString));
    }

    [Fact]
    public void Test_EqualityOperators_Symmetric()
    {
        var string1 = _createPdfString("test", PdfStringEncoding.PdfDocEncoding, false);
        var string2 = _createPdfString("test", PdfStringEncoding.PdfDocEncoding, false);

        Assert.True(string1 == string2);
        Assert.True(string2 == string1);
        Assert.True(string1.Equals(string2));
        Assert.True(string2.Equals(string1));
    }

    [Fact]
    public void Test_EqualityOperators_Transitive()
    {
        var string1 = _createPdfString("test", PdfStringEncoding.PdfDocEncoding, false);
        var string2 = _createPdfString("test", PdfStringEncoding.PdfDocEncoding, false);
        var string3 = _createPdfString("test", PdfStringEncoding.PdfDocEncoding, false);

        Assert.True(string1 == string2);
        Assert.True(string2 == string3);
        Assert.True(string1 == string3);
    }

    [Theory]
    [InlineData(PdfStringEncoding.PdfDocEncoding)]
    [InlineData(PdfStringEncoding.Utf8)]
    [InlineData(PdfStringEncoding.Utf16BE)]
    [InlineData(PdfStringEncoding.Utf16LE)]
    public void Test_AllEncodingTypes(PdfStringEncoding encoding)
    {
        var pdfString = _createPdfString("test", encoding, false);

        Assert.Equal("test", pdfString.Value);
        Assert.Equal(encoding, pdfString.Encoding);
        Assert.False(pdfString.IsHex);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_IsHexProperty(bool isHex)
    {
        var pdfString = _createPdfString("test", PdfStringEncoding.PdfDocEncoding, isHex);

        Assert.Equal("test", pdfString.Value);
        Assert.Equal(PdfStringEncoding.PdfDocEncoding, pdfString.Encoding);
        Assert.Equal(isHex, pdfString.IsHex);
    }

    [Fact]
    public void Test_EmptyString_HandledCorrectly()
    {
        var pdfString = _createPdfString("", PdfStringEncoding.PdfDocEncoding, false);

        Assert.Equal("", pdfString.Value);
        Assert.Equal(PdfStringEncoding.PdfDocEncoding, pdfString.Encoding);
        Assert.False(pdfString.IsHex);
    }

    [Fact]
    public void Test_UnicodeString_HandledCorrectly()
    {
        var unicodeText = "こんにちは世界";
        var pdfString = _createPdfString(unicodeText, PdfStringEncoding.Utf8, false);

        Assert.Equal(unicodeText, pdfString.Value);
        Assert.Equal(PdfStringEncoding.Utf8, pdfString.Encoding);
        Assert.False(pdfString.IsHex);
    }

    [Fact]
    public void Test_LongString_HandledCorrectly()
    {
        var longString = new string('A', 10000);
        var pdfString = _createPdfString(longString, PdfStringEncoding.PdfDocEncoding, false);

        Assert.Equal(longString, pdfString.Value);
        Assert.Equal(PdfStringEncoding.PdfDocEncoding, pdfString.Encoding);
        Assert.False(pdfString.IsHex);
    }

    [Fact]
    public void Test_SpecialCharacters_HandledCorrectly()
    {
        var specialChars = "!@#$%^&*()_+-=[]{}|;':\",./<>?";
        var pdfString = _createPdfString(specialChars, PdfStringEncoding.PdfDocEncoding, false);

        Assert.Equal(specialChars, pdfString.Value);
        Assert.Equal(PdfStringEncoding.PdfDocEncoding, pdfString.Encoding);
        Assert.False(pdfString.IsHex);
    }

    [Fact]
    public void Test_WhitespaceString_HandledCorrectly()
    {
        var whitespace = "   \t\r\n   ";
        var pdfString = _createPdfString(whitespace, PdfStringEncoding.PdfDocEncoding, false);

        Assert.Equal(whitespace, pdfString.Value);
        Assert.Equal(PdfStringEncoding.PdfDocEncoding, pdfString.Encoding);
        Assert.False(pdfString.IsHex);
    }

    [Fact]
    public void Test_HashCodeConsistency()
    {
        var pdfString = _createPdfString("test", PdfStringEncoding.PdfDocEncoding, false);

        var hash1 = pdfString.GetHashCode();
        var hash2 = pdfString.GetHashCode();

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void Test_StringsInCollections()
    {
        var strings = new HashSet<PdfString>
        {
            _createPdfString("test", PdfStringEncoding.PdfDocEncoding, false),
            _createPdfString("test", PdfStringEncoding.Utf8, false),
            _createPdfString("test", PdfStringEncoding.PdfDocEncoding, true),
            _createPdfString("test", PdfStringEncoding.PdfDocEncoding, false) // Duplicate
        };

        Assert.Equal(3, strings.Count);
    }
}
