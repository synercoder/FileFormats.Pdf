using Synercoding.FileFormats.Pdf.Generation.Internal;
using Synercoding.FileFormats.Pdf.Generation;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.Generation.Internal;

public class CrossReferenceTableWriterTests
{
    [Fact]
    public void Test_WriteTo_EmptyTable_WritesCorrectXRef()
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        var writer = new CrossReferenceTableWriter(tableBuilder);
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);

        // Act
        var xrefPosition = writer.WriteTo(pdfStream);

        // Assert
        Assert.Equal(0, xrefPosition);

        var content = Encoding.ASCII.GetString(stream.ToArray());
        var lines = content.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal("xref", lines[0]);
        Assert.Equal("0 1", lines[1]); // Only free object at position 0
        Assert.Equal("0000000000 65535 f", lines[2]); // Free object entry
    }

    [Fact]
    public void Test_WriteTo_SingleObject_WritesCorrectXRef()
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        var writer = new CrossReferenceTableWriter(tableBuilder);

        // Reserve and set position for one object
        var objId = tableBuilder.ReserveId();
        tableBuilder.TrySetPosition(objId, 123);

        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);

        // Act
        var xrefPosition = writer.WriteTo(pdfStream);

        // Assert
        Assert.Equal(0, xrefPosition);

        var content = Encoding.ASCII.GetString(stream.ToArray());
        var lines = content.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal("xref", lines[0]);
        Assert.Equal("0 2", lines[1]); // Free object + 1 real object
        Assert.Equal("0000000000 65535 f", lines[2]); // Free object entry
        Assert.Equal("0000000123 00000 n", lines[3]); // Real object entry
    }

    [Fact]
    public void Test_WriteTo_MultipleObjects_WritesCorrectXRef()
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        var writer = new CrossReferenceTableWriter(tableBuilder);

        // Reserve and set positions for multiple objects
        var objId1 = tableBuilder.ReserveId();
        var objId2 = tableBuilder.ReserveId();
        var objId3 = tableBuilder.ReserveId();

        tableBuilder.TrySetPosition(objId1, 100);
        tableBuilder.TrySetPosition(objId2, 250);
        tableBuilder.TrySetPosition(objId3, 500);

        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);

        // Act
        var xrefPosition = writer.WriteTo(pdfStream);

        // Assert
        Assert.Equal(0, xrefPosition);

        var content = Encoding.ASCII.GetString(stream.ToArray());
        var lines = content.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal("xref", lines[0]);
        Assert.Equal("0 4", lines[1]); // Free object + 3 real objects
        Assert.Equal("0000000000 65535 f", lines[2]); // Free object
        Assert.Equal("0000000100 00000 n", lines[3]); // Object 1
        Assert.Equal("0000000250 00000 n", lines[4]); // Object 2
        Assert.Equal("0000000500 00000 n", lines[5]); // Object 3
    }

    [Fact]
    public void Test_WriteTo_ReturnsCorrectPosition()
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        var writer = new CrossReferenceTableWriter(tableBuilder);

        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);

        // Write some content first to offset the position
        pdfStream.Write("Some content before xref");
        var expectedPosition = pdfStream.Position;

        // Act
        var xrefPosition = writer.WriteTo(pdfStream);

        // Assert
        Assert.Equal(expectedPosition, xrefPosition);
    }

    [Fact]
    public void Test_Constructor_NullTableBuilder_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CrossReferenceTableWriter(null!));
    }

    [Fact]
    public void Test_WriteTo_UnwrittenObject_ThrowsInvalidOperationException()
    {
        // Arrange
        var tableBuilder = new TableBuilder();
        var writer = new CrossReferenceTableWriter(tableBuilder);

        // Reserve but don't set position
        tableBuilder.ReserveId();

        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => writer.WriteTo(pdfStream));
    }
}
