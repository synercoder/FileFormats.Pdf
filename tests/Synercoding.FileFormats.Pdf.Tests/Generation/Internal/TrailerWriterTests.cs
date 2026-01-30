using Synercoding.FileFormats.Pdf.Generation.Internal;
using Synercoding.FileFormats.Pdf.Generation;
using Synercoding.FileFormats.Pdf.Primitives;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.Generation.Internal;

public class TrailerWriterTests
{
    [Fact]
    public void Test_WriteTo_WithoutInfoRef_WritesCorrectTrailer()
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        var settings = new WriterSettings();
        var writer = new TrailerWriter(tableBuilder, settings);

        var catalogRef = new PdfReference(new PdfObjectId(1, 0));
        var xrefPosition = 1234;

        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);

        // Act
        writer.WriteTo(pdfStream, catalogRef, null, xrefPosition);

        // Assert
        var content = Encoding.ASCII.GetString(stream.ToArray());
        var lines = content.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal("trailer", lines[0]);
        Assert.Contains("<</Size 1/Root 1 0 R>>", content);
        Assert.Contains("startxref", content);
        Assert.Contains("1234", content);
        Assert.EndsWith("%%EOF", content);
    }

    [Fact]
    public void Test_WriteTo_WithInfoRef_WritesCorrectTrailer()
    {
        // Arrange
        var tableBuilder = new TableBuilder();

        // Add some objects to the table
        var objId1 = tableBuilder.ReserveId();
        var objId2 = tableBuilder.ReserveId();
        tableBuilder.TrySetPosition(objId1, 100);
        tableBuilder.TrySetPosition(objId2, 200);

        var settings = new WriterSettings();
        var writer = new TrailerWriter(tableBuilder, settings);

        var catalogRef = new PdfReference(new PdfObjectId(1, 0));
        var infoRef = new PdfReference(new PdfObjectId(2, 0));
        var xrefPosition = 5678;

        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);

        // Act
        writer.WriteTo(pdfStream, catalogRef, infoRef, xrefPosition);

        // Assert
        var content = Encoding.ASCII.GetString(stream.ToArray());

        Assert.Contains("trailer", content);
        Assert.Contains("/Size 3", content); // 2 objects + 1 for free object
        Assert.Contains("/Root 1 0 R", content);
        Assert.Contains("/Info 2 0 R", content);
        Assert.Contains("startxref", content);
        Assert.Contains("5678", content);
        Assert.EndsWith("%%EOF", content);
    }

    [Fact]
    public void Test_WriteTo_CorrectStructure()
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        var settings = new WriterSettings();
        var writer = new TrailerWriter(tableBuilder, settings);

        var catalogRef = new PdfReference(new PdfObjectId(1, 0));
        var xrefPosition = 999;

        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);

        // Act
        writer.WriteTo(pdfStream, catalogRef, null, xrefPosition);

        // Assert
        var content = Encoding.ASCII.GetString(stream.ToArray());
        var lines = content.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);

        // Check the structure
        int trailerIndex = Array.IndexOf(lines, "trailer");
        int startxrefIndex = Array.IndexOf(lines, "startxref");
        int eofIndex = Array.IndexOf(lines, "%%EOF");

        Assert.True(trailerIndex >= 0, "trailer not found");
        Assert.True(startxrefIndex > trailerIndex, "startxref should come after trailer");
        Assert.True(eofIndex > startxrefIndex, "%%EOF should come after startxref");

        // Check that xref position is right after startxref
        Assert.Equal("999", lines[startxrefIndex + 1]);
    }

    [Fact]
    public void Test_Constructor_NullTableBuilder_ThrowsArgumentNullException()
    {
        // Arrange
        var settings = new WriterSettings();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TrailerWriter(null!, settings));
    }

    [Fact]
    public void Test_Constructor_NullSettings_ThrowsArgumentNullException()
    {
        // Arrange
        var tableBuilder = new TableBuilder();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TrailerWriter(tableBuilder, null!));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(123)]
    [InlineData(999999)]
    public void Test_WriteTo_VariousXRefPositions_WritesCorrectPosition(long xrefPosition)
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        var settings = new WriterSettings();
        var writer = new TrailerWriter(tableBuilder, settings);

        var catalogRef = new PdfReference(new PdfObjectId(1, 0));

        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);

        // Act
        writer.WriteTo(pdfStream, catalogRef, null, xrefPosition);

        // Assert
        var content = Encoding.ASCII.GetString(stream.ToArray());
        Assert.Contains($"startxref\r\n{xrefPosition}\r\n", content);
    }
}
