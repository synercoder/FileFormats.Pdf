using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Generation.Internal;

internal class DictionaryWrapper : IDictionary<string, string>
{
    private readonly IDictionary<string, string> _dictionary = new Dictionary<string, string>();
    private readonly PdfWriter _pdfWriter;

    public DictionaryWrapper(PdfWriter pdfWriter)
    {
        _pdfWriter = pdfWriter;
    }

    public string this[string key]
    {
        get => _dictionary[key];
        set
        {
            _pdfWriter.ThrowsWhenEndingWritten();
            _dictionary[key] = value;
        }
    }

    public ICollection<string> Keys
        => _dictionary.Keys;

    public ICollection<string> Values
        => _dictionary.Values;

    public int Count
        => _dictionary.Count;

    public bool IsReadOnly
        => _dictionary.IsReadOnly;

    public void Add(string key, string value)
    {
        _pdfWriter.ThrowsWhenEndingWritten();
        _dictionary.Add(key, value);
    }

    public void Add(KeyValuePair<string, string> item)
    {
        _pdfWriter.ThrowsWhenEndingWritten();
        _dictionary.Add(item);
    }

    public void Clear()
    {
        _pdfWriter.ThrowsWhenEndingWritten();
        _dictionary.Clear();
    }

    public bool Contains(KeyValuePair<string, string> item)
    {
        return _dictionary.Contains(item);
    }

    public bool ContainsKey(string key)
    {
        return _dictionary.ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
    {
        _dictionary.CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        return _dictionary.GetEnumerator();
    }

    public bool Remove(string key)
    {
        _pdfWriter.ThrowsWhenEndingWritten();
        return _dictionary.Remove(key);
    }

    public bool Remove(KeyValuePair<string, string> item)
    {
        _pdfWriter.ThrowsWhenEndingWritten();
        return _dictionary.Remove(item);
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value)
    {
        return _dictionary.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ( (IEnumerable)_dictionary ).GetEnumerator();
    }
}
