namespace Synercoding.FileFormats.Pdf.IO;

internal sealed class DisposableBytesProvider : IPdfBytesProvider, IDisposable
{
    private readonly PdfStreamBytesProvider _internalProvider;
    private readonly Stream? _stream;
    private bool _disposedValue;

    public static DisposableBytesProvider GetFrom(Stream stream, ReaderSettings settings, bool disposeStream)
    {
        if (!stream.CanRead)
            throw new ArgumentException("Provided stream does not support reading.", nameof(stream));

        var needsCopy = !stream.CanSeek || stream.Length <= settings.MaxMemoryCopy;

        if (!needsCopy)
            return new DisposableBytesProvider(stream, disposeStream);

        var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        memoryStream.Position = 0;

        if (disposeStream)
            stream.Dispose();

        return new DisposableBytesProvider(memoryStream, disposeStream);
    }

    private DisposableBytesProvider(Stream stream, bool disposeStream)
    {
        _internalProvider = new PdfStreamBytesProvider(stream);
        _stream = disposeStream ? stream : null;
    }

    public long Position
    {
        get => _internalProvider.Position;
        set => _internalProvider.Position = value;
    }

    public long Length => _internalProvider.Length;

    public long Seek(long offset, SeekOrigin origin)
    {
        return _internalProvider.Seek(offset, origin);
    }

    public bool TryRead(out byte b)
    {
        return _internalProvider.TryRead(out b);
    }

    public bool TryRead(byte[] buffer, int offset, int count)
    {
        return _internalProvider.TryRead(buffer, offset, count);
    }

    public void Dispose()
    {
        if (!_disposedValue)
        {
            _stream?.Dispose();
            _disposedValue = true;
        }
        GC.SuppressFinalize(this);
    }
}
