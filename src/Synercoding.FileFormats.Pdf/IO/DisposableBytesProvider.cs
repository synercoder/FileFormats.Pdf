namespace Synercoding.FileFormats.Pdf.IO;

internal class DisposableBytesProvider : IPdfBytesProvider, IDisposable
{
    private readonly PdfStreamBytesProvider _internalProvider;
    private readonly Stream? _stream;
    protected bool _disposedValue;

    public static DisposableBytesProvider GetFrom(Stream stream, ReaderSettings settings, bool leaveOpen)
    {
        if (!stream.CanRead)
            throw new ArgumentException("Provided stream does not support reading.", nameof(stream));

        // First check if the stream is not seekable (needed for pdf),
        // but larger than the max capacity for a memorystream.
        // If so, use a temp file instead.
        if (!stream.CanSeek && stream.Length > int.MaxValue)
            return _copyToAndUseFileStream(stream, leaveOpen);

        // Check if we should copy the provided pdf stream to memory, because:
        // A) Stream is not seekable
        // B) Stream is not already a MemoryStream, and length is less then the provided max.
        var needsCopy = !stream.CanSeek || ( stream is not MemoryStream && stream.Length <= settings.MaxMemoryCopy );

        if (!needsCopy)
            return new DisposableBytesProvider(stream, leaveOpen);

        // Copy the provided stream to memory
        var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        memoryStream.Position = 0;

        // Dispose original stream if needed
        if (!leaveOpen)
            stream.Dispose();

        return new DisposableBytesProvider(memoryStream, true);
    }

    private static TempFileBytesProvider _copyToAndUseFileStream(Stream stream, bool leaveOpen)
    {
        var filePath = Path.GetTempFileName();

        // Copy the provided stream to a temp file
        var fs = File.OpenWrite(filePath);
        stream.CopyTo(fs);
        fs.Position = 0;

        // Dispose original stream if needed
        if (!leaveOpen)
            stream.Dispose();

        return new TempFileBytesProvider(stream, filePath);
    }

    protected DisposableBytesProvider(Stream stream, bool leaveOpen)
    {
        _internalProvider = new PdfStreamBytesProvider(stream);
        _stream = !leaveOpen ? stream : null;
    }

    public long Position
    {
        get
        {
            _throwsIfNotAlreadyDisposed();
            return _internalProvider.Position;
        }
        set
        {
            _throwsIfNotAlreadyDisposed();
            _internalProvider.Position = value;
        }
    }

    public long Length
    {
        get
        {
            _throwsIfNotAlreadyDisposed();
            return _internalProvider.Length;
        }
    }

    public long Seek(long offset, SeekOrigin origin)
    {
        _throwsIfNotAlreadyDisposed();
        return _internalProvider.Seek(offset, origin);
    }

    public bool TryRead(out byte b)
    {
        _throwsIfNotAlreadyDisposed();
        return _internalProvider.TryRead(out b);
    }

    public bool TryRead(byte[] buffer, int offset, int count)
    {
        _throwsIfNotAlreadyDisposed();
        return _internalProvider.TryRead(buffer, offset, count);
    }

    protected virtual void _dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _stream?.Dispose();
            }

            _disposedValue = true;
        }
    }

    private void _throwsIfNotAlreadyDisposed()
    {
        if (_disposedValue)
            throw new ObjectDisposedException(nameof(DisposableBytesProvider), "This provider is already disposed.");
    }

    public void Dispose()
    {
        _dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
