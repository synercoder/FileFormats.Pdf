using Synercoding.FileFormats.Pdf.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.IO;

public class PdfStreamBytesProvider : IPdfBytesProvider
{
    private readonly Stream _stream;

    public PdfStreamBytesProvider(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanRead)
            throw new ArgumentException("Provided stream does not support reading.", nameof(stream));
        if (!stream.CanSeek)
            throw new ArgumentException("Provided stream does not support seeking.", nameof(stream));

        _stream = stream;
    }

    public long Position
        => _stream.Position;

    public long Length
        => _stream.Length;

    public bool TryRead(out byte b)
    {
        b = default;
        var value = _stream.ReadByte();
        if (value == -1)
            return false;

        b = (byte)value;
        return true;
    }

    public bool TryRead(byte[] buffer, int offset, int count)
    {
        if (offset + count > buffer.Length)
            return false;

        var read = _stream.Read(buffer, offset, count);
        if (read != count)
            return false;

        return true;
    }

    public long Seek(long offset, SeekOrigin origin)
        => _stream.Seek(offset, origin);
}
