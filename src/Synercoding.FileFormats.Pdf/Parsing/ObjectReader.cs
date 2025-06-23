using Synercoding.FileFormats.Pdf.Encryption;
using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Parsing.Encryption;
using Synercoding.FileFormats.Pdf.Parsing.Internal;
using Synercoding.FileFormats.Pdf.Parsing.Internal.XRef;
using Synercoding.FileFormats.Pdf.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Parsing;

public sealed class ObjectReader : IDisposable
{
    private readonly IDictionary<PdfObjectId, ObjectStream> _objectStreams = new Dictionary<PdfObjectId, ObjectStream>();

    private string? _password;

    private Parser _parser;
    private bool _disposedValue;

    private PdfHeader? _pdfHeader;
    private Trailer? _trailer;
    private XRefTable? _xRefTable;

    public ObjectReader(string filePath)
        : this(filePath, string.Empty)
    { }

    public ObjectReader(string filePath, ReaderSettings settings)
        : this(filePath, string.Empty, settings)
    { }

    public ObjectReader(Stream stream)
        : this(stream, string.Empty)
    { }

    public ObjectReader(Stream stream, ReaderSettings settings)
        : this(stream, string.Empty, settings)
    { }

    public ObjectReader(byte[] bytes)
        : this(bytes, string.Empty)
    { }

    public ObjectReader(Stream stream, ReaderSettings settings, bool disposeStream)
        : this(stream, string.Empty, settings, disposeStream)
    { }

    public ObjectReader(byte[] bytes, ReaderSettings settings)
        : this(bytes, string.Empty, settings)
    { }

    public ObjectReader(string filePath, string password)
        : this(filePath, password, new ReaderSettings())
    { }

    public ObjectReader(string filePath, string password, ReaderSettings settings)
        : this(File.OpenRead(filePath), password, settings, true)
    { }

    public ObjectReader(Stream stream, string password)
        : this(stream, password, new ReaderSettings())
    { }

    public ObjectReader(Stream stream, string password, ReaderSettings settings)
        : this(stream, password, settings, false)
    { }

    public ObjectReader(byte[] bytes, string password)
        : this(bytes, password, new ReaderSettings())
    { }

    public ObjectReader(Stream stream, string password, ReaderSettings settings, bool disposeStream)
        : this(DisposableBytesProvider.GetFrom(stream, settings, disposeStream), password, settings)
    { }

    public ObjectReader(byte[] bytes, string password, ReaderSettings settings)
        : this(new PdfByteArrayProvider(bytes), password, settings)
    { }
    public ObjectReader(IPdfBytesProvider pdfBytesProvider, ReaderSettings settings)
        : this(pdfBytesProvider, string.Empty, settings)
    { }

    public ObjectReader(IPdfBytesProvider pdfBytesProvider, string password, ReaderSettings settings)
    {
        Settings = settings ?? throw new ArgumentNullException(nameof(settings));

        _password = password;

        var tokenizer = new Lexer(pdfBytesProvider, Settings.Logger);
        _parser = new Parser(tokenizer, null, Settings.Logger);
    }

    public PdfVersion Version
    {
        get
        {
            _ensureHeaderAndFooterIsRead();
            return _pdfHeader.Version;
        }
    }

    private EncryptionInfo? _encryptionInfo;
    public EncryptionInfo Encryption
    {
        get
        {
            _ensureHeaderAndFooterIsRead();
            return _encryptionInfo;
        }
    }

    internal ReaderSettings Settings { get; }

    internal Trailer Trailer
    {
        get
        {
            _ensureHeaderAndFooterIsRead();
            return _trailer;
        }
    }

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

    [MemberNotNull(nameof(_pdfHeader), nameof(_trailer), nameof(_xRefTable), nameof(_encryptionInfo))]
    private void _ensureHeaderAndFooterIsRead()
    {
        if (_pdfHeader is null || _trailer is null || _xRefTable is null || _encryptionInfo is null)
        {
            _pdfHeader = PdfHeader.Parse(_parser.Lexer.PdfBytesProvider);
            var footer = PdfFooter.CreateDefault();
            _trailer = footer.GetTrailer(_parser.Lexer.PdfBytesProvider, _pdfHeader.PdfStart, this.Settings);

            if (_encryptionInfo is null)
            {
                _encryptionInfo = new EncryptionInfo();

                if (_trailer.ID is not null && _trailer.Encrypt is not null)
                {
                    var decryptorDictionary = new StandardEncryptionDictionary(_trailer.Encrypt, this);

                    _encryptionInfo = new EncryptionInfo(decryptorDictionary, this);

                    if (Encryption.Decrypt(_password))
                    {
                        _password = null;
                        _parser = new Parser(_parser.Lexer, Encryption.Decryptor, Settings.Logger);
                    }
                    else
                    {
                        Encryption.DecryptionSuccessful += (s, e) =>
                        {
                            _parser = new Parser(_parser.Lexer, Encryption.Decryptor, Settings.Logger);
                        };
                    }
                }
            }

            _xRefTable = footer.Parse(_parser.Lexer.PdfBytesProvider, _pdfHeader.PdfStart, this);
        }
    }

    private bool _tryReadFromStreamObject<TPrimitive>(CompressedXRefItem compressedXRef, [NotNullWhen(true)] out TPrimitive? pdfObject)
        where TPrimitive : IPdfPrimitive
    {
        _ensureHeaderAndFooterIsRead();
        pdfObject = default;

        if (!_objectStreams.TryGetValue(compressedXRef.ObjectStreamId, out var objStream))
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
        _parser.Lexer.PdfBytesProvider.Seek(_pdfHeader.PdfStart + classicXRef.Offset, SeekOrigin.Begin);

        try
        {
            pdfObject = _parser.ReadObject<TPrimitive>().Value;
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
            if (_parser.Lexer.PdfBytesProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }

            _disposedValue = true;
        }
        GC.SuppressFinalize(this);
    }
}
