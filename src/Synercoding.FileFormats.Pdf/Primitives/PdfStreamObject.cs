using Synercoding.FileFormats.Pdf.Exceptions;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Primitives;

public sealed class PdfStreamObject : IPdfStreamObject
{
    private readonly IPdfDictionary _dictionary;

    public PdfStreamObject()
        : this(Array.Empty<byte>())
    { }

    public PdfStreamObject(byte[] rawData)
        : this(new PdfDictionary() { [PdfNames.Length] = new PdfInteger(rawData.LongLength) }, rawData)
    { }

    public PdfStreamObject(IPdfDictionary dictionary, byte[] rawData)
    {
        if (!dictionary.ContainsKey(PdfNames.Length))
            throw new ArgumentException("Provided dictionary does not contain a \"/Length\" property.", nameof(dictionary));

        _dictionary = dictionary;
        RawData = rawData ?? throw new ArgumentNullException(nameof(rawData));
    }

    public IPdfPrimitive? this[PdfName key] => _dictionary[key];

    public byte[] RawData { get; }

    public long Length
    {
        get
        {
            if (!TryGetValue(PdfNames.Length, out var lengthPrimitive) || lengthPrimitive is not PdfInteger lengthInteger)
                throw new PdfException("Tried to retrieve the length property from a stream object, but no length integer was found.");

            return lengthInteger.Value;
        }
    }

    public ICollection<PdfName> Keys => _dictionary.Keys;

    public int Count => _dictionary.Count;

    public bool ContainsKey(PdfName key)
    {
        return _dictionary.ContainsKey(key);
    }

    public IEnumerator<KeyValuePair<PdfName, IPdfPrimitive>> GetEnumerator()
    {
        return _dictionary.GetEnumerator();
    }

    public bool TryGetValue(PdfName key, [NotNullWhen(true)] out IPdfPrimitive? value)
    {
        return _dictionary.TryGetValue(key, out value);
    }

    public bool TryGetValue<TPrimitive>(PdfName key, [NotNullWhen(true)] out TPrimitive? value) where TPrimitive : IPdfPrimitive
    {
        return _dictionary.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ( (IEnumerable)_dictionary ).GetEnumerator();
    }
}
