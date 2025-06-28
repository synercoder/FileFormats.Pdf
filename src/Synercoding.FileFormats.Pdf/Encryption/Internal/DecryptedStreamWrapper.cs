using Synercoding.FileFormats.Pdf.Primitives;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Encryption.Internal;

internal class DecryptedStreamWrapper : IPdfStreamObject
{
    private readonly IPdfStreamObject _innerObject;

    public DecryptedStreamWrapper(IPdfStreamObject innerObject, byte[] decryptedBytes)
    {
        _innerObject = innerObject;
        RawData = decryptedBytes;
    }

    public IPdfPrimitive? this[PdfName key]
        => _innerObject[key];

    public byte[] RawData { get; }

    public long Length
        => _innerObject.Length;

    public ICollection<PdfName> Keys
        => _innerObject.Keys;

    public int Count
        => _innerObject.Count;

    public IPdfDictionary AsPureDictionary()
        => _innerObject.AsPureDictionary();

    public bool ContainsKey(PdfName key)
        => _innerObject.ContainsKey(key);

    public IEnumerator<KeyValuePair<PdfName, IPdfPrimitive>> GetEnumerator()
        => _innerObject.GetEnumerator();

    public bool TryGetValue(PdfName key, [NotNullWhen(true)] out IPdfPrimitive? value)
        => _innerObject.TryGetValue(key, out value);

    public bool TryGetValue<TPrimitive>(PdfName key, [NotNullWhen(true)] out TPrimitive? value)
        where TPrimitive : IPdfPrimitive
        => _innerObject.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator()
        => ( (IEnumerable)_innerObject ).GetEnumerator();
}
