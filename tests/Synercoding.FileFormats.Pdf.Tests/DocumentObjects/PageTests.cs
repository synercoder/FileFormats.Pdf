using Synercoding.FileFormats.Pdf.DocumentObjects;
using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Parsing;
using Synercoding.FileFormats.Pdf.Primitives;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.DocumentObjects;

public class PageTests
{
    private static ObjectReader _createObjectReaderWithPagePdf()
    {
        var pdfContent = """
            %PDF-1.7
            1 0 obj
            << /Type /Page /MediaBox [0 0 612 792] >>
            endobj
            xref
            0 2
            0000000000 65535 f 
            0000000010 00000 n 
            trailer
            << /Size 2 >>
            startxref
            10
            %%EOF
            """;
        
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

        var page = new Page(pageId, pageDict, objectReader);

        Assert.Equal(pageId, page.Id);
        Assert.Null(page.Parent);
    }

    [Fact] 
    public void Test_Constructor_NullDictionary_ThrowsArgumentNullException()
    {
        var objectReader = _createObjectReaderWithPagePdf();
        var pageId = new PdfObjectId(1, 0);

        Assert.Throws<NullReferenceException>(() => new Page(pageId, null!, objectReader));
    }

    [Fact]
    public void Test_Constructor_NullObjectReader_ThrowsArgumentNullException()
    {
        var pageDict = _createValidPageDictionary();
        var pageId = new PdfObjectId(1, 0);

        Assert.Throws<NullReferenceException>(() => new Page(pageId, pageDict, null!));
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

        var page = new Page(pageId, pageDict, objectReader);

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

        var page = new Page(pageId, pageDict, objectReader);

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

        var page = new Page(pageId, pageDict, objectReader);

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

        var page = new Page(pageId, pageDict, objectReader);

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

        var page = new Page(pageId, pageDict, objectReader);

        Assert.Equal(rotation, page.Rotate);
    }

    [Fact]
    public void Test_Rotate_NotPresent_ReturnsNull()
    {
        var objectReader = _createObjectReaderWithPagePdf();
        var pageDict = _createValidPageDictionary();
        pageDict.Add(PdfNames.MediaBox, _createRectangleArray(0, 0, 612, 792));
        var pageId = new PdfObjectId(1, 0);

        var page = new Page(pageId, pageDict, objectReader);

        Assert.Null(page.Rotate);
    }
}
