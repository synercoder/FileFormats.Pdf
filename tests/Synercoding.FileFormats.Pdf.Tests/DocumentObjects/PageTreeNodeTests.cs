using Synercoding.FileFormats.Pdf.DocumentObjects;
using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Parsing;
using Synercoding.FileFormats.Pdf.Primitives;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.DocumentObjects;

public class PageTreeNodeTests
{
    private static ObjectReader _createObjectReaderWithPageTreePdf()
    {
        var pdfContent =
            "%PDF-1.7" + '\n' +
            "1 0 obj" + '\n' +
            "<< /Type /Pages /Count 0 /Kids [] >>" + '\n' +
            "endobj" + '\n' +
            "xref" + '\n' +
            "0 2" + '\n' +
            "0000000000 65535 f " + '\n' +
            "0000000009 00000 n " + '\n' +
            "trailer" + '\n' +
            "<< /Size 2 >>" + '\n' +
            "startxref" + '\n' +
            "61" + '\n' +
            "%%EOF";
        
        var bytes = Encoding.ASCII.GetBytes(pdfContent);
        var provider = new PdfByteArrayProvider(bytes);
        return new ObjectReader(provider, new ReaderSettings());
    }

    private static PdfDictionary _createValidPageTreeDictionary(int count = 0)
    {
        var dictionary = new PdfDictionary();
        dictionary.Add(PdfNames.Type, PdfNames.Pages);
        dictionary.Add(PdfNames.Count, new PdfNumber(count));
        dictionary.Add(PdfNames.Kids, new PdfArray());
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
    public void Test_GetRoot_ValidReference_ReturnsPageTreeNode()
    {
        var objectReader = _createObjectReaderWithPageTreePdf();
        var pageTreeId = new PdfObjectId(1, 0);
        var reference = new PdfReference { Id = pageTreeId };

        var root = PageTreeNode.GetRoot(reference, objectReader);

        Assert.NotNull(root);
        Assert.Equal(pageTreeId, root.Id);
        Assert.Null(root.Parent);
    }

    [Fact]
    public void Test_GetRoot_NonExistentReference_ThrowsParseException()
    {
        var objectReader = _createObjectReaderWithPageTreePdf();
        var pageTreeId = new PdfObjectId(999, 0);
        var reference = new PdfReference { Id = pageTreeId };

        var exception = Assert.Throws<ParseException>(() => PageTreeNode.GetRoot(reference, objectReader));
        Assert.Contains("Could not retrieve a dictionary with reference", exception.Message);
    }

    [Fact]
    public void Test_Constructor_ValidDictionary_CreatesInstance()
    {
        var objectReader = _createObjectReaderWithPageTreePdf();
        var pageTreeDict = _createValidPageTreeDictionary();
        var pageTreeId = new PdfObjectId(1, 0);

        var node = new PageTreeNode(pageTreeId, pageTreeDict, objectReader);

        Assert.Equal(pageTreeId, node.Id);
        Assert.Null(node.Parent);
    }

    [Fact]
    public void Test_Constructor_NullDictionary_ThrowsArgumentNullException()
    {
        var objectReader = _createObjectReaderWithPageTreePdf();
        var pageTreeId = new PdfObjectId(1, 0);

        Assert.Throws<ArgumentNullException>(() => new PageTreeNode(pageTreeId, null!, objectReader));
    }

    [Fact]
    public void Test_Constructor_NullObjectReader_ThrowsArgumentNullException()
    {
        var pageTreeDict = _createValidPageTreeDictionary();
        var pageTreeId = new PdfObjectId(1, 0);

        Assert.Throws<ArgumentNullException>(() => new PageTreeNode(pageTreeId, pageTreeDict, null!));
    }

    [Fact]
    public void Test_Kids_EmptyKidsArray_ReturnsEmptyList()
    {
        var objectReader = _createObjectReaderWithPageTreePdf();
        var pageTreeDict = _createValidPageTreeDictionary();
        var pageTreeId = new PdfObjectId(1, 0);

        var node = new PageTreeNode(pageTreeId, pageTreeDict, objectReader);

        Assert.Empty(node.Kids);
    }

    [Fact]
    public void Test_Kids_MissingKidsArray_ThrowsParseException()
    {
        var objectReader = _createObjectReaderWithPageTreePdf();
        var pageTreeDict = new PdfDictionary();
        pageTreeDict.Add(PdfNames.Type, PdfNames.Pages);
        pageTreeDict.Add(PdfNames.Count, new PdfNumber(0));
        var pageTreeId = new PdfObjectId(1, 0);

        var node = new PageTreeNode(pageTreeId, pageTreeDict, objectReader);

        var exception = Assert.Throws<ParseException>(() => node.Kids);
        Assert.Contains("does not contain the required key /Kids", exception.Message);
    }

    [Fact]
    public void Test_Kids_CachesResult()
    {
        var objectReader = _createObjectReaderWithPageTreePdf();
        var pageTreeDict = _createValidPageTreeDictionary();
        var pageTreeId = new PdfObjectId(1, 0);

        var node = new PageTreeNode(pageTreeId, pageTreeDict, objectReader);

        var kids1 = node.Kids;
        var kids2 = node.Kids;

        Assert.Same(kids1, kids2);
    }

    [Fact]
    public void Test_Count_EmptyTree_ReturnsZero()
    {
        var objectReader = _createObjectReaderWithPageTreePdf();
        var pageTreeDict = _createValidPageTreeDictionary(0);
        var pageTreeId = new PdfObjectId(1, 0);

        var node = new PageTreeNode(pageTreeId, pageTreeDict, objectReader);

        Assert.Equal(0, node.Count);
    }

    [Theory]
    [InlineData(0, 0, 612, 792)]
    [InlineData(100, 100, 500, 600)]
    public void Test_MediaBox_FromDictionary_ReturnsCorrectRectangle(double x, double y, double width, double height)
    {
        var objectReader = _createObjectReaderWithPageTreePdf();
        var pageTreeDict = _createValidPageTreeDictionary();
        pageTreeDict.Add(PdfNames.MediaBox, _createRectangleArray(x, y, width, height));
        var pageTreeId = new PdfObjectId(1, 0);

        var node = new PageTreeNode(pageTreeId, pageTreeDict, objectReader);

        var mediaBox = node.MediaBox;
        Assert.True(mediaBox.HasValue);
        Assert.Equal(x, mediaBox.Value.LLX.Raw);
        Assert.Equal(y, mediaBox.Value.LLY.Raw);
        Assert.Equal(x + width, mediaBox.Value.URX.Raw);
        Assert.Equal(y + height, mediaBox.Value.URY.Raw);
    }

    [Fact]
    public void Test_MediaBox_NotPresent_ReturnsNull()
    {
        var objectReader = _createObjectReaderWithPageTreePdf();
        var pageTreeDict = _createValidPageTreeDictionary();
        var pageTreeId = new PdfObjectId(1, 0);

        var node = new PageTreeNode(pageTreeId, pageTreeDict, objectReader);

        Assert.Null(node.MediaBox);
    }

    [Theory]
    [InlineData(50, 50, 500, 700)]
    [InlineData(0, 0, 612, 792)]
    public void Test_CropBox_FromDictionary_ReturnsCorrectRectangle(double x, double y, double width, double height)
    {
        var objectReader = _createObjectReaderWithPageTreePdf();
        var pageTreeDict = _createValidPageTreeDictionary();
        pageTreeDict.Add(PdfNames.CropBox, _createRectangleArray(x, y, width, height));
        var pageTreeId = new PdfObjectId(1, 0);

        var node = new PageTreeNode(pageTreeId, pageTreeDict, objectReader);

        var cropBox = node.CropBox;
        Assert.True(cropBox.HasValue);
        Assert.Equal(x, cropBox.Value.LLX.Raw);
        Assert.Equal(y, cropBox.Value.LLY.Raw);
        Assert.Equal(x + width, cropBox.Value.URX.Raw);
        Assert.Equal(y + height, cropBox.Value.URY.Raw);
    }

    [Fact]
    public void Test_CropBox_NotPresent_ReturnsNull()
    {
        var objectReader = _createObjectReaderWithPageTreePdf();
        var pageTreeDict = _createValidPageTreeDictionary();
        var pageTreeId = new PdfObjectId(1, 0);

        var node = new PageTreeNode(pageTreeId, pageTreeDict, objectReader);

        Assert.Null(node.CropBox);
    }

    [Fact]
    public void Test_Resources_FromDictionary_ReturnsCorrectDictionary()
    {
        var objectReader = _createObjectReaderWithPageTreePdf();
        var pageTreeDict = _createValidPageTreeDictionary();
        var resourcesDict = new PdfDictionary();
        resourcesDict.Add(PdfName.Get("Font"), new PdfDictionary());
        pageTreeDict.Add(PdfNames.Resources, resourcesDict);
        var pageTreeId = new PdfObjectId(1, 0);

        var node = new PageTreeNode(pageTreeId, pageTreeDict, objectReader);

        Assert.Same(resourcesDict, node.Resources);
    }

    [Fact]
    public void Test_Resources_NotPresent_ReturnsNull()
    {
        var objectReader = _createObjectReaderWithPageTreePdf();
        var pageTreeDict = _createValidPageTreeDictionary();
        var pageTreeId = new PdfObjectId(1, 0);

        var node = new PageTreeNode(pageTreeId, pageTreeDict, objectReader);

        Assert.Null(node.Resources);
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
        var objectReader = _createObjectReaderWithPageTreePdf();
        var pageTreeDict = _createValidPageTreeDictionary();
        pageTreeDict.Add(PdfNames.Rotate, new PdfNumber(rotation));
        var pageTreeId = new PdfObjectId(1, 0);

        var node = new PageTreeNode(pageTreeId, pageTreeDict, objectReader);

        Assert.Equal(rotation, node.Rotate);
    }

    [Fact]
    public void Test_Rotate_NotPresent_ReturnsNull()
    {
        var objectReader = _createObjectReaderWithPageTreePdf();
        var pageTreeDict = _createValidPageTreeDictionary();
        var pageTreeId = new PdfObjectId(1, 0);

        var node = new PageTreeNode(pageTreeId, pageTreeDict, objectReader);

        Assert.Null(node.Rotate);
    }
}
