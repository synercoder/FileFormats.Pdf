using Synercoding.FileFormats.Pdf.IO;

namespace Synercoding.FileFormats.Pdf.Tests.IO;

public class PdfStreamBytesProviderTests
{
    [Fact]
    public void Constructor_WithNullStream_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new PdfStreamBytesProvider(null!));
    }

    [Fact]
    public void Constructor_WithNonReadableStream_ThrowsArgumentException()
    {
        using var stream = new NonReadableStream();
        
        var exception = Assert.Throws<ArgumentException>(() => new PdfStreamBytesProvider(stream));
        Assert.Contains("reading", exception.Message);
    }

    [Fact]
    public void Constructor_WithNonSeekableStream_ThrowsArgumentException()
    {
        using var stream = new NonSeekableStream();
        
        var exception = Assert.Throws<ArgumentException>(() => new PdfStreamBytesProvider(stream));
        Assert.Contains("seeking", exception.Message);
    }

    [Fact]
    public void Constructor_WithValidStream_CreatesProvider()
    {
        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        var provider = new PdfStreamBytesProvider(stream);

        Assert.Equal(3, provider.Length);
        Assert.Equal(0, provider.Position);
    }

    [Fact]
    public void Position_GetAndSet_WorksCorrectly()
    {
        using var stream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5 });
        var provider = new PdfStreamBytesProvider(stream);

        provider.Position = 2;
        Assert.Equal(2, provider.Position);
        Assert.Equal(2, stream.Position);

        provider.Position = 0;
        Assert.Equal(0, provider.Position);
        Assert.Equal(0, stream.Position);
    }

    [Fact]
    public void Length_ReturnsStreamLength()
    {
        using var stream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5 });
        var provider = new PdfStreamBytesProvider(stream);

        Assert.Equal(5, provider.Length);
    }

    [Fact]
    public void TryRead_SingleByte_FromBeginning_ReturnsCorrectByte()
    {
        using var stream = new MemoryStream(new byte[] { 0x41, 0x42, 0x43 });
        var provider = new PdfStreamBytesProvider(stream);

        var success = provider.TryRead(out byte result);

        Assert.True(success);
        Assert.Equal(0x41, result);
        Assert.Equal(1, provider.Position);
    }

    [Fact]
    public void TryRead_SingleByte_AtEnd_ReturnsFalse()
    {
        using var stream = new MemoryStream(new byte[] { 0x41 });
        var provider = new PdfStreamBytesProvider(stream);
        provider.Position = 1;

        var success = provider.TryRead(out byte result);

        Assert.False(success);
        Assert.Equal(0, result);
        Assert.Equal(1, provider.Position);
    }

    [Fact]
    public void TryRead_SingleByte_EmptyStream_ReturnsFalse()
    {
        using var stream = new MemoryStream();
        var provider = new PdfStreamBytesProvider(stream);

        var success = provider.TryRead(out byte result);

        Assert.False(success);
        Assert.Equal(0, result);
        Assert.Equal(0, provider.Position);
    }

    [Fact]
    public void TryRead_Buffer_ValidRead_ReturnsTrue()
    {
        using var stream = new MemoryStream(new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45 });
        var provider = new PdfStreamBytesProvider(stream);
        var buffer = new byte[10];

        var success = provider.TryRead(buffer, 0, 3);

        Assert.True(success);
        Assert.Equal(new byte[] { 0x41, 0x42, 0x43, 0, 0, 0, 0, 0, 0, 0 }, buffer);
        Assert.Equal(3, provider.Position);
    }

    [Fact]
    public void TryRead_Buffer_WithOffset_ReadsCorrectly()
    {
        using var stream = new MemoryStream(new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45 });
        var provider = new PdfStreamBytesProvider(stream);
        var buffer = new byte[10];

        var success = provider.TryRead(buffer, 2, 3);

        Assert.True(success);
        Assert.Equal(new byte[] { 0, 0, 0x41, 0x42, 0x43, 0, 0, 0, 0, 0 }, buffer);
        Assert.Equal(3, provider.Position);
    }

    [Fact]
    public void TryRead_Buffer_NotEnoughDataInStream_ReturnsFalse()
    {
        using var stream = new MemoryStream(new byte[] { 0x41, 0x42 });
        var provider = new PdfStreamBytesProvider(stream);
        var buffer = new byte[10];

        var success = provider.TryRead(buffer, 0, 5);

        Assert.False(success);
        // Position advances by the number of bytes actually read (2 in this case)
        Assert.Equal(2, provider.Position);
    }

    [Fact]
    public void TryRead_Buffer_NotEnoughSpaceInBuffer_ReturnsFalse()
    {
        using var stream = new MemoryStream(new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45 });
        var provider = new PdfStreamBytesProvider(stream);
        var buffer = new byte[3];

        var success = provider.TryRead(buffer, 2, 3);

        Assert.False(success);
        Assert.Equal(0, provider.Position);
    }

    [Theory]
    [InlineData(SeekOrigin.Begin, 0, 0)]
    [InlineData(SeekOrigin.Begin, 2, 2)]
    [InlineData(SeekOrigin.Current, 1, 3)]
    [InlineData(SeekOrigin.Current, -1, 1)]
    [InlineData(SeekOrigin.End, 0, 5)]
    [InlineData(SeekOrigin.End, -2, 3)]
    public void Seek_ValidOperations_ReturnsCorrectPosition(SeekOrigin origin, long offset, long expectedPosition)
    {
        using var stream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5 });
        var provider = new PdfStreamBytesProvider(stream);

        if (origin == SeekOrigin.Current)
            provider.Position = 2;

        var result = provider.Seek(offset, origin);

        Assert.Equal(expectedPosition, result);
        Assert.Equal(expectedPosition, provider.Position);
    }

    [Fact]
    public void SequentialRead_ReadAllBytes_WorksCorrectly()
    {
        var bytes = new byte[] { 0x41, 0x42, 0x43 };
        using var stream = new MemoryStream(bytes);
        var provider = new PdfStreamBytesProvider(stream);
        var results = new List<byte>();

        while (provider.TryRead(out byte b))
            results.Add(b);

        Assert.Equal(bytes, results.ToArray());
        Assert.Equal(3, provider.Position);
    }

    [Fact]
    public void LargeStream_WorksCorrectly()
    {
        var bytes = new byte[10000];
        for (int i = 0; i < bytes.Length; i++)
            bytes[i] = (byte)(i % 256);

        using var stream = new MemoryStream(bytes);
        var provider = new PdfStreamBytesProvider(stream);

        Assert.Equal(10000, provider.Length);

        provider.Position = 5000;
        Assert.True(provider.TryRead(out byte result));
        Assert.Equal((byte)(5000 % 256), result);
    }

    [Fact]
    public void FileStream_WorksCorrectly()
    {
        var tempFile = Path.GetTempFileName();
        var testData = new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45 };
        
        try
        {
            File.WriteAllBytes(tempFile, testData);
            
            using var fileStream = File.OpenRead(tempFile);
            var provider = new PdfStreamBytesProvider(fileStream);

            Assert.Equal(5, provider.Length);
            Assert.True(provider.TryRead(out byte first));
            Assert.Equal(0x41, first);
            
            provider.Position = 2;
            Assert.True(provider.TryRead(out byte third));
            Assert.Equal(0x43, third);
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    private class NonReadableStream : Stream
    {
        public override bool CanRead => false;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length => 0;
        public override long Position { get; set; }
        public override void Flush() { }
        public override int Read(byte[] buffer, int offset, int count) => 0;
        public override long Seek(long offset, SeekOrigin origin) => 0;
        public override void SetLength(long value) { }
        public override void Write(byte[] buffer, int offset, int count) { }
    }

    private class NonSeekableStream : Stream
    {
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => 0;
        public override long Position { get; set; }
        public override void Flush() { }
        public override int Read(byte[] buffer, int offset, int count) => 0;
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) { }
        public override void Write(byte[] buffer, int offset, int count) { }
    }
}