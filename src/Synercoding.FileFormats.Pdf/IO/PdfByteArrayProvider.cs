namespace Synercoding.FileFormats.Pdf.IO;

public class PdfByteArrayProvider : IPdfBytesProvider
{
    private readonly byte[] _bytes;

    public PdfByteArrayProvider(byte[] bytes)
    {
        _bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));
    }

    private long _position;
    public long Position
    {
        get
        {
            return _position;
        }
        set
        {
            if(value < 0)
                throw new ArgumentOutOfRangeException(nameof(Position), "Provided position can't be negative.");

            if (value > Length)
                throw new ArgumentOutOfRangeException(nameof(Position), $"Provided value ({value}) is higher than {nameof(Length)} ({Length}).");

            _position = value;
        }
    }

    public long Length => _bytes.LongLength;

    public long Seek(long offset, SeekOrigin origin)
    {
        Position = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.Current => Position + offset,
            SeekOrigin.End => _bytes.Length + offset,
            var unknown => throw new ArgumentOutOfRangeException(nameof(origin), $"Unsupported value {unknown} for enum {typeof(SeekOrigin).Name}")
        };

        return Position;
    }

    public bool TryRead(out byte b)
    {
        b = default;

        if (Position >= Length)
            return false;

        b = _bytes[Position++];
        return true;
    }

    public bool TryRead(byte[] buffer, int offset, int count)
    {
        if (Position + count > Length)
            return false;
        if (offset + count > buffer.Length)
            return false;

        for (int index = offset; index < offset + count; index++)
        {
            buffer[index] = _bytes[Position++];
        }

        return true;
    }
}
