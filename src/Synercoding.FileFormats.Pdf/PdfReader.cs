using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Parsing;

namespace Synercoding.FileFormats.Pdf;

public class PdfReader
{
    private readonly ObjectReader _objectReader;

    public PdfReader(string filePath)
        : this(filePath, new ReaderSettings())
    { }

    public PdfReader(string filePath, ReaderSettings settings)
        : this(File.OpenRead(filePath), settings, true)
    { }

    public PdfReader(Stream stream)
        : this(stream, new ReaderSettings())
    { }

    public PdfReader(Stream stream, ReaderSettings settings)
        : this(stream, settings, false)
    { }

    public PdfReader(byte[] bytes)
        : this(bytes, new ReaderSettings())
    { }

    public PdfReader(Stream stream, ReaderSettings settings, bool disposeStream)
        : this(DisposableBytesProvider.GetFrom(stream, settings, disposeStream), settings)
    { }

    public PdfReader(byte[] bytes, ReaderSettings settings)
        : this(new PdfByteArrayProvider(bytes), settings)
    { }

    public PdfReader(IPdfBytesProvider bytesProvider, ReaderSettings settings)
        : this(new ObjectReader(bytesProvider, settings))
    { }

    public PdfReader(ObjectReader objectReader)
    {
        _objectReader = objectReader;
    }
}
