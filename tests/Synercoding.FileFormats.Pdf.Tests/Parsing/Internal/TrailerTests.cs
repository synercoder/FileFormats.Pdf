using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.Parsing.Internal;
using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Tests.Parsing.Internal;

public class TrailerTests
{
    [Fact]
    public void Test_Constructor_WithNullDictionary_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Trailer(null!));
    }

    [Fact]
    public void Test_Constructor_WithValidDictionary_DoesNotThrow()
    {
        var dictionary = new PdfDictionary();
        var trailer = new Trailer(dictionary);

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
        var trailer = new Trailer(dictionary);

        var result = trailer.Size;

        Assert.Equal(sizeValue, result);
    }

    [Fact]
    public void Test_Size_WithMissingKey_ThrowsParseException()
    {
        var dictionary = new PdfDictionary();
        var trailer = new Trailer(dictionary);

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
        var trailer = new Trailer(dictionary);

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
        var trailer = new Trailer(dictionary);

        var result = trailer.Prev;

        Assert.Equal(prevValue, result);
    }

    [Fact]
    public void Test_Prev_WithMissingKey_ReturnsNull()
    {
        var dictionary = new PdfDictionary();
        var trailer = new Trailer(dictionary);

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
        var trailer = new Trailer(dictionary);

        var result = trailer.Root;

        Assert.Equal(expectedReference, result);
    }

    [Fact]
    public void Test_Root_WithMissingKey_ThrowsParseException()
    {
        var dictionary = new PdfDictionary();
        var trailer = new Trailer(dictionary);

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
        var trailer = new Trailer(dictionary);

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
        var trailer = new Trailer(dictionary);

        var result = trailer.Info;

        Assert.Equal(expectedReference, result);
    }

    [Fact]
    public void Test_ID_WithValidTwoElementArray_ReturnsArray()
    {
        var expectedArray = new PdfArray(new IPdfPrimitive[]
        {
            new PdfNumber(1),
            new PdfNumber(2)
        });
        var dictionary = new PdfDictionary()
        {
            [PdfNames.ID] = expectedArray
        };
        var trailer = new Trailer(dictionary);

        var result = trailer.ID;

        Assert.Equal(expectedArray, result);
    }

    [Fact]
    public void Test_ID_WithMissingKey_ReturnsNull()
    {
        var dictionary = new PdfDictionary();
        var trailer = new Trailer(dictionary);

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
            items[i] = new PdfNumber(i);

        var invalidArray = new PdfArray(items);
        var dictionary = new PdfDictionary()
        {
            [PdfNames.ID] = invalidArray
        };
        var trailer = new Trailer(dictionary);

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
        var trailer = new Trailer(dictionary);

        var result = trailer.XRefStm;

        Assert.Equal(xrefStmValue, result);
    }

    [Fact]
    public void Test_XRefStm_WithMissingKey_ReturnsNull()
    {
        var dictionary = new PdfDictionary();
        var trailer = new Trailer(dictionary);

        var result = trailer.XRefStm;

        Assert.Null(result);
    }
}
