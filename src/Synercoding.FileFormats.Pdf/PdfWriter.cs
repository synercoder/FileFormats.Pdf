using Synercoding.FileFormats.Pdf.DocumentObjects;
using Synercoding.FileFormats.Pdf.Generation;
using Synercoding.FileFormats.Pdf.Generation.Internal;

namespace Synercoding.FileFormats.Pdf;

public class PdfWriter : IDisposable
{
    private readonly PdfStream _pdfStream;
    private readonly WriterSettings _settings;
    private readonly ObjectWriter _objectWriter;

    private bool _trailerWritten = false;

    public PdfWriter(string filePath)
        : this(filePath, new WriterSettings())
    { }

    public PdfWriter(string filePath, WriterSettings settings)
        : this(File.OpenRead(filePath), settings, true)
    { }

    public PdfWriter(Stream stream)
        : this(stream, new WriterSettings())
    { }

    public PdfWriter(Stream stream, WriterSettings settings)
        : this(stream, settings, false)
    { }

    public PdfWriter(Stream stream, WriterSettings settings, bool ownsStream)
        : this(new PdfStream(stream, ownsStream), settings)
    { }

    public PdfWriter(PdfStream pdfStream, WriterSettings settings)
    {
        _pdfStream = pdfStream;
        _settings = settings;

        var pdfStart = PdfHeaderWriter.WriteTo(_pdfStream, new PdfVersion(2, 0));

        _objectWriter = new ObjectWriter(_pdfStream, pdfStart);

        DocumentInformation = new DocumentInformation(this);
    }

    internal void ThrowsWhenEndingWritten()
        => _ = _trailerWritten
        ? throw new InvalidOperationException("Can not modify the PDF further since the trailer has been written to the stream.")
        : 0;

    /// <summary>
    /// Document information, such as the author and title
    /// </summary>
    public DocumentInformation DocumentInformation { get; }

    public void Dispose()
    {
        if (_trailerWritten)
            return;

        _writeTrailer();
        _pdfStream.Dispose();
    }

    private void _writeTrailer()
    {
        _trailerWritten = true;

        throw new NotImplementedException();
    }
}
