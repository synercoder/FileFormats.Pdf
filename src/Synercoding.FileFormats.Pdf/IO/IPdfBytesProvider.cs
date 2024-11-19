using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.IO;

public interface IPdfBytesProvider
{
    long Position { get; set; }
    long Length { get; }
    bool TryRead(out byte b);
    bool TryRead(byte[] buffer, int offset, int count);
    long Seek(long offset, SeekOrigin origin);
}
