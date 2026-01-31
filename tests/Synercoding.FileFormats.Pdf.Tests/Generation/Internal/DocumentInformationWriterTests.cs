using Synercoding.FileFormats.Pdf.DocumentObjects;
using Synercoding.FileFormats.Pdf.Generation.Internal;
using Synercoding.FileFormats.Pdf.Generation;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.Generation.Internal;

public class DocumentInformationWriterTests
{
    [Fact]
    public void Test_WriteIfNeeded_EmptyInfo_ReturnsNull()
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var objectWriter = new ObjectWriter(pdfStream, 0);
        var writer = new DocumentInformationWriter(objectWriter);

        using var pdfWriter = new PdfWriter(new MemoryStream());
        var info = new DocumentInformation(pdfWriter)
        {
            CreationDate = null
        };

        // Act
        var result = writer.WriteIfNeeded(info);

        // Assert
        Assert.Null(result);
        Assert.Equal(0, stream.Length); // Nothing should be written
    }

    [Fact]
    public void Test_WriteIfNeeded_WithTitle_WritesInfoObject()
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var objectWriter = new ObjectWriter(pdfStream, 0);
        var writer = new DocumentInformationWriter(objectWriter);

        using var pdfWriter = new PdfWriter(new MemoryStream());
        var info = new DocumentInformation(pdfWriter)
        {
            Title = "Test Document"
        };

        // Act
        var result = writer.WriteIfNeeded(info);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Value.Id.ObjectNumber);
        Assert.Equal(0, result.Value.Id.Generation);

        var content = Encoding.ASCII.GetString(stream.ToArray());
        Assert.Contains("1 0 obj", content);
        Assert.Contains("/Title", content);
        Assert.Contains("Test Document", content);
        Assert.Contains("endobj", content);
    }

    [Fact]
    public void Test_WriteIfNeeded_WithAllFields_WritesAllFields()
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var objectWriter = new ObjectWriter(pdfStream, 0);
        var writer = new DocumentInformationWriter(objectWriter);

        using var pdfWriter = new PdfWriter(new MemoryStream());
        var testDate = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(-5));
        var info = new DocumentInformation(pdfWriter)
        {
            Title = "Test Document",
            Author = "Test Author",
            Subject = "Test Subject",
            Keywords = "test, keywords",
            Creator = "Test Creator",
            Producer = "Test Producer",
            CreationDate = testDate,
            ModDate = testDate.AddDays(1)
        };

        // Act
        var result = writer.WriteIfNeeded(info);

        // Assert
        Assert.NotNull(result);

        var content = Encoding.ASCII.GetString(stream.ToArray());
        Assert.Contains("/Title", content);
        Assert.Contains("/Author", content);
        Assert.Contains("/Subject", content);
        Assert.Contains("/Keywords", content);
        Assert.Contains("/Creator", content);
        Assert.Contains("/Producer", content);
        Assert.Contains("/CreationDate", content);
        Assert.Contains("/ModDate", content);

        Assert.Contains("Test Document", content);
        Assert.Contains("Test Author", content);
        Assert.Contains("Test Subject", content);
        Assert.Contains("test, keywords", content);
        Assert.Contains("Test Creator", content);
        Assert.Contains("Test Producer", content);
    }

    [Fact]
    public void Test_WriteIfNeeded_WithExtraInfo_WritesExtraInfo()
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var objectWriter = new ObjectWriter(pdfStream, 0);
        var writer = new DocumentInformationWriter(objectWriter);

        using var pdfWriter = new PdfWriter(new MemoryStream());
        var info = new DocumentInformation(pdfWriter);
        info.ExtraInfo["CustomField"] = "Custom Value";
        info.ExtraInfo["AnotherField"] = "Another Value";

        // Act
        var result = writer.WriteIfNeeded(info);

        // Assert
        Assert.NotNull(result);

        var content = Encoding.ASCII.GetString(stream.ToArray());
        Assert.Contains("/CustomField", content);
        Assert.Contains("/AnotherField", content);
        Assert.Contains("Custom Value", content);
        Assert.Contains("Another Value", content);
    }

    [Fact]
    public void Test_WriteIfNeeded_WithOnlyCreationDate_WritesInfoObject()
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var objectWriter = new ObjectWriter(pdfStream, 0);
        var writer = new DocumentInformationWriter(objectWriter);

        using var pdfWriter = new PdfWriter(new MemoryStream());
        var testDate = new DateTimeOffset(2024, 6, 15, 14, 30, 45, TimeSpan.FromHours(2));
        var info = new DocumentInformation(pdfWriter)
        {
            CreationDate = testDate
        };

        // Act
        var result = writer.WriteIfNeeded(info);

        // Assert
        Assert.NotNull(result);

        var content = Encoding.ASCII.GetString(stream.ToArray());
        Assert.Contains("/CreationDate", content);
        // Check date format: D:YYYYMMDDHHmmSS+HH'MM
        Assert.Contains("D:20240615143045+02'00", content);
    }

    [Fact]
    public void Test_WriteIfNeeded_WithNegativeTimezone_FormatsDateCorrectly()
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var objectWriter = new ObjectWriter(pdfStream, 0);
        var writer = new DocumentInformationWriter(objectWriter);

        using var pdfWriter = new PdfWriter(new MemoryStream());
        var testDate = new DateTimeOffset(2024, 12, 25, 9, 15, 30, TimeSpan.FromHours(-8));
        var info = new DocumentInformation(pdfWriter)
        {
            ModDate = testDate
        };

        // Act
        var result = writer.WriteIfNeeded(info);

        // Assert
        Assert.NotNull(result);

        var content = Encoding.ASCII.GetString(stream.ToArray());
        Assert.Contains("/ModDate", content);
        // Check negative timezone format
        Assert.Contains("D:20241225091530-08'00", content);
    }

    [Theory]
    [InlineData("Simple Title")]
    [InlineData("Title with (parentheses)")]
    [InlineData("Title with special chars: áéíóú")]
    public void Test_WriteIfNeeded_VariousTitles_EncodesCorrectly(string title)
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);
        var objectWriter = new ObjectWriter(pdfStream, 0);
        var writer = new DocumentInformationWriter(objectWriter);

        using var pdfWriter = new PdfWriter(new MemoryStream());
        var info = new DocumentInformation(pdfWriter)
        {
            Title = title
        };

        // Act
        var result = writer.WriteIfNeeded(info);

        // Assert
        Assert.NotNull(result);
        var content = Encoding.ASCII.GetString(stream.ToArray());
        Assert.Contains("/Title", content);
    }

    [Fact]
    public void Test_Constructor_NullObjectWriter_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new DocumentInformationWriter(null!));
    }
}
