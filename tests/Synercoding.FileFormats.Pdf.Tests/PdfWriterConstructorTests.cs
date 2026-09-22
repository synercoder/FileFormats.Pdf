using Synercoding.FileFormats.Pdf.Generation;

namespace Synercoding.FileFormats.Pdf.Tests;

/// <summary>
/// Tests for the <see cref="PdfWriter"/> constructors.
/// </summary>
/// <remarks>
/// The file path based tests are regression tests: the file used to be opened for reading instead
/// of for writing, which made every file path based <see cref="PdfWriter"/> unusable.
/// </remarks>
public class PdfWriterConstructorTests : IDisposable
{
    private readonly string _tempDirectory;

    public PdfWriterConstructorTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), "PdfWriterConstructorTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDirectory);
    }

    public void Dispose()
    {
        try
        {
            Directory.Delete(_tempDirectory, true);
        }
        catch (IOException)
        {
            // Best effort cleanup, a leftover temp file should never fail a test.
        }
        GC.SuppressFinalize(this);
    }

    private string _getFilePath()
        => Path.Combine(_tempDirectory, Guid.NewGuid().ToString("N") + ".pdf");

    [Fact]
    public void Test_PdfWriter_FilePathConstructor_CreatesPdfFile()
    {
        // Arrange
        var filePath = _getFilePath();

        // Act
        using (var writer = new PdfWriter(filePath))
        {
            writer.AddPage(page => page.MediaBox = Sizes.A4.AsRectangle());
        }

        // Assert
        Assert.True(File.Exists(filePath));
        var pdfContent = File.ReadAllText(filePath, System.Text.Encoding.ASCII);
        Assert.StartsWith("%PDF-", pdfContent);
        Assert.EndsWith("%%EOF", pdfContent);
    }

    [Fact]
    public void Test_PdfWriter_FilePathAndSettingsConstructor_CreatesPdfFile()
    {
        // Arrange
        var filePath = _getFilePath();

        // Act
        using (var writer = new PdfWriter(filePath, new WriterSettings()))
        {
            writer.AddPage(page => page.MediaBox = Sizes.A4.AsRectangle());
        }

        // Assert
        Assert.True(File.Exists(filePath));
        var pdfContent = File.ReadAllText(filePath, System.Text.Encoding.ASCII);
        Assert.StartsWith("%PDF-", pdfContent);
        Assert.EndsWith("%%EOF", pdfContent);
    }

    [Fact]
    public void Test_PdfWriter_FilePathConstructor_WithAlreadyExistingFile_WritesPdfContent()
    {
        // Arrange
        var filePath = _getFilePath();
        File.WriteAllText(filePath, string.Empty);

        // Act
        using (var writer = new PdfWriter(filePath))
        {
            writer.AddPage(page => page.MediaBox = Sizes.A4.AsRectangle());
        }

        // Assert
        var pdfContent = File.ReadAllText(filePath, System.Text.Encoding.ASCII);
        Assert.StartsWith("%PDF-", pdfContent);
        Assert.EndsWith("%%EOF", pdfContent);
    }

    [Fact]
    public void Test_PdfWriter_FilePathConstructor_WithNonExistingFile_CreatesFile()
    {
        // Arrange
        var filePath = _getFilePath();
        Assert.False(File.Exists(filePath));

        // Act
        using var writer = new PdfWriter(filePath);

        // Assert
        // The file is created by the constructor, opening an existing file for reading would have
        // thrown a FileNotFoundException instead.
        Assert.True(File.Exists(filePath));
    }

    [Fact]
    public void Test_PdfWriter_FilePathConstructor_DisposeReleasesFileHandle()
    {
        // Arrange
        var filePath = _getFilePath();
        var writer = new PdfWriter(filePath);
        writer.AddPage(page => page.MediaBox = Sizes.A4.AsRectangle());

        // Act
        writer.Dispose();

        // Assert
        // Opening with FileShare.None only succeeds when no other handle to the file is left open,
        // proving the writer owns and disposes the file stream it created.
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
        Assert.True(fileStream.Length > 0);
    }

    [Fact]
    public void Test_PdfWriter_StreamConstructor_DoesNotOwnStream()
    {
        // Arrange
        var stream = new MemoryStream();

        // Act
        using (var writer = new PdfWriter(stream))
        {
            writer.AddPage(page => page.MediaBox = Sizes.A4.AsRectangle());
        }

        // Assert
        Assert.True(stream.CanWrite);
        stream.Dispose();
    }

    [Fact]
    public void Test_PdfWriter_StreamAndSettingsConstructor_DoesNotOwnStream()
    {
        // Arrange
        var stream = new MemoryStream();

        // Act
        using (var writer = new PdfWriter(stream, new WriterSettings()))
        {
            writer.AddPage(page => page.MediaBox = Sizes.A4.AsRectangle());
        }

        // Assert
        Assert.True(stream.CanWrite);
        stream.Dispose();
    }

    [Fact]
    public void Test_PdfWriter_StreamConstructor_WithOwnsStreamTrue_DisposesStream()
    {
        // Arrange
        var stream = new MemoryStream();

        // Act
        using (var writer = new PdfWriter(stream, new WriterSettings(), true))
        {
            writer.AddPage(page => page.MediaBox = Sizes.A4.AsRectangle());
        }

        // Assert
        Assert.False(stream.CanWrite);
    }

    [Fact]
    public void Test_PdfWriter_StreamConstructor_WithOwnsStreamFalse_LeavesStreamOpen()
    {
        // Arrange
        var stream = new MemoryStream();

        // Act
        using (var writer = new PdfWriter(stream, new WriterSettings(), false))
        {
            writer.AddPage(page => page.MediaBox = Sizes.A4.AsRectangle());
        }

        // Assert
        Assert.True(stream.CanWrite);
        stream.Dispose();
    }

    [Fact]
    public void Test_PdfWriter_PdfStreamConstructor_WritesPdf()
    {
        // Arrange
        using var stream = new MemoryStream();
        var pdfStream = new PdfStream(stream, false);

        // Act
        using (var writer = new PdfWriter(pdfStream, new WriterSettings()))
        {
            writer.AddPage(page => page.MediaBox = Sizes.A4.AsRectangle());
        }

        // Assert
        var pdfContent = System.Text.Encoding.ASCII.GetString(stream.ToArray());
        Assert.StartsWith("%PDF-", pdfContent);
        Assert.EndsWith("%%EOF", pdfContent);
    }

    [Fact]
    public void Test_PdfWriter_WithNonWritableStream_Throws()
    {
        // Arrange
        using var stream = new MemoryStream(new byte[16], writable: false);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PdfWriter(stream));
    }

    [Fact]
    public void Test_PdfWriter_WithReadOnlyFileStream_Throws()
    {
        // Arrange
        var filePath = _getFilePath();
        File.WriteAllText(filePath, string.Empty);
        using var stream = File.OpenRead(filePath);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PdfWriter(stream));
    }

    [Fact]
    public void Test_PdfWriter_Constructor_WritesHeader()
    {
        // Arrange
        using var stream = new MemoryStream();

        // Act
        using var writer = new PdfWriter(stream);

        // Assert
        var pdfContent = System.Text.Encoding.ASCII.GetString(stream.ToArray());
        Assert.StartsWith("%PDF-2.0", pdfContent);
    }

    [Fact]
    public void Test_PdfWriter_Constructor_SetsDefaults()
    {
        // Arrange
        using var stream = new MemoryStream();

        // Act
        using var writer = new PdfWriter(stream);

        // Assert
        Assert.Equal(0, writer.PageCount);
        Assert.Null(writer.PageMode);
        Assert.Null(writer.PageLayout);
        Assert.NotNull(writer.DocumentInformation);
        Assert.StartsWith("Synercoding.FileFormats.Pdf ", writer.DocumentInformation.Producer);
        Assert.NotNull(writer.DocumentInformation.CreationDate);
    }
}
