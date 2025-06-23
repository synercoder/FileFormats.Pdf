using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Parsing;
using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.FileFormats.Pdf.Primitives.Extensions;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.Primitives.Extensions;

public class IPdfArrayExtensionsTests
{
    private static ObjectReader _createObjectReader()
    {
        var pdfContent = 
            "%PDF-1.7" + '\n' +
            "1 0 obj" + '\n' +
            "123" + '\n' +
            "endobj" + '\n' +
            "xref" + '\n' +
            "0 2" + '\n' +
            "0000000000 65535 f " + '\n' +
            "0000000009 00000 n " + '\n' +
            "trailer" + '\n' +
            "<< /Size 2 >>" + '\n' +
            "startxref" + '\n' +
            "28" + '\n' +
            "%%EOF";
        
        var bytes = Encoding.ASCII.GetBytes(pdfContent);
        var provider = new PdfByteArrayProvider(bytes);
        return new ObjectReader(provider, new ReaderSettings());
    }

    private static IPdfArray _createRectangleArray(double x, double y, double width, double height)
    {
        var array = new PdfArray();
        array.Add(new PdfNumber(x));           // llx
        array.Add(new PdfNumber(y));           // lly
        array.Add(new PdfNumber(x + width));   // urx
        array.Add(new PdfNumber(y + height));  // ury
        return array;
    }

    [Theory]
    [InlineData(0, 0, 612, 792)]
    [InlineData(100, 100, 500, 600)]
    [InlineData(-50, -50, 200, 300)]
    [InlineData(0.5, 0.5, 10.25, 20.75)]
    public void Test_TryGetAsRectangle_ValidArray_ReturnsTrue(double x, double y, double width, double height)
    {
        var objectReader = _createObjectReader();
        var array = _createRectangleArray(x, y, width, height);

        var result = array.TryGetAsRectangle(objectReader, out var rectangle);

        Assert.True(result);
        Assert.NotNull(rectangle);
        Assert.Equal(x, rectangle.Value.LLX.Raw);
        Assert.Equal(y, rectangle.Value.LLY.Raw);
        Assert.Equal(x + width, rectangle.Value.URX.Raw);
        Assert.Equal(y + height, rectangle.Value.URY.Raw);
        Assert.Equal(Unit.Points, rectangle.Value.LLX.Unit);
    }

    [Fact]
    public void Test_TryGetAsRectangle_NullArray_ThrowsArgumentNullException()
    {
        var objectReader = _createObjectReader();

        Assert.Throws<ArgumentNullException>(() => 
            ((IPdfArray)null!).TryGetAsRectangle(objectReader, out _));
    }

    [Theory]
    [InlineData(0)] // Empty array
    [InlineData(1)] // Too few elements
    [InlineData(2)] // Too few elements
    [InlineData(3)] // Too few elements
    [InlineData(5)] // Too many elements
    [InlineData(6)] // Too many elements
    public void Test_TryGetAsRectangle_InvalidElementCount_ReturnsFalse(int elementCount)
    {
        var objectReader = _createObjectReader();
        var array = new PdfArray();
        
        for (int i = 0; i < elementCount; i++)
        {
            array.Add(new PdfNumber(i * 10));
        }

        var result = array.TryGetAsRectangle(objectReader, out var rectangle);

        Assert.False(result);
        Assert.Null(rectangle);
    }

    [Theory]
    [InlineData(0)] // First element non-numeric
    [InlineData(1)] // Second element non-numeric
    [InlineData(2)] // Third element non-numeric
    [InlineData(3)] // Fourth element non-numeric
    public void Test_TryGetAsRectangle_NonNumericElement_ReturnsFalse(int nonNumericIndex)
    {
        var objectReader = _createObjectReader();
        var array = new PdfArray();
        
        for (int i = 0; i < 4; i++)
        {
            if (i == nonNumericIndex)
                array.Add(PdfNames.Type); // Non-numeric element
            else
                array.Add(new PdfNumber(i * 10));
        }

        var result = array.TryGetAsRectangle(objectReader, out var rectangle);

        Assert.False(result);
        Assert.Null(rectangle);
    }

    [Theory]
    [InlineData(1.0, 0, 0, 100, 100)]
    [InlineData(2.0, 50, 50, 200, 300)]
    [InlineData(72.0, 10, 20, 30, 40)]
    [InlineData(0.5, 100, 200, 400, 600)]
    [InlineData(144.0, 25, 25, 100, 150)]
    public void Test_TryGetAsRectangle_WithUserUnit_ReturnsCorrectUnit(double userUnit, double x, double y, double width, double height)
    {
        var objectReader = _createObjectReader();
        var array = _createRectangleArray(x, y, width, height);

        var result = array.TryGetAsRectangle(objectReader, userUnit, out var rectangle);

        Assert.True(result);
        Assert.NotNull(rectangle);
        Assert.Equal(x, rectangle.Value.LLX.Raw);
        Assert.Equal(y, rectangle.Value.LLY.Raw);
        Assert.Equal(x + width, rectangle.Value.URX.Raw);
        Assert.Equal(y + height, rectangle.Value.URY.Raw);
        
        if (userUnit == 1.0)
        {
            Assert.Equal(Unit.Points, rectangle.Value.LLX.Unit);
        }
        else
        {
            Assert.Equal(Unit.Pixels(userUnit / 72), rectangle.Value.LLX.Unit);
        }
    }

    [Fact]
    public void Test_TryGetAsRectangle_WithNullUserUnit_UsesPoints()
    {
        var objectReader = _createObjectReader();
        var array = _createRectangleArray(0, 0, 612, 792);

        var result = array.TryGetAsRectangle(objectReader, null, out var rectangle);

        Assert.True(result);
        Assert.NotNull(rectangle);
        Assert.Equal(Unit.Points, rectangle.Value.LLX.Unit);
    }

    [Fact]
    public void Test_TryGetAsRectangle_WithUserUnitOne_UsesPoints()
    {
        var objectReader = _createObjectReader();
        var array = _createRectangleArray(0, 0, 612, 792);

        var result = array.TryGetAsRectangle(objectReader, 1.0, out var rectangle);

        Assert.True(result);
        Assert.NotNull(rectangle);
        Assert.Equal(Unit.Points, rectangle.Value.LLX.Unit);
    }

    [Theory]
    [InlineData(0.0001)] // Very small UserUnit
    [InlineData(0.1)]    // Small UserUnit
    [InlineData(10.0)]   // Large UserUnit
    [InlineData(1000.0)] // Very large UserUnit
    public void Test_TryGetAsRectangle_WithExtremeUserUnits_HandlesCorrectly(double userUnit)
    {
        var objectReader = _createObjectReader();
        var array = _createRectangleArray(0, 0, 100, 100);

        var result = array.TryGetAsRectangle(objectReader, userUnit, out var rectangle);

        Assert.True(result);
        Assert.NotNull(rectangle);
        Assert.Equal(Unit.Pixels(userUnit / 72), rectangle.Value.LLX.Unit);
    }


    [Theory]
    [InlineData(-100, -200, 100, 200)] // Negative coordinates
    [InlineData(1000, 2000, -500, -1000)] // URX/URY less than LLX/LLY
    [InlineData(0, 0, 0, 0)] // Zero dimensions
    public void Test_TryGetAsRectangle_WithSpecialCoordinates_ReturnsTrue(double llx, double lly, double urx, double ury)
    {
        var objectReader = _createObjectReader();
        var array = new PdfArray();
        array.Add(new PdfNumber(llx));
        array.Add(new PdfNumber(lly));
        array.Add(new PdfNumber(urx));
        array.Add(new PdfNumber(ury));

        var result = array.TryGetAsRectangle(objectReader, out var rectangle);

        Assert.True(result);
        Assert.NotNull(rectangle);
        Assert.Equal(llx, rectangle.Value.LLX.Raw);
        Assert.Equal(lly, rectangle.Value.LLY.Raw);
        Assert.Equal(urx, rectangle.Value.URX.Raw);
        Assert.Equal(ury, rectangle.Value.URY.Raw);
    }

    [Theory]
    [InlineData(double.MaxValue)]
    [InlineData(double.MinValue)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    [InlineData(double.NaN)]
    public void Test_TryGetAsRectangle_WithExtremeDoubleValues_ReturnsTrue(double value)
    {
        var objectReader = _createObjectReader();
        var array = new PdfArray();
        array.Add(new PdfNumber(value));
        array.Add(new PdfNumber(0));
        array.Add(new PdfNumber(100));
        array.Add(new PdfNumber(200));

        var result = array.TryGetAsRectangle(objectReader, out var rectangle);

        Assert.True(result);
        Assert.NotNull(rectangle);
        Assert.Equal(value, rectangle.Value.LLX.Raw);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Test_TryGetValue_ValidIndex_ReturnsCorrectValue(int index)
    {
        var array = new PdfArray();
        array.Add(new PdfNumber(10));
        array.Add(new PdfNumber(20));
        array.Add(new PdfNumber(30));
        array.Add(new PdfNumber(40));

        var result = array.TryGetValue<PdfNumber>(index, out var value);

        Assert.True(result);
        Assert.Equal((index + 1) * 10, value.Value);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(100)]
    public void Test_TryGetValue_IndexOutOfRange_ThrowsIndexOutOfRangeException(int index)
    {
        var array = new PdfArray();
        array.Add(new PdfNumber(10));
        array.Add(new PdfNumber(20));

        Assert.Throws<IndexOutOfRangeException>(() => 
            array.TryGetValue<PdfNumber>(index, out _));
    }

    [Fact]
    public void Test_TryGetValue_WrongType_ReturnsFalse()
    {
        var array = new PdfArray();
        array.Add(new PdfNumber(10));
        array.Add(PdfNames.Type);

        var result = array.TryGetValue<PdfNumber>(1, out var value);

        Assert.False(result);
    }

    [Fact]
    public void Test_TryGetValue_WithObjectReader_ValidIndex_ReturnsCorrectValue()
    {
        var objectReader = _createObjectReader();
        var array = new PdfArray();
        array.Add(new PdfNumber(10));
        array.Add(new PdfNumber(20));

        var result = array.TryGetValue<PdfNumber>(0, objectReader, out var value);

        Assert.True(result);
        Assert.Equal(10, value.Value);
    }

    [Fact]
    public void Test_TryGetValue_WithObjectReader_IndexOutOfRange_ThrowsIndexOutOfRangeException()
    {
        var objectReader = _createObjectReader();
        var array = new PdfArray();
        array.Add(new PdfNumber(10));

        Assert.Throws<IndexOutOfRangeException>(() => 
            array.TryGetValue<PdfNumber>(5, objectReader, out _));
    }

    [Fact]
    public void Test_TryGetValue_WithObjectReader_WrongType_ReturnsFalse()
    {
        var objectReader = _createObjectReader();
        var array = new PdfArray();
        array.Add(PdfNames.Type);

        var result = array.TryGetValue<PdfNumber>(0, objectReader, out var value);

        Assert.False(result);
    }
}
