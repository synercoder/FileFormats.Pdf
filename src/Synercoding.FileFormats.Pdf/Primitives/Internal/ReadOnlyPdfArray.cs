using System.Collections;
using System.Diagnostics;

namespace Synercoding.FileFormats.Pdf.Primitives.Internal;

[DebuggerDisplay("{ToString(),nq}")]
internal sealed class ReadOnlyPdfArray : IPdfArray
{
    private readonly IPdfArray _array;

    public ReadOnlyPdfArray(IPdfArray array)
    {
        _array = array;
    }

    public IPdfPrimitive this[int index]
        => _array[index];

    public int Count
        => _array.Count;

    public IEnumerator<IPdfPrimitive> GetEnumerator()
        => _array.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => ( (IEnumerable)_array ).GetEnumerator();

    [DebuggerStepThrough]
    public override string ToString()
        => $"[Pdf Array] Count = {Count}";
}
