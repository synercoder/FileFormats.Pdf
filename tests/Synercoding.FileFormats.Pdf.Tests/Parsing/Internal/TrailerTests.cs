using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.Parsing.Internal;
using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Tests.Parsing.Internal;

public class TrailerTests
{
    [Fact]
    public void Test_Constructor_WithNullDictionary_ThrowsArgumentNullException()
    {
        var readerSettings = new ReaderSettings();
        Assert.Throws<ArgumentNullException>(() => new Trailer(null!, readerSettings));
    }

    [Fact]
    public void Test_Constructor_WithValidDictionary_DoesNotThrow()
    {
        var dictionary = new PdfDictionary();
        var readerSettings = new ReaderSettings();
        var trailer = new Trailer(dictionary, readerSettings);

        Assert.NotNull(trailer);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(int.MaxValue)]
    public void Test_Size_WithValidValue_ReturnsValue(int sizeValue)
    {
        var dictionary = new PdfDictionary()
        {
            [PdfNames.Size] = new PdfNumber(sizeValue)
        };
        var readerSettings = new ReaderSettings();
        var trailer = new Trailer(dictionary, readerSettings);

        var result = trailer.Size;

        Assert.Equal(sizeValue, result);
    }

    [Fact]
    public void Test_Size_WithMissingKey_ThrowsParseException()
    {
        var dictionary = new PdfDictionary();
        var readerSettings = new ReaderSettings();
        var trailer = new Trailer(dictionary, readerSettings);

        var exception = Assert.Throws<ParseException>(() => trailer.Size);
        Assert.Contains("/Size", exception.Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(int.MinValue)]
    public void Test_Size_WithNegativeValue_ThrowsParseException(int negativeValue)
    {
        var dictionary = new PdfDictionary()
        {
            [PdfNames.Size] = new PdfNumber(negativeValue)
        };
        var readerSettings = new ReaderSettings();
        var trailer = new Trailer(dictionary, readerSettings);

        var exception = Assert.Throws<ParseException>(() => trailer.Size);
        Assert.Contains("/Size", exception.Message);
        Assert.Contains("zero or higher", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    [InlineData(long.MaxValue)]
    public void Test_Prev_WithValidValue_ReturnsValue(long prevValue)
    {
        var dictionary = new PdfDictionary()
        {
            [PdfNames.Prev] = new PdfNumber(prevValue)
        };
        var readerSettings = new ReaderSettings();
        var trailer = new Trailer(dictionary, readerSettings);

        var result = trailer.Prev;

        Assert.Equal(prevValue, result);
    }

    [Fact]
    public void Test_Prev_WithMissingKey_ReturnsNull()
    {
        var dictionary = new PdfDictionary();
        var readerSettings = new ReaderSettings();
        var trailer = new Trailer(dictionary, readerSettings);

        var result = trailer.Prev;

        Assert.Null(result);
    }

    [Fact]
    public void Test_Root_WithValidReference_ReturnsReference()
    {
        var expectedReference = new PdfReference { Id = new PdfObjectId(1, 0) };
        var dictionary = new PdfDictionary()
        {
            [PdfNames.Root] = expectedReference
        };
        var readerSettings = new ReaderSettings();
        var trailer = new Trailer(dictionary, readerSettings);

        var result = trailer.Root;

        Assert.Equal(expectedReference, result);
    }

    [Fact]
    public void Test_Root_WithMissingKey_ThrowsParseException()
    {
        var dictionary = new PdfDictionary();
        var readerSettings = new ReaderSettings();
        var trailer = new Trailer(dictionary, readerSettings);

        var exception = Assert.Throws<ParseException>(() => trailer.Root);
        Assert.Contains("/Root", exception.Message);
    }

    [Fact]
    public void Test_Encrypt_WithValidValue_ReturnsValue()
    {
        var expectedValue = new PdfNumber(42);
        var dictionary = new PdfDictionary()
        {
            [PdfNames.Encrypt] = expectedValue
        };
        var readerSettings = new ReaderSettings();
        var trailer = new Trailer(dictionary, readerSettings);

        var result = trailer.Encrypt;

        Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void Test_Info_WithValidReference_ReturnsReference()
    {
        var expectedReference = new PdfReference { Id = new PdfObjectId(2, 0) };
        var dictionary = new PdfDictionary()
        {
            [PdfNames.Info] = expectedReference
        };
        var readerSettings = new ReaderSettings();
        var trailer = new Trailer(dictionary, readerSettings);

        var result = trailer.Info;

        Assert.Equal(expectedReference, result);
    }

    [Fact]
    public void Test_ID_WithValidTwoElementArray_ReturnsTuple()
    {
        var id1 = new PdfString(Convert.FromHexString("48656C6C6F"), true);
        var id2 = new PdfString(Convert.FromHexString("576F726C64"), true);
        var idArray = new PdfArray(new IPdfPrimitive[] { id1, id2 });
        var dictionary = new PdfDictionary()
        {
            [PdfNames.ID] = idArray
        };
        var readerSettings = new ReaderSettings();
        var trailer = new Trailer(dictionary, readerSettings);

        var result = trailer.ID;

        Assert.NotNull(result);
        Assert.Equal(id1.Raw, result.OriginalId);
        Assert.Equal(id2.Raw, result.LastVersionId);
    }

    [Fact]
    public void Test_ID_WithMissingKey_ReturnsNull()
    {
        var dictionary = new PdfDictionary();
        var readerSettings = new ReaderSettings();
        var trailer = new Trailer(dictionary, readerSettings);

        var result = trailer.ID;

        Assert.Null(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void Test_ID_WithIncorrectArraySize_ThrowsParseException(int arraySize)
    {
        var items = new IPdfPrimitive[arraySize];
        for (int i = 0; i < arraySize; i++)
            items[i] = new PdfString(Convert.FromHexString($"{i:X2}"), true);

        var invalidArray = new PdfArray(items);
        var dictionary = new PdfDictionary()
        {
            [PdfNames.ID] = invalidArray
        };
        var readerSettings = new ReaderSettings { Strict = true };
        var trailer = new Trailer(dictionary, readerSettings);

        var exception = Assert.Throws<ParseException>(() => trailer.ID);
        Assert.Contains("ID array should contain 2 values", exception.Message);
        Assert.Contains($"found array contains {arraySize} values", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    [InlineData(long.MaxValue)]
    public void Test_XRefStm_WithValidValue_ReturnsValue(long xrefStmValue)
    {
        var dictionary = new PdfDictionary() { [PdfNames.XRefStm] = new PdfNumber(xrefStmValue) };
        var readerSettings = new ReaderSettings();
        var trailer = new Trailer(dictionary, readerSettings);

        var result = trailer.XRefStm;

        Assert.Equal(xrefStmValue, result);
    }

    [Fact]
    public void Test_XRefStm_WithMissingKey_ReturnsNull()
    {
        var dictionary = new PdfDictionary();
        var readerSettings = new ReaderSettings();
        var trailer = new Trailer(dictionary, readerSettings);

        var result = trailer.XRefStm;

        Assert.Null(result);
    }
}
