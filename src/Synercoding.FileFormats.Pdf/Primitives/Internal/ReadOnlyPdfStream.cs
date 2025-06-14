using Synercoding.FileFormats.Pdf.Exceptions;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Primitives.Internal;

[DebuggerDisplay("{ToString(),nq}")]
internal sealed class ReadOnlyPdfStream : IPdfStream
{
    private readonly IPdfDictionary _dictionary;

    public ReadOnlyPdfStream()
        : this(Array.Empty<byte>())
    { }

    public ReadOnlyPdfStream(byte[] rawData)
        : this(new PdfDictionary() { [PdfNames.Length] = new PdfNumber(rawData.LongLength) }, rawData)
    { }

    public ReadOnlyPdfStream(IPdfDictionary dictionary, byte[] rawData)
    {
        if (!dictionary.ContainsKey(PdfNames.Length))
            throw new ArgumentException("Provided dictionary does not contain a \"/Length\" property.", nameof(dictionary));

        _dictionary = dictionary;
        RawData = rawData ?? throw new ArgumentNullException(nameof(rawData));
    }

    public IPdfPrimitive? this[PdfName key]
        => _dictionary[key];

    public byte[] RawData { get; }

    public long Length
    {
        get
        {
            if (!TryGetValue(PdfNames.Length, out var lengthPrimitive) || lengthPrimitive is not PdfNumber lengthInteger)
                throw new PdfException("Tried to retrieve the length property from a stream object, but no length integer was found.");

            return (long)lengthInteger.Value;
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

    [DebuggerStepThrough]
    public override string ToString()
        => $"[Pdf Stream] Length = {Length}";
}
