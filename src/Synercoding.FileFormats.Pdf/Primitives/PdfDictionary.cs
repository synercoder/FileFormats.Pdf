using Synercoding.FileFormats.Pdf.Exceptions;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Primitives;

[DebuggerDisplay("{ToString(),nq}")]
public class PdfDictionary : IPdfDictionary
{
    private readonly Dictionary<PdfName, IPdfPrimitive> _dictionary = new();

    public PdfDictionary()
    { }

    public PdfDictionary(IPdfDictionary dictionary)
    {
        foreach (var (key, value) in dictionary)
            _dictionary.Add(key, value);
    }

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

    public virtual bool TryGetValue<TPrimitive>(PdfName key, [NotNullWhen(true)] out TPrimitive? value)
        where TPrimitive : IPdfPrimitive
    {
        if (_dictionary.TryGetValue(key, out var primitive) && primitive is TPrimitive actualValue)
        {
            value = actualValue;
            return true;
        }

        value = default;
        return false;
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    [DebuggerStepThrough]
    public override string ToString()
        => $"[Pdf Dictionary] Count = {Count}";
}
