using Synercoding.FileFormats.Pdf.IO;

namespace Synercoding.FileFormats.Pdf.Tests.IO;

public class DisposableBytesProviderTests
{
    [Fact]
    public void GetFrom_NonReadableStream_ThrowsArgumentException()
    {
        using var stream = new NonReadableStream();
        var settings = new ReaderSettings();

        var exception = Assert.Throws<ArgumentException>(() => DisposableBytesProvider.GetFrom(stream, settings, false));
        Assert.Contains("reading", exception.Message);
    }

    [Fact]
    public void GetFrom_SeekableMemoryStream_LeaveOpen_ReturnsDisposableProvider()
    {
        var data = new byte[] { 1, 2, 3, 4, 5 };
        using var stream = new MemoryStream(data);
        var settings = new ReaderSettings();

        using var provider = DisposableBytesProvider.GetFrom(stream, settings, leaveOpen: true);

        Assert.Equal(5, provider.Length);
        Assert.Equal(0, provider.Position);
        
        Assert.True(provider.TryRead(out byte first));
        Assert.Equal(1, first);
    }

    [Fact]
    public void GetFrom_SeekableMemoryStream_DontLeaveOpen_ReturnsDisposableProvider()
    {
        var data = new byte[] { 1, 2, 3, 4, 5 };
        var stream = new MemoryStream(data);
        var settings = new ReaderSettings();

        using var provider = DisposableBytesProvider.GetFrom(stream, settings, leaveOpen: false);

        Assert.Equal(5, provider.Length);
        Assert.Equal(0, provider.Position);
    }

    [Fact]
    public void GetFrom_NonSeekableStream_SmallSize_CopiesStreamToMemory()
    {
        var data = new byte[] { 1, 2, 3, 4, 5 };
        using var stream = new NonSeekableStream(data);
        var settings = new ReaderSettings();

        using var provider = DisposableBytesProvider.GetFrom(stream, settings, leaveOpen: true);

        Assert.Equal(5, provider.Length);
        Assert.Equal(0, provider.Position);
        
        Assert.True(provider.TryRead(out byte first));
        Assert.Equal(1, first);
    }

    [Fact]
    public void GetFrom_SeekableStreamBelowMaxMemoryCopy_CopiesStreamToMemory()
    {
        var data = new byte[1000];
        for (int i = 0; i < data.Length; i++)
            data[i] = (byte)(i % 256);
            
        using var stream = new FileStreamWrapper(data);
        var settings = new ReaderSettings { MaxMemoryCopy = 2000 };

        using var provider = DisposableBytesProvider.GetFrom(stream, settings, leaveOpen: true);

        Assert.Equal(1000, provider.Length);
        Assert.Equal(0, provider.Position);
        
        Assert.True(provider.TryRead(out byte first));
        Assert.Equal(0, first);
    }

    [Fact]
    public void GetFrom_SeekableStreamAboveMaxMemoryCopy_UsesOriginalStream()
    {
        var data = new byte[1000];
        for (int i = 0; i < data.Length; i++)
            data[i] = (byte)(i % 256);
            
        using var stream = new FileStreamWrapper(data);
        var settings = new ReaderSettings { MaxMemoryCopy = 500 };

        using var provider = DisposableBytesProvider.GetFrom(stream, settings, leaveOpen: true);

        Assert.Equal(1000, provider.Length);
        Assert.Equal(0, provider.Position);
        
        Assert.True(provider.TryRead(out byte first));
        Assert.Equal(0, first);
    }

    [Fact]
    public void Position_GetAndSet_WorksCorrectly()
    {
        var data = new byte[] { 1, 2, 3, 4, 5 };
        using var stream = new MemoryStream(data);
        var settings = new ReaderSettings();
        using var provider = DisposableBytesProvider.GetFrom(stream, settings, leaveOpen: true);

        provider.Position = 2;
        Assert.Equal(2, provider.Position);

        provider.Position = 0;
        Assert.Equal(0, provider.Position);
    }

    [Fact]
    public void TryRead_SingleByte_WorksCorrectly()
    {
        var data = new byte[] { 0x41, 0x42, 0x43 };
        using var stream = new MemoryStream(data);
        var settings = new ReaderSettings();
        using var provider = DisposableBytesProvider.GetFrom(stream, settings, leaveOpen: true);

        Assert.True(provider.TryRead(out byte first));
        Assert.Equal(0x41, first);
        Assert.Equal(1, provider.Position);

        Assert.True(provider.TryRead(out byte second));
        Assert.Equal(0x42, second);
        Assert.Equal(2, provider.Position);
    }

    [Fact]
    public void TryRead_Buffer_WorksCorrectly()
    {
        var data = new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45 };
        using var stream = new MemoryStream(data);
        var settings = new ReaderSettings();
        using var provider = DisposableBytesProvider.GetFrom(stream, settings, leaveOpen: true);

        var buffer = new byte[10];
        Assert.True(provider.TryRead(buffer, 0, 3));
        Assert.Equal(new byte[] { 0x41, 0x42, 0x43, 0, 0, 0, 0, 0, 0, 0 }, buffer);
        Assert.Equal(3, provider.Position);
    }

    [Fact]
    public void Seek_WorksCorrectly()
    {
        var data = new byte[] { 1, 2, 3, 4, 5 };
        using var stream = new MemoryStream(data);
        var settings = new ReaderSettings();
        using var provider = DisposableBytesProvider.GetFrom(stream, settings, leaveOpen: true);

        var newPosition = provider.Seek(2, SeekOrigin.Begin);
        Assert.Equal(2, newPosition);
        Assert.Equal(2, provider.Position);

        newPosition = provider.Seek(1, SeekOrigin.Current);
        Assert.Equal(3, newPosition);
        Assert.Equal(3, provider.Position);
    }

    [Fact]
    public void Dispose_DisposesUnderlyingStream_WhenNotLeaveOpen()
    {
        var data = new byte[] { 1, 2, 3, 4, 5 };
        var stream = new TrackableMemoryStream(data);
        var settings = new ReaderSettings();

        var provider = DisposableBytesProvider.GetFrom(stream, settings, leaveOpen: false);
        provider.Dispose();

        Assert.True(stream.IsDisposed);
    }

    [Fact]
    public void Dispose_DoesNotDisposeUnderlyingStream_WhenLeaveOpen()
    {
        var data = new byte[] { 1, 2, 3, 4, 5 };
        using var stream = new TrackableMemoryStream(data);
        var settings = new ReaderSettings();

        var provider = DisposableBytesProvider.GetFrom(stream, settings, leaveOpen: true);
        provider.Dispose();

        Assert.False(stream.IsDisposed);
    }

    [Fact]
    public void AccessAfterDispose_ThrowsObjectDisposedException()
    {
        var data = new byte[] { 1, 2, 3, 4, 5 };
        using var stream = new MemoryStream(data);
        var settings = new ReaderSettings();

        var provider = DisposableBytesProvider.GetFrom(stream, settings, leaveOpen: true);
        provider.Dispose();

        Assert.Throws<ObjectDisposedException>(() => provider.Position);
        Assert.Throws<ObjectDisposedException>(() => provider.Position = 0);
        Assert.Throws<ObjectDisposedException>(() => provider.Length);
        Assert.Throws<ObjectDisposedException>(() => provider.TryRead(out _));
        Assert.Throws<ObjectDisposedException>(() => provider.TryRead(new byte[1], 0, 1));
        Assert.Throws<ObjectDisposedException>(() => provider.Seek(0, SeekOrigin.Begin));
    }

    [Fact]
    public void MultipleDispose_DoesNotThrow()
    {
        var data = new byte[] { 1, 2, 3, 4, 5 };
        using var stream = new MemoryStream(data);
        var settings = new ReaderSettings();

        var provider = DisposableBytesProvider.GetFrom(stream, settings, leaveOpen: true);
        provider.Dispose();
        provider.Dispose(); // Should not throw
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
        private readonly MemoryStream _innerStream;

        public NonSeekableStream(byte[] data)
        {
            _innerStream = new MemoryStream(data);
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => _innerStream.Length;
        public override long Position 
        { 
            get => _innerStream.Position; 
            set => throw new NotSupportedException(); 
        }

        public override void Flush() => _innerStream.Flush();
        public override int Read(byte[] buffer, int offset, int count) => _innerStream.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _innerStream?.Dispose();
            base.Dispose(disposing);
        }
    }

    private class FileStreamWrapper : Stream
    {
        private readonly MemoryStream _innerStream;

        public FileStreamWrapper(byte[] data)
        {
            _innerStream = new MemoryStream(data);
        }

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length => _innerStream.Length;
        public override long Position 
        { 
            get => _innerStream.Position; 
            set => _innerStream.Position = value; 
        }

        public override void Flush() => _innerStream.Flush();
        public override int Read(byte[] buffer, int offset, int count) => _innerStream.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => _innerStream.Seek(offset, origin);
        public override void SetLength(long value) => _innerStream.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => _innerStream.Write(buffer, offset, count);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _innerStream?.Dispose();
            base.Dispose(disposing);
        }
    }

    private class TrackableMemoryStream : MemoryStream
    {
        public bool IsDisposed { get; private set; }

        public TrackableMemoryStream(byte[] buffer) : base(buffer) { }

        protected override void Dispose(bool disposing)
        {
            IsDisposed = true;
            base.Dispose(disposing);
        }
    }
}