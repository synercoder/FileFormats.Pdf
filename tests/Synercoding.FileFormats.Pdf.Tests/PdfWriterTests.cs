namespace Synercoding.FileFormats.Pdf.Tests;

public class PdfWriterTests
{
    [Fact]
    public void Test_PdfWriter_CreatesBasicPdf()
    {
        // Arrange
        using var stream = new MemoryStream();

        // Act
        using (var writer = new PdfWriter(stream))
        {
            writer.AddPage(page =>
            {
                // Set A4 page size
                page.MediaBox = Sizes.A4.AsRectangle();
            });
        }

        // Assert
        stream.Position = 0;
        var pdfContent = System.Text.Encoding.ASCII.GetString(stream.ToArray());

        // Check PDF header
        Assert.StartsWith("%PDF-", pdfContent);

        // Check EOF marker
        Assert.EndsWith("%%EOF", pdfContent);

        // Check basic structure elements
        Assert.Contains("1 0 obj", pdfContent); // At least one object
        Assert.Contains("xref", pdfContent); // Cross-reference table
        Assert.Contains("trailer", pdfContent); // Trailer
        Assert.Contains("startxref", pdfContent); // Start xref position
    }

    [Fact]
    public void Test_PdfWriter_MultiplePagesWithBoxSizes()
    {
        // Arrange
        using var stream = new MemoryStream();

        // Act
        using (var writer = new PdfWriter(stream))
        {
            // Add first page with various box sizes
            writer.AddPage(page =>
            {
                page.MediaBox = new Rectangle(0, 0, 612, 792, Unit.Points); // US Letter
                page.CropBox = new Rectangle(10, 10, 602, 782, Unit.Points);
                page.BleedBox = new Rectangle(0, 0, 612, 792, Unit.Points);
                page.TrimBox = new Rectangle(20, 20, 592, 772, Unit.Points);
                page.ArtBox = new Rectangle(30, 30, 582, 762, Unit.Points);
                page.Rotation = PageRotation.Rotation90;
            });

            // Add second page with just media box
            writer.AddPage(page =>
            {
                page.MediaBox = Sizes.A3.AsRectangle();
            });

            // Add third page with custom size
            writer.AddPage(page =>
            {
                page.MediaBox = new Rectangle(0, 0, 200, 300, Unit.Points);
                page.Rotation = PageRotation.Rotation180;
            });
        }

        // Assert
        stream.Position = 0;
        var pdfContent = System.Text.Encoding.ASCII.GetString(stream.ToArray());

        // Check for page objects
        Assert.Contains("/Type/Page", pdfContent);
        Assert.Contains("/MediaBox", pdfContent);
        Assert.Contains("/CropBox", pdfContent);
        Assert.Contains("/BleedBox", pdfContent);
        Assert.Contains("/TrimBox", pdfContent);
        Assert.Contains("/ArtBox", pdfContent);
        Assert.Contains("/Rotate", pdfContent);

        // Check page count
        Assert.Contains("/Count 3", pdfContent); // 3 pages
    }

    [Fact]
    public void Test_PdfWriter_WithDocumentInformation()
    {
        // Arrange
        using var stream = new MemoryStream();
        var testDate = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(-5));

        // Act
        using (var writer = new PdfWriter(stream))
        {
            writer.DocumentInformation.Title = "Test PDF Document";
            writer.DocumentInformation.Author = "Test Author";
            writer.DocumentInformation.Subject = "Testing PdfWriter";
            writer.DocumentInformation.Keywords = "test, pdf, writer";
            writer.DocumentInformation.Creator = "Unit Test";
            writer.DocumentInformation.CreationDate = testDate;
            writer.DocumentInformation.ModDate = testDate.AddDays(1);

            writer.AddPage(page =>
            {
                page.MediaBox = Sizes.A4.AsRectangle();
            });
        }

        // Assert
        stream.Position = 0;
        var pdfContent = System.Text.Encoding.ASCII.GetString(stream.ToArray());

        // Check for info dictionary entries
        Assert.Contains("/Title", pdfContent);
        Assert.Contains("/Author", pdfContent);
        Assert.Contains("/Subject", pdfContent);
        Assert.Contains("/Keywords", pdfContent);
        Assert.Contains("/Creator", pdfContent);
        Assert.Contains("/CreationDate", pdfContent);
        Assert.Contains("/ModDate", pdfContent);
    }

    [Fact]
    public void Test_PdfWriter_WithCustomSettings()
    {
        // Arrange
        using var stream = new MemoryStream();
        var settings = new WriterSettings();

        // Act
        using (var writer = new PdfWriter(stream, settings))
        {
            writer.PageMode = PageMode.UseOutlines;
            writer.PageLayout = PageLayout.TwoColumnLeft;

            writer.AddPage(page =>
            {
                page.MediaBox = Sizes.A4.AsRectangle();
            });
        }

        // Assert
        stream.Position = 0;
        var pdfContent = System.Text.Encoding.ASCII.GetString(stream.ToArray());

        // Check for catalog entries
        Assert.Contains("/PageMode/UseOutlines", pdfContent);
        Assert.Contains("/PageLayout/TwoColumnLeft", pdfContent);
    }

    [Fact]
    public void Test_PdfWriter_WithNullPageModeAndLayout_DoesNotWriteToCatalog()
    {
        // Arrange
        using var stream = new MemoryStream();

        // Act
        using (var writer = new PdfWriter(stream))
        {
            // PageMode and PageLayout are null by default
            writer.AddPage(page =>
            {
                page.MediaBox = Sizes.A4.AsRectangle();
            });
        }

        // Assert
        stream.Position = 0;
        var pdfContent = System.Text.Encoding.ASCII.GetString(stream.ToArray());

        // Check that PageMode and PageLayout are not in the catalog
        Assert.DoesNotContain("/PageMode", pdfContent);
        Assert.DoesNotContain("/PageLayout", pdfContent);
    }

    [Fact]
    public void Test_PdfWriter_WithOnlyPageMode_WritesOnlyPageMode()
    {
        // Arrange
        using var stream = new MemoryStream();

        // Act
        using (var writer = new PdfWriter(stream))
        {
            writer.PageMode = PageMode.FullScreen;
            // PageLayout remains null

            writer.AddPage(page =>
            {
                page.MediaBox = Sizes.A4.AsRectangle();
            });
        }

        // Assert
        stream.Position = 0;
        var pdfContent = System.Text.Encoding.ASCII.GetString(stream.ToArray());

        // Check that only PageMode is in the catalog
        Assert.Contains("/PageMode/FullScreen", pdfContent);
        Assert.DoesNotContain("/PageLayout", pdfContent);
    }

    [Fact]
    public void Test_PdfWriter_WithOnlyPageLayout_WritesOnlyPageLayout()
    {
        // Arrange
        using var stream = new MemoryStream();

        // Act
        using (var writer = new PdfWriter(stream))
        {
            // PageMode remains null
            writer.PageLayout = PageLayout.TwoPageRight;

            writer.AddPage(page =>
            {
                page.MediaBox = Sizes.A4.AsRectangle();
            });
        }

        // Assert
        stream.Position = 0;
        var pdfContent = System.Text.Encoding.ASCII.GetString(stream.ToArray());

        // Check that only PageLayout is in the catalog
        Assert.DoesNotContain("/PageMode", pdfContent);
        Assert.Contains("/PageLayout/TwoPageRight", pdfContent);
    }
}
