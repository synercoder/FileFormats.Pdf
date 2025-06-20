using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Parsing;
using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.FileFormats.Pdf.Primitives.Internal;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.Parsing;

public class ParserTests
{
    [Fact]
    public void Constructor_WithValidLexer_SetsLexerProperty()
    {
        var bytes = Encoding.ASCII.GetBytes("test");
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        
        var parser = new Parser(lexer);
        
        Assert.Same(lexer, parser.Lexer);
    }

    [Fact]
    public void Constructor_WithNullLexer_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Parser(null!));
    }

    [Theory]
    [InlineData("123", 123)]
    [InlineData("-456", -456)]
    [InlineData("0", 0)]
    [InlineData("999999999", 999999999)]
    public void ReadInteger_ValidIntegers_ReturnsCorrectValue(string input, long expected)
    {
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);
        
        var result = parser.ReadInteger();
        
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ReadInteger_FractionalNumber_ThrowsParseException()
    {
        var bytes = Encoding.ASCII.GetBytes("123.45");
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);
        
        var exception = Assert.Throws<ParseException>(() => parser.ReadInteger());
        Assert.Contains("Expected an integer", exception.Message);
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void ReadBoolean_ValidBooleans_ReturnsCorrectValue(string input, bool expected)
    {
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);
        
        var result = parser.ReadBoolean();
        
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ReadDictionary_SimpleDictionary_ReturnsCorrectDictionary()
    {
        var input = "<< /Type /Catalog /Pages 1 0 R >>";
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);
        
        var result = parser.ReadDictionary();
        
        Assert.NotNull(result);
        Assert.True(result.ContainsKey(PdfName.Get("Type")));
        Assert.True(result.ContainsKey(PdfName.Get("Pages")));
        
        var typeValue = result[PdfName.Get("Type")];
        Assert.IsType<PdfName>(typeValue);
        Assert.Equal("Catalog", ((PdfName)typeValue).Display);
        
        var pagesValue = result[PdfName.Get("Pages")];
        Assert.IsType<PdfReference>(pagesValue);
        var reference = (PdfReference)pagesValue;
        Assert.Equal(1, reference.Id.ObjectNumber);
        Assert.Equal(0, reference.Id.Generation);
    }

    [Fact]
    public void ReadDictionary_EmptyDictionary_ReturnsEmptyDictionary()
    {
        var input = "<< >>";
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);
        
        var result = parser.ReadDictionary();
        
        Assert.NotNull(result);
        Assert.Empty(result.Keys);
    }

    [Fact]
    public void ReadArray_SimpleArray_ReturnsCorrectArray()
    {
        var input = "[ 1 2 3 /Name true ]";
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);
        
        var result = parser.ReadArray();
        
        Assert.NotNull(result);
        Assert.Equal(5, result.Count);
        
        Assert.IsType<PdfNumber>(result[0]);
        Assert.Equal(1, ((PdfNumber)result[0]).Value);
        
        Assert.IsType<PdfNumber>(result[1]);
        Assert.Equal(2, ((PdfNumber)result[1]).Value);
        
        Assert.IsType<PdfNumber>(result[2]);
        Assert.Equal(3, ((PdfNumber)result[2]).Value);
        
        Assert.IsType<PdfName>(result[3]);
        Assert.Equal("Name", ((PdfName)result[3]).Display);
        
        Assert.IsType<PdfBoolean>(result[4]);
        Assert.True(((PdfBoolean)result[4]).Value);
    }

    [Fact]
    public void ReadArray_EmptyArray_ReturnsEmptyArray()
    {
        var input = "[ ]";
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);
        
        var result = parser.ReadArray();
        
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ReadArray_NestedArray_ReturnsCorrectArray()
    {
        var input = "[ 1 [ 2 3 ] 4 ]";
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);
        
        var result = parser.ReadArray();
        
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        
        Assert.IsType<PdfNumber>(result[0]);
        Assert.Equal(1, ((PdfNumber)result[0]).Value);
        
        Assert.IsAssignableFrom<IPdfArray>(result[1]);
        var nestedArray = (IPdfArray)result[1];
        Assert.Equal(2, nestedArray.Count);
        Assert.Equal(2, ((PdfNumber)nestedArray[0]).Value);
        Assert.Equal(3, ((PdfNumber)nestedArray[1]).Value);
        
        Assert.IsType<PdfNumber>(result[2]);
        Assert.Equal(4, ((PdfNumber)result[2]).Value);
    }

    [Theory]
    [InlineData("1 0 R", 1, 0)]
    [InlineData("42 3 R", 42, 3)]
    [InlineData("0 0 R", 0, 0)]
    public void ReadReference_ValidReference_ReturnsCorrectReference(string input, int expectedId, int expectedGeneration)
    {
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);
        
        var result = parser.ReadReference();
        
        Assert.Equal(expectedId, result.Id.ObjectNumber);
        Assert.Equal(expectedGeneration, result.Id.Generation);
    }

    [Fact]
    public void ReadStringHex_ValidHexString_ReturnsCorrectString()
    {
        var input = "<48656C6C6F>";
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);
        
        var result = parser.ReadStringHex();
        
        Assert.True(result.IsHex);
        Assert.Equal("Hello", result.Value);
    }

    [Fact]
    public void ReadStringLiteral_ValidLiteralString_ReturnsCorrectString()
    {
        var input = "(Hello World)";
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);
        
        var result = parser.ReadStringLiteral();
        
        Assert.False(result.IsHex);
        Assert.Equal("Hello World", result.Value);
    }

    [Fact]
    public void ReadStringLiteral_WithEscapeSequences_ReturnsCorrectString()
    {
        var input = @"(Line1\nLine2\tTabbed)";
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);
        
        var result = parser.ReadStringLiteral();
        
        Assert.False(result.IsHex);
        Assert.Contains("\n", result.Value);
        Assert.Contains("\t", result.Value);
    }

    [Theory]
    [InlineData("<< /Type /Catalog >>", typeof(IPdfDictionary))]
    [InlineData("[ 1 2 3 ]", typeof(IPdfArray))]
    [InlineData("true", typeof(PdfBoolean))]
    [InlineData("false", typeof(PdfBoolean))]
    [InlineData("123", typeof(PdfNumber))]
    [InlineData("123.45", typeof(PdfNumber))]
    [InlineData("(Hello)", typeof(PdfString))]
    [InlineData("<48656C6C6F>", typeof(PdfString))]
    [InlineData("/Name", typeof(PdfName))]
    [InlineData("null", typeof(PdfNull))]
    [InlineData("1 0 R", typeof(PdfReference))]
    public void ReadNext_VariousTokenTypes_ReturnsCorrectType(string input, Type expectedType)
    {
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);
        
        var result = parser.ReadNext();
        
        Assert.NotNull(result);
        Assert.IsAssignableFrom(expectedType, result);
    }

    [Fact]
    public void ReadNext_EndOfFile_ThrowsParseException()
    {
        var bytes = new byte[0];
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);

        Assert.Throws<UnexpectedEndOfFileException>(() => parser.ReadNext());
    }

    [Fact]
    public void ReadObject_Generic_ValidObject_ReturnsCorrectTypedObject()
    {
        var input = "1 0 obj\n<< /Type /Catalog >>\nendobj";
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);
        
        var result = parser.ReadObject<IPdfDictionary>();
        
        Assert.NotNull(result);
        Assert.Equal(1, result.Id.ObjectNumber);
        Assert.Equal(0, result.Id.Generation);
        Assert.IsAssignableFrom<IPdfDictionary>(result.Value);
        
        var dictionary = result.Value;
        Assert.True(dictionary.ContainsKey(PdfName.Get("Type")));
    }

    [Fact]
    public void ReadObject_Generic_WrongType_ThrowsParseException()
    {
        var input = "1 0 obj\n<< /Type /Catalog >>\nendobj";
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);
        
        var exception = Assert.Throws<ParseException>(() => parser.ReadObject<PdfString>());
        Assert.Contains("not of the correct type", exception.Message);
    }

    [Fact]
    public void ReadDictionaryOrStream_OnlyDictionary_ReturnsDictionary()
    {
        var input = "<< /Type /Catalog >>";
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);
        
        var result = parser.ReadDictionaryOrStream();
        
        Assert.NotNull(result);
        Assert.IsType<ReadOnlyPdfDictionary>(result);
        Assert.IsNotAssignableFrom<IPdfStreamObject>(result);
    }

    [Fact]
    public void ReadDictionaryOrStream_WithStream_ReturnsStream()
    {
        var streamContent = "Hello World";
        var input = $"<< /Length {streamContent.Length} >>\nstream\n{streamContent}\nendstream";
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);
        
        var result = parser.ReadDictionaryOrStream();
        
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IPdfStreamObject>(result);
        
        var stream = (IPdfStreamObject)result;
        Assert.True(stream.ContainsKey(PdfName.Get("Length")));
        Assert.Equal(streamContent.Length, stream.RawData.Length);
        Assert.Equal(streamContent, Encoding.ASCII.GetString(stream.RawData));
    }

    [Fact]
    public void ReadStreamObject_ValidStream_ReturnsStream()
    {
        var streamContent = "Hello World";
        var input = $"<< /Length {streamContent.Length} >>\nstream\n{streamContent}\nendstream";
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);
        
        var result = parser.ReadStreamObject();
        
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IPdfStreamObject>(result);
        Assert.Equal(streamContent.Length, result.RawData.Length);
        Assert.Equal(streamContent, Encoding.ASCII.GetString(result.RawData));
    }

    [Fact]
    public void ReadStreamObject_OnlyDictionary_ThrowsParseException()
    {
        var input = "<< /Type /Catalog >>";
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);
        
        var exception = Assert.Throws<ParseException>(() => parser.ReadStreamObject());
        Assert.Contains("stream token was expected", exception.Message);
    }

    [Fact]
    public void ReadDictionaryOrStream_StreamWithoutLength_ThrowsParseException()
    {
        var input = "<< /Type /Stream >>\nstream\nHello World\nendstream";
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);
        
        var exception = Assert.Throws<ParseException>(() => parser.ReadDictionaryOrStream());
        Assert.Contains("does not contain a Length property", exception.Message);
    }

    [Theory]
    [InlineData("123", typeof(PdfNumber), 123d)]
    [InlineData("123.45", typeof(PdfNumber), 123.45)]
    [InlineData("1 0 R", typeof(PdfReference), null)]
    public void ReadNext_NumbersAndReferences_ReturnsCorrectType(string input, Type expectedType, double? expectedValue)
    {
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);
        
        var result = parser.ReadNext();
        
        Assert.NotNull(result);
        Assert.IsType(expectedType, result);
        
        if (expectedValue != null)
        {
            if (result is PdfNumber realResult)
                Assert.Equal(expectedValue.Value, realResult.Value, precision: 6);
        }
        else if (result is PdfReference referenceResult)
        {
            Assert.Equal(1, referenceResult.Id.ObjectNumber);
            Assert.Equal(0, referenceResult.Id.Generation);
        }
    }

    [Fact]
    public void ReadNext_ComplexDictionary_HandlesCorrectly()
    {
        var input = "<< /Type /Catalog /Pages 1 0 R /Names << /Dests 2 0 R >> /ViewerPreferences << /HideToolbar true >> >>";
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);
        
        var result = parser.ReadNext();
        
        Assert.NotNull(result);
        Assert.IsType<ReadOnlyPdfDictionary>(result);
        
        var dictionary = (IPdfDictionary)result;
        Assert.True(dictionary.ContainsKey(PdfName.Get("Type")));
        Assert.True(dictionary.ContainsKey(PdfName.Get("Pages")));
        Assert.True(dictionary.ContainsKey(PdfName.Get("Names")));
        Assert.True(dictionary.ContainsKey(PdfName.Get("ViewerPreferences")));
        
        var namesValue = dictionary[PdfName.Get("Names")];
        Assert.IsType<ReadOnlyPdfDictionary>(namesValue);
        
        var viewerPrefsValue = dictionary[PdfName.Get("ViewerPreferences")];
        Assert.IsType<ReadOnlyPdfDictionary>(viewerPrefsValue);
        var viewerPrefs = (IPdfDictionary)viewerPrefsValue;
        Assert.True(viewerPrefs.ContainsKey(PdfName.Get("HideToolbar")));
        var hideToolbar = viewerPrefs[PdfName.Get("HideToolbar")];
        Assert.IsType<PdfBoolean>(hideToolbar);
        Assert.True(((PdfBoolean)hideToolbar).Value);
    }

    [Fact]
    public void ReadNext_UnsupportedTokenType_ThrowsParseException()
    {
        var input = "obj";
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);

        Assert.Throws<UnexpectedTokenException>(() => parser.ReadNext());
    }

    [Fact]
    public void ReadStringHex_WrongTokenType_ThrowsUnexpectedTokenException()
    {
        var input = "(literal string)";
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);
        
        Assert.Throws<UnexpectedTokenException>(() => parser.ReadStringHex());
    }

    [Fact]
    public void ReadStringLiteral_WrongTokenType_ThrowsUnexpectedTokenException()
    {
        var input = "<48656C6C6F>";
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);
        
        Assert.Throws<UnexpectedTokenException>(() => parser.ReadStringLiteral());
    }

    [Fact]
    public void ReadString_Utf16BE_BOM_HandlesCorrectly()
    {
        var unicodeText = "Hello";
        var utf16Bytes = Encoding.BigEndianUnicode.GetBytes(unicodeText);
        var bomBytes = new byte[] { 0xFE, 0xFF };
        var fullBytes = bomBytes.Concat(utf16Bytes).ToArray();
        
        var hexString = "<" + Convert.ToHexString(fullBytes) + ">";
        var bytes = Encoding.ASCII.GetBytes(hexString);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);
        
        var result = parser.ReadStringHex();
        
        Assert.NotNull(result);
        Assert.Equal(unicodeText, result.Value);
        Assert.Equal(PdfStringEncoding.Utf16BE, result.Encoding);
    }

    [Fact]
    public void ReadString_Utf16LE_BOM_HandlesCorrectly()
    {
        var unicodeText = "Hello";
        var utf16Bytes = Encoding.Unicode.GetBytes(unicodeText);
        var bomBytes = new byte[] { 0xFF, 0xFE };
        var fullBytes = bomBytes.Concat(utf16Bytes).ToArray();
        
        var hexString = "<" + Convert.ToHexString(fullBytes) + ">";
        var bytes = Encoding.ASCII.GetBytes(hexString);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);
        
        var result = parser.ReadStringHex();
        
        Assert.NotNull(result);
        Assert.Equal(unicodeText, result.Value);
        Assert.Equal(PdfStringEncoding.Utf16LE, result.Encoding);
    }

    [Fact]  
    public void ReadString_Utf8_BOM_HandlesCorrectly()
    {
        var unicodeText = "Hello";
        var utf8Bytes = Encoding.UTF8.GetBytes(unicodeText);
        var bomBytes = new byte[] { 0xEF, 0xBB, 0xBF };
        var fullBytes = bomBytes.Concat(utf8Bytes).ToArray();
        
        var hexString = "<" + Convert.ToHexString(fullBytes) + ">";
        var bytes = Encoding.ASCII.GetBytes(hexString);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);
        
        var result = parser.ReadStringHex();
        
        Assert.NotNull(result);
        Assert.Equal(unicodeText, result.Value);
        Assert.Equal(PdfStringEncoding.Utf8, result.Encoding);
    }

    [Fact]
    public void ReadString_PdfDocEncoding_HandlesCorrectly()
    {
        var input = "(Hello World)";
        var bytes = Encoding.ASCII.GetBytes(input);
        var provider = new PdfByteArrayProvider(bytes);
        var lexer = new Lexer(provider);
        var parser = new Parser(lexer);
        
        var result = parser.ReadStringLiteral();
        
        Assert.NotNull(result);
        Assert.Equal("Hello World", result.Value);
        Assert.Equal(PdfStringEncoding.PdfDocEncoding, result.Encoding);
    }
}
