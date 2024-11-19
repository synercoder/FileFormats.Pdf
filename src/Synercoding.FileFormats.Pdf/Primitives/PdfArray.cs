using System.Collections;
using System.Diagnostics;

namespace Synercoding.FileFormats.Pdf.Primitives;

[DebuggerDisplay("{ToString(),nq}")]
public sealed class PdfArray : IPdfPrimitive, IEnumerable<IPdfPrimitive>
{
    private readonly List<IPdfPrimitive> _list;

    public PdfArray()
        : this(Array.Empty<IPdfPrimitive>())
    { }

    public PdfArray(IEnumerable<IPdfPrimitive> items)
    {
        _list = new List<IPdfPrimitive>(items);
    }

    public IPdfPrimitive this[int index]
    {
        get { return _list[index]; }
        set { _list[index] = value;}
    }

    public int Count
        => _list.Count;

    public void Add(IPdfPrimitive item)
        => _list.Add(item);

    public void Insert(int index, IPdfPrimitive item)
        => _list.Insert(index, item);

    public bool Remove(IPdfPrimitive item)
        => _list.Remove(item);

    public void RemoveAt(int index)
        => _list.RemoveAt(index);

    public void Clear()
        => _list.Clear();

    public IEnumerator<IPdfPrimitive> GetEnumerator()
        => _list.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    [DebuggerStepThrough]
    public override string ToString()
        => $"[Pdf Array] Count = {Count}";
}
