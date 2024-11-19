using Synercoding.FileFormats.Pdf.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Parsing.Internal;

internal class XRefTable
{
    private readonly Dictionary<PdfObjectId, XRefItem> _lookup = [];
    public XRefTable(IEnumerable<XRefItem> items)
    {
        foreach (var item in items)
            _lookup[item.Id] = item;
    }

    private XRefTable(Dictionary<PdfObjectId, XRefItem> lookup)
    {
        _lookup = lookup;
    }

    public XRefTable Merge(XRefTable update)
    {
        Dictionary<PdfObjectId, XRefItem> merged = [];

        foreach (var item in Items)
            merged[item.Id] = item;

        foreach (var item in update.Items)
            merged[item.Id] = item;

        return new XRefTable(merged);
    }

    public XRefTable SetItem(XRefItem item)
    {
        _lookup[item.Id] = item;
        return this;
    }

    public XRefItem[] Items { get => _lookup.Values.ToArray(); }

    public XRefItem this[PdfObjectId id]
        => _lookup[id];

    public bool TryGet(PdfObjectId id, [NotNullWhen(true)] out XRefItem? item)
        => _lookup.TryGetValue(id, out item);
}
