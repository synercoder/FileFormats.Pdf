using Synercoding.FileFormats.Pdf.Generation.Internal;
using Synercoding.FileFormats.Pdf.Generation;
using Synercoding.FileFormats.Pdf.Primitives;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.Generation.Internal;

public class CatalogWriterTests
{
    [Fact]
    public void Test_WriteCatalog_BasicCatalog_WritesCorrectStructure()
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var objectWriter = new ObjectWriter(pdfStream, 0);
        var writer = new CatalogWriter(objectWriter);

        var settings = new WriterSettings();
        var pagesRef = new PdfReference(new PdfObjectId(2, 0));

        // Act
        var catalogRef = writer.WriteCatalog(pagesRef, settings, null, null);

        // Assert
        // catalogRef is a value type, so we just check its properties
        Assert.Equal(1, catalogRef.Id.ObjectNumber);
        Assert.Equal(0, catalogRef.Id.Generation);

        var content = Encoding.ASCII.GetString(stream.ToArray());
        Assert.Contains("1 0 obj", content);
        Assert.Contains("/Type/Catalog", content);
        Assert.Contains("/Pages 2 0 R", content);
        Assert.Contains("endobj", content);
    }

    [Fact]
    public void Test_WriteCatalog_WithPageModeUseOutlines_WritesPageMode()
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var objectWriter = new ObjectWriter(pdfStream, 0);
        var writer = new CatalogWriter(objectWriter);

        var settings = new WriterSettings();
        var pagesRef = new PdfReference(new PdfObjectId(2, 0));

        // Act
        var catalogRef = writer.WriteCatalog(pagesRef, settings, PageMode.UseOutlines, null);

        // Assert
        var content = Encoding.ASCII.GetString(stream.ToArray());
        Assert.Contains("/PageMode/UseOutlines", content);
    }

    [Fact]
    public void Test_WriteCatalog_WithPageModeUseNone_DoesNotWritePageMode()
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var objectWriter = new ObjectWriter(pdfStream, 0);
        var writer = new CatalogWriter(objectWriter);

        var settings = new WriterSettings();
        var pagesRef = new PdfReference(new PdfObjectId(2, 0));

        // Act
        var catalogRef = writer.WriteCatalog(pagesRef, settings, null, null);

        // Assert
        var content = Encoding.ASCII.GetString(stream.ToArray());
        Assert.DoesNotContain("/PageMode", content);
    }

    [Fact]
    public void Test_WriteCatalog_WithPageLayoutTwoColumnLeft_WritesPageLayout()
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var objectWriter = new ObjectWriter(pdfStream, 0);
        var writer = new CatalogWriter(objectWriter);

        var settings = new WriterSettings();
        var pagesRef = new PdfReference(new PdfObjectId(2, 0));

        // Act
        var catalogRef = writer.WriteCatalog(pagesRef, settings, null, PageLayout.TwoColumnLeft);

        // Assert
        var content = Encoding.ASCII.GetString(stream.ToArray());
        Assert.Contains("/PageLayout/TwoColumnLeft", content);
    }

    [Fact]
    public void Test_WriteCatalog_WithPageLayoutSinglePage_DoesNotWritePageLayout()
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var objectWriter = new ObjectWriter(pdfStream, 0);
        var writer = new CatalogWriter(objectWriter);

        var settings = new WriterSettings();
        var pagesRef = new PdfReference(new PdfObjectId(2, 0));

        // Act
        var catalogRef = writer.WriteCatalog(pagesRef, settings, null, null);

        // Assert
        var content = Encoding.ASCII.GetString(stream.ToArray());
        Assert.DoesNotContain("/PageLayout", content);
    }

    [Fact]
    public void Test_WriteCatalog_WithAllOptions_WritesAllOptions()
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var objectWriter = new ObjectWriter(pdfStream, 0);
        var writer = new CatalogWriter(objectWriter);

        var settings = new WriterSettings();
        var pagesRef = new PdfReference(new PdfObjectId(5, 0));

        // Act
        var catalogRef = writer.WriteCatalog(pagesRef, settings, PageMode.FullScreen, PageLayout.TwoPageRight);

        // Assert
        var content = Encoding.ASCII.GetString(stream.ToArray());
        Assert.Contains("/Type/Catalog", content);
        Assert.Contains("/Pages 5 0 R", content);
        Assert.Contains("/PageMode/FullScreen", content);
        Assert.Contains("/PageLayout/TwoPageRight", content);
    }

    [Theory]
    [InlineData(PageMode.UseAttachments)]
    [InlineData(PageMode.UseOC)]
    [InlineData(PageMode.UseThumbs)]
    public void Test_WriteCatalog_VariousPageModes_WritesCorrectly(PageMode pageMode)
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var objectWriter = new ObjectWriter(pdfStream, 0);
        var writer = new CatalogWriter(objectWriter);

        var settings = new WriterSettings();
        var pagesRef = new PdfReference(new PdfObjectId(2, 0));

        // Act
        var catalogRef = writer.WriteCatalog(pagesRef, settings, pageMode, null);

        // Assert
        var content = Encoding.ASCII.GetString(stream.ToArray());
        Assert.Contains($"/PageMode/{pageMode}", content);
    }

    [Theory]
    [InlineData(PageLayout.OneColumn)]
    [InlineData(PageLayout.TwoColumnRight)]
    [InlineData(PageLayout.TwoPageLeft)]
    public void Test_WriteCatalog_VariousPageLayouts_WritesCorrectly(PageLayout pageLayout)
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var objectWriter = new ObjectWriter(pdfStream, 0);
        var writer = new CatalogWriter(objectWriter);

        var settings = new WriterSettings();
        var pagesRef = new PdfReference(new PdfObjectId(2, 0));

        // Act
        var catalogRef = writer.WriteCatalog(pagesRef, settings, null, pageLayout);

        // Assert
        var content = Encoding.ASCII.GetString(stream.ToArray());
        Assert.Contains($"/PageLayout/{pageLayout}", content);
    }

    [Fact]
    public void Test_Constructor_NullObjectWriter_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CatalogWriter(null!));
    }
}
