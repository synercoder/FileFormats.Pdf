using Synercoding.FileFormats.Pdf.DocumentObjects;
using Synercoding.FileFormats.Pdf.DocumentObjects.Internal;
using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Logging;
using Synercoding.FileFormats.Pdf.Parsing;
using Synercoding.FileFormats.Pdf.Primitives;
using System.Collections;

namespace Synercoding.FileFormats.Pdf;

public sealed class PdfReader : IReadOnlyList<IReadOnlyPage>, IDisposable
{
    private readonly ObjectReader _objectReader;
    private readonly IPdfLogger _logger;
    private readonly Catalog _catalog;

    private readonly IList<IReadOnlyPage> _pages = new List<IReadOnlyPage>();

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
        _logger = _objectReader.Settings.Logger;

        var root = _objectReader.Trailer.Root;

        if (!_objectReader.TryGet<IPdfDictionary>(root.Id, out var catalogDictionary))
        {
            _logger.LogCritical<PdfReader>("Could not retrieve a dictionary for id {Id}", root.Id);
            throw new ParseException($"Could not retrieve a dictionary for id {root.Id}");
        }

        _catalog = new Catalog(catalogDictionary, _objectReader);

        if (_objectReader.Trailer.Info is PdfReference infoRef
            && _objectReader.TryGet<IPdfDictionary>(infoRef.Id, out var documentInfoDictionary))
        {
            DocumentInformation = new ReadOnlyDocumentInformation(documentInfoDictionary, _objectReader);
        }

        if (_objectReader.Trailer.ID.HasValue)
        {
            Id = new PdfIds(
                originalId: _objectReader.Trailer.ID.Value.OriginalId,
                lastVersionId: _objectReader.Trailer.ID.Value.LastVersionId
            );
        }
    }

    public IReadOnlyPage this[int index]
    {
        get
        {
            _ensurePagesAreLoaded();
            return _pages[index];
        }
    }

    public int Count
    {
        get
        {
            _ensurePagesAreLoaded();
            return _pages.Count;
        }
    }

    public IReadOnlyDocumentInformation? DocumentInformation { get; }

    public PdfIds? Id { get; }

    public void Dispose()
    {
        _objectReader.Dispose();
    }

    public IEnumerator<IReadOnlyPage> GetEnumerator()
    {
        _ensurePagesAreLoaded();
        return _pages.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void _ensurePagesAreLoaded()
    {
        if (_pages.Count > 0)
            return;

        foreach (var page in _getPagesFrom(_catalog.Pages))
            _pages.Add(page);
    }

    private IEnumerable<IReadOnlyPage> _getPagesFrom(PageTreeNode node)
    {
        foreach (var kid in node.Kids)
        {
            if (kid.TryGetFirst(out var subNode))
            {
                foreach (var subPage in _getPagesFrom(subNode))
                    yield return subPage;
            }
            else if (kid.TryGetSecond(out var page))
            {
                yield return page;
            }
        }
    }
}
