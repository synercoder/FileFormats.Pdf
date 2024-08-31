using Synercoding.FileFormats.Pdf.Exceptions;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Primitives;

[DebuggerDisplay("{ToString(),nq}")]
public sealed class PdfDictionary : IPdfPrimitive, IEnumerable<KeyValuePair<PdfName, IPdfPrimitive>>
{
    private readonly Dictionary<PdfName, IPdfPrimitive> _dictionary = new();

    public IPdfPrimitive? this[PdfName key]
    {
        get => _dictionary[key];
        set
        {
            ArgumentNullException.ThrowIfNull(key);

            // Pdf 2.0 Ref 7.3.7: A dictionary entry whose value is null (see 7.3.9, "Null object")
            // shall be treated the same as if the entry does not exist.
            if (value is null)
            {
                _dictionary.Remove(key);
            }
            else
            {
                _dictionary[key] = value;
            }
        }
    }

    public ICollection<PdfName> Keys
        => _dictionary.Keys;

    public int Count
        => _dictionary.Count;

    public void Add(PdfName key, IPdfPrimitive value)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);

        if (!_dictionary.TryAdd(key, value))
        {
            throw new PdfException($"{nameof(PdfDictionary)} already contains an entry with key {key.Display}");
        }
    }

    public void Clear()
        => _dictionary.Clear();

    public bool ContainsKey(PdfName key)
        => _dictionary.ContainsKey(key);

    public IEnumerator<KeyValuePair<PdfName, IPdfPrimitive>> GetEnumerator()
        => _dictionary.GetEnumerator();

    public bool Remove(PdfName key)
        => _dictionary.Remove(key);

    public bool TryGetValue(PdfName key, [NotNullWhen(true)] out IPdfPrimitive? value)
        => _dictionary.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    [DebuggerStepThrough]
    public override string ToString()
        => $"[Pdf Dictionary] Count = {Count}";
}
