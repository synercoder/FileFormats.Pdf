using Synercoding.FileFormats.Pdf.DocumentObjects.Internal;
using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Parsing;
using Synercoding.FileFormats.Pdf.Primitives;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.DocumentObjects.Internal;

public class ReadOnlyPageTests
{
    private static ObjectReader _createObjectReaderWithPagePdf()
    {
        var pdfContent = 
            "%PDF-1.7" + '\n' +
            "1 0 obj" + '\n' +
            "<< /Type /Page /MediaBox [0 0 612 792] >>" + '\n' +
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

    private static PdfDictionary _createValidPageDictionary()
    {
        var dictionary = new PdfDictionary();
        dictionary.Add(PdfNames.Type, PdfNames.Page);
        return dictionary;
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

    [Fact]
    public void Test_Constructor_ValidPageDictionary_CreatesInstance()
    {
        var objectReader = _createObjectReaderWithPagePdf();
        var pageDict = _createValidPageDictionary();
        var pageId = new PdfObjectId(1, 0);

        var page = new ReadOnlyPage(pageId, pageDict, objectReader);

        Assert.Equal(pageId, page.Id);
        Assert.Null(page.Parent);
    }

    [Fact] 
    public void Test_Constructor_NullDictionary_ThrowsArgumentNullException()
    {
        var objectReader = _createObjectReaderWithPagePdf();
        var pageId = new PdfObjectId(1, 0);

        Assert.Throws<NullReferenceException>(() => new ReadOnlyPage(pageId, null!, objectReader));
    }

    [Fact]
    public void Test_Constructor_NullObjectReader_ThrowsArgumentNullException()
    {
        var pageDict = _createValidPageDictionary();
        var pageId = new PdfObjectId(1, 0);

        Assert.Throws<NullReferenceException>(() => new ReadOnlyPage(pageId, pageDict, null!));
    }

    [Theory]
    [InlineData(0, 0, 612, 792)]
    [InlineData(100, 100, 500, 600)]
    [InlineData(-50, -50, 200, 300)]
    public void Test_MediaBox_FromPageDictionary_ReturnsCorrectRectangle(double x, double y, double width, double height)
    {
        var objectReader = _createObjectReaderWithPagePdf();
        var pageDict = _createValidPageDictionary();
        pageDict.Add(PdfNames.MediaBox, _createRectangleArray(x, y, width, height));
        var pageId = new PdfObjectId(1, 0);

        var page = new ReadOnlyPage(pageId, pageDict, objectReader);

        var mediaBox = page.MediaBox;
        Assert.Equal(x, mediaBox.LLX.Raw);
        Assert.Equal(y, mediaBox.LLY.Raw);
        Assert.Equal(x + width, mediaBox.URX.Raw);
        Assert.Equal(y + height, mediaBox.URY.Raw);
    }

    [Fact]
    public void Test_MediaBox_NoMediaBoxAnywhere_ThrowsParseException()
    {
        var objectReader = _createObjectReaderWithPagePdf();
        var pageDict = _createValidPageDictionary();
        var pageId = new PdfObjectId(1, 0);

        var page = new ReadOnlyPage(pageId, pageDict, objectReader);

        var exception = Assert.Throws<ParseException>(() => page.MediaBox);
        Assert.Contains("did not have a required /MediaBox", exception.Message);
    }

    [Theory]
    [InlineData(50, 50, 500, 700)]
    [InlineData(0, 0, 612, 792)]
    public void Test_CropBox_FromPageDictionary_ReturnsCorrectRectangle(double x, double y, double width, double height)
    {
        var objectReader = _createObjectReaderWithPagePdf();
        var pageDict = _createValidPageDictionary();
        pageDict.Add(PdfNames.MediaBox, _createRectangleArray(0, 0, 612, 792));
        pageDict.Add(PdfNames.CropBox, _createRectangleArray(x, y, width, height));
        var pageId = new PdfObjectId(1, 0);

        var page = new ReadOnlyPage(pageId, pageDict, objectReader);

        var cropBox = page.CropBox;
        Assert.True(cropBox.HasValue);
        Assert.Equal(x, cropBox.Value.LLX.Raw);
        Assert.Equal(y, cropBox.Value.LLY.Raw);
        Assert.Equal(x + width, cropBox.Value.URX.Raw);
        Assert.Equal(y + height, cropBox.Value.URY.Raw);
    }

    [Fact]
    public void Test_CropBox_NotPresent_ReturnsNull()
    {
        var objectReader = _createObjectReaderWithPagePdf();
        var pageDict = _createValidPageDictionary();
        pageDict.Add(PdfNames.MediaBox, _createRectangleArray(0, 0, 612, 792));
        var pageId = new PdfObjectId(1, 0);

        var page = new ReadOnlyPage(pageId, pageDict, objectReader);

        Assert.Null(page.CropBox);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(90)]
    [InlineData(180)]
    [InlineData(270)]
    [InlineData(360)]
    [InlineData(-90)]
    public void Test_Rotate_ValidRotation_ReturnsCorrectValue(int rotation)
    {
        var objectReader = _createObjectReaderWithPagePdf();
        var pageDict = _createValidPageDictionary();
        pageDict.Add(PdfNames.MediaBox, _createRectangleArray(0, 0, 612, 792));
        pageDict.Add(PdfNames.Rotate, new PdfNumber(rotation));
        var pageId = new PdfObjectId(1, 0);

        var page = new ReadOnlyPage(pageId, pageDict, objectReader);

        Assert.Equal(rotation, page.Rotate);
    }

    [Fact]
    public void Test_Rotate_NotPresent_ReturnsNull()
    {
        var objectReader = _createObjectReaderWithPagePdf();
        var pageDict = _createValidPageDictionary();
        pageDict.Add(PdfNames.MediaBox, _createRectangleArray(0, 0, 612, 792));
        var pageId = new PdfObjectId(1, 0);

        var page = new ReadOnlyPage(pageId, pageDict, objectReader);

        Assert.Null(page.Rotate);
    }

    [Theory]
    [InlineData(1.0)]
    [InlineData(2.0)]
    [InlineData(72.0)]
    [InlineData(0.5)]
    [InlineData(144.0)]
    public void Test_UserUnit_ValidValue_ReturnsCorrectValue(double userUnit)
    {
        var objectReader = _createObjectReaderWithPagePdf();
        var pageDict = _createValidPageDictionary();
        pageDict.Add(PdfNames.MediaBox, _createRectangleArray(0, 0, 612, 792));
        pageDict.Add(PdfNames.UserUnit, new PdfNumber(userUnit));
        var pageId = new PdfObjectId(1, 0);

        var page = new ReadOnlyPage(pageId, pageDict, objectReader);

        Assert.Equal(userUnit, page.UserUnit);
    }

    [Fact]
    public void Test_UserUnit_NotPresent_ReturnsNull()
    {
        var objectReader = _createObjectReaderWithPagePdf();
        var pageDict = _createValidPageDictionary();
        pageDict.Add(PdfNames.MediaBox, _createRectangleArray(0, 0, 612, 792));
        var pageId = new PdfObjectId(1, 0);

        var page = new ReadOnlyPage(pageId, pageDict, objectReader);

        Assert.Null(page.UserUnit);
    }

    [Theory]
    [InlineData(2.0, 0, 0, 100, 100)]
    [InlineData(72.0, 50, 50, 200, 300)]
    [InlineData(0.5, 100, 200, 400, 600)]
    public void Test_MediaBox_WithUserUnit_ReturnsCorrectRectangle(double userUnit, double x, double y, double width, double height)
    {
        var objectReader = _createObjectReaderWithPagePdf();
        var pageDict = _createValidPageDictionary();
        pageDict.Add(PdfNames.MediaBox, _createRectangleArray(x, y, width, height));
        pageDict.Add(PdfNames.UserUnit, new PdfNumber(userUnit));
        var pageId = new PdfObjectId(1, 0);

        var page = new ReadOnlyPage(pageId, pageDict, objectReader);

        var mediaBox = page.MediaBox;
        Assert.Equal(x, mediaBox.LLX.Raw);
        Assert.Equal(y, mediaBox.LLY.Raw);
        Assert.Equal(x + width, mediaBox.URX.Raw);
        Assert.Equal(y + height, mediaBox.URY.Raw);
        Assert.Equal(Unit.Pixels(userUnit / 72), mediaBox.LLX.Unit);
    }

    [Theory]
    [InlineData(2.0, 10, 10, 50, 50)]
    [InlineData(144.0, 25, 25, 100, 150)]
    public void Test_CropBox_WithUserUnit_ReturnsCorrectRectangle(double userUnit, double x, double y, double width, double height)
    {
        var objectReader = _createObjectReaderWithPagePdf();
        var pageDict = _createValidPageDictionary();
        pageDict.Add(PdfNames.MediaBox, _createRectangleArray(0, 0, 612, 792));
        pageDict.Add(PdfNames.CropBox, _createRectangleArray(x, y, width, height));
        pageDict.Add(PdfNames.UserUnit, new PdfNumber(userUnit));
        var pageId = new PdfObjectId(1, 0);

        var page = new ReadOnlyPage(pageId, pageDict, objectReader);

        var cropBox = page.CropBox;
        Assert.True(cropBox.HasValue);
        Assert.Equal(x, cropBox.Value.LLX.Raw);
        Assert.Equal(y, cropBox.Value.LLY.Raw);
        Assert.Equal(x + width, cropBox.Value.URX.Raw);
        Assert.Equal(y + height, cropBox.Value.URY.Raw);
        Assert.Equal(Unit.Pixels(userUnit / 72), cropBox.Value.LLX.Unit);
    }

    [Theory]
    [InlineData(3.0, 5, 5, 30, 40)]
    [InlineData(36.0, 20, 30, 80, 120)]
    public void Test_BleedBox_WithUserUnit_ReturnsCorrectRectangle(double userUnit, double x, double y, double width, double height)
    {
        var objectReader = _createObjectReaderWithPagePdf();
        var pageDict = _createValidPageDictionary();
        pageDict.Add(PdfNames.MediaBox, _createRectangleArray(0, 0, 612, 792));
        pageDict.Add(PdfNames.BleedBox, _createRectangleArray(x, y, width, height));
        pageDict.Add(PdfNames.UserUnit, new PdfNumber(userUnit));
        var pageId = new PdfObjectId(1, 0);

        var page = new ReadOnlyPage(pageId, pageDict, objectReader);

        var bleedBox = page.BleedBox;
        Assert.True(bleedBox.HasValue);
        Assert.Equal(x, bleedBox.Value.LLX.Raw);
        Assert.Equal(y, bleedBox.Value.LLY.Raw);
        Assert.Equal(x + width, bleedBox.Value.URX.Raw);
        Assert.Equal(y + height, bleedBox.Value.URY.Raw);
        Assert.Equal(Unit.Pixels(userUnit / 72), bleedBox.Value.LLX.Unit);
    }

    [Theory]
    [InlineData(4.0, 15, 15, 60, 80)]
    [InlineData(18.0, 40, 50, 120, 160)]
    public void Test_TrimBox_WithUserUnit_ReturnsCorrectRectangle(double userUnit, double x, double y, double width, double height)
    {
        var objectReader = _createObjectReaderWithPagePdf();
        var pageDict = _createValidPageDictionary();
        pageDict.Add(PdfNames.MediaBox, _createRectangleArray(0, 0, 612, 792));
        pageDict.Add(PdfNames.TrimBox, _createRectangleArray(x, y, width, height));
        pageDict.Add(PdfNames.UserUnit, new PdfNumber(userUnit));
        var pageId = new PdfObjectId(1, 0);

        var page = new ReadOnlyPage(pageId, pageDict, objectReader);

        var trimBox = page.TrimBox;
        Assert.True(trimBox.HasValue);
        Assert.Equal(x, trimBox.Value.LLX.Raw);
        Assert.Equal(y, trimBox.Value.LLY.Raw);
        Assert.Equal(x + width, trimBox.Value.URX.Raw);
        Assert.Equal(y + height, trimBox.Value.URY.Raw);
        Assert.Equal(Unit.Pixels(userUnit / 72), trimBox.Value.LLX.Unit);
    }

    [Theory]
    [InlineData(6.0, 25, 25, 90, 110)]
    [InlineData(12.0, 60, 70, 150, 200)]
    public void Test_ArtBox_WithUserUnit_ReturnsCorrectRectangle(double userUnit, double x, double y, double width, double height)
    {
        var objectReader = _createObjectReaderWithPagePdf();
        var pageDict = _createValidPageDictionary();
        pageDict.Add(PdfNames.MediaBox, _createRectangleArray(0, 0, 612, 792));
        pageDict.Add(PdfNames.ArtBox, _createRectangleArray(x, y, width, height));
        pageDict.Add(PdfNames.UserUnit, new PdfNumber(userUnit));
        var pageId = new PdfObjectId(1, 0);

        var page = new ReadOnlyPage(pageId, pageDict, objectReader);

        var artBox = page.ArtBox;
        Assert.True(artBox.HasValue);
        Assert.Equal(x, artBox.Value.LLX.Raw);
        Assert.Equal(y, artBox.Value.LLY.Raw);
        Assert.Equal(x + width, artBox.Value.URX.Raw);
        Assert.Equal(y + height, artBox.Value.URY.Raw);
        Assert.Equal(Unit.Pixels(userUnit / 72), artBox.Value.LLX.Unit);
    }

    [Fact]
    public void Test_MediaBox_UserUnitIsOne_UsesNormalPoints()
    {
        var objectReader = _createObjectReaderWithPagePdf();
        var pageDict = _createValidPageDictionary();
        pageDict.Add(PdfNames.MediaBox, _createRectangleArray(0, 0, 612, 792));
        pageDict.Add(PdfNames.UserUnit, new PdfNumber(1.0));
        var pageId = new PdfObjectId(1, 0);

        var page = new ReadOnlyPage(pageId, pageDict, objectReader);

        var mediaBox = page.MediaBox;
        Assert.Equal(Unit.Points, mediaBox.LLX.Unit);
    }

    [Fact]
    public void Test_CropBox_UserUnitIsOne_UsesNormalPoints()
    {
        var objectReader = _createObjectReaderWithPagePdf();
        var pageDict = _createValidPageDictionary();
        pageDict.Add(PdfNames.MediaBox, _createRectangleArray(0, 0, 612, 792));
        pageDict.Add(PdfNames.CropBox, _createRectangleArray(50, 50, 500, 700));
        pageDict.Add(PdfNames.UserUnit, new PdfNumber(1.0));
        var pageId = new PdfObjectId(1, 0);

        var page = new ReadOnlyPage(pageId, pageDict, objectReader);

        var cropBox = page.CropBox;
        Assert.True(cropBox.HasValue);
        Assert.Equal(Unit.Points, cropBox.Value.LLX.Unit);
    }
}
