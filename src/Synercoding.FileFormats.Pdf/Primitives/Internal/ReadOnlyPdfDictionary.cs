using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Primitives.Internal;

[DebuggerDisplay("{ToString(),nq}")]
internal class ReadOnlyPdfDictionary : IPdfDictionary
{
    private readonly IPdfDictionary _dictionary;

    public ReadOnlyPdfDictionary(IPdfDictionary dictionary)
    {
        _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
    }

    public static IPdfDictionary Empty { get; }
        = new ReadOnlyPdfDictionary(new PdfDictionary());

    public IPdfPrimitive? this[PdfName key]
        => _dictionary[key];

    public ICollection<PdfName> Keys
        => _dictionary.Keys;

    public int Count
        => _dictionary.Count;

    public bool ContainsKey(PdfName key)
        => _dictionary.ContainsKey(key);

    public IEnumerator<KeyValuePair<PdfName, IPdfPrimitive>> GetEnumerator()
        => _dictionary.GetEnumerator();

    public bool TryGetValue(PdfName key, [NotNullWhen(true)] out IPdfPrimitive? value)
        => _dictionary.TryGetValue(key, out value);

    public bool TryGetValue<TPrimitive>(PdfName key, [NotNullWhen(true)] out TPrimitive? value)
        where TPrimitive : IPdfPrimitive
        => _dictionary.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator()
        => ( (IEnumerable)_dictionary ).GetEnumerator();

    [DebuggerStepThrough]
    public override string ToString()
        => $"[Pdf Dictionary] Count = {Count}";
}
