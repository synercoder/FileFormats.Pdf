using Synercoding.FileFormats.Pdf.DocumentObjects;
using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Parsing;
using Synercoding.FileFormats.Pdf.Primitives;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.DocumentObjects;

public class CatalogTests
{
    private static ObjectReader _createObjectReaderWithSimplePdf()
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
            "<< /Size 2 /Root 1 0 R >>" + '\n' +
            "startxref" + '\n' +
            "61" + '\n' +
            "%%EOF";
        
        var bytes = Encoding.ASCII.GetBytes(pdfContent);
        var provider = new PdfByteArrayProvider(bytes);
        return new ObjectReader(provider, new ReaderSettings());
    }

    private static IPdfDictionary _createValidCatalogDictionary()
    {
        var dictionary = new PdfDictionary();
        dictionary.Add(PdfNames.Type, PdfNames.Catalog);
        dictionary.Add(PdfNames.Pages, new PdfReference { Id = new PdfObjectId(1, 0) });
        return dictionary;
    }

    [Fact]
    public void Test_Constructor_NullDictionary_ThrowsArgumentNullException()
    {
        var objectReader = _createObjectReaderWithSimplePdf();

        Assert.Throws<NullReferenceException>(() => new Catalog(null!, objectReader));
    }

    [Fact]
    public void Test_Constructor_NullObjectReader_ThrowsArgumentNullException()
    {
        var catalogDict = _createValidCatalogDictionary();

        Assert.Throws<NullReferenceException>(() => new Catalog(catalogDict, null!));
    }

    [Fact]
    public void Test_Constructor_MissingTypeKey_ThrowsArgumentException()
    {
        var dictionary = new PdfDictionary();
        dictionary.Add(PdfNames.Pages, new PdfReference { Id = new PdfObjectId(1, 0) });
        
        var objectReader = _createObjectReaderWithSimplePdf();

        var exception = Assert.Throws<ArgumentException>(() => new Catalog(dictionary, objectReader));
        Assert.Contains("does not contain the required key /Type", exception.Message);
        Assert.Equal("pdfDictionary", exception.ParamName);
    }

    [Fact]
    public void Test_Constructor_WrongTypeValue_ThrowsArgumentException()
    {
        var dictionary = new PdfDictionary();
        dictionary.Add(PdfNames.Type, PdfNames.Page);
        dictionary.Add(PdfNames.Pages, new PdfReference { Id = new PdfObjectId(1, 0) });
        
        var objectReader = _createObjectReaderWithSimplePdf();

        var exception = Assert.Throws<ArgumentException>(() => new Catalog(dictionary, objectReader));
        Assert.Contains("type is not /Catalog", exception.Message);
        Assert.Equal("pdfDictionary", exception.ParamName);
    }

    [Fact]
    public void Test_Constructor_MissingPagesKey_ThrowsArgumentException()
    {
        var dictionary = new PdfDictionary();
        dictionary.Add(PdfNames.Type, PdfNames.Catalog);
        
        var objectReader = _createObjectReaderWithSimplePdf();

        var exception = Assert.Throws<ArgumentException>(() => new Catalog(dictionary, objectReader));
        Assert.Contains("does not contain the required key /Pages", exception.Message);
        Assert.Equal("pdfDictionary", exception.ParamName);
    }

    [Fact]
    public void Test_Constructor_ValidCatalogDictionary_CreatesInstance()
    {
        var catalogDict = _createValidCatalogDictionary();
        var objectReader = _createObjectReaderWithSimplePdf();

        var catalog = new Catalog(catalogDict, objectReader);

        Assert.NotNull(catalog);
        Assert.NotNull(catalog.Pages);
    }

    [Fact]
    public void Test_Pages_Property_ReturnsSameInstance()
    {
        var catalogDict = _createValidCatalogDictionary();
        var objectReader = _createObjectReaderWithSimplePdf();

        var catalog = new Catalog(catalogDict, objectReader);

        var pages1 = catalog.Pages;
        var pages2 = catalog.Pages;

        Assert.Same(pages1, pages2);
    }
}
