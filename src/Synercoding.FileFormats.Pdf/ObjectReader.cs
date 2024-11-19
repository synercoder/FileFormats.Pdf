using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Parsing;
using Synercoding.FileFormats.Pdf.Parsing.Internal;
using Synercoding.FileFormats.Pdf.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf;

public class ObjectReader : IDisposable
{
    private readonly IPdfBytesProvider _pdfBytesProvider;
    private bool _disposedValue;

    private PdfHeader? _pdfHeader;
    private Trailer? _trailer;
    private XRefTable? _xRefTable;

    public ObjectReader(string filePath)
        : this(filePath, new ReaderSettings())
    { }

    public ObjectReader(string filePath, ReaderSettings settings)
        : this(File.OpenRead(filePath), settings, true)
    { }

    public ObjectReader(Stream stream)
        : this(stream, new ReaderSettings())
    { }

    public ObjectReader(Stream stream, ReaderSettings settings)
        : this(stream, settings, false)
    { }

    public ObjectReader(byte[] bytes)
        : this(bytes, new ReaderSettings())
    { }

    public ObjectReader(Stream stream, ReaderSettings settings, bool disposeStream)
        : this(DisposableBytesProvider.GetFrom(stream, settings, disposeStream), settings)
    { }

    public ObjectReader(byte[] bytes, ReaderSettings settings)
        : this(new PdfByteArrayProvider(bytes), settings)
    { }

    public ObjectReader(IPdfBytesProvider pdfBytesProvider, ReaderSettings settings)
    {
        _pdfBytesProvider = pdfBytesProvider ?? throw new ArgumentNullException(nameof(pdfBytesProvider));
        Settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    [MemberNotNull(nameof(_pdfHeader), nameof(_trailer), nameof(_xRefTable))]
    private void _ensureHeaderAndFooterIsRead()
    {
        if (_pdfHeader is null || _trailer is null || _xRefTable is null)
        {
            _pdfHeader = PdfHeader.Parse(_pdfBytesProvider);
            (_trailer, _xRefTable) = PdfFooter.Parse(_pdfBytesProvider, _pdfHeader.PdfStart, this);
        }
    }

    public PdfVersion Version
    {
        get
        {
            _ensureHeaderAndFooterIsRead();
            return _pdfHeader.Version;
        }
    }

    internal ReaderSettings Settings { get; }

    public bool TryGet<TPrimitive>(PdfObjectId id, [NotNullWhen(true)] out TPrimitive? pdfObject)
        where TPrimitive : IPdfPrimitive
    {
        _ensureHeaderAndFooterIsRead();
        pdfObject = default;

        if (!_xRefTable.TryGet(id, out var xRefItem))
            return false;

        if (xRefItem is FreeXRefItem)
            return false;

        if (xRefItem is ClassicXRefItem classicXRef)
            return _tryReadFromMainStream(classicXRef, out pdfObject);

        if (xRefItem is CompressedXRefItem compressedXRefItem)
            return _tryReadFromStreamObject(compressedXRefItem, out pdfObject);

        throw new InvalidOperationException($"Unknown XRefItem type: {typeof(XRefItem).Name}");
    }

    private readonly IDictionary<PdfObjectId, ObjectStream> _objectStreams = new Dictionary<PdfObjectId, ObjectStream>();
    private bool _tryReadFromStreamObject<TPrimitive>(CompressedXRefItem compressedXRef, [NotNullWhen(true)] out TPrimitive? pdfObject)
        where TPrimitive : IPdfPrimitive
    {
        _ensureHeaderAndFooterIsRead();
        pdfObject = default;

        if(!_objectStreams.TryGetValue(compressedXRef.ObjectStreamId, out var objStream))
        {
            if (!TryGet<IPdfStreamObject>(compressedXRef.ObjectStreamId, out var streamObject))
                return false;
            objStream = new ObjectStream(streamObject, this);
        }

        if (!objStream.TryGet(compressedXRef.Id, out pdfObject))
            return false;

        return true;
    }

    private bool _tryReadFromMainStream<TPrimitive>(ClassicXRefItem classicXRef, [NotNullWhen(true)] out TPrimitive? pdfObject)
        where TPrimitive : IPdfPrimitive
    {
        _ensureHeaderAndFooterIsRead();
        _pdfBytesProvider.Seek(_pdfHeader.PdfStart + classicXRef.Offset, SeekOrigin.Begin);
        var tokenizer = new Tokenizer(_pdfBytesProvider, Settings.Logger);
        var parser = new Parser(tokenizer, Settings.Logger);

        try
        {
            pdfObject = parser.ReadObject<TPrimitive>().Value;
            return true;
        }
        catch
        {
            pdfObject = default;
            return false;
        }
    }

    public void Dispose()
    {
        if (!_disposedValue)
        {
            if (_pdfBytesProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }

            _disposedValue = true;
        }
        GC.SuppressFinalize(this);
    }
}
